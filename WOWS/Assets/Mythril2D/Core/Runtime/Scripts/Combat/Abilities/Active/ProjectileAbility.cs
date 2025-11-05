using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ProjectileAbilityDataBlock : ActiveAbilityBaseDataBlock
    {
        [SerializeReference] public ProjectileDataBlock[] projectiles;
    }

    public class ProjectileAbility : ActiveAbility<ProjectileAbilitySheet>
    {
        [Header("References")]
        [SerializeField] private Animator m_animator = null;
        [SerializeField] private Transform m_projectileSpawnPoint = null;

        [Header("Animation Parameters")]
        [SerializeField] private string m_fireAnimationParameter = "fire";

        private List<Projectile> m_projectiles = new();

        public override void Init(CharacterBase character, AbilitySheet settings)
        {
            base.Init(character, settings);

            Debug.Assert(m_animator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_animator.GetBehaviour<StateMessageDispatcher>(), string.Format("{0} not found on the projectile animator controller", typeof(StateMessageDispatcher).Name));
        }

        protected override void Fire()
        {
            m_animator?.SetTrigger(m_fireAnimationParameter);
        }

        public void OnCastAnimationEnded()
        {
            if (!m_character.dead)
            {
                for (int i = 0; i < activeAbilitySheet.projectileCount; ++i)
                {
                    ThrowProjectile(i);
                }

                TerminateCasting();
            }
        }

        private void ThrowProjectile(int projectileIndex)
        {
            Vector2 direction = m_character.GetTargetDirection();
            float totalSpread = activeAbilitySheet.spread;
            float spreadStep = totalSpread / Mathf.Max(1, activeAbilitySheet.projectileCount - 1);
            float spreadAngle = -totalSpread / 2 + spreadStep * projectileIndex;
            Vector2 actualDirection = Quaternion.Euler(0, 0, spreadAngle) * direction;
            Projectile projectile = InstantiateProjectile(m_projectileSpawnPoint.position);

            projectile.Throw(m_character, actualDirection, activeAbilitySheet);
        }

        private Projectile InstantiateProjectile(Vector3 position)
        {
            GameObject go = Instantiate(activeAbilitySheet.projectile, position, Quaternion.identity);
            Projectile projectile = go.GetComponent<Projectile>();
            Debug.Assert(projectile, ErrorMessages.MissingComponentReference<Projectile>());
            m_projectiles.Add(projectile);
            return projectile;
        }

        protected override Type GetDataBlockType() => typeof(ProjectileAbilityDataBlock);

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);

            foreach (ProjectileDataBlock projectileData in block.As<ProjectileAbilityDataBlock>().projectiles)
            {
                Projectile projectile = InstantiateProjectile(projectileData.position);
                projectile.LoadDataBlock(projectileData);
            }
        }

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<ProjectileAbilityDataBlock>().projectiles = m_projectiles.Where(p => p != null).Select(p => p.CreateDataBlockManual() as ProjectileDataBlock).ToArray();
        }
    }
}
