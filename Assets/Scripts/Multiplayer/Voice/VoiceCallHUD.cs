using OpenWorldDinoSurvival.Multiplayer.Chat;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer.Voice
{
    public class VoiceCallHUD : MonoBehaviour
    {
        [SerializeField] private NetworkPlayerVoice playerVoice;
        [SerializeField] private VoiceNetworkTransmitter transmitter;

        private ulong _callTargetId = 1;
        private bool _pushToTalk;
        private GUIStyle _labelStyle;
        private GUIStyle _buttonStyle;

        private void Awake()
        {
            playerVoice ??= GetComponent<NetworkPlayerVoice>();
            transmitter ??= GetComponent<VoiceNetworkTransmitter>();
        }

        private void OnEnable()
        {
            if (playerVoice != null)
            {
                playerVoice.OnCallStateChanged += HandleCallStateChanged;
            }
        }

        private void OnDisable()
        {
            if (playerVoice != null)
            {
                playerVoice.OnCallStateChanged -= HandleCallStateChanged;
            }
        }

        public void SetPushToTalk(bool active)
        {
            _pushToTalk = active;
            transmitter?.SetTransmitting(active && playerVoice != null && playerVoice.IsInCall);
        }

        public void ToggleCallToDefaultTarget()
        {
            if (playerVoice == null)
            {
                return;
            }

            switch (playerVoice.CallState)
            {
                case VoiceCallState.Idle:
                    playerVoice.RequestCall(_callTargetId);
                    break;
                case VoiceCallState.Incoming:
                    playerVoice.AcceptCall();
                    break;
                case VoiceCallState.Connected:
                case VoiceCallState.Outgoing:
                    playerVoice.EndCall();
                    break;
            }
        }

        public void RejectIncomingCall()
        {
            playerVoice?.RejectCall();
        }

        private void HandleCallStateChanged(VoiceCallState state, ulong partnerId)
        {
            if (state == VoiceCallState.Idle)
            {
                SetPushToTalk(false);
            }
        }

        private void OnGUI()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening || playerVoice == null)
            {
                return;
            }

            EnsureStyles();

            float x = Screen.width - 260f;
            float y = Screen.height - 150f;
            GUI.Box(new Rect(x, y, 244f, 120f), "Chamada de voz");

            string status = playerVoice.CallState switch
            {
                VoiceCallState.Idle => "Sem chamada ativa",
                VoiceCallState.Outgoing => $"Chamando {NetworkPlayerChat.GetPlayerDisplayName(playerVoice.CallPartnerId)}...",
                VoiceCallState.Incoming => $"{NetworkPlayerChat.GetPlayerDisplayName(playerVoice.CallPartnerId)} está chamando!",
                VoiceCallState.Connected => $"Em chamada com {NetworkPlayerChat.GetPlayerDisplayName(playerVoice.CallPartnerId)}",
                _ => string.Empty
            };

            GUI.Label(new Rect(x + 10f, y + 24f, 224f, 20f), status, _labelStyle);
            GUI.Label(new Rect(x + 10f, y + 44f, 224f, 20f), "Alvo ID:", _labelStyle);
            string targetText = GUI.TextField(new Rect(x + 70f, y + 42f, 40f, 22f), _callTargetId.ToString());
            ulong.TryParse(targetText, out _callTargetId);

            if (playerVoice.CallState == VoiceCallState.Incoming)
            {
                if (GUI.Button(new Rect(x + 10f, y + 72f, 110f, 28f), "Aceitar (C)", _buttonStyle))
                {
                    playerVoice.AcceptCall();
                }

                if (GUI.Button(new Rect(x + 124f, y + 72f, 110f, 28f), "Recusar", _buttonStyle))
                {
                    playerVoice.RejectCall();
                }
            }
            else
            {
                string callLabel = playerVoice.CallState == VoiceCallState.Connected ? "Encerrar (C)" : "Ligar (C)";
                if (GUI.Button(new Rect(x + 10f, y + 72f, 110f, 28f), callLabel, _buttonStyle))
                {
                    ToggleCallToDefaultTarget();
                }
            }

            if (playerVoice.IsInCall)
            {
                string ptt = _pushToTalk ? "Falando (V)" : "Segure V para falar";
                GUI.Label(new Rect(x + 10f, y + 96f, 224f, 20f), ptt, _labelStyle);
            }
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                normal = { textColor = Color.white }
            };
            _buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 12 };
        }
    }
}
