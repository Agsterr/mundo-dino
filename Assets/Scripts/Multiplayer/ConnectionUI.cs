using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// UI simples para Host / Client local (Milestone 4).
    /// </summary>
    public class ConnectionUI : MonoBehaviour
    {
        [SerializeField] private string serverAddress = "127.0.0.1";
        [SerializeField] private ushort serverPort = 7777;

        private GUIStyle _titleStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _statusStyle;
        private bool _showMenu = true;

        private void OnGUI()
        {
            if (NetworkManager.Singleton == null)
            {
                return;
            }

            EnsureStyles();

            if (NetworkManager.Singleton.IsListening)
            {
                string role = NetworkManager.Singleton.IsHost ? "Host" : "Client";
                GUI.Label(new Rect(16, 16, 500, 28), $"Multiplayer ativo — {role}", _statusStyle);
                return;
            }

            if (!_showMenu)
            {
                return;
            }

            float width = 280f;
            float x = Screen.width * 0.5f - width * 0.5f;
            float y = Screen.height * 0.35f;

            GUI.Label(new Rect(x, y, width, 36), "Mundo Dino — Milestone 4", _titleStyle);
            y += 44f;

            if (GUI.Button(new Rect(x, y, width, 40), "Host (Jogador 1)", _buttonStyle))
            {
                StartHost();
            }

            y += 48f;

            if (GUI.Button(new Rect(x, y, width, 40), "Client (Jogador 2)", _buttonStyle))
            {
                StartClient();
            }

            y += 52f;
            GUI.Label(new Rect(x, y, width, 48), "Host: inicia servidor + jogador 1\nClient: conecta em localhost", _statusStyle);
        }

        private void StartHost()
        {
            if (!NetworkManager.Singleton.StartHost())
            {
                Debug.LogError("Falha ao iniciar Host.");
                return;
            }

            _showMenu = false;
        }

        private void StartClient()
        {
            var transport = NetworkManager.Singleton.GetComponent<Unity.Netcode.Transports.UTP.UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData(serverAddress, serverPort);
            }

            if (!NetworkManager.Singleton.StartClient())
            {
                Debug.LogError("Falha ao conectar como Client.");
                return;
            }

            _showMenu = false;
        }

        private void EnsureStyles()
        {
            if (_titleStyle != null)
            {
                return;
            }

            _titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            _buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold
            };

            _statusStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 14,
                alignment = TextAnchor.UpperCenter,
                normal = { textColor = new Color(0.85f, 0.9f, 1f) }
            };
        }
    }
}
