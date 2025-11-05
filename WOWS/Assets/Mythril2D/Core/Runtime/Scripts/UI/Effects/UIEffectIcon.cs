using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIEffectIcon : MonoBehaviour
    {
        [SerializeField] private Image m_icon = null;

        public void Show(Sprite sprite)
        {
            gameObject.SetActive(true);
            m_icon.sprite = sprite;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
