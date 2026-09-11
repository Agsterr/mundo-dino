using OpenWorldDinoSurvival.Inventory;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    public class ResourceNodeSpawner : MonoBehaviour
    {
        [SerializeField] private Vector3[] scrapPositions =
        {
            new Vector3(-14f, 0.5f, 8f),
            new Vector3(14f, 0.5f, -6f),
        };

        [SerializeField] private Vector3[] fiberPositions =
        {
            new Vector3(-5f, 0.5f, 10f),
            new Vector3(8f, 0.5f, 12f),
            new Vector3(-10f, 0.5f, -8f),
        };

        private void Start()
        {
            foreach (Vector3 position in scrapPositions)
            {
                CreateNode("ScrapNode", position, PrimitiveType.Cube, ItemIds.ScrapMetal, 3,
                    new Color(0.55f, 0.55f, 0.6f));
            }

            foreach (Vector3 position in fiberPositions)
            {
                CreateNode("FiberBush", position, PrimitiveType.Cylinder, ItemIds.Fiber, 2,
                    new Color(0.2f, 0.7f, 0.25f));
            }
        }

        private static void CreateNode(string nodeName, Vector3 position, PrimitiveType shape, int itemId, int amount, Color color)
        {
            GameObject node = GameObject.CreatePrimitive(shape);
            node.name = nodeName;
            node.transform.position = position;
            node.transform.localScale = shape == PrimitiveType.Cylinder
                ? new Vector3(1.2f, 0.8f, 1.2f)
                : new Vector3(1f, 0.8f, 1f);

            ResourceNode resource = node.AddComponent<ResourceNode>();
            resource.Configure(itemId, amount, color);

            Renderer renderer = node.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = color;
            }
        }
    }
}
