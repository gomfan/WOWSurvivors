# Mythril2D: Changelog
## Version 3.3
### Features
- Added angle snapping option to polydirectional abilities.
- Added a settings button to the main menu.

### Fixes
- Fixed missing sprite in melee idle animation.
- Fixed monster spawner doesn't spawn extra monsters in release builds.
- Fixed walk animation breaking after respawn.
- Fixed incorrect summon stats when using the "increase level to match summoner" option.
- Fixed abilities not being properly removed.
- Fixed bonus abilities being counted multiple times after saving and loading.
- Fixed ability count in ability menu not updating after leveling up.
- Fixed invalid camera position when a camera shake occurs during a camera movement command.
  
## Version 3.2
### Features
- Added a command to add or remove mana.

### Fixes
- Fixed an issue where the journal quest list didn't scroll automatically.
- Fixed an error that occurred when unloading a map while standing in a command trigger collider.
- Fixed the broken cooldown visual after using a no-cooldown ability.
- Adjusted the monster spawner to properly enforce cooldowns between respawns.
- Fixed the damage solver to use current stats instead of max stats.
- Improved the temporal stat modifier to prevent dying after a health boost or having negative mana.

## Version 3.1
### Features
- Updated quest interaction order to: "Try to complete quest" > "Try to complete task" > "Try to offer quest" > "Try to give hint."
- Cleaned up the "M2DEngine" scene by removing a duplicated `UISystem` instance and resetting system positions to (0.0, 0.0, 0.0).
- Added a warning message when an ability not available to a character is marked as equipped in a save file.
- Added documentation for the camera.

### Fixes
- Fixed a bug that prevented save files from being properly deleted.
- Fixed missing references in `4_Directional_Archer`.
- Fixed quest floating icon not matching the quest interaction flow.
- Fixed incorrect character name being displayed when a dialogue is created from `TalkToNPCTask`.
- Fixed a bug where a quest instance would be duplicated if its tasks were fulfilled before the quest had started.

## Version 3.0
### Features
- Updated to use Unity 6000.0.43f1 (new recommended version for any project using Mythril2D).
- Reworked core systems:
  - Reworked movement system, now cleaner and separated from `Character` code into its own `Movable` class.
  - Reworked the dialogue system to be asynchronous.
  - Reworked commands & interactions to be asynchronous, enabling proper sequential execution while waiting for previous commands to complete.
  - Reworked playtest mode to better represent real game conditions by replacing the `PlaytestManager` with editor-specific code that automatically loads the "M2DEngine" scene layer first.
  - Reworked the NPC icon system into a more versatile "Floating Icon" system, usable by any entity.
  - Added `Persistable` class, allowing any game element to be saved and reloaded between sessions.
  - Added `PersistableReference` database entry for saving references to `Persistable` objects in save files.
  - Checkpoint system for player respawning and defining initial player position
  - Reworked teleporter system to leverage the new checkpoint system.
  - Improved database performance, especially noticeable with larger projects.
- Added character and animation improvements:
  - Support for multiple character animation strategies, including polydirectional (4-directional & 8-directional).
  - Option for character instances to override the alignment specified in their associated `CharacterSheet`.
  - Added a new hero: Devon, playable in debug builds and from the editor, with all available abilities for faster playtesting and iteration.
  - Added `CharacterInfo`, replacing the `MonsterInfo` class, now usable on any type of character.
  - Added support for name tags in `CharacterInfo`, allowing any character to display their name and level above their head.
- Enhanced ability system:
  - Added dozens of new abilities including summoning, status effect arrows, and self-boosting abilities.
  - Added cooldown system for abilities.
  - Added a system for abilities to apply customizable immediate effects (damage, heal, etc.) and temporal effects (status).
  - Added option for equipment to grant bonus abilities while equipped.
  - Added an option to add an explosion radius to any projectile ability.
  - Added a passive ability to restore mana over time.
  - Added a cooldown per target to the corrosion ability, enabling the disabling of invincibility frames for more challenging gameplay.
  - Added new icons for various abilities.
  - Updated the ability menu to display active and passive abilities with improved UX when equipping abilities.
