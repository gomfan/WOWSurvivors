# Scenes
> On this page, we'll learn about the Mythril2D scene system and how you can create your very own maps.

## Scene System
Mythril2D uses Unity's additive scene loading and asynchronous loading. There are two main scenes that can be loaded: Main Menu and M2DEngine. These two scenes are standalone scenes that contain all the game logic, UI, and systems. However, loading only the M2DEngine scene is a bit boring, and you may find yourself in an empty world! To make your M2DEngine scene more interesting, read more about maps!

## Maps
A map can be viewed as a layer that Mythril2D loads on top of the M2DEngine scene. It typically consists of level design elements, such as decorations, gameplay components, characters, and music. You'll spend most of your time creating maps, while the M2DEngine scene will mostly remain unchanged. A map can be loaded from the `SaveSystem` (see Game Systems) when a save file is loaded, or by a `Teleporter` (see Teleporters) when the player interacts with it (e.g., by entering a house).

## Playtesting a Map
As mentioned earlier, only the Main Menu and M2DEngine scenes are actual standalone scenes. Maps require the M2DEngine to be fully functional. While in the editor, when you hit the "Play" button, the `EditorPlayModeOverride` script will execute and force the M2DEngine scene to load first, followed by the map. This allows you to playtest the map without having to manually add the M2DEngine scene layer to it.

## Changing the Default Playtest Save File
When playtesting a map, the default save file will be loaded. You can change which save file to start the playtest with by updating the game config (`CFG_Config`) and changing the "Playtest Save File" field value.

## Creating a Map
By default, if you create a new scene and hit play, not much will happen. For your scene to be identified as a map, it needs to have the `MapInfo` component somewhere. I recommend using the "Map Info" prefab already present in the demo game and placing it anywhere in your scene. Another effective approach is to duplicate an existing map from the demo game and start from there. This will give you a good structure to build your map, with tilemap layers already set up for you. You can then open the tile palette (*Window > 2D > Tile Palette*), select the erase tool, and erase everything on each layer (A, B, C, Default). After that, you can remove everything under "GAMEPLAY" and modify the background music under "AUDIO > Background Music".