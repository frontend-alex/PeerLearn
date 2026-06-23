import pandas as pd

from app.server.src.data.loader import load
from app.server.src.utils.file import save_csv
from app.server.src.config.config import FEATURES, PROCESSED_CSV, TARGET

def process(
    df: pd.DataFrame | None = None, 
    save: bool = True
) -> pd.DataFrame:

    if df is None:
        df = load()
    
    df = df.copy()

    required_columns  = FEATURES + TARGET
    

    if save:
        save_csv(df, PROCESSED_CSV)

    return df