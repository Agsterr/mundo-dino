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
    }
}
