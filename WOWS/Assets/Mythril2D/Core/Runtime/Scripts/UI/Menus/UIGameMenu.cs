using UnityEngine;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIGameMenu : AUIMenu
    {
        [Header("References")]
        [SerializeField] private UIGameMenuEntry[] m_menus;
        [SerializeField] private GameObject[] m_disableWhileOpened = null;
        [SerializeField] private UIEffectList m_effectList = null;

        [Header("Audio")]
        [SerializeField] private AudioClipResolver m_pauseSound;
        [SerializeField] private AudioClipResolver m_resumeSound;

        private Selectable m_selected = null;

        public override void OnMenuPushed()
        {
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_pauseSound);
        }

        public override void OnMenuPopped()
        {
            GameManager.NotificationSystem.audioPlaybackRequested.Invoke(m_resumeSound);
        }

        protected override void OnInit()
        {
            m_selected?.Select();
        }

        protected override void OnMenuShown(params object[] args)
        {
            foreach (GameObject gameObject in m_disableWhileOpened)
            {
                gameObject.SetActive(false);
            }

            m_effectList.Show();
        }

        protected override void OnMenuHidden()
        {
            foreach (GameObject gameObject in m_disableWhileOpened)
            {
                gameObject.SetActive(true);
            }

            m_effectList.Hide();
        }

        public override GameObject FindSomethingToSelect()
        {
            if (m_selected)
            {
                return m_selected.gameObject;
            }

            return null;
        }

        private void OnGameMenuEntrySelected(Selectable selected)
        {
            m_selected = selected;
        }
    }
}
