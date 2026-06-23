from pathlib import Path
import pandas as pd

from app.server.src.config.config import RAW_CSV
from app.server.src.utils import resolve_path, validate_csv


def load(path: str | Path | None = RAW_CSV) -> pd.DataFrame:

    filepath = resolve_path(path, RAW_CSV)
    validate_csv(filepath)

    df = pd.read_csv(filepath)

    return df
