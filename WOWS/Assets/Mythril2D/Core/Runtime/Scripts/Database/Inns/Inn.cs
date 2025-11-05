using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Inns + nameof(Inn))]
    public class Inn : DatabaseEntry
    {
        public int price;
        public int healAmount;
        public int manaRecoveredAmount;
        public AudioClipResolver healingSound;
    }
}