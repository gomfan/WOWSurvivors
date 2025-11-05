# Game Config
> On this page, we'll learn how to configure your game.

## GameConfig <`DatabaseEntry`> (*Create > Mythril2D > Game > GameConfig*)
The `GameConfig` is a `DatabaseEntry` that lets you fully configure your game, such as whether or not you can critically hit or miss a hit, and configure all the game terms, such as currency, experience, level, etc. This allows you to customize how attributes like "Health" are named in your game. Maybe you want to call it "HP" or "Life Points". It's up to you to decide!

Some important settings include:
💥 **Collision Contact Filter**: Determines which layers characters will collide with.
👁️ **Visibility Contact Filter**: Determines which layers the characters won't be able to see through. This is mostly used by the AI Controller to evaluate whether the player is in sight. You'll notice that this contact filter has similar values to the collision one, except that the layer "Collision A" is excluded, which includes water and holes (this lets AIs see you through water!).
📄 **Game Terms**: These are extremely important since they are used **everywhere** in Mythril2D! This dictionary maps a unique identifier (term ID) to its definition (icon, full name, short name, description). If you want to change the icon for "Health" in your game, well, game terms are there for you!

**You'll find many settings there, so feel free to explore and try things!**