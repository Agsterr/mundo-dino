# AGENTS.md — Open World Dino Survival

Instruções para assistentes de IA que trabalham neste repositório.

## Princípio central

Desenvolver em **pequenas entregas jogáveis**. Nunca implementar sistemas futuros antes dos atuais funcionarem.

## Milestone ativa

**Milestone 5** — Sessões de até 4 jogadores, loot na morte, spawn por jogador.

Próximo: Milestone 6 (mundo aberto 2×2 km, regiões, estruturas).

## Regras de código

1. Não inventar classes, managers ou APIs que não existem — verificar o código antes de referenciar
2. Não criar abstrações só por estética
3. Alterar o mínimo necessário por tarefa
4. Multiplayer: servidor é autoridade para estado de gameplay (a partir do Milestone 4)
5. Cliente controla apenas: input, câmera, UI e efeitos locais

## Estrutura de scripts

```
Assets/Scripts/
├── Player/       PlayerMovement, ThirdPersonCamera, PlayerWeaponController, PlayerWings, PlayerMeleeCombat, PlayerDinosaurDomination
├── Weapons/      WeaponStats, Weapon
├── Dinosaurs/    DinosaurStats, DinosaurMountable, DinosaurPlayerControl
├── AI/           VelociraptorAI (Idle→Patrol→Chase→Attack→Search→Return)
├── Systems/      Health
├── UI/           WeaponHUD, CraftingHUD, AbilitiesHUD, SessionHUD
├── World/        TrainingRangeSetup, DinosaurSpawner
├── Multiplayer/  NetworkPlayerSetup, NetworkPlayerHealth, NetworkPlayerCombat, ConnectionUI, PlayerSpawnPoints, NetworkSessionConfig
├── Inventory/    ItemIds, PlayerInventory, NetworkPlayerInventory, PlayerCrafting, PlayerArmor, CraftingRecipe
└── Loot/         LootPickup, LootOnDeath, ResourceNode, ResourceNodeSpawner
```

## Como testar (Milestone 5)

1. Unity 2022.3 LTS → `Assets/Scenes/MainScene.unity` → Play
2. Até 4 instâncias: **Host** na primeira, **Entrar** nas demais
3. HUD superior direito lista jogadores conectados (0–3)
4. Morte derruba materiais do inventário no chão
5. 5º jogador é rejeitado (sessão cheia)

> PvP usa Netcode for GameObjects. Cliente nunca aplica dano diretamente.

## Unity

- Versão: **2022.3.52f1** (LTS)
- Input: **New Input System** (`com.unity.inputsystem`)
- Plataforma inicial: PC

## Cursor Cloud Agent

Este repositório não inclui o Unity Editor no ambiente cloud. Testes end-to-end exigem Unity local. Use `npm run validate` para checar estrutura de arquivos.
