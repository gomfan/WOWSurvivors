# Game Systems
> On this page, we'll learn about Mythril2D game systems.

## What are game systems?
Mythril2D uses a system approach to handle gameplay logic that should be shared between maps, such as the inventory system, the audio system, the journal system, etc. Systems are generally set up under the `GameManager` in the Main Menu and M2DEngine scene, meaning that they should be instanced once and stay alive even when the currently loaded map changes.

## M2DEngine
This scene typically contains all the game systems. It is automatically loaded first, before any map, whether you are playing your game through the editor or in a build.

## InputSystem <`AGameSystem`>
Organizes all the inputs set up in Unity's Input System and exposes them to other scripts. It also handles input locking (during a map transition, for instance) and action map switches.

## JournalSystem <`AGameSystem`>
Stores and handles the player's quests, including operations such as starting a quest and completing a quest.

## NotificationSystem <`AGameSystem`>
Contains several events that can be subscribed to by other systems or scripts and invoked to react to gameplay events, such as dispatching an audio playback request.

## SaveSystem <`AGameSystem`>
Handles the creation, deletion, and loading of save files.

## GameStateSystem <`AGameSystem`>
Keeps track of game states, such as Gameplay and UI, to allow systems and scripts to adapt themselves based on the current game state.

## GameFlagSystem <`AGameSystem`>
Manages game flags that can be used to track game progression and trigger events. Game flags are typically used to create conditional behavior in your game.

## MapSystem <`AGameSystem`>
Handles the loading and unloading of maps. When set up with delegated transition responsibility, it will delegate its operations to allow for an external system or script to initiate and stop the loading/unloading.

## DialogueSystem <`AGameSystem`>
Stores a reference to the main dialogue channel, which will handle all the dialogue logic. Can be extended to support multiple channels, such as: dialogue message box that pauses the game (currently used in the demo game), and world-space speech bubbles on top of a character.

## PlayerSystem <`AGameSystem`>
Handles the instantiation of the player prefab. By default, the `PlayerSystem` will spawn the prefab set in its "Dummy Player Prefab" field. Do note that the save system will have the last say on which prefab the game should use as the player. The dummy prefab will generally be used when no save file is loaded.

## AudioSystem <`AGameSystem`>
Handles multiple `AudioChannel` for `AudioClipResolver` to be played (see Audio). You can set up your channel list as you see fit, with up to 5 different channels: background music, background sound, gameplay sound FX, interface sound FX, and miscellaneous.

## PersistenceSystem <`AGameSystem`>
The persistence system ensures each component inheriting from `Persistable` is saved and loaded properly. It is necessary for your game to save and load data about your game elements.

## TransitionSystem <`AGameSystem`>
Provides a way for maps to trigger a fade-out/fade-in animation when transitioning between scenes.

## UISystem <`AGameSystem`>
Ensures the UI is instantiated at the right time (after the gameplay systems are initialized) and provides utility methods to show and hide the user interface.
