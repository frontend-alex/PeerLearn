from app.server.src.model.random_forest import train_rf

from app.server.src.data.loader import load
from app.server.src.config.config import RAW_CSV

def main():
    df = load(RAW_CSV)

    model, accuracy = train_rf(df)
    
if __name__ == "__main__":
    main()
