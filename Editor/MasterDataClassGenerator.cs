// Copyright 2019 Shintaro Tanikawa
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_EDITOR

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityExcel;

namespace UnityMasterData.Editor
{
    /// <summary>
    /// The tool to generate data class scripts directly in your project.
    /// We support just excel files(not included binary files, like xls.) that have below data lines.
    /// 
    ///     A           | B ~
    /// ------------------------------
    /// 1: key name     | field name   // Must be non-empty
    /// ------------------------------
    /// 2: any comments | any comments // Nullable
    /// ------------------------------
    /// 3:              | enum define  // Nullable
    /// ------------------------------
    /// 4: key type     | field type   // Must be non-empty
    /// ------------------------------
    /// 5~: key value   | value        // Nullable if string, otherwise must be non-empty
    /// ------------------------------
    /// 
    /// When you invoke GenerateAllDataScripts, then generate automatically scripts as below
    /// 
    /// - ${ROOT_PATH}/DAO/Generated/${EXCEL_NAME_WITHOUT_EXT}/${SHEET_NAME}DAO.cs
    /// - ${ROOT_PATH}/DTO/Generated/${EXCEL_NAME_WITHOUT_EXT}/${SHEET_NAME}DTO.cs
    /// - ${ROOT_PATH}/VO/Generated/${EXCEL_NAME_WITHOUT_EXT}/${SHEET_NAME}VO.cs
    /// - ${ROOT_PATH}/Type/Generated/MasterDataType.cs
    /// - ${ROOT_PATH}/Collection/Generated/MasterDataAccessorObjectCollection.cs
    /// - ${ROOT_PATH}/Editor/Exporter/Generated/${EXCEL_NAME_WITOUT_EXT}Exporter.cs
    /// </summary>
    public static class MasterDataClassGenerator
    {
        /// <summary>
        /// The definitions each row meanings.
        /// </summary>
        public enum RowSettings
        {
            /// <summary>
            /// The row index that VO's field names are contained.
            /// </summary>
            KeyName = 0,

            /// <summary>
            /// The row index that data comments are contained.
            /// </summary>
            Comment = 1,

            /// <summary>
            /// The row index that data enum definitions are contained.
            /// </summary>
            EnumDefine = 2,

            /// <summary>
            /// The row index that VO's field types are contained.
            /// </summary>
            Type = 3,

            /// <summary>
            /// The row index that data assets values are contained.
            /// </summary>
            Value = 4,
        }

        /// <summary>
        /// The definitions each column meanings.
        /// </summary>
        public enum ColumnSettings
        {

            /// <summary>
            /// The column index that VO's key values are contained.
            /// </summary>
            Key = 0,
        }

        private const string kVoTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData.Interfaces;
using ${BASE_NAMESPACE}.Type;

namespace ${BASE_NAMESPACE}.VO.${BASE_NAME}
{
    [SerializableAttribute]
    public partial class ${NAME}VO : IValueObject<${KEY_TYPE}>
    {
${FIELDS}

        public ${KEY_TYPE} GetKey ()
        {
            return ${KEY_NAME};
        }
    }
}
";
        private const string kDtoTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData;

namespace ${BASE_NAMESPACE}.DTO.${BASE_NAME}
{
    public partial class ${NAME}DTO : MasterDataTransferObject<VO.${BASE_NAME}.${NAME}VO, ${KEY_TYPE}>
    {
    }
}
";
        private const string kDaoTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using UnityEngine;
using UnityMasterData;

namespace ${BASE_NAMESPACE}.DAO.${BASE_NAME}
{
    public partial class ${NAME}DAO : MasterDataAccessorObject<${NAME}DAO, DTO.${BASE_NAME}.${NAME}DTO, VO.${BASE_NAME}.${NAME}VO, ${KEY_TYPE}>
    {
        public override string GetAssetPath ()
        {
            return " + "\"${ASSET_DEST_BASE_PATH}/${BASE_NAME}/${NAME}.asset\"" + @";
        }

        public override string GetName ()
        {
            return " + "\"${NAME}\"" + @";
        }
    }
}
";
        private const string kTypeTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.

namespace ${BASE_NAMESPACE}.Type
{
${TYPE_DEFINE}
}";
        private const string kExporterTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using UnityEngine;
using UnityMasterData.Editor;
using UnityMasterData.Editor.Interfaces;

namespace ${BASE_NAMESPACE}.Editor.Exporter
{
    public class ${BASE_NAME}Exporter : IMasterDataExporter
    {
        public void Export ()
        {
${EXPORT_DATA}
        }
    }
}
";
        private const string kCollectionTemplate = @"// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityMasterData;
using UnityMasterData.Interfaces;

namespace ${BASE_NAMESPACE}.Collection
{
    public class MasterDataAccessorObjectCollection : IMasterDataAccessorObjectCollection
    {
        private List<IMasterDataAccessorObject> _collection = new List<IMasterDataAccessorObject> ()
        {
${DATA_ACCESSOR_OBJECTS}
        };

        IEnumerator<IMasterDataAccessorObject> IEnumerable<IMasterDataAccessorObject>.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }
    }
}
";
        private const string kMasterDataRootDirectoryName = "MasterData";

