# Save Files
> On this page, we'll learn about Mythril2D save files.

## SaveFile <`DatabaseEntry`> (*Create > Mythril2D > Save > SaveFile*)
A `SaveFile` in Mythril2D is a `DatabaseEntry` that contains settings defining the initial state of the game when creating a new game. It includes settings like player settings, initial checkpoints (where the player should start their adventure), inventory, unlocked quests, abilities equipped by default, and more.

## Important Sections
There are a few important sections in save files to be aware of when building your game.

📖 **Content > Journal > Unlocked Quests**: Unlocked quests define which quests the players will be able to start, and therefore complete, from the beginning of the adventure. It could contain, for instance, the first quest of the main storyline and a set of side quests. Quests can also be set up to unlock other quests.

✅ **Content > Map > Checkpoint**: Checkpoints are locations (map + position) that can be used to initially spawn the player (when starting a new game), or respawn (after death, for instance). You should always make sure there is at least one checkpoint included. The demo game contains one checkpoint for each save file (`SF_Archer`, `SF_Knight`, `SF_Wizard`, `SF_Devon`), which references the game start location in the map: "Brusselia_Forest". You can read more about checkpoints on the "Checkpoints" page.

📦 **Content > Player > Instance**: This field expects a `PrefabReference`; you can read about them in the "PrefabReferences" documentation. The value provided here will determine which prefab to use for the player when loading a save file.

🚩 **Content > Game Flags > Flags**: This list of strings can be populated to set some game flags to `true` when the game starts. Game flags can influence gameplay by creating branches (e.g., if the game flag "player_has_visited_this_location" is set, then this command should execute; otherwise, this other command should). When playtesting, you can add extra game flags to your playtest save file (`SF_Devon` by default) so you can skip cutscenes and other elements you might not want to encounter during debugging.

## Using a SaveFile
After creating a new save file, you need to add a way to start the game with it. Save files can be used in the main menu as an argument passed to the `UIMainMenu.StartNewGameFromDefaultSaveFile` method. In the demo game, the main menu contains 3 buttons to start a new game (4 in debug mode): Wizard, Knight, Archer, and Devon (in debug mode). Each of these buttons, when clicked, will call the `UIMainMenu.StartNewGameFromDefaultSaveFile` method using the given save file as an argument. Your newly created save files can be referenced from the inspector of these buttons, under the Button component's `OnClick` event.