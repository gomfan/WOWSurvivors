# Game Flags
> On this page, we'll learn about Mythril2D game flags.

## What's a game flag?
A game flag is a unique string identifier that can be set or unset (boolean), stored and saved in a save file, and used for game condition checking. For instance, you may want a certain action in your game to alter the dialogues, monsters, or visuals. You can do this by setting a game flag when this action occurs and checking if this game flag is set using a `IsGameFlagSet` condition (see Conditions).