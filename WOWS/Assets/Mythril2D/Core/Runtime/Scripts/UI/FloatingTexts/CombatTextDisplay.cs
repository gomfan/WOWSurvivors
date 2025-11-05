using UnityEngine;

namespace Gyvr.Mythril2D
{
    [RequireComponent(typeof(FloatingTextPool))]
    public class CombatTextDisplay : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private bool m_showDamages = true;
        [SerializeField] private bool m_showHeals = true;
        [SerializeField] private bool m_showNullHeals = true;

        [Header("Mana Settings")]
        [SerializeField] private bool m_showConsumedMana = false;
        [SerializeField] private bool m_showRecoveredMana = true;
        [SerializeField] private bool m_showNullManaConsumption = false;
        [SerializeField] private bool m_showNullManaRecovery = true;

        [Header("Temporal Effects Settings")]
        [SerializeField] private bool m_showTemporalEffects = true;

        [Header("Text Colors")]
        [SerializeField] private Color m_damageColor = Color.white;
        [SerializeField] private Color m_silentDamageColor = Color.red;
        [SerializeField] private Color m_missDamageColor = Color.white;
        [SerializeField] private Color m_criticalDamageColor = Color.yellow;
        [SerializeField] private Color m_healColor = Color.green;
        [SerializeField] private Color m_manaConsumedColor = Color.cyan;
        [SerializeField] private Color m_manaRecoveredColor = Color.cyan;
        [SerializeField] private Color m_temporalEffectAppliedColor = Color.white;

        [Header("Text Content")]
        [SerializeField] private string m_missText = "Miss";

        [Header("Animation Parameters")]
        [SerializeField] private string m_textAnimationParameter = "bounce";

        private FloatingTextPool m_floatingTextPool = null;

        private void Awake()
        {
            m_floatingTextPool = GetComponent<FloatingTextPool>();
        }

        private void Start()
        {
            GameManager.NotificationSystem.damageApplied.AddListener(OnDamageApplied);
            GameManager.NotificationSystem.healthRecovered.AddListener(OnHealthRecovered);
            GameManager.NotificationSystem.manaConsumed.AddListener(OnManaConsumed);
            GameManager.NotificationSystem.manaRecovered.AddListener(OnManaRecovered);
            GameManager.NotificationSystem.temporalEffectApplied.AddListener(OnTemporalEffectApplied);
        }

        private void OnDamageApplied(CharacterBase target, DamageInputDescriptor input, EEffectVisualFlags visualFlags)
        {
            if (m_showDamages && !visualFlags.HasFlag(EEffectVisualFlags.NoFloatingText))
            {
                string text = input.flags.HasFlag(EDamageFlag.Miss) ? m_missText : input.damage.ToString();

                Color color =
                    input.flags.HasFlag(EDamageFlag.Miss) ? m_missDamageColor :
                    input.flags.HasFlag(EDamageFlag.Critical) ? m_criticalDamageColor :
                    (input.silent && !input.flags.HasFlag(EDamageFlag.Miss) ? m_silentDamageColor : m_damageColor);

                m_floatingTextPool.ShowText(text, target.transform.position, color, m_textAnimationParameter);
            }
        }

        private void OnHealthRecovered(CharacterBase target, int amount, EEffectVisualFlags visualFlags)
        {
            if (m_showHeals && !visualFlags.HasFlag(EEffectVisualFlags.NoFloatingText) && (m_showNullHeals || amount > 0))
            {
                m_floatingTextPool.ShowText(amount.ToString(), target.transform.position, m_healColor, m_textAnimationParameter);
            }
        }

        private void OnManaConsumed(CharacterBase target, int amount, EEffectVisualFlags visualFlags)
        {
            if (m_showConsumedMana && !visualFlags.HasFlag(EEffectVisualFlags.NoFloatingText) && (m_showNullManaConsumption || amount > 0))
            {
                m_floatingTextPool.ShowText(amount.ToString(), target.transform.position, m_manaConsumedColor, m_textAnimationParameter);
            }
        }

        private void OnManaRecovered(CharacterBase target, int amount, EEffectVisualFlags visualFlags)
        {
            if (m_showRecoveredMana && !visualFlags.HasFlag(EEffectVisualFlags.NoFloatingText) && (m_showNullManaRecovery || amount > 0))
            {
                m_floatingTextPool.ShowText(amount.ToString(), target.transform.position, m_manaRecoveredColor, m_textAnimationParameter);
            }
        }

        private void OnTemporalEffectApplied(CharacterBase target, ITemporalEffect effect)
        {
            if (m_showTemporalEffects && !effect.visualFlags.HasFlag(EEffectVisualFlags.NoFloatingText) && effect.info.HasValue)
            {
                m_floatingTextPool.ShowText(effect.info.Value.shortName, target.transform.position, m_temporalEffectAppliedColor, m_textAnimationParameter);
            }
        }
    }
}
