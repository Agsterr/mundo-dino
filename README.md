# mundo-dino 🦕

Aplicação full-stack de exemplo para explorar e cadastrar dinossauros do "Mundo Dino".

## Arquitetura

Monorepo com [npm workspaces](https://docs.npmjs.com/cli/using-npm/workspaces):

- `server/` — API REST em Node.js + Express + TypeScript (porta `4000`).
- `client/` — SPA em React + Vite + TypeScript (porta `5173`).

O cliente usa um proxy do Vite para encaminhar `/api` ao servidor.

## Pré-requisitos

- Node.js `>= 20` (testado com Node 22)
- npm `>= 10`

## Instalação

```bash
npm install
```

## Desenvolvimento

Em dois terminais separados:

```bash
npm run dev:server   # inicia a API em http://localhost:4000
npm run dev:client   # inicia o front em http://localhost:5173
```

Acesse http://localhost:5173.

## Comandos úteis

| Comando | Descrição |
| --- | --- |
| `npm run dev:server` | API em modo watch |
| `npm run dev:client` | Front-end em modo dev |
| `npm run build` | Build de produção (server + client) |
| `npm run typecheck` | Type-check de ambos os pacotes |
| `npm test` | Testes do servidor |

## API

| Método | Rota | Descrição |
| --- | --- | --- |
| `GET` | `/api/health` | Health check |
| `GET` | `/api/dinos` | Lista os dinossauros |
| `GET` | `/api/dinos/:id` | Detalhe de um dinossauro |
| `POST` | `/api/dinos` | Cria um novo dinossauro |

Exemplo:

```bash
curl -X POST http://localhost:4000/api/dinos \
  -H 'Content-Type: application/json' \
  -d '{"name":"Denver","species":"Corythosaurus","period":"Cretaceo","diet":"herbivoro","lengthMeters":9}'
```
