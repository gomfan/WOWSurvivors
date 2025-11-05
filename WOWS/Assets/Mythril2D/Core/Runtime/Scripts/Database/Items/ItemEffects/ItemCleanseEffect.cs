using System;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public class ItemCleanseEffect : AItemEffect
    {
        protected override ItemUsageResult OnUse(Item item, CharacterBase target, EItemLocation location)
        {
            int negativeEffectCount = target.Cleanse(new[] { EEffectType.Debuff });

            if (negativeEffectCount > 0)
            {
                return new()
                {
                    success = true,
                    message = $"You recovered from {negativeEffectCount} negative effects"
                };
            }

            return new ItemUsageResult { success = false };
        }
    }
}
