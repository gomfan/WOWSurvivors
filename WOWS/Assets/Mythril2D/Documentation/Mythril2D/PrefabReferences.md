# Prefab References
> On this page, we'll learn about prefab references in Mythril2D.

## PrefabReference <`DatabaseEntry`> (`Create > Mythril2D > Save > PrefabReference`)
A `PrefabReference` is a `DatabaseEntry` that contains a reference to a prefab. References to prefabs cannot be saved in Unity, which quickly becomes an issue when you want your save files to contain information about which prefab a particular runtime-instanced GameObject was created from. The `PrefabReference` solves this issue by inheriting from `DatabaseEntry`, which makes it serializable by giving it a unique identifier (GUID).

## Player prefab reference in a save file
Save files need to store which prefab (character) the player chose (Archer, Wizard, Knight...). In order to do that, default save files: `SF_Archer`, `SF_Knight`, and `SF_Wizard`, must reference a `PrefabReference` instead of a regular prefab (GameObject).