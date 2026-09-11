using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public static class ItemIds
    {
        public const int ScrapMetal = 1;
        public const int DinoHide = 2;
        public const int Fiber = 3;
        public const int GunParts = 4;

        public static string GetDisplayName(int itemId)
        {
            return itemId switch
            {
                ScrapMetal => "Sucata de metal",
                DinoHide => "Couro de dino",
                Fiber => "Fibra",
                GunParts => "Peças de arma",
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
                _ => new Color(0.9f, 0.75f, 0.2f)
            };
        }
    }
}
