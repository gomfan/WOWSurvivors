# Tilemap Layers
> On this page, we'll learn about Mythril2D tilemap layers.

## What are layers used for?
In the Mythril2D demo game, the environment is rendered using Unity tilemaps. The maps in the demo game are set up to use a combination of 4 layers: A, B, C, and Default. Each tilemap layer has its own tilemap collider and is rendered with its own sorting layer. The purpose of using multiple tilemaps is to visually represent depth in the environment. For instance, you may want a mushroom tile (with a transparent background) to be displayed on top of a grass tile, so placing the grass tile on Layer A and the mushroom tile on Layer B would be a good idea. It's up to you to decide what goes on each layer, as well as how many layers you want to use.

## Unity layer types
Unity has two types of layers: "regular" layers, and sorting layers:
- "Regular" layers: determine how GameObjects should interact with each other (Physics)
- Sorting layers: determine how GameObjects should be rendered (in which order)

## Demo game layers
In each map of the demo game, you'll find these GameObjects:
### Environment > Layer A
- Layer: "Collision A"
- Sorting layer: "Layer A"
### Environment > Layer B
- Layer: "Collision B"
- Sorting layer: "Layer B"
### Environment > Layer C
- Layer: "Collision C"
- Sorting layer: "Layer C"
### Environment > Layer Default
- Layer: "Default"
- Sorting layer: "Default"

> Note: in the demo game, Layer A is set up (See "Game Config") so that the player can collide with it (i.e., if you put water on Layer A, the player won't be able to go through it). Layer A is however excluded from the "Visibility Contact Filter" (See "Game Config"), which means anything that the player can collide with on this layer won't prevent the AIs from seeing through.

## Sorting order
Entities (characters, chests, projectiles, etc.) are rendered in the Default sorting layer. When both entities and tiles exist on the Default layer, their drawing order is determined by their Y coordinates. This creates a natural depth effect where objects lower on the screen appear in front of objects higher on the screen.

For example, if you want characters to walk behind objects like trees, rocks, fences, or houses, place these tiles on the Default layer. Unity will automatically draw the character behind the object when the character's Y position is greater than the object's Y position, creating the illusion that the character is passing behind the object.