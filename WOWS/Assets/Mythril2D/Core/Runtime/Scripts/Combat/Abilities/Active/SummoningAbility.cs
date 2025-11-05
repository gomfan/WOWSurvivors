using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class SummoningAbilityDataBlock : ActiveAbilityBaseDataBlock
    {
        public CharacterBaseDataBlock[] summons;
    }

    public class SummoningAbility : ActiveAbility<SummoningAbilitySheet>
    {
        [Header("Reference")]
        [SerializeField] private Animator m_animator = null;

        [Header("Animation Parameters")]
        [SerializeField] private string m_fireAnimationParameter = "fire";

        private HashSet<CharacterBase> m_summons = new();

        public override void Init(CharacterBase character, AbilitySheet settings)
        {
            base.Init(character, settings);

            Debug.Assert(m_animator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_animator.GetBehaviour<StateMessageDispatcher>(), string.Format("{0} not found on the ability's animator controller", typeof(StateMessageDispatcher).Name));

            m_character.teleported.AddListener(OnMasterTeleported);
        }

        public override void Destroy()
        {
            base.Destroy();

            if (m_character != null)
            {
                m_character.teleported.RemoveListener(OnMasterTeleported);
            }
        }

        public override void Interrupt()
        {
            base.Interrupt();

            foreach (CharacterBase summon in m_summons)
            {
                summon.Kill();
            }

            m_summons.Clear();
        }

        private void OnMasterTeleported()
        {
            foreach (CharacterBase summon in m_summons)
            {
                summon.transform.position = m_character.transform.position;
            }
        }

        protected override void Fire()
        {
            m_animator?.SetTrigger(m_fireAnimationParameter);
        }

        public void OnCastAnimationEnded()
        {
            if (!m_character.dead)
            {
                Summon();
                TerminateCasting();
            }
        }

        private void MakeSpaceIfNecessary()
        {
            if (m_summons.Count >= activeAbilitySheet.maxSummonCount)
            {
                CharacterBase summon = m_summons.Last();

                m_summons.Remove(summon);

                if (summon != null && !summon.dead)
                {
                    summon.Kill();
                }
            }
        }

        public void Summon(CharacterBaseDataBlock data = null)
        {
            MakeSpaceIfNecessary();

            Vector2 direction = m_character.GetTargetDirection();

            Persistable persistable = GameManager.PersistenceSystem.InstantiateCustom(
                activeAbilitySheet.toSummon,
                Vector3.zero,
                Quaternion.identity,
                m_character.transform, // <-- attach the summoned object to the summoner (so they are on the same map)
                data != null && data.info is IIdentifiablePersistentDataHandler identifiablePersistentDataHandler ? identifiablePersistentDataHandler.GetIdentifier() : null
            );

            persistable.transform.parent = null; // Unparent the summoned object (it was parented to the summoner to ensure they are spawned on the same map)
            persistable.transform.position = m_character.transform.position + new Vector3(direction.x, direction.y, 0.0f);

            CharacterBase character = persistable as CharacterBase;
            Debug.Assert(character != null, "SummoningAbility: toSummon must have a CharacterBase component");

            character.destroyedEvent.AddListener(() => m_summons.Remove(character));
            if (data != null)
            {
                character.LoadDataBlock(data);
            }

            m_summons.Add(character);

            character.FlagAsSummoned();

            if (activeAbilitySheet.matchSummonerAlignment)
            {
                character.SetAlignmentOverride(m_character.currentAlignment);
            }

            if (activeAbilitySheet.matchSummonerInvincibilityOnHit)
            {
                character.SetInvincibleOnHit(m_character.invincibleOnHit);
            }

            if (activeAbilitySheet.followSummoner)
            {
                Debug.Assert(character.controller is AIController, "The summoned character must have an AIController to follow the summoner, otherwise you need to disable the followSummoner option");
                AIController aiController = (AIController)character.controller;
                aiController.SetMaster(m_character);
            }

            if (activeAbilitySheet.increaseLevelToMatchSummoner)
            {
                while (character.level < m_character.level)
                {
                    character.LevelUp(true);
                }
            }

            foreach (ActiveAbilitySheet abilitySheet in activeAbilitySheet.bonusAbilities)
            {
                character.AddBonusAbility(abilitySheet);
            }
        }

        protected override Type GetDataBlockType() => typeof(SummoningAbilityDataBlock);

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);

            foreach (CharacterBaseDataBlock summonData in block.As<SummoningAbilityDataBlock>().summons)
            {
                Summon(summonData);
            }
        }

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);

            block.As<SummoningAbilityDataBlock>().summons = m_summons.Select(
                s => s.CreateDataBlock().As<CharacterBaseDataBlock>()
            ).ToArray();
        }
    }
}