        /// <summary>
        /// Generate data class scripts directly in your project.
        /// Even if you not have the root directory or some necessary directories, we are going to create that.
        /// This method just do to generate/update data class scripts, so you need to delete scripts to clean your project, if necessary.
        /// </summary>
        /// <param name="sourcePath">The path contains excel files</param>
        /// <param name="destPath">The root path of destination to generate data class scripts</param>
        /// <param name="assetDestPath">The root path of destination to generate data assets</param>
        /// <param name="projectCode">Your project code</param>
        public static void GenerateAllDataScripts (string sourcePath, string destPath, string assetDestPath, string projectCode = null)
        {
            destPath = Path.Combine (destPath, kMasterDataRootDirectoryName);
            assetDestPath = Path.Combine (assetDestPath, kMasterDataRootDirectoryName);

            var excelMap = new Dictionary<string, Excel> ();
            foreach (var xlsx in Directory.GetFiles (sourcePath, "*.xlsx"))
            {
                if (Path.GetFileName (xlsx).StartsWith ("~$"))
                {
                    continue;
                }
                Excel excel;
                if (!Excel.TryRead (xlsx, out excel))
                {
                    return;
                }

                GenerateDTOScripts (projectCode, destPath, excel);
                GenerateVOScripts (projectCode, destPath, excel);
                GenerateDAOScripts (projectCode, destPath, assetDestPath, excel);
                GenerateTypeScript (projectCode, destPath, excel);
                GenerateExporterScript (projectCode, destPath, assetDestPath, xlsx, excel);

                excelMap.Add (xlsx, excel);
            }

            GenerateCollectionScript (projectCode, destPath, excelMap);

            AssetDatabase.Refresh ();
        }

        private static void GenerateDTOScripts (string projectCode, string destPath, Excel excel)
        {
            foreach (var sheet in excel.Sheets)
            {
                var keyType = sheet.GetCell ((int) RowSettings.Type, (int) ColumnSettings.Key).value;
                var keyName = sheet.GetCell ((int) RowSettings.KeyName, (int) ColumnSettings.Key).value;
                GenerateDTOScript (projectCode, destPath, excel.name, keyType, keyName, sheet);
            }
        }

        private static void GenerateVOScripts (string projectCode, string destPath, Excel excel)
        {
            foreach (var sheet in excel.Sheets)
            {
                GenerateVOScript (projectCode, destPath, excel.name, sheet);
            }
        }

        private static void GenerateDAOScripts (string projectCode, string destPath, string assetDestPath, Excel excel)
        {
            foreach (var sheet in excel.Sheets)
            {
                var keyType = sheet.GetCell ((int) RowSettings.Type, (int) ColumnSettings.Key).value;
                GenerateDAOScript (projectCode, destPath, assetDestPath, excel.name, keyType, sheet);
            }
        }

