#!/usr/bin/env bash
# Cria o repositório no GitHub (requer gh autenticado com permissão de criar repos)
# Uso: ./scripts/setup-github-repo.sh

set -euo pipefail

REPO="Agsterr/mundo-dino"
DESCRIPTION="Jogo 3D multiplayer de sobrevivência em mundo aberto com dinossauros — Unity/C#"

if gh repo view "$REPO" >/dev/null 2>&1; then
  echo "Repositório $REPO já existe."
else
  echo "Criando repositório $REPO..."
  gh repo create "$REPO" \
    --public \
    --description "$DESCRIPTION" \
    --source=. \
    --remote=origin \
    --push
  echo "Repositório criado e código enviado."
  exit 0
fi

if ! git remote get-url origin >/dev/null 2>&1; then
  git remote add origin "https://github.com/$REPO.git"
fi

git push -u origin main
echo "Push concluído para $REPO."
