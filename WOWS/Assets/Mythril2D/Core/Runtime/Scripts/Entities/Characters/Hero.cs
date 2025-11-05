using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    public enum EEquipmentOperationResult
    {
        Valid,
        NotEnoughHealth,
        NotEnoughMana,
    }

    [Serializable]
    public class HeroDataBlock : CharacterBaseDataBlock
    {
        public int usedPoints;
        public int experience;
        public Stats customStats;
        public DatabaseEntryReference<Equipment>[] equipments;
        public DatabaseEntryReference<ActiveAbilitySheet>[] equippedAbilities;
    }

    public class Hero : Character<HeroSheet>
    {
        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_levelUpSound;

        public int experience => m_experience;
        public int nextLevelExperience => GetTotalExpRequirement(m_level + 1);
        public int availablePoints => m_sheet.pointsPerLevel * (m_level - Constants.MinLevel) - m_usedPoints;
        public SerializableDictionary<EEquipmentType, Equipment> equipments => m_equipments;
        public ActiveAbilitySheet[] equippedAbilities => m_equippedAbilities;
        public UnityEvent<ActiveAbilitySheet[]> equippedAbilitiesChanged => m_equippedAbilitiesChanged;
        public Stats customStats => m_customStats;
        public int usedPoints => m_usedPoints;

        private Stats m_customStats = new();
        private int m_usedPoints = 0;
        private int m_experience = 0;
        private SerializableDictionary<EEquipmentType, Equipment> m_equipments = new();
        private ActiveAbilitySheet[] m_equippedAbilities = new ActiveAbilitySheet[Constants.MaxEquipedAbilityCount];
        private UnityEvent<ActiveAbilitySheet[]> m_equippedAbilitiesChanged = new();

        public override void Revive()
        {
            base.Revive();
            m_animationStrategy?.Resume();
        }

        public int GetTotalExpRequirement(int level)
        {
            int total = 0;

            for (int i = 1; i < level; i++)
            {
                total += m_sheet.experience[i];
            }

            return total;
        }

        public void AddExperience(int experience, bool silentMode = false)
        {
            Debug.Assert(experience > 0, "Cannot add a negative amount of experience.");

            GameManager.NotificationSystem.experienceGained.Invoke(experience);

            m_experience += experience;

            while (m_experience >= GetTotalExpRequirement(m_level + 1))
            {
                LevelUp(silentMode);
            }
        }

        public void AddCustomStats(Stats customStats)
        {
            m_customStats += customStats;
            UpdateStats();
        }

        public void LogUsedPoints(int points)
        {
            m_usedPoints += points;
        }

        protected override void InitializeStats()
        {
            UpdateStats();
        }

        private EEquipmentOperationResult CanEquip(EEquipmentType type, Equipment equipment)
        {
            var tempEquipement = new Dictionary<EEquipmentType, Equipment>(m_equipments);
            tempEquipement[type] = equipment;

            Stats potentialEquipmentStats = CalculateEquipmentStats(tempEquipement.Values);
            Stats actualEquipmentStats = CalculateEquipmentStats(m_equipments.Values);
            Stats diff = potentialEquipmentStats - actualEquipmentStats;
            Stats potentialNewCurrentStats = m_currentStats.values + diff;

            if (potentialNewCurrentStats[EStat.Health] <= 0)
            {
                return EEquipmentOperationResult.NotEnoughHealth;
            }

            if (potentialNewCurrentStats[EStat.Mana] < 0)
            {
                return EEquipmentOperationResult.NotEnoughMana;
            }

            return EEquipmentOperationResult.Valid;
        }

        private EEquipmentOperationResult CanUnequip(EEquipmentType type) => CanEquip(type, null);

        public EEquipmentOperationResult TryEquip(Equipment equipment, out Equipment previousEquipment)
        {
            EEquipmentOperationResult result = CanEquip(equipment.type, equipment);

            previousEquipment =
                result == EEquipmentOperationResult.Valid ?
                Equip(equipment) :
                null;

            return result;
        }

        public EEquipmentOperationResult TryUnequip(EEquipmentType type, out Equipment previousEquipment)
        {
            EEquipmentOperationResult result = CanUnequip(type);

            previousEquipment =
                result == EEquipmentOperationResult.Valid ?
                Unequip(type) :
                null;

            return result;
        }

        protected Equipment Equip(Equipment equipment, bool autoUpdateStats = true)
        {
            Equipment previousEquipment = Unequip(equipment.type);
            m_equipments[equipment.type] = equipment;

            foreach (AbilitySheet ability in equipment.bonusAbilities)
            {
                AddBonusAbility(ability);
            }

            GameManager.NotificationSystem.itemEquipped.Invoke(equipment);

            if (autoUpdateStats)
            {
                UpdateStats();
            }

            return previousEquipment;
        }

        protected Equipment Unequip(EEquipmentType type, bool autoUpdateStats = true)
        {
            m_equipments.TryGetValue(type, out Equipment toUnequip);

            if (toUnequip)
            {
                foreach (AbilitySheet ability in toUnequip.bonusAbilities)
                {
                    RemoveBonusAbility(ability);
                }

                m_equipments.Remove(type);
                GameManager.NotificationSystem.itemUnequipped.Invoke(toUnequip);

                if (autoUpdateStats)
                {
                    UpdateStats();
                }
            }

            return toUnequip;
        }

        protected override void OnAbilityAdded(AbilitySheet ability)
        {
            base.OnAbilityAdded(ability);

            for (int i = 0; i < m_equippedAbilities.Length; ++i)
            {
                if (m_equippedAbilities[i] == null && ability is ActiveAbilitySheet activeAbilitySheet && !IsAbilityEquiped(activeAbilitySheet))
                {
                    Equip(activeAbilitySheet, i);
                    break;
                }
            }

            GameManager.NotificationSystem.abilityAdded.Invoke(ability);
        }

        protected override void OnAbilityRemoved(AbilitySheet ability)
        {
            base.OnAbilityRemoved(ability);

            for (int i = 0; i < m_equippedAbilities.Length; ++i)
            {
                if (m_equippedAbilities[i] == ability)
                {
                    Unequip(i);
                }
            }

            GameManager.NotificationSystem.abilityRemoved.Invoke(ability);
        }

        public void Equip(ActiveAbilitySheet ability, int index)
        {
            Debug.Assert(index >= 0 && index < GameManager.Config.maxEquippableAbilities, "Invalid ability index.");

            m_equippedAbilities[index] = ability;
            m_equippedAbilitiesChanged.Invoke(m_equippedAbilities);
        }

        public void Unequip(int index)
        {
            Debug.Assert(index >= 0 && index < GameManager.Config.maxEquippableAbilities, "Invalid ability index.");

            m_equippedAbilities[index] = null;
            m_equippedAbilitiesChanged.Invoke(m_equippedAbilities);
        }

        public bool IsAbilityEquiped(ActiveAbilitySheet abilitySheet)
        {
            foreach (ActiveAbilitySheet ability in m_equippedAbilities)
            {
                if (ability == abilitySheet) return true;
            }

            return false;
        }

        private Stats CalculateEquipmentStats(IEnumerable<Equipment> equipments)
        {
            Stats equipmentStats = new();

            foreach (Equipment piece in equipments)
            {
                if (piece)
                {
                    equipmentStats += piece.bonusStats;
                }
            }

            return equipmentStats;
        }

        private void UpdateStats()
        {
            Stats equipmentStats = CalculateEquipmentStats(m_equipments.Values);
            Stats totalStats = m_sheet.baseStats + m_customStats + equipmentStats;
            m_stats.Set(totalStats);
        }

        public override void LevelUp(bool silentMode = false)
        {
            base.LevelUp(silentMode);

            if (!silentMode)
            {
                GameManager.NotificationSystem.levelUp.Invoke(m_level);
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_levelUpSound);
            }
        }

        protected override void OnDeath()
        {
            m_destroyOnDeath = false; // Prevents the Hero GameObject from being destroyed, so it can be used in the death screen.
            base.OnDeath();
            m_animationStrategy?.Pause();
            GameManager.NotificationSystem.heroKilled.Invoke(this);
        }

        protected override Type GetDataBlockType() => typeof(HeroDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            var heroBlock = block.As<HeroDataBlock>();
            heroBlock.usedPoints = usedPoints;
            heroBlock.experience = experience;
            heroBlock.equipments = equipments.Values.Select(equipment => GameManager.Database.CreateReference(equipment)).ToArray();
            heroBlock.customStats = customStats;
            heroBlock.equippedAbilities = equippedAbilities.Where(ability => ability != null).Select(ability => GameManager.Database.CreateReference(ability)).ToArray();
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            var heroBlock = block.As<HeroDataBlock>();

            m_usedPoints = heroBlock.usedPoints;

            if (heroBlock.experience > 0)
            {
                AddExperience(heroBlock.experience, true);
            }

            m_equipments = new SerializableDictionary<EEquipmentType, Equipment>();

            foreach (var piece in heroBlock.equipments)
            {
                Equip(GameManager.Database.LoadFromReference(piece), false); // 'false' so we don't update stats until all equipment is equipped
            }

            m_customStats = heroBlock.customStats;

            UpdateStats(); // Necessary since "Equip" above is called with autoUpdateStats = false

            for (int i = 0; i < m_equippedAbilities.Length; ++i)
            {
                if (i < heroBlock.equippedAbilities.Length)
                {
                    ActiveAbilitySheet ability = GameManager.Database.LoadFromReference(heroBlock.equippedAbilities[i]);

                    if (HasAbility(ability))
                    {
                        m_equippedAbilities[i] = ability;
                    }
                    else
                    {
                        Debug.LogWarning($"The ability {ability} is set to be equipped, but this character hasn't unlocked it yet (or no longer has it).");
                        m_equippedAbilities[i] = null;
                    }
                }
                else
                {
                    m_equippedAbilities[i] = null;
                }
            }

            m_equippedAbilitiesChanged.Invoke(m_equippedAbilities);

            base.OnLoad(block); // We load the CharacterBase block last so the current stats are reloaded after the whole stats logic here
        }

        protected override void OnStuckInAWall()
        {
            Debug.Assert(false, "Oops! The player is stuck in a wall. This should never happen.");
        }
    }
}
