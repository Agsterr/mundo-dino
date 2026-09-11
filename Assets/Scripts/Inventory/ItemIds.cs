using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public static class ItemIds
    {
        public const int ScrapMetal = 1;
        public const int DinoHide = 2;
        public const int Fiber = 3;
        public const int GunParts = 4;
        public const int FoodRation = 5;
        public const int WaterFlask = 6;

        public const float FoodRestoreAmount = 35f;
        public const float WaterRestoreAmount = 35f;

        public static bool IsConsumable(int itemId)
        {
            return itemId == FoodRation || itemId == WaterFlask;
        }

        public static string GetDisplayName(int itemId)
        {
            return itemId switch
            {
                ScrapMetal => "Sucata de metal",
                DinoHide => "Couro de dino",
                Fiber => "Fibra",
                GunParts => "Peças de arma",
                FoodRation => "Ração de comida",
                WaterFlask => "Cantil de água",
                _ => "Item desconhecido"
            };
        }

        public static Color GetLootColor(int itemId)
        {
            return itemId switch
            {
                ScrapMetal => new Color(0.7f, 0.7f, 0.75f),
                DinoHide => new Color(0.45f, 0.3f, 0.15f),
                Fiber => new Color(0.35f, 0.65f, 0.25f),
                GunParts => new Color(0.55f, 0.55f, 0.6f),
                FoodRation => new Color(0.75f, 0.45f, 0.2f),
                WaterFlask => new Color(0.35f, 0.65f, 0.95f),
                _ => new Color(0.9f, 0.75f, 0.2f)
            };
        }
    }
}
