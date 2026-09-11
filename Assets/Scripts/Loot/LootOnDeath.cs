using OpenWorldDinoSurvival.Inventory;
using OpenWorldDinoSurvival.Systems;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    public class LootOnDeath : MonoBehaviour
    {
        [SerializeField] private LootDropEntry[] drops;

        public void SetDrops(LootDropEntry[] newDrops)
        {
            drops = newDrops;
        }

        private Health _health;
        private GameObject _lootPrefab;

        private void Awake()
        {
            _health = GetComponent<Health>();
            _lootPrefab = Resources.Load<GameObject>("Prefabs/LootPickup");
            if (_health != null)
            {
                _health.OnDied += HandleDied;
            }
        }

        private void OnDestroy()
        {
            if (_health != null)
            {
                _health.OnDied -= HandleDied;
            }
        }

        private void HandleDied()
        {
            if (drops == null)
            {
                return;
            }

            bool isNetworked = NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;
            if (isNetworked && !NetworkManager.Singleton.IsServer)
            {
                return;
            }

            Vector3 origin = transform.position + Vector3.up * 0.5f;
            foreach (LootDropEntry drop in drops)
            {
                if (drop.amount <= 0)
                {
                    continue;
                }

                if (drop.chance < 1f && Random.value > drop.chance)
                {
                    continue;
                }

                SpawnPickup(drop, origin, isNetworked);
            }
        }

        private void SpawnPickup(LootDropEntry drop, Vector3 origin, bool isNetworked)
        {
            Vector3 offset = new Vector3(Random.Range(-0.8f, 0.8f), 0.2f, Random.Range(-0.8f, 0.8f));
            Vector3 position = origin + offset;

            GameObject pickupObject = _lootPrefab != null
                ? Instantiate(_lootPrefab, position, Quaternion.identity)
                : CreateFallbackPickup(position);

            LootPickup pickup = pickupObject.GetComponent<LootPickup>();
            pickup.Configure(drop.itemId, drop.amount, drop.color);

            if (isNetworked)
            {
                NetworkObject networkObject = pickupObject.GetComponent<NetworkObject>();
                networkObject?.Spawn();
            }
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

    [System.Serializable]
    public struct LootDropEntry
    {
        public int itemId;
        public int amount;
        [Range(0f, 1f)] public float chance;
        public Color color;
    }
}
