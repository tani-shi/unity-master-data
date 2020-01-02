using System.Collections.Generic;

namespace UnityMasterData.Interfaces {

    /// <summary>
    /// An interface of a collection that has all of DAO instances.
    /// No need to extend manually, since this is extended by auto-generating.
    /// </summary>
    public interface IMasterDataAccessorObjectCollection : IEnumerable<IMasterDataAccessorObject> {
    }
}