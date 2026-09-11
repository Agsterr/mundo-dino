# AGENTS.md — Open World Dino Survival

Instruções para assistentes de IA que trabalham neste repositório.

## Princípio central

Desenvolver em **pequenas entregas jogáveis**. Nunca implementar sistemas futuros antes dos atuais funcionarem.

## Milestone ativa

**Milestone 4 + Crafting** — Multiplayer PvP + coleta/crafting de armas e armaduras.

Próximo: Milestone 5 (grupos de até 4 jogadores).

## Regras de código

1. Não inventar classes, managers ou APIs que não existem — verificar o código antes de referenciar
2. Não criar abstrações só por estética
3. Alterar o mínimo necessário por tarefa
4. Multiplayer: servidor é autoridade para estado de gameplay (a partir do Milestone 4)
5. Cliente controla apenas: input, câmera, UI e efeitos locais

## Estrutura de scripts

```
Assets/Scripts/
├── Player/       PlayerMovement, ThirdPersonCamera, PlayerWeaponController, PlayerHealth
├── Weapons/      WeaponStats, Weapon
├── Dinosaurs/    DinosaurStats
├── AI/           VelociraptorAI (Idle→Patrol→Chase→Attack→Search→Return)
├── Systems/      Health
├── UI/           WeaponHUD (munição, vida, mira)
├── World/        TrainingRangeSetup, DinosaurSpawner
├── Multiplayer/  NetworkPlayerSetup, NetworkPlayerHealth, NetworkPlayerCombat, ConnectionUI
├── Inventory/    ItemIds, PlayerInventory, NetworkPlayerInventory, PlayerCrafting, PlayerArmor, CraftingRecipe
└── Loot/         LootPickup, LootOnDeath, ResourceNode, ResourceNodeSpawner
```

## Como testar (Milestone 4)

1. Unity 2022.3 LTS → `Assets/Scenes/MainScene.unity` → Play
2. Clique **Host** na primeira instância, **Client** na segunda (build ou ParrelSync)
3. Atire no outro jogador — dano é calculado no **servidor**
4. Velociraptor (Host) continua com IA server-side
5. HUD mostra munição + vida do jogador local

> PvP usa Netcode for GameObjects. Cliente nunca aplica dano diretamente.

## Unity

- Versão: **2022.3.52f1** (LTS)
- Input: **New Input System** (`com.unity.inputsystem`)
- Plataforma inicial: PC

## Cursor Cloud Agent

Este repositório não inclui o Unity Editor no ambiente cloud. Testes end-to-end exigem Unity local. Use `npm run validate` para checar estrutura de arquivos.
