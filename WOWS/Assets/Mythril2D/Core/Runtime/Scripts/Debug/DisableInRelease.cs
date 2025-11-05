using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class DisableInRelease : MonoBehaviour
    {
        private void Awake()
        {
            if (!Debug.isDebugBuild && !Application.isEditor)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
