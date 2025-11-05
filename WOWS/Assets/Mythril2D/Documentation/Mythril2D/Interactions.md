# Interactions
> On this page, we'll learn about Mythril2D's interaction system.

An interaction in Mythril2D is a way to define a behavior that executes when the player uses the interaction key while looking at an entity and when certain conditions are met. Interactions can call commands or execute more complex entity-dependent behaviors. Their execution flow can be controlled based on the result of a previous interaction. If you only want to execute a command, you should consider using a `CommandTrigger` instead (see Commands documentation).

## IInteraction <`Interface`>
`IInteraction` is the base interface for all interactions in Mythril2D. It contains a single method: `TryExecute`, that is called when the interaction is triggered. `TryExecute` is expected to return `true` if the interaction has been executed, and `false` otherwise. Depending on your use case, you could chain interactions based on the return of the `TryExecute` method, allowing you to create complex interactions, like, for instance, trying to complete a quest, and if this interaction didn't return `true`, showing a dialogue message.

## Built-in interactions
Mythril2D comes with a few built-in interactions that you can use out of the box, such as the `DialogueInteraction`, `QuestInteraction`, and `ShopInteraction`. These interactions are used in the demo game to create dialogues, quest interactions (giving a hint, offering, or completing a quest), and shops. Some more advanced interactions are also available, such as the `SequentialInteraction`, which allows you to chain multiple interactions together, and the `ConditionalInteraction`, which allows you to execute an interaction based on a condition.

## Creating a custom interaction
You can also create your own interactions by implementing the `IInteraction` interface.
