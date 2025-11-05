using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Characters + nameof(MonsterSheet))]
    public class MonsterSheet : CharacterSheet
    {
        [Header("Monster")]
        public LevelScaledStats stats = new();

        [Header("Rewards")]
        public LevelScaledInteger experience = new();
        public LevelScaledInteger money = new();
        public Loot[] potentialLoot;

        [Header("Commands")]
        [SerializeReference, SubclassSelector]
        public ICommand executeOnDeath;

        public MonsterSheet() : base(EAlignment.Evil) { }
    }
}
