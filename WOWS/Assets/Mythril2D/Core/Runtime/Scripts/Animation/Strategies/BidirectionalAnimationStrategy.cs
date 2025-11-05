using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EBidrectionalAnimationDirection
    {
        Right,
        Left
    }

    [Serializable]
    public class BidirectionalAnimationStrategy : AAnimationStrategy
    {
        [Header("Bidirectional Animation Settings")]
        [SerializeField] private EBidrectionalAnimationDirection m_defaultDirection = EBidrectionalAnimationDirection.Right;

        public override void SetLookAtDirection(Vector2 direction)
        {
            base.SetLookAtDirection(direction);

            if (m_spriteRenderer && direction.x != 0.0f)
            {
                m_spriteRenderer.flipX =
                    m_defaultDirection == EBidrectionalAnimationDirection.Right ?
                    direction.x < 0.0f :
                    direction.x > 0.0f;
            }
        }
    }
}
