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
using System.Collections;
using UnityEngine;

namespace UnityMasterData.Interfaces {

    /// <summary>
    /// Interface of data accessor object classes.
    /// No need to extend manually since this is extended by auto-generating.
    /// </summary>
    public interface IMasterDataAccessorObject {

        /// <summary>
        /// Get the data as a json.
        /// </summary>
        /// <returns>A serialized data as a json</returns>
        string ToJson ();

        /// <summary>
        /// Get the Addressables path of a specified master data asset.
        /// </summary>
        /// <returns>The path of data asset</returns>
        string GetAssetPath ();

        /// <summary>
        /// Get the data name. (That will be same as the excel sheet name.)
        /// </summary>
        /// <returns>The data name</returns>
        string GetName ();

        /// <summary>
        /// Set data as a DTO class to access for data values.
        /// </summary>
        /// <param name="data">A specified master data asset instance</param>
        void SetData (ScriptableObject data);
    }
}