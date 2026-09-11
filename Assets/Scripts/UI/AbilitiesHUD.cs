using OpenWorldDinoSurvival.Player;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    /// <summary>
    /// HUD de asas, combate corpo-a-corpo e dominação de dinossauros.
    /// </summary>
    public class AbilitiesHUD : MonoBehaviour
    {
        [SerializeField] private PlayerWings wings;
        [SerializeField] private PlayerMeleeCombat meleeCombat;
        [SerializeField] private PlayerDinosaurDomination domination;

        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;
        private GUIStyle _hintStyle;

        private void Awake()
        {
            wings ??= GetComponent<PlayerWings>();
            meleeCombat ??= GetComponent<PlayerMeleeCombat>();
            domination ??= GetComponent<PlayerDinosaurDomination>();
        }

        private void OnGUI()
        {
            EnsureStyles();

            float x = 16f;
            float y = Screen.height - 180f;
            float width = 280f;

            GUI.Box(new Rect(x, y, width, 160f), GUIContent.none);
            GUI.Label(new Rect(x + 12, y + 8, width - 24, 24), "Habilidades", _titleStyle);

            float lineY = y + 36f;
            DrawWingsStatus(ref lineY, x + 12, width - 24);
            DrawMeleeStatus(ref lineY, x + 12, width - 24);
            DrawDominationStatus(ref lineY, x + 12, width - 24);

            GUI.Label(
                new Rect(x, Screen.height - 28f, 520f, 22f),
                "F asas | Q soco | Shift+Q pesado | X esquiva | G dominar dino",
                _hintStyle);
        }

        private void DrawWingsStatus(ref float lineY, float x, float width)
        {
            if (wings == null)
            {
                return;
            }

            string status = wings.WingsDeployed ? "abertas" : "fechadas";
            float fuelPercent = wings.MaxFuel > 0f ? wings.CurrentFuel / wings.MaxFuel : 0f;
            GUI.Label(new Rect(x, lineY, width, 20f), $"Asas: {status} — combustível {fuelPercent * 100f:0}%", _labelStyle);
            lineY += 22f;
        }

        private void DrawMeleeStatus(ref float lineY, float x, float width)
        {
            if (meleeCombat == null)
            {
                return;
            }

            string dodge = FormatCooldown(meleeCombat.DodgeCooldownRemaining);
            string light = FormatCooldown(meleeCombat.LightCooldownRemaining);
            string heavy = FormatCooldown(meleeCombat.HeavyCooldownRemaining);
            GUI.Label(new Rect(x, lineY, width, 20f), $"Luta: soco {light} | pesado {heavy} | esquiva {dodge}", _labelStyle);
            lineY += 22f;
        }

        private void DrawDominationStatus(ref float lineY, float x, float width)
        {
            if (domination == null)
            {
                return;
            }

            if (domination.IsDominating)
            {
                GUI.Label(new Rect(x, lineY, width, 20f), "Dominação: controlando dinossauro (G soltar)", _labelStyle);
            }
            else
            {
                string cooldown = FormatCooldown(domination.DominateCooldownRemaining);
                GUI.Label(new Rect(x, lineY, width, 20f), $"Dominação: pronta {cooldown} (dino <50% HP, 5m)", _labelStyle);
            }

            lineY += 22f;
        }

        private static string FormatCooldown(float remaining)
        {
            return remaining > 0f ? $"{remaining:0.1}s}" : "ok";
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
            _hintStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 11,
                normal = { textColor = new Color(0.85f, 0.85f, 0.85f) }
            };
        }
    }
}
