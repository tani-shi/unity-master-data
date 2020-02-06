// DON'T EDIT. THIS IS GENERATED AUTOMATICALLY.
using System;
using System.Collections;
using System.Collections.Generic;
using UnityMasterData;
using UnityMasterData.Interfaces;

namespace UnityMasterDataDemo.MasterData.Collection
{
    public class MasterDataAccessorObjectCollection : IMasterDataAccessorObjectCollection
    {
        private List<IMasterDataAccessorObject> _collection = new List<IMasterDataAccessorObject> ()
        {
            (Activator.CreateInstance(typeof(DAO.Sample.CharacterDAO)) as IMasterDataAccessorObject),
        };

        IEnumerator<IMasterDataAccessorObject> IEnumerable<IMasterDataAccessorObject>.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _collection.GetEnumerator ();
        }
    }
}
