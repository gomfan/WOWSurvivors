# Stats
> On this page, we'll learn about Mythril2D stats system.

In Mythril2D, character statistics are composed of several attributes:
- ❤️ **Health:** Determines the amount of damage a character can take before succumbing to defeat. Improving this stat increases overall survivability.
- 💪 **Physical Attack:** Determines the amount of damage dealt by physical attacks, such as sword strikes or arrow shots.
- 🧙 **Magical Attack:** Governs the potency of magical spells and abilities, allowing for the manipulation of elements and the casting of powerful spells.
- 🛡️ **Physical Defense:** Determines the amount of damage absorbed from physical attacks, mitigating the harm dealt by blades and arrows.
- 🍥 **Magical Defense:** Governs the character's resistance to magical attacks, reducing the damage taken from spells.
- ⚡ **Agility:** Increases the chances of dodging a hit, and reduces the chance of missing a hit.
- 🍀 **Luck:** Increases the chances of critical hits.

Increasing these attributes allows players to create unique characters to match their gameplay style.

## Changing Attribute Names & Icons
Each attribute can be renamed directly in the editor through the `GameConfig` (see "Game Config" documentation).

## Changing Damage Calculation Formula
Damage calculation can be customized for your game by editing the code in the `DamageSolver` class.

## Changing Max Level
The maximum level is by default set to 20. You can change this by editing `MaxLevel` in the Constants script (see `Constants.cs`).