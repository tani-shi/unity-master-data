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
using System.Collections.Generic;
using UnityEngine;
using UnityMasterData.Interfaces;

namespace UnityMasterData
{
    /// <summary>
    /// The DAO base class that can be accessible value objects from data assets(DTO).
    /// Basically no need extending manually, since auto-generating.
    /// </summary>
    /// <typeparam name="DAO">DAO type</typeparam>
    /// <typeparam name="DTO">DTO type</typeparam>
    /// <typeparam name="VO">VO type</typeparam>
    /// <typeparam name="K">Primary key type</typeparam>
    public abstract class MasterDataAccessorObject<DAO, DTO, VO, K> : IMasterDataAccessorObject, IEnumerable<VO> where DAO : MasterDataAccessorObject<DAO, DTO, VO, K>, new () where DTO : MasterDataTransferObject<VO, K>, IEnumerable<VO> where VO : class, IValueObject<K>, new ()
    {
        private DTO _dto;
        private Dictionary<K, VO> _dictionary = new Dictionary<K, VO> ();

        /// <summary>
        /// Get the data as a json.
        /// </summary>
        /// <returns>A serialized data as a json</returns>
        public string ToJson ()
        {
            var json = "{\"" + GetName () + "\":[";
            for (int i = 0; i < _dto.list.Count; i++)
            {
                json += JsonUtility.ToJson (_dto.list[i]);
                if (i < _dto.list.Count - 1)
                {
                    json += ",";
                }
            }
            return json + "]}";
        }

        /// <summary>
        /// Get the Addressables path of a specified master data asset.
        /// </summary>
        /// <returns>The path of data asset</returns>
        public abstract string GetAssetPath ();

        /// <summary>
        /// Get the data name. (That will be same as the excel sheet name.)
        /// </summary>
        /// <returns>The data name</returns>
        public abstract string GetName ();

        /// <summary>
        /// Set data as a DTO class to access for data values.
        /// </summary>
        /// <param name="data">A specified master data asset instance</param>
        public void SetData (ScriptableObject data)
        {
            _dto = data as DTO;
            _dictionary.Clear ();
            foreach (var e in _dto)
            {
                var key = e.GetKey ();
                if (_dictionary.ContainsKey (key))
                {
                    Debug.LogWarning ("duplicated key: [" + key + "] in VO:[" + typeof (DTO).Name + "].");
                }
                _dictionary[key] = e;
            }
        }

        /// <summary>
        /// Get the data element by a specified primary key value.
        /// </summary>
        /// <param name="key">The primary key value</param>
        /// <returns>An instance of VO that has the specified primary key value if exists; otherwise null</returns>
        public VO Get (K key)
        {
            VO element;
            if (_dictionary.TryGetValue (key, out element))
            {
                return element;
            }
            Debug.LogError (string.Format ("attempted to get non-existed object in {0}. key={1}", typeof (DTO).Name, key.ToString ()));
            return null;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Get the data element by a specified primary key value without logging if not exists.
        /// </summary>
        /// <param name="key">The primary key value</param>
        /// <returns>An instance of VO that has the specified primary key value if exists; otherwise null</returns>
        public VO GetSilently (K key)
        {
            VO element;
            if (_dictionary.TryGetValue (key, out element))
            {
                return element;
            }
            return null;
        }
#endif

        /// <summary>
        /// Whether exists any items that has specific primary key value.
        /// </summary>
        /// <param name="key">The primary key value</param>
        /// <returns>If true, exists any items in the data assets; otherwise false</returns>
        public bool Contains (K key)
        {
            return Exists (item => item.GetKey ().Equals (key));
        }

        /// <summary>
        /// Determines whether an element is in the data assets.
        /// </summary>
        /// <param name="item">The object to locate</param>
        /// <returns>If true, item is found in the data assets</returns>
        public bool Contains (VO item)
        {
            return _dto.list.Contains (item);
        }

        /// <summary>
        /// Determines whether the data assets contains elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The delegate that defines the conditions of the elements to search for</param>
        /// <returns>
        /// If true the data assets contains one or more elements that match the conditions
        /// defined by the specified predicate
        /// </returns>
        public bool Exists (Predicate<VO> match)
        {
            return _dto.list.Exists (match);
        }

        /// <summary>
        /// Searches for an element that matches the conditions defined by the specified predicate,
        /// and returns the first occurrence within the entire data assets.
        /// </summary>
        /// <param name="match">The delegate that defines the conditions of the elements to search for.</param>
        /// <returns>The first element that matches the conditions defined by the specified predicate, if found; otherwise, null</returns>
        public VO Find (Predicate<VO> match)
        {
            return _dto.list.Find (match);
        }

        /// <summary>
        /// Retrieves all the elements that match the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="match">The delegate that defines the conditions of the elements to search for.</param>
        /// <returns>
        /// A data assets containing all the elements that match the conditions defined by
        /// the specified predicate, if found; otherwise, an empty list.
        /// </returns>
        public List<VO> FindAll (Predicate<VO> match)
        {
            return _dto.list.FindAll (match);
        }

        /// <summary>
        /// Performs the specified action on each element of the data assets.
        /// </summary>
        /// <param name="action">The delegate to perform on each element of the data assets</param>
        public void ForEach (Action<VO> action)
        {
            _dto.list.ForEach (action);
        }

        IEnumerator<VO> IEnumerable<VO>.GetEnumerator ()
        {
            return _dto.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _dto.GetEnumerator ();
        }
    }
}