using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gyvr.Mythril2D
{
    [Serializable]
    public struct ChestLootEntry
    {
        public Item item;
        public int quantity;
    }

    [Serializable]
    public struct ChestLoot
    {
        public ChestLootEntry[] entries;
        public int money;

        public bool HasMoney() => money != 0;
        public bool HasItems() => entries != null && entries.Length > 0;
        public bool IsEmpty() => !(HasItems() || HasMoney());

        public Sprite[] GetLootSprites()
        {
            List<Sprite> sprites = new();

            if (HasItems())
            {
                for (int i = 0; i < entries.Length; ++i)
                {
                    sprites.Add(entries[i].item.icon);
                }
            }

            if (HasMoney())
            {
                sprites.Add(GameManager.Config.GetTermDefinition("currency").icon);
            }

            sprites.RemoveAll((sprite) => sprite == null);

            return sprites.ToArray();
        }
    }
}
