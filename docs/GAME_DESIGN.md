# Game Design — Open World Dino Survival

Documento de referência para desenvolvimento incremental do jogo.

## Conceito

Mundo aberto perigoso em uma ilha com dinossauros. Jogadores sobrevivem, cooperam ou traem uns aos outros. Cada encontro pode gerar uma história diferente.

## Princípio fundamental

**Não criar o jogo inteiro de uma vez.** Cada milestone produz algo jogável.

## Milestones

| # | Entrega |
|---|---------|
| 1 | Personagem + movimentação + câmera + mapa simples |
| 2 | Arma + mira + tiro + munição + recarga |
| 3 | Velociraptor + IA (Idle→Patrol→Detect→Chase→Attack→Search→Return) |
| 4 | Multiplayer local, 2 jogadores, dano server-side |
| 5 | Grupos de até 4 jogadores ✅ |
| 6 | Mapa 2×2 km, regiões, loot, estruturas ✅ |
| 7 | Inventário, comida, água, crafting simples |
| 8 | Progressão de equipamentos e regiões |
| 9 | Servidor persistente, contas, banco de dados |

## Arquitetura server-authoritative

O cliente **nunca** decide diretamente:

- dano, vida, munição, inventário, loot, XP, morte de outros jogadores

Fluxo de tiro (Milestone 4+):

```
Cliente solicita tiro → Servidor valida → Servidor calcula dano → Clientes recebem resultado
```

## Mapa

- Inicial: **2 km × 2 km** (densidade > tamanho)
- Regiões: Montanha, Floresta, Vilarejo, Rio, Praia, Porto, Laboratório

## Dinossauros

- Milestone 3: apenas **Velociraptor**
- Futuro: T-Rex, Triceratops, grupos de raptors, herbívoros

## PvP e coop

- PvP livre; traição é feature, não bug
- Grupos de até 4 jogadores (Milestone 5)
- Loot na morte: mochila fica no mundo

## Regras para IA assistente

1. Verificar código existente antes de referenciar classes
2. Alterar o mínimo necessário
3. Uma entrega por vez
4. Explicar mudanças arquiteturais importantes
5. Não remover código funcional sem explicar

## Anti-cheat (desde Milestone 4)

Validações iniciais no servidor:

- velocidade, cadência de tiro, munição, inventário, dano, rate limiting
