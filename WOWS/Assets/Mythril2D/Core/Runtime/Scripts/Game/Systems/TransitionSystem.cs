using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class TransitionSystem : AGameSystem
    {
        [Header("Settings")]
        [SerializeField] private bool m_startWithBlackScreen = true;
        [SerializeField] private string m_fadeInAnimationParameter;
        [SerializeField] private string m_fadeOutAnimationParameter;
        [SerializeField] private string m_skipFadeOutAnimationParameter;

        [Header("References")]
        [SerializeField] private Animator m_animator;

        private bool m_hasFadeInAnimation = false;
        private bool m_hasFadeOutAnimation = false;
        private bool m_hasSkipFadeOutAnimation = false;

        private bool m_isBlackScreen = false;

        private MapLoadingDelegationParams m_mapLoadingDelegationParams = null;

        public override void OnSystemInit()
        {
            Debug.Assert(m_animator, ErrorMessages.InspectorMissingComponentReference<Animator>());

            m_hasFadeInAnimation = AnimationUtils.HasParameter(m_animator, m_fadeInAnimationParameter);
            m_hasFadeOutAnimation = AnimationUtils.HasParameter(m_animator, m_fadeOutAnimationParameter);
            m_hasSkipFadeOutAnimation = AnimationUtils.HasParameter(m_animator, m_skipFadeOutAnimationParameter);

            if (m_startWithBlackScreen)
            {
                TryShowBlackScreen();
            }
        }

        public override void OnSystemStart()
        {
            GameManager.NotificationSystem.mapTransitionDelegationRequested.AddListener(OnMapTransitionDelegationRequested);
        }

        public override void OnSystemStop()
        {
            GameManager.NotificationSystem.mapTransitionDelegationRequested.RemoveListener(OnMapTransitionDelegationRequested);
        }

        private void OnMapTransitionDelegationRequested(MapLoadingDelegationParams delegationParams)
        {
            m_mapLoadingDelegationParams = delegationParams;

            Debug.Assert(m_mapLoadingDelegationParams.unloadDelegate != null, "Unload delegate is null");
            Debug.Assert(m_mapLoadingDelegationParams.loadDelegate != null, "Load delegate is null");
            Debug.Assert(m_mapLoadingDelegationParams.completionDelegate != null, "Completion delegate is null");

            if (!m_isBlackScreen)
            {
                TryPlayFadeOutTransition();
            }
            else
            {
                OnFadeOutCompleted();
            }
        }

        // Invoked by the StateMessageDispatcher attached to the FadeOut animation in the animation controller 
        private void OnFadeOutCompleted()
        {
            m_isBlackScreen = true;

            m_mapLoadingDelegationParams.unloadDelegate(() =>
            {
                m_mapLoadingDelegationParams.loadDelegate(() =>
                {
                    TryPlayFadeInTransition();
                });
            });
        }

        // Invoked by the StateMessageDispatcher attached to the FadeIn animation in the animation controller 
        private void OnFadeInCompleted()
        {
            m_isBlackScreen = false;
            m_mapLoadingDelegationParams.completionDelegate();
        }

        public bool TryPlayFadeInTransition()
        {
            Debug.Assert(m_isBlackScreen, "Can't play fade in transition if the screen is not black");

            if (m_hasFadeInAnimation)
            {
                m_animator.SetTrigger(m_fadeInAnimationParameter);
                return true;
            }

            return false;
        }

        public bool TryPlayFadeOutTransition()
        {
            if (m_hasFadeOutAnimation)
            {
                m_animator.SetTrigger(m_fadeOutAnimationParameter);
                return true;
            }

            return false;
        }

        public bool TryShowBlackScreen()
        {
            if (m_hasSkipFadeOutAnimation)
            {
                m_isBlackScreen = true;
                m_animator.SetTrigger(m_skipFadeOutAnimationParameter);
                return true;
            }

            return false;
        }
    }
}
