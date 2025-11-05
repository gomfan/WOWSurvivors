using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIHUDEffectBar : InstancePool
    {
        private Dictionary<ITemporalEffect, UIEffectIcon> m_effectIcons = new();

        private void Start()
        {
            var player = GameManager.Player;

            player.temporalEffectAdded.AddListener(OnTemporalEffectAdded);
            player.temporalEffectRemoved.AddListener(OnTemporalEffectRemoved);

            // Recover any existing effects
            foreach (ITemporalEffect effect in player.temporalEffects)
            {
                OnTemporalEffectAdded(effect);
            }
        }

        private void OnTemporalEffectAdded(ITemporalEffect effect)
        {
            if (effect.info.HasValue)
            {
                var availableInstance = GetAvailableInstance();
                if (availableInstance)
                {
                    var effectIcon = availableInstance.GetComponent<UIEffectIcon>();
                    Debug.Assert(effectIcon, "Could not find UIEffectIcon component on instance");
                    m_effectIcons[effect] = effectIcon;
                    effectIcon.Show(effect.info.Value.icon);
                }
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
    }
}
