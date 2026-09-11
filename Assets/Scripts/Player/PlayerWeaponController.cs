using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Weapons;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OpenWorldDinoSurvival.Player
{
    public class PlayerWeaponController : MonoBehaviour
    {
        [SerializeField] private Camera aimCamera;
        [SerializeField] private Weapon pistol;
        [SerializeField] private Weapon rifle;
        [SerializeField] private WeaponStats pistolStats;
        [SerializeField] private WeaponStats rifleStats;

        private Weapon _activeWeapon;
        private bool _fireHeld;
        private NetworkPlayerCombat _networkCombat;
        private bool _networkReloading;

        public Weapon ActiveWeapon => _activeWeapon;

        public event System.Action<Weapon> OnWeaponChanged;

        private void Awake()
        {
            if (aimCamera == null)
            {
                aimCamera = GetComponentInChildren<Camera>();
            }

            if (pistolStats == null)
            {
                pistolStats = Resources.Load<WeaponStats>("Weapons/PistolStats");
            }

            if (rifleStats == null)
            {
                rifleStats = Resources.Load<WeaponStats>("Weapons/RifleStats");
            }

            pistol ??= FindOrCreateWeapon("Pistol", pistolStats);
            rifle ??= FindOrCreateWeapon("Rifle", rifleStats);

            EquipWeapon(pistol);
        }

        private Weapon FindOrCreateWeapon(string weaponName, WeaponStats stats)
        {
            Transform existing = transform.Find(weaponName);
            if (existing != null && existing.TryGetComponent(out Weapon found))
            {
                if (stats != null)
                {
                    found.SetStats(stats);
                }

                return found;
            }

            GameObject weaponObject = new GameObject(weaponName);
            weaponObject.transform.SetParent(transform);
            weaponObject.transform.localPosition = new Vector3(0.35f, 1.2f, 0.45f);

            Weapon weapon = weaponObject.AddComponent<Weapon>();
            if (stats != null)
            {
                weapon.SetStats(stats);
            }

            return weapon;
        }

        public void SetNetworkHandler(NetworkPlayerCombat combat)
        {
            _networkCombat = combat;
        }

        public void ClearNetworkHandler()
        {
            _networkCombat = null;
        }

        public void ApplyNetworkAmmo(
            int activeWeaponIndex,
            int pistolMagazine,
            int pistolReserve,
            int rifleMagazine,
            int rifleReserve,
            bool isReloading)
        {
            _networkReloading = isReloading;

            if (pistol != null)
            {
                pistol.SetAmmoState(pistolMagazine, pistolReserve, isReloading && activeWeaponIndex == 0);
            }

            if (rifle != null)
            {
                rifle.SetAmmoState(rifleMagazine, rifleReserve, isReloading && activeWeaponIndex == 1);
            }

            Weapon target = activeWeaponIndex == 0 ? pistol : rifle;
            if (target != null && target != _activeWeapon)
            {
                EquipWeapon(target);
            }
        }

        private void Update()
        {
            if (_activeWeapon == null || !_fireHeld)
            {
                return;
            }

            WeaponStats stats = _activeWeapon.Stats;
            if (stats != null && stats.automatic)
            {
                TryFireActiveWeapon();
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            _fireHeld = context.ReadValueAsButton();

            if (!context.performed || _activeWeapon == null)
            {
                return;
            }

            WeaponStats stats = _activeWeapon.Stats;
            if (stats != null && !stats.automatic)
            {
                TryFireActiveWeapon();
            }
        }

        public void OnReload(InputAction.CallbackContext context)
        {
            if (!context.performed || _activeWeapon == null)
            {
                return;
            }

            if (_networkCombat != null)
            {
                _networkCombat.RequestReload();
                return;
            }

            _activeWeapon.TryReload();
        }

        public void OnSwitchPistol(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (_networkCombat != null)
            {
                _networkCombat.RequestSwitchWeapon(0);
                return;
            }

            EquipWeapon(pistol);
        }

        public void OnSwitchRifle(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }

            if (_networkCombat != null)
            {
                _networkCombat.RequestSwitchWeapon(1);
                return;
            }

            EquipWeapon(rifle);
        }

        private void TryFireActiveWeapon()
        {
            if (_activeWeapon == null)
            {
                return;
            }

            if (_networkCombat != null)
            {
                if (!_networkReloading)
                {
                    _networkCombat.RequestFire(aimCamera);
                }

                return;
            }

            _activeWeapon.TryFire(aimCamera);
        }

        public void ApplyWeaponUpgrade(int weaponSlot, WeaponStats stats)
        {
            if (stats == null)
            {
                return;
            }

            if (weaponSlot == 0 && pistol != null)
            {
                pistol.SetStats(stats);
            }
            else if (weaponSlot == 1 && rifle != null)
            {
                rifle.SetStats(stats);
            }

            if (_activeWeapon != null && _activeWeapon.Stats == stats)
            {
                OnWeaponChanged?.Invoke(_activeWeapon);
            }
        }

        private void EquipWeapon(Weapon weapon)
        {
            if (weapon == null || weapon == _activeWeapon)
            {
                return;
            }

            if (pistol != null)
            {
                pistol.gameObject.SetActive(weapon == pistol);
            }

            if (rifle != null)
            {
                rifle.gameObject.SetActive(weapon == rifle);
            }

            _activeWeapon = weapon;
            OnWeaponChanged?.Invoke(_activeWeapon);
        }
    }
}
