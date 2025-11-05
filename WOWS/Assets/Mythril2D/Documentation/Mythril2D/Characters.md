# Characters
> On this page, we'll learn about Mythril2D characters.

## What is a character?
A character is a game entity that can fight, evolve (increase its stats), and potentially interact with the player. There are currently 3 main categories of characters:

🧌 **Monsters**: their stats are automatically calculated based on their level.
🧝 **Heroes**: their stats can be customized (typically by the player).
🙋 **NPCs**: they are generally interacted with (shops, inns, crafting...), and can be used as a way to make your character progress in your storyline through quests.

## CharacterSheet <`DatabaseEntry`> (*Create > Mythril2D > Characters > ...*)
A `CharacterSheet` is a `DatabaseEntry` that serves as the base class for specific character sheets, such as `HeroSheet`, `MonsterSheet`, and `NPCSheet`. Creating a character sheet of any type is the first step in creating any game character.

## Controllers
Characters can be set to use different types of controllers.

### AIController <`AController`>
The `AIController` is a type of controller (`AController`) that allows a character to automatically determine which movement to perform or ability to fire based on its environment. You can extend the `AIController` class to create unique behaviors for your monsters. The current `AIController` doesn't support multiple abilities or advanced decision-making.

### PlayerController <`AController`>
The `PlayerController` lets you manually control a character with a gamepad, keyboard, or other input device. When this controller is selected, you can move the character around, interact, and use abilities directly.

## Animation Strategies
An animation strategy can be selected for each character. In the demo game, all characters use a "bidirectional" animation strategy, meaning that characters can look either right or left.

- **Bidirectional**: the default one used in the demo game, it allows your character to automatically flip its sprite when looking left (so you can visually show 2 animations: left and right, while having only one animation: right).
- **Polydirectional**: allows your character to have up to 8-directional animations, which you can set up by changing the "Animation Direction Overrides" setting. You can also check out the `Prefabs/Entities/Characters/Heroes/Archer_4_Directional` prefab for reference, as it already uses this animation strategy.
- **Axis-based**: allows your animator controller to receive the x and y speed of your character, for you to implement any animation logic you want directly in your custom controller (more advanced, but gives you more control. Recommended to be used when working with blend trees, if the polydirectional strategy isn't sufficient).

> Note: These animation strategies all use the same animator controller, so you don't need to add extra logic to it! For instance, with the polydirectional strategy, blend trees to handle animation directions aren't necessary since this system already switches between sprite libraries on the fly depending on the character's direction.

You can also expand the animation system by implementing your own character animation strategy. To do this, inherit from the `AAnimationStrategy` C# class and select your custom strategy under any `NPC`, `Monster`, or `Hero` component.

## UICharacterInfo <`MonoBehaviour`> 
Under every character prefab, you'll find a "Canvas" GameObject with a `UICharacterInfo` component. This component is responsible for showing and updating UI information about the character, such as the health and mana bars, the name tag (including the level), and the active effects.

You can open any character prefab and toggle one of the following GameObjects: "Effects", "Name", "Health", "Mana".
I'd recommend toggling these GameObjects directly on the base prefabs: `0_Character_Base`, `0_Monster_Base`, `0_NPC_Base`, and `0_Hero_Base`. This way, you can show the same UI over characters of the same type. For instance, in the demo game, all the monsters show their health bar, active effects, and name tag (with level). While other characters (NPCs and Heroes) don't show anything. You could, for instance, modify the `0_NPC_Base` prefab and enable the "Name" GameObject, which will effectively enable the name tag for all your NPCs. Of course, you can always override these settings per instance (i.e., a boss or a mini-boss might want to show its mana bar).
