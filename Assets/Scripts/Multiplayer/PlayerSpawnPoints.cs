using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// Pontos de spawn compartilhados para sessões de até 4 jogadores (Milestone 5).
    /// </summary>
    public static class PlayerSpawnPoints
    {
        public const int MaxGroupSize = 4;

        private static readonly Vector3[] DefaultSpawns =
        {
            new Vector3(-40f, 1f, 120f),
            new Vector3(40f, 1f, 120f),
            new Vector3(-40f, 1f, 180f),
            new Vector3(40f, 1f, 180f),
        };

        public static Vector3 GetSpawn(ulong clientId, Vector3[] customSpawns = null)
        {
            Vector3[] spawns = HasValidCustomSpawns(customSpawns) ? customSpawns : DefaultSpawns;
            int index = Mathf.Clamp((int)clientId, 0, spawns.Length - 1);
            return spawns[index];
        }

        public static bool HasValidCustomSpawns(Vector3[] customSpawns)
        {
            return customSpawns != null && customSpawns.Length > 0;
        }
    }
}
