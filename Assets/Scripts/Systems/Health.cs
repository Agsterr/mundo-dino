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

            if (!IsAlive)
            {
                Die();
            }
        }

        private void Die()
        {
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
