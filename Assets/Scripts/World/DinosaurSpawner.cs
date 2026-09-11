using OpenWorldDinoSurvival.AI;
using OpenWorldDinoSurvival.Dinosaurs;
using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Systems;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.World
{
    /// <summary>
    /// Spawna o Velociraptor na cena (Milestone 3) com suporte a dominação em rede.
    /// </summary>
    public class DinosaurSpawner : MonoBehaviour
    {
        [SerializeField] private Vector3 spawnPosition = new Vector3(12f, 1f, 18f);
        [SerializeField] private DinosaurStats stats;
        [SerializeField] private Color bodyColor = new Color(0.45f, 0.35f, 0.15f);
        [SerializeField] private GameObject velociraptorPrefab;

        private void Start()
        {
            if (stats == null)
            {
                stats = Resources.Load<DinosaurStats>("Dinosaurs/VelociraptorStats");
            }

            if (velociraptorPrefab != null)
            {
                SpawnFromPrefab();
                return;
            }

            CreateVelociraptorRuntime();
        }

        private void SpawnFromPrefab()
        {
            GameObject raptor = Instantiate(velociraptorPrefab, spawnPosition, Quaternion.identity);
            raptor.name = "Velociraptor";

            VelociraptorAI ai = raptor.GetComponent<VelociraptorAI>();
            if (ai != null && stats != null)
            {
                ai.Initialize(stats);
            }

            NetworkObject networkObject = raptor.GetComponent<NetworkObject>();
            if (networkObject != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsServer)
            {
                networkObject.Spawn();
            }
        }

        private void CreateVelociraptorRuntime()
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
            raptor.AddComponent<DinosaurMountable>();
            raptor.AddComponent<DinosaurPlayerControl>();

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

            NetworkObject networkObject = raptor.AddComponent<NetworkObject>();
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening && NetworkManager.Singleton.IsServer)
            {
                networkObject.Spawn();
            }
        }
    }
}
