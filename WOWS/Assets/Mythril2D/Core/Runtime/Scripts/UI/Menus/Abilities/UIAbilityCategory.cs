using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIAbilityCategory : MonoBehaviour, ISelectHandler
    {
        [Header("Settings")]
        [SerializeField] private Sprite m_selectedSprite;
        [SerializeField] private Sprite m_unselectedSprite;

        [Header("References")]
        [SerializeField] private Button m_button = null;
        [SerializeField] private Image m_icon = null;
        [SerializeField] private TextMeshProUGUI m_text = null;

        private EAbilityType m_category;

        public void SetCategory(EAbilityType category, int count)
        {
            m_category = category;
            m_icon.sprite = GameManager.Config.GetTermDefinition(m_category).icon;
            m_text.text = $"{GameManager.Config.GetTermDefinition(m_category).shortName} ({count})";
        }

        public void SetHighlight(bool value)
        {
            ((Image)m_button.targetGraphic).sprite = value ? m_selectedSprite : m_unselectedSprite;
        }

        public void SelectCategory()
        {
            SendMessageUpwards("OnAbilityCategorySelected", m_category, SendMessageOptions.RequireReceiver);
        }

        public void OnSelect(BaseEventData eventData)
        {
            SendMessageUpwards("OnAbilityCategoryHovered", m_category);
        }
    }
}
