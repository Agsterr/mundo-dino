using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    public enum WorldRegionType
    {
        Unknown,
        Village,
        Forest,
        Mountain,
        River,
        Beach,
        Port,
        Laboratory
    }

    /// <summary>
    /// Mapa 2 km × 2 km com regiões nomeadas (Milestone 6).
    /// </summary>
    public static class WorldRegions
    {
        public const float MapHalfSize = 1000f;
        public const float MapSize = MapHalfSize * 2f;
        public const float RiverHalfWidth = 50f;
        public const float VillageHalfSize = 300f;

        public static WorldRegionType GetRegionAt(Vector3 worldPosition)
        {
            float x = worldPosition.x;
            float z = worldPosition.z;

            if (Mathf.Abs(z) <= RiverHalfWidth)
            {
                return WorldRegionType.River;
            }

            bool inVillageX = Mathf.Abs(x) <= VillageHalfSize;
            bool inVillageZ = Mathf.Abs(z) <= VillageHalfSize;
            if (inVillageX && inVillageZ)
            {
                return WorldRegionType.Village;
            }

            if (x < -VillageHalfSize)
            {
                return WorldRegionType.Forest;
            }

            if (z > VillageHalfSize)
            {
                return x > VillageHalfSize ? WorldRegionType.Laboratory : WorldRegionType.Mountain;
            }

            if (z < -VillageHalfSize)
            {
                return x > VillageHalfSize ? WorldRegionType.Port : WorldRegionType.Beach;
            }

            return WorldRegionType.Unknown;
        }

        public static string GetDisplayName(WorldRegionType region)
        {
            return region switch
            {
                WorldRegionType.Village => "Vilarejo",
                WorldRegionType.Forest => "Floresta",
                WorldRegionType.Mountain => "Montanha",
                WorldRegionType.River => "Rio",
                WorldRegionType.Beach => "Praia",
                WorldRegionType.Port => "Porto",
                WorldRegionType.Laboratory => "Laboratório",
                _ => "Desconhecido"
            };
        }

        public static Color GetRegionColor(WorldRegionType region)
        {
            return region switch
            {
                WorldRegionType.Village => new Color(0.55f, 0.48f, 0.35f),
                WorldRegionType.Forest => new Color(0.18f, 0.45f, 0.2f),
                WorldRegionType.Mountain => new Color(0.5f, 0.5f, 0.52f),
                WorldRegionType.River => new Color(0.2f, 0.45f, 0.75f),
                WorldRegionType.Beach => new Color(0.85f, 0.78f, 0.55f),
                WorldRegionType.Port => new Color(0.42f, 0.38f, 0.32f),
                WorldRegionType.Laboratory => new Color(0.65f, 0.7f, 0.8f),
                _ => new Color(0.35f, 0.35f, 0.35f)
            };
        }
    }
}
