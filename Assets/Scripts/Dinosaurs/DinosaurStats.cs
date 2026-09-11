using UnityEngine;

namespace OpenWorldDinoSurvival.Dinosaurs
{
    [CreateAssetMenu(fileName = "VelociraptorStats", menuName = "Open World Dino Survival/Dinosaur Stats")]
    public class DinosaurStats : ScriptableObject
    {
        [Header("Identificação")]
        public string dinosaurName = "Velociraptor";

        [Header("Vida")]
        public float maxHealth = 150f;

        [Header("Movimento")]
        public float walkSpeed = 2.5f;
        public float runSpeed = 6f;
        public float rotationSpeed = 8f;

        [Header("Combate")]
        public float attackDamage = 20f;
        public float attackRange = 2f;
        public float attackCooldown = 1.2f;

        [Header("Percepção")]
        public float sightRange = 18f;
        [Range(10f, 180f)] public float sightAngle = 110f;
        public float hearingRange = 25f;

        [Header("Patrulha")]
        public float patrolRadius = 12f;
        public float patrolWaitTime = 2f;
        public float idleTime = 1.5f;

        [Header("Busca")]
        public float searchDuration = 5f;
        public float loseTargetDistance = 30f;
    }
}
