using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UICharacterInfo : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI m_nameText = null;
        [SerializeField] private Slider m_healthSlider = null;
        [SerializeField] private Slider m_manaSlider = null;
        [SerializeField] private InstancePool m_effectPool = null;
        [SerializeField] private CharacterBase m_target = null;

        private string m_nameAndLevelFormat = string.Empty;

        private Dictionary<ITemporalEffect, UIEffectIcon> m_effectIcons = new();

        private void Awake()
        {
            m_target.statsChanged.AddListener(OnStatsChanged);
            m_target.currentStatsChanged.AddListener(OnStatsChanged);
            m_target.temporalEffectAdded.AddListener(OnTemporalEffectAdded);
            m_target.temporalEffectRemoved.AddListener(OnTemporalEffectRemoved);
            m_target.levelUpped.AddListener(OnLevelUpped);
        }

        private void OnDestroy()
        {
            m_target.statsChanged.RemoveListener(OnStatsChanged);
            m_target.currentStatsChanged.RemoveListener(OnStatsChanged);
            m_target.temporalEffectAdded.RemoveListener(OnTemporalEffectAdded);
            m_target.temporalEffectRemoved.RemoveListener(OnTemporalEffectRemoved);
            m_target.levelUpped.RemoveListener(OnLevelUpped);
        }

        private void Start()
        {
            m_nameAndLevelFormat = m_nameText.text;

            UpdateResourceBars();
            UpdateNameAndLevel();

            // Recover any existing effects
            foreach (ITemporalEffect effect in m_target.temporalEffects)
            {
                OnTemporalEffectAdded(effect);
            }
        }

        public void UpdateResourceBars()
        {
            if (m_healthSlider?.isActiveAndEnabled ?? false)
            {
                m_healthSlider.minValue = 0;
                m_healthSlider.maxValue = m_target.stats[EStat.Health];
                m_healthSlider.value = m_target.currentStats[EStat.Health];
            }

            if (m_manaSlider?.isActiveAndEnabled ?? false)
            {
                m_manaSlider.minValue = 0;
                m_manaSlider.maxValue = m_target.stats[EStat.Mana];
                m_manaSlider.value = m_target.currentStats[EStat.Mana];
            }
        }

        public void UpdateNameAndLevel()
        {
            if (m_nameText?.isActiveAndEnabled ?? false)
            {
                m_nameText.text = StringFormatter.Format(m_nameAndLevelFormat).Replace("{name}", m_target.characterSheet.displayName).Replace("{level}", m_target.level.ToString());
            }
        }

        private void OnStatsChanged(Stats previous) => UpdateResourceBars();

        private void OnTemporalEffectAdded(ITemporalEffect effect)
        {
            if (m_effectPool?.isActiveAndEnabled ?? false && effect.info.HasValue)
            {
                var effectIcon = m_effectPool.GetAvailableInstance().GetComponent<UIEffectIcon>();
                m_effectIcons[effect] = effectIcon;
                effectIcon.Show(effect.info.Value.icon);
            }
        }

        private void OnTemporalEffectRemoved(ITemporalEffect effect)
        {
            if (m_effectIcons.TryGetValue(effect, out UIEffectIcon effectIcon))
            {
                effectIcon.Hide();
                m_effectIcons.Remove(effect);
            }
        }

        private void OnLevelUpped(int level) => UpdateNameAndLevel();
    }
}
