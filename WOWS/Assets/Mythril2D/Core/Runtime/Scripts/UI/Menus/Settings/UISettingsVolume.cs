using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UISettingsVolume : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TextMeshProUGUI m_value = null;
        [SerializeField] protected Button m_decreaseButton;
        [SerializeField] protected Button m_increaseButton;

        public void UpdateUI(int volume, string suffix = "")
        {
            m_value.text = $"{volume}{suffix}";
        }

        // Used for selection
        public Button GetFirstButton() => m_decreaseButton;
    }
}
