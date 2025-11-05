using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public struct ShakeHandler
    {
        public Transform target;
        public Vector3 initialPosition;
        public Coroutine coroutine;
    }

    public static class TransformShaker
    {
        public static ShakeHandler Shake(Transform target, float amplitude, float2 frequency, float duration)
        {
            return new()
            {
                target = target,
                initialPosition = target.localPosition,
                coroutine = GameManager.Instance.StartCoroutine(
                    ShakeCoroutine(target, amplitude, frequency, duration)
                )
            };
        }

        public static bool InterruptShakeIfInProgress(ShakeHandler handler)
        {
            if (handler.coroutine != null)
            {
                GameManager.Instance.StopCoroutine(handler.coroutine);

                if (handler.target)
                {
                    handler.target.localPosition = handler.initialPosition;
                }

                return true;
            }

            return false;
        }

        private static IEnumerator ShakeCoroutine(Transform target, float amplitude, float2 frequency, float duration)
        {
            float elapsedTime = 0f;
            Vector3 initialPosition = target.localPosition;

            while (elapsedTime < duration)
            {
                float2 offset = math.sin(frequency * elapsedTime) * amplitude;
                target.localPosition = new(initialPosition.x + offset.x, initialPosition.y + offset.y, initialPosition.z);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            target.localPosition = initialPosition;
        }
    }
}
