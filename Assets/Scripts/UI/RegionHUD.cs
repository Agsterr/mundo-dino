using OpenWorldDinoSurvival.World;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    /// <summary>
    /// Mostra a região atual do jogador no mapa aberto.
    /// </summary>
    public class RegionHUD : MonoBehaviour
    {
        [SerializeField] private RegionTracker regionTracker;

        private GUIStyle _labelStyle;

        private void Awake()
        {
            regionTracker ??= GetComponent<RegionTracker>();
        }

        private void OnGUI()
        {
            if (regionTracker == null)
            {
                return;
            }

            EnsureStyles();

            WorldRegionType region = regionTracker.CurrentRegion;
            string regionName = regionTracker.CurrentRegionName;
            Color regionColor = WorldRegions.GetRegionColor(region);

            float width = 220f;
            float x = 16f;
            float y = 16f;

            GUI.Box(new Rect(x, y, width, 52f), GUIContent.none);
            GUI.Label(new Rect(x + 10f, y + 8f, width - 20f, 18f), "Região atual", _labelStyle);

            GUIStyle regionStyle = new GUIStyle(_labelStyle)
            {
                fontStyle = FontStyle.Bold,
                normal = { textColor = regionColor }
            };
            GUI.Label(new Rect(x + 10f, y + 28f, width - 20f, 20f), regionName, regionStyle);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
        }
    }
}
