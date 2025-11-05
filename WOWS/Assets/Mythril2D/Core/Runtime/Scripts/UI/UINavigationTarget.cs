using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UINavigationTarget : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler
    {
        [SerializeField] private AudioClipResolver m_navigationSelectSoundOverride = null;
        [SerializeField] private AudioClipResolver m_pointerSelectSoundOverride = null;
        [SerializeField] private AudioClipResolver m_submitSoundOverride = null;

        private AudioClipResolver navigationSelectSound => m_navigationSelectSoundOverride ?? GameManager.Config.navigationSelectSound;
        private AudioClipResolver pointerSelectSound => m_pointerSelectSoundOverride ?? GameManager.Config.pointerSelectSound;
        private AudioClipResolver submitSound => m_submitSoundOverride ?? GameManager.Config.submitSound;

        private void OnSelectWithPointer()
        {
            if (pointerSelectSound)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(pointerSelectSound);
            }
        }

        private void OnSelectWithNavigation()
        {
            if (navigationSelectSound)
            {
                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(navigationSelectSound);
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (eventData is AxisEventData)
            {
                OnSelectWithNavigation();
            }
            else
            {
                OnSelectWithPointer();
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            GameObject selected = eventData.selectedObject;

            if (selected != null)
            {
                Selectable selectable = selected.GetComponent<Selectable>();

                if (selectable != null && selectable.interactable && submitSound)
                {
                    GameManager.NotificationSystem.audioPlaybackRequested.Invoke(submitSound);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData) => OnSubmit(eventData);
    }
}