# Commands
> On this page, we'll learn about the Mythril2D command system, including how to use and create new commands.

## ICommand <`Interface`>
`ICommand` is an interface that you can implement in C# to create your very own commands and call them from almost anywhere (quest completion, monster's death, on interaction...). Anything can be turned into a command: healing you player, adding items to the inventory, playing a dialogue or a sound, etc. Mythril2D comes with a plethora of basic commands that can be used in many games, but creating your own is very simple! Check out the add-ons section on the Discord server; I'm sure you'll find some cool commands there.

## CommandHandler <`DatabaseEntry`> (*Create > Mythril2D > Utils > CommandHandler*)
A `CommandHandler` allows you to create a preset for a specific command and its settings. Let's say you want to execute a command with some predetermined settings from multiple locations. Instead of setting up your command and its settings at each location where you need to execute it, you can create an instance of `CommandHandler`, set it up, and pass it as a reference to the `ExecuteCommandHandler` command where you want to execute it. It also works well with Unity's built-in event system, allowing you to specify a `CommandHandler` to execute by referencing the `Execute()` method of a particular `CommandHandler` instance!

## CommandTrigger <`MonoBehaviour`>
You can add the `CommandTrigger` component to any GameObject in your scene to execute a command when a specific event occurs (on start, on interaction, on trigger enter, etc.). You can also define multiple conditions that need to be met for the command to be executed. The `CommandTrigger` can come in very handy for:
- Playing background music on start (used on each map of the demo game).
- Playing a dialogue when a map is loaded (used by the demo game for the Abandoned House).
- Making an element of your game interactive by adding dialogues (such as the ladder in the Abandoned House of the demo game, although an entity with a `DialogueInteraction` would achieve the same result).
- And much more!

> The demo game uses a lot of `CommandTrigger` here and there to show simple dialogue lines (i.e., when interacting with a sign), to block the way, or to play a cinematic (like in the plains with the pickle quest).

**ℹ️ Creating your very own command is super easy! I'd highly recommend experimenting with that. You can try to copy-paste a simple command, such as `AddExperience`, and modify the content of its `Execute()` method.**