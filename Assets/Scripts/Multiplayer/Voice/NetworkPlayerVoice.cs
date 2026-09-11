using System;
using OpenWorldDinoSurvival.Multiplayer.Chat;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer.Voice
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerVoice : NetworkBehaviour
    {
        private readonly NetworkVariable<ulong> _callPartnerId = new(
            ulong.MaxValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private readonly NetworkVariable<int> _callState = new(
            (int)VoiceCallState.Idle,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private VoiceNetworkTransmitter _transmitter;

        public static NetworkPlayerVoice Local { get; private set; }

        public VoiceCallState CallState => (VoiceCallState)_callState.Value;
        public ulong CallPartnerId => _callPartnerId.Value;
        public bool IsInCall => CallState == VoiceCallState.Connected && CallPartnerId != ulong.MaxValue;

        public event Action<VoiceCallState, ulong> OnCallStateChanged;

        private void Awake()
        {
            _transmitter = GetComponent<VoiceNetworkTransmitter>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                Local = this;
            }

            _callState.OnValueChanged += HandleCallStateChanged;
            _callPartnerId.OnValueChanged += HandlePartnerChanged;
        }

        public override void OnNetworkDespawn()
        {
            _callState.OnValueChanged -= HandleCallStateChanged;
            _callPartnerId.OnValueChanged -= HandlePartnerChanged;

            if (IsOwner && Local == this)
            {
                Local = null;
            }
        }

        public void RequestCall(ulong targetClientId)
        {
            if (!IsOwner || CallState != VoiceCallState.Idle)
            {
                return;
            }

            RequestCallServerRpc(targetClientId);
        }

        public void AcceptCall()
        {
            if (!IsOwner || CallState != VoiceCallState.Incoming)
            {
                return;
            }

            AcceptCallServerRpc();
        }

        public void RejectCall()
        {
            if (!IsOwner)
            {
                return;
            }

            RejectCallServerRpc();
        }

        public void EndCall()
        {
            if (!IsOwner)
            {
                return;
            }

            EndCallServerRpc();
        }

        [ServerRpc]
        private void RequestCallServerRpc(ulong targetClientId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || CallState != VoiceCallState.Idle)
            {
                return;
            }

            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(targetClientId, out NetworkClient targetClient))
            {
                return;
            }

            NetworkPlayerVoice targetVoice = targetClient.PlayerObject.GetComponent<NetworkPlayerVoice>();
            if (targetVoice == null || targetVoice.CallState != VoiceCallState.Idle)
            {
                return;
            }

            _callState.Value = (int)VoiceCallState.Outgoing;
            _callPartnerId.Value = targetClientId;
            targetVoice.SetIncomingCall(OwnerClientId);
        }

        [ServerRpc]
        private void AcceptCallServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || CallState != VoiceCallState.Incoming)
            {
                return;
            }

            ulong callerId = _callPartnerId.Value;
            if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(callerId, out NetworkClient callerClient))
            {
                ResetCallServer();
                return;
            }

            NetworkPlayerVoice callerVoice = callerClient.PlayerObject.GetComponent<NetworkPlayerVoice>();
            if (callerVoice == null)
            {
                ResetCallServer();
                return;
            }

            SetConnectedCall(OwnerClientId);
            callerVoice.SetConnectedCall(OwnerClientId);
        }

        [ServerRpc]
        private void RejectCallServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            ulong partnerId = _callPartnerId.Value;
            ResetCallServer();

            if (partnerId != ulong.MaxValue
                && NetworkManager.Singleton.ConnectedClients.TryGetValue(partnerId, out NetworkClient partnerClient))
            {
                partnerClient.PlayerObject.GetComponent<NetworkPlayerVoice>()?.ResetCallServer();
            }
        }

        [ServerRpc]
        private void EndCallServerRpc(ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            ulong partnerId = _callPartnerId.Value;
            ResetCallServer();

            if (partnerId != ulong.MaxValue
                && NetworkManager.Singleton.ConnectedClients.TryGetValue(partnerId, out NetworkClient partnerClient))
            {
                partnerClient.PlayerObject.GetComponent<NetworkPlayerVoice>()?.ResetCallServer();
            }
        }

        public void SetIncomingCall(ulong callerClientId)
        {
            if (!IsServer)
            {
                return;
            }

            _callState.Value = (int)VoiceCallState.Incoming;
            _callPartnerId.Value = callerClientId;
        }

        public void SetConnectedCall(ulong partnerClientId)
        {
            if (!IsServer)
            {
                return;
            }

            _callState.Value = (int)VoiceCallState.Connected;
            _callPartnerId.Value = partnerClientId;
        }

        public void ResetCallServer()
        {
            if (!IsServer)
            {
                return;
            }

            _callState.Value = (int)VoiceCallState.Idle;
            _callPartnerId.Value = ulong.MaxValue;
            _transmitter?.StopTransmitting();
        }

        private void HandleCallStateChanged(int previous, int current)
        {
            OnCallStateChanged?.Invoke((VoiceCallState)current, _callPartnerId.Value);

            if (IsOwner && (VoiceCallState)current == VoiceCallState.Connected)
            {
                _transmitter?.BeginCall(CallPartnerId);
            }

            if (IsOwner && (VoiceCallState)previous == VoiceCallState.Connected && (VoiceCallState)current == VoiceCallState.Idle)
            {
                _transmitter?.EndCall();
            }
        }

        private void HandlePartnerChanged(ulong previous, ulong current)
        {
            OnCallStateChanged?.Invoke(CallState, current);
        }
    }
}
