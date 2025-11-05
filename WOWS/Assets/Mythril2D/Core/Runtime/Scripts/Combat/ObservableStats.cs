using UnityEngine.Events;

namespace Gyvr.Mythril2D
{
    public class ObservableStats
    {
        public Stats values => m_values;
        public UnityEvent<Stats> changed => m_changed;

        private Stats m_values;
        private UnityEvent<Stats> m_changed = new();

        public ObservableStats() : this(new Stats()) { }

        public ObservableStats(Stats stats)
        {
            m_values = stats;
        }

        public int this[EStat stat]
        {
            get => m_values[stat];
            set
            {
                Stats previous = new(m_values);
                m_values[stat] = value;
                m_changed.Invoke(previous);
            }
        }

        public void Set(Stats stats)
        {
            Stats previous = new(m_values);
            m_values = new(stats);
            m_changed.Invoke(previous);
        }
    }
}
