using System;
using OpenWorldDinoSurvival.Inventory;
using UnityEngine;

namespace OpenWorldDinoSurvival.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float respawnDelay = 3f;
        [SerializeField] private Vector3 respawnPosition = new Vector3(0f, 1f, 0f);

        private float _currentHealth;
        private Vector3 _spawnPosition;
        private PlayerMovement _movement;
        private PlayerWeaponController _weaponController;
        private PlayerInputController _inputController;
        private CharacterController _controller;
        private PlayerArmor _armor;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth + (_armor != null ? _armor.HealthBonus : 0f);
        public bool IsAlive => _currentHealth > 0f;

        public event Action<float> OnDamaged;
        public event Action OnDied;
        public event Action OnRespawned;

        private void Awake()
        {
            _currentHealth = maxHealth;
            _spawnPosition = transform.position;
            _movement = GetComponent<PlayerMovement>();
            _weaponController = GetComponent<PlayerWeaponController>();
            _inputController = GetComponent<PlayerInputController>();
            _controller = GetComponent<CharacterController>();
            _armor = GetComponent<PlayerArmor>();
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            float reduction = _armor != null ? _armor.DamageReduction : 0f;
            float finalDamage = amount * (1f - reduction);
            _currentHealth = Mathf.Max(0f, _currentHealth - finalDamage);
            OnDamaged?.Invoke(amount);

            if (!IsAlive)
            {
                Die();
            }
        }

        private void Die()
        {
            SetControlsEnabled(false);
            OnDied?.Invoke();
            Invoke(nameof(Respawn), respawnDelay);
        }

        private void Respawn()
        {
            if (_controller != null)
            {
                _controller.enabled = false;
            }

            transform.position = _spawnPosition;
            _currentHealth = maxHealth;

            if (_controller != null)
            {
                _controller.enabled = true;
            }

            SetControlsEnabled(true);
            OnRespawned?.Invoke();
        }

        private void SetControlsEnabled(bool enabled)
        {
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
