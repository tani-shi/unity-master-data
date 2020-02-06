// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using UnityEngine;
using UnityMasterData.Editor;
using UnityMasterData.Editor.Interfaces;

namespace UnityMasterDataDemo.MasterData.Editor.Exporter
{
    public class SampleExporter : IMasterDataExporter
    {
        public void Export ()
        {
            MasterDataExporter.Export<DTO.Sample.CharacterDTO, VO.Sample.CharacterVO, uint>("Assets/UnityMasterDataDemo/Excels/Sample.xlsx", "Assets/UnityMasterDataDemo/AddressableAssets/MasterData", "Sample", "Character");
        }
    }
}
