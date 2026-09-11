using OpenWorldDinoSurvival.Inventory;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    /// <summary>
    /// Cria pickups de loot no mundo (rede ou local).
    /// </summary>
    public static class LootSpawner
    {
        private static GameObject _lootPrefab;

        public static void SpawnItem(int itemId, int amount, Vector3 origin, Color color)
        {
            if (amount <= 0)
            {
                return;
            }

            bool isNetworked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
            if (isNetworked && !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            Vector3 offset = new Vector3(Random.Range(-0.8f, 0.8f), 0.2f, Random.Range(-0.8f, 0.8f));
            Vector3 position = origin + offset;

            GameObject pickupObject = GetLootPrefab() != null
                ? Object.Instantiate(GetLootPrefab(), position, Quaternion.identity)
                : CreateFallbackPickup(position);

            LootPickup pickup = pickupObject.GetComponent<LootPickup>();
            pickup.Configure(itemId, amount, color);

            if (isNetworked)
            {
                NetworkObject networkObject = pickupObject.GetComponent<NetworkObject>();
                networkObject?.Spawn();
            }
        }

        private static GameObject GetLootPrefab()
        {
            if (_lootPrefab == null)
            {
                _lootPrefab = Resources.Load<GameObject>("Prefabs/LootPickup");
            }

            return _lootPrefab;
        }

        private static GameObject CreateFallbackPickup(Vector3 position)
        {
            GameObject pickupObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickupObject.transform.position = position;
            pickupObject.transform.localScale = Vector3.one * 0.45f;
            pickupObject.GetComponent<SphereCollider>().isTrigger = true;
            pickupObject.AddComponent<LootPickup>();
            pickupObject.AddComponent<NetworkObject>();
            return pickupObject;
        }
    }
}
