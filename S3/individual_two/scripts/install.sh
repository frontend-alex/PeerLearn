#!/usr/bin/env bash
set -euo pipefail

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
SERVER_DIR="$ROOT_DIR/app/server"
WEB_DIR="$ROOT_DIR/app/web"
PYTHON_BIN="${PYTHON_BIN:-python3}"

if ! command -v "$PYTHON_BIN" >/dev/null 2>&1; then
  echo "Python command '$PYTHON_BIN' was not found. Install Python 3.10+ or set PYTHON_BIN." >&2
  exit 1
fi

if ! command -v npm >/dev/null 2>&1; then
  echo "npm was not found. Install Node.js 20+ and try again." >&2
  exit 1
fi

echo "Setting up backend virtual environment..."
"$PYTHON_BIN" -m venv "$SERVER_DIR/venv"

# shellcheck source=/dev/null
source "$SERVER_DIR/venv/bin/activate"

echo "Installing backend packages..."
python -m pip install --upgrade pip
python -m pip install -r "$SERVER_DIR/requirements.txt"

echo "Installing frontend packages..."
npm --prefix "$WEB_DIR" install

echo ""
echo "Installation complete."
echo "Run both apps with: make dev"
echo "Or run backend: cd app/server && source venv/bin/activate && uvicorn api.main:app --reload"
echo "And frontend: cd app/web && npm run dev"
