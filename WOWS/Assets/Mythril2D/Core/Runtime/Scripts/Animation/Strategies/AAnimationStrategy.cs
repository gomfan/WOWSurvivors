using System;
using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public abstract class AAnimationStrategy : IAnimationStrategy
    {
        [Header("Movable Animation Parameters")]
        [SerializeField] private string m_isMovingAnimationParameter = "isMoving";

        [Header("Character Animation Parameters")]
        [SerializeField] private string m_hitAnimationParameter = "hit";
        [SerializeField] private string m_deathAnimationParameter = "death";
        [SerializeField] private string m_invincibleAnimationParameter = "invincible";

        [Header("Character Animation Messages")]
        [SerializeField] private string m_invincibilityAnimationStartMessage = "OnInvincibleAnimationStart";
        [SerializeField] private string m_invincibilityAnimationStopMessage = "OnInvincibleAnimationStop";
        [SerializeField] private string m_deathAnimationStartMessage = "OnDeathAnimationStart";
        [SerializeField] private string m_deathAnimationStopMessage = "OnDeathAnimationStop";

        [Header("General Settings")]
        [SerializeField] private bool m_dynamicSortingOrder = true;
        [SerializeField] private int m_orderInLayerOverrideWhenMovingUp = 2;

        [Header("References")]
        [SerializeField] protected Animator m_animator = null;
        [SerializeField] protected SpriteRenderer m_spriteRenderer = null;

        public UnityEvent deathAnimationStarted => m_deathAnimationStarted;
        public UnityEvent deathAnimationEnded => m_deathAnimationEnded;

        private bool m_hasDeathAnimation = false;
        private bool m_hasHitAnimation = false;
        private bool m_hasInvincibleAnimation = false;
        private bool m_hasMovingAnimation;
        private UnityEvent m_deathAnimationStarted = new();
        private UnityEvent m_deathAnimationEnded = new();
        private bool m_invincibleAnimationPlaying = false;

        private int m_defaultOrderInLayer = 0;

        public virtual void Initialize()
        {
            Debug.Assert(m_animator, ErrorMessages.InspectorMissingComponentReference<Animator>());
            Debug.Assert(m_spriteRenderer, ErrorMessages.InspectorMissingComponentReference<SpriteRenderer>());

            m_defaultOrderInLayer = m_spriteRenderer.sortingOrder;

            CheckForAnimations();
        }

        public void Resume()
        {
            m_animator.enabled = true;

            // Workaround to fix broken animations after a revive or respawn
            // For more information, see: https://issuetracker.unity3d.com/issues/animator-does-not-continue-animation-indefinitely-when-toggling-animator-dot-enabled-through-code
            m_animator.Rebind(); // added these next two lines
            m_animator.Update(0); // to fix broken animations
        }

        public void Pause() => m_animator.enabled = false;

        public virtual void OnMessageReceived(string message, SendMessageOptions options)
        {
            if (message == m_invincibilityAnimationStartMessage)
            {
                m_invincibleAnimationPlaying = true;
            }
            else if (message == m_invincibilityAnimationStopMessage)
            {
                m_invincibleAnimationPlaying = false;
            }
            else if (message == m_deathAnimationStartMessage)
            {
                m_deathAnimationStarted.Invoke();
            }
            else if (message == m_deathAnimationStopMessage)
            {
                m_deathAnimationEnded.Invoke();
            }
            else
            {
                Debug.Assert(options != SendMessageOptions.RequireReceiver, $"Received unknown message '{message}' with option 'RequireReceiver' but no receiver was found.");
            }
        }

        protected virtual void CheckForAnimations()
        {
            if (m_animator)
            {
                m_hasHitAnimation = AnimationUtils.HasParameter(m_animator, m_hitAnimationParameter);
                m_hasDeathAnimation = AnimationUtils.HasParameter(m_animator, m_deathAnimationParameter);
                m_hasInvincibleAnimation = AnimationUtils.HasParameter(m_animator, m_invincibleAnimationParameter);
                m_hasMovingAnimation = AnimationUtils.HasParameter(m_animator, m_isMovingAnimationParameter);
            }
        }

        public virtual void SetLookAtDirection(Vector2 direction) { }

        public virtual void SetTargetDirection(Vector2 direction)
        {
            if (m_dynamicSortingOrder)
            {
                // Set the sorting order based on the direction:
                // When moving up, the sprite should be rendered on top of everything (hands, weapon, etc.)
                m_spriteRenderer.sortingOrder = direction.y > 0.0f ? m_orderInLayerOverrideWhenMovingUp : m_defaultOrderInLayer;
            }
        }

        public virtual void SetMovement(Vector2 speed)
        {
            if (m_hasMovingAnimation)
            {
                m_animator.SetBool(m_isMovingAnimationParameter, speed.magnitude > 0.0f);
            }
        }

        public virtual bool PlayHitAnimation()
        {
            if (m_animator && m_hasHitAnimation)
            {
                m_animator.SetTrigger(m_hitAnimationParameter);
                return true;
            }

            return false;
        }

        public virtual bool PlayDeathAnimation()
        {
            if (m_animator && m_hasDeathAnimation)
            {
                m_animator.SetTrigger(m_deathAnimationParameter);
                return true;
            }

            return false;
        }

        public virtual bool PlayInvincibleAnimation()
        {
            if (m_animator && m_hasInvincibleAnimation)
            {
                m_animator.SetTrigger(m_invincibleAnimationParameter);
                return true;
            }

            return false;
        }

        public virtual bool IsInvincibleAnimationPlaying()
        {
            return m_invincibleAnimationPlaying;
        }
    }
}