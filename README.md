# Open World Dino Survival

Jogo 3D multiplayer de sobrevivência em mundo aberto com dinossauros, PvP e cooperação.

**Stack:** Unity 2022.3 LTS · C# · Input System · (futuro) Netcode for GameObjects

## Milestone atual: 2 — Atirar

- [x] Milestone 1 (movimentação + câmera + mapa)
- [x] Pistola e rifle com stats configuráveis (`WeaponStats`)
- [x] Tiro hitscan com spread, alcance e dano
- [x] Munição, pente e recarga
- [x] Efeito visual de trilha (LineRenderer)
- [x] Alvos de treino + HUD (munição e mira)

## Pré-requisitos

- [Unity Hub](https://unity.com/download) com **Unity 2022.3 LTS**
- Git

## Como abrir o projeto

```bash
git clone https://github.com/Agsterr/mundo-dino.git
```

1. Abra o **Unity Hub**
2. **Add** → selecione a pasta clonada
3. Abra com **Unity 2022.3 LTS**
4. Abra a cena `Assets/Scenes/MainScene.unity`
5. Pressione **Play**

## Controles

| Ação | Tecla |
|------|-------|
| Mover | W A S D |
| Olhar | Mouse |
| Sprint | Left Shift |
| Pular | Espaço |
| Atirar | Botão esquerdo do mouse |
| Recarregar | R |
| Pistola | 1 |
| Rifle | 2 |

## Testar tiros (Milestone 2)

1. Play na cena `MainScene`
2. Ande até os **3 alvos vermelhos** à frente
3. Atire com pistola (1) ou rifle automático (2)
4. Observe HUD no canto superior esquerdo e mira `+` no centro
5. Pressione **R** para recarregar quando o pente esvaziar

## Estrutura do projeto

```
Assets/
├── Scripts/
│   ├── Player/       ← movimentação, câmera, armas
│   ├── Weapons/      ← WeaponStats, Weapon (Milestone 2)
│   ├── Dinosaurs/    ← Milestone 3
│   ├── AI/
│   ├── Multiplayer/
│   ├── Inventory/
│   ├── Loot/
│   ├── World/
│   ├── UI/
│   └── Systems/
├── Prefabs/
├── Scenes/
├── Materials/
├── Models/
├── Animations/
├── Audio/
└── Resources/
```

## Desenvolvimento incremental

O jogo é construído em milestones pequenas e jogáveis. **Não implemente sistemas futuros antes dos atuais funcionarem.**

| # | Objetivo |
|---|----------|
| 1 | Personagem andando no mapa 3D |
| 2 | Atirar (arma, mira, munição, recarga) |
| 3 | Primeiro dinossauro (Velociraptor + IA) |
| 4 | PvP (2 jogadores, dano server-side) |
| 5 | Cooperação (grupo de 4) |
| 6 | Mundo aberto (regiões, loot) |
| 7 | Sobrevivência (inventário, crafting) |
| 8 | Progressão |
| 9 | Servidor persistente |

Detalhes completos em [`docs/GAME_DESIGN.md`](docs/GAME_DESIGN.md).

## Regras de arquitetura

- Código simples, sem abstrações desnecessárias
- Multiplayer server-authoritative desde o Milestone 4
- Cliente **nunca** decide dano, vida, munição ou inventário
- Uma entrega funcional por vez

## Scripts npm (validação local)

```bash
npm install
npm run validate    # verifica estrutura de pastas e arquivos essenciais
npm run tree        # mostra árvore do projeto
```

## Licença

MIT — veja [LICENSE](LICENSE).

## Publicar no GitHub

Repositório: **https://github.com/Agsterr/mundo-dino**

Se você acabou de criar o repo, envie o código local:

```bash
cd open-world-dino-survival   # pasta do projeto Unity
git remote set-url origin https://github.com/Agsterr/mundo-dino.git
git push -u origin main
```

Se o GitHub pedir porque o repo já tem README:

```bash
git pull origin main --allow-unrelated-histories
git push -u origin main
```
