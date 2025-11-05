# Abilities
> On this page, we'll learn about Mythril2D abilities.

Abilities rely on three important concepts:
- 📜 `AbilitySheet`: a `DatabaseEntry` that stores the settings of an ability.
- ⚙️ `AbilityBase`: a component that implements the logic of the ability and relies on an ability sheet to alter its behavior.
- 🪄 `IEffect`: used by most abilities. Classes that implement this interface contain gameplay logic that can be either immediate (applied once) or temporal (lasting for a given duration).

This page will focus on the first two concepts: `AbilitySheet` (`DatabaseEntry`) and `AbilityBase` (`Persistable`).

> To learn more about effects, you can read the "Effects" page.

## AbilitySheet <`DatabaseEntry`> (*Create > Mythril2D > Abilities > ...*)
Similar to character sheets, ability sheets allow you to define settings for your abilities, such as the name, icon, and description. The `AbilitySheet` class is the base class for any type of ability, including `ActiveAbilitySheet`, which specifies settings for an ability that can be cast (most abilities), and `PassiveAbilitySheet` (for abilities that execute in the background when certain conditions are met).

There are many types of `AbilitySheet` that you can use to create your own abilities. All abilities that you can cast (`ActiveAbilitySheet`) expose a list of effects in the inspector, allowing you to use this list in your ability logic. It's up to you to decide when these effects should be applied! For example, the `ProjectileAbility` (which requires a `ProjectileAbilitySheet` to work) will pass these effects to any projectile spawned. The projectile will then attempt to apply these effects (using the `EffectDispatcher`) to anything it collides with.

## Ability prefabs
For each ability sheet you create, you must include a reference to a prefab containing the actual visuals and gameplay code of your ability. This prefab must include an ability script that is compatible with the type of ability sheet you created. For example, if you create a prefab with a `DashAbility` component for your ability, you must create a sheet of type `DashAbilitySheet` and add a reference to your prefab in this sheet.

## Adding abilities to your characters
Heroes and monsters can be assigned abilities through their respective character sheets. When a character using a `HeroSheet` or a `MonsterSheet` is spawned, the corresponding ability prefabs for each ability the character possesses will be automatically instantiated.

## Default equipped abilities
To choose which abilities each character will have equipped by default, you need to update the save file for the hero you want to edit. For instance, if you want the archer to have different startup abilities, you can open the `SF_Archer` save file in the inspector, navigate to "Content > Player > Hero Data > Equipped Abilities", and modify the values in this list.