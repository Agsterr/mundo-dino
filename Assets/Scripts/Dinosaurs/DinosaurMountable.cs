using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Systems;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Dinosaurs
{
    /// <summary>
    /// Permite que um jogador domine e controle este dinossauro.
    /// </summary>
    [RequireComponent(typeof(Health))]
    public class DinosaurMountable : NetworkBehaviour
    {
        [SerializeField] private float dominateHealthThreshold = 0.5f;
        [SerializeField] private float dominateRange = 5f;

        private readonly NetworkVariable<ulong> _possessorClientId = new(
            ulong.MaxValue,
            NetworkVariableReadPermission.Everyone,
            NetworkVariableWritePermission.Server);

        private Health _health;
        private VelociraptorAI _ai;
        private DinosaurPlayerControl _control;
        private Renderer _renderer;
        private Color _originalColor;
        private Vector2 _moveInput;
        private bool _sprintInput;
        private bool _attackRequested;

        public ulong PossessorClientId => _possessorClientId.Value;
        public bool IsPossessed => _possessorClientId.Value != ulong.MaxValue;
        public float DominateHealthThreshold => dominateHealthThreshold;
        public float DominateRange => dominateRange;
        public Vector2 MoveInput => _moveInput;
        public bool SprintInput => _sprintInput;
        public bool AttackRequested => _attackRequested;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _ai = GetComponent<VelociraptorAI>();
            _control = GetComponent<DinosaurPlayerControl>();
            _renderer = GetComponent<Renderer>();
            if (_renderer != null)
            {
                _originalColor = _renderer.material.color;
            }
        }

        public override void OnNetworkSpawn()
        {
            _possessorClientId.OnValueChanged += HandlePossessorChanged;
            ApplyPossessionVisual();
        }

        public override void OnNetworkDespawn()
        {
            _possessorClientId.OnValueChanged -= HandlePossessorChanged;
        }

        public bool CanBeDominatedBy(Transform playerTransform)
        {
            if (IsPossessed || _health == null || !_health.IsAlive || playerTransform == null)
            {
                return false;
            }

            float healthRatio = _health.CurrentHealth / _health.MaxHealth;
            if (healthRatio > dominateHealthThreshold)
            {
                return false;
            }

            return Vector3.Distance(transform.position, playerTransform.position) <= dominateRange;
        }

        public void TryDominate(ulong clientId, Transform playerTransform)
        {
            if (!IsServer)
            {
                return;
            }

            if (!CanBeDominatedBy(playerTransform))
            {
                return;
            }

            _possessorClientId.Value = clientId;
        }

        public void ReleasePossession()
        {
            if (!IsServer)
            {
                return;
            }

            ClearControlInput();
            _possessorClientId.Value = ulong.MaxValue;
        }

        public void ApplyControlInput(Vector2 move, bool sprint, bool attack)
        {
            if (!IsServer || !IsPossessed)
            {
                return;
            }

            _moveInput = move;
            _sprintInput = sprint;
            if (attack)
            {
                _attackRequested = true;
            }
        }

        public void ConsumeAttackRequest()
        {
            _attackRequested = false;
        }

        private void ClearControlInput()
        {
            _moveInput = Vector2.zero;
            _sprintInput = false;
            _attackRequested = false;
        }

        private void HandlePossessorChanged(ulong previous, ulong current)
        {
            if (!IsPossessed)
            {
                ClearControlInput();
            }

            if (_ai != null)
            {
                _ai.SetPossessed(IsPossessed);
            }

            if (_control != null)
            {
                _control.SetPossessed(IsPossessed, current);
            }

            ApplyPossessionVisual();
        }

        private void ApplyPossessionVisual()
        {
            if (_renderer == null)
            {
                return;
            }

            _renderer.material.color = IsPossessed
                ? new Color(0.3f, 0.85f, 1f)
                : _originalColor;
        }
    }
}
