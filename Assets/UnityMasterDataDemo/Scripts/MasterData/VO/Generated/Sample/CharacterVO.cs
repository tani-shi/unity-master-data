// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using UnityEngine;
using UnityMasterData.Interfaces;
using UnityMasterDataDemo.MasterData.Type;

namespace UnityMasterDataDemo.MasterData.VO.Sample {
    [SerializableAttribute]
    public partial class CharacterVO : IValueObject<uint> {
        public uint id;
        public CharacterType type;
        public string assetName;
        public float moveSpeed;
        public uint baseHp;
        public uint baseSp;
        public uint baseAtk;
        public uint baseDef;

        public uint GetKey() {
            return id;
        }
    }
}
