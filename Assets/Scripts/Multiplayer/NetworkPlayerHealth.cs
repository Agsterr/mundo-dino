using System;
using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Player;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// Vida autoritativa no servidor para PvP (Milestone 4).
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerHealth : NetworkBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float respawnDelay = 3f;
        [SerializeField] private Vector3 respawnPosition = new Vector3(0f, 1f, 0f);
        [SerializeField] private Vector3 alternateRespawnPosition = new Vector3(6f, 1f, 0f);

        private readonly NetworkVariable<float> _networkHealth = new(
            100f,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private Vector3 _spawnPosition;
        private PlayerMovement _movement;
        private PlayerWeaponController _weaponController;
        private PlayerInputController _inputController;
        private CharacterController _controller;
        private NetworkPlayerInventory _inventory;

        public float CurrentHealth => _networkHealth.Value;
        public float MaxHealth => maxHealth + (_inventory != null ? _inventory.HealthBonus : 0f);
        public bool IsAlive => _networkHealth.Value > 0f;

        public event Action<float> OnDamaged;
        public event Action OnDied;
        public event Action OnRespawned;

        private void Awake()
        {
            _spawnPosition = transform.position;
            _movement = GetComponent<PlayerMovement>();
            _weaponController = GetComponent<PlayerWeaponController>();
            _inputController = GetComponent<PlayerInputController>();
            _controller = GetComponent<CharacterController>();
            _inventory = GetComponent<NetworkPlayerInventory>();
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                _networkHealth.Value = MaxHealth;
                _spawnPosition = OwnerClientId == 0 ? respawnPosition : alternateRespawnPosition;
            }

            _networkHealth.OnValueChanged += HandleHealthChanged;
        }

        public override void OnNetworkDespawn()
        {
            _networkHealth.OnValueChanged -= HandleHealthChanged;
        }

        public void TakeDamage(float amount)
        {
            if (!IsServer || !IsAlive || amount <= 0f)
            {
                return;
            }

            float reduction = _inventory != null ? _inventory.DamageReduction : 0f;
            float finalDamage = amount * (1f - reduction);
            _networkHealth.Value = Mathf.Max(0f, _networkHealth.Value - finalDamage);

            if (!IsAlive)
            {
                HandleDeathServer();
            }
        }

        private void HandleHealthChanged(float previous, float current)
        {
            if (current < previous)
            {
                OnDamaged?.Invoke(previous - current);
            }

            if (previous > 0f && current <= 0f)
            {
                OnDied?.Invoke();
            }
        }

        private void HandleDeathServer()
        {
            SetControlsEnabled(false);
            Invoke(nameof(RespawnServer), respawnDelay);
        }

        private void RespawnServer()
        {
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            transform.position = _spawnPosition;
            _networkHealth.Value = maxHealth;

            if (_controller != null)
            {
                _controller.enabled = true;
            }

            SetControlsEnabled(true);
            OnRespawned?.Invoke();
        }

        private void SetControlsEnabled(bool enabled)
        {
            if (!IsOwner)
            {
                return;
            }

            if (_movement != null)
            {
                _movement.enabled = enabled;
            }

            if (_weaponController != null)
            {
                _weaponController.enabled = enabled;
            }

            if (_inputController != null)
            {
                _inputController.enabled = enabled;
            }
        }
    }
}
