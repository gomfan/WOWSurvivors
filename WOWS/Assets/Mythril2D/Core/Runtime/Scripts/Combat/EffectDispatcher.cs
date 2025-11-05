using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    public enum EEffectInteractionResult
    {
        NotApplicable,
        ApplyFailed,
        ApplySucceeded,
        Consumed
    }

    public enum EEffectImpactDataType
    {
        SourcePosition,
        Velocity
    }

    [Serializable]
    public struct EffectImpactSettings
    {
        public EEffectImpactDataType impactDataType;
        public Vector2 impactData;
    }

    public struct EffectApplicationResult
    {
        public IEnumerable<EEffectInteractionResult> feed;
        public IEnumerable<CharacterBase> affectedTargets;
    }

    public static class EffectDispatcher
    {
        public static EffectApplicationResult Apply(CharacterBase source, IEnumerable<CharacterBase> targets, IEnumerable<IEffect> effects, EffectImpactSettings? impactSettings = null)
        {
            Debug.Assert(effects != null, "Effects must not be null.");
            Debug.Assert(targets != null, "Targets must not be null.");

            List<EEffectInteractionResult> feed = new();
            HashSet<CharacterBase> affectedTargets = new();

            if (!targets.Any())
            {
                return new EffectApplicationResult { feed = feed, affectedTargets = affectedTargets };
            }

            foreach (IEffect effect in effects)
            {
                effect.Init(source);
            }

            foreach (CharacterBase target in targets)
            {
                if (target != null)
                {
                    ApplyEffectsToTarget(target, effects, impactSettings, feed, affectedTargets);
                }
            }

            foreach (IEffect effect in effects)
            {
                effect.Deinit();
            }

            Debug.Assert(effects.Where(effect => effect.initialized).Count() == 0, "Effects must be deinitialized after applying.");

            return new()
            {
                feed = feed,
                affectedTargets = affectedTargets
            };
        }

        private static void ApplyEffectsToTarget(CharacterBase target, IEnumerable<IEffect> effects, EffectImpactSettings? impactSettings, List<EEffectInteractionResult> feed, HashSet<CharacterBase> affectedTargets)
        {
            foreach (IEffect effect in effects)
            {
                EEffectInteractionResult interaction = EEffectInteractionResult.NotApplicable;

                if (effect.IsApplicable(target))
                {
                    IEffect effectInstance = effect;

                    if (effect is ITemporalEffect temporalEffect)
                    {
                        if (TryConsumeTemporalEffect(target, temporalEffect))
                        {
                            interaction = EEffectInteractionResult.Consumed;
                        }
                        else
                        {
                            effectInstance = temporalEffect.Clone();
                        }
                    }

                    if (interaction != EEffectInteractionResult.Consumed)
                    {
                        interaction =
                            effectInstance.Apply(target, impactSettings) ?
                            EEffectInteractionResult.ApplySucceeded :
                            EEffectInteractionResult.ApplyFailed;
                    }
                }

                if (interaction == EEffectInteractionResult.ApplySucceeded || interaction == EEffectInteractionResult.Consumed)
                {
                    affectedTargets.Add(target);
                }

                feed.Add(interaction);

                if (effect.interruptionPolicy == EEffectInterruptionPolicy.AfterSuccess && (interaction == EEffectInteractionResult.ApplySucceeded || interaction == EEffectInteractionResult.Consumed))
                {
                    break;
                }
                else if (effect.interruptionPolicy == EEffectInterruptionPolicy.AfterFail && interaction == EEffectInteractionResult.ApplyFailed)
                {
                    break;
                }
            }
        }

        private static bool TryConsumeTemporalEffect(CharacterBase target, ITemporalEffect temporalEffect)
        {
            foreach (ITemporalEffect targetEffect in target.temporalEffects)
            {
                if (targetEffect.TryStack(temporalEffect))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
