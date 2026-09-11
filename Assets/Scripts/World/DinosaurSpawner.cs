using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Systems;
using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    /// <summary>
    /// Spawna o Velociraptor na cena (Milestone 3).
    /// </summary>
    public class DinosaurSpawner : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnPosition = new Vector3(12f, 1f, 18f);
        [SerializeField] private DinosaurStats stats;
        [SerializeField] private Color bodyColor = new Color(0.45f, 0.35f, 0.15f);

        private void Start()
        {
            if (stats == null)
            {
                stats = Resources.Load<DinosaurStats>("Dinosaurs/VelociraptorStats");
            }

            CreateVelociraptor();
        }

        private void CreateVelociraptor()
        {
            GameObject raptor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            raptor.name = "Velociraptor";
            raptor.transform.position = spawnPosition;
            raptor.transform.localScale = new Vector3(1.2f, 1f, 1.2f);

            Destroy(raptor.GetComponent<CapsuleCollider>());

            CharacterController controller = raptor.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;
            controller.center = Vector3.zero;

            Renderer renderer = raptor.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = bodyColor;
            }

            raptor.AddComponent<Health>();
            VelociraptorAI ai = raptor.AddComponent<VelociraptorAI>();
            ai.Initialize(stats);

            LootOnDeath loot = raptor.AddComponent<LootOnDeath>();
            loot.SetDrops(new[]
            {
                new LootDropEntry
                {
                    itemId = ItemIds.DinoHide,
                    amount = 4,
                    chance = 1f,
                    color = new Color(0.45f, 0.3f, 0.15f)
                },
                new LootDropEntry
                {
                    itemId = ItemIds.GunParts,
                    amount = 2,
                    chance = 0.7f,
                    color = new Color(0.7f, 0.7f, 0.75f)
                }
            });
        }
    }
}


