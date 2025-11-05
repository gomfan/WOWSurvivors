using System.Collections;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EAudioChannelMode
    {
        Multiple,
        Exclusive
    }

    public class AudioChannel : MonoBehaviour
    {
        [Header("General Settings")]
        [SerializeField] private EAudioChannelMode m_audioChannelMode;
        [SerializeField] private AudioSource m_audioSource = null;
        [SerializeField] private float m_volumeScale = 0.5f;

        [Header("Exclusive Mode Settings")]
        [SerializeField] private float m_fadeOutDuration = 0.5f;
        [SerializeField] private float m_fadeInDuration = 0.25f;

        private Coroutine m_transitionCoroutine = null;
        private AudioClipResolver m_lastPlayedClip = null;

        public AudioClipResolver lastPlayedAudioClipResolver => m_lastPlayedClip;

        private void Awake()
        {
            m_audioSource.volume = m_volumeScale;
        }

        public void Play(AudioClipResolver audioClipResolver)
        {
            AudioClip audioClip = audioClipResolver.GetClip();
            m_lastPlayedClip = audioClipResolver;

            if (audioClip != null)
            {
                if (m_audioChannelMode == EAudioChannelMode.Exclusive)
                {
                    if (m_transitionCoroutine != null)
                    {
                        StopCoroutine(m_transitionCoroutine);
                    }

                    m_transitionCoroutine = StartCoroutine(FadeOutAndIn(audioClip));
                }
                else
                {
                    m_audioSource.PlayOneShot(audioClip);
                }
            }
        }

        public void SetVolumeScale(float scale)
        {
            m_volumeScale = scale;
            m_audioSource.volume = m_volumeScale;
        }

        public float GetVolumeScale()
        {
            return m_volumeScale;
        }

        public IEnumerator FadeOutAndIn(AudioClip newClip)
        {
            // Avoid stalling the game when calling "m_audioSource.Play();" with a new clip that hasn't loaded its audio data yet.
            // If stalling still occurs, a workaround could be to extend the fade out duration.
            newClip.LoadAudioData();

            // Fade out
            while (m_audioSource.volume > 0)
            {
                m_audioSource.volume -= m_volumeScale * Time.unscaledDeltaTime / m_fadeOutDuration;
                yield return null;
            }
            m_audioSource.Stop();
            m_audioSource.clip = newClip;
            m_audioSource.Play();

            // Fade in
            while (m_audioSource.volume < m_volumeScale)
            {
                m_audioSource.volume += m_volumeScale * Time.unscaledDeltaTime / m_fadeInDuration;
                yield return null;
            }
            m_audioSource.volume = m_volumeScale;
        }
    }
}
