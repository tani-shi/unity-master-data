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

using UnityEngine;
using UnityMasterDataDemo.MasterData.Collection;
using UnityMasterDataDemo.MasterData.DAO.Sample;
using UnityMasterDataDemo.MasterData.Type;
using UnityMasterDataDemo.MasterData.VO.Sample;

namespace UnityMasterDataDemo
{
    public class DemoScene : MonoBehaviour
    {
        private void Start ()
        {
            DemoMasterDataManager.Instance.LoadAsync (new MasterDataAccessorObjectCollection (), () =>
            {
                LogParameterByKey ();
                LogParametersByType ();
                LogAllParameters ();
            });
        }

        private void LogParameterByKey ()
        {
            Debug.Log ("START LOGGING: Parameter By Key");
            LogCharacterParameter (DemoMasterDataManager.Instance.Get<CharacterDAO> ().Get (101));
            Debug.Log ("END LOGGING");
        }

        private void LogParametersByType ()
        {
            Debug.Log ("START LOGGING: Parameters By Type");
            foreach (var data in DemoMasterDataManager.Instance.Get<CharacterDAO> ().FindAll (item => item.type == CharacterType.Dragon))
            {
                LogCharacterParameter (data);
            }
            Debug.Log ("END LOGGING");
        }

        private void LogAllParameters ()
        {
            Debug.Log ("START LOGGING: All Parameters");
            foreach (var data in DemoMasterDataManager.Instance.Get<CharacterDAO> ())
            {
                LogCharacterParameter (data);
            }
            Debug.Log ("END LOGGING");
        }

        private void LogCharacterParameter (CharacterVO data)
        {
            if (data != null)
            {
                Debug.Log (string.Format ("<color=cyan>id={0}, type={1}, assetName={2}, moveSpeed={3}, baseHp={4}, baseSp={5}, baseAkt={6}, baseDef={7}</color>",
                    data.id, data.type, data.assetName, data.moveSpeed, data.baseHp, data.baseSp, data.baseAtk, data.baseDef));
            }
        }
    }
}