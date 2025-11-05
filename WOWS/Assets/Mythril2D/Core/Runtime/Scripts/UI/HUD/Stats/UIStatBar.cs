using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIStatBar : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_label = null;
        [SerializeField] private Slider m_slider = null;
        [SerializeField] private TextMeshProUGUI m_sliderText = null;

        [Header("General Settings")]
        [SerializeField] private EStat m_stat;

        [Header("Visual Settings")]
        [SerializeField] private bool m_shakeOnDecrease = false;
        [SerializeField] private float m_shakeAmplitude = 5.0f;
        [SerializeField] private float2 m_shakeFrequency = new(30.0f, 25.0f);
        [SerializeField] private float m_shakeDuration = 0.2f;

        private CharacterBase m_target = null;
        private ShakeHandler? m_shakeHandler = null;

        // Hack-ish way to make sure we don't start shaking before the UI is fully initialized,
        // which usually take one frame because of Unity's layout system
        const int kFramesToWaitBeforeAllowingShake = 1;
        private int m_elapsedFrames = 0;
        private bool CanShake() => m_elapsedFrames >= kFramesToWaitBeforeAllowingShake;

        private void Start()
        {
            m_target = GameManager.Player;

            m_target.statsChanged.AddListener(OnStatsChanged);
            m_target.currentStatsChanged.AddListener(OnStatsChanged);

            m_elapsedFrames = 0;

            UpdateUI();
        }

        private void OnStatsChanged(Stats previous)
        {
            UpdateUI();
        }

        private void Update()
        {
            if (!CanShake())
            {
                ++m_elapsedFrames;
            }
        }

        private void UpdateUI()
        {
            m_label.text = GameManager.Config.GetTermDefinition(m_stat).shortName;

            int current = m_target.currentStats[m_stat];
            int max = m_target.stats[m_stat];

            float previousSliderValue = m_slider.value;

            m_slider.minValue = 0;
            m_slider.maxValue = max;
            m_slider.value = current;

            if (m_slider.value < previousSliderValue && CanShake() && m_shakeOnDecrease)
            {
                Shake();
            }

            m_sliderText.text = StringFormatter.Format("{0}/{1}", current, max);
        }

        private void Shake()
        {
            if (m_shakeHandler.HasValue)
            {
                TransformShaker.InterruptShakeIfInProgress(m_shakeHandler.Value);
                m_shakeHandler = null;
            }

            m_shakeHandler = TransformShaker.Shake(
                target: m_slider.transform,
                amplitude: m_shakeAmplitude,
                frequency: m_shakeFrequency,
                duration: m_shakeDuration
            );
        }
    }
}
