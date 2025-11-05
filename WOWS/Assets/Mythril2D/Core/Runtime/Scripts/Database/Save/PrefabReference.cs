using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Save + nameof(PrefabReference))]
    public class PrefabReference : DatabaseEntry
    {
        [Header("References")]
        public GameObject prefab;
    }
}
