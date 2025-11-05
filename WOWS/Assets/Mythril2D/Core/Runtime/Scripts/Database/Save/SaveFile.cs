using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Save + nameof(SaveFile))]
    public class SaveFile : DatabaseEntry
    {
        [SerializeField] private SaveDataBlock m_content;

        public SaveDataBlock content => m_content;
    }
}
