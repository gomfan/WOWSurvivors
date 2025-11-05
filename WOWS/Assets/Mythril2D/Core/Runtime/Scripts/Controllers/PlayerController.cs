using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PlayerController : AController<CharacterBase>
    {
        [Header("General")]
        [SerializeField] private Transform m_overrideInteractionPivot = null;
        [SerializeField] private float m_interactionDistance = 0.75f;
        [SerializeField] private bool m_castAbilitiesInPointerDirection = false;

        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_interactionSound;

        public GameObject interactionTarget => m_interactionTarget;
        private Transform m_interactionPivot => m_overrideInteractionPivot != null ? m_overrideInteractionPivot : m_subject.transform;
        private GameObject m_interactionTarget = null;
        private bool m_interactedThisFrame = false;

        protected override void OnInitialize()
        {
            Debug.Assert(m_subject is CharacterBase, "PlayerController can only be attached to a CharacterBase");
        }

        protected override void OnStart()
        {
            GameManager.InputSystem.gameplay.interact.performed += OnInteract;
            GameManager.InputSystem.gameplay.fireAbility1.performed += OnFireAbility1;
            GameManager.InputSystem.gameplay.fireAbility2.performed += OnFireAbility2;
            GameManager.InputSystem.gameplay.fireAbility3.performed += OnFireAbility3;
            GameManager.InputSystem.gameplay.fireAbility4.performed += OnFireAbility4;
            GameManager.InputSystem.gameplay.fireAbility5.performed += OnFireAbility5;
            GameManager.InputSystem.gameplay.move.performed += OnMove;
            GameManager.InputSystem.gameplay.move.canceled += OnStoppedMoving;
            GameManager.InputSystem.gameplay.openGameMenu.performed += OnOpenGameMenu;
        }

        protected override void OnStop()
        {
            GameManager.InputSystem.gameplay.interact.performed -= OnInteract;
            GameManager.InputSystem.gameplay.fireAbility1.performed -= OnFireAbility1;
            GameManager.InputSystem.gameplay.fireAbility2.performed -= OnFireAbility2;
            GameManager.InputSystem.gameplay.fireAbility3.performed -= OnFireAbility3;
            GameManager.InputSystem.gameplay.fireAbility4.performed -= OnFireAbility4;
            GameManager.InputSystem.gameplay.fireAbility5.performed -= OnFireAbility5;
            GameManager.InputSystem.gameplay.move.performed -= OnMove;
            GameManager.InputSystem.gameplay.move.canceled -= OnStoppedMoving;
            GameManager.InputSystem.gameplay.openGameMenu.performed -= OnOpenGameMenu;

            m_subject.ResetTargetDirection();
        }

        protected override void OnUpdate()
        {
            m_interactedThisFrame = false;
            m_interactionTarget = GetInteractibleObject();

            if (m_castAbilitiesInPointerDirection && GameManager.InputSystem.IsPointerActive(EActionMap.Gameplay))
            {
                Vector2 pointerPosition = GameManager.InputSystem.gameplay.point.ReadValue<Vector2>();
                Vector2 mousePosWorld = Camera.main.ScreenToWorldPoint(pointerPosition);
                Vector2 characterToMouseDir = (mousePosWorld - (Vector2)m_interactionPivot.transform.position).normalized;
                m_subject.SetTargetDirection(characterToMouseDir);
            }
            else
            {
                m_subject.ResetTargetDirection();
            }
        }

        protected ActiveAbilitySheet GetAbilityAtIndex(int index)
        {
            return GameManager.Player.equippedAbilities[index];
        }

        private GameObject GetInteractibleObject()
        {
            if (m_subject.Can(EActionFlags.Interact))
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(m_interactionPivot.position, m_interactionDistance, LayerMask.GetMask(GameManager.Config.interactionLayer));
                Array.Sort(colliders, (x, y) =>
                {
                    return Vector3.Distance(m_interactionPivot.position, x.transform.position).CompareTo(
                        Vector3.Distance(m_interactionPivot.position, y.transform.position));
                });
                foreach (Collider2D collider in colliders)
                {
                    Vector3 a = m_subject.GetTargetDirection();
                    Vector3 b = collider.transform.position + new Vector3(collider.offset.x, collider.offset.y, 0) - m_interactionPivot.position;
                    if (Vector3.Dot(a, b) > 0)
                    {
                        return collider.gameObject;
                    }
                }
            }

            return null;
        }

        private void OnInteract()
        {
            if (!m_running) return;

            GameObject interactionTarget = GetInteractibleObject();

            if (interactionTarget)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_interactionSound);
                interactionTarget.SendMessageUpwards("OnInteract", m_subject);
                m_interactedThisFrame = true;
            }
        }

        private void OnOpenGameMenu(InputAction.CallbackContext context)
        {
            if (!m_running) return;
            if (!m_subject.Can(EActionFlags.Interact)) return;

            GameManager.NotificationSystem.gameMenuRequested.Invoke(null);
        }

        private void OnMove(InputAction.CallbackContext context)
        {
            if (!m_running) return;

            Vector2 direction = context.ReadValue<Vector2>();
            m_subject.SetMovementDirection(direction);
        }

        private void OnStoppedMoving(InputAction.CallbackContext context)
        {
            if (!m_running) return;

            m_subject.SetMovementDirection(Vector2.zero);
        }

        private void OnInteract(InputAction.CallbackContext context) => OnInteract();

        private void OnFireAbility1(InputAction.CallbackContext context) => FireAbilityAtIndex(0);
        private void OnFireAbility2(InputAction.CallbackContext context) => FireAbilityAtIndex(1);
        private void OnFireAbility3(InputAction.CallbackContext context) => FireAbilityAtIndex(2);
        private void OnFireAbility4(InputAction.CallbackContext context) => FireAbilityAtIndex(3);
        private void OnFireAbility5(InputAction.CallbackContext context) => FireAbilityAtIndex(4);

        private void FireAbilityAtIndex(int i)
        {
            if (!m_running || m_interactedThisFrame) return;

            ActiveAbilitySheet selectedAbility = GetAbilityAtIndex(i);

            if (selectedAbility != null)
            {
                EAbilityFireCheckResult result = m_subject.FireAbility(selectedAbility, out ITriggerableAbility triggerableAbility);

                if (result != EAbilityFireCheckResult.Valid)
                {
                    GameManager.NotificationSystem.playerFireFailed.Invoke(triggerableAbility, result);
                }
            }
        }
    }
}
