using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UISettingsChannelVolume : UISettingsVolume
    {
        [Header("Settings")]
        [SerializeField] private EAudioChannel m_audioChannel;

        public EAudioChannel audioChannel => m_audioChannel;

        public void RegisterCallbacks(UnityAction<EAudioChannel, Button> decrease, UnityAction<EAudioChannel, Button> increase)
        {
            m_decreaseButton.onClick.AddListener(() => decrease(m_audioChannel, m_decreaseButton));
            m_increaseButton.onClick.AddListener(() => increase(m_audioChannel, m_increaseButton));
        }
    }
}
