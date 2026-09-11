using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    [CreateAssetMenu(fileName = "NewArmor", menuName = "Open World Dino Survival/Armor Stats")]
    public class ArmorStats : ScriptableObject
    {
        public string armorName = "Colete";
        [Range(0f, 0.9f)] public float damageReduction = 0.2f;
        public float healthBonus = 0f;
        public Color bodyTint = new Color(0.55f, 0.45f, 0.3f);
    }
}
