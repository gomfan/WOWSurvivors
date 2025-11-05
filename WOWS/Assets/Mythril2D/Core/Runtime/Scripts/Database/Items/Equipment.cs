using UnityEngine;
using UnityEngine.U2D.Animation;

namespace Gyvr.Mythril2D
{
    public enum EEquipmentType
    {
        Weapon,
        Head,
        Torso,
        Hands,
        Feet
    }

    public enum EOperationType
    {
        Equip,
        Unequip
    }

    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Items + nameof(Equipment))]
    public class Equipment : Item
    {
        [Header("Equipment")]
        [SerializeField] private EEquipmentType m_type;
        [SerializeField] private Stats m_bonusStats;
        [SerializeField] private SpriteLibraryAsset m_visualOverride;
        [SerializeField] private AbilitySheet[] m_bonusAbilities;

        public EEquipmentType type => m_type;
        public Stats bonusStats => m_bonusStats;
        public SpriteLibraryAsset visualOverride => m_visualOverride;
        public AbilitySheet[] bonusAbilities => m_bonusAbilities;
    }
}
