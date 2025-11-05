using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UIHUDAbilityBar : MonoBehaviour
    {
        protected UIHUDAbilityBarEntry[] m_abilities = null;

        private void Start()
        {
            GameManager.Player.equippedAbilitiesChanged.AddListener(OnEquippedAbilitiesChanged);

            m_abilities = GetComponentsInChildren<UIHUDAbilityBarEntry>();

            for (int i = 0; i < m_abilities.Length; ++i)
            {
                m_abilities[i].SetAbility(null, i);
            }

            OnEquippedAbilitiesChanged(GameManager.Player.equippedAbilities);
        }

        private void OnEquippedAbilitiesChanged(ActiveAbilitySheet[] abilities)
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