using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;
using MackySoft.SerializeReferenceExtensions;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    public enum EDirection
    {
        Left,
        Right,
        Up,
        Down,
        None,
        Default = Right
    }

    public enum ELookAtDirectionUpdateStrategy
    {
        MovementBased,
        TargetBased
    }

    [Serializable]
    public class MovableDataBlock : EntityDataBlock
    {
        public Vector2 lookAtDirection;
        [SerializeReference, SubclassSelector] public IControllerDataBlock controllerData;
    }

    [Serializable]
    public struct MoveOrder
    {
        public Vector2 targetPosition;
        public float? speedOverride;
        public TaskCompletionSource<bool> task;
    }

    [Serializable]
    public struct PushOrder
    {
        public Vector2 direction;
        public float intensity;
        public float resistance;
        public SerializableHashSet<GameObject> collisionSet;

        public void SetIntensity(float intensity)
        {
            this.intensity = intensity;
        }

        public bool AddCollision(GameObject gameObject) => collisionSet.Add(gameObject);
    }

    public abstract class Movable : Entity, IAnimationMessageReceiver
    {
        [Header("References")]
        [SerializeField] private Rigidbody2D m_rigidbody = null;

        [Header("Movement Settings")]
        [SerializeField] private float m_moveSpeed = 4.0f;
        [SerializeField] private float m_pushIntensityScale = 1.0f;
        [SerializeField] private float m_pushResistanceScale = 1.0f;
        [SerializeField] private bool m_disableAllMovements = false;
        [SerializeField] private bool m_canMoveDuringDeath = false;
        [SerializeField] protected ELookAtDirectionUpdateStrategy m_lookAtDirectionUpdateStrategy = ELookAtDirectionUpdateStrategy.MovementBased;

        [Header("Controller Settings")]
        [SerializeReference, SubclassSelector] protected IController m_controller = null;

        [Header("Animation Settings")]
        [SerializeReference, SubclassSelector] protected IAnimationStrategy m_animationStrategy = null;

        public IController controller => m_controller;
        public UnityEvent teleported => m_teleported;
        public UnityEvent<Vector2> targetDirectionChangedEvent => m_targetDirectionChangedEvent;

        protected bool m_destroyOnDeath = true;
        protected Vector2 m_lookAtDirection = Vector2.zero;
        private UnityEvent<Vector2> m_targetDirectionChangedEvent = new();
        private Vector2? m_targetDirectionOverride = null;
        private List<RaycastHit2D> m_castCollisions = new();
        private HashSet<GameObject> m_castCollisionSet = new();
        private Vector2 m_movementDirection;
        private Vector2 m_lastSuccessfulMovement;
        private UnityEvent m_teleported = new();
        private MoveOrder? m_moveOrder = null;
        private PushOrder? m_pushOrder = null;

        protected virtual void Awake()
        {
            Debug.Assert(m_rigidbody, ErrorMessages.InspectorMissingComponentReference<Rigidbody2D>());

            m_animationStrategy?.deathAnimationEnded.AddListener(OnDeathAnimationEnd);

            m_animationStrategy?.Initialize();
            m_controller?.Initialize(this);
        }

        public virtual void Revive()
        {
            m_controller?.Start();
            MarkAsNotDestroyed();
        }

        public void DispatchAnimationMessage(string message, SendMessageOptions options)
        {
            Debug.Assert(options == SendMessageOptions.DontRequireReceiver || m_animationStrategy != null, $"Animation message '{message}' was not handled: no animation strategy is set.");
            m_animationStrategy?.OnMessageReceived(message, options);
        }

        protected virtual AudioClipResolver GetDeathAudio() => null;

        public virtual void Kill()
        {
            m_controller?.Stop();
            MarkAsDestroyed();
            PlayDeathAudio();

            m_movementDirection = Vector2.zero;

            if (!(m_animationStrategy?.PlayDeathAnimation() ?? false))
            {
                OnDeath();
            }
        }

        public void TeleportTo(Vector3 position)
        {
            transform.position = position;
            m_teleported.Invoke();
        }

        private void PlayDeathAudio()
        {
            AudioClipResolver deathAudio = GetDeathAudio();

            if (deathAudio)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(deathAudio);
            }
        }

        protected virtual void OnDeath()
        {
            if (m_destroyOnDeath)
            {
                Destroy();
            }
        }

        protected virtual void OnDestroy()
        {
            m_controller?.Terminate();
        }

        protected virtual void OnDeathAnimationEnd() => OnDeath();

        private bool IsInWall(Vector2? direction = null, float speed = 0.0f, float deltaTime = 0.0f)
        {
            int collisions = m_rigidbody.Cast(
                direction ?? Vector2.zero,
                GameManager.Config.collisionContactFilter,
                m_castCollisions,
                speed * deltaTime + Constants.CollisionOffset
            );

            foreach (RaycastHit2D hit in m_castCollisions)
            {
                m_castCollisionSet.Add(hit.collider.gameObject);
            }

            return collisions > 0;
        }

        private bool IsMovementValid(Vector2 direction, float speed, float deltaTime) => !IsInWall(direction, speed, deltaTime);

        private Vector2? FindNearestValidPosition(float maxDistance, int attempts = 16)
        {
            Debug.Assert(IsInWall(), "Movable entity not in a wall, FindNearestValidPosition() shouldn't be called.");

            if (maxDistance == 0.0f)
            {
                return null;
            }

            return TryFindValidPosition(m_rigidbody.position, maxDistance, attempts);
        }

        private Vector2? TryFindValidPosition(Vector2 currentPosition, float maxDistance, int attempts)
        {
            Vector2[] directions = new Vector2[]
            {
                Vector2.up, Vector2.right, Vector2.down, Vector2.left,
                new Vector2(1, 1).normalized, new Vector2(1, -1).normalized,
                new Vector2(-1, -1).normalized, new Vector2(-1, 1).normalized
            };

            for (int i = 1; i <= attempts; i++)
            {
                float distance = i / (float)attempts * maxDistance;

                foreach (Vector2 dir in directions)
                {
                    Vector2 testPosition = currentPosition + dir * distance;

                    if (IsValidPosition(testPosition))
                    {
                        return testPosition;
                    }
                }
            }
            return null;
        }

        private bool IsValidPosition(Vector2 testPosition)
        {
            Vector2 originalPosition = m_rigidbody.position;
            m_rigidbody.position = testPosition;
            bool isValid = !IsInWall();
            m_rigidbody.position = originalPosition;
            return isValid;
        }

        protected virtual void OnStuckInAWall() { }

        private void OnEnable() => m_controller?.Start();
        private void OnDisable() => m_controller?.Stop();
        private void OnDrawGizmos() => m_controller?.DrawGizmos();
        protected virtual void Update() => m_controller?.Update();
        protected virtual float CalculateMoveSpeed() => m_moveSpeed;

        protected virtual void FixedUpdate()
        {
            m_controller?.FixedUpdate();

            // Convenient if we want to disable all movements/physics for a character
            // For instance, a character that is not supposed to move or be pushed,
            // and is positioned on top of a suposedly collidable object.
            if (!m_disableAllMovements)
            {
                HandleWallCollision();
                HandleMovement();
                HandlePush();
            }
        }

        private void HandleWallCollision()
        {
            if (IsInWall())
            {
                float maxTeleportDistance = GameManager.Config.maxTeleportDistanceWhenStuckInWall;
                Vector2? nearestValidPosition = FindNearestValidPosition(maxTeleportDistance);
                if (nearestValidPosition.HasValue)
                {
                    m_rigidbody.position = nearestValidPosition.Value;
                    Debug.Log($"Movable '{gameObject.name}' moved from inside a collider to a valid position within {maxTeleportDistance} units.");
                }
                else
                {
                    Debug.LogWarning($"Movable '{gameObject.name}' is stuck and could not find a valid position within {maxTeleportDistance} units.");
                    OnStuckInAWall();
                }
            }
        }

        public void SetMovementDirection(Vector2 direction)
        {
            m_movementDirection = direction;
        }

        private void HandleMovement()
        {
            if (m_moveOrder.HasValue)
            {
                ExecuteMoveOrder();
            }
            else
            {
                MoveInDirection(m_movementDirection, CalculateMoveSpeed());
            }
        }

        public virtual bool CanUpdateTargetDirection() => !IsMarkedAsDestroyed();
        public virtual bool CanMove() => !m_disableAllMovements && !IsPushed() && (!IsMarkedAsDestroyed() || m_canMoveDuringDeath);

        private void ExecuteMoveOrder()
        {
            Vector2 direction = (m_moveOrder.Value.targetPosition - (Vector2)transform.position).normalized;
            float speed = m_moveOrder.Value.speedOverride ?? CalculateMoveSpeed();

            if (!MoveInDirection(direction, speed, true))
            {
                m_moveOrder.Value.task.SetResult(false);
                m_moveOrder = null;
            }
            else
            {
                if (Vector2.Distance(m_rigidbody.position, m_moveOrder.Value.targetPosition) < Constants.AcceptableDistanceFromTarget)
                {
                    m_moveOrder.Value.task.SetResult(true);
                    m_moveOrder = null;
                }
            }
        }

        private bool MoveInDirection(Vector2 direction, float moveSpeed, bool force = false)
        {
            BeginMove();

            if ((force || CanMove()) && moveSpeed > 0.0f && direction.magnitude > 0.0f)
            {
                // Try to move on X and Y
                if (!TryMove(direction, moveSpeed))
                {
                    // Try to move only on Y
                    if (!TryMove(new Vector2(direction.x, 0), moveSpeed))
                    {
                        // Try to move only on X
                        TryMove(new Vector2(0, direction.y), moveSpeed);
                    }
                }

                if (m_lookAtDirectionUpdateStrategy == ELookAtDirectionUpdateStrategy.MovementBased)
                {
                    SetLookAtDirection(direction);
                }
            }

            EndMove();

            m_animationStrategy?.SetMovement(m_lastSuccessfulMovement);

            return m_castCollisionSet.Count == 0;
        }

        private void BeginMove()
        {
            m_lastSuccessfulMovement = Vector2.zero;
            m_castCollisionSet.Clear();
        }

        private void EndMove(bool sendCollisionNotifications = true)
        {
            if (sendCollisionNotifications && m_castCollisionSet.Count > 0)
            {
                foreach (GameObject go in m_castCollisionSet)
                {
                    CollisionDispatcher.RegisterCollision(this, go);
                }
            }
        }

        private bool TryMove(Vector2 direction, float speed)
        {
            if (direction.magnitude > 0.0f)
            {
                if (IsMovementValid(direction, speed, Time.fixedDeltaTime))
                {
                    m_lastSuccessfulMovement = direction * speed;
                    m_rigidbody.MovePosition(m_rigidbody.position + direction * speed * Time.fixedDeltaTime);
                    return true;
                }
            }

            return false;
        }

        public TaskCompletionSource<bool> MoveTo(Vector2 destination, float? speedOverride = null)
        {
            if (m_moveOrder.HasValue)
            {
                m_moveOrder.Value.task.SetCanceled();
            }

            m_moveOrder = new()
            {
                targetPosition = destination,
                speedOverride = speedOverride,
                task = new()
            };

            return m_moveOrder.Value.task;
        }

        public bool IsMovingUp() => m_movementDirection.y > 0.0f || (IsPushed() && m_pushOrder.Value.direction.y > 0.0f);
        public bool IsMovingDown() => m_movementDirection.y < 0.0f || (IsPushed() && m_pushOrder.Value.direction.y < 0.0f);
        public bool IsMovingLeft() => m_movementDirection.x < 0.0f || (IsPushed() && m_pushOrder.Value.direction.x < 0.0f);
        public bool IsMovingRight() => m_movementDirection.x > 0.0f || (IsPushed() && m_pushOrder.Value.direction.x > 0.0f);
        public bool IsMoving() => m_lastSuccessfulMovement.magnitude > 0.0f;

        public void LookAtTarget(Transform target)
        {
            float3 direction = target.position - transform.position;
            SetLookAtDirection(direction.xy);
        }

        public void LookAtTarget(Vector2 targetPosition)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            SetLookAtDirection(direction);
        }

        public void SetLookAtDirection(Vector2 direction)
        {
            if (direction.magnitude > 0.0f && direction != m_lookAtDirection)
            {
                // If no target direction override is set, update the target direction to match the look-at direction.
                // This ensures the character's target direction aligns with its facing direction when no specific target direction is provided (i.e. when using a controller).
                if (!m_targetDirectionOverride.HasValue)
                {
                    m_targetDirectionChangedEvent.Invoke(direction);
                    m_animationStrategy.SetTargetDirection(direction);
                }

                m_lookAtDirection = direction;
                m_animationStrategy?.SetLookAtDirection(direction);
            }
        }

        public void SetTargetDirection(Vector2 direction)
        {
            if (CanUpdateTargetDirection())
            {
                m_targetDirectionOverride = direction;
                m_targetDirectionChangedEvent.Invoke(direction);
                m_animationStrategy?.SetTargetDirection(direction);

                if (m_lookAtDirectionUpdateStrategy == ELookAtDirectionUpdateStrategy.TargetBased)
                {
                    SetLookAtDirection(direction);
                }
            }
        }

        public void ResetTargetDirection()
        {
            m_targetDirectionOverride = null;
        }

        public Vector2 GetTargetDirection()
        {
            return m_targetDirectionOverride ?? m_lookAtDirection;
        }

        private void ExecutePushOrder()
        {
            BeginMove();

            PushOrder pushOrder = m_pushOrder.Value;

            if (pushOrder.intensity > 0.2f)
            {
                bool movementSuccess = TryMove(pushOrder.direction, pushOrder.intensity);
                pushOrder.intensity = math.lerp(pushOrder.intensity, 0.0f, Time.fixedDeltaTime * pushOrder.resistance);
                m_pushOrder = pushOrder;

                if (!movementSuccess)
                {
                    foreach (GameObject go in m_castCollisionSet)
                    {
                        if (m_pushOrder.Value.AddCollision(go))
                        {
                            CollisionDispatcher.RegisterCollision(this, go);
                        }
                    }
                }
            }
            else
            {
                m_pushOrder = null;
            }

            EndMove(false);
        }

        private void HandlePush()
        {
            if (m_pushOrder.HasValue)
            {
                ExecutePushOrder();
            }
        }

        public bool IsPushable() => m_pushIntensityScale > 0.0f;
        public bool IsPushed() => m_pushOrder.HasValue;
        public void InterruptPush() => m_pushOrder = null;

        protected bool TryPush(DamageInputDescriptor damageInput, Vector2 velocity)
        {
            bool isSelfTargeted = damageInput.source is CharacterDamageSource characterDamage && characterDamage.character == this;

            bool disablePush =
                isSelfTargeted ||
                (damageInput.silent && !GameManager.Config.allowPushOnSilentHit) ||
                (damageInput.flags == EDamageFlag.None && !GameManager.Config.allowPushOnRegularHit) ||
                (damageInput.flags.HasFlag(EDamageFlag.Critical) && !GameManager.Config.allowPushOnCriticalHit) ||
                (damageInput.flags.HasFlag(EDamageFlag.Miss) && !GameManager.Config.allowPushOnMissedHit);


            if (!disablePush)
            {
                Push(velocity.normalized);
                return true;
            }

            return false;
        }

        public void Push(Vector2 direction, float intensity = 5.0f, float resistance = 10.0f)
        {
            if (IsPushable())
            {
                m_rigidbody.linearVelocity = Vector2.zero;

                m_pushOrder = new()
                {
                    direction = direction,
                    intensity = intensity * m_pushIntensityScale,
                    resistance = resistance * m_pushResistanceScale,
                    collisionSet = new()
                };
            }
        }

        protected override Type GetDataBlockType() => typeof(MovableDataBlock);

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            MovableDataBlock movableBlock = block.As<MovableDataBlock>();
            movableBlock.lookAtDirection = m_lookAtDirection;
            movableBlock.controllerData = m_controller?.CreateDataBlock();
        }

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            var movableBlock = block.As<MovableDataBlock>();
            SetLookAtDirection(movableBlock.lookAtDirection);
            m_controller?.LoadDataBlock(movableBlock.controllerData);
        }
    }
}
