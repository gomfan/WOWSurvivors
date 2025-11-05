using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIAbilityBar : MonoBehaviour
    {
        protected UIAbilityBarEntry[] m_abilities = null;

        public void Init()
        {
            m_abilities = GetComponentsInChildren<UIAbilityBarEntry>();

            for (int i = 0; i < m_abilities.Length; ++i)
            {
                m_abilities[i].SetAbility(null, i);
            }

            GameManager.Player.equippedAbilitiesChanged.AddListener(FillAbilityBar);
        }

        public void SelectFirstElement()
        {
            m_abilities[0].ForceSelection();
        }

        public void UpdateUI()
        {
            FillAbilityBar(GameManager.Player.equippedAbilities);
        }

        private void FillAbilityBar(ActiveAbilitySheet[] abilities)
        {
            for (int i = 0; i < math.min(m_abilities.Length, GameManager.Config.maxEquippableAbilities); ++i)
            {
                if (abilities.Length > i)
                {
                    ActiveAbilitySheet ability = abilities[i];
                    m_abilities[i].SetAbility(ability, i);
                }
                else
                {
                    m_abilities[i].SetAbility(null, i);
                }
            }
        }
    }
}