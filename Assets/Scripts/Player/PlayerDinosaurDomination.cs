using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Player
{
    /// <summary>
    /// Domina dinossauros enfraquecidos e controla seus movimentos.
    /// </summary>
    public class PlayerDinosaurDomination : MonoBehaviour
    {
        [SerializeField] private float dominateCooldown = 3f;

        private DinosaurMountable _mountedDino;
        private DinosaurPlayerControl _mountedControl;
        private NetworkPlayerAbilities _networkAbilities;
        private PlayerMovement _movement;
        private PlayerWeaponController _weaponController;
        private PlayerWings _wings;
        private Renderer _bodyRenderer;
        private float _nextDominateTime;
        private bool _isDominating;
        private Vector2 _lastMoveInput;
        private bool _lastSprintInput;

        public bool IsDominating => _isDominating;
        public DinosaurMountable MountedDino => _mountedDino;
        public float DominateCooldownRemaining => Mathf.Max(0f, _nextDominateTime - Time.time);

        private void Awake()
        {
            _networkAbilities = GetComponent<NetworkPlayerAbilities>();
            _movement = GetComponent<PlayerMovement>();
            _weaponController = GetComponent<PlayerWeaponController>();
            _wings = GetComponent<PlayerWings>();
            _bodyRenderer = GetComponentInChildren<Renderer>();
        }

        public void TryToggleDomination()
        {
            if (_isDominating)
            {
                ReleaseDomination();
                return;
            }

            if (Time.time < _nextDominateTime)
            {
                return;
            }

            DinosaurMountable target = FindDominateTarget();
            if (target == null)
            {
                return;
            }

            if (_networkAbilities != null)
            {
                _networkAbilities.RequestDominateServerRpc(target.NetworkObjectId);
                _nextDominateTime = Time.time + dominateCooldown;
                return;
            }

            BeginDominationLocal(target);
            _nextDominateTime = Time.time + dominateCooldown;
        }

        public void BeginDomination(DinosaurMountable dinosaur)
        {
            if (dinosaur == null || _isDominating)
            {
                return;
            }

            _mountedDino = dinosaur;
            _mountedControl = dinosaur.GetComponent<DinosaurPlayerControl>();
            _isDominating = true;
            SetPlayerBodyVisible(false);
            SetPlayerControlsEnabled(false);
        }

        public void ReleaseDomination()
        {
            if (!_isDominating)
            {
                return;
            }

            if (_networkAbilities != null)
            {
                _networkAbilities.RequestReleaseDominationServerRpc();
            }
            else
            {
                EndDominationLocal();
            }
        }

        public void EndDomination()
        {
            if (!_isDominating)
            {
                return;
            }

            if (_mountedDino != null)
            {
                transform.position = _mountedDino.transform.position + Vector3.up * 0.5f;
            }

            _mountedDino = null;
            _mountedControl = null;
            _isDominating = false;
            SetPlayerBodyVisible(true);
            SetPlayerControlsEnabled(true);
        }

        public void RelayDinoMove(Vector2 moveInput, bool sprint)
        {
            if (!_isDominating)
            {
                return;
            }

            _lastMoveInput = moveInput;
            _lastSprintInput = sprint;

            if (_networkAbilities != null)
            {
                _networkAbilities.RelayDinoInputServerRpc(moveInput, sprint, false);
                return;
            }

            if (_mountedControl != null)
            {
                _mountedControl.SetMoveInput(moveInput);
                _mountedControl.SetSprintInput(sprint);
            }
        }

        public void RelayDinoAttack()
        {
            if (!_isDominating)
            {
                return;
            }

            if (_networkAbilities != null)
            {
                _networkAbilities.RelayDinoInputServerRpc(_lastMoveInput, _lastSprintInput, true);
                return;
            }

            if (_mountedControl != null)
            {
                _mountedControl.RequestAttack();
            }
        }

        private void LateUpdate()
        {
            if (!_isDominating || _mountedDino == null)
            {
                return;
            }

            transform.position = _mountedDino.transform.position + Vector3.up * 1.5f;
            transform.rotation = _mountedDino.transform.rotation;
        }

        private void BeginDominationLocal(DinosaurMountable target)
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                return;
            }

            target.TryDominate(0, transform);
            BeginDomination(target);
        }

        private void EndDominationLocal()
        {
            if (_mountedDino != null)
            {
                _mountedDino.ReleasePossession();
            }

            EndDomination();
        }

        private DinosaurMountable FindDominateTarget()
        {
            DinosaurMountable[] dinosaurs = FindObjectsByType<DinosaurMountable>(FindObjectsSortMode.None);
            DinosaurMountable closest = null;
            float bestDistance = float.MaxValue;

            foreach (DinosaurMountable dinosaur in dinosaurs)
            {
                if (!dinosaur.CanBeDominatedBy(transform))
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, dinosaur.transform.position);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    closest = dinosaur;
                }
            }

            return closest;
        }

        private void SetPlayerBodyVisible(bool visible)
        {
            if (_bodyRenderer != null)
            {
                _bodyRenderer.enabled = visible;
            }
        }

        private void SetPlayerControlsEnabled(bool enabled)
        {
            if (_movement != null)
            {
                _movement.enabled = enabled;
            }

            if (_weaponController != null)
            {
                _weaponController.enabled = enabled;
            }

            if (_wings != null && !enabled && _wings.WingsDeployed)
            {
                _wings.ToggleWings();
            }
        }
    }
}
