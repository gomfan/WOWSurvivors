# NPCs
> On this page, we'll learn about NPCs.

NPCs are characters that players generally don't fight against but can interact with, especially for quests. Whether you want to set up a shop, an inn, display dialogue, or start, progress, or complete a quest, NPCs provide these functionalities. NPCs inherit their interaction capabilities from the `Entity` class (see Entities). You can create unique characters for your game and add depth to your story by developing meaningful interactions between NPCs and the player.

## NPC <`Character`>
An `NPC` is a component derived from `Character`, so it inherits all movement and combat logic. Although NPCs typically don't engage in combat, they technically could. NPCs extend functionality with the quest system, automatically updating the icon above their heads based on the player's quest status, and allowing players to interact with them to progress quests.

## NPCSheet <`DatabaseEntry`> (*Create > Mythril2D > Characters > NPCSheet*)
An `NPCSheet` serves as a reference to a specific character. In video games, the same NPC (one `NPCSheet`) may appear in different locations. This sheet identifies the character, making the specific GameObject location irrelevant when a quest requires interaction with that character.

## Recommended Interactions
A `QuestInteraction` is typically the first interaction you'll want to add to an NPC. All NPCs in the demo game include this interaction in their interaction list. You can add other interactions such as shop interactions, inn interactions, and more. Position the `QuestInteraction` first in the sequence to prioritize quest-related actions (obtaining or completing quests). For simple conversations, add a `DialogueInteraction` at the end of the interaction sequence. When the NPC has no other available actions (giving quests, completing quests, trading, etc.), it will default to this dialogue sequence.
