using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    public class UIHUDAbilityMessage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] protected TextMeshProUGUI m_message = null;

        [Header("Animation Settings")]
        [SerializeField] protected float m_delayBeforeFadeOut = 1.5f;
        [SerializeField] protected float m_fadeOutDuration = 0.5f;

        [Header("Message Settings")]
        [SerializeField] private SerializableDictionary<EAbilityFireCheckResult, string> m_messages = null;

        private Coroutine m_hideCoroutine = null;

        private void Start()
        {
            m_message.enabled = false;

            GameManager.NotificationSystem.playerFireFailed.AddListener(OnPlayerFireFailed);
        }

        private void OnPlayerFireFailed(ITriggerableAbility ability, EAbilityFireCheckResult reason)
        {
            if (m_messages.ContainsKey(reason))
            {
                Show(m_messages[reason]);
                return;
            }
        }

        public void Show(string message)
        {
            InterruptPreviousMessage();

            m_message.text = message;
            m_message.enabled = true;
            m_message.alpha = 1.0f;

            m_hideCoroutine = StartCoroutine(FadeOutAfterDelay(m_delayBeforeFadeOut));
        }

        private void InterruptPreviousMessage()
        {
            if (m_hideCoroutine != null)
            {
                StopCoroutine(m_hideCoroutine);
                m_hideCoroutine = null;
            }
        }

        private IEnumerator FadeOutAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            yield return StartCoroutine(FadeOut(m_fadeOutDuration));
            Hide();
        }

        private IEnumerator FadeOut(float duration)
        {
            float elapsedTime = 0.0f;

            while (elapsedTime < duration)
            {
                m_message.alpha = math.lerp(1.0f, 0.0f, elapsedTime / duration);
                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            m_message.alpha = 0.0f;
        }

        private void Hide()
        {
            InterruptPreviousMessage();
            m_message.enabled = false;
        }
    }
}