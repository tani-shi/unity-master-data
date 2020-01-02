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

using System.Text.RegularExpressions;

namespace UnityMasterData.Libs.Extensions {

    /// <summary>
    /// Provides to extend 'System.String' with some of utility methods.
    /// </summary>
    public static class StringExtensions {

        /// <summary>
        /// Inject spaces at a start of line.
        /// </summary>
        /// <param name="count">The count of spaces</param>
        /// <returns>Indented string value</returns>
        public static string Indent (this string str, int count) {
            str = str.Trim ().ReplaceEOL ("\n");
            str = Regex.Replace (str, @"^", "".PadLeft (count), RegexOptions.Multiline);
            return str;
        }

        /// <summary>
        /// Replace all EOL characters to the specified EOL character.
        /// </summary>
        /// <param name="newValue">A specified EOL character</param>
        /// <returns>Replaced string value</returns>
        public static string ReplaceEOL (this string str, string newValue) {
            return str.Replace ("\r\n", newValue).Replace ("\r", newValue).Replace ("\n", newValue);
        }
    }
}