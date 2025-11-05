using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using MackySoft.SerializeReferenceExtensions;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    public enum EAlignment
    {
        Good,
        Evil,
        Neutral,
        Default = Neutral
    }

    [Flags]
    public enum EActionFlags
    {
        [HideInInspector] None = 0,
        Move = 1 << 0,
        Interact = 1 << 1,
        UseAbility = 1 << 2,
        UpdateTargetDirection = 1 << 3,
        [HideInInspector] All = ~None
    }

    [Serializable]
    public class CharacterBaseDataBlock : MovableDataBlock
    {
        public int level;
        [SerializeReference, SubclassSelector] public Stats currentStats;
        [SerializeReference, SubclassSelector] public AbilityBaseDataBlock[] abilitiesData;
        public SerializableDictionary<DatabaseEntryReference<AbilitySheet>, int> bonusAbilities;
        [SerializeReference, SubclassSelector] public ITemporalEffect[] temporalEffects;
        public SerializableDictionary<string, EActionFlags> lockedActions;
        public SerializableDictionary<string, float> speedModifiers;
    }

    public abstract class CharacterBase : Movable
    {
        [Header("Character Base Settings")]
        [Range(Constants.MinLevel, Constants.MaxLevel)]
        [SerializeField] protected int m_level = Constants.MinLevel;
        [SerializeField] private bool m_invincibleOnHit = false;
        [SerializeField] private bool m_invincibleOnRevive = false;
        [SerializeField] private bool m_invincible = false;
        [SerializeField] private Transform m_abilitiesStaticRoot = null;
        [SerializeField] private Transform m_abilitiesPolydirectionalRoot = null;
        [SerializeField] private Transform m_abilitiesHorizontalRoot = null;
        [SerializeField] private ActiveAbilitySheet[] m_additionalAbilities = null;
        [SerializeField] private bool m_restoreHealthOnLevelUp = true;
        [SerializeField] private bool m_restoreManaOnLevelUp = true;

        public UnityEvent<CharacterBase> provokedEvent => m_provokedEvent;
        public IEnumerable<AbilityBase> abilityInstances => m_abilitiesInstances.Values;
        public IEnumerable<ITriggerableAbility> triggerableAbilities => m_triggerableAbilities;
        public abstract CharacterSheet characterSheet { get; }
        public bool dead => m_currentStats[EStat.Health] == 0;
        public int level => m_level;
        public bool invincibleOnHit => m_invincibleOnHit;
        public EAlignment currentAlignment => m_alignmentOverride ?? characterSheet.alignment;
        public bool invincible => m_invincible || m_invincibilityFrames > 0 || dead || (m_animationStrategy?.IsInvincibleAnimationPlaying() ?? false);
        public ObservableStats stats => m_stats;
        public ObservableStats currentStats => m_currentStats;
        public UnityEvent<Stats> currentStatsChanged => m_currentStats.changed;
        public UnityEvent<Stats> statsChanged => m_stats.changed;
        public IEnumerable<ITemporalEffect> temporalEffects => m_temporalEffects;
        public UnityEvent<ITemporalEffect> temporalEffectAdded => m_temporalEffectAdded;
        public UnityEvent<ITemporalEffect> temporalEffectRemoved => m_temporalEffectRemoved;
        public UnityEvent<int> levelUpped => m_levelUpped;

        // Character Base Private Members
        private UnityEvent<CharacterBase> m_provokedEvent = new();
        private EActionFlags m_actionFlags = EActionFlags.All;
        private Dictionary<string, EActionFlags> m_lockedActions = new();
        private Dictionary<string, float> m_moveSpeedFactors = new();
        private HashSet<AbilitySheet> m_abilities = new();
        private Dictionary<AbilitySheet, int> m_bonusAbilities = new();
        private Dictionary<AbilitySheet, AbilityBase> m_abilitiesInstances = new();
        private HashSet<ITriggerableAbility> m_triggerableAbilities = new();
        protected ObservableStats m_stats = new();
        protected ObservableStats m_currentStats = new();
        protected List<ITemporalEffect> m_temporalEffects = new();

        // Move Private Members
        private EAlignment? m_alignmentOverride = null;
        private UnityEvent<ITemporalEffect> m_temporalEffectAdded = new();
        private UnityEvent<ITemporalEffect> m_temporalEffectRemoved = new();
        private UnityEvent<int> m_levelUpped = new();
        private bool m_isDeadAndDestroyed = false;
        private int m_invincibilityFrames = 0;

        protected bool m_isSummoned = false;
        private bool m_levelChanged = false;

        protected override void Awake()
        {
            base.Awake();

            m_stats.changed.AddListener(OnStatsChanged);
            m_currentStats.changed.AddListener(OnCurrentStatsChanged);

            InitializeStats();
            InitializeAbilities();
        }

        protected override void Update()
        {
            base.Update();

            foreach (AbilityBase ability in abilityInstances)
            {
                ability.UpdateCooldowns();
            }

            m_temporalEffects.RemoveAll(effect =>
            {
                effect.Update();
                if (effect.completed)
                {
                    effect.Complete();
                    m_temporalEffectRemoved.Invoke(effect);
                    return true;
                }
                return false;
            });
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (m_invincibilityFrames > 0)
            {
                --m_invincibilityFrames;
            }
        }

        protected override void OnDeathAnimationEnd()
        {
            // Prevent from the animator to call this method multiple times after being disabled/enabled.
            // Not ideal, but it does the job.
            if (!m_isDeadAndDestroyed)
            {
                base.OnDeathAnimationEnd();
            }
        }

        protected override void OnDeath()
        {
            base.OnDeath();
            m_isDeadAndDestroyed = true;
        }

        protected override AudioClipResolver GetDeathAudio() => characterSheet.deathAudio;

        public override void Revive()
        {
            Heal(int.MaxValue, EEffectVisualFlags.NoFloatingText);
            RecoverMana(int.MaxValue, EEffectVisualFlags.NoFloatingText);

            // Workaround to prevent the character from getting hit immediately after being revived.
            // Even if m_invincibleOnRevive is used, invincibility only happens one frame after the animation
            // is requested to play. This is a problem because the character can get hit during that frame.
            m_invincibilityFrames = 2;

            foreach (AbilityBase ability in abilityInstances)
            {
                ability.Reset();
            }

            base.Revive();

            m_isDeadAndDestroyed = false;

            if (m_invincibleOnRevive)
            {
                m_animationStrategy?.PlayInvincibleAnimation();
            }
        }

        public int Cleanse(IEnumerable<EEffectType> effectTypes)
        {
            IEnumerable<ITemporalEffect> effects = m_temporalEffects.Where(effect => effectTypes.Contains(effect.GetEffectType()));

            int effectCount = effects.Count();

            if (effectCount > 0)
            {
                List<ITemporalEffect> effectsToRemove = effects.ToList();
                effectsToRemove.ForEach(effect => RemoveTemporalEffectPrematurely(effect));
            }

            return effectCount;
        }

        public void SetInvincibleOnHit(bool invincibleOnHit) => m_invincibleOnHit = invincibleOnHit;

        public override string GetSpeakerName() => characterSheet.displayName;

        public override void OnInteract(CharacterBase sender)
        {
            LookAtTarget(sender.transform);
            base.OnInteract(sender);
        }

        public override bool CanUpdateTargetDirection() => base.CanUpdateTargetDirection() && Can(EActionFlags.UpdateTargetDirection);
        public override bool CanMove() => base.CanMove() && Can(EActionFlags.Move);

        protected override float CalculateMoveSpeed()
        {
            float speed = base.CalculateMoveSpeed();

            foreach (float factor in m_moveSpeedFactors.Values)
            {
                speed *= factor;
            }

            return speed;
        }

        protected virtual void InitializeStats()
        {
            Stats initial = new();
            initial[EStat.Health] = 1;
            m_stats.Set(initial);
        }

        private void OnStatsChanged(Stats previous)
        {
            Stats difference = m_stats.values - previous;
            m_currentStats.Set(m_currentStats.values + difference);
        }

        private void OnCurrentStatsChanged(Stats previous)
        {
            if (previous[EStat.Health] > 0 && m_currentStats[EStat.Health] == 0)
            {
                Kill();
            }
        }

        public override void Kill()
        {
            base.Kill();

            Cleanse(new[] { EEffectType.Buff, EEffectType.Debuff });

            foreach (AbilityBase ability in abilityInstances)
            {
                ability.Interrupt();
            }
        }

        protected virtual void OnAbilityAdded(AbilitySheet sheet)
        {
            m_abilitiesInstances[sheet] = InstantiateAbilityPrefab(sheet);

            if (m_abilitiesInstances[sheet] is ITriggerableAbility triggerableAbility)
            {
                m_triggerableAbilities.Add(triggerableAbility);
            }
        }

        protected virtual void OnAbilityRemoved(AbilitySheet sheet)
        {
            if (m_abilitiesInstances.ContainsKey(sheet))
            {
                AbilityBase instance = m_abilitiesInstances[sheet];

                if (instance is ITriggerableAbility triggerableAbility)
                {
                    m_triggerableAbilities.Remove(triggerableAbility);
                }

                m_abilitiesInstances.Remove(sheet);

                if (instance)
                {
                    Destroy(instance.gameObject);
                }
            }
        }

        private void InitializeAbilities()
        {
            var abilities = characterSheet.GetAvailableAbilitiesAtLevel(m_level).Union(m_additionalAbilities);

            foreach (AbilitySheet ability in abilities)
            {
                if (m_abilities.Add(ability))
                {
                    OnAbilityAdded(ability);
                }
            }
        }

        public void AddBonusAbility(AbilitySheet ability, int count = 1)
        {
            if (m_bonusAbilities.ContainsKey(ability))
            {
                m_bonusAbilities[ability] += count;
            }
            else
            {
                m_bonusAbilities.Add(ability, count);
                OnAbilityAdded(ability);
            }
        }

        public void RemoveBonusAbility(AbilitySheet ability)
        {
            if (m_bonusAbilities.ContainsKey(ability))
            {
                if (m_bonusAbilities[ability] == 1)
                {
                    m_bonusAbilities.Remove(ability);
                    OnAbilityRemoved(ability);
                }
                else
                {
                    --m_bonusAbilities[ability];
                }
            }
        }

        public bool HasAbility(AbilitySheet ability) => m_abilitiesInstances.ContainsKey(ability);

        public AbilityBase GetAbility(AbilitySheet sheet) => m_abilitiesInstances[sheet];

        private Transform GetAbilityRoot(AbilitySheet.EAbilityOrientationMode orientationMode)
        {
            switch (orientationMode)
            {
                case AbilitySheet.EAbilityOrientationMode.Static: return m_abilitiesStaticRoot;
                case AbilitySheet.EAbilityOrientationMode.Polydirectional: return m_abilitiesPolydirectionalRoot;
                case AbilitySheet.EAbilityOrientationMode.Horizontal: return m_abilitiesHorizontalRoot;
            }

            return null;
        }

        private bool GetDefaultAbilityState(AbilityBase abilityInstance)
        {
            switch (abilityInstance.baseAbilitySheet.abilityStateManagementMode)
            {
                case AbilitySheet.EAbilityStateManagementMode.AlwaysOn: return true;
                case AbilitySheet.EAbilityStateManagementMode.AlwaysOff: return false;
                case AbilitySheet.EAbilityStateManagementMode.Automatic: return abilityInstance is not ITriggerableAbility;
            }

            return false;
        }

        private AbilityBase InstantiateAbilityPrefab(AbilitySheet sheet)
        {
            Transform abilityRoot = GetAbilityRoot(sheet.orientationMode);
            Debug.Assert(abilityRoot != null, "No ability root found! Make sure to assign each ability root in the inspector.");

            GameObject instance = Instantiate(sheet.prefab, abilityRoot);
            instance.name = sheet.displayName;

            AbilityBase ability = instance.GetComponent<AbilityBase>();
            Debug.Assert(ability, $"The provided ability prefab doesn't have a behaviour of type {typeof(AbilityBase).Name} attached to its root");

            ability.Init(this, sheet);
            ability.gameObject.SetActive(GetDefaultAbilityState(ability));
            return ability;
        }

        public EAbilityFireCheckResult FireAbility(ActiveAbilitySheet sheet, out ITriggerableAbility ability)
        {
            if (m_abilitiesInstances.TryGetValue(sheet, out AbilityBase abilityBase) && abilityBase is ITriggerableAbility triggerableAbility)
            {
                ability = triggerableAbility;
                return FireAbility(triggerableAbility);
            }
            else
            {
                Debug.LogError($"Could not find triggerable ability matching ability sheet [{sheet.name}]");
            }

            ability = null;
            return EAbilityFireCheckResult.Unknown;
        }

        public EAbilityFireCheckResult FireAbility(ITriggerableAbility ability)
        {
            ActiveAbilityBase abilityBase = ability.GetAbilityBase();

            EAbilityFireCheckResult triggerAbilityCheckResult = ability.CanFire();

            if (triggerAbilityCheckResult == EAbilityFireCheckResult.Valid)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(abilityBase.abilitySheet.fireAudio);

                bool isAbilityStateAutomaticallyManaged = abilityBase.baseAbilitySheet.abilityStateManagementMode == ActiveAbilitySheet.EAbilityStateManagementMode.Automatic;

                if (abilityBase.abilitySheet.updateLookAtDirectionOnFire)
                {
                    SetLookAtDirection(GetTargetDirection());
                }

                if (isAbilityStateAutomaticallyManaged)
                {
                    abilityBase.gameObject.SetActive(true);
                    ability.Fire(() => abilityBase.gameObject.SetActive(false));
                }
                else
                {
                    ability.Fire(null);
                }
            }

            return triggerAbilityCheckResult;
        }

        public void FlagAsSummoned()
        {
            m_isSummoned = true;
        }

        public void SetAlignmentOverride(EAlignment? alignment)
        {
            m_alignmentOverride = alignment;
        }

        private void InterruptActions()
        {
            BroadcastMessage("OnActionInterrupted", SendMessageOptions.DontRequireReceiver);
        }

        public void AddTemporalEffect(ITemporalEffect effect)
        {
            m_temporalEffects.Add(effect);
            m_temporalEffectAdded.Invoke(effect);
            GameManager.NotificationSystem.temporalEffectApplied.Invoke(this, effect);
        }

        public void RemoveTemporalEffectPrematurely(ITemporalEffect effect)
        {
            effect.Complete();
            m_temporalEffectRemoved.Invoke(effect);
            m_temporalEffects.Remove(effect);
        }

        public bool Damage(DamageOutputDescriptor damageOutput, EEffectVisualFlags visualFlags = EEffectVisualFlags.None, Vector2? velocity = null)
        {
            CharacterBase sourceCharacter = (damageOutput.source is CharacterDamageSource characterDamage) ? characterDamage.character : null;

            bool isSelfTargeted = sourceCharacter == this;

            if (CombatSolver.CanTarget(damageOutput, this))
            {
                DamageInputDescriptor damageInput = DamageSolver.SolveDamageInput(this, damageOutput);

                if (velocity.HasValue)
                {
                    TryPush(damageInput, velocity.Value);
                }

                if (sourceCharacter != null)
                {
                    provokedEvent.Invoke(sourceCharacter);
                }

                if (damageInput.damage > 0)
                {
                    if (!damageInput.silent)
                    {
                        InterruptActions();
                        m_animationStrategy?.PlayHitAnimation();
                    }

                    m_currentStats[EStat.Health] -= math.min(damageInput.damage, m_currentStats[EStat.Health]);

                    GameManager.NotificationSystem.audioPlaybackRequested.Invoke(characterSheet.hitAudio);

                    // Doesn't play invincible animation if the damage source is the character itself. Otherwise the character could
                    // abuse it to avoid getting hit by certain abilities.
                    if (!dead && !damageInput.silent && m_invincibleOnHit && !isSelfTargeted)
                    {
                        m_animationStrategy?.PlayInvincibleAnimation();
                    }
                }

                GameManager.NotificationSystem.damageApplied.Invoke(this, damageInput, visualFlags);

                return !damageInput.flags.HasFlag(EDamageFlag.Miss);
            }

            return false;
        }

        public void Heal(int value, EEffectVisualFlags visualFlags = EEffectVisualFlags.None)
        {
            int missingHealth = m_stats[EStat.Health] - m_currentStats[EStat.Health];
            m_currentStats[EStat.Health] += math.min(value, missingHealth);
            GameManager.NotificationSystem.healthRecovered.Invoke(this, value, visualFlags);
        }

        public void RecoverMana(int value, EEffectVisualFlags visualFlags = EEffectVisualFlags.None)
        {
            int missingMana = m_stats[EStat.Mana] - m_currentStats[EStat.Mana];
            m_currentStats[EStat.Mana] += math.min(value, missingMana);
            GameManager.NotificationSystem.manaRecovered.Invoke(this, value, visualFlags);
        }

        public void ConsumeMana(int value, EEffectVisualFlags visualFlags = EEffectVisualFlags.None)
        {
            m_currentStats[EStat.Mana] -= math.min(value, m_currentStats[EStat.Mana]);
            GameManager.NotificationSystem.manaConsumed.Invoke(this, value, visualFlags);
        }

        public virtual void LevelUp(bool silentMode = false)
        {
            m_levelChanged = true;

            ++m_level;

            if (!silentMode)
            {
                if (m_restoreHealthOnLevelUp)
                {
                    Heal(m_stats[EStat.Health] - m_currentStats[EStat.Health]);
                }

                if (m_restoreManaOnLevelUp)
                {
                    RecoverMana(m_stats[EStat.Mana] - m_currentStats[EStat.Mana]);
                }
            }

            foreach (AbilitySheet ability in characterSheet.GetAbilitiesUnlockedAtLevel(m_level))
            {
                if (m_abilities.Add(ability))
                {
                    OnAbilityAdded(ability);
                }
            }

            m_levelUpped.Invoke(m_level);
        }

        public string ApplyMoveSpeedFactor(float factor)
        {
            string key = Guid.NewGuid().ToString();
            m_moveSpeedFactors[key] = factor;
            return key;
        }

        public void UpdateMoveSpeedFactor(string key, float factor)
        {
            Debug.Assert(m_moveSpeedFactors.ContainsKey(key), "Invalid key, no move speed factor is applied with this key.");
            m_moveSpeedFactors[key] = factor;
        }

        public void RemoveMoveSpeedFactor(string key)
        {
            Debug.Assert(m_moveSpeedFactors.ContainsKey(key), "Invalid key, no move speed factor is applied with this key.");
            m_moveSpeedFactors.Remove(key);
        }

        public string LockActions(EActionFlags actions)
        {
            // The key needs to be used to unlock the actions
            string key = Guid.NewGuid().ToString();
            m_lockedActions[key] = actions;
            return key;
        }

        public void UnlockActions(string key)
        {
            Debug.Assert(m_lockedActions.ContainsKey(key), "Invalid key, no actions are locked with this key.");
            m_lockedActions.Remove(key);
        }

        public bool IsActionLocked(EActionFlags actions)
        {
            foreach (EActionFlags lockedActions in m_lockedActions.Values)
            {
                if (lockedActions.HasFlag(actions))
                {
                    return true;
                }
            }

            return false;
        }

        public void EnableActions(EActionFlags actions)
        {
            m_actionFlags |= actions;
        }

        public void DisableActions(EActionFlags actions)
        {
            m_actionFlags &= ~actions;
        }

        public bool Can(EActionFlags actions)
        {
            return m_actionFlags.HasFlag(actions) && !IsActionLocked(actions);
        }

        protected override Type GetDataBlockType() => typeof(CharacterBaseDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            CharacterBaseDataBlock characterBlock = block.As<CharacterBaseDataBlock>();
            characterBlock.currentStats = currentStats.values;
            characterBlock.level = m_levelChanged ? m_level : 0;
            characterBlock.bonusAbilities = new SerializableDictionary<DatabaseEntryReference<AbilitySheet>, int>(m_bonusAbilities.ToDictionary(kvp => GameManager.Database.CreateReference(kvp.Key), kvp => kvp.Value));
            characterBlock.temporalEffects = m_temporalEffects.ToArray();
            characterBlock.lockedActions = new SerializableDictionary<string, EActionFlags>(m_lockedActions);
            characterBlock.speedModifiers = new SerializableDictionary<string, float>(m_moveSpeedFactors);

            var abilitiesData = new List<AbilityBaseDataBlock>();
            foreach (var ability in abilityInstances)
            {
                abilitiesData.Add(ability.CreateDataBlockManual().As<AbilityBaseDataBlock>());
            }

            characterBlock.abilitiesData = abilitiesData.ToArray();
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);

            var characterBlock = block.As<CharacterBaseDataBlock>();

            if (characterBlock.currentStats != null)
            {
                m_currentStats.Set(characterBlock.currentStats);
            }

            // Make sure to clear existing bonus abilities.
            // Since derived classes might add-back their own bonus abilities, and we don't want to count them multiple times.
            m_bonusAbilities.Clear();

            foreach (var ability in characterBlock.bonusAbilities)
            {
                AddBonusAbility(GameManager.Database.LoadFromReference(ability.Key), ability.Value);
            }

            while (m_level < characterBlock.level)
            {
                LevelUp(silentMode: true);
            }

            if (characterBlock.abilitiesData != null)
            {
                foreach (var abilityDataBlock in characterBlock.abilitiesData)
                {
                    if (abilityDataBlock != null && abilityDataBlock.sheet != null)
                    {
                        AbilitySheet sheet = GameManager.Database.LoadFromReference(abilityDataBlock.sheet);

                        if (m_abilitiesInstances.ContainsKey(sheet))
                        {
                            AbilityBase ability = m_abilitiesInstances[sheet];
                            ability.LoadDataBlock(abilityDataBlock);
                        }
                        else
                        {
                            Debug.LogWarning($"Could not find ability instance matching ability sheet [{sheet.name}]");
                        }
                    }
                }
            }

            m_temporalEffects = new List<ITemporalEffect>(characterBlock.temporalEffects);
            m_lockedActions = characterBlock.lockedActions.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            m_moveSpeedFactors = characterBlock.speedModifiers.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}
