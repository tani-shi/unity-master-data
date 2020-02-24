#if UNITY_EDITOR

namespace UnityMasterData.Editor.Interfaces
{
    /// <summary>
    /// An interface of a class to export master data assets as a ScriptableObject.
    /// No need to extend since this is extended auto-generating.
    /// </summary>
    public interface IMasterDataExporter
    {
        /// <summary>
        /// Do to export master data assets as a ScriptableObject in your project with a path
        /// that is specified by DAO classes. (See also MasterDataAccessorObject.cs)
        /// No need to extend manually, since this is extended by auto-generating.
        /// </summary>
        void Export ();
    }
}

#endif