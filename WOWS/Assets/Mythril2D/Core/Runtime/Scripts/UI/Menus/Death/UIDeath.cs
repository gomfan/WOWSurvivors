using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gyvr.Mythril2D
{
    public class UIDeath : AUIMenu
    {
        [Header("References")]
        [SerializeField] private Button m_quitButton = null;

        public override bool CanPop() => false;

        public override GameObject FindSomethingToSelect()
        {
            return m_quitButton.gameObject;
        }

        public void GoToMainMenu()
        {
            SceneManager.LoadScene(GameManager.Config.mainMenuSceneName);
        }
    }
}
