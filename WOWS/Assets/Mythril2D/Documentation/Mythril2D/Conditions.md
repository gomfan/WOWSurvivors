# Conditions
> Conditions can be found in many locations: conditional activators, interactions, and more! They are important to understand and should be expanded depending on your production's needs.

## ICondition <`Interface`>
This interface is the base interface for any condition to implement. It has 3 methods that need to be implemented by inheriting classes:
- `bool Evaluate()`
- `void StartListening(Action onStateChanged)`
- `void StopListening()`
Although it's possible to create a condition from `ICondition`, it's recommended to inherit from `ABaseCondition` instead, which provides some additional functionalities to inheriting classes.

## ABaseCondition <`ICondition`>
Abstract class providing basic functionalities for inheriting conditions.
It's recommended to inherit from this class when creating custom conditions.

## Built-in conditions
Mythril2D comes with a bunch of pre-implemented conditions, such as:
- `IsGameFlagSet`: checks whether a game flag is set
- `AreConditionsMet`: checks if two conditions are met with 2 different operators: Any (OR), All (AND)
- `IsAbilityUnlocked`: checks whether an ability is unlocked
- `IsItemInInventory`: checks whether an item is in the player's inventory
- `IsNot`: checks if a condition is not true (negates the result of a condition)
- `IsQuestInState`: checks if a quest is in a given state
- `IsQuestTaskActive`: checks if a quest task is active
- `IsQuestTaskInState`: a more powerful condition than `IsQuestTaskActive`, allowing you to check if a quest task is in one or multiple states