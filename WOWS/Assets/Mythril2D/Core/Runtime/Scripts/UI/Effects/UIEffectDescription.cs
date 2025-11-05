using TMPro;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIEffectDescription : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_text = null;

        [Header("Settings")]
        [SerializeField] private int m_maxLineCount = 1;

        public int maxLineCount => m_maxLineCount;

        public void Show(ITemporalEffect effect, float positionY)
        {
            transform.position = new(transform.position.x, positionY, transform.position.z);
            m_text.text = GenerateDescription(effect);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private static string GenerateDescription(ITemporalEffect effect)
        {
            EffectDescription effectDescription = effect.GenerateDescription();
            return effectDescription.details;
        }
    }
}
