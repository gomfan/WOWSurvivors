using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using azixMcAze.SerializableDictionary;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class PerTargetCooldownDataBlock<TargetType> : DataBlock where TargetType : Persistable
    {
        public SerializableDictionary<PersistableReference<TargetType>, float> perTargetCooldowns = new();
    }

    public class PerTargetCooldown<TargetType> : IDataBlockHandler<PerTargetCooldownDataBlock<TargetType>> where TargetType : Persistable
    {
        private Dictionary<TargetType, float> m_perTargetCooldowns = new();

        public IEnumerable<TargetType> FilterInvalidTargets(IEnumerable<TargetType> potentialTargets)
        {
            return potentialTargets.Where(target => !IsTargetOnCooldown(target));
        }

        public bool IsTargetOnCooldown(TargetType target) => m_perTargetCooldowns.ContainsKey(target);

        public void StartCooldown(TargetType target, float duration)
        {
            if (duration > 0.0f)
            {
                m_perTargetCooldowns[target] = duration;
            }
        }

        public void Reset()
        {
            m_perTargetCooldowns.Clear();
        }

        public void Update()
        {
            foreach (var key in new HashSet<TargetType>(m_perTargetCooldowns.Keys))
            {
                m_perTargetCooldowns[key] -= Time.deltaTime;
                if (m_perTargetCooldowns[key] <= 0.0f)
                {
                    m_perTargetCooldowns.Remove(key);
                }
            }
        }

        public PerTargetCooldownDataBlock<TargetType> CreateDataBlock() => new()
        {
            perTargetCooldowns = new SerializableDictionary<PersistableReference<TargetType>, float>(
                m_perTargetCooldowns.ToDictionary(pair => new PersistableReference<TargetType>(pair.Key), pair => pair.Value)
            )
        };

        public void LoadDataBlock(PerTargetCooldownDataBlock<TargetType> block)
        {
            m_perTargetCooldowns = block.perTargetCooldowns.ToDictionary(pair => pair.Key.instance, pair => pair.Value);
        }
    }
}
