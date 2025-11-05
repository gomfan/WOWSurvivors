# Dialogues
> On this page, we'll learn about Mythril2D dialogue system, and how to create complex dialogue sequences.

## DialogueSequence <`DatabaseEntry`> (*Create > Mythril2D > Dialogues > DialogueSequence*)
This `DatabaseEntry` is the entry point for any dialogue.

## Creating a dialogue
The first step when creating a dialogue is to create a `DialogueSequence`.

> Note: when using the `PlayDialogueLine` command, you can "bypass" the use of a `DialogueSequence` and directly write your dialogue content inside the command's inspector. This comes in handy when playing a very simple dialogue line (like a sign, an decor element, etc.). Do note that it also makes it harder to find your dialogue line again, as you need to remember where it's located in your map's hierarchy. `DialogueSequence` are more robust, and often preferred.

## Dialogue lines
A `DialogueSequence` contains an array of lines (`DialogueNode`) that can be displayed in a UI dialogue message box. When writing your lines, you have to make sure not to overflow the output message box.

## Dialogue options
After all the lines in your `DialogueSequence` have been displayed, it's time to offer choices to the player. If no options are provided (i.e., option count = 0), the dialogue will terminate after the last line is displayed. If only one option is set, the provided `DialogueSequence` will automatically play following the current sequence. This is very convenient when working with branching conversations, as multiple sequences can converge to the same sequence. If more than one option is available to the player, you can name these options by filling in the first text field. Finally, the dropdown at the end of the option line allows you to add specific strings to the message feed, which will be explained later.

## Message Feed
Dialogue options can populate the message feed, changing the course of action in your game. Some scripts may play a dialogue sequence and read the message feed at the end of its execution. Each option can add one string to the message feed. For instance, a dialogue sequence used to initiate a quest is expected to contain "Accept" or "Decline", meaning that your sequence should probably include these options somewhere. The Demo game's quest dialogues offer great examples of using the message feed. You can also populate the message feed with custom strings and retrieve and parse these strings in a script. This can be useful for advanced users who want to expand Mythril2D by creating custom scripts. In most cases, "Accept" and "Decline" messages will be used for dialogue sequences used for shops, inns, or quests.

## Execute commands
It's also possible to execute commands (see Commands documentation) at the beginning and end of the execution of a `DialogueSequence`. This allows your dialogue to trigger gameplay logic, such as healing a player, setting a game flag, or giving the player an item.
