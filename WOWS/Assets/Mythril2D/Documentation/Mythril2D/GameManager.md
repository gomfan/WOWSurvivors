# Game Manager
> On this page, we'll learn about the Mythril2D game manager.

## GameManager <`MonoBehaviour`>
The `GameManager` is a component that needs to be added to the Main Menu and M2DEngine scenes. It is essential for any game made with Mythril2D. The role of the `GameManager` is to keep track of all game systems (see Game Systems) and make them publicly available using a singleton pattern. You can think of the `GameManager` as the mediator for your game logic.

**ℹ️ In C#, you can call the `GameManager` from anywhere, as long as your game is running. For example: `GameManager.Player.Heal(10)`.**