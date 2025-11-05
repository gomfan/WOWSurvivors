# Persistables
> On this page, we'll learn about persistable objects.

## Persistable <`MonoBehaviour`>
A `Persistable` in Mythril2D is a `MonoBehaviour` whose state can be saved in a save file. For instance, during gameplay, the state of an element may change (from enabled to disabled, or even destroyed). If you want to save this state between sessions (using the save system), you need to add the `Persistable` component to your GameObject.

Most things in Mythril2D already inherit from `Persistable`, such as entities (and therefore all characters), chests, projectiles, abilities, etc. Classes that inherit from `Persistable` can implement the `OnSave` and `OnLoad` methods:
- `OnSave(PersistableDataBlock block)`
- `OnLoad(PersistableDataBlock block)`

Implementing these methods allows you to save any data you need to keep between sessions. For instance, by default, entities save their position, rotation, and scale; characters save their stats; abilities save their cooldown, etc. The persistence system (See Game Systems) is responsible for saving and restoring the state of all persistable objects whenever a map is loaded, unloaded, or the game is saved/loaded.