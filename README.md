# Open World Dino Survival

Jogo 3D multiplayer de sobrevivência em mundo aberto com dinossauros, PvP e cooperação.

**Stack:** Unity 2022.3 LTS · C# · Input System · (futuro) Netcode for GameObjects

## Milestone atual: 1 — Personagem no mapa

- [x] Projeto Unity configurado
- [x] Personagem com movimentação (WASD, sprint, pulo)
- [x] Câmera em terceira pessoa
- [x] Mapa simples (plano 200×200 m)

## Pré-requisitos

- [Unity Hub](https://unity.com/download) com **Unity 2022.3 LTS**
- Git

## Como abrir o projeto

```bash
git clone https://github.com/Agsterr/open-world-dino-survival.git
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

## Estrutura do projeto

```
Assets/
├── Scripts/
│   ├── Player/       ← Milestone 1
│   ├── Weapons/      ← Milestone 2
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

O projeto já está com `git init` e commits prontos. Crie o repositório no GitHub e envie:

```bash
# Opção A — GitHub CLI (com sua conta autenticada)
cd open-world-dino-survival
./scripts/setup-github-repo.sh

# Opção B — manual
# 1. Crie um repo vazio em github.com/new → nome: open-world-dino-survival
# 2. Execute:
git remote add origin https://github.com/SEU_USUARIO/open-world-dino-survival.git
git push -u origin main
```
