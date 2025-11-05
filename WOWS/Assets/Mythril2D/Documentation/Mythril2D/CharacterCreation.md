# Character Creation
> On this page, we'll learn how to create characters for your game.

## Step 1: Creating a `CharacterSheet`
The first step when making a new character is to create a `CharacterSheet` for it. It can be any type of `CharacterSheet` based on the type of character you are creating: `HeroSheet`, `NPCSheet`, or `MonsterSheet`. You can read more about `CharacterSheet` in the "Characters" documentation.

## Step 2: Importing or re-using a sprite sheet
Once you know what type of character you want to create, you need to decide what this character is going to look like. You can use one of the sprite sheets included in the demo game (they usually start with "SPS_"), but keep in mind that most sprite sheets in Mythril2D already have a `SpriteLibrary` associated with them, so you may want to skip this step if a `SpriteLibrary` already exists with the visuals you are looking for.

## Step 3: Creating or re-using a `SpriteLibrary`
Characters use a `SpriteLibrary` to determine their appearance. In the demo game, a `SpriteLibrary` typically contains animation frames (idle, run, hit, death) and a visual for the character's hand. Different characters often use different `SpriteLibraries`.

To fill in a new `SpriteLibrary`, you may need to import a sprite sheet or use an existing one. Note that in the demo game, each character's `SpriteLibrary` uses `Libraries/SLIB_Default` as their base library. This ensures that all created character libraries provide the same structure of information (4 animations + 1 hand visual). One `SpriteLibrary` usually only contains information for a single character orientation (left, right, up, down, etc.). In the demo game, character sprites usually have a single direction (right), and the left animation is obtained by flipping the sprite using the `BidirectionalCharacterAnimationStrategy`. If you'd like to use a 4-directional character animation instead, you'll need to create 3 (or 4) `SpriteLibraries` (Up, Down, Right, Left: can be obtained by flipping the "Right" `SpriteLibrary`).

> If you want to change how many animations your character can perform, you should update the base library (`SLIB_Default`). By default, this `SpriteLibrary` expects each animation to have 4 animation frames. If you want to change that, you will also need to edit the default `SpriteLibrary` (`SLIB_Default`) and update/create animation clips to use all the new frames & animations you added to `SLIB_Default`.

## Step 4: Creating a prefab variant
Once your visuals are properly defined, you can create a prefab variant. To do so, open the `Prefabs/Entities/Characters/` folder, and navigate to the default character you want to create your new character from. I recommend creating a prefab variant (*Right-click on a prefab > Create > Prefab Variant*) from one of these default characters: `0_Hero_Base`, `0_Monster_Base`, or `0_NPC_Base`.

## Step 5: Changing your character's settings
Different character types (monsters, heroes, NPCs) will display different settings, but some of them are shared between all types. The most important settings are:
- **Controller Settings**: you'll find an option to select which controller to use. By default, NPCs have no controller (they don't move or attack), heroes have a `PlayerController`, and monsters have an `AIController`. You can learn more about controllers in the "Characters" documentation.
- **Animation Strategy**: this setting lets you decide how your character will be animated. You can learn more about animation strategies in the "Characters" documentation.
- **Sheet**: you need to fill this in by referencing the `CharacterSheet` that you created in "Step 1".
- **Level**: especially important for monsters, since this will affect their stats.

## Step 6: Adding your character to a map
Generally, after you create a character, you want to add them to a map, except for heroes, which are often referenced in default save files (See "Save Files" documentation). To do this, you can simply drag-and-drop your prefab variant into a map. You can also override some settings of your character directly after it has been added to a map, but these changes won't be shared between all instances of the same character.
