# Entities
> On this page, we'll learn about entities.

**ℹ️ before diving into entities, it's worth mentioning that creating an `Entity` isn't the only way to make something interactable in your game. For instance, you could use a simple `CommandTrigger` script, attached to a GameObject on the `Interactable` layer. An `Entity` comes in handy when you need more advanced interactions that can affect the entity itself. An `Entity`, as opposed to a simple `CommandTrigger`, can have its state saved in save files.**

## Entity <`Persistable`>
The `Entity` class is the base class for pretty much anything that can interact, or can be interacted with. Think of it as Mythril2D's main component that your GameObjects should have if you want them to interact or be interacted with. An interactable sign, a chest, or even a lever for your players to toggle, are all entities. Monsters, NPCs, and even Heroes (such as your player), inherit from `Movable`, which itself inherits from `Entity`. Since entities also inherit from `Persistable`, you can save their state in a save file. Custom entities can also override the `Entity.OnSave` and `Entity.OnLoad` methods to store any information they might need to persist between sessions.

## Defining an interaction for an entity
Entities become interactable when they have an interaction defined through the inspector. An "Interaction" can actually be many things, such as a single action, a conditional action, a sequence of actions, and more! Learn more about the interaction system in the "Interactions" documentation!

## Adding an interactable entity to your scene
In order for a GameObject to use the interaction system, it must have an `Entity` component. You can add an `Entity` component to any GameObject by clicking on the "Add Component" button in the inspector, searching for "Entity", and clicking on it. Once the Entity component is added, you can define an interaction for the entity by changing the "Interaction" setting through the inspector. For a GameObject to be interactable, you'll need to set its layer to "Interaction", and add a rigibody and collider to it.

**⚠️ Important: Your entity won't be interactable until you set its layer to "Interaction"!**

## Floating Icons
Entities all have the ability to display an icon above their head, such as an exclamation mark, a quest indicator, a heart, etc.

For now, this can only be done programmatically in C#, using the `SetFloatingIcon` method. It's for instance used by the `NPC` class to update the icon based on the context (i.e. if the `NPC` has a quest to offer, a "?" icon will be shown, etc.).