# Navigation Cursors
> On this page, we'll learn about Mythril2D navigation cursors.

## What is a navigation cursor?
A navigation cursor is a UI element that follows the selected UI element to provide information to the player about which element is currently active. For instance, you may want a frame to show around an item slot in your inventory whenever this slot is selected.

## UINavigationCursor <`MonoBehaviour`>
This component, when added to a GameObject in your screen space UI, displays an image on top of selected UI elements.

## UINavigationCursorTarget <`UINavigationTarget` (from `MonoBehaviour`)>
Add this component to any UI element you want the navigation cursor to react to. If a UI element has this component, the `UINavigationCursor` will automatically move to it once selected.

## NavigationCursorStyle <`DatabaseEntry`> (*Create > Mythril2D > UI > NavigationCursorStyle*)
This `DatabaseEntry` defines the style the navigation cursor should adopt when highlighting an element. You can create as many styles as you want for each different UI element in your game.
