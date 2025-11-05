using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIHUDAbilityBarEntry : UIAbility
    {
        [Header("References")]
        [SerializeField] private UIControllerButton m_controllerButton = null;
        [SerializeField] private Slider m_cooldownSlider = null;
        [SerializeField] private TextMeshProUGUI m_cooldownText = null;

        public override void SetAbility(ActiveAbilitySheet sheet, int index)
        {
            base.SetAbility(sheet, index);

            // Hide empty abilities in the HUD
            gameObject.SetActive(sheet);

            if (sheet)
            {
                m_controllerButton.SetAction((UIControllerButtonManager.EAction)(index + (int)UIControllerButtonManager.EAction.FireAbility1));
            }
        }

        private void Update()
        {
            AbilityBase ability = GameManager.Player.GetAbility(m_abilitySheet);

            if (ability is ITriggerableAbility triggerableAbility)
            {
                ActiveAbilityBase activeAbility = triggerableAbility.GetAbilityBase();

                if (activeAbility.cooldown > 0.0f)
                {
                    float cooldownRatio = (activeAbility.cooldown - activeAbility.remainingCooldown) / activeAbility.cooldown;
                    m_cooldownSlider.value = 1.0f - cooldownRatio;
                }
                else
                {
                    m_cooldownSlider.value = 0.0f;
                }

                int seconds = Mathf.CeilToInt(activeAbility.remainingCooldown);
                m_cooldownText.text =
                    activeAbility.remainingCooldown > 0.0f ?
                    (activeAbility.remainingCooldown < 0.5f ? $"{activeAbility.remainingCooldown:0.0}" : $"{seconds}") :
                    string.Empty;
            }
        }
    }
}
