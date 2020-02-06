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

using UnityEditor;
using UnityMasterData.Editor;

namespace UnityMasterDataDemo.Editor
{
    public static class DemoMenuItems
    {
        [MenuItem ("UnityMasterDataDemo/Generate Master Data Class Scripts")]
        private static void GenerateMasterDataClassScripts ()
        {
            MasterDataClassGenerator.GenerateAllDataScripts (
                "Assets/UnityMasterDataDemo/Excels",
                "Assets/UnityMasterDataDemo/Scripts",
                "Assets/UnityMasterDataDemo/AddressableAssets",
                "UnityMasterDataDemo");
        }

        [MenuItem ("UnityMasterDataDemo/Export Master Data Assets")]
        private static void ExportMasterDataClass ()
        {
            MasterDataExporter.Export ();
        }
    }
}