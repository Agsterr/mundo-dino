using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class CraftingDatabase : MonoBehaviour
    {
        private static CraftingDatabase _instance;
        private CraftingRecipe[] _recipes;

        public static CraftingDatabase Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<CraftingDatabase>();
                    if (_instance == null)
                    {
                        GameObject host = new GameObject("CraftingDatabase");
                        _instance = host.AddComponent<CraftingDatabase>();
                    }
                }

                return _instance;
            }
        }

        public CraftingRecipe[] Recipes
        {
            get
            {
                if (_recipes == null || _recipes.Length == 0)
                {
                    _recipes = Resources.LoadAll<CraftingRecipe>("Crafting");
                }

                return _recipes;
            }
        }

        public CraftingRecipe FindById(string recipeId)
        {
            foreach (CraftingRecipe recipe in Recipes)
            {
                if (recipe != null && recipe.recipeId == recipeId)
                {
                    return recipe;
                }
            }

            return null;
        }
    }
}
