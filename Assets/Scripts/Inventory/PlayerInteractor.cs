using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Systems;
using OpenWorldDinoSurvival.World;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    public class PlayerInteractor : MonoBehaviour
    {
        [SerializeField] private float interactRadius = 2.2f;
        [SerializeField] private PlayerInventory localInventory;
        [SerializeField] private NetworkPlayerInventory networkInventory;
        [SerializeField] private NetworkPlayerSurvival survival;

        private LootPickup _nearbyLoot;
        private ResourceNode _nearbyNode;
        private WaterSource _nearbyWater;

        public LootPickup NearbyLoot => _nearbyLoot;
        public ResourceNode NearbyNode => _nearbyNode;
        public WaterSource NearbyWater => _nearbyWater;
        public bool HasInteractable => _nearbyLoot != null
            || (_nearbyNode != null && _nearbyNode.CanHarvest)
            || (_nearbyWater != null && _nearbyWater.CanDrink);

        private void Awake()
        {
            localInventory ??= GetComponent<PlayerInventory>();
            networkInventory ??= GetComponent<NetworkPlayerInventory>();
            survival ??= GetComponent<NetworkPlayerSurvival>();
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

            if (_nearbyWater != null)
            {
                TryDrinkWater(_nearbyWater);
                return;
            }

            if (_nearbyNode != null && _nearbyNode.CanHarvest)
            {
                TryHarvestNode(_nearbyNode);
            }
        }

        public string GetInteractHint()
        {
            if (_nearbyLoot != null)
            {
                return $"[E] Coletar {ItemIds.GetDisplayName(_nearbyLoot.ItemId)}";
            }

            if (_nearbyWater != null)
            {
                return _nearbyWater.CanDrink ? "[E] Beber água" : "[E] Água (aguarde)";
            }

            if (_nearbyNode != null && _nearbyNode.CanHarvest)
            {
                return $"[E] Coletar {ItemIds.GetDisplayName(_nearbyNode.ItemId)}";
            }

            return string.Empty;
        }

        private void RefreshNearbyTargets()
        {
            _nearbyLoot = null;
            _nearbyNode = null;
            _nearbyWater = null;
            float bestDistance = interactRadius;

            LootPickup[] loots = FindObjectsByType<LootPickup>(FindObjectsSortMode.None);
            foreach (LootPickup loot in loots)
            {
                float distance = Vector3.Distance(transform.position, loot.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    _nearbyLoot = loot;
                }
            }

            if (_nearbyLoot != null)
            {
                return;
            }

            bestDistance = interactRadius;
            WaterSource[] waterSources = FindObjectsByType<WaterSource>(FindObjectsSortMode.None);
            foreach (WaterSource water in waterSources)
            {
                float distance = Vector3.Distance(transform.position, water.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    _nearbyWater = water;
                }
            }

            if (_nearbyWater != null)
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

        private void TryDrinkWater(WaterSource waterSource)
        {
            if (survival == null)
            {
                return;
            }

            if (networkInventory != null && NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                if (networkInventory.IsOwner)
                {
                    survival.RequestDrinkWaterServerRpc();
                }

                return;
            }

            waterSource.TryDrink(survival);
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
