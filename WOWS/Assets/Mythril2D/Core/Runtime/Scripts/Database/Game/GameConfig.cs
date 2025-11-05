using UnityEngine;
using MackySoft.SerializeReferenceExtensions;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    [System.Flags]
    public enum EOptionalCharacterStatistics
    {
        None = 0,
        Mana = 1 << 0,
        MagicalAttack = 1 << 1,
        MagicalDefense = 1 << 2,
        Agility = 1 << 3,
        Luck = 1 << 4,
    }

    [System.Flags]
    public enum ECameraShakeSources
    {
        None = 0,
        PlayerReceiveDamage = 1 << 0,
        AnyCharacterReceiveDamageFromPlayer = 1 << 1
    }

    public enum EGameTerm
    {
        Currency,
        Level,
        Experience
    }

    [System.Serializable]
    public struct StatSettings
    {
        public string name;
        public string shortened;
        public string description;
        public Sprite icon;
        public bool hide;
    }

    [System.Serializable]
    public struct TermDefinition
    {
        public string fullName;
        public string shortName;
        public string description;
        public Sprite icon;
    }

    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Game + nameof(GameConfig))]
    public class GameConfig : DatabaseEntry
    {
        [Header("General Settings")]
        public DatabaseRegistry databaseRegistry = null;
        public string mainMenuSceneName = "Main Menu";

        [Header("Physics Settings")]
        public string interactionLayer = "Interaction";
        public string hitboxLayer = "Hitbox";
        [Min(0.0f)] public float maxTeleportDistanceWhenStuckInWall = 5.0f;
        public ContactFilter2D collisionContactFilter;
        public ContactFilter2D visibilityContactFilter;

        [Header("Playtest Settings")]
        public SaveFile playtestSaveFile = null;

        [Header("Visual Settings")]
        public ECameraShakeSources cameraShakeSources = ECameraShakeSources.None;

        [Header("Gameplay Settings")]
        public CraftingStation onTheGoCraftingStation = null;
        [SerializeReference, SubclassSelector] public ICommand toExecuteOnPlayerDeath = null;

        [Header("Combat Settings")]
        [Range(1, Constants.MaxEquipedAbilityCount)] public int maxEquippableAbilities = 5;
        public bool canCriticalHit = true;
        public bool canMissHit = true;
        public bool allowPushOnRegularHit = true;
        public bool allowPushOnCriticalHit = true;
        public bool allowPushOnMissedHit = true;
        public bool allowPushOnSilentHit = false;

        [Header("Save Settings")]
        public SerializableDictionary<string, string> persistentIdentifierMappings = new();

        [Header("UI Settings")]
        public AudioClipResolver navigationSelectSound = null;
        public AudioClipResolver pointerSelectSound = null;
        public AudioClipResolver submitSound = null;

        [Header("Game Terms")]
        [SerializeField] private SerializableDictionary<string, TermDefinition> m_gameTerms = new();

        [Header("Game Terms Bindings (Advanced Settings)")]
        [SerializeField] private SerializableDictionary<EStat, string> m_statTermsBinding = new();
        [SerializeField] private SerializableDictionary<EStat, string> m_statIncreaseTermsBinding = new();
        [SerializeField] private SerializableDictionary<EStat, string> m_statDecreaseTermsBinding = new();
        [SerializeField] private SerializableDictionary<EItemCategory, string> m_itemCategoryTermsBinding = new();
        [SerializeField] private SerializableDictionary<EDamageType, string> m_damageTypesBinding = new();
        [SerializeField] private SerializableDictionary<EControlType, string> m_controlTypesBinding = new();
        [SerializeField] private SerializableDictionary<EAbilityType, string> m_abilityTypesBinding = new();

        private TermDefinition m_defaultTermDefinition = new()
        {
            fullName = "[INVALID_FULLNAME]",
            shortName = "[INVALID_SHORTNAME]",
            description = "[INVALID_DESCRIPTION]",
            icon = null
        };

        public TermDefinition GetTermDefinition(string termID)
        {
            if (m_gameTerms.ContainsKey(termID))
            {
                return m_gameTerms[termID];
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetTermDefinition(EStat stat)
        {
            if (m_statTermsBinding.ContainsKey(stat))
            {
                return GetTermDefinition(m_statTermsBinding[stat]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetStatIncreaseTermDefinition(EStat stat)
        {
            if (m_statIncreaseTermsBinding.ContainsKey(stat))
            {
                return GetTermDefinition(m_statIncreaseTermsBinding[stat]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetStatDecreaseTermDefinition(EStat stat)
        {
            if (m_statDecreaseTermsBinding.ContainsKey(stat))
            {
                return GetTermDefinition(m_statDecreaseTermsBinding[stat]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetTermDefinition(EItemCategory category)
        {
            if (m_itemCategoryTermsBinding.ContainsKey(category))
            {
                return GetTermDefinition(m_itemCategoryTermsBinding[category]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetTermDefinition(EDamageType type)
        {
            if (m_damageTypesBinding.ContainsKey(type))
            {
                return GetTermDefinition(m_damageTypesBinding[type]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetTermDefinition(EControlType type)
        {
            if (m_controlTypesBinding.ContainsKey(type))
            {
                return GetTermDefinition(m_controlTypesBinding[type]);
            }

            return m_defaultTermDefinition;
        }

        public TermDefinition GetTermDefinition(EAbilityType abilityType)
        {
            if (m_abilityTypesBinding.ContainsKey(abilityType))
            {
                return GetTermDefinition(m_abilityTypesBinding[abilityType]);
            }

            return m_defaultTermDefinition;
        }
    }
}
