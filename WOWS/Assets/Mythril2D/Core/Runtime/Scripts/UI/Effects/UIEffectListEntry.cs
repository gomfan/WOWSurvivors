using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public struct EffectHoveredEvent
    {
        public ITemporalEffect effect;
        public float listElementY;
    }

    public class UIEffectListEntry : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IDeselectHandler
    {
        [Header("References")]
        [SerializeField] private Image m_icon = null;
        [SerializeField] private TextMeshProUGUI m_text = null;
        [SerializeField] private Button m_button = null;

        private ITemporalEffect m_effect = null;

        public void SetEffect(ITemporalEffect effect)
        {
            m_effect = effect;
            m_icon.sprite = effect.info.Value.icon;
            m_text.text = effect.info.Value.shortName;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (m_button.IsInteractable())
            {
                m_button.Select();
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            SendMessageUpwards("OnEffectHovered", new EffectHoveredEvent()
            {
                effect = m_effect,
                listElementY = transform.position.y
            });
        }

        public void OnDeselect(BaseEventData eventData)
        {
            SendMessageUpwards("OnEffectNotHovered");
        }
    }
}
