using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// UI para Host / Join em sessões de até 4 jogadores (Milestone 5).
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
                return;
            }

            if (!_showMenu)
            {
                return;
            }

            float width = 320f;
            float x = Screen.width * 0.5f - width * 0.5f;
            float y = Screen.height * 0.32f;

            GUI.Label(new Rect(x, y, width, 36), "Mundo Dino — até 4 jogadores", _titleStyle);
            y += 44f;

            if (GUI.Button(new Rect(x, y, width, 40), "Host (criar sessão)", _buttonStyle))
            {
                StartHost();
            }

            y += 48f;

            if (GUI.Button(new Rect(x, y, width, 40), "Entrar (conectar)", _buttonStyle))
            {
                StartClient();
            }

            y += 52f;
            GUI.Label(
                new Rect(x, y, width, 72f),
                $"Host: inicia servidor + jogador local\nEntrar: conecta em {serverAddress}:{serverPort}\nMáximo de {PlayerSpawnPoints.MaxGroupSize} jogadores por sessão",
                _statusStyle);
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
