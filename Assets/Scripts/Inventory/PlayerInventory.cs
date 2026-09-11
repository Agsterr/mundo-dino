using System;
using System.Collections.Generic;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        private readonly Dictionary<int, int> _items = new();

        public event Action OnChanged;

        public int GetCount(int itemId)
        {
            return _items.TryGetValue(itemId, out int count) ? count : 0;
        }

        public void AddItem(int itemId, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            _items[itemId] = GetCount(itemId) + amount;
            OnChanged?.Invoke();
        }

        public bool RemoveItem(int itemId, int amount)
        {
            if (amount <= 0 || GetCount(itemId) < amount)
            {
                return false;
            }

            int remaining = GetCount(itemId) - amount;
            if (remaining <= 0)
            {
                _items.Remove(itemId);
            }
            else
            {
                _items[itemId] = remaining;
            }

            OnChanged?.Invoke();
            return true;
        }

        public bool HasIngredients(CraftingRecipe recipe)
        {
            if (recipe == null || recipe.ingredients == null)
            {
                return false;
            }

            foreach (CraftIngredient ingredient in recipe.ingredients)
            {
                if (GetCount(ingredient.itemId) < ingredient.amount)
                {
                    return false;
                }
            }

            return true;
        }

        public bool ConsumeIngredients(CraftingRecipe recipe)
        {
            if (!HasIngredients(recipe))
            {
                return false;
            }

            foreach (CraftIngredient ingredient in recipe.ingredients)
            {
                RemoveItem(ingredient.itemId, ingredient.amount);
            }

            return true;
        }
    }
}
