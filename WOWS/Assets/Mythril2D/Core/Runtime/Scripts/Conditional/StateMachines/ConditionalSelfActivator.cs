namespace Gyvr.Mythril2D
{
    public class ConditionalSelfActivator : AConditionalActivator
    {
        protected override void Activate(bool state)
        {
            gameObject.SetActive(state);
        }
    }
}
