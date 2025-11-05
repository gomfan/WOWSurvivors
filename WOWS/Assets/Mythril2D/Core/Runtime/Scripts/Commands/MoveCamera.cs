using System;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions;

namespace Gyvr.Mythril2D
{
    public interface ICameraMovementStrategy
    {
        Task MoveCameraAsync();
    }

    public abstract class ACameraMovementStrategy : ICameraMovementStrategy
    {
        [SerializeField] private float m_speed = 10f;

        public abstract Task MoveCameraAsync();

        public async Task MoveCameraToAsync(Vector2 targetPosition, bool localMode = false)
        {
            Vector3 _GetCameraPosition() => localMode ? Camera.main.transform.localPosition : Camera.main.transform.position;

            var targetCamera = Camera.main;
            var initialPosition = _GetCameraPosition();

            float duration = 0.0f;

            float transitionDuration = Vector2.Distance(
                initialPosition,
                targetPosition
            ) / m_speed;

            while (duration < transitionDuration)
            {
                Vector2 positionThisFrame = Vector2.Lerp(
                    initialPosition,
                    targetPosition,
                    math.min(duration, transitionDuration) / transitionDuration
                );

                Vector3 currentPosition = new(
                    positionThisFrame.x,
                    positionThisFrame.y,
                    _GetCameraPosition().z
                );

                if (localMode)
                {
                    targetCamera.transform.localPosition = currentPosition;
                }
                else
                {
                    targetCamera.transform.position = currentPosition;
                }

                duration += Time.deltaTime;

                await Task.Yield();
            }
        }
    }

    [Serializable]
    public class MoveCameraToPosition : ACameraMovementStrategy
    {
        [SerializeField] private Vector2 m_targetPosition;

        public override async Task MoveCameraAsync() => await MoveCameraToAsync(m_targetPosition);
    }

    [Serializable]
    public class MoveCameraToGameObject : ACameraMovementStrategy
    {
        [SerializeField] private GameObject m_targetGameObject;

        public override async Task MoveCameraAsync() => await MoveCameraToAsync(m_targetGameObject.transform.position);
    }

    [Serializable]
    public class ResetCamera : ACameraMovementStrategy
    {
        public override async Task MoveCameraAsync() => await MoveCameraToAsync(Vector2.zero, true);
    }

    [Serializable]
    public class MoveCamera : ICommand
    {
        [SerializeReference, SubclassSelector] private ICameraMovementStrategy m_cameraMovementStrategy;

        public Task Execute()
        {
            return m_cameraMovementStrategy.MoveCameraAsync();
        }
    }
}