- Improved item and equipment system:
  - Reworked the item system to have only 2 base classes: `Item` and `Equipment`.
  - Introduced a new `ItemEffect` system to specify item behavior when used.
  - Added support for negative stats on equipment.
  - Added extra validation to prevent characters from equipping or unequipping items that would be fatal.
  - Added new items: reviving orb (enables respawning on death) and ability unlock scrolls.
- Added control and interface improvements:
  - Mouse support for targeting and UI navigation.
  - Message display above the ability bar when casting fails, explaining the reason (insufficient mana, ability in cooldown, etc.).
  - Improved pushback direction based on hit location, with support for custom overrides.
- Added new commands:
  - Destroy entities
  - Wait for specified seconds
  - Toggle controller states
  - Move characters
  - Move camera
  - Open any menu
  - Save checkpoints
  - Teleport to checkpoints
  - Respawn or revive player
  - And many more!
- Added new conditions:
  - `IsGameFlagSet`
  - `IsQuestTaskActive`
- Added new quest tasks:
  - `TalkToNPCTask`
  - `GameFlagTask`
- Added improved documentation:
  - New built-in documentation system using markdown files instead of hard-coded C# strings.
  - Support for creating custom documentation.
  - Search functionality for faster information finding.
- Demo game updates:
  - Added two new maps: Brusselia Forest and Plains.
  - Added a new quest with in-game cinematics (camera and character movements).
  - Added a new NPC (Joe) and monsters (Fungus and Bandit).
  - Added new musics.
  - Added a "Made with Mythril2D" splashscreen at game start.
  - Added additional event log options to control when certain events are logged based on context.

### Fixes
- Fixed issue where dashing into obstacles sometimes caused characters to get stuck.
- Fixed projectiles occasionally passing through walls.
- Fixed potential loss of changes when switching between maps using Mythril2D's scene selector UI tool.
- Fixed case-sensitive database searches.
- Fixed incorrect character stat updates after equipping/unequipping items multiple times.
- Fixed errors occurring when a character died during dialogue.
- Fixed issue where characters could end up inside walls upon death.
- Fixed health bar offset problems after multiple deaths when resource bar shaking was enabled.
- Fixed incorrectly set tile rules that caused visual artifacts in paths and water.
- Fixed issue where enemies couldn't see through water.
- Fixed the scene selector not updating after creating a scene.
- Fixed the scene selector overflowing the screen when the project has too many scenes.

## Version 2.0
### Features
- Added a new monster: water slime (fast, but squishy).
- Added a crafting system with new items:
  - Slime Ball (Dropped by slimes).
  - Water Slime Ball (Dropped by the newly added water slime).
  - Water Bucket (Dropped by water slime or purchased at the store).
  - Bucket (Obtained after using a Water Bucket in a craft).
- Replaced the `ScriptableAction` system with a more versatile command-based system (`ICommand`):
  - Uses a subclass selector to select the command to execute.
  - Allows for easier authoring of new commands.
  - Provides better inspector visuals.
  - Doesn't require a `ScriptableObject` instance (like `ScriptableAction` did).
  - Integrates with Unity's Event System using a `CommandHandler`.
- Added a "GameObjects" palette to quickly populate your maps with prefabs.
- Added a scene overlay to navigate through the available scenes.
- Added new commands to open a shop or craft menu.
- Added item categories and inventory tabs.
- Revamped the monster spawner system:
  - Added 2 separate implementations:
    - `MonsterAreaSpawner`: spawns a monster anywhere within a collider's area.
    - `MonsterSpawner`: spawns a monster at one specific location.
  - Added a max monster count setting to the monster spawner.
- Added `ExecuteCommandIf` to create a branch in a command execution flow.
- Added `PlayDialogueLine` to execute a single line of dialogue without a proper `DialogueSequence`.
- Added a level requirement to quests, making unlocked quests not available until a certain level is reached.
- Added events for when an item is equipped or unequipped.
- Reworked the condition system:
  - Uses a subclass selector to select the condition to evaluate.
  - Supports nesting of conditions, condition lists with operators, and negation.
  - Easily extendable to add new conditions.
  - Improved memory efficiency compared to the previous system in place.
  - Integrates with the command system to check for conditions in any execution flow.
