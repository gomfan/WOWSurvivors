# Database
> Learn about Mythril2D's database system, and how it can help you manage your game's assets.

**ℹ️ The database is where you'll spend most of your time browsing your game content, so make sure you get familiar with it! Whenever you select an element in the database, the project view (where you typically browse assets) will be updated to focus on the selected asset. You can leverage this to quickly browse your game data!**

## Database Window <`EditorWindow`> (*Window > Mythril2D > Database*)
Mythril2D's database window is a convenient editor window for browsing your `DatabaseEntry`. You can search for entries by type, such as `HeroSheet`, `Item`, or `Quest`. To open the database window, click on the "Window" option in Unity's toolbar, navigate to "Mythril2D", and then select "Database" from the drop-down menu.

## DatabaseEntry <`ScriptableObject`>
A `DatabaseEntry` is an extension of a `ScriptableObject` that works in tandem with a `DatabaseRegistry`, so that an asset can be referenced in a file saved on the disk at runtime, using a `DatabaseEntryReference`, and loaded from anywhere. It is the base class for most `ScriptableObject` used in Mythril2D, such as: `Item`, `Quest`, `CharacterSheet`, `DialogueSequence` etc...

## DatabaseRegistry <`ScriptableObject`> (*Create > Mythril2D > DatabaseRegistry*)
The `DatabaseRegistry` is a `ScriptableObject` that contains references to all the `DatabaseEntry` in your project. It is used to store and manage all the data you create for your game, such as characters, items, quests, and abilities. The `DatabaseRegistry` is automatically updated when you create or delete a `DatabaseEntry`, ensuring that your database is always up-to-date. This registry is referenced in the `GameConfig` (see "Game Config" documentation Manager), and you shouldn't need to create more than one registry, unless you know exactly what you're doing! The demo game already comes with a registry, located at: *Assets/Mythril2D/Demo/Database/REGISTRY_Default*.