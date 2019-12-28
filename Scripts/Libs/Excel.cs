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

// If error here, you have to install ExcelDataReader plugins in your project.
// See README. https://github.com/tani-shi/unity-master-data
using ExcelDataReader;
using UnityEngine;

namespace UnityMasterData.Libs {

    /// <summary>
    /// The tool to read excel files (including no binary files like xls files.)
    /// </summary>
    public class Excel {

        /// <summary>
        /// Excel sheet
        /// </summary>
        public struct Sheet {

            /// <summary>
            /// Sheet name
            /// </summary>
            public string name;

            /// <summary>
            /// The all cells the sheet has.
            /// </summary>
            public Cell[] cells;

            /// <summary>
            /// Get a specific sheet cell by row and column.
            /// </summary>
            /// <param name="row">Row index</param>
            /// <param name="column">Column index</param>
            /// <returns>Sheet cell instance</returns>
            public Cell GetCell (int row, int column) {
                return cells.FirstOrDefault (c => c.row == row && c.column == column);
            }

            /// <summary>
            /// Get specific sheet cells by row index.
            /// </summary>
            /// <param name="row">Row index</param>
            /// <returns>Array of sheet cell</returns>
            public Cell[] GetRowCells (int row) {
                return cells.Where (c => c.row == row).ToArray ();
            }

            public Cell[] GetColumnCells (int column) {
                return cells.Where (c => c.column == column).ToArray ();
            }
        }

        /// <summary>
        /// Sheet cell data
        /// </summary>
        public struct Cell {

            /// <summary>
            /// Row index
            /// </summary>
            public int row;

            /// <summary>
            /// Column index
            /// </summary>
            public int column;

            /// <summary>
            /// The value inside the cell
            /// </summary>
            public string value;
        }

        /// <summary>
        /// The excel file name without file extension
        /// </summary>
        public string name {
            get {
                return _name;
            }
        }

        /// <summary>
        /// The error details that failed reading. If null or empty, successfully readed.
        /// </summary>
        public string error {
            get {
                return _error;
            }
        }

        /// <summary>
        /// The all sheets the excel file has.
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
        /// Try to read the xlsx file, then output to the instance you set.
        /// </summary>
        /// <param name="path">The xlsx file path</param>
        /// <param name="excel">The output destination instance</param>
        /// <returns>If true, successfully read the file.</returns>
        public static bool TryRead (string path, out Excel excel) {
            excel = Read (path);
            return string.IsNullOrEmpty (excel._error);
        }

        /// <summary>
        /// Read the xlsx file
        /// </summary>
        /// <param name="path">The xlsx file path</param>
        /// <returns>The parsed instance.</returns>
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
        /// Get a specific sheet by sheet name.
        /// </summary>
        /// <param name="name">Sheet name</param>
        /// <returns>Sheet</returns>
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