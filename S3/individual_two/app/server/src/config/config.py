from pathlib import Path


ROOT_DIR = Path(__file__).resolve().parent.parent.parent
DATA_DIR = ROOT_DIR / "data"
SAVED_MODELS_DIR = DATA_DIR / "models"

RAW_CSV = DATA_DIR / "raw" / "raw.csv"
PROCESSED_CSV = DATA_DIR / "processed" / "processed.csv"


# Filenames
SAVED_MODEL_LR = "logistic_regression.pkl"
SAVED_MODEL_RF = "random_forest.pkl"

# Features
FEATURES = [
    "Age", 
    "Duration", 
    "Frequency", 
    "Location", 
    "Character", 
    "Intensity", 
    "Nausea", 
    "Vomit", 
    "Phonophobia", 
    "Photophobia", 
    "Visual", 
    "Sensory", 
    "Dysphasia", 
    "Dysarthria", 
    "Vertigo", 
    "Tinnitus", 
    "Hypoacusis", 
    "Diplopia",
    "Defect", 
    "Ataxia", 
    "Conscience",
    "Paresthesia", 
    "DPF", 
]

TARGET = "Type"
