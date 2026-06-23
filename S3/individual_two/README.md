# Individual Two: Migraine Type Prediction

This project is a full-stack migraine classification prototype. The frontend asks a user migraine symptom questions, sends the answers to a FastAPI backend, and the backend uses a trained Random Forest model to predict the migraine type.

The main goal of the assignment was to connect a machine learning model to a usable application instead of leaving the model only in notebooks. The project includes the dataset, model training code, saved model file, API layer, and web interface.

## Project Structure

| Path | Purpose |
| --- | --- |
| `README.md` | Main setup and review guide for the whole project. |
| `install.sh` | Installs both backend Python packages and frontend npm packages. |
| `Makefile` | Convenience commands for running the backend and frontend together. |
| `server/` | Python backend, data, notebooks, trained model, and API. |
| `server/api/` | FastAPI app, request/response DTOs, and prediction wrapper. |
| `server/src/model/random_forest.py` | Random Forest training implementation. |
| `server/src/notebook/` | Data cleaning, visualisation, iteration, and model experiment notebooks. |
| `server/data/raw/raw.csv` | Original migraine dataset used for training. |
| `server/data/processed/processed.csv` | Processed dataset. |
| `server/data/models/random_forest.pkl` | Saved trained model used by the API. |
| `server/README.md` | Model evaluation notes and Random Forest results. |
| `server/docs/SECOND_MODEL_EVAL.md` | Additional model evaluation documentation. |
| `web/` | Next.js frontend application. |
| `web/app/` | Main pages and API route that proxies requests to FastAPI. |
| `web/api/migraine.ts` | Frontend validation schemas for migraine prediction input/output. |

## What To Review

Start with these files if you are reviewing the submission:

1. `README.md` for the project overview and setup instructions.
2. `server/README.md` for model evaluation and performance results.
3. `server/src/model/random_forest.py` for the training code.
4. `server/api/main.py` and `server/api/model.py` for the backend prediction API.
5. `web/app/page.tsx`, `web/app/migraine-form.tsx`, and `web/app/api/migraine/route.ts` for the frontend flow.
6. `server/src/notebook/` for the exploratory work, cleaning, visualisation, and experiments.

## Prerequisites

Install these before running the project:

| Tool | Recommended Version | Why It Is Needed |
| --- | --- | --- |
| Python | 3.10 or newer | Runs the FastAPI backend and ML model. |
| Node.js | 20 or newer | Runs the Next.js frontend. |
| npm | Comes with Node.js | Installs frontend packages. |
| make | Optional | Runs both apps with one command on macOS/Linux. |

## Install Everything

From the project root, run:

```bash
./install.sh
```

The script does the following:

1. Creates a Python virtual environment at `server/venv` if it does not already exist.
2. Installs the backend packages from `server/requirements.txt`.
3. Installs the frontend packages from `web/package-lock.json` using `npm install`.

If the script is not executable after downloading or unzipping the project, run:

```bash
chmod +x install.sh
./install.sh
```

## Run The Project

The backend and frontend must both be running.

### Option 1: Run Both With Make

On macOS/Linux, after running `./install.sh`, start both applications with:

```bash
make dev
```

This starts:

| Service | URL |
| --- | --- |
| Frontend | `http://localhost:3000` |
| Backend API | `http://127.0.0.1:8000` |
| Backend API docs | `http://127.0.0.1:8000/docs` |

### Option 2: Run Manually In Two Terminals

Terminal 1, backend:

```bash
cd server
source venv/bin/activate
uvicorn api.main:app --reload
```

Terminal 2, frontend:

```bash
cd web
npm run dev
```

Then open:

```text
http://localhost:3000
```

## Windows Setup Notes

Use the same installer if you have Git Bash, WSL, or another Bash-compatible shell:

```bash
./install.sh
```

If you prefer PowerShell, install manually:

```powershell
cd server
py -m venv venv
.\venv\Scripts\Activate.ps1
python -m pip install --upgrade pip
python -m pip install -r requirements.txt
cd ..\web
npm install
```

Then run the backend and frontend in separate terminals:

```powershell
cd server
.\venv\Scripts\Activate.ps1
uvicorn api.main:app --reload
```

```powershell
cd web
npm run dev
```

## Backend API

The FastAPI backend exposes two main endpoints:

| Method | Endpoint | Purpose |
| --- | --- | --- |
| `GET` | `/health` | Checks that the backend is running. |
| `POST` | `/predict` | Predicts the migraine type from symptom input. |

To check the backend manually, open:

```text
http://127.0.0.1:8000/health
```

Expected response:

```json
{
  "status": "ok"
}
```

You can also test the prediction endpoint through the interactive FastAPI docs:

```text
http://127.0.0.1:8000/docs
```

## Frontend And Backend Connection

The frontend calls its own Next.js API route at `web/app/api/migraine/route.ts`. That route forwards prediction requests to the FastAPI backend.

By default, the frontend expects the backend at:

```text
http://127.0.0.1:8000
```

To use another backend URL, set `FASTAPI_BASE_URL` before starting the frontend:

```bash
cd web
FASTAPI_BASE_URL=http://127.0.0.1:8000 npm run dev
```

## Train Or Rebuild The Model

The saved model already exists at:

```text
server/data/models/random_forest.pkl
```

To retrain it, run:

```bash
cd server
source venv/bin/activate
python main.py
```

The training code loads `server/data/raw/raw.csv`, trains the Random Forest model, prints evaluation metrics, and saves the model file used by the API.

## Machine Learning Summary

The current production model is a Random Forest classifier. The dataset is imbalanced, so the implementation uses `class_weight="balanced"` to reduce majority-class bias. The model predicts one of the migraine type labels from 23 symptom/input features.

More detail is available in:

```text
server/README.md
server/docs/SECOND_MODEL_EVAL.md
server/src/notebook/
```

## Common Problems

| Problem | Fix |
| --- | --- |
| `uvicorn: command not found` | Activate `server/venv` first or rerun `./install.sh`. |
| Frontend loads but prediction fails | Make sure the backend is running at `http://127.0.0.1:8000`. |
| `ModuleNotFoundError` in backend | Run `./install.sh` again or install `server/requirements.txt` inside the virtual environment. |
| Port `3000` already in use | Stop the existing process or run Next.js on another port with `npm run dev -- -p 3001`. |
| Port `8000` already in use | Stop the existing process or run FastAPI on another port and set `FASTAPI_BASE_URL`. |


