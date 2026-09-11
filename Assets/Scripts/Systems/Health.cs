using System;
using UnityEngine;

namespace OpenWorldDinoSurvival.Systems
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath = true;

        private float _currentHealth;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => _currentHealth > 0f;

        public event Action<float> OnDamaged;
        public event Action OnDied;

        private void Awake()
        {
            _currentHealth = maxHealth;
        }

        public void Initialize(float maxHp)
        {
            maxHealth = maxHp;
            _currentHealth = maxHp;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            _currentHealth = Mathf.Max(0f, _currentHealth - amount);
            OnDamaged?.Invoke(amount);

            if (!IsAlive)
            {
                Die();
            }
        }

        private void Die()
        {
            OnDied?.Invoke();

            if (destroyOnDeath)
            {
                Destroy(gameObject);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
