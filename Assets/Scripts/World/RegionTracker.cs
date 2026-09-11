using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    /// <summary>
    /// Detecta a região atual do jogador no mapa aberto.
    /// </summary>
    public class RegionTracker : MonoBehaviour
    {
        [SerializeField] private float updateInterval = 0.25f;

        private float _nextUpdateTime;
        private WorldRegionType _currentRegion = WorldRegionType.Village;

        public WorldRegionType CurrentRegion => _currentRegion;
        public string CurrentRegionName => WorldRegions.GetDisplayName(_currentRegion);

        private void Update()
        {
            if (Time.time < _nextUpdateTime)
            {
                return;
            }

            _nextUpdateTime = Time.time + updateInterval;
            _currentRegion = WorldRegions.GetRegionAt(transform.position);
        }
    }
}
