using System.Threading.Tasks;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public abstract class AUIMenu : MonoBehaviour, IUIMenu
    {
        private TaskCompletionSource<bool> m_menuClosedTask = null;

        public virtual void OnMenuPushed() { }
        public virtual void OnMenuPopped() { }
        public virtual bool OnCancel() => false;
        protected virtual void OnInit() { }
        protected virtual void OnMenuShown(params object[] args) { }
        protected virtual void OnMenuHidden() { }

        public virtual bool CanPop() => true;

        public virtual GameObject FindSomethingToSelect() => null;

        public void Init()
        {
            OnInit();
        }

        public void Show(TaskCompletionSource<bool> menuClosedTask, params object[] args)
        {
            m_menuClosedTask = menuClosedTask;
            gameObject.SetActive(true);
            OnMenuShown(args);
        }

        public void Hide()
        {
            m_menuClosedTask?.SetResult(true);
            gameObject.SetActive(false);
            OnMenuHidden();
        }

        public virtual void EnableInteractions(bool enable)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();

            if (canvasGroup)
            {
                canvasGroup.interactable = enable;
            }
        }
    }
}
