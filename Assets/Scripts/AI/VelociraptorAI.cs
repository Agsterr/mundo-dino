using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Systems;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.AI
{
    public enum RaptorState
    {
        Idle,
        Patrol,
        Detect,
        Chase,
        Attack,
        Search,
        Return
    }

    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Health))]
    public class VelociraptorAI : MonoBehaviour
    {
        [SerializeField] private DinosaurStats stats;
        [SerializeField] private LayerMask obstacleMask = ~0;

        private CharacterController _controller;
        private Health _health;
        private Transform _player;
        private Vector3 _homePosition;
        private Vector3 _patrolTarget;
        private Vector3 _lastKnownPlayerPosition;

        private RaptorState _state = RaptorState.Idle;
        private float _stateTimer;
        private float _nextAttackTime;
        private float _currentSpeed;
        private bool _possessed;

        public RaptorState CurrentState => _state;
        public bool IsPossessed => _possessed;

        public void Initialize(DinosaurStats dinosaurStats)
        {
            stats = dinosaurStats;
            _homePosition = transform.position;

            if (_health == null)
            {
                _health = GetComponent<Health>();
            }

            if (stats != null && _health != null)
            {
                _health.Initialize(stats.maxHealth);
            }
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _health = GetComponent<Health>();
            _homePosition = transform.position;

            if (stats == null)
            {
                stats = Resources.Load<DinosaurStats>("Dinosaurs/VelociraptorStats");
            }

            if (stats != null)
            {
                Initialize(stats);
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
        }

        private void OnEnable()
        {
            _health.OnDamaged += HandleDamaged;
            _health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            _health.OnDamaged -= HandleDamaged;
            _health.OnDied -= HandleDied;
        }

        private void Update()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            if (_possessed || ! _health.IsAlive || stats == null)
            {
                return;
            }

            RefreshPlayerReference();
            UpdateState();
        }

        public void SetPossessed(bool possessed)
        {
            _possessed = possessed;
        }

        public void AlertToPosition(Vector3 worldPosition)
        {
            if (! _health.IsAlive)
            {
                return;
            }

            _lastKnownPlayerPosition = worldPosition;
            SetState(RaptorState.Search);
        }

        public void AlertToGunfire(Vector3 shotOrigin)
        {
            if (! _health.IsAlive || stats == null)
            {
                return;
            }

            if (Vector3.Distance(transform.position, shotOrigin) <= stats.hearingRange)
            {
                AlertToPosition(shotOrigin);
            }
        }

        private void HandleDamaged(float amount)
        {
            if (_player != null)
            {
                _lastKnownPlayerPosition = _player.position;
            }

            SetState(RaptorState.Chase);
        }

        private void HandleDied()
        {
            enabled = false;
        }

        private void RefreshPlayerReference()
        {
            if (_player != null)
            {
                NetworkPlayerHealth networkHealth = _player.GetComponent<NetworkPlayerHealth>();
                if (networkHealth != null && !networkHealth.IsAlive)
                {
                    _player = null;
                }
                else
                {
                    PlayerHealth localHealth = _player.GetComponent<PlayerHealth>();
                    if (localHealth != null && !localHealth.IsAlive)
                    {
                        _player = null;
                    }
                }
            }

            if (_player != null)
            {
                return;
            }

            NetworkPlayerHealth[] networkPlayers = FindObjectsByType<NetworkPlayerHealth>(FindObjectsSortMode.None);
            Transform closest = null;
            float closestDistance = float.MaxValue;

            foreach (NetworkPlayerHealth networkPlayer in networkPlayers)
            {
                if (!networkPlayer.IsAlive)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, networkPlayer.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = networkPlayer.transform;
                }
            }

            if (closest != null)
            {
                _player = closest;
                return;
            }

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject.transform;
            }
        }

        private void UpdateState()
        {
            switch (_state)
            {
                case RaptorState.Idle:
                    UpdateIdle();
                    break;
                case RaptorState.Patrol:
                    UpdatePatrol();
                    break;
                case RaptorState.Detect:
                    UpdateDetect();
                    break;
                case RaptorState.Chase:
                    UpdateChase();
                    break;
                case RaptorState.Attack:
                    UpdateAttack();
                    break;
                case RaptorState.Search:
                    UpdateSearch();
                    break;
                case RaptorState.Return:
                    UpdateReturn();
                    break;
            }

            if (_state != RaptorState.Attack && CanSeePlayer())
            {
                _lastKnownPlayerPosition = _player.position;
                SetState(RaptorState.Chase);
            }
        }

        private void UpdateIdle()
        {
            _stateTimer -= Time.deltaTime;
            if (_stateTimer <= 0f)
            {
                PickPatrolTarget();
                SetState(RaptorState.Patrol);
            }
        }

        private void UpdatePatrol()
        {
            _currentSpeed = stats.walkSpeed;
            if (MoveTowards(_patrolTarget))
            {
                SetState(RaptorState.Idle, stats.patrolWaitTime);
            }
        }

        private void UpdateDetect()
        {
            SetState(RaptorState.Patrol);
        }

        private void UpdateChase()
        {
            if (_player == null)
            {
                SetState(RaptorState.Return);
                return;
            }

            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance > stats.loseTargetDistance)
            {
                SetState(RaptorState.Search);
                return;
            }

            if (distance <= stats.attackRange)
            {
                SetState(RaptorState.Attack);
                return;
            }

            _currentSpeed = stats.runSpeed;
            MoveTowards(_player.position);
        }

        private void UpdateAttack()
        {
            if (_player == null)
            {
                SetState(RaptorState.Search);
                return;
            }

            FaceTarget(_player.position);

            float distance = Vector3.Distance(transform.position, _player.position);
            if (distance > stats.attackRange * 1.5f)
            {
                SetState(RaptorState.Chase);
                return;
            }

            if (Time.time >= _nextAttackTime)
            {
                TryAttackPlayer();
                _nextAttackTime = Time.time + stats.attackCooldown;
            }
        }

        private void UpdateSearch()
        {
            _stateTimer -= Time.deltaTime;
            _currentSpeed = stats.runSpeed;

            if (MoveTowards(_lastKnownPlayerPosition))
            {
                if (_stateTimer <= 0f)
                {
                    SetState(RaptorState.Return);
                }
            }
        }

        private void UpdateReturn()
        {
            _currentSpeed = stats.walkSpeed;
            if (MoveTowards(_homePosition))
            {
                SetState(RaptorState.Idle, stats.idleTime);
            }
        }

        private void TryAttackPlayer()
        {
            if (_player == null)
            {
                return;
            }

            NetworkPlayerHealth networkHealth = _player.GetComponent<NetworkPlayerHealth>();
            if (networkHealth != null && networkHealth.IsAlive)
            {
                networkHealth.TakeDamage(stats.attackDamage);
                return;
            }

            PlayerHealth playerHealth = _player.GetComponent<PlayerHealth>();
            if (playerHealth != null && playerHealth.IsAlive)
            {
                playerHealth.TakeDamage(stats.attackDamage);
            }
        }

        private bool MoveTowards(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude < 0.25f)
            {
                return true;
            }

            direction.Normalize();
            FaceTarget(target);

            Vector3 motion = direction * (_currentSpeed * Time.deltaTime);
            motion.y = -2f * Time.deltaTime;
            _controller.Move(motion);
            return false;
        }

        private void FaceTarget(Vector3 target)
        {
            Vector3 direction = target - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 0.01f)
            {
                return;
            }

            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, stats.rotationSpeed * Time.deltaTime);
        }

        private bool CanSeePlayer()
        {
            if (_player == null)
            {
                return false;
            }

            Vector3 origin = transform.position + Vector3.up * 1.2f;
            Vector3 targetPoint = _player.position + Vector3.up * 1f;
            Vector3 direction = targetPoint - origin;
            float distance = direction.magnitude;

            if (distance > stats.sightRange)
            {
                return false;
            }

            direction.Normalize();
            float angle = Vector3.Angle(transform.forward, direction);
            if (angle > stats.sightAngle * 0.5f)
            {
                return false;
            }

            if (Physics.Raycast(origin, direction, out RaycastHit hit, distance, obstacleMask, QueryTriggerInteraction.Ignore))
            {
                return hit.transform.CompareTag("Player");
            }

            return true;
        }

        private void PickPatrolTarget()
        {
            Vector2 offset = Random.insideUnitCircle * stats.patrolRadius;
            _patrolTarget = _homePosition + new Vector3(offset.x, 0f, offset.y);
        }

        private void SetState(RaptorState newState, float timer = 0f)
        {
            _state = newState;
            _stateTimer = timer;

            if (newState == RaptorState.Search && _stateTimer <= 0f)
            {
                _stateTimer = stats.searchDuration;
            }

            if (newState == RaptorState.Idle && _stateTimer <= 0f)
            {
                _stateTimer = stats.idleTime;
            }
        }
    }
}
