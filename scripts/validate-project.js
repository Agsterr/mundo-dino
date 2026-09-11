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
  'Assets/Scripts/Player/PlayerWeaponController.cs',
  'Assets/Scripts/Weapons/WeaponStats.cs',
  'Assets/Scripts/Weapons/Weapon.cs',
  'Assets/Scripts/Systems/Health.cs',
  'Assets/Scripts/UI/WeaponHUD.cs',
  'Assets/Scripts/World/TrainingRangeSetup.cs',
  'Assets/Resources/Weapons/PistolStats.asset',
  'Assets/Resources/Weapons/RifleStats.asset',
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
