using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ProjectileDataBlock : EntityDataBlock
    {
        public Vector2 direction;
        public float speed;
        public float remainingLifetime;
        public bool operating;
        public PersistableReference<CharacterBase> source;
        [SerializeReference] public IEffect[] baseEffects;
        [SerializeReference] public IEffect[] explosionEffects;
        public float explosionRadius;
        public bool explosionIgnorePrimaryTarget;
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : Entity
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D m_rigidbody = null;
        [SerializeField] private Animator m_animator = null;

        [Header("Settings")]
        [SerializeField] private bool m_reverseRotation = false;
        [SerializeField] private float m_maxDuration = 2.0f;

        [Header("Animation Parameters")]
        [SerializeField] private string m_destroyAnimationParameter = "destroy";

        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_collisionSound;

        private CharacterBase m_source;
        private List<IEffect> m_baseEffects;
        private float m_explosionRadius;
        private bool m_explosionApplyBaseEffects;
        private bool m_explosionBaseEffectsIgnorePrimaryTarget;
        private bool m_explosionAdditionalEffectsIgnorePrimaryTarget;
        private List<IEffect> m_explosionAdditionalEffects;
        private Vector2 m_direction;
        private float m_speed;
        private float m_remainingLifetime;
        private bool m_hasDestroyAnimation;
        private bool m_operating = false;

        private void Awake()
        {
            m_hasDestroyAnimation = m_animator && AnimationUtils.HasParameter(m_animator, m_destroyAnimationParameter);
        }

        public void Throw(CharacterBase source, Vector2 direction, ProjectileAbilitySheet abilitySheet)
        {
            m_source = source;

            m_baseEffects = new List<IEffect>(abilitySheet.effects);
            m_direction = direction;
            m_speed = abilitySheet.projectileSpeed;
            m_remainingLifetime = m_maxDuration;

            // Explosion settings
            m_explosionRadius = abilitySheet.explosionRadius;
            m_explosionApplyBaseEffects = abilitySheet.explosionApplyBaseEffects;
            m_explosionBaseEffectsIgnorePrimaryTarget = abilitySheet.explosionBaseEffectsIgnorePrimaryTarget;
            m_explosionAdditionalEffectsIgnorePrimaryTarget = abilitySheet.explosionAdditionalEffectsIgnorePrimaryTarget;
            m_explosionAdditionalEffects = new List<IEffect>(abilitySheet.explosionAdditionalEffects);

            m_operating = true;
        }

        public void OnDestroyAnimationEnd()
        {
            Destroy();
        }

        private bool TryApplyingExplosionBaseEffects(Vector2 explosionOrigin, CharacterBase primaryTarget, IEnumerable<CharacterBase> characters)
        {
            if (m_explosionApplyBaseEffects)
            {
                var baseEffectsTargets = new HashSet<CharacterBase>(characters);
                if (primaryTarget != null && m_explosionBaseEffectsIgnorePrimaryTarget)
                {
                    baseEffectsTargets.Remove(primaryTarget);
                }

                ApplyExplosion(explosionOrigin, baseEffectsTargets, m_baseEffects);
                return true;
            }

            return false;
        }

        private bool TryApplyingExplosionAdditionalEffects(Vector2 explosionOrigin, CharacterBase primaryTarget, IEnumerable<CharacterBase> characters)
        {
            if (m_explosionAdditionalEffects.Any())
            {
                var additionalEffectsTargets = new HashSet<CharacterBase>(characters);
                if (primaryTarget != null && m_explosionAdditionalEffectsIgnorePrimaryTarget)
                {
                    additionalEffectsTargets.Remove(primaryTarget);
                }

                ApplyExplosion(explosionOrigin, additionalEffectsTargets, m_explosionAdditionalEffects);
                return true;
            }

            return false;
        }

        private void ApplyExplosion(Vector2 explosionOrigin, IEnumerable<CharacterBase> targets, IEnumerable<IEffect> effects)
        {
            EffectDispatcher.Apply(m_source, targets, effects, new()
            {
                impactDataType = EEffectImpactDataType.SourcePosition,
                impactData = explosionOrigin
            });
        }

        private void HandleExplosion(CharacterBase primaryTarget)
        {
            if (m_explosionRadius > 0.0f)
            {
                if (!m_hasDestroyAnimation)
                {
                    Debug.LogWarning("This projectile has an explosion radius but no destroy animation. The explosion may not be visible.");
                }

                Vector2 explosionOrigin = transform.position;

                var characters = Physics2D.OverlapCircleAll(explosionOrigin, m_explosionRadius)
                    .Select(collider => collider.GetComponentInParent<CharacterBase>())
                    .Where(character => character != null);

                TryApplyingExplosionBaseEffects(explosionOrigin, primaryTarget, characters);
                TryApplyingExplosionAdditionalEffects(explosionOrigin, primaryTarget, characters);
            }
        }

        private void Terminate(CharacterBase primaryTarget = null)
        {
            if (m_operating)
            {
                m_operating = false;
                m_rigidbody.linearVelocity = Vector3.zero;
                HandleExplosion(primaryTarget);

                if (m_hasDestroyAnimation)
                {
                    m_animator?.SetTrigger(m_destroyAnimationParameter);
                }
                else
                {
                    Destroy();
                }
            }
        }

        private void Update()
        {
            if (m_operating)
            {
                m_remainingLifetime -= Time.deltaTime;

                if (m_remainingLifetime <= 0.0f)
                {
                    Terminate();
                }
            }
        }

        private void FixedUpdate()
        {
            transform.rotation = Quaternion.LookRotation(Vector3.forward, m_direction * (m_reverseRotation ? -1.0f : 1.0f));

            m_rigidbody.linearVelocity =
                m_operating ?
                m_direction * m_speed :
                Vector2.zero;
        }

        private void OnCollision(CharacterBase primaryTarget = null)
        {
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_collisionSound);
            Terminate(primaryTarget);
        }

        private void HandleCollision(GameObject target)
        {
            CharacterBase character = target.GetComponentInParent<CharacterBase>();

            if (character)
            {
                EffectApplicationResult result = EffectDispatcher.Apply(m_source, new[] { character }, m_baseEffects, new()
                {
                    impactDataType = EEffectImpactDataType.Velocity,
                    impactData = m_direction
                });

                // We only apply the collision if at least one effect was applicable
                // If the effect missed (EEffectInteractionResult.ApplyFailed) we still want to apply the collision
                if (result.feed.Any(result => result != EEffectInteractionResult.NotApplicable))
                {
                    // Successfull collision with a valid character target 
                    OnCollision(character);
                }
            }
            else
            {
                // Successfull collision with anything else than a character target
                OnCollision();
            }
        }

        private bool TryColliding(GameObject target)
        {
            if (target.layer == LayerMask.NameToLayer(GameManager.Config.hitboxLayer))
            {
                if (m_operating && target != gameObject)
                {
                    HandleCollision(target);
                    return true;
                }
            }

            return false;
        }

        private bool IsProperCollider(int layer)
        {
            int layermask = GameManager.Config.collisionContactFilter.layerMask;
            return layermask == (layermask | (1 << layer));
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!TryColliding(collision.gameObject) && IsProperCollider(collision.gameObject.layer))
            {
                OnCollision();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TryColliding(collision.gameObject);
        }

        protected override Type GetDataBlockType() => typeof(ProjectileDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<ProjectileDataBlock>().direction = m_direction;
            block.As<ProjectileDataBlock>().speed = m_speed;
            block.As<ProjectileDataBlock>().remainingLifetime = m_remainingLifetime;
            block.As<ProjectileDataBlock>().operating = m_operating;
            block.As<ProjectileDataBlock>().source = m_source;
            block.As<ProjectileDataBlock>().baseEffects = m_baseEffects.ToArray();
            block.As<ProjectileDataBlock>().explosionEffects = m_explosionAdditionalEffects.ToArray();
            block.As<ProjectileDataBlock>().explosionRadius = m_explosionRadius;
            block.As<ProjectileDataBlock>().explosionIgnorePrimaryTarget = m_explosionBaseEffectsIgnorePrimaryTarget;
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            m_direction = block.As<ProjectileDataBlock>().direction;
            m_speed = block.As<ProjectileDataBlock>().speed;
            m_remainingLifetime = block.As<ProjectileDataBlock>().remainingLifetime;
            m_operating = block.As<ProjectileDataBlock>().operating;
            m_source = block.As<ProjectileDataBlock>().source;
            m_baseEffects = new List<IEffect>(block.As<ProjectileDataBlock>().baseEffects);
            m_explosionAdditionalEffects = new List<IEffect>(block.As<ProjectileDataBlock>().explosionEffects);
            m_explosionRadius = block.As<ProjectileDataBlock>().explosionRadius;
            m_explosionBaseEffectsIgnorePrimaryTarget = block.As<ProjectileDataBlock>().explosionIgnorePrimaryTarget;
        }
    }
}
