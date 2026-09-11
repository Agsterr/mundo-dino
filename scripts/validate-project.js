#!/usr/bin/env node
/**
 * Valida estrutura mínima do projeto Unity.
 */
const fs = require('fs');
const path = require('path');

const root = path.join(__dirname, '..');

const required = [
  'Assets/Scenes/MainScene.unity',
  'Assets/Scripts/Player/PlayerMovement.cs',
  'Assets/Scripts/Player/ThirdPersonCamera.cs',
  'Assets/Scripts/Player/PlayerInputController.cs',
  'Assets/Scripts/Weapons/WeaponStats.cs',
  'Assets/Scripts/Weapons/Weapon.cs',
  'Assets/Scripts/Dinosaurs/DinosaurStats.cs',
  'Assets/Scripts/AI/VelociraptorAI.cs',
  'Assets/Scripts/Player/PlayerHealth.cs',
  'Assets/Scripts/Player/PlayerWeaponController.cs',
  'Assets/Scripts/Systems/Health.cs',
  'Assets/Scripts/Systems/NetworkPlayerSurvival.cs',
  'Assets/Scripts/World/WaterSource.cs',
  'Assets/Scripts/UI/SurvivalHUD.cs',
  'Assets/Resources/Crafting/Recipe_FoodRation.asset',
  'Assets/Resources/Crafting/Recipe_WaterFlask.asset',
  'Assets/Scripts/UI/WeaponHUD.cs',
  'Assets/Scripts/World/TrainingRangeSetup.cs',
  'Assets/Scripts/World/DinosaurSpawner.cs',
  'Assets/Scripts/World/WorldRegions.cs',
  'Assets/Scripts/World/OpenWorldGenerator.cs',
  'Assets/Scripts/World/RegionTracker.cs',
  'Assets/Scripts/UI/RegionHUD.cs',
  'Assets/Scripts/Multiplayer/NetworkPlayerSetup.cs',
  'Assets/Scripts/Multiplayer/NetworkPlayerHealth.cs',
  'Assets/Scripts/Multiplayer/NetworkPlayerCombat.cs',
  'Assets/Scripts/Multiplayer/ConnectionUI.cs',
  'Assets/Scripts/Multiplayer/PlayerSpawnPoints.cs',
  'Assets/Scripts/Multiplayer/NetworkSessionConfig.cs',
  'Assets/Scripts/Loot/LootSpawner.cs',
  'Assets/Scripts/UI/SessionHUD.cs',
  'Assets/Scripts/Inventory/ItemIds.cs',
  'Assets/Scripts/Inventory/ArmorStats.cs',
  'Assets/Scripts/Inventory/CraftingRecipe.cs',
  'Assets/Scripts/Inventory/CraftingDatabase.cs',
  'Assets/Scripts/Inventory/PlayerInventory.cs',
  'Assets/Scripts/Inventory/PlayerArmor.cs',
  'Assets/Scripts/Inventory/PlayerCrafting.cs',
  'Assets/Scripts/Inventory/NetworkPlayerInventory.cs',
  'Assets/Scripts/Inventory/PlayerInteractor.cs',
  'Assets/Scripts/Loot/LootPickup.cs',
  'Assets/Scripts/Loot/LootOnDeath.cs',
  'Assets/Scripts/Loot/ResourceNode.cs',
  'Assets/Scripts/Loot/ResourceNodeSpawner.cs',
  'Assets/Scripts/UI/CraftingHUD.cs',
  'Assets/Scripts/Multiplayer/Chat/NetworkPlayerChat.cs',
  'Assets/Scripts/Multiplayer/Chat/ChatHUD.cs',
  'Assets/Scripts/Multiplayer/Voice/NetworkPlayerVoice.cs',
  'Assets/Scripts/Multiplayer/Voice/VoiceNetworkTransmitter.cs',
  'Assets/Scripts/Multiplayer/Voice/VoiceCallHUD.cs',
  'Assets/Scripts/Player/PlayerWings.cs',
  'Assets/Scripts/Player/PlayerMeleeCombat.cs',
  'Assets/Scripts/Player/PlayerDinosaurDomination.cs',
  'Assets/Scripts/Multiplayer/NetworkPlayerAbilities.cs',
  'Assets/Scripts/Dinosaurs/DinosaurMountable.cs',
  'Assets/Scripts/Dinosaurs/DinosaurPlayerControl.cs',
  'Assets/Scripts/UI/AbilitiesHUD.cs',
  'Assets/Prefabs/NetworkPlayer.prefab',
  'Assets/Prefabs/Velociraptor.prefab',
  'Assets/DefaultNetworkPrefabs.asset',
  'Assets/Resources/Prefabs/LootPickup.prefab',
  'Assets/Resources/Weapons/CraftedPistolStats.asset',
  'Assets/Resources/Weapons/CraftedRifleStats.asset',
  'Assets/Resources/Armor/HideVest.asset',
  'Assets/Resources/Armor/MetalPlateArmor.asset',
  'Assets/Resources/Crafting/Recipe_CraftedPistol.asset',
  'Assets/Resources/Crafting/Recipe_CraftedRifle.asset',
  'Assets/Resources/Crafting/Recipe_HideVest.asset',
  'Assets/Resources/Crafting/Recipe_MetalArmor.asset',
  'Assets/Resources/Weapons/PistolStats.asset',
  'Assets/Resources/Weapons/RifleStats.asset',
  'Assets/Resources/Dinosaurs/VelociraptorStats.asset',
  'Assets/Input/PlayerControls.inputactions',
  'Packages/manifest.json',
  'ProjectSettings/ProjectVersion.txt',
  'ProjectSettings/ProjectSettings.asset',
  'README.md',
  'AGENTS.md',
  'docs/GAME_DESIGN.md',
];

const folders = [
  'Assets/Scripts/Weapons',
  'Assets/Scripts/Dinosaurs',
  'Assets/Scripts/AI',
  'Assets/Scripts/Multiplayer',
  'Assets/Scripts/Inventory',
  'Assets/Scripts/Loot',
  'Assets/Scripts/World',
  'Assets/Scripts/UI',
  'Assets/Scripts/Systems',
  'Assets/Prefabs',
  'Assets/Models',
  'Assets/Animations',
  'Assets/Audio',
  'Assets/Resources',
];

let errors = 0;

console.log('Validando Open World Dino Survival...\n');

for (const file of required) {
  const full = path.join(root, file);
  if (fs.existsSync(full)) {
    console.log(`  OK  ${file}`);
  } else {
    console.error(`  FALTA  ${file}`);
    errors++;
  }
}

for (const folder of folders) {
  const full = path.join(root, folder);
  if (fs.existsSync(full)) {
    console.log(`  OK  ${folder}/`);
  } else {
    console.error(`  FALTA  ${folder}/`);
    errors++;
  }
}

console.log('');
if (errors === 0) {
  console.log('Projeto válido.');
  process.exit(0);
} else {
  console.error(`${errors} problema(s) encontrado(s).`);
  process.exit(1);
}
