using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Systems;
using OpenWorldDinoSurvival.Weapons;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Multiplayer
{
    /// <summary>
    /// Tiros e munição autoritativos no servidor (Milestone 4).
    /// </summary>
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerCombat : NetworkBehaviour
    {
        [SerializeField] private PlayerWeaponController weaponController;
        [SerializeField] private Weapon pistol;
        [SerializeField] private Weapon rifle;
        [SerializeField] private LayerMask hitMask = ~0;

        private readonly NetworkVariable<int> _pistolMagazine = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _pistolReserve = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _rifleMagazine = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _rifleReserve = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _activeWeaponIndex = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<bool> _isReloading = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private WeaponStats _pistolStats;
        private WeaponStats _rifleStats;
        private float _nextPistolFireTime;
        private float _nextRifleFireTime;
        private int _reloadingWeaponIndex;

        public int ActiveWeaponIndex => _activeWeaponIndex.Value;
        public bool IsReloading => _isReloading.Value;

        public int GetMagazineAmmo(int weaponIndex) => weaponIndex == 0 ? _pistolMagazine.Value : _rifleMagazine.Value;
        public int GetReserveAmmo(int weaponIndex) => weaponIndex == 0 ? _pistolReserve.Value : _rifleReserve.Value;

        private void Reset()
        {
            weaponController = GetComponent<PlayerWeaponController>();
        }

        private void Awake()
        {
            weaponController ??= GetComponent<PlayerWeaponController>();
            _pistolStats = Resources.Load<WeaponStats>("Weapons/PistolStats");
            _rifleStats = Resources.Load<WeaponStats>("Weapons/RifleStats");
            pistol ??= weaponController != null ? weaponController.ActiveWeapon : null;
        }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                ResetServerAmmo();
            }

            pistol = transform.Find("Pistol")?.GetComponent<Weapon>();
            rifle = transform.Find("Rifle")?.GetComponent<Weapon>();

            if (IsOwner && weaponController != null)
            {
                weaponController.SetNetworkHandler(this);
            }

            _pistolMagazine.OnValueChanged += (_, __) => SyncAmmoToWeapons();
            _rifleMagazine.OnValueChanged += (_, __) => SyncAmmoToWeapons();
            _pistolReserve.OnValueChanged += (_, __) => SyncAmmoToWeapons();
            _rifleReserve.OnValueChanged += (_, __) => SyncAmmoToWeapons();
            _activeWeaponIndex.OnValueChanged += (_, __) => SyncAmmoToWeapons();
            _isReloading.OnValueChanged += (_, __) => SyncAmmoToWeapons();

            SyncAmmoToWeapons();
        }

        public override void OnNetworkDespawn()
        {
            if (IsOwner && weaponController != null)
            {
                weaponController.ClearNetworkHandler();
            }
        }

        private void ResetServerAmmo()
        {
            if (_pistolStats != null)
            {
                _pistolMagazine.Value = _pistolStats.magazineSize;
                _pistolReserve.Value = _pistolStats.maxReserveAmmo;
            }

            if (_rifleStats != null)
            {
                _rifleMagazine.Value = _rifleStats.magazineSize;
                _rifleReserve.Value = _rifleStats.maxReserveAmmo;
            }

            _activeWeaponIndex.Value = 0;
            _isReloading.Value = false;
        }

        public bool RequestFire(Camera aimCamera)
        {
            if (!IsOwner || aimCamera == null || _isReloading.Value)
            {
                return false;
            }

            Ray ray = aimCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 origin = transform.position + Vector3.up * 1.2f;
            RequestFireServerRpc(origin, ray.direction, _activeWeaponIndex.Value);
            return true;
        }

        public bool RequestReload()
        {
            if (!IsOwner || _isReloading.Value)
            {
                return false;
            }

            RequestReloadServerRpc(_activeWeaponIndex.Value);
            return true;
        }

        public void RequestSwitchWeapon(int weaponIndex)
        {
            if (!IsOwner)
            {
                return;
            }

            RequestSwitchWeaponServerRpc(weaponIndex);
        }

        [ServerRpc]
        private void RequestFireServerRpc(Vector3 origin, Vector3 direction, int weaponIndex, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || _isReloading.Value)
            {
                return;
            }

            WeaponStats stats = weaponIndex == 0 ? _pistolStats : _rifleStats;
            if (stats == null)
            {
                return;
            }

            if (Time.time < (weaponIndex == 0 ? _nextPistolFireTime : _nextRifleFireTime))
            {
                return;
            }

            int magazine = weaponIndex == 0 ? _pistolMagazine.Value : _rifleMagazine.Value;
            if (magazine <= 0)
            {
                return;
            }

            if (weaponIndex == 0)
            {
                _pistolMagazine.Value--;
                _nextPistolFireTime = Time.time + stats.fireRate;
            }
            else
            {
                _rifleMagazine.Value--;
                _nextRifleFireTime = Time.time + stats.fireRate;
            }

            direction = ApplySpread(direction, stats.spreadDegrees);
            Vector3 endPoint = origin + direction * stats.range;

            if (Physics.Raycast(origin, direction, out RaycastHit hit, stats.range, hitMask, QueryTriggerInteraction.Ignore))
            {
                endPoint = hit.point;
                ApplyDamage(hit.collider, stats.damage);
            }

            NotifyGunfire(origin);
            PlayShotEffectsClientRpc(origin, endPoint);
        }

        [ServerRpc]
        private void RequestReloadServerRpc(int weaponIndex, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || _isReloading.Value)
            {
                return;
            }

            WeaponStats stats = weaponIndex == 0 ? _pistolStats : _rifleStats;
            if (stats == null)
            {
                return;
            }

            int magazine = weaponIndex == 0 ? _pistolMagazine.Value : _rifleMagazine.Value;
            int reserve = weaponIndex == 0 ? _pistolReserve.Value : _rifleReserve.Value;
            if (magazine >= stats.magazineSize || reserve <= 0)
            {
                return;
            }

            _isReloading.Value = true;
            _reloadingWeaponIndex = weaponIndex;
            Invoke(nameof(FinishReload), stats.reloadTime);
        }

        [ServerRpc]
        private void RequestSwitchWeaponServerRpc(int weaponIndex, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            _activeWeaponIndex.Value = Mathf.Clamp(weaponIndex, 0, 1);
        }

        private void FinishReload()
        {
            if (!IsServer)
            {
                return;
            }

            int weaponIndex = _reloadingWeaponIndex;
            WeaponStats stats = weaponIndex == 0 ? _pistolStats : _rifleStats;
            if (stats == null)
            {
                _isReloading.Value = false;
                return;
            }

            int magazine = weaponIndex == 0 ? _pistolMagazine.Value : _rifleMagazine.Value;
            int reserve = weaponIndex == 0 ? _pistolReserve.Value : _rifleReserve.Value;
            int needed = stats.magazineSize - magazine;
            int toLoad = Mathf.Min(needed, reserve);

            if (weaponIndex == 0)
            {
                _pistolMagazine.Value += toLoad;
                _pistolReserve.Value -= toLoad;
            }
            else
            {
                _rifleMagazine.Value += toLoad;
                _rifleReserve.Value -= toLoad;
            }

            _isReloading.Value = false;
        }

        [ClientRpc]
        private void PlayShotEffectsClientRpc(Vector3 origin, Vector3 endPoint)
        {
            Weapon active = _activeWeaponIndex.Value == 0 ? pistol : rifle;
            active?.PlayShotVisual(origin, endPoint);
        }

        private static void ApplyDamage(Collider hitCollider, float damage)
        {
            NetworkPlayerHealth playerHealth = hitCollider.GetComponentInParent<NetworkPlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                return;
            }

            Health health = hitCollider.GetComponentInParent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        private static void NotifyGunfire(Vector3 shotOrigin)
        {
            VelociraptorAI[] raptors = FindObjectsByType<VelociraptorAI>(FindObjectsSortMode.None);
            foreach (VelociraptorAI raptor in raptors)
            {
                raptor.AlertToGunfire(shotOrigin);
            }
        }

        private static Vector3 ApplySpread(Vector3 direction, float spreadDegrees)
        {
            if (spreadDegrees <= 0f)
            {
                return direction.normalized;
            }

            float spreadX = Random.Range(-spreadDegrees, spreadDegrees);
            float spreadY = Random.Range(-spreadDegrees, spreadDegrees);
            Quaternion spread = Quaternion.Euler(spreadY, spreadX, 0f);
            return (spread * direction).normalized;
        }

        private void SyncAmmoToWeapons()
        {
            if (weaponController == null)
            {
                return;
            }

            weaponController.ApplyNetworkAmmo(
                _activeWeaponIndex.Value,
                _pistolMagazine.Value,
                _pistolReserve.Value,
                _rifleMagazine.Value,
                _rifleReserve.Value,
                _isReloading.Value);
        }
    }
}
