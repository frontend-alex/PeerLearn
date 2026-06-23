import pickle
import pandas as pd

from pathlib import Path

from app.server.src.config.config import SAVED_MODELS_DIR

def resolve_path(override: str | Path | None, default: Path) -> Path:
    return Path(override) if override is not None else default


def validate_csv(path: Path) -> None:
    if not path.exists():
        raise FileNotFoundError(f"CSV file not found at {path}")

def save_model(model: object, name: str | Path) -> None:
    SAVED_MODELS_DIR.mkdir(parents=True, exist_ok=True)
    path = SAVED_MODELS_DIR / Path(name)
    with open(path, "wb") as f:
        pickle.dump(model, f)


def save_csv(df: pd.DataFrame, path: str | Path) -> None:
    filepath = Path(path)
    filepath.parent.mkdir(parents=True, exist_ok=True)
    df.to_csv(filepath, index=False)
