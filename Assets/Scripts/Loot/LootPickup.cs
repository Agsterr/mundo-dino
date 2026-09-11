using OpenWorldDinoSurvival.Inventory;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Loot
{
    [RequireComponent(typeof(NetworkObject))]
    public class LootPickup : NetworkBehaviour
    {
        [SerializeField] private int itemId = ItemIds.ScrapMetal;
        [SerializeField] private int amount = 1;
        [SerializeField] private Color pickupColor = new Color(0.9f, 0.75f, 0.2f);

        private Renderer _renderer;

        public int ItemId => itemId;
        public int Amount => amount;

        public void Configure(int newItemId, int newAmount, Color color)
        {
            itemId = newItemId;
            amount = newAmount;
            pickupColor = color;
            ApplyVisual();
        }

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
            ApplyVisual();
        }

        private void ApplyVisual()
        {
            if (_renderer != null)
            {
                _renderer.material.color = pickupColor;
            }
        }

        public bool TryCollect(NetworkPlayerInventory inventory)
        {
            if (!IsServer || inventory == null)
            {
                return false;
            }

            inventory.AddItem(itemId, amount);
            NetworkObject.Despawn();
            return true;
        }

        public bool TryCollectLocal(PlayerInventory inventory)
        {
            if (inventory == null)
            {
                return false;
            }

            inventory.AddItem(itemId, amount);
            Destroy(gameObject);
            return true;
        }
    }
}
