import pandas as pd

from app.server.src.config.config import (SAVED_MODEL_RF, FEATURES, TARGET)
from app.server.src.utils.file import save_model

from sklearn.ensemble import RandomForestClassifier

from sklearn.metrics import accuracy_score, classification_report, confusion_matrix, f1_score, roc_auc_score
from sklearn.model_selection import train_test_split


def train_rf(
    df: pd.DataFrame,
    test_size: float = 0.2,
    random_state: int = 42,
    save: bool = True,
) -> tuple[RandomForestClassifier, float]:

    X = df[FEATURES]
    y = df[TARGET]

    X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=test_size, random_state=random_state)

    model = RandomForestClassifier(
        n_estimators=100,
        max_depth=10,
        max_features="sqrt",
        min_samples_leaf=1,
        class_weight="balanced",
        random_state=random_state,
    )

    model.fit(X_train, y_train)

    y_pred = model.predict(X_test)
    y_proba = model.predict_proba(X_test)

    accuracy = accuracy_score(y_test, y_pred)
    roc_auc = roc_auc_score(y_test, y_proba, multi_class="ovr", average="weighted")
    f1_weighted = f1_score(y_test, y_pred, average="weighted", zero_division=0)
    report = classification_report(y_test, y_pred, zero_division=0)
    
    cm = pd.DataFrame(
        confusion_matrix(y_test, y_pred, labels=model.classes_),
        index=model.classes_,
        columns=model.classes_,
    )

    print(f"\n{'='*60}")
    print(f"  Random Forest - Migraine Type Prediction")
    print(f"{'='*60}")
    print(f"  Accuracy:      {accuracy:.4f}")
    print(f"  ROC-AUC OVR:   {roc_auc:.4f}")
    print(f"  F1 weighted:   {f1_weighted:.4f}")
    print(f"\n{report}")
    print(f"  Confusion Matrix:")
    print(cm)

    if save:
        save_model(model, SAVED_MODEL_RF)

    return model, accuracy
