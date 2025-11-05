# Checkpoints
> On this page, we'll learn about Mythril2D checkpoints.

**Note: Checkpoints are important to understand if you want to update the start location of your player in a save file!**

## Checkpoint <`Persistable`>
A `Checkpoint` in Mythril2D is a component that can be added to a game object to turn it into a location that can be used for teleportation, including respawning. This is useful for the `RespawnPlayer` command, which will automatically look through all the checkpoints the player has saved.

## ICheckpoint <`interface`>
This interface can be implemented to specify different types of checkpoints. There are several areas where you may need to set a checkpoint, such as the save file, which contains a list of `ICheckpoint` under "Content > Map".

All that `ICheckpoint` does is require its implementation to provide a map name (`string`) and a position (`Vector3`). The way the map and location are retrieved depends on the checkpoint implementation.

Whenever you need to set a checkpoint, you can select the implementation you want to use:
- **SimpleCheckpoint**: This is the simplest implementation; it lets you choose a map and a location (`Vector3`).
- **GameObjectCheckpoint**: This lets you choose the map and name of the GameObject to retrieve the location from.
- **PersistableCheckpoint**: This expects a reference to a `Persistable` entity (such as a `Checkpoint` component or a `Teleporter`). This reference can be set "by object" or "by identifier". The object mode lets you select a `Persistable` from the current scene, while the identifier mode lets you select a `Persistable` from any scene. To find the identifier of a persistable in another map, you need to open this map and select your `Persistable` object in the hierarchy. In the inspector, under "Persistable Settings", you'll find an identifier that you can copy and paste wherever you need. When using the identifier mode, don't forget to also select the map where this identifier is located.

## Saving a Checkpoint
You can save any checkpoint at any point during gameplay using the `SaveCheckpoint` command or using the "Save Checkpoint On Arrival" toggle on a teleporter.