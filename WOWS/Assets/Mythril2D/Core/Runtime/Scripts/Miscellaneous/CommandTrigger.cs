using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public class CommandTrigger : MonoBehaviour
    {
        public enum EActivationEvent
        {
            OnStart,
            OnEnable,
            OnDisable,
            OnFixedUpdate,
            OnUpdate,
            OnPlayerEnterTrigger,
            OnPlayerExitTrigger,
            OnPlayerCollision,
            OnPlayerInteract,
            OnConditionStateChanged
        }

        [Header("Requirements")]
        [SerializeField] private EActivationEvent m_activationEvent;
        [SerializeReference, SubclassSelector] private ICondition m_condition;

        [Header("Actions")]
        [SerializeReference, SubclassSelector] private ICommand m_toExecute;

        [Header("Settings")]
        [SerializeField] private int m_frameDelay = 0;

        private void OnEnable()
        {
            AttemptExecution(EActivationEvent.OnEnable);

            if (m_activationEvent == EActivationEvent.OnConditionStateChanged)
            {
                m_condition.StartListening(OnConditionStateChanged);
            }
        }

        private void OnDisable()
        {
            AttemptExecution(EActivationEvent.OnDisable);

            if (m_activationEvent == EActivationEvent.OnConditionStateChanged)
            {
                m_condition.StopListening();
            }
        }

        private void OnConditionStateChanged() => AttemptExecution(EActivationEvent.OnConditionStateChanged);

        private void AttemptExecution(EActivationEvent currentEvent, GameObject go = null)
        {
            // Avoid execution to occur if the player system is not present.
            // Not ideal, but it works for now.
            if (GameManager.Exists() && GameManager.TryGetSystem(out PlayerSystem playerSystem))
            {
                if (!playerSystem.PlayerInstance || !playerSystem.PlayerInstance.gameObject || !GameManager.Player)
                {
                    return;
                }
            }

            if (currentEvent == m_activationEvent && (!go || go == GameManager.Player.gameObject) && (m_condition?.Evaluate() ?? true))
            {
                if (m_frameDelay <= 0)
                {
                    Execute();
                }
                else
                {
                    StartCoroutine(CoroutineHelpers.ExecuteInXFrames(m_frameDelay, Execute));
                }
            }
        }

        private void AttemptTriggerExecution(EActivationEvent currentEvent, Collider2D collider = null)
        {
            if (collider == null || collider.gameObject == null)
            {
                return;
            }

            AttemptExecution(currentEvent, collider.gameObject);
        }

        private void Execute()
        {
            m_toExecute?.Execute();
        }

        private void Start() => AttemptExecution(EActivationEvent.OnStart);
        private void FixedUpdate() => AttemptExecution(EActivationEvent.OnFixedUpdate);
        private void Update() => AttemptExecution(EActivationEvent.OnUpdate);
        private void OnTriggerEnter2D(Collider2D collider) => AttemptTriggerExecution(EActivationEvent.OnPlayerEnterTrigger, collider);
        private void OnTriggerExit2D(Collider2D collider) => AttemptTriggerExecution(EActivationEvent.OnPlayerExitTrigger, collider);
        private void OnMovableCollision(Movable movable) => AttemptExecution(EActivationEvent.OnPlayerCollision, movable.gameObject);
        private void OnInteract(CharacterBase sender) => AttemptExecution(EActivationEvent.OnPlayerInteract, sender.gameObject);
    }
}