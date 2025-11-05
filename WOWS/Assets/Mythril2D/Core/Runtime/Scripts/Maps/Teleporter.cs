using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public enum EVerticalDirection { None, Up, Down }
    public enum EHorizontalDirection { None, Left, Right }

    public class Teleporter : Checkpoint
    {
        [Header("Destination Settings")]
        [SerializeReference, SubclassSelector] private ICheckpoint m_destination;
        [SerializeField] private bool m_saveCheckpointOnArrival = false;

        [Header("Activation Settings")]
        [SerializeField] private EVerticalDirection m_requiredVerticalMovement = EVerticalDirection.None;
        [SerializeField] private EHorizontalDirection m_requiredHorizontalMovement = EHorizontalDirection.None;

        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_activationAudio;

        // Used to prevent a teleporter from triggering multiple teleportations before the previous one is fully completed
        private static bool _teleportationInProgress = false;

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!_teleportationInProgress && collision != null && collision.gameObject == GameManager.Player.gameObject)
            {
                if (GameManager.Player.dead) return;

                if (m_requiredVerticalMovement == EVerticalDirection.Up && !GameManager.Player.IsMovingUp()) return;
                if (m_requiredVerticalMovement == EVerticalDirection.Down && !GameManager.Player.IsMovingDown()) return;
                if (m_requiredHorizontalMovement == EHorizontalDirection.Left && !GameManager.Player.IsMovingLeft()) return;
                if (m_requiredHorizontalMovement == EHorizontalDirection.Right && !GameManager.Player.IsMovingRight()) return;

                GameManager.Player.InterruptPush();

                GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_activationAudio);

                _teleportationInProgress = true;

                GameManager.MapSystem.TeleportTo(m_destination, null, () =>
                {
                    if (m_saveCheckpointOnArrival)
                    {
                        GameManager.MapSystem.SaveCheckpoint(m_destination);
                    }
                    _teleportationInProgress = false;
                });
            }
        }
    }
}
