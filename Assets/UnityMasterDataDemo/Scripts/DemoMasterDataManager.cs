// Copyright 2020 Shintaro Tanikawa
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
using UnityEngine;

// If error here, you need to install The Addressables Assets System package by Package Manager.
// See README. https://github.com/tani-shi/unity-master-data
using UnityEngine.AddressableAssets;
using UnityMasterData;
using UnityMasterData.Interfaces;

namespace UnityMasterDataDemo {
    public class DemoMasterDataManager : MasterDataManagerBase<DemoMasterDataManager> {
        protected override IEnumerator LoadAsyncProc (IMasterDataAccessorObject dao) {
            Debug.Log ("LOAD: " + dao.GetName ());
            var handle = Addressables.LoadAssetAsync<ScriptableObject> (dao.GetAssetPath ());
            yield return handle;
            if (handle.Result != null) {
                dao.SetData (Instantiate (handle.Result));
                Debug.Log (dao.ToJson ());
            } else {
                Debug.LogError ("Failed to load a master data asset. " + dao.GetAssetPath ());
            }
        }

        protected override void LoadProc (IMasterDataAccessorObject dao) {
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                var data = UnityEditor.AssetDatabase.LoadAssetAtPath<ScriptableObject> (dao.GetAssetPath ());
                if (data != null) {
                    dao.SetData (data);
                } else {
                    Debug.LogError ("Failed to load a master data asset. " + dao.GetAssetPath ());
                }
            } else
#endif
            {
                Debug.LogWarning ("It is not supported to load on a frame for except Editor Mode.");
            }
        }
    }
}