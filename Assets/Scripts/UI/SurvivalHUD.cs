using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    /// <summary>
    /// HUD de fome, sede e consumíveis (Milestone 7).
    /// </summary>
    public class SurvivalHUD : MonoBehaviour
    {
        [SerializeField] private NetworkPlayerSurvival survival;
        [SerializeField] private NetworkPlayerInventory inventory;

        private GUIStyle _labelStyle;
        private GUIStyle _hintStyle;

        private void Awake()
        {
            survival ??= GetComponent<NetworkPlayerSurvival>();
            inventory ??= GetComponent<NetworkPlayerInventory>();
        }

        private void OnGUI()
        {
            if (survival == null)
            {
                return;
            }

            EnsureStyles();

            float x = 16f;
            float y = 76f;
            float width = 220f;

            GUI.Box(new Rect(x, y, width, 88f), GUIContent.none);
            GUI.Label(new Rect(x + 10f, y + 8f, width - 20f, 18f), "Sobrevivência", _labelStyle);
            DrawBar(new Rect(x + 10f, y + 30f, width - 20f, 16f), survival.Hunger, new Color(0.85f, 0.55f, 0.2f), "Fome");
            DrawBar(new Rect(x + 10f, y + 54f, width - 20f, 16f), survival.Thirst, new Color(0.25f, 0.55f, 0.95f), "Sede");

            int food = inventory != null ? inventory.GetCount(ItemIds.FoodRation) : 0;
            int water = inventory != null ? inventory.GetCount(ItemIds.WaterFlask) : 0;
            GUI.Label(
                new Rect(x, y + 96f, 360f, 20f),
                $"Comida: {food} [H] | Água: {water} [B] | Beber no rio: [E]",
                _hintStyle);
        }

        private void DrawBar(Rect rect, float value, Color fillColor, string label)
        {
            GUI.Label(new Rect(rect.x, rect.y - 2f, 60f, rect.height), label, _labelStyle);
            Rect barRect = new Rect(rect.x + 52f, rect.y, rect.width - 52f, rect.height);
            GUI.Box(barRect, GUIContent.none);

            float fillWidth = Mathf.Clamp01(value / 100f) * (barRect.width - 4f);
            Color previous = GUI.color;
            GUI.color = fillColor;
            GUI.Box(new Rect(barRect.x + 2f, barRect.y + 2f, fillWidth, barRect.height - 4f), GUIContent.none);
            GUI.color = previous;

            GUI.Label(new Rect(barRect.x + barRect.width - 36f, barRect.y, 34f, barRect.height), $"{value:0}", _labelStyle);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11
            };
            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
            };
        }
    }
}
