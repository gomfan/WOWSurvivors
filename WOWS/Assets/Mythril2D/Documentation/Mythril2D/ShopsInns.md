# Shops & Inns
> On this page, we'll learn about Mythril2D shops and inns.

## Shop <`DatabaseEntry`> (*Create > Mythril2D > Shops > Shop*)
A `Shop` is a `DatabaseEntry` that contains a list of items that are available for sale. It can also specify a selling and buying multiplier to discount or increase the price of items bought or sold. Once created, the shop instance can be passed to an NPC using the `ShopInteraction` interaction (see NPCs).

## Inn <`DatabaseEntry`> (*Create > Mythril2D > Inns > Inn*)
An `Inn` is a `DatabaseEntry` that defines how much health and mana should be restored and for what price. Once created, the inn instance can be passed to an NPC using the `InnInteraction` interaction (see NPCs).