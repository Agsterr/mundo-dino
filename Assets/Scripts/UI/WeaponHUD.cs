using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Weapons;
using UnityEngine;

namespace OpenWorldDinoSurvival.UI
{
    /// <summary>
    /// HUD simples de munição e mira (OnGUI — sem Canvas necessário no Milestone 2).
    /// </summary>
    public class WeaponHUD : MonoBehaviour
    {
        [SerializeField] private PlayerWeaponController weaponController;
        [SerializeField] private PlayerHealth playerHealth;
        [SerializeField] private NetworkPlayerHealth networkPlayerHealth;
        [SerializeField] private PlayerArmor playerArmor;
        [SerializeField] private NetworkPlayerInventory networkInventory;

        private Weapon _trackedWeapon;
        private GUIStyle _labelStyle;
        private GUIStyle _crosshairStyle;
        private GUIStyle _healthStyle;

        private void Awake()
        {
            weaponController ??= GetComponent<PlayerWeaponController>();
            networkPlayerHealth ??= GetComponent<NetworkPlayerHealth>();
            playerHealth ??= GetComponent<PlayerHealth>();
            playerArmor ??= GetComponent<PlayerArmor>();
            networkInventory ??= GetComponent<NetworkPlayerInventory>();

            weaponController ??= FindFirstObjectByType<PlayerWeaponController>();
            playerHealth ??= FindFirstObjectByType<PlayerHealth>();
            networkPlayerHealth ??= FindFirstObjectByType<NetworkPlayerHealth>();
        }

        private void OnEnable()
        {
            if (weaponController == null)
            {
                return;
            }

            weaponController.OnWeaponChanged += HandleWeaponChanged;
            HandleWeaponChanged(weaponController.ActiveWeapon);
        }

        private void OnDisable()
        {
            if (weaponController != null)
            {
                weaponController.OnWeaponChanged -= HandleWeaponChanged;
            }

            UnsubscribeWeaponEvents();
        }

        private void HandleWeaponChanged(Weapon weapon)
        {
            UnsubscribeWeaponEvents();
            _trackedWeapon = weapon;

            if (_trackedWeapon != null)
            {
                _trackedWeapon.OnAmmoChanged += MarkDirty;
                _trackedWeapon.OnReloadStarted += MarkDirty;
                _trackedWeapon.OnReloadFinished += MarkDirty;
            }
        }

        private void UnsubscribeWeaponEvents()
        {
            if (_trackedWeapon == null)
            {
                return;
            }

            _trackedWeapon.OnAmmoChanged -= MarkDirty;
            _trackedWeapon.OnReloadStarted -= MarkDirty;
            _trackedWeapon.OnReloadFinished -= MarkDirty;
        }

        private void MarkDirty() { }

        private void OnGUI()
        {
            EnsureStyles();

            if (_trackedWeapon == null || _trackedWeapon.Stats == null)
            {
                GUI.Label(new Rect(16, 16, 400, 28), "Sem arma", _labelStyle);
                DrawCrosshair();
                return;
            }

            WeaponStats stats = _trackedWeapon.Stats;
            string reload = _trackedWeapon.IsReloading ? "  |  Recarregando..." : string.Empty;
            GUI.Label(
                new Rect(16, 16, 500, 28),
                $"{stats.weaponName}: {_trackedWeapon.AmmoInMagazine} / {_trackedWeapon.ReserveAmmo}{reload}",
                _labelStyle);

            if (networkPlayerHealth != null)
            {
                string lifeText = networkPlayerHealth.IsAlive
                    ? $"Vida: {Mathf.CeilToInt(networkPlayerHealth.CurrentHealth)} / {Mathf.CeilToInt(networkPlayerHealth.MaxHealth)}"
                    : "Você morreu — respawn em breve...";
                GUI.Label(new Rect(16, 44, 400, 28), lifeText, _healthStyle);
            }
            else if (playerHealth != null)
            {
                string lifeText = playerHealth.IsAlive
                    ? $"Vida: {Mathf.CeilToInt(playerHealth.CurrentHealth)} / {Mathf.CeilToInt(playerHealth.MaxHealth)}"
                    : "Você morreu — respawn em breve...";
                GUI.Label(new Rect(16, 44, 400, 28), lifeText, _healthStyle);
            }

            DrawArmorInfo();
            DrawCrosshair();
        }

        private void DrawArmorInfo()
        {
            string armorText = "Armadura: nenhuma";
            if (networkInventory != null && networkInventory.ArmorTier > 0)
            {
                armorText = networkInventory.ArmorTier == 1
                    ? "Armadura: Colete de Couro"
                    : "Armadura: Placas de Metal";
            }
            else if (playerArmor != null && playerArmor.EquippedArmor != null)
            {
                armorText = $"Armadura: {playerArmor.EquippedArmor.armorName}";
            }

            GUI.Label(new Rect(16, 72, 400, 24), armorText, _healthStyle);
        }

        private void DrawCrosshair()
        {
            float size = 12f;
            float x = Screen.width * 0.5f - size * 0.5f;
            float y = Screen.height * 0.5f - size * 0.5f;
            GUI.Label(new Rect(x, y, size, size), "+", _crosshairStyle);
        }

        private void EnsureStyles()
        {
            if (_labelStyle != null)
            {
                return;
            }

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Bold,
                normal = { textColor = Color.white }
            };

            _crosshairStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 22,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = new Color(1f, 1f, 1f, 0.85f) }
            };

            _healthStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold,
                normal = { textColor = new Color(0.4f, 1f, 0.5f) }
            };
        }
    }
}
