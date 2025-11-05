# Effects
> On this page, we'll learn about the Mythril2D effect system.

Effects are mostly used by abilities, so I'd recommend getting familiar with the ability system first. You can read more about abilities on the "Abilities" page.

## IEffect (`interface`)
This is the interface that all the effects implement. Currently, Mythril2D offers two types of effects: `IImmediateEffect` and `ITemporalEffect`.

## IImmediateEffect (`IEffect`)
An effect that is applied right away. It's already partially implemented by the abstract class `AImmediateEffect`, so when creating a new type of immediate ability, you should consider inheriting from `AImmediateEffect`.

## ITemporalEffect (`IEffect`)
An effect that is applied over time (damage over time, healing over time, stun, root, slow, etc.). It's already partially implemented by the abstract class `ATemporalEffect`, so when creating a new type of temporal ability, you should consider inheriting from `ATemporalEffect`.

Temporal effects have the particularity that they need to be clonable, meaning that the interface requires subclasses to implement the `Clone` method. Cloning is required for temporal effects, as one effect (as specified in an `AbilitySheet` for instance) can be instanced for as many targets as the ability can affect. Each instance of the temporal effect will live in memory on its own and be serialized in save files, so that it can be reloaded later (i.e., if you poison an enemy and save the game, the enemy will still be poisoned after you load back your save file). You can find inspiration on how to implement the `Clone` method by looking at existing temporal effects, such as `TemporalDamageEffect`.

## Built-in effects
Built-in effects should cover most cases.
⚡ **Immediate (applied once):**
- `ImmediateDamageEffect`: applies damage
- `ImmediateHealEffect`: heals a target
- `ImmediateRestoreManaEffect`: restores the target's mana
⌛ **Temporal (over-time):**
- `TemporalControlEffect`: restricts actions on a target for a period of time
- `TemporalDamageEffect`: applies damage over time
- `TemporalHealEffect`: heals over time
- `TemporalRestoreManaEffect`: restores mana over time
- `TemporalSpeedModifierEffect`: modifies the target's move speed for a period of time
- `TemporalStatModifierEffect`: modifies a target's given stat for a period of time

## Using effects in abilities
Most abilities use effects, and ability sheets generally expose a list of `IEffect` for you to populate. Since each ability might apply the effects at different times, refer to the implementation of each ability to understand when the effects will be triggered. For instance, the `ProjectileAbility` will always apply its effects when its projectiles hit a valid target, and the `SelfCastAbility` will always apply its effects when the cast animation sends the `OnCast` animation message (which is about halfway through the casting animation).

In addition, any ability that can be cast will expose a second list of effects called "Auto Applied Effects To Caster On Fire". The effects in this list will be applied when the ability is cast (independently of any animation) and will target the caster. This is useful for applying effects like "increased movement speed" when an ability is cast, such as shooting an arrow.

Note that when trying to apply effects to the caster (i.e., when a character boosts or heals itself), abilities using the "Self_Cast" prefab will typically rely on the first "Effects" list (since the effect application time is dictated by the `OnCast` animation message, as we discussed earlier), while "Auto Applied Effects To Caster On Fire" is a quick and easy way to apply effects when an ability is cast (without relying on the animation). The "Effects" list offers more flexibility, allowing for greater control over when and how the effects are applied.

## Some Important Settings
You'll find lots of settings under each effect, which gives you lots of control regarding the execution and behavior of each and every effect. Some important ones to keep in mind are:

**Effect Data > Target Flags**: this setting allows the effect to "skip" application if the potential target doesn't match the requirements. For instance, if you want to create a castable healing orb, you might want to use a `ProjectileAbilitySheet`, and set effects to have an `ImmediateHealEffect`, with a target flag set to "Allies". Otherwise, your healing orb might also heal enemies! For most damaging effects, you might want to set the target flags to "Enemies". For self-cast abilities, you might want to use "Self", otherwise nothing will happen!

**Effect Data > Interruption Policy**: when working with an ability that has multiple effects, the interruption policy lets you decide what should happen to the execution flow if one of the effects failed to apply (i.e., the effect missed, wasn't applicable, or else). By default, the interruption policy is set to "After Fail", but you could decide to set it to "After Success" to try applying multiple effects and stop once one of them succeeded, or "Never" if you want all effects to apply no matter what. For instance, if an arrow deals damage and then slows, having the damage effect set up with an interruption policy of "After Fail" makes sense, so that if the arrow damage misses, the slow won't be applied.

ℹ️ Effects can seem daunting at first, given the depth and number of settings they provide. As always, looking into the abilities already in the demo game can be an invaluable source of information! Try duplicating any ability from the demo game, playing with its effects settings, and assigning it to one of your heroes through their `HeroSheet`.