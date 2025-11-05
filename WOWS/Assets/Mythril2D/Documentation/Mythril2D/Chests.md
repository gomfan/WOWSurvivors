# Chests
> On this page, we'll learn about chests in Mythril2D.

## Chest <`Entity`>
A chest is an entity that may contain a reward (items and/or money), and can be opened by the player. Since entities in Mythril2D can be made persistent (state saved in a save file), a chest can have its state (opened/closed) preserved between sessions.

## ChestInteraction <`IInteraction`>
The `ChestInteraction` class contains the logic to trigger the opening of a chest. It should be placed at the top of the interaction list for this entity. This interaction will return `true` if the chest was successfully opened, and `false` otherwise. With its "Interruption Policy" set to "On Success", you can add a second interaction that will be executed if the player interacts with the chest after it has already been opened, such as displaying a dialogue message: "This chest has already been opened!".