# Monsters
> On this page, we'll learn about monsters.

## Monster <`Character`>
A `Monster` is a component derived from `Character`, so it inherits all the movement and combat logic while adding a scalable stats system. This allows its attributes to automatically scale with level, based on a given evolution profile in its associated `MonsterSheet`.

## MonsterSheet (*Create > Mythril2D > Characters > MonsterSheet*)
A `MonsterSheet` lets you define how a monster's stats evolve with level, its available abilities, and rewards when defeated (money, experience, items...).

## MonsterSpawner <`AMonsterSpawner`>
The `MonsterSpawner` allows you to spawn monsters at a specific location based on predetermined criteria.

## MonsterAreaSpawner <`AMonsterSpawner`>
The `MonsterAreaSpawner` allows you to spawn monsters anywhere within a collider's area. This is useful when you want to create dynamic and unpredictable monster spawns in your game.