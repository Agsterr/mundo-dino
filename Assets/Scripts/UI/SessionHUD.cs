using OpenWorldDinoSurvival.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    /// <summary>
    /// Mostra jogadores conectados e limite da sessão (Milestone 5).
    /// </summary>
    public class SessionHUD : MonoBehaviour
    {
        private GUIStyle _titleStyle;
        private GUIStyle _labelStyle;

        private void OnGUI()
        {
            NetworkManager networkManager = NetworkManager.Singleton;
            if (networkManager == null || !networkManager.IsListening)
            {
                return;
            }

            EnsureStyles();

            float x = Screen.width - 220f;
            float y = 16f;
            float width = 200f;

            int playerCount = networkManager.ConnectedClientsIds.Count;
            string role = networkManager.IsHost ? "Host" : "Client";
            GUI.Box(new Rect(x, y, width, 24f + playerCount * 20f + 12f), GUIContent.none);
            GUI.Label(new Rect(x + 10f, y + 6f, width - 20f, 20f),
                $"Sessão — {role} ({playerCount}/{PlayerSpawnPoints.MaxGroupSize})",
                _titleStyle);

            float lineY = y + 28f;
            foreach (ulong clientId in networkManager.ConnectedClientsIds)
            {
                string marker = clientId == networkManager.LocalClientId ? " (você)" : string.Empty;
                GUI.Label(new Rect(x + 10f, lineY, width - 20f, 18f), $"Jogador {clientId}{marker}", _labelStyle);
                lineY += 20f;
            }
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 13,
                fontStyle = FontStyle.Bold
            };
            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12
            };
        }
    }
}
