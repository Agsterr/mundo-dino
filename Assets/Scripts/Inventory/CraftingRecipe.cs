using System;
using OpenWorldDinoSurvival.Weapons;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public enum CraftResultType
    {
        WeaponUpgrade,
        Armor
    }

    [Serializable]
    public struct CraftIngredient
    {
        public int itemId;
        public int amount;
    }

    [CreateAssetMenu(fileName = "NewRecipe", menuName = "Open World Dino Survival/Crafting Recipe")]
    public class CraftingRecipe : ScriptableObject
    {
        public string recipeId;
        public string displayName;
        public CraftIngredient[] ingredients;
        public CraftResultType resultType;
        public WeaponStats weaponStats;
        public int weaponSlot;
        public ArmorStats armorStats;
    }
}
