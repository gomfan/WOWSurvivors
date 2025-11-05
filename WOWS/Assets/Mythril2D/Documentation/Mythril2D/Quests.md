# Quests
> On this page, we'll learn about Mythril2D quests.

## Quest <`DatabaseEntry`> (*Create > Mythril2D > Quests > Quest*)
A `Quest` in Mythril2D is a `DatabaseEntry` that contains a list of tasks that the player has to execute to receive a reward and/or execute a specific command (see Commands). Quests are offered by NPCs and must be given back to the same or other NPCs. You can also set a level requirement for a quest, making it only available to players of a certain level.

## QuestTask <`DatabaseEntry`> (*Create > Mythril2D > Quests > Tasks > ...*)
A `QuestTask` is also a `DatabaseEntry`. It is the base class for any task that can be added to a quest, and can be expanded programmatically to create new objectives. By default, Mythril2D includes several task types: `ItemTask`, `KillMonsterTask`, `TalkToNPCTask`, and `GameFlagTask`.

## ItemTask <`QuestTask`> (*Create > Mythril2D > Quests > Tasks > ItemTask*)
An `ItemTask` is a task where the player needs to collect a specific item. For example, if a quest asks the player to collect 4 crystal balls, you must make sure that these crystal balls cannot be disposed of; otherwise, the player could make your quest unfinishable by, for instance, selling your quest item (note: you can make an item non-sellable by setting its price to 0, or its category to "Key").

## KillMonsterTask <`QuestTask`> (*Create > Mythril2D > Quests > Tasks > KillMonsterTask*)
This task requires the player to kill a certain monster a specific number of times.

## TalkToNPCTask <`QuestTask`> (*Create > Mythril2D > Quests > Tasks > TalkToNPCTask*)
This task requires the player to talk to a given NPC to move forward in the quest progression.

## GameFlagTask <`QuestTask`> (*Create > Mythril2D > Quests > Tasks > GameFlagTask*)
This task requires specific game flags to be in a certain state. This can be useful for quests that require finding something or interacting with various objects. It's used in the demo game for the ambush by the bandits. Each bandit sets a unique game flag when they die, and the quest needs all these game flags to be set to progress.