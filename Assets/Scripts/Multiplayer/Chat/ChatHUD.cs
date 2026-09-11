using System.Collections.Generic;
using OpenWorldDinoSurvival.Player;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer.Chat
{
    public class ChatHUD : MonoBehaviour
    {
        [SerializeField] private NetworkPlayerChat playerChat;
        [SerializeField] private int maxVisibleMessages = 8;

        private readonly List<ChatMessage> _messages = new();
        private string _inputText = string.Empty;
        private bool _chatFocused;
        private bool _showHistory = true;
        private ulong _privateTargetId = ChatChannels.Global;
        private GUIStyle _boxStyle;
        private GUIStyle _messageStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _tabStyle;

        private void Awake()
        {
            playerChat ??= GetComponent<NetworkPlayerChat>();
        }

        private void OnEnable()
        {
            if (playerChat != null)
            {
                playerChat.OnMessageReceived += HandleMessage;
            }
        }

        private void OnDisable()
        {
            if (playerChat != null)
            {
                playerChat.OnMessageReceived -= HandleMessage;
            }
        }

        public void ToggleChatFocus()
        {
            _chatFocused = !_chatFocused;
            UpdateCursor();
        }

        public void SetChatFocused(bool focused)
        {
            _chatFocused = focused;
            UpdateCursor();
        }

        public void SendCurrentInput()
        {
            if (string.IsNullOrWhiteSpace(_inputText) || playerChat == null)
            {
                return;
            }

            if (TryHandleCommand(_inputText))
            {
                _inputText = string.Empty;
                return;
            }

            playerChat.SendMessage(_inputText, _privateTargetId);
            _inputText = string.Empty;
        }

        private void HandleMessage(ChatMessage message)
        {
            _messages.Add(message);
            if (_messages.Count > 50)
            {
                _messages.RemoveAt(0);
            }
        }

        private bool TryHandleCommand(string text)
        {
            if (!text.StartsWith("/"))
            {
                return false;
            }

            string[] parts = text.Split(' ', 3);
            string command = parts[0].ToLowerInvariant();

            if (command == "/g" && parts.Length >= 2)
            {
                _privateTargetId = ChatChannels.Global;
                playerChat.SendMessage(string.Join(" ", parts, 1, parts.Length - 1), ChatChannels.Global);
                return true;
            }

            if ((command == "/w" || command == "/p") && parts.Length >= 3)
            {
                if (!ulong.TryParse(parts[1], out ulong targetId))
                {
                    AddLocalSystem("Uso: /w <id> <mensagem> — ex: /w 1 oi");
                    return true;
                }

                _privateTargetId = targetId;
                playerChat.SendMessage(parts[2], targetId);
                return true;
            }

            if (command == "/players")
            {
                if (NetworkManager.Singleton == null)
                {
                    AddLocalSystem("Sem conexão.");
                    return true;
                }

                foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
                {
                    AddLocalSystem($"ID {clientId}: {NetworkPlayerChat.GetPlayerDisplayName(clientId)}");
                }

                return true;
            }

            if (command == "/global")
            {
                _privateTargetId = ChatChannels.Global;
                AddLocalSystem("Modo: chat global");
                return true;
            }

            AddLocalSystem("Comandos: /g, /w <id> <msg>, /players, /global");
            return true;
        }

        private void AddLocalSystem(string text)
        {
            _messages.Add(ChatMessage.System(text));
        }

        private void OnGUI()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                return;
            }

            EnsureStyles();

            float width = 420f;
            float x = 16f;
            float y = Screen.height - 220f;

            if (_showHistory)
            {
                GUI.Box(new Rect(x, y, width, 180f), GUIContent.none, _boxStyle);
                DrawTabs(x + 8f, y + 8f, width - 16f);

                float messageY = y + 40f;
                int start = Mathf.Max(0, _messages.Count - maxVisibleMessages);
                for (int i = start; i < _messages.Count; i++)
                {
                    GUI.Label(new Rect(x + 10f, messageY, width - 20f, 18f), FormatMessage(_messages[i]), _messageStyle);
                    messageY += 18f;
                }
            }

            GUI.SetNextControlName("ChatInput");
            _inputText = GUI.TextField(new Rect(x, y + 185f, width - 70f, 28f), _inputText, _inputStyle);

            if (GUI.Button(new Rect(x + width - 64f, y + 185f, 60f, 28f), "Enviar"))
            {
                SendCurrentInput();
            }

            if (_chatFocused && Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("ChatInput");
            }
        }

        private void DrawTabs(float x, float y, float width)
        {
            bool globalSelected = _privateTargetId == ChatChannels.Global;
            if (GUI.Toggle(new Rect(x, y, 90f, 24f), globalSelected, "Global", _tabStyle))
            {
                _privateTargetId = ChatChannels.Global;
            }

            string privateLabel = _privateTargetId == ChatChannels.Global
                ? "Privado"
                : $"Privado → {_privateTargetId}";
            GUI.Label(new Rect(x + 100f, y, width - 100f, 24f), privateLabel, _messageStyle);
        }

        private static string FormatMessage(ChatMessage message)
        {
            string time = message.Timestamp.ToString("HH:mm");
            return message.Type switch
            {
                ChatMessageType.Global => $"[{time}] {message.SenderName}: {message.Text}",
                ChatMessageType.Private => $"[{time}] [PV {message.SenderName}→{message.TargetClientId}] {message.Text}",
                ChatMessageType.System => $"[{time}] * {message.Text}",
                _ => message.Text
            };
        }

        private void UpdateCursor()
        {
            if (_chatFocused)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void EnsureStyles()
        {
            if (_boxStyle != null)
            {
                return;
            }

            _boxStyle = new GUIStyle(GUI.skin.box);
            _messageStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            _inputStyle = new GUIStyle(GUI.skin.textField) { fontSize = 13 };
            _tabStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };
        }
    }
}
