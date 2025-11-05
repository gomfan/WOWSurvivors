using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class AIControllerDataBlock : ControllerDataBlock
    {
        public Vector3 initialPosition;
        public PersistableReference<CharacterBase> target;
        public float retargetCooldownTimer;
        public float attackCooldownTimer;
        public float timeSinceTargetLastSeen;
    }

    [Serializable]
    public class AIController : AController<CharacterBase>
    {
        [Header("References")]
        [SerializeField] private Entity m_master = null;

        [Header("Chase Settings")]
        [SerializeField, Min(1.0f)] private float m_detectionRadius = 5.0f;
        [SerializeField, Min(1.0f)] private float m_resetFromInitialPositionRadius = 10.0f;
        [SerializeField, Min(1.0f)] private float m_resetFromTargetDistanceRadius = 10.0f;
        [SerializeField, Min(0.5f)] private float m_targetOutOfRangeRetargetCooldown = 3.0f;
        [SerializeField, Min(0.1f)] private float m_soughtDistanceFromMasterTarget = 1.0f;
        [SerializeField, Min(0.1f)] private float m_soughtDistanceFromTarget = 1.0f;

        [Header("Steering Settings")]
        [SerializeField, Min(0.1f)] private float m_steeringDriftResponsiveness = 3.0f;
        [SerializeField, Min(0.1f)] private float m_timeBeforeResetAfterTargetSightLost = 3.0f;
        [SerializeField, Min(0.1f)] private float m_cannotSeeTargetRetargetCooldown = 1.0f;

        [Header("Attack Settings")]
        [SerializeField] public float m_attackTriggerRadius = 1.0f;
        [SerializeField] public float m_attackCooldown = 1.0f;

        private Transform transform => m_subject.transform;

        private Vector2 m_homePosition =>
            m_master ?
            (Vector2)m_master.transform.position :
            m_initialPosition;

        private Rigidbody2D m_rigidbody = null;
        private Vector2 m_initialPosition;
        private CharacterBase m_target = null;
        private float m_retargetCooldownTimer = 0.0f;
        private float m_attackCooldownTimer = 0.0f;
        private List<RaycastHit2D> m_castCollisions = new();

        private float[] m_interests = new float[8];
        private float[] m_dangers = new float[8];
        private float[] m_steering = new float[8];
        private Vector2 m_steeringAverageOutput = Vector2.zero;
        private Vector2 m_targetPosition = Vector2.zero;
        private Vector2 m_lerpedTargetDirection = Vector2.zero;
        private float m_timeSinceTargetLastSeen = 0.0f;

        private Vector2[] m_directions = new Vector2[8]
        {
            Vector2.up,
            new Vector2(0.5f, 0.5f).normalized,
            Vector3.right,
            new Vector2(0.5f, -0.5f).normalized,
            Vector2.down,
            new Vector2(-0.5f, -0.5f).normalized,
            Vector2.left,
            new Vector2(-0.5f, 0.5f).normalized,
        };

        protected override void OnInitialize()
        {
            m_rigidbody = m_subject.GetComponent<Rigidbody2D>();
            m_initialPosition = m_subject.transform.position;
            Debug.Assert(m_subject is CharacterBase, "AIController can only be attached to a CharacterBase");
            Debug.Assert(m_rigidbody != null, "No rigidbody found attached to this character");
        }

        protected override void OnStart()
        {
            m_subject.provokedEvent.AddListener(OnProvoked);
        }

        protected override void OnStop()
        {
            m_subject.provokedEvent.RemoveListener(OnProvoked);
        }

        public void SetMaster(Entity master, float? soughtDistanceFromMaster = null)
        {
            m_soughtDistanceFromMasterTarget = soughtDistanceFromMaster ?? m_soughtDistanceFromMasterTarget;
            m_master = master;
        }

        private void OnProvoked(CharacterBase source)
        {
            if (source && !m_target && m_retargetCooldownTimer == 0.0f && CombatSolver.IsJudiciousTarget(m_subject, source))
            {
                m_target = source;
                GameManager.NotificationSystem.targetDetected.Invoke(this, m_subject);
            }
        }

        private bool CanSee(CharacterBase other)
        {
            Vector2 targetPosition = other.transform.position;
            Vector2 currentPosition = transform.position;
            Vector2 directionToTarget = targetPosition - currentPosition;
            RaycastHit2D hit = Physics2D.Raycast(currentPosition, directionToTarget, Vector2.Distance(currentPosition, targetPosition), GameManager.Config.visibilityContactFilter.layerMask);
            return hit.collider == null;
        }

        private CharacterBase FindTarget()
        {
            RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, m_detectionRadius, Vector2.zero, 0.0f);

            foreach (RaycastHit2D hit in hits)
            {
                if (hit.transform.TryGetComponent(out CharacterBase potentialTarget) &&
                    CombatSolver.IsHostileTowards(m_subject, potentialTarget) &&
                    CanSee(potentialTarget))
                {
                    return potentialTarget;
                }
            }

            return null;
        }

        private void UpdateCooldowns()
        {
            if (m_retargetCooldownTimer > 0.0f)
            {
                m_retargetCooldownTimer = Math.Max(m_retargetCooldownTimer - Time.fixedDeltaTime, 0.0f);
            }

            if (m_attackCooldownTimer > 0.0f)
            {
                m_attackCooldownTimer = Math.Max(m_attackCooldownTimer - Time.fixedDeltaTime, 0.0f);
            }
        }

        private void TryToAttackTarget(float distanceToTarget)
        {
            if (CanSee(m_subject) && m_attackCooldownTimer == 0.0f && distanceToTarget < m_attackTriggerRadius)
            {
                // Find the first triggerable ability available on the character and fire it
                foreach (AbilityBase ability in m_subject.abilityInstances)
                {
                    if (ability is ITriggerableAbility)
                    {
                        m_subject.SetTargetDirection((m_target.transform.position - transform.position).normalized);
                        m_subject.FireAbility((ITriggerableAbility)ability);
                        m_attackCooldownTimer = m_attackCooldown;
                        break;
                    }
                }
            }
        }

        private void CheckIfTargetOutOfRange(float distanceToTarget)
        {
            float distanceToInitialPosition = Vector2.Distance(m_homePosition, transform.position);
            bool isTooFarFromInitialPosition = distanceToInitialPosition > m_resetFromInitialPositionRadius;
            bool isTooFarFromTarget = distanceToTarget > m_resetFromTargetDistanceRadius;

            if (isTooFarFromInitialPosition || isTooFarFromTarget)
            {
                StopChase(m_targetOutOfRangeRetargetCooldown);
            }
        }

        private void StopChase(float retargetCooldown)
        {
            m_retargetCooldownTimer = retargetCooldown;
            m_target = null;
        }

        private void ProcessChaseBehaviour(int index)
        {
            Vector2 direction = m_directions[index];

            Vector2 targetPosition = m_targetPosition;
            Vector2 currentPosition = transform.position;
            Vector2 directionToTarget = targetPosition - currentPosition;
            directionToTarget.Normalize();

            float angleToTargetDirection = Vector2.Angle(direction, directionToTarget);

            m_interests[index] = Math.Max(1.0f - (angleToTargetDirection / 90.0f), 0.0f);
        }

        private void ProcessAvoidBehaviour(int index)
        {
            Vector2 direction = m_directions[index];

            int count = m_rigidbody.Cast(
                    direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                    GameManager.Config.collisionContactFilter, // The settings that determine where a collision can occur on such as layers to collide with
                    m_castCollisions, // List of collisions to store the found collisions into after the Cast is finished
            1.0f); // The amount to cast equal to the movement plus an offset

            m_dangers[index] = count > 0 ? 1.0f - m_castCollisions[0].distance : 0.0f;
        }

        private void ProcessSteeringBehaviour(int index)
        {
            ProcessChaseBehaviour(index);
            ProcessAvoidBehaviour(index);

            m_steering[index] = m_interests[index] - m_dangers[index];
        }

        private void UpdateTargetPosition()
        {
            if (m_target)
            {
                // While we can see the target, store its last position, so if the AI looses sight of its target,
                // it will go to the last position the target was seen at.
                if (CanSee(m_target))
                {
                    m_targetPosition = (Vector2)m_target.transform.position;
                    m_timeSinceTargetLastSeen = 0.0f;
                }
                else
                {
                    m_timeSinceTargetLastSeen += Time.deltaTime;

                    if (m_timeSinceTargetLastSeen > m_timeBeforeResetAfterTargetSightLost)
                    {
                        StopChase(m_cannotSeeTargetRetargetCooldown);
                    }
                }
            }
            else
            {
                m_targetPosition = m_homePosition;
            }
        }

        protected override void OnFixedUpdate()
        {
            UpdateCooldowns();

            // If the target is dead or the character can't be provoked by the target anymore (alignment changed), reset the target
            if (m_target && (m_target.dead || !CombatSolver.IsJudiciousTarget(m_subject, m_target)))
            {
                m_target = null;
            }

            if (!m_target)
            {
                if (m_retargetCooldownTimer == 0.0f)
                {
                    m_target = FindTarget();
                    if (m_target)
                    {
                        GameManager.NotificationSystem.targetDetected.Invoke(this, m_target);
                    }
                }
            }
            else
            {
                float distanceToTarget = Vector2.Distance(m_target.transform.position, transform.position);

                TryToAttackTarget(distanceToTarget);
                CheckIfTargetOutOfRange(distanceToTarget);
            }

            UpdateTargetPosition();

            float soughtDistance =
                m_target ?
                m_soughtDistanceFromTarget :
                m_soughtDistanceFromMasterTarget;

            if (Vector2.Distance(transform.position, m_targetPosition) > soughtDistance)
            {
                m_steeringAverageOutput = Vector2.zero;

                // Process the steering behaviour for each direction
                for (int i = 0; i < 8; ++i)
                {
                    ProcessSteeringBehaviour(i);
                    m_steeringAverageOutput += m_directions[i] * m_steering[i];
                }

                m_steeringAverageOutput.Normalize();

                m_lerpedTargetDirection =
                    !m_subject.IsMoving() ?
                    m_steeringAverageOutput :
                    Vector2.Lerp(m_lerpedTargetDirection, m_steeringAverageOutput, Time.fixedDeltaTime * m_steeringDriftResponsiveness);

                m_subject.SetMovementDirection(m_lerpedTargetDirection.normalized);
            }
            else
            {
                m_subject.SetMovementDirection(Vector2.zero);

                if (m_subject.CanMove())
                {
                    m_subject.LookAtTarget(m_targetPosition); // Make sure the AI face its target
                }
            }
        }

        protected override void OnDrawGizmos()
        {
            for (int i = 0; i < 8; ++i)
            {
                Gizmos.color = m_steering[i] > 0.0f ? Color.green : Color.red;
                Gizmos.DrawRay(transform.position, m_directions[i] * Mathf.Abs(m_steering[i]));
            }

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, m_steeringAverageOutput);

            if (m_target)
            {
                Gizmos.color = CanSee(m_target) ? Color.cyan : Color.magenta;
                Gizmos.DrawLine(transform.position, m_target.transform.position);
            }
        }

        protected override Type GetDataBlockType() => typeof(AIControllerDataBlock);

        protected override void OnLoad(IControllerDataBlock block)
        {
            base.OnLoad(block);
            var aiControllerDataBlock = block.As<AIControllerDataBlock>();
            m_initialPosition = aiControllerDataBlock.initialPosition;
            m_target = aiControllerDataBlock.target;
            m_retargetCooldownTimer = aiControllerDataBlock.retargetCooldownTimer;
            m_attackCooldownTimer = aiControllerDataBlock.attackCooldownTimer;
            m_timeSinceTargetLastSeen = aiControllerDataBlock.timeSinceTargetLastSeen;
        }

        protected override void OnSave(IControllerDataBlock block)
        {
            base.OnSave(block);
            var aiControllerDataBlock = block.As<AIControllerDataBlock>();
            aiControllerDataBlock.initialPosition = m_initialPosition;
            aiControllerDataBlock.target = m_target;
            aiControllerDataBlock.retargetCooldownTimer = m_retargetCooldownTimer;
            aiControllerDataBlock.attackCooldownTimer = m_attackCooldownTimer;
            aiControllerDataBlock.timeSinceTargetLastSeen = m_timeSinceTargetLastSeen;
        }
    }
}
