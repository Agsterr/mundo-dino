using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer.Chat
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerChat : NetworkBehaviour
    {
        private const int MaxMessageLength = 200;

        public static NetworkPlayerChat Local { get; private set; }

        public event Action<ChatMessage> OnMessageReceived;

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Local = this;
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && Local == this)
            {
                Local = null;
            }
        }

        public void SendMessage(string text, ulong targetClientId)
        {
            if (!IsOwner || string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            text = text.Trim();
            if (text.Length > MaxMessageLength)
            {
                text = text.Substring(0, MaxMessageLength);
            }

            FixedString128Bytes netText = new FixedString128Bytes(text);
            SendChatServerRpc(netText, targetClientId);
        }

        [ServerRpc]
        private void SendChatServerRpc(FixedString128Bytes text, ulong targetClientId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            FixedString32Bytes senderName = new FixedString32Bytes(GetPlayerDisplayName(OwnerClientId));

            if (targetClientId == ChatChannels.Global)
            {
                DeliverChatClientRpc(OwnerClientId, senderName, text, ChatChannels.Global);
                return;
            }

            if (!NetworkManager.Singleton.ConnectedClients.ContainsKey(targetClientId))
            {
                SendSystemToClient(OwnerClientId, "Jogador não encontrado.");
                return;
            }

            ClientRpcParams privateParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { OwnerClientId, targetClientId }
                }
            };

            DeliverChatClientRpc(OwnerClientId, senderName, text, targetClientId, privateParams);
        }

        [ClientRpc]
        private void DeliverChatClientRpc(ulong senderClientId, FixedString32Bytes senderName, FixedString128Bytes text, ulong targetClientId, ClientRpcParams rpcParams = default)
        {
            ChatMessageType type = targetClientId == ChatChannels.Global
                ? ChatMessageType.Global
                : ChatMessageType.Private;

            ChatMessage message = new ChatMessage
            {
                Type = type,
                SenderClientId = senderClientId,
                TargetClientId = targetClientId,
                SenderName = senderName.ToString(),
                Text = text.ToString(),
                Timestamp = DateTime.Now
            };

            OnMessageReceived?.Invoke(message);
        }

        [ClientRpc]
        private void DeliverSystemClientRpc(FixedString128Bytes text, ClientRpcParams rpcParams = default)
        {
            OnMessageReceived?.Invoke(ChatMessage.System(text.ToString()));
        }

        private void SendSystemToClient(ulong clientId, string text)
        {
            ClientRpcParams paramsForClient = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new[] { clientId }
                }
            };

            DeliverSystemClientRpc(new FixedString128Bytes(text), paramsForClient);
        }

        public static string GetPlayerDisplayName(ulong clientId)
        {
            return $"Jogador {clientId + 1}";
        }
    }
}
