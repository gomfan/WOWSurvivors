using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public interface IAnimationStrategy
    {
        public UnityEvent deathAnimationStarted { get; }
        public UnityEvent deathAnimationEnded { get; }

        void Initialize();
        void Pause();
        void Resume();
        void OnMessageReceived(string message, SendMessageOptions options);
        void SetLookAtDirection(Vector2 direction);
        void SetTargetDirection(Vector2 direction);
        void SetMovement(Vector2 speed);
        bool PlayHitAnimation();
        bool PlayDeathAnimation();
        bool PlayInvincibleAnimation();
        bool IsInvincibleAnimationPlaying();
    }
}
