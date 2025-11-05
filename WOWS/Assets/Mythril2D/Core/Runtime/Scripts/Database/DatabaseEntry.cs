using UnityEngine;

namespace Gyvr.Mythril2D
{
#if UNITY_EDITOR
    public partial class DatabaseEntry
    {
        public string GetAssetGUID()
        {
            string assetPath = UnityEditor.AssetDatabase.GetAssetPath(this);
            return UnityEditor.AssetDatabase.AssetPathToGUID(assetPath);
        }
    }
#endif

    public partial class DatabaseEntry : ScriptableObject { }
}
