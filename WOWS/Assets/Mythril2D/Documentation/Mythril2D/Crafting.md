# Crafting
> On this page, we'll learn about Mythril2D crafting system.

## Recipe (*Create > Mythril2D > Crafting > Recipe*)
A Recipe in Mythril2D is a `DatabaseEntry` that contains a list of ingredients and their quantities (input) as well as an item (output). Creating a recipe is necessary for each item you want your players to be able to craft. You can also set a crafting fee, which will require your players to have a set amount of your game currency to craft this recipe.

## CraftingStation (*Create > Mythril2D > Crafting > CraftingStation*)
Similarly to shops, a `CraftingStation` is a `DatabaseEntry` that allows you to create a set of recipes to be used somewhere in your game, whether it's from your players' inventories, or through a specific NPC (see NPCs), as well as defining additional fees your players will need to pay to craft items.

## "On The Go" CraftingStation
This special `CraftingStation` can be set through the `GameConfig` (see Game Manager), and will determine which recipes should be made available to your players directly through the pause menu (which we usually refer to as "On The Go" crafting). You can also set this field to "None" in the `GameConfig`, this will prevent the crafting menu from showing up in the pause menu. Even if the "On The Go" crafting station isn't set, you can still create `CraftingStations` and NPCs to allow your players to craft at specific locations (such as an alchemist, a blacksmith, etc...).