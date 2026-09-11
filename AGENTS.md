# AGENTS.md — Open World Dino Survival

Instruções para assistentes de IA que trabalham neste repositório.

## Princípio central

Desenvolver em **pequenas entregas jogáveis**. Nunca implementar sistemas futuros antes dos atuais funcionarem.

## Milestone ativa

**Milestone 3** — Velociraptor com IA básica.

Próximo: Milestone 4 (multiplayer PvP, dano server-side).

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
├── Multiplayer/  (Milestone 4+)
├── Inventory/    (Milestone 7)
└── Loot/         (Milestone 6)
```

## Como testar (Milestone 3)

1. Unity 2022.3 LTS → `Assets/Scenes/MainScene.unity` → Play
2. Velociraptor spawna em `(12, 1, 18)` — patrulha a área
3. Aproxime ou atire para alertá-lo
4. Mate o raptor (150 HP) ou seja atacado (20 dano, respawn 3s)
5. HUD mostra munição + vida do jogador

> Dano é local até o Milestone 4 (multiplayer server-authoritative).

## Unity

- Versão: **2022.3.52f1** (LTS)
- Input: **New Input System** (`com.unity.inputsystem`)
- Plataforma inicial: PC

## Cursor Cloud Agent

Este repositório não inclui o Unity Editor no ambiente cloud. Testes end-to-end exigem Unity local. Use `npm run validate` para checar estrutura de arquivos.
