using System;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Weapons;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class PlayerCrafting : MonoBehaviour
    {
        [SerializeField] private PlayerInventory inventory;
        [SerializeField] private PlayerArmor armor;
        [SerializeField] private PlayerWeaponController weaponController;

        private bool _pistolUpgraded;
        private bool _rifleUpgraded;
        private string _equippedArmorId;

        public event Action<CraftingRecipe> OnCrafted;

        public bool PistolUpgraded => _pistolUpgraded;
        public bool RifleUpgraded => _rifleUpgraded;
        public string EquippedArmorId => _equippedArmorId;

        private void Awake()
        {
            inventory ??= GetComponent<PlayerInventory>();
            armor ??= GetComponent<PlayerArmor>();
            weaponController ??= GetComponent<PlayerWeaponController>();
        }

        public bool CanCraft(CraftingRecipe recipe)
        {
            if (recipe == null || inventory == null)
            {
                return false;
            }

            if (recipe.resultType == CraftResultType.WeaponUpgrade)
            {
                if (recipe.weaponSlot == 0 && _pistolUpgraded)
                {
                    return false;
                }

                if (recipe.weaponSlot == 1 && _rifleUpgraded)
                {
                    return false;
                }
            }

            if (recipe.resultType == CraftResultType.Armor && recipe.armorStats != null
                && _equippedArmorId == recipe.recipeId)
            {
                return false;
            }

            return inventory.HasIngredients(recipe);
        }

        public bool TryCraft(CraftingRecipe recipe)
        {
            if (!CanCraft(recipe) || !inventory.ConsumeIngredients(recipe))
            {
                return false;
            }

            ApplyRecipeResult(recipe);
            OnCrafted?.Invoke(recipe);
            return true;
        }

        private void ApplyRecipeResult(CraftingRecipe recipe)
        {
            if (recipe.resultType == CraftResultType.WeaponUpgrade)
            {
                ApplyWeaponUpgrade(recipe);
                return;
            }

            if (recipe.resultType == CraftResultType.Armor)
            {
                ApplyArmor(recipe);
            }
        }

        private void ApplyWeaponUpgrade(CraftingRecipe recipe)
        {
            if (weaponController == null || recipe.weaponStats == null)
            {
                return;
            }

            weaponController.ApplyWeaponUpgrade(recipe.weaponSlot, recipe.weaponStats);

            if (recipe.weaponSlot == 0)
            {
                _pistolUpgraded = true;
            }
            else
            {
                _rifleUpgraded = true;
            }
        }

        private void ApplyArmor(CraftingRecipe recipe)
        {
            if (armor == null || recipe.armorStats == null)
            {
                return;
            }

            armor.Equip(recipe.armorStats);
            _equippedArmorId = recipe.recipeId;
        }
    }
}
