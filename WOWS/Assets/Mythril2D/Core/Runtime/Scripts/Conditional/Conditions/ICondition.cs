using System;

namespace Gyvr.Mythril2D
{
    public interface ICondition
    {
        public bool Evaluate();
        public void StartListening(Action onStateChanged);
        public void StopListening();
    }
}
