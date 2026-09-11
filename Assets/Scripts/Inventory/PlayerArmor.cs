using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class PlayerArmor : MonoBehaviour
    {
        [SerializeField] private Renderer bodyRenderer;

        private ArmorStats _equippedArmor;

        public ArmorStats EquippedArmor => _equippedArmor;
        public float DamageReduction => _equippedArmor != null ? _equippedArmor.damageReduction : 0f;
        public float HealthBonus => _equippedArmor != null ? _equippedArmor.healthBonus : 0f;

        private void Awake()
        {
            bodyRenderer ??= GetComponentInChildren<Renderer>();
            if (bodyRenderer == null)
            {
                bodyRenderer = GetComponent<Renderer>();
            }
        }

        public void Equip(ArmorStats armor)
        {
            _equippedArmor = armor;

            if (bodyRenderer != null && armor != null)
            {
                bodyRenderer.material.color = armor.bodyTint;
            }
        }
    }
}
