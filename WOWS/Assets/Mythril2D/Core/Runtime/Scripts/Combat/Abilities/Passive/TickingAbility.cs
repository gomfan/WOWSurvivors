using System;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class TickingAbilityDataBlock : PassiveAbilityBaseDataBlock
    {
        public float timer;
    }

    public class TickingAbility : PassiveAbility<TickingAbilitySheet>
    {
        private float m_timer = 0f;

        private void Update()
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0f)
            {
                m_timer = passiveAbilitySheet.delayBetweenTicks;
                ApplyEffectsToSelf(passiveAbilitySheet.effects);
            }
        }

        protected override Type GetDataBlockType() => typeof(TickingAbilityDataBlock);

        protected override void OnLoad(PersistableDataBlock block)
        {
            base.OnLoad(block);
            m_timer = block.As<TickingAbilityDataBlock>().timer;
        }

        protected override void OnSave(PersistableDataBlock block)
        {
            base.OnSave(block);
            block.As<TickingAbilityDataBlock>().timer = m_timer;
        }
    }
}
