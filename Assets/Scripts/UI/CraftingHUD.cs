using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    public class CraftingHUD : MonoBehaviour
    {
        [SerializeField] private PlayerInventory localInventory;
        [SerializeField] private PlayerCrafting localCrafting;
        [SerializeField] private NetworkPlayerInventory networkInventory;
        [SerializeField] private PlayerInteractor interactor;

        private bool _menuOpen;
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _hintStyle;

        private void Awake()
        {
            localInventory ??= GetComponent<PlayerInventory>();
            localCrafting ??= GetComponent<PlayerCrafting>();
            networkInventory ??= GetComponent<NetworkPlayerInventory>();
            interactor ??= GetComponent<PlayerInteractor>();
        }

        public void ToggleMenu()
        {
            _menuOpen = !_menuOpen;
        }

        public void SetMenuOpen(bool open)
        {
            _menuOpen = open;
        }

        private void OnGUI()
        {
            EnsureStyles();
            DrawInteractHint();

            if (!_menuOpen)
            {
                return;
            }

            float width = 420f;
            float height = 360f;
            float x = Screen.width * 0.5f - width * 0.5f;
            float y = Screen.height * 0.5f - height * 0.5f;
            GUI.Box(new Rect(x, y, width, height), GUIContent.none);
            GUI.Label(new Rect(x + 16, y + 12, width - 32, 28), "Crafting — Materiais e Equipamentos", _titleStyle);

            float contentY = y + 48f;
            DrawInventory(new Rect(x + 16, contentY, width - 32, 90f));
            contentY += 100f;
            DrawRecipes(new Rect(x + 16, contentY, width - 32, height - contentY + y - 16f));
        }

        private void DrawInteractHint()
        {
            if (interactor == null || !interactor.HasInteractable)
            {
                return;
            }

            string hint = interactor.GetInteractHint();
            if (string.IsNullOrEmpty(hint))
            {
                return;
            }

            GUI.Label(new Rect(16, Screen.height - 48, 500, 24), hint, _hintStyle);
        }

        private void DrawInventory(Rect area)
        {
            GUI.Label(area, "Materiais:", _labelStyle);
            float lineY = area.y + 24f;
            DrawMaterialLine(ItemIds.ScrapMetal, ref lineY, area.x);
            DrawMaterialLine(ItemIds.DinoHide, ref lineY, area.x);
            DrawMaterialLine(ItemIds.Fiber, ref lineY, area.x);
            DrawMaterialLine(ItemIds.GunParts, ref lineY, area.x);
            DrawMaterialLine(ItemIds.FoodRation, ref lineY, area.x);
            DrawMaterialLine(ItemIds.WaterFlask, ref lineY, area.x);
        }

        private void DrawMaterialLine(int itemId, ref float y, float x)
        {
            int count = GetCount(itemId);
            GUI.Label(new Rect(x, y, 360, 20), $"{ItemIds.GetDisplayName(itemId)}: {count}", _labelStyle);
            y += 20f;
        }

        private void DrawRecipes(Rect area)
        {
            GUI.Label(area, "Receitas (clique para craftar):", _labelStyle);
            CraftingRecipe[] recipes = CraftingDatabase.Instance.Recipes;
            float y = area.y + 24f;

            for (int i = 0; i < recipes.Length; i++)
            {
                CraftingRecipe recipe = recipes[i];
                if (recipe == null)
                {
                    continue;
                }

                string ingredients = BuildIngredientText(recipe);
                bool canCraft = CanCraft(recipe);
                GUI.enabled = canCraft;
                if (GUI.Button(new Rect(area.x, y, area.width, 42f), $"{recipe.displayName}\n{ingredients}", _buttonStyle))
                {
                    TryCraft(recipe);
                }

                GUI.enabled = true;
                y += 48f;
            }
        }

        private static string BuildIngredientText(CraftingRecipe recipe)
        {
            if (recipe.ingredients == null || recipe.ingredients.Length == 0)
            {
                return string.Empty;
            }

            System.Text.StringBuilder builder = new();
            for (int i = 0; i < recipe.ingredients.Length; i++)
            {
                CraftIngredient ingredient = recipe.ingredients[i];
                if (i > 0)
                {
                    builder.Append(" | ");
                }

                builder.Append($"{ItemIds.GetDisplayName(ingredient.itemId)} x{ingredient.amount}");
            }

            return builder.ToString();
        }

        private int GetCount(int itemId)
        {
            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return networkInventory.GetCount(itemId);
            }

            return localInventory != null ? localInventory.GetCount(itemId) : 0;
        }

        private bool CanCraft(CraftingRecipe recipe)
        {
            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return networkInventory.CanCraft(recipe);
            }

            return localCrafting != null && localCrafting.CanCraft(recipe);
        }

        private void TryCraft(CraftingRecipe recipe)
        {
            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (networkInventory.IsOwner)
                {
                    networkInventory.RequestCraftServerRpc(recipe.recipeId);
                }

                return;
            }

            localCrafting?.TryCraft(recipe);
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                normal = { textColor = Color.white }
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 13,
                alignment = TextAnchor.MiddleLeft
            };

            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(1f, 0.95f, 0.5f) }
            };
        }
    }
}
