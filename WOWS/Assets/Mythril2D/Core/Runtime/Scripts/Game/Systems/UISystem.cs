using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UISystem : AGameSystem
    {
        [Header("References")]
        [SerializeField] private GameObject m_uiPrefab;

        private GameObject m_uiInstance = null;

        // Called after gameplay has been initialized properly.
        // We do this to make sure the UI, when it's created, is created after the gameplay has been initialized.
        // As the UI might depend on some gameplay data.
        public override void OnSaveFileLoaded()
        {
            ShowUI();
        }

        public void ShowUI()
        {
            if (m_uiInstance == null)
            {
                m_uiInstance = Instantiate(m_uiPrefab, transform);
            }
            else
            {
                m_uiInstance.SetActive(true);
            }
        }

        public void HideUI()
        {
            if (m_uiInstance != null)
            {
                m_uiInstance.SetActive(false);
            }
        }
    }
}
