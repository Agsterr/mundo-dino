using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer.Voice
{
    /// <summary>
    /// Transmissão de voz PTT entre dois jogadores em chamada (prototype via Custom Messaging).
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class VoiceNetworkTransmitter : NetworkBehaviour
    {
        private const string VoiceMessageName = "VoicePacket";
        private const int SampleRate = 8000;
        private const int ClipLengthSeconds = 1;

        [SerializeField] private AudioSource playbackSource;

        private AudioClip _microphoneClip;
        private int _lastMicPosition;
        private ulong _callPartnerId = ulong.MaxValue;
        private bool _isTransmitting;
        private float[] _playbackBuffer = new float[SampleRate];
        private int _playbackWriteIndex;

        private void Awake()
        {
            playbackSource ??= gameObject.AddComponent<AudioSource>();
            playbackSource.loop = true;
            playbackSource.playOnAwake = false;
            playbackSource.spatialBlend = 0f;
            playbackSource.volume = 0.9f;
        }

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(VoiceMessageName, ReceiveVoicePacket);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.CustomMessagingManager.UnregisterNamedMessageHandler(VoiceMessageName);
            }

            StopMicrophone();
        }

        public void BeginCall(ulong partnerClientId)
        {
            _callPartnerId = partnerClientId;
            StartMicrophone();
        }

        public void EndCall()
        {
            StopTransmitting();
            _callPartnerId = ulong.MaxValue;
            StopMicrophone();
        }

        public void SetTransmitting(bool transmitting)
        {
            _isTransmitting = transmitting && _callPartnerId != ulong.MaxValue;
        }

        public void StopTransmitting()
        {
            _isTransmitting = false;
        }

        private void Update()
        {
            if (!IsOwner || !_isTransmitting || _callPartnerId == ulong.MaxValue || _microphoneClip == null)
            {
                return;
            }

            int currentPosition = Microphone.GetPosition(null);
            if (currentPosition < 0 || currentPosition == _lastMicPosition)
            {
                return;
            }

            int sampleCount = currentPosition - _lastMicPosition;
            if (sampleCount < 0)
            {
                sampleCount += _microphoneClip.samples;
            }

            if (sampleCount <= 0)
            {
                return;
            }

            sampleCount = Mathf.Min(sampleCount, 800);
            float[] samples = new float[sampleCount];
            _microphoneClip.GetData(samples, _lastMicPosition);

            byte[] payload = new byte[sampleCount];
            for (int i = 0; i < sampleCount; i++)
            {
                payload[i] = (byte)Mathf.Clamp(Mathf.RoundToInt((samples[i] + 1f) * 127.5f), 0, 255);
            }

            FastBufferWriter writer = new FastBufferWriter(payload.Length + 8, Unity.Collections.Allocator.Temp);
            writer.WriteValueSafe(OwnerClientId);
            writer.WriteValueSafe(payload.Length);
            writer.WriteBytesSafe(payload, payload.Length);

            NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
                VoiceMessageName,
                _callPartnerId,
                writer,
                NetworkDelivery.Unreliable);

            writer.Dispose();
            _lastMicPosition = currentPosition;
        }

        private void ReceiveVoicePacket(ulong senderClientId, FastBufferReader reader)
        {
            if (senderClientId != _callPartnerId)
            {
                return;
            }

            reader.ReadValueSafe(out ulong senderId);
            reader.ReadValueSafe(out int length);
            byte[] payload = new byte[length];
            reader.ReadBytesSafe(ref payload, length);

            for (int i = 0; i < length; i++)
            {
                _playbackBuffer[_playbackWriteIndex] = payload[i] / 127.5f - 1f;
                _playbackWriteIndex = (_playbackWriteIndex + 1) % _playbackBuffer.Length;
            }

            if (!playbackSource.isPlaying)
            {
                AudioClip clip = AudioClip.Create("VoicePlayback", SampleRate, 1, SampleRate, true, OnAudioRead);
                playbackSource.clip = clip;
                playbackSource.Play();
            }
        }

        private void OnAudioRead(float[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = _playbackBuffer[_playbackWriteIndex];
                _playbackWriteIndex = (_playbackWriteIndex + 1) % _playbackBuffer.Length;
            }
        }

        private void StartMicrophone()
        {
            if (Microphone.devices.Length == 0)
            {
                return;
            }

            _microphoneClip = Microphone.Start(null, true, ClipLengthSeconds, SampleRate);
            _lastMicPosition = 0;
        }

        private void StopMicrophone()
        {
            if (Microphone.IsRecording(null))
            {
                Microphone.End(null);
            }

            _microphoneClip = null;

            if (playbackSource != null && playbackSource.isPlaying)
            {
                playbackSource.Stop();
            }
        }
    }
}
