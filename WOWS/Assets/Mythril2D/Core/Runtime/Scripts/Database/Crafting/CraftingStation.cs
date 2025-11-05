using UnityEngine;

namespace Gyvr.Mythril2D
{
    [CreateAssetMenu(menuName = AssetMenuIndexer.Mythril2D_Crafting + nameof(CraftingStation))]
    public class CraftingStation : DatabaseEntry
    {
        public Recipe[] recipes = null;
        public float priceMultiplier = 1.0f;
        public int flatPrice = 0;

        public int GetCraftingCost(Recipe recipe)
        {
            return recipe.CalculateCraftCost(flatPrice, priceMultiplier);
        }

        public bool CanCraft(Recipe recipe, out bool hasMoney, out bool hasIngredients)
        {
            return recipe.CanCraft(out hasMoney, out hasIngredients, flatPrice, priceMultiplier);
        }

        public void Craft(Recipe recipe)
        {
            int craftCost = recipe.CalculateCraftCost(flatPrice, priceMultiplier);

            GameManager.InventorySystem.RemoveMoney(craftCost);

            foreach (var requirement in recipe.ingredients)
            {
                GameManager.InventorySystem.RemoveFromBag(requirement.Key, requirement.Value, EItemTransferType.Crafting);
            }

            GameManager.InventorySystem.AddToBag(recipe.item, recipe.quantity, EItemTransferType.Crafting);

            foreach (var entry in recipe.additionalOutput)
            {
                Debug.Assert(entry.Value > 0, $"Invalid provided quantity ({entry.Value}). Expected quantity > 0");
                GameManager.InventorySystem.AddToBag(entry.Key, entry.Value, EItemTransferType.Crafting);
            }
        }
    }
}
