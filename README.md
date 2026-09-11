# Open World Dino Survival

Jogo 3D multiplayer de sobrevivência em mundo aberto com dinossauros, PvP e cooperação.

**Stack:** Unity 2022.3 LTS · C# · Input System · Netcode for GameObjects

## Milestone atual: 6 — Mundo aberto 2×2 km

- [x] Milestones 1–5
- [x] Mapa **2 km × 2 km** com 7 regiões exploráveis
- [x] Estruturas por região (vilarejo, porto, laboratório, etc.)
- [x] Loot e recursos espalhados pelo mapa
- [x] 5 Velociraptors em regiões diferentes
- [x] HUD de região atual

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
5. Pressione **Play** → escolha **Host** ou **Client**

### Crafting — materiais, armas e armaduras

**Materiais:**
- **Sucata de metal** — alvos vermelhos, nós de sucata no mapa
- **Couro de dino** — mate o Velociraptor
- **Fibra** — colete dos arbustos verdes
- **Peças de arma** — alvos e raptor (drop parcial)

**Receitas (Tab → clique):**
| Item | Materiais |
|------|-----------|
| Pistola Reforçada | 5 sucata + 2 peças |
| Rifle de Caça | 8 sucata + 3 couro + 3 peças |
| Colete de Couro | 4 couro + 2 fibra |
| Armadura de Placas | 10 sucata + 5 couro |

### Chat e chamadas de voz

**Chat global:** digite e pressione Enter (ou clique Enviar). Todos os jogadores veem.

**Chat privado:** `/w 1 oi` — envia só para o Jogador com ID 1. Use `/players` para ver IDs.

**Chamada de voz:**
1. Defina o ID do alvo no painel (canto inferior direito)
2. **C** para ligar — o outro jogador aceita com **C**
3. Segure **V** para falar (push-to-talk)
4. **C** novamente para encerrar

> A voz usa microfone do PC e transmissão em tempo real entre os dois jogadores em chamada. Qualidade é de protótipo; para produção considere Vivox.

### Explorar o mundo aberto (Milestone 6)

O mapa tem **2 km × 2 km** centrado na origem. Regiões:

| Região | Localização aproximada |
|--------|------------------------|
| Vilarejo | Centro (casas, poço, celeiro) |
| Rio | Faixa central (Z ≈ 0) |
| Floresta | Oeste (árvores, acampamento) |
| Montanha | Norte (rochas) |
| Praia | Sul (acampamentos) |
| Porto | Sudeste (doca, armazém) |
| Laboratório | Nordeste (prédio, antena) |

O HUD superior esquerdo mostra sua **região atual** conforme você explora.

### Testar multiplayer (Milestone 5 — até 4 jogadores)

1. **Build** ou abra até 4 instâncias (Editor + Builds, ou ParrelSync)
2. Instância 1: **Host (criar sessão)**
3. Instâncias 2–4: **Entrar (conectar)** em `127.0.0.1:7777`
4. Cada jogador spawna no **Vilarejo** (norte do rio):
   - Jogador 0 → `(-40, 1, 120)`
   - Jogador 1 → `(40, 1, 120)`
   - Jogador 2 → `(-40, 1, 180)`
   - Jogador 3 → `(40, 1, 180)`
5. HUD no canto superior direito mostra jogadores conectados
6. Atire em outro jogador — dano calculado no **servidor**
7. **Morte:** inventário (materiais) cai no chão; upgrades craftados são perdidos; respawn em 3 s
8. Um 5º jogador é **rejeitado** automaticamente (sessão cheia)

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
| Coletar loot / recurso | E |
| Menu de crafting | Tab |
| Chat | T (focar) + Enter (enviar) |
| Mensagem privada | `/w <id> <texto>` |
| Ligar / desligar chamada | C |
| Falar na chamada (PTT) | Segurar V |
| Asas (voo/planar) | F |
| Soco leve | Q |
| Golpe pesado | Botão direito do mouse (ou Shift+Q) |
| Esquiva | X |
| Dominar / soltar dinossauro | G |

### Asas, luta e dominação de dinossauros

**Asas (F):** abre asas para planar e voar. Segure **Espaço** no ar para subir. O combustível drena enquanto você voa e recarrega no chão.

**Luta corpo-a-corpo:**
- **Q** — soco leve (18 de dano)
- **Botão direito** ou **Shift+Q** — golpe pesado (40 de dano)
- **X** — esquiva com invulnerabilidade breve

**Dominação (G):** com um Velociraptor abaixo de **50% de vida** e a até **5 m**, pressione **G** para controlá-lo. Use **WASD** para mover, **Shift** para correr e **clique esquerdo** para atacar. Pressione **G** novamente para soltar.

## Testar Velociraptor (Milestone 3)

1. Play na cena `MainScene`
2. O **Velociraptor** spawna perto de `(12, 0, 18)` — capsule marrom
3. Aproxime-se: ele **patrulha**, **te vê** e **persegue**
4. Atire nele (150 HP — ~6 tiros de pistola) ou deixe chegar perto (20 de dano/ataque)
5. Se morrer, **respawn em 3 segundos** no ponto inicial
6. Tiros próximos **alertam** o raptor mesmo fora do campo de visão

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
| 4 | PvP (2 jogadores, dano server-side) ✅ |
| 5 | Cooperação (grupo de 4) ✅ |
| 6 | Mundo aberto (regiões, loot) ✅ |
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
