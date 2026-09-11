# AGENTS.md — Open World Dino Survival

Instruções para assistentes de IA que trabalham neste repositório.

## Princípio central

Desenvolver em **pequenas entregas jogáveis**. Nunca implementar sistemas futuros antes dos atuais funcionarem.

## Milestone ativa

**Milestone 1** — personagem andando em mapa 3D com câmera terceira pessoa.

Próximo: Milestone 2 (armas e tiro).

## Regras de código

1. Não inventar classes, managers ou APIs que não existem — verificar o código antes de referenciar
2. Não criar abstrações só por estética
3. Alterar o mínimo necessário por tarefa
4. Multiplayer: servidor é autoridade para estado de gameplay (a partir do Milestone 4)
5. Cliente controla apenas: input, câmera, UI e efeitos locais

## Estrutura de scripts

```
Assets/Scripts/
├── Player/       PlayerMovement, ThirdPersonCamera, PlayerInputController
├── Weapons/      (Milestone 2)
├── Dinosaurs/    (Milestone 3)
├── AI/           (Milestone 3)
├── Multiplayer/  (Milestone 4+)
├── Inventory/    (Milestone 7)
├── Loot/         (Milestone 6)
├── World/        (Milestone 6)
├── UI/
└── Systems/
```

## Como testar (Milestone 1)

1. Abrir projeto no Unity 2022.3 LTS
2. Cena: `Assets/Scenes/MainScene.unity`
3. Play → WASD move, mouse gira câmera, Shift sprint, Espaço pula

## Unity

- Versão: **2022.3.52f1** (LTS)
- Input: **New Input System** (`com.unity.inputsystem`)
- Plataforma inicial: PC

## Cursor Cloud Agent

Este repositório não inclui o Unity Editor no ambiente cloud. Testes end-to-end exigem Unity local. Use `npm run validate` para checar estrutura de arquivos.
