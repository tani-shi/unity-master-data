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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityMasterData.Interfaces;

namespace UnityMasterData
{
    /// <summary>
    /// Provides to transfer master data assets to a readable data for the DAO instance.
    /// No need to extend manually, since this is extended by auto-generating.
    /// </summary>
    /// <typeparam name="T">ValueObject type</typeparam>
    /// <typeparam name="K">Primary key type</typeparam>
    public abstract class MasterDataTransferObject<T, K> : ScriptableObject, IEnumerable<T> where T : IValueObject<K>
    {
        public List<T> list = new List<T> ();

        IEnumerator<T> IEnumerable<T>.GetEnumerator ()
        {
            return list.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return list.GetEnumerator ();
        }
    }
}