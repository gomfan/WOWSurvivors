using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public struct GameplayActions
    {
        public InputAction move;
        public InputAction interact;
        public InputAction fireAbility1;
        public InputAction fireAbility2;
        public InputAction fireAbility3;
        public InputAction fireAbility4;
        public InputAction fireAbility5;
        public InputAction openGameMenu;
        public InputAction point;
    }

    public struct UIActions
    {
        public InputAction submit;
        public InputAction cancel;
        public InputAction click;
        public InputAction navigate;
        public InputAction point;
    }

    public enum EActionMap
    {
        Gameplay,
        UI,
        None
    }

    [RequireComponent(typeof(PlayerInput))]
    public class InputSystem : AGameSystem
    {
        public GameplayActions gameplay => m_gameplayActions;
        public UIActions ui => m_uiActions;
        public PlayerInput playerInput => m_playerInput;

        private PlayerInput m_playerInput = null;

        private GameplayActions m_gameplayActions;
        private UIActions m_uiActions;

        public override void OnSystemInit()
        {
            m_playerInput = GetComponent<PlayerInput>();
            SetupGameplayActions(m_playerInput.actions.FindActionMap("Gameplay"));
            SetupUIActions(m_playerInput.actions.FindActionMap("UI"));
        }

        public override void OnSystemStart()
        {
            GameManager.NotificationSystem.mapTransitionStarted.AddListener(LockInputs);
            GameManager.NotificationSystem.mapTransitionCompleted.AddListener(UnlockInputs);
        }

        public override void OnSystemStop()
        {
            GameManager.NotificationSystem.mapTransitionStarted.RemoveListener(LockInputs);
            GameManager.NotificationSystem.mapTransitionCompleted.RemoveListener(UnlockInputs);
        }

        public bool IsPointerActive(EActionMap map)
        {
            switch (map)
            {
                case EActionMap.Gameplay: return m_gameplayActions.point.activeControl != null;
                case EActionMap.UI: return m_uiActions.point.activeControl != null;
                default: return false;
            }
        }

        private void LockInputs()
        {
            m_playerInput.DeactivateInput();
        }

        private void UnlockInputs()
        {
            m_playerInput.ActivateInput();
        }

        public void SetActionMap(EActionMap actionMap)
        {
            m_playerInput.SwitchCurrentActionMap(actionMap.ToString());
        }

        private void SetupGameplayActions(InputActionMap actions)
        {
            m_gameplayActions = new GameplayActions
            {
                interact = actions.FindAction("Interact"),
                fireAbility1 = actions.FindAction("FireAbility1"),
                fireAbility2 = actions.FindAction("FireAbility2"),
                fireAbility3 = actions.FindAction("FireAbility3"),
                fireAbility4 = actions.FindAction("FireAbility4"),
                fireAbility5 = actions.FindAction("FireAbility5"),
                move = actions.FindAction("Move"),
                openGameMenu = actions.FindAction("OpenGameMenu"),
                point = actions.FindAction("Point")
            };
        }

        private void SetupUIActions(InputActionMap actions)
        {
            m_uiActions = new UIActions
            {
                submit = actions.FindAction("Submit"),
                cancel = actions.FindAction("Cancel"),
                click = actions.FindAction("Click"),
                navigate = actions.FindAction("Navigate"),
                point = actions.FindAction("Point")
            };
        }

        private void Update()
        {
            if (IsPointerActive(EActionMap.UI))
            {
                var eventSystem = EventSystem.current;

                if (eventSystem?.IsPointerOverGameObject() ?? false)
                {
                    var pointerEventData = new PointerEventData(eventSystem)
                    {
                        position = Mouse.current.position.ReadValue()
                    };

                    var results = new List<RaycastResult>();
                    eventSystem.RaycastAll(pointerEventData, results);

                    foreach (var result in results)
                    {
                        // If a selectable is found, try and select it
                        var selectable = result.gameObject.GetComponentInParent<Selectable>();
                        if (selectable != null && selectable.isActiveAndEnabled && (!selectable.targetGraphic || selectable.targetGraphic.raycastTarget))
                        {
                            if (selectable.gameObject != eventSystem.currentSelectedGameObject)
                            {
                                eventSystem.SetSelectedGameObject(selectable.gameObject);
                            }

                            return;
                        }

                        // Stop iterating when a graphic with raycastTarget is found
                        var graphic = result.gameObject.GetComponent<Graphic>();
                        if (graphic && graphic.raycastTarget)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
