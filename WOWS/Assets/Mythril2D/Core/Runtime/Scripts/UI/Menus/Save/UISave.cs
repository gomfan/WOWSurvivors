using UnityEngine;

namespace Gyvr.Mythril2D
{
    public class UISave : AUIMenu
    {
        [Header("References")]
        [SerializeField] private UISaveFile[] m_saveFiles = null;

        protected override void OnMenuShown(params object[] args)
        {
            Debug.Assert(args.Length == 0, "SaveMenu panel invoked with incorrect arguments");
            UpdateUI();
        }

        private void UpdateUI(bool skipItemSlots = false)
        {
            foreach (UISaveFile saveFile in m_saveFiles)
            {
                saveFile.UpdateUI();
            }
        }

        private void OnSaveFileClicked(SaveFileActionDesc desc)
        {
            GameManager.SaveSystem.SaveToFile(desc.filename);
            UpdateUI();
        }
    }
}
