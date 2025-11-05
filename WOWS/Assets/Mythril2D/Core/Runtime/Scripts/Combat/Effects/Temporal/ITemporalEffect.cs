namespace Gyvr.Mythril2D
{

    public enum EEffectType
    {
        Buff,
        Debuff
    }

    public interface ITemporalEffect : IEffect
    {
        public TermDefinition? info { get; }
        public bool completed { get; }
        public string stackableEffectId { get; }
        public float duration { get; }
        public void Update();
        public void Complete();
        public bool TryStack(ITemporalEffect effect);
        public EEffectType GetEffectType();
        public ITemporalEffect Clone();
    }
}
