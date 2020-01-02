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

namespace UnityMasterData.Interfaces {

    /// <summary>
    /// An interfaces of value object classes that is contained as a list element in ScriptableObject.
    /// No need to extend manually, since this is extended by auto-generating.
    /// </summary>
    /// <typeparam name="K">The primary key type (Commonly, this will be 'int' or 'uint'.)</typeparam>
    public interface IValueObject<K> {

        /// <summary>
        /// Get the primary key value.
        /// </summary>
        /// <returns>The primary key value</returns>
        K GetKey ();
    }
}