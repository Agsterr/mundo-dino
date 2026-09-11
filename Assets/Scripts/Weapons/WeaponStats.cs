using UnityEngine;

namespace OpenWorldDinoSurvival.Weapons
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Open World Dino Survival/Weapon Stats")]
    public class WeaponStats : ScriptableObject
    {
        [Header("Identificação")]
        public string weaponName = "Pistola";

        [Header("Combate")]
        public float damage = 25f;
        public float range = 50f;
        public float fireRate = 0.3f;
        [Range(0f, 10f)] public float spreadDegrees = 1.5f;
        public bool automatic = false;

        [Header("Munição")]
        public int magazineSize = 12;
        public int maxReserveAmmo = 60;
        public float reloadTime = 1.5f;
    }
}
