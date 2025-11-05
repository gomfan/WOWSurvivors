using UnityEngine;
using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public class UINavigationCursorTarget : UINavigationTarget
    {
        public NavigationCursorStyle navigationCursorStyle => m_navigationCursorStyle;
        public Vector3 totalPositionOffset => m_navigationCursorStyle.positionOffset + m_positionOffset;
        public Vector2 totalSizeOffset => m_navigationCursorStyle.sizeOffset + m_sizeOffset;
        public UnityEvent destroyed => m_destroyed;

        [SerializeField] private NavigationCursorStyle m_navigationCursorStyle = null;
        [SerializeField] private Vector2 m_positionOffset = Vector2.zero;
        [SerializeField] private Vector2 m_sizeOffset = Vector2.zero;

        private UnityEvent m_destroyed = new();

        private void OnDestroy()
        {
            destroyed.Invoke();
        }
    }
}