- Reworked the interaction system:
  - Introduced the `Entity` class, serving as the base class for all entities in the game.
  - Extended the interaction system to work with non-NPC entities.
  - Updated all NPCs and interactions to use the new system.
- Reworked the save system to introduce a database to manage references to `ScriptableObject`:
  - Added `DatabaseEntry`: a base class for a `ScriptableObject` that can be referenced by a GUID.
  - Added `DatabaseEntryReference`: a serializable class used to reference a `ScriptableObject` using its GUID (in a save file).

### Fixes
- Fixed a `MissingReferenceException` in some specific cases when using `UINavigationCursor`.
- Fixed an incorrect character reference being sent to `ANPCInteraction.Interact(sender)`.
- Fixed the "Stolen Heirloom" quest by re-adding the missing item in its chest.
- Fixed the Necromancer death animation when finishing the quest "Necromancy In The Vicinity".
- Fixed the `MonsterSpawner` prespawn not properly working with conditional activators.
- Fixed a rare `MissingReferenceException` occurrence with the `UINavigationCursor`.
- Fixed corrupted saved files after restarting the game.

## Version 1.4.1
### Fixes
- Corrected the position of the health bar when loading a save where the player's health is not full.
  
## Version 1.4
### Features
- Added an option for the camera to shake when the player takes a hit and/or when the player hits an enemy:
  - This option is enabled by default only when the player takes a hit.
- Added an option for stat bars (e.g., health bar, mana bar) to shake when their values decrease:
  - This option is enabled by default only for the health bar.
- Added game condition to check if an ability is unlocked.
- Added a line to the event log when an ability is added or removed.
- Added a `ScriptableAction` to add or remove an ability.
- Newly added abilities will automatically be equipped if a slot is available.
- Chests can now contain multiple items and currency.
- Added a "Settings" menu with volume settings for each audio channel.
- Added `playerSpawned` event to the `NotificationSystem`.
- Added "Select" and "Back" input indicators to game menus.
- Added an `EquipmentSpriteLibraryUpdater` component:
  - Different swords will now have different visuals when using sword abilities.
- Added a provocation system for enemies:
  - Hitting an enemy from afar now provokes the enemy even if the player is outside of the enemy detection radius.

### Fixes
- Fixed conditional state machines not being properly updated when some condition where met.
- Fixed enemies sometimes not facing the correct direction in combat.

## Version 1.3
### Features
- Added quest starter items.
- Added conditional loot on monsters.
- Added a death screen.
- Updated the demo game with a new quest that can be initiated by a special item dropped by skeletons (with a 10% drop rate).
- Updated documentation to include a section on save files.

### Fixes
- Minor fixes

## Version 1.2
### Features
- Added dash ability, unlocked by the archer upon reaching level 3.
- Implemented a new AI navigation system using context steering:
  - AI can now avoid simple obstacles (complex shapes, such as mazes, are not supported).
  - AI can no longer see behind objects, allowing the player to hide effectively.
  - When the AI loses sight of the player, it will move to the last known position and reset after a while if it cannot reestablish visual contact.

### Fixes
- Rectified projectile hitbox (preventing instances where they would hit a target behind the caster).
- Resolved a potential path normalization issue on UNIX platforms.
- Fixed player stats updates:
  - Increasing max stats (equip item, apply points...) will now properly adjust current stats by the same amount instead of resetting them to their maximum value.
  - Leveling up now restores the player's stats.
- Rectified typos in the triple shot and corrosion ability sheets.
- Removed unused code.

## Version 1.1
### Features
- Added the ability to execute scriptable actions when a dialogue starts.
- Added a permanent death setting for monsters.
- Added a new scriptable action to either heal or damage the player.

### Fixes
- Fixed a bug where the `booleanChanged` event (renamed to `gameFlagChanged`) wasn't triggering properly, resulting in some broken interactions with conditional activators.
- Fixed a bug where monster projectiles would sometimes remain attached to their caster.
- Fixed a bug causing teleporters without activation settings to teleport the player multiple times.

## Version 1.0
This is the first version of Mythril2D!
