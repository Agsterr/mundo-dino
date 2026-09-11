using System;
using OpenWorldDinoSurvival.Loot;
using OpenWorldDinoSurvival.Multiplayer;
using OpenWorldDinoSurvival.Player;
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
        private readonly NetworkVariable<bool> _pistolUpgraded = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<bool> _rifleUpgraded = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
        private readonly NetworkVariable<int> _armorTier = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

        private ArmorStats _hideVest;
        private ArmorStats _metalArmor;
        private WeaponStats _craftedPistol;
        private WeaponStats _craftedRifle;

        public event Action OnChanged;

        public int GetCount(int itemId)
        {
            return itemId switch
            {
                ItemIds.ScrapMetal => _scrap.Value,
                ItemIds.DinoHide => _hide.Value,
                ItemIds.Fiber => _fiber.Value,
                ItemIds.GunParts => _gunParts.Value,
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
        }

        private void RemoveItemServer(int itemId, int amount)
        {
            switch (itemId)
            {
                case ItemIds.ScrapMetal: _scrap.Value = Mathf.Max(0, _scrap.Value - amount); break;
                case ItemIds.DinoHide: _hide.Value = Mathf.Max(0, _hide.Value - amount); break;
                case ItemIds.Fiber: _fiber.Value = Mathf.Max(0, _fiber.Value - amount); break;
                case ItemIds.GunParts: _gunParts.Value = Mathf.Max(0, _gunParts.Value - amount); break;
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

        private void NotifyChanged(int _, int __) => OnChanged?.Invoke();
    }
}
