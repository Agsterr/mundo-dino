using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Systems;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Dinosaurs
{
    /// <summary>
    /// Controle do dinossauro quando dominado por um jogador.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class DinosaurPlayerControl : MonoBehaviour
    {
        [SerializeField] private DinosaurStats stats;

        private CharacterController _controller;
        private VelociraptorAI _ai;
        private Health _health;
        private DinosaurMountable _mountable;
        private ulong _controllerClientId = ulong.MaxValue;
        private Vector2 _localMoveInput;
        private bool _localSprintInput;
        private bool _localAttackRequested;
        private float _nextAttackTime;

        public bool IsPossessed => _controllerClientId != ulong.MaxValue;
        public ulong ControllerClientId => _controllerClientId;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _ai = GetComponent<VelociraptorAI>();
            _health = GetComponent<Health>();
            _mountable = GetComponent<DinosaurMountable>();

            if (stats == null)
            {
                stats = Resources.Load<DinosaurStats>("Dinosaurs/VelociraptorStats");
            }
        }

        public void SetPossessed(bool possessed, ulong clientId)
        {
            _controllerClientId = possessed ? clientId : ulong.MaxValue;
            _localMoveInput = Vector2.zero;
            _localAttackRequested = false;
        }

        public void SetMoveInput(Vector2 input)
        {
            _localMoveInput = input;
        }

        public void SetSprintInput(bool sprint)
        {
            _localSprintInput = sprint;
        }

        public void RequestAttack()
        {
            _localAttackRequested = true;
        }

        private void Update()
        {
            if (!IsPossessed || stats == null)
            {
                return;
            }

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (!NetworkManager.Singleton.IsServer)
                {
                    return;
                }
            }
            else if (!IsLocalController())
            {
                return;
            }

            MoveDinosaur();
            TryAttack();
        }

        private bool IsLocalController()
        {
            if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsListening)
            {
                return true;
            }

            return NetworkManager.Singleton.LocalClientId == _controllerClientId;
        }

        private Vector2 GetMoveInput()
        {
            if (_mountable != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return _mountable.MoveInput;
            }

            return _localMoveInput;
        }

        private bool GetSprintInput()
        {
            if (_mountable != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return _mountable.SprintInput;
            }

            return _localSprintInput;
        }

        private bool HasAttackRequest()
        {
            if (_mountable != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return _mountable.AttackRequested;
            }

            return _localAttackRequested;
        }

        private void ClearAttackRequest()
        {
            if (_mountable != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                _mountable.ConsumeAttackRequest();
                return;
            }

            _localAttackRequested = false;
        }

        private void MoveDinosaur()
        {
            Vector2 moveInput = GetMoveInput();
            Vector3 input = new Vector3(moveInput.x, 0f, moveInput.y);
            if (input.sqrMagnitude < 0.01f)
            {
                return;
            }

            input.Normalize();
            float speed = GetSprintInput() ? stats.runSpeed : stats.walkSpeed;
            Vector3 move = transform.TransformDirection(input) * speed;
            move.y = -2f;
            _controller.Move(move * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(new Vector3(move.x, 0f, move.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, stats.rotationSpeed * Time.deltaTime);
        }

        private void TryAttack()
        {
            if (!HasAttackRequest() || Time.time < _nextAttackTime)
            {
                return;
            }

            ClearAttackRequest();
            _nextAttackTime = Time.time + stats.attackCooldown;

            Vector3 origin = transform.position + Vector3.up;
            Collider[] hits = Physics.OverlapSphere(origin + transform.forward * stats.attackRange, 1f);
            foreach (Collider hit in hits)
            {
                if (hit.transform == transform)
                {
                    continue;
                }

                NetworkPlayerHealth networkHealth = hit.GetComponentInParent<NetworkPlayerHealth>();
                if (networkHealth != null && networkHealth.IsAlive)
                {
                    networkHealth.TakeDamage(stats.attackDamage);
                    continue;
                }

                PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();
                if (playerHealth != null && playerHealth.IsAlive)
                {
                    playerHealth.TakeDamage(stats.attackDamage);
                }
            }
        }
    }
}
