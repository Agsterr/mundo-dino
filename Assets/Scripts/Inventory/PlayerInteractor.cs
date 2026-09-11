using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Multiplayer;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRadius = 2.2f;
        [SerializeField] private PlayerInventory localInventory;
        [SerializeField] private NetworkPlayerInventory networkInventory;

        private LootPickup _nearbyLoot;
        private ResourceNode _nearbyNode;

        public LootPickup NearbyLoot => _nearbyLoot;
        public ResourceNode NearbyNode => _nearbyNode;
        public bool HasInteractable => _nearbyLoot != null || (_nearbyNode != null && _nearbyNode.CanHarvest);

        private void Awake()
        {
            localInventory ??= GetComponent<PlayerInventory>();
            networkInventory ??= GetComponent<NetworkPlayerInventory>();
        }

        private void Update()
        {
            RefreshNearbyTargets();
        }

        public void TryInteract()
        {
            if (_nearbyLoot != null)
            {
                TryCollectLoot(_nearbyLoot);
                return;
            }

            if (_nearbyNode != null && _nearbyNode.CanHarvest)
            {
                TryHarvestNode(_nearbyNode);
            }
        }

        private void RefreshNearbyTargets()
        {
            _nearbyLoot = null;
            _nearbyNode = null;
            float bestDistance = interactRadius;

            LootPickup[] loots = FindObjectsByType<LootPickup>(FindObjectsSortMode.None);
            foreach (LootPickup loot in loots)
            {
                float distance = Vector3.Distance(transform.position, loot.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    _nearbyLoot = loot;
                    _nearbyNode = null;
                }
            }

            if (_nearbyLoot != null)
            {
                return;
            }

            bestDistance = interactRadius;
            ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);
            foreach (ResourceNode node in nodes)
            {
                if (!node.CanHarvest)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    _nearbyNode = node;
                }
            }
        }

        private void TryCollectLoot(LootPickup loot)
        {
            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (networkInventory.IsOwner)
                {
                    networkInventory.RequestCollectLootServerRpc(loot.NetworkObjectId);
                }

                return;
            }

            if (localInventory != null)
            {
                loot.TryCollectLocal(localInventory);
            }
        }

        private void TryHarvestNode(ResourceNode node)
        {
            if (!node.TryHarvest())
            {
                return;
            }

            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (networkInventory.IsOwner)
                {
                    networkInventory.RequestHarvestServerRpc(node.ItemId, node.HarvestAmount);
                }

                return;
            }

            localInventory?.AddItem(node.ItemId, node.HarvestAmount);
        }
    }
}
