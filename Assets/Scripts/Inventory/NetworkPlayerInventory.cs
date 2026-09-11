using System;
using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
using OpenWorldDinoSurvival.Systems;
using OpenWorldDinoSurvival.Weapons;
using Unity.Netcode;
using UnityEngine;

namespace OpenWorldDinoSurvival.Inventory
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkPlayerInventory : NetworkBehaviour
    {
        [SerializeField] private PlayerArmor armor;
        [SerializeField] private PlayerWeaponController weaponController;
        [SerializeField] private NetworkPlayerCombat combat;

        private readonly NetworkVariable<int> _scrap = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _hide = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _fiber = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _gunParts = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _food = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _water = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<bool> _pistolUpgraded = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<bool> _rifleUpgraded = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _armorTier = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private ArmorStats _hideVest;
        private ArmorStats _metalArmor;
        private WeaponStats _craftedPistol;
        private WeaponStats _craftedRifle;
        private NetworkPlayerSurvival _survival;

        public event Action OnChanged;

        public int GetCount(int itemId)
        {
            return itemId switch
            {
                ItemIds.ScrapMetal => _scrap.Value,
                ItemIds.DinoHide => _hide.Value,
                ItemIds.Fiber => _fiber.Value,
                ItemIds.GunParts => _gunParts.Value,
                ItemIds.FoodRation => _food.Value,
                ItemIds.WaterFlask => _water.Value,
                _ => 0
            };
        }

        public bool PistolUpgraded => _pistolUpgraded.Value;
        public bool RifleUpgraded => _rifleUpgraded.Value;
        public int ArmorTier => _armorTier.Value;
        public float DamageReduction => GetEquippedArmor()?.damageReduction ?? 0f;
        public float HealthBonus => GetEquippedArmor()?.healthBonus ?? 0f;

        private void Awake()
        {
            armor ??= GetComponent<PlayerArmor>();
            weaponController ??= GetComponent<PlayerWeaponController>();
            combat ??= GetComponent<NetworkPlayerCombat>();
            _survival ??= GetComponent<NetworkPlayerSurvival>();
            _hideVest = Resources.Load<ArmorStats>("Armor/HideVest");
            _metalArmor = Resources.Load<ArmorStats>("Armor/MetalPlateArmor");
            _craftedPistol = Resources.Load<WeaponStats>("Weapons/CraftedPistolStats");
            _craftedRifle = Resources.Load<WeaponStats>("Weapons/CraftedRifleStats");
        }

        public override void OnNetworkSpawn()
        {
            _scrap.OnValueChanged += NotifyChanged;
            _hide.OnValueChanged += NotifyChanged;
            _fiber.OnValueChanged += NotifyChanged;
            _gunParts.OnValueChanged += NotifyChanged;
            _food.OnValueChanged += NotifyChanged;
            _water.OnValueChanged += NotifyChanged;
            _pistolUpgraded.OnValueChanged += (_, __) => ApplyWeaponUpgrades();
            _rifleUpgraded.OnValueChanged += (_, __) => ApplyWeaponUpgrades();
            _armorTier.OnValueChanged += (_, __) => ApplyArmorVisual();

            ApplyWeaponUpgrades();
            ApplyArmorVisual();
        }

        public override void OnNetworkDespawn()
        {
            _scrap.OnValueChanged -= NotifyChanged;
            _hide.OnValueChanged -= NotifyChanged;
            _fiber.OnValueChanged -= NotifyChanged;
            _gunParts.OnValueChanged -= NotifyChanged;
            _food.OnValueChanged -= NotifyChanged;
            _water.OnValueChanged -= NotifyChanged;
        }

        public void AddItem(int itemId, int amount)
        {
            if (!IsServer || amount <= 0)
            {
                return;
            }

            switch (itemId)
            {
                case ItemIds.ScrapMetal: _scrap.Value += amount; break;
                case ItemIds.DinoHide: _hide.Value += amount; break;
                case ItemIds.Fiber: _fiber.Value += amount; break;
                case ItemIds.GunParts: _gunParts.Value += amount; break;
                case ItemIds.FoodRation: _food.Value += amount; break;
                case ItemIds.WaterFlask: _water.Value += amount; break;
            }
        }

        [ServerRpc]
        public void RequestConsumeItemServerRpc(int itemId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId || _survival == null)
            {
                return;
            }

            if (GetCount(itemId) <= 0)
            {
                return;
            }

            RemoveItemServer(itemId, 1);

            if (itemId == ItemIds.FoodRation)
            {
                _survival.RestoreHunger(ItemIds.FoodRestoreAmount);
            }
            else if (itemId == ItemIds.WaterFlask)
            {
                _survival.RestoreThirst(ItemIds.WaterRestoreAmount);
            }
        }

        public void TryConsumeItem(int itemId)
        {
            if (!IsOwner)
            {
                return;
            }

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                RequestConsumeItemServerRpc(itemId);
                return;
            }

            if (GetCount(itemId) <= 0 || _survival == null)
            {
                return;
            }

            RemoveItemServer(itemId, 1);
            if (itemId == ItemIds.FoodRation)
            {
                _survival.RestoreHunger(ItemIds.FoodRestoreAmount);
            }
            else if (itemId == ItemIds.WaterFlask)
            {
                _survival.RestoreThirst(ItemIds.WaterRestoreAmount);
            }
        }

        public bool CanCraft(CraftingRecipe recipe)
        {
            if (recipe == null)
            {
                return false;
            }

            if (recipe.resultType == CraftResultType.WeaponUpgrade)
            {
                if (recipe.weaponSlot == 0 && _pistolUpgraded.Value)
                {
                    return false;
                }

                if (recipe.weaponSlot == 1 && _rifleUpgraded.Value)
                {
                    return false;
                }
            }

            if (recipe.resultType == CraftResultType.Armor)
            {
                int targetTier = recipe.recipeId == "armor_hide" ? 1 : 2;
                if (_armorTier.Value >= targetTier)
                {
                    return false;
                }
            }

            foreach (CraftIngredient ingredient in recipe.ingredients)
            {
                if (GetCount(ingredient.itemId) < ingredient.amount)
                {
                    return false;
                }
            }

            return true;
        }

        [ServerRpc]
        public void RequestHarvestServerRpc(int itemId, int amount, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            ResourceNode closest = FindClosestHarvestableNode(itemId);
            if (closest == null || !closest.TryHarvest())
            {
                return;
            }

            AddItem(itemId, amount);
        }

        [ServerRpc]
        public void RequestCollectLootServerRpc(ulong lootNetworkId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(lootNetworkId, out NetworkObject netObj))
            {
                return;
            }

            LootPickup loot = netObj.GetComponent<LootPickup>();
            if (loot == null)
            {
                return;
            }

            if (Vector3.Distance(transform.position, loot.transform.position) > 2.5f)
            {
                return;
            }

            loot.TryCollect(this);
        }

        [ServerRpc]
        public void RequestCraftServerRpc(string recipeId, ServerRpcParams rpcParams = default)
        {
            if (rpcParams.Receive.SenderClientId != OwnerClientId)
            {
                return;
            }

            CraftingRecipe recipe = CraftingDatabase.Instance.FindById(recipeId);
            if (!CanCraft(recipe))
            {
                return;
            }

            foreach (CraftIngredient ingredient in recipe.ingredients)
            {
                RemoveItemServer(ingredient.itemId, ingredient.amount);
            }

            if (recipe.resultType == CraftResultType.WeaponUpgrade)
            {
                if (recipe.weaponSlot == 0)
                {
                    _pistolUpgraded.Value = true;
                }
                else
                {
                    _rifleUpgraded.Value = true;
                }
            }
            else if (recipe.resultType == CraftResultType.Armor)
            {
                _armorTier.Value = recipe.recipeId == "armor_hide" ? 1 : 2;
            }
            else if (recipe.resultType == CraftResultType.Consumable)
            {
                AddItem(recipe.consumableItemId, recipe.consumableAmount);
            }
        }

        private void RemoveItemServer(int itemId, int amount)
        {
            switch (itemId)
            {
                case ItemIds.ScrapMetal: _scrap.Value = Mathf.Max(0, _scrap.Value - amount); break;
                case ItemIds.DinoHide: _hide.Value = Mathf.Max(0, _hide.Value - amount); break;
                case ItemIds.Fiber: _fiber.Value = Mathf.Max(0, _fiber.Value - amount); break;
                case ItemIds.GunParts: _gunParts.Value = Mathf.Max(0, _gunParts.Value - amount); break;
                case ItemIds.FoodRation: _food.Value = Mathf.Max(0, _food.Value - amount); break;
                case ItemIds.WaterFlask: _water.Value = Mathf.Max(0, _water.Value - amount); break;
            }
        }

        private ArmorStats GetEquippedArmor()
        {
            return _armorTier.Value switch
            {
                1 => _hideVest,
                2 => _metalArmor,
                _ => null
            };
        }

        private void ApplyArmorVisual()
        {
            if (armor == null)
            {
                return;
            }

            armor.Equip(GetEquippedArmor());
        }

        private void ApplyWeaponUpgrades()
        {
            if (weaponController == null)
            {
                return;
            }

            if (_pistolUpgraded.Value && _craftedPistol != null)
            {
                weaponController.ApplyWeaponUpgrade(0, _craftedPistol);
            }

            if (_rifleUpgraded.Value && _craftedRifle != null)
            {
                weaponController.ApplyWeaponUpgrade(1, _craftedRifle);
            }

            combat?.RefreshWeaponStats();
        }

        private ResourceNode FindClosestHarvestableNode(int itemId)
        {
            ResourceNode closest = null;
            float bestDistance = 2.5f;
            ResourceNode[] nodes = FindObjectsByType<ResourceNode>(FindObjectsSortMode.None);

            foreach (ResourceNode node in nodes)
            {
                if (!node.CanHarvest || node.ItemId != itemId)
                {
                    continue;
                }

                float distance = Vector3.Distance(transform.position, node.transform.position);
                if (distance <= bestDistance)
                {
                    bestDistance = distance;
                    closest = node;
                }
            }

            return closest;
        }

        public void DropAllOnDeathServer(Vector3 origin)
        {
            if (!IsServer)
            {
                return;
            }

            Vector3 dropOrigin = origin + Vector3.up * 0.5f;
            DropStack(dropOrigin, ItemIds.ScrapMetal, _scrap.Value);
            DropStack(dropOrigin, ItemIds.DinoHide, _hide.Value);
            DropStack(dropOrigin, ItemIds.Fiber, _fiber.Value);
            DropStack(dropOrigin, ItemIds.GunParts, _gunParts.Value);
            DropStack(dropOrigin, ItemIds.FoodRation, _food.Value);
            DropStack(dropOrigin, ItemIds.WaterFlask, _water.Value);

            _scrap.Value = 0;
            _hide.Value = 0;
            _fiber.Value = 0;
            _gunParts.Value = 0;
            _food.Value = 0;
            _water.Value = 0;
            _pistolUpgraded.Value = false;
            _rifleUpgraded.Value = false;
            _armorTier.Value = 0;

            ApplyWeaponUpgrades();
            ApplyArmorVisual();
        }

        private static void DropStack(Vector3 origin, int itemId, int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            LootSpawner.SpawnItem(itemId, amount, origin, ItemIds.GetLootColor(itemId));
        }

        private void NotifyChanged(int _, int __) => OnChanged?.Invoke();
    }
}
