# Integração — Milestones 1–3 → mundo-dino

Este documento explica como levar o código Unity deste agente para o repo privado **mundo-dino**.

## Quem tem o quê

| Agente / local | Conteúdo |
|----------------|----------|
| **Agente com `open-world-dino-survival/`** | Código Unity completo (M1–M3) ✅ |
| **Repo `mundo-dino` (privado)** | Só README inicial ❌ |

Os agentes **não compartilham arquivos automaticamente**. O repo privado bloqueia push/clone deste ambiente.

---

## Opção 1 — No agente `mundo-dino` (cole este prompt)

```
Integre o projeto Unity dos Milestones 1–3. Estrutura esperada:

- Assets/Scripts/Player/ (movimento, câmera, armas, vida)
- Assets/Scripts/Weapons/ (WeaponStats, Weapon)
- Assets/Scripts/AI/VelociraptorAI.cs
- Assets/Scripts/Dinosaurs/DinosaurStats.cs
- Assets/Scripts/Systems/Health.cs
- Assets/Scripts/UI/WeaponHUD.cs
- Assets/Scripts/World/ (TrainingRangeSetup, DinosaurSpawner)
- Assets/Scenes/MainScene.unity
- Assets/Resources/Weapons/ e Dinosaurs/
- Packages/, ProjectSettings/, .gitignore Unity

Recrie os arquivos seguindo a skill Open World Dino Survival Milestones 1–3.
Remova o app web Node/React da branch cursor/setup-mundo-dino-env se conflitar.
Commit: feat: Milestones 1-3 — Unity dino survival
```

---

## Opção 2 — Push manual no PC

```bash
git clone https://github.com/Agsterr/mundo-dino.git
cd mundo-dino

# Copie TUDO de open-world-dino-survival/ para cá, EXCETO a pasta .git
# (Assets, Packages, ProjectSettings, scripts, README, etc.)

git add .
git commit -m "feat: Milestones 1-3 — personagem, armas, Velociraptor com IA"
git push
```

---

## Opção 3 — Git bundle (se tiver o arquivo `.bundle`)

```bash
git clone mundo-dino-milestones-1-3.bundle mundo-dino-temp
cd mundo-dino-temp
git remote set-url origin https://github.com/Agsterr/mundo-dino.git
git push -u origin main --force   # cuidado: sobrescreve main se necessário
```

---

## Opção 4 — Liberar Cursor no GitHub

1. GitHub → **Settings** → **Applications** → **Cursor**
2. **Repository access** → selecione **mundo-dino**
3. No agente com o código, avise **"liberei"** para tentar push automático

---

## O que está incluído (M1–M3)

### Milestone 1
- Movimentação WASD, sprint, pulo
- Câmera terceira pessoa
- Mapa plano 200×200 m

### Milestone 2
- Pistola (1) e rifle (2)
- Tiro hitscan, munição, recarga (R)
- Alvos de treino + HUD

### Milestone 3
- Velociraptor (IA: Idle→Patrol→Chase→Attack→Search→Return)
- Visão + alerta por tiros
- Vida do jogador + respawn

---

## Validar após integrar

```bash
npm install
npm run validate
```

Abrir no **Unity 2022.3 LTS** → `Assets/Scenes/MainScene.unity` → Play.
