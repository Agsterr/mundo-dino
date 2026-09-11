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
    /// Gera o mapa 2×2 km com regiões, estruturas, loot e dinossauros (Milestone 6).
    /// </summary>
    public class OpenWorldGenerator : MonoBehaviour
    {
        [SerializeField] private GameObject legacyGround;
        [SerializeField] private GameObject velociraptorPrefab;
        [SerializeField] private DinosaurStats velociraptorStats;

        private Transform _worldRoot;

        private void Start()
        {
            if (legacyGround != null)
            {
                legacyGround.SetActive(false);
            }

            if (velociraptorStats == null)
            {
                velociraptorStats = Resources.Load<DinosaurStats>("Dinosaurs/VelociraptorStats");
            }

            _worldRoot = new GameObject("OpenWorld").transform;
            BuildTerrain();
            BuildRiver();
            BuildRegionMarkers();
            BuildStructures();
            SpawnResources();
            SpawnTrainingTargets();
            SpawnDinosaurs();
        }

        private void BuildTerrain()
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Terrain_2km";
            ground.transform.SetParent(_worldRoot);
            ground.transform.position = Vector3.zero;
            float planeScale = WorldRegions.MapSize / 10f;
            ground.transform.localScale = new Vector3(planeScale, 1f, planeScale);

            Renderer renderer = ground.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = new Color(0.32f, 0.38f, 0.28f);
            }
        }

        private void BuildRiver()
        {
            GameObject river = GameObject.CreatePrimitive(PrimitiveType.Cube);
            river.name = "River";
            river.transform.SetParent(_worldRoot);
            river.transform.position = new Vector3(0f, 0.05f, 0f);
            river.transform.localScale = new Vector3(WorldRegions.MapSize, 0.1f, WorldRegions.RiverHalfWidth * 2f);
            SetColor(river, WorldRegions.GetRegionColor(WorldRegionType.River));
        }

        private void BuildRegionMarkers()
        {
            CreateRegionMarker("Marker_Village", new Vector3(0f, 0.02f, 120f), 420f, 360f, WorldRegionType.Village);
            CreateRegionMarker("Marker_Forest", new Vector3(-650f, 0.02f, 0f), 700f, WorldRegions.MapSize, WorldRegionType.Forest);
            CreateRegionMarker("Marker_Mountain", new Vector3(0f, 0.02f, 650f), 600f, 700f, WorldRegionType.Mountain);
            CreateRegionMarker("Marker_Beach", new Vector3(0f, 0.02f, -650f), 600f, 700f, WorldRegionType.Beach);
            CreateRegionMarker("Marker_Port", new Vector3(650f, 0.02f, -650f), 700f, 700f, WorldRegionType.Port);
            CreateRegionMarker("Marker_Lab", new Vector3(650f, 0.02f, 650f), 700f, 700f, WorldRegionType.Laboratory);
        }

        private void CreateRegionMarker(string markerName, Vector3 center, float width, float depth, WorldRegionType region)
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Quad);
            marker.name = markerName;
            marker.transform.SetParent(_worldRoot);
            marker.transform.position = center;
            marker.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            marker.transform.localScale = new Vector3(width, depth, 1f);
            Destroy(marker.GetComponent<Collider>());

            Color color = WorldRegions.GetRegionColor(region);
            color.a = 0.35f;
            SetColor(marker, color);
        }

        private void BuildStructures()
        {
            BuildVillage();
            BuildForest();
            BuildMountain();
            BuildBeach();
            BuildPort();
            BuildLaboratory();
        }

        private void BuildVillage()
        {
            CreateBuilding("Village_House_1", new Vector3(-120f, 1.5f, 140f), new Vector3(8f, 3f, 8f), new Color(0.6f, 0.45f, 0.3f));
            CreateBuilding("Village_House_2", new Vector3(120f, 1.5f, 130f), new Vector3(7f, 3f, 7f), new Color(0.55f, 0.4f, 0.28f));
            CreateBuilding("Village_House_3", new Vector3(-90f, 1.5f, 200f), new Vector3(6f, 3f, 6f), new Color(0.58f, 0.42f, 0.3f));
            CreateBuilding("Village_Well", new Vector3(0f, 0.8f, 170f), new Vector3(3f, 1.6f, 3f), new Color(0.45f, 0.45f, 0.5f));
            CreateBuilding("Village_Barn", new Vector3(180f, 2f, 180f), new Vector3(12f, 4f, 8f), new Color(0.5f, 0.35f, 0.22f));
        }

        private void BuildForest()
        {
            Vector3[] treePositions =
            {
                new Vector3(-420f, 0f, -200f),
                new Vector3(-550f, 0f, 100f),
                new Vector3(-700f, 0f, 300f),
                new Vector3(-480f, 0f, 500f),
                new Vector3(-800f, 0f, -400f),
                new Vector3(-620f, 0f, -600f),
            };

            foreach (Vector3 position in treePositions)
            {
                CreateTree(position);
            }

            CreateBuilding("Forest_Camp", new Vector3(-500f, 1f, 0f), new Vector3(6f, 2f, 4f), new Color(0.4f, 0.3f, 0.2f));
        }

        private void BuildMountain()
        {
            Vector3[] rockPositions =
            {
                new Vector3(-150f, 0f, 520f),
                new Vector3(80f, 0f, 600f),
                new Vector3(200f, 0f, 750f),
                new Vector3(-60f, 0f, 820f),
            };

            foreach (Vector3 position in rockPositions)
            {
                float height = Random.Range(4f, 10f);
                CreateBuilding($"Mountain_Rock_{position.x}", new Vector3(position.x, height * 0.5f, position.z),
                    new Vector3(Random.Range(6f, 12f), height, Random.Range(6f, 12f)),
                    new Color(0.45f, 0.45f, 0.48f));
            }
        }

        private void BuildBeach()
        {
            CreateBuilding("Beach_Camp_1", new Vector3(-200f, 0.6f, -520f), new Vector3(4f, 1.2f, 4f), new Color(0.9f, 0.85f, 0.6f));
            CreateBuilding("Beach_Camp_2", new Vector3(150f, 0.6f, -600f), new Vector3(4f, 1.2f, 4f), new Color(0.88f, 0.82f, 0.58f));
            CreateBuilding("Beach_Lookout", new Vector3(0f, 2f, -750f), new Vector3(3f, 4f, 3f), new Color(0.75f, 0.7f, 0.5f));
        }

        private void BuildPort()
        {
            CreateBuilding("Port_Dock", new Vector3(700f, 0.3f, -700f), new Vector3(30f, 0.6f, 12f), new Color(0.35f, 0.28f, 0.2f));
            CreateBuilding("Port_Crates", new Vector3(620f, 1f, -620f), new Vector3(8f, 2f, 8f), new Color(0.5f, 0.35f, 0.2f));
            CreateBuilding("Port_Warehouse", new Vector3(780f, 2.5f, -580f), new Vector3(16f, 5f, 10f), new Color(0.42f, 0.4f, 0.38f));
        }

        private void BuildLaboratory()
        {
            CreateBuilding("Lab_Main", new Vector3(700f, 3f, 700f), new Vector3(20f, 6f, 14f), new Color(0.7f, 0.75f, 0.82f));
            CreateBuilding("Lab_Antenna", new Vector3(760f, 5f, 760f), new Vector3(1.5f, 10f, 1.5f), new Color(0.55f, 0.55f, 0.6f));
            CreateBuilding("Lab_Fence", new Vector3(650f, 1f, 650f), new Vector3(40f, 2f, 1f), new Color(0.5f, 0.5f, 0.55f));
        }

        private void SpawnResources()
        {
            SpawnScrapNodes(new[]
            {
                new Vector3(-450f, 0.5f, -300f),
                new Vector3(450f, 0.5f, -550f),
                new Vector3(600f, 0.5f, 600f),
                new Vector3(-200f, 0.5f, 500f),
                new Vector3(100f, 0.5f, -400f),
            });

            SpawnFiberNodes(new[]
            {
                new Vector3(-600f, 0.5f, 200f),
                new Vector3(-350f, 0.5f, -150f),
                new Vector3(250f, 0.5f, -500f),
                new Vector3(-100f, 0.5f, 250f),
                new Vector3(500f, 0.5f, 500f),
                new Vector3(300f, 0.5f, -700f),
            });
        }

        private void SpawnTrainingTargets()
        {
            Vector3[] positions =
            {
                new Vector3(-60f, 1f, 150f),
                new Vector3(60f, 1f, 150f),
                new Vector3(0f, 1f, 220f),
            };

            foreach (Vector3 position in positions)
            {
                CreateTrainingTarget(position);
            }
        }

        private void SpawnDinosaurs()
        {
            Vector3[] positions =
            {
                new Vector3(-550f, 1f, 350f),
                new Vector3(-700f, 1f, -250f),
                new Vector3(120f, 1f, 680f),
                new Vector3(-80f, 1f, 780f),
                new Vector3(720f, 1f, 620f),
            };

            foreach (Vector3 position in positions)
            {
                SpawnVelociraptor(position);
            }
        }

        private void CreateBuilding(string buildingName, Vector3 position, Vector3 scale, Color color)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = buildingName;
            building.transform.SetParent(_worldRoot);
            building.transform.position = position;
            building.transform.localScale = scale;
            SetColor(building, color);
        }

        private void CreateTree(Vector3 position)
        {
            GameObject trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Forest_Tree";
            trunk.transform.SetParent(_worldRoot);
            trunk.transform.position = new Vector3(position.x, 2f, position.z);
            trunk.transform.localScale = new Vector3(1.2f, 4f, 1.2f);
            SetColor(trunk, new Color(0.35f, 0.22f, 0.12f));

            GameObject leaves = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            leaves.name = "Forest_Tree_Leaves";
            leaves.transform.SetParent(trunk.transform);
            leaves.transform.localPosition = new Vector3(0f, 1.2f, 0f);
            leaves.transform.localScale = new Vector3(2.5f, 2f, 2.5f);
            SetColor(leaves, new Color(0.15f, 0.5f, 0.18f));
        }

        private void SpawnScrapNodes(Vector3[] positions)
        {
            foreach (Vector3 position in positions)
            {
                CreateResourceNode("ScrapNode", position, PrimitiveType.Cube, ItemIds.ScrapMetal, 3,
                    new Color(0.55f, 0.55f, 0.6f));
            }
        }

        private void SpawnFiberNodes(Vector3[] positions)
        {
            foreach (Vector3 position in positions)
            {
                CreateResourceNode("FiberBush", position, PrimitiveType.Cylinder, ItemIds.Fiber, 2,
                    new Color(0.2f, 0.7f, 0.25f));
            }
        }

        private static void CreateResourceNode(string nodeName, Vector3 position, PrimitiveType shape, int itemId, int amount, Color color)
        {
            GameObject node = GameObject.CreatePrimitive(shape);
            node.name = nodeName;
            node.transform.position = position;
            node.transform.localScale = shape == PrimitiveType.Cylinder
                ? new Vector3(1.2f, 0.8f, 1.2f)
                : new Vector3(1f, 0.8f, 1f);

            ResourceNode resource = node.AddComponent<ResourceNode>();
            resource.Configure(itemId, amount, color);
            SetColor(node, color);
        }

        private void CreateTrainingTarget(Vector3 position)
        {
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "TrainingTarget";
            target.transform.SetParent(_worldRoot);
            target.transform.position = position;
            target.transform.localScale = new Vector3(1.5f, 2f, 0.5f);
            SetColor(target, new Color(0.8f, 0.2f, 0.2f));

            Health health = target.AddComponent<Health>();
            health.Initialize(100f);

            LootOnDeath loot = target.AddComponent<LootOnDeath>();
            loot.SetDrops(new[]
            {
                new LootDropEntry
                {
                    itemId = ItemIds.ScrapMetal,
                    amount = 2,
                    chance = 1f,
                    color = new Color(0.65f, 0.65f, 0.7f)
                },
                new LootDropEntry
                {
                    itemId = ItemIds.GunParts,
                    amount = 1,
                    chance = 0.35f,
                    color = new Color(0.75f, 0.75f, 0.8f)
                }
            });
        }

        private void SpawnVelociraptor(Vector3 position)
        {
            bool isNetworked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
            if (isNetworked && !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            GameObject raptor;
            if (velociraptorPrefab != null)
            {
                raptor = Instantiate(velociraptorPrefab, position, Quaternion.identity, _worldRoot);
                raptor.name = "Velociraptor";
                VelociraptorAI ai = raptor.GetComponent<VelociraptorAI>();
                if (ai != null && velociraptorStats != null)
                {
                    ai.Initialize(velociraptorStats);
                }
            }
            else
            {
                raptor = CreateRuntimeVelociraptor(position);
            }

            NetworkObject networkObject = raptor.GetComponent<NetworkObject>();
            if (networkObject != null && isNetworked)
            {
                networkObject.Spawn();
            }
        }

        private GameObject CreateRuntimeVelociraptor(Vector3 position)
        {
            GameObject raptor = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            raptor.name = "Velociraptor";
            raptor.transform.SetParent(_worldRoot);
            raptor.transform.position = position;
            raptor.transform.localScale = new Vector3(1.2f, 1f, 1.2f);
            Destroy(raptor.GetComponent<CapsuleCollider>());

            CharacterController controller = raptor.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;

            SetColor(raptor, new Color(0.45f, 0.35f, 0.15f));

            raptor.AddComponent<Health>();
            VelociraptorAI ai = raptor.AddComponent<VelociraptorAI>();
            ai.Initialize(velociraptorStats);
            raptor.AddComponent<DinosaurMountable>();
            raptor.AddComponent<DinosaurPlayerControl>();

            LootOnDeath loot = raptor.AddComponent<LootOnDeath>();
            loot.SetDrops(new[]
            {
                new LootDropEntry { itemId = ItemIds.DinoHide, amount = 4, chance = 1f, color = new Color(0.45f, 0.3f, 0.15f) },
                new LootDropEntry { itemId = ItemIds.GunParts, amount = 2, chance = 0.7f, color = new Color(0.7f, 0.7f, 0.75f) }
            });

            raptor.AddComponent<NetworkObject>();
            return raptor;
        }

        private static void SetColor(GameObject target, Color color)
        {
            Renderer renderer = target.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
}