        private static void GenerateDTOScript (string projectCode, string destPath, string baseName, string keyType, string keyName, Excel.Sheet sheet)
        {
            var path = DTOScriptPath (destPath, baseName, sheet.name);
            var content = kDtoTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${BASE_NAME}", baseName)
                .Replace ("${NAME}", sheet.name)
                .Replace ("${KEY_TYPE}", keyType)
                .Replace ("${KEY_NAME}", keyName)
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateVOScript (string projectCode, string destPath, string baseName, Excel.Sheet sheet)
        {
            var defineBuilder = new StringBuilder ();
            var fieldBuilder = new StringBuilder ();
            var nameCells = sheet.GetRowCells ((int) RowSettings.KeyName);
            var defineCells = sheet.GetRowCells ((int) RowSettings.EnumDefine);
            var typeCells = sheet.GetRowCells ((int) RowSettings.Type);
            for (int i = 0; i < nameCells.Length; i++)
            {
                if (string.IsNullOrEmpty (defineCells[i].value))
                {
                    continue;
                }
                defineBuilder.AppendLine ("public enum " + typeCells[i].value + " {");
                defineBuilder.AppendLine (defineCells[i].value.Indent (4));
                defineBuilder.AppendLine ("}");
            }
            for (int i = 0; i < nameCells.Length; i++)
            {
                fieldBuilder.AppendLine (string.Format ("public {0} {1};", typeCells[i].value, nameCells[i].value));
            }
            var path = VOScriptPath (destPath, baseName, sheet.name);
            var content = kVoTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${BASE_NAME}", baseName)
                .Replace ("${NAME}", sheet.name)
                .Replace ("${FIELDS}", fieldBuilder.ToString ().Indent (8))
                .Replace ("${KEY_TYPE}", typeCells[(int) ColumnSettings.Key].value)
                .Replace ("${KEY_NAME}", nameCells[(int) ColumnSettings.Key].value)
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateDAOScript (string projectCode, string destPath, string assetDestPath, string baseName, string keyType, Excel.Sheet sheet)
        {
            var path = DAOScriptPath (destPath, baseName, sheet.name);
            var content = kDaoTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${BASE_NAME}", baseName)
                .Replace ("${NAME}", sheet.name)
                .Replace ("${KEY_TYPE}", keyType)
                .Replace ("${ASSET_DEST_BASE_PATH}", assetDestPath)
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateTypeScript (string projectCode, string destPath, Excel excel)
        {
            var builder = new StringBuilder ();
            foreach (var sheet in excel.Sheets)
            {
                var nameCells = sheet.GetRowCells ((int) RowSettings.KeyName);
                var typeCells = sheet.GetRowCells ((int) RowSettings.Type);
                var defineCells = sheet.GetRowCells ((int) RowSettings.EnumDefine);
                for (int i = 0; i < nameCells.Length; i++)
                {
                    if (string.IsNullOrEmpty (defineCells[i].value))
                    {
                        continue;
                    }
                    builder.AppendLine ("public enum " + typeCells[i].value + " {");
                    builder.AppendLine (defineCells[i].value.Indent (4));
                    builder.AppendLine ("}");
                }
            }
            var path = TypeScriptPath (destPath);
            var content = kTypeTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${TYPE_DEFINE}", builder.ToString ().Indent (4))
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateExporterScript (string projectCode, string destPath, string assetDestPath, string xlsxPath, Excel excel)
        {
            var builder = new StringBuilder ();
            foreach (var sheet in excel.Sheets)
            {
                var keyType = sheet.GetCell ((int) RowSettings.Type, (int) ColumnSettings.Key).value;
                builder.AppendLine (string.Format ("MasterDataExporter.Export<DTO.{0}.{1}DTO, VO.{0}.{1}VO, {2}>(\"{3}\", \"{4}\", \"{0}\", \"{1}\");",
                    excel.name,
                    sheet.name,
                    keyType,
                    xlsxPath,
                    assetDestPath));
            }
            var path = ExporterScriptPath (destPath, excel.name);
            var content = kExporterTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${BASE_NAME}", excel.name)
                .Replace ("${EXPORT_DATA}", builder.ToString ().Indent (12))
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateCollectionScript (string projectCode, string destPath, Dictionary<string, Excel> excelMap)
        {
            var builder = new StringBuilder ();
            foreach (var kv in excelMap)
            {
                foreach (var sheet in kv.Value.Sheets)
                {
                    builder.AppendLine (string.Format ("(Activator.CreateInstance(typeof(DAO.{0}.{1}DAO)) as IMasterDataAccessorObject),", kv.Value.name, sheet.name));
                }
            }
            var path = CollectionScriptPath (destPath);
            var content = kCollectionTemplate
                .Replace ("${BASE_NAMESPACE}", BaseNamespace (projectCode, destPath))
                .Replace ("${DATA_ACCESSOR_OBJECTS}", builder.ToString ().Indent (12))
                .ReplaceEOL ("\n");
            GenerateScript (path, content);
        }

        private static void GenerateScript (string path, string content)
        {
            if (File.Exists (path))
            {
                if (content != File.ReadAllText (path))
                {
                    Debug.Log ("UPDATE: " + path);
                }
                else
                {
                    return;
                }
            }
            else
            {
                Debug.Log ("GENERATE: " + path);
            }
            Directory.CreateDirectory (Path.GetDirectoryName (path));
            File.WriteAllText (path, content);
        }

        private static string BaseNamespace (string projectCode, string destPath)
        {
            var path = destPath;
            if (destPath.Contains ("Scripts/"))
            {
                path = destPath.Substring (destPath.LastIndexOf ("Scripts/") + "Scripts/".Count ());
            }
            if (!string.IsNullOrEmpty (projectCode))
            {
                path = projectCode + "/" + path;
            }
            return path.Replace ("/", ".");
        }

        private static string DTOScriptPath (string destPath, string baseName, string name)
        {
            return Path.Combine (destPath, string.Format ("DTO/Generated/{0}/{1}DTO.cs", baseName, name));
        }

        private static string VOScriptPath (string destPath, string baseName, string name)
        {
            return Path.Combine (destPath, string.Format ("VO/Generated/{0}/{1}VO.cs", baseName, name));
        }

        private static string DAOScriptPath (string destPath, string baseName, string name)
        {
            return Path.Combine (destPath, string.Format ("DAO/Generated/{0}/{1}DAO.cs", baseName, name));
        }

        private static string TypeScriptPath (string destPath)
        {
            return Path.Combine (destPath, "Type/Generated/MasterDataType.cs");
        }

        private static string ExporterScriptPath (string destPath, string baseName)
        {
            return Path.Combine (destPath, string.Format ("Editor/Exporter/Generated/{0}Exporter.cs", baseName));
        }

        private static string CollectionScriptPath (string destPath)
        {
            return Path.Combine (destPath, "Collection/Generated/MasterDataAccessorObjectCollection.cs");
        }

        private static string Indent (this string str, int count)
        {
            str = str.Trim ().ReplaceEOL ("\n");
            str = Regex.Replace (str, @"^", "".PadLeft (count), RegexOptions.Multiline);
            return str;
        }

        private static string ReplaceEOL (this string str, string newValue)
        {
            return str.Replace ("\r\n", newValue).Replace ("\r", newValue).Replace ("\n", newValue);
        }
    }
}

#endif