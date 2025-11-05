using UnityEngine;

namespace Gyvr.Mythril2D
{
    public interface IAnimationMessageReceiver
    {
        public void DispatchAnimationMessage(string message, SendMessageOptions options);
    }
}
