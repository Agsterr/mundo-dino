using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.Player
{
    /// <summary>
    /// Habilidades de luta corpo-a-corpo: soco, golpe pesado e esquiva.
    /// </summary>
    public class PlayerMeleeCombat : MonoBehaviour
    {
        [SerializeField] private float lightDamage = 18f;
        [SerializeField] private float heavyDamage = 40f;
        [SerializeField] private float attackRange = 2.2f;
        [SerializeField] private float attackRadius = 0.8f;
        [SerializeField] private float lightCooldown = 0.45f;
        [SerializeField] private float heavyCooldown = 1.4f;
        [SerializeField] private float dodgeCooldown = 1.1f;
        [SerializeField] private float dodgeDistance = 4f;
        [SerializeField] private float dodgeDuration = 0.2f;
        [SerializeField] private float invulnerableDuration = 0.35f;

        private float _nextLightTime;
        private float _nextHeavyTime;
        private float _nextDodgeTime;
        private float _dodgeEndTime;
        private float _invulnerableEndTime;
        private Vector3 _dodgeDirection;
        private CharacterController _controller;
        private PlayerMovement _movement;
        private NetworkPlayerAbilities _networkAbilities;

        public bool IsDodging => Time.time < _dodgeEndTime;
        public bool IsInvulnerable => Time.time < _invulnerableEndTime;
        public float LightCooldownRemaining => Mathf.Max(0f, _nextLightTime - Time.time);
        public float HeavyCooldownRemaining => Mathf.Max(0f, _nextHeavyTime - Time.time);
        public float DodgeCooldownRemaining => Mathf.Max(0f, _nextDodgeTime - Time.time);

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _movement = GetComponent<PlayerMovement>();
            _networkAbilities = GetComponent<NetworkPlayerAbilities>();
        }

        private void Update()
        {
            if (!IsDodging || _controller == null)
            {
                return;
            }

            float speed = dodgeDistance / dodgeDuration;
            _controller.Move(_dodgeDirection * speed * Time.deltaTime);
        }

        public void TryLightAttack()
        {
            if (Time.time < _nextLightTime || IsDodging)
            {
                return;
            }

            if (_networkAbilities != null)
            {
                _networkAbilities.RequestMeleeAttackServerRpc(false);
                _nextLightTime = Time.time + lightCooldown;
                return;
            }

            PerformMeleeAttack(lightDamage);
            _nextLightTime = Time.time + lightCooldown;
        }

        public void TryHeavyAttack()
        {
            if (Time.time < _nextHeavyTime || IsDodging)
            {
                return;
            }

            if (_networkAbilities != null)
            {
                _networkAbilities.RequestMeleeAttackServerRpc(true);
                _nextHeavyTime = Time.time + heavyCooldown;
                return;
            }

            PerformMeleeAttack(heavyDamage);
            _nextHeavyTime = Time.time + heavyCooldown;
        }

        public void TryDodge()
        {
            if (Time.time < _nextDodgeTime || IsDodging || _controller == null)
            {
                return;
            }

            _dodgeDirection = transform.forward;
            if (_dodgeDirection.sqrMagnitude < 0.01f)
            {
                _dodgeDirection = transform.right;
            }

            _dodgeDirection.y = 0f;
            _dodgeDirection.Normalize();

            _dodgeEndTime = Time.time + dodgeDuration;
            _invulnerableEndTime = Time.time + invulnerableDuration;
            _nextDodgeTime = Time.time + dodgeCooldown;

            if (_networkAbilities != null)
            {
                _networkAbilities.RequestDodgeServerRpc(_dodgeDirection);
            }
        }

        public void PerformMeleeAttack(float damage)
        {
            Vector3 origin = transform.position + Vector3.up * 1f;
            Vector3 direction = transform.forward;

            if (Physics.SphereCast(origin, attackRadius, direction, out RaycastHit hit, attackRange))
            {
                ApplyDamageToTarget(hit.collider, damage);
            }
        }

        public static void ApplyDamageToTarget(Collider targetCollider, float damage)
        {
            NetworkPlayerHealth networkHealth = targetCollider.GetComponentInParent<NetworkPlayerHealth>();
            if (networkHealth != null)
            {
                networkHealth.TakeDamage(damage);
                return;
            }

            Health health = targetCollider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        public void ApplyDodge(Vector3 direction)
        {
            _dodgeDirection = direction;
            _dodgeEndTime = Time.time + dodgeDuration;
            _invulnerableEndTime = Time.time + invulnerableDuration;
            _nextDodgeTime = Time.time + dodgeCooldown;
        }
    }
}
