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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

// If error here, you need to install ExcelDataReader plugins in your project.
// See README. https://github.com/tani-shi/unity-master-data
using ExcelDataReader;
using UnityEngine;

namespace UnityMasterData.Libs {

    /// <summary>
    /// Provites to read and access a specified excel data file by the path.
    /// 
    /// *** Now, it is not supported to read binary files like xls files. ***
    /// </summary>
    public class Excel {

        /// <summary>
        /// Provides to access a specified excel sheet data.
        /// </summary>
        public struct Sheet {

            /// <summary>
            /// The name of an excel sheet.
            /// </summary>
            public string name;

            /// <summary>
            /// The array that contains all of cell data instance that an excel sheet has.
            /// </summary>
            public Cell[] cells;

            /// <summary>
            /// Get a specific sheet cell data instance by a specified row index and a specified column index.
            /// </summary>
            /// <param name="row">A specified row index</param>
            /// <param name="column">A specified column index</param>
            /// <returns>An instance of the specified cell if exists; otherwise null</returns>
            public Cell GetCell (int row, int column) {
                return cells.FirstOrDefault (c => c.row == row && c.column == column);
            }

            /// <summary>
            /// Get the specific sheet cells as an array by the specified row index.
            /// </summary>
            /// <param name="row">The specified row index</param>
            /// <returns>An array that has the specified cells if exists; otherwise an empty array.</returns>
            public Cell[] GetRowCells (int row) {
                return cells.Where (c => c.row == row).ToArray ();
            }

            /// <summary>
            /// Get the cells as an array by the specified column index.
            /// </summary>
            /// <param name="column">The specified column index</param>
            /// <returns>The cells as an array</returns>
            public Cell[] GetColumnCells (int column) {
                return cells.Where (c => c.column == column).ToArray ();
            }
        }

        /// <summary>
        /// Provides to access the specified cell data in the excel sheet.
        /// </summary>
        public struct Cell {

            /// <summary>
            /// The row index where the cell is in.
            /// </summary>
            public int row;

            /// <summary>
            /// The column index where the cell is in.
            /// </summary>
            public int column;

            /// <summary>
            /// The value as a string inside the cell.
            /// </summary>
            public string value;
        }

        /// <summary>
        /// The excel file name without a file extension
        /// </summary>
        public string name {
            get {
                return _name;
            }
        }

        /// <summary>
        /// The error details when failed reading. If null or empty, it was readed successfully.
        /// </summary>
        public string error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// The array of all of the excel sheet instances.
        /// </summary>
        public Sheet[] Sheets {
            get {
                return _sheets;
            }
        }

        private Sheet[] _sheets = null;
        private string _error = string.Empty;
        private string _name = string.Empty;

        /// <summary>
        /// If successfully to read the excel file, set a parsed instance to the instance you set,
        /// otherwise, set an empty instance(non-null) to the instance you set.
        /// </summary>
        /// <param name="path">The excel file path</param>
        /// <param name="excel">A instance of the destination to set a parsed instance</param>
        /// <returns>If true, successfully read the file; otherwise, false.</returns>
        public static bool TryRead (string path, out Excel excel) {
            excel = Read (path);
            return string.IsNullOrEmpty (excel._error);
        }

        /// <summary>
        /// Try to read the excel file to parse.
        /// If successfully to read, returns a parsed instance; otherwise returns an
        /// empty instance(non-null) that has a detail string of error.
        /// </summary>
        /// <param name="path">A path of the excel file</param>
        /// <returns>A parsed instance if successufully to read; otherwise an empty instance(non-null).</returns>
        public static Excel Read (string path) {
            var excel = new Excel ();
            excel._name = Path.GetFileNameWithoutExtension (path);
            try {
                using (var stream = File.Open (path, FileMode.Open, FileAccess.Read)) {
                    var reader = ExcelReaderFactory.CreateOpenXmlReader (stream);
                    if (reader != null) {
                        excel.ParseDataSet (reader.AsDataSet ());
                    }
                }
            } catch (Exception e) {
                Debug.LogError (e);
                excel._sheets = new Sheet[] { };
                excel._error = e.ToString ();
            }
            return excel;
        }

        /// <summary>
        /// Get an instance of the specified excel sheet by a name of the excel sheet.
        /// </summary>
        /// <param name="name">A name of the excel sheet</param>
        /// <returns>An instance of the specified excel sheet if exists; otherwise null.</returns>
        public Sheet GetSheet (string name) {
            return _sheets.FirstOrDefault (s => s.name == name);
        }

        private void ParseDataSet (DataSet dataSet) {
            var sheetList = new List<Sheet> ();
            foreach (DataTable table in dataSet.Tables) {
                var sheet = new Sheet ();
                sheet.name = table.TableName;
                var cellList = new List<Cell> ();
                for (int row = 0; row < table.Rows.Count; row++) {
                    for (int column = 0; column < table.Columns.Count; column++) {
                        var cell = new Cell ();
                        cell.row = row;
                        cell.column = column;
                        cell.value = table.Rows[row][column].ToString ();
                        cellList.Add (cell);
                    }
                }
                sheet.cells = cellList.ToArray ();
                sheetList.Add (sheet);
            }
            _sheets = sheetList.ToArray ();
        }
    }
}