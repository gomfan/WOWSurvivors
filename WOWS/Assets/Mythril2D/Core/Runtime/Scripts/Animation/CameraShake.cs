using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class CameraShake : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float m_amplitude = 0.05f;
        [SerializeField] private float2 m_frequency = new(60.0f, 50.0f);
        [SerializeField] private float m_duration = 0.2f;
        [SerializeField] private float m_criticalHitAmplitudeModifier = 2.0f;

        private ShakeHandler? m_shakeHandler = null;

        private void OnEnable()
        {
            GameManager.NotificationSystem.damageApplied.AddListener(OnDamageApplied);
        }

        private void OnDisable()
        {
            GameManager.NotificationSystem.damageApplied.RemoveListener(OnDamageApplied);
        }

        private bool IsCameraAllowedToShake()
        {
            return GameManager.Config.cameraShakeSources != ECameraShakeSources.None;
        }

        private bool IsValidShakeSource(CharacterBase target, DamageInputDescriptor damageInputDescriptor)
        {
            return
                !damageInputDescriptor.silent
                &&
                (
                    GameManager.Config.cameraShakeSources.HasFlag(ECameraShakeSources.PlayerReceiveDamage) &&
                    target == GameManager.Player
                )
                ||
                (
                    GameManager.Config.cameraShakeSources.HasFlag(ECameraShakeSources.AnyCharacterReceiveDamageFromPlayer) &&
                    damageInputDescriptor.source is CharacterDamageSource source &&
                    source.character == GameManager.Player
                );
        }

        private void OnDamageApplied(CharacterBase target, DamageInputDescriptor damageInputDescriptor, EEffectVisualFlags visualFlags)
        {
            if (IsCameraAllowedToShake() && IsValidShakeSource(target, damageInputDescriptor) && !visualFlags.HasFlag(EEffectVisualFlags.NoCameraShake))
            {
                if (!damageInputDescriptor.flags.HasFlag(EDamageFlag.Miss))
                {
                    if (m_shakeHandler.HasValue)
                    {
                        TransformShaker.InterruptShakeIfInProgress(m_shakeHandler.Value);
                        m_shakeHandler = null;
                    }

                    bool isCriticalHit = damageInputDescriptor.flags.HasFlag(EDamageFlag.Critical);
                    float amplitude = isCriticalHit ? m_amplitude * m_criticalHitAmplitudeModifier : m_amplitude;
                    transform.localPosition = new Vector3(0.0f, 0.0f, transform.localPosition.z);
                    m_shakeHandler = TransformShaker.Shake(transform, amplitude, m_frequency, m_duration);
                }
            }
        }
    }
}
