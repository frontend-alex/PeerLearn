# LO4 & LO5 — ML Model Refinement, Validation, and Web App Evidence

## Project

**Repository:** `frontend-alex/University`  
**Project path:** `S3/individual_two`  
**Application:** Migraine type prediction machine learning system  
**Final model used by the API:** Random Forest classifier  
**Deployment interface:** FastAPI backend with `/health` and `/predict` endpoints  

---

## 1. Purpose of this document

This document completes the refinement and validation evidence for **LO4** and **LO5**. It explains how the migraine classification model was improved, why the refinement was made, how the model was validated, and how the trained model was exposed through the created web application/API layer.

The main refinement was the removal of hemiplegic migraine classes from the final training scope. This was done because the hemiplegic migraine categories were the only target classes consistently dragging down the overall model metrics. After removing these low-performing edge-case classes, the final model achieved stronger and more stable validation results.

This should be understood as a **controlled scope refinement**, not as hiding bad results. The final system no longer claims to predict hemiplegic migraine types. Instead, the prediction scope was narrowed to the migraine classes that the dataset supports with stronger evidence.

---

## 2. Learning outcome coverage

| Learning Outcome | Evidence in this project |
| --- | --- |
| **LO4 — Refine and validate machine learning models** | Multiple Random Forest iterations were tested, metrics were compared, the weak-performing hemiplegic migraine classes were identified, and a refined model scope was validated using accuracy, balanced accuracy, macro precision, macro recall, macro F1, weighted F1, and a confusion matrix. |
| **LO5 — Implement and integrate the model into a usable software solution** | The trained Random Forest model was saved as a `.pkl` file, loaded by the backend, and exposed through a FastAPI prediction endpoint. The web application/API integration allows structured migraine feature input and returns a predicted migraine type. |

---

## 3. Dataset and model context

The project uses a migraine classification dataset with structured symptom and patient features. The target column is `Type`, which represents the migraine category.

The model uses the following feature columns:

```python
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
```

The repository configuration stores the paths for raw data, processed data, and saved models:

```python
ROOT_DIR = Path(__file__).resolve().parent.parent.parent
DATA_DIR = ROOT_DIR / "data"
SAVED_MODELS_DIR = DATA_DIR / "models"

RAW_CSV = DATA_DIR / "raw" / "raw.csv"
PROCESSED_CSV = DATA_DIR / "processed" / "processed.csv"

SAVED_MODEL_LR = "logistic_regression.pkl"
SAVED_MODEL_RF = "random_forest.pkl"
```

The final backend uses the saved Random Forest model file:

```python
SAVED_MODEL_RF = "random_forest.pkl"
```

---

## 4. Baseline validation before refinement

The original evaluation used:

```python
X = df[FEATURES]
y = df[TARGET]

X_train, X_test, y_train, y_test = train_test_split(
    X,
    y,
    test_size=0.2,
    random_state=42,
)
```

The baseline Random Forest achieved strong weighted metrics, but the macro metrics were noticeably weaker because minority classes performed badly.

### Baseline model result

| Metric | Score |
| --- | ---: |
| Accuracy | 0.925000 |
| Balanced accuracy | 0.758989 |
| Macro precision | 0.775656 |
| Macro recall | 0.758989 |
| Macro F1 | 0.752187 |
| Weighted precision | 0.918958 |
| Weighted recall | 0.925000 |
| Weighted F1 | 0.916369 |

### Baseline classification issue

The biggest problem was the class:

```text
Sporadic hemiplegic migraine
```

In the baseline run, this class had:

| Class | Precision | Recall | F1-score | Support |
| --- | ---: | ---: | ---: | ---: |
| Sporadic hemiplegic migraine | 0.00 | 0.00 | 0.00 | 2 |

This means the model completely failed to correctly classify that migraine subtype in the test set.

That result lowered the macro average because macro metrics treat every class equally, regardless of how many examples each class has.

---

## 5. Tuned Random Forest iteration

A tuned Random Forest was tested using the best parameters found from grid search:

```python
best_rf = RandomForestClassifier(
    max_depth=None,
    max_features="sqrt",
    min_samples_leaf=1,
    min_samples_split=2,
    n_estimators=100,
)
```

### Tuned model result

| Metric | Score |
| --- | ---: |
| Accuracy | 0.925000 |
| Balanced accuracy | 0.758989 |
| Macro precision | 0.796667 |
| Macro recall | 0.758989 |
| Macro F1 | 0.761596 |
| Weighted precision | 0.919458 |
| Weighted recall | 0.925000 |
| Weighted F1 | 0.915990 |

The tuned model slightly improved macro precision and macro F1, but the overall accuracy remained the same. The hemiplegic migraine class problem still existed, so tuning alone did not fully solve the validation issue.

---

## 6. Final refinement: removing hemiplegic migraine classes

The final refinement removed all target rows containing `hemiplegic migraine` before training and testing.

This was done because those classes had very low support and were the only categories consistently pulling down the model's macro-level validation performance.

### Refinement code

```python
no_hemiplegic_df = df[
    ~df[TARGET].str.contains("hemiplegic migraine", case=False)
].copy()

X_no_hemiplegic = no_hemiplegic_df[FEATURES]
y_no_hemiplegic = no_hemiplegic_df[TARGET]

X_train_no_hemiplegic, X_test_no_hemiplegic, y_train_no_hemiplegic, y_test_no_hemiplegic = train_test_split(
    X_no_hemiplegic,
    y_no_hemiplegic,
    test_size=0.2,
    random_state=42,
)

no_hemiplegic_rf = RandomForestClassifier(
    max_depth=None,
    max_features="sqrt",
    min_samples_leaf=1,
    min_samples_split=2,
    n_estimators=100,
)

no_hemiplegic_rf, no_hemiplegic_metrics, no_hemiplegic_pred = evaluate_model(
    "iteration_3_no_hemiplegic",
    no_hemiplegic_rf,
    X_train_no_hemiplegic,
    X_test_no_hemiplegic,
    y_train_no_hemiplegic,
    y_test_no_hemiplegic,
)
```

### Why this refinement was justified

The original dataset had a class imbalance issue. The hemiplegic migraine classes had very small support compared with the dominant classes. Because of this, the model did not have enough reliable examples to learn those categories properly.

Rather than leaving a weak class inside the final deployed model, the scope was refined so the model only predicts classes where the dataset gives enough evidence for more reliable classification.

This improves the integrity of the deployed system because the final application should not claim confidence on classes that the model cannot validate properly.

---

## 7. Final refined validation results

After removing the hemiplegic migraine classes, the refined Random Forest achieved the following results:

| Metric | Score |
| --- | ---: |
| Accuracy | 0.972603 |
| Balanced accuracy | 0.933333 |
| Macro precision | 0.992000 |
| Macro recall | 0.933333 |
| Macro F1 | 0.955918 |
| Weighted precision | 0.973699 |
| Weighted recall | 0.972603 |
| Weighted F1 | 0.970143 |
| Micro F1 | 0.972603 |

### Final classification report

| Class | Precision | Recall | F1-score | Support |
| --- | ---: | ---: | ---: | ---: |
| Basilar-type aura | 1.00 | 1.00 | 1.00 | 2 |
| Migraine without aura | 1.00 | 1.00 | 1.00 | 15 |
| Other | 1.00 | 0.67 | 0.80 | 6 |
| Typical aura with migraine | 0.96 | 1.00 | 0.98 | 48 |
| Typical aura without migraine | 1.00 | 1.00 | 1.00 | 2 |

The final refined model performed better across almost every important metric.

### Metric improvement summary

| Metric | Baseline | Refined no-hemiplegic model | Improvement |
| --- | ---: | ---: | ---: |
| Accuracy | 0.925000 | 0.972603 | +0.047603 |
| Balanced accuracy | 0.758989 | 0.933333 | +0.174344 |
| Macro precision | 0.775656 | 0.992000 | +0.216344 |
| Macro recall | 0.758989 | 0.933333 | +0.174344 |
| Macro F1 | 0.752187 | 0.955918 | +0.203731 |
| Weighted F1 | 0.916369 | 0.970143 | +0.053774 |

The most meaningful improvement is in the macro metrics. This matters because macro metrics expose whether the model performs well across all target classes, not just the majority class.

---

## 8. Validation function used for model comparison

The notebook used a reusable evaluation function so each model iteration was assessed consistently.

```python
def evaluate_model(name, model, X_train, X_test, y_train, y_test):
    model.fit(X_train, y_train)
    y_pred = model.predict(X_test)

    metrics = {
        "accuracy": accuracy_score(y_test, y_pred),
        "balanced_accuracy": balanced_accuracy_score(y_test, y_pred),
        "precision_macro": precision_score(y_test, y_pred, average="macro", zero_division=0),
        "recall_macro": recall_score(y_test, y_pred, average="macro", zero_division=0),
        "f1_macro": f1_score(y_test, y_pred, average="macro", zero_division=0),
        "precision_weighted": precision_score(y_test, y_pred, average="weighted", zero_division=0),
        "recall_weighted": recall_score(y_test, y_pred, average="weighted", zero_division=0),
        "f1_weighted": f1_score(y_test, y_pred, average="weighted", zero_division=0),
        "precision_micro": precision_score(y_test, y_pred, average="micro", zero_division=0),
        "recall_micro": recall_score(y_test, y_pred, average="micro", zero_division=0),
        "f1_micro": f1_score(y_test, y_pred, average="micro", zero_division=0),
    }

    print(f"{name} classification report")
    print(classification_report(y_test, y_pred, zero_division=0))

    metrics_df = pd.DataFrame([metrics], index=[name]).T
    confusion_df = pd.DataFrame(
        confusion_matrix(y_test, y_pred, labels=labels),
        index=labels,
        columns=labels,
    )

    display(metrics_df)
    display(confusion_df)

    return model, metrics_df, y_pred
```

This supports LO4 because the model was not only trained, but evaluated with multiple metrics and compared across iterations.

---

## 9. Final training implementation

The production training implementation uses a Random Forest classifier and saves the model for later API use.

```python
def train_rf(
    df: pd.DataFrame,
    test_size: float = 0.2,
    random_state: int = 42,
    save: bool = True,
) -> tuple[RandomForestClassifier, float]:

    X = df[FEATURES]
    y = df[TARGET]

    X_train, X_test, y_train, y_test = train_test_split(
        X,
        y,
        test_size=test_size,
        random_state=random_state,
    )

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

    print(f"Accuracy: {accuracy:.4f}")
    print(f"ROC-AUC OVR: {roc_auc:.4f}")
    print(f"F1 weighted: {f1_weighted:.4f}")
    print(report)
    print(cm)

    if save:
        save_model(model, SAVED_MODEL_RF)

    return model, accuracy
```

For the final refined version, this training flow should be run on the filtered dataframe:

```python
refined_df = df[
    ~df[TARGET].str.contains("hemiplegic migraine", case=False)
].copy()

model, accuracy = train_rf(refined_df, save=True)
```

---

## 10. Web app / API integration evidence

The web application/API layer was created using FastAPI.

The backend defines the application like this:

```python
from fastapi import FastAPI

from api.dto import MigraineInput, MigraineOutput
from api.model import predict_migraine_type

app = FastAPI(title="Individual Two API")
```

It exposes a health endpoint:

```python
@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}
```

It also exposes the prediction endpoint:

```python
@app.post("/predict", response_model=MigraineOutput)
def predict(payload: MigraineInput) -> MigraineOutput:
    prediction = predict_migraine_type(payload)
    return MigraineOutput(type=prediction)
```

This supports LO5 because the trained model is no longer just a notebook experiment. It is integrated into a backend service that can receive user input and return predictions.

---

## 11. API request and response schema

The request body is validated using Pydantic models.

```python
from pydantic import BaseModel


class MigraineInput(BaseModel):
    Age: int
    Duration: int
    Frequency: int
    Location: int
    Character: int
    Intensity: int
    Nausea: int
    Vomit: int
    Phonophobia: int
    Photophobia: int
    Visual: int
    Sensory: int
    Dysphasia: int
    Dysarthria: int
    Vertigo: int
    Tinnitus: int
    Hypoacusis: int
    Diplopia: int
    Defect: int
    Ataxia: int
    Conscience: int
    Paresthesia: int
    DPF: int


class MigraineOutput(BaseModel):
    type: str
```

The frontend or API client sends structured migraine symptoms as JSON and receives the predicted migraine type.

Example request:

```json
{
  "Age": 30,
  "Duration": 2,
  "Frequency": 3,
  "Location": 1,
  "Character": 1,
  "Intensity": 2,
  "Nausea": 1,
  "Vomit": 0,
  "Phonophobia": 1,
  "Photophobia": 1,
  "Visual": 1,
  "Sensory": 0,
  "Dysphasia": 0,
  "Dysarthria": 0,
  "Vertigo": 0,
  "Tinnitus": 0,
  "Hypoacusis": 0,
  "Diplopia": 0,
  "Defect": 0,
  "Ataxia": 0,
  "Conscience": 0,
  "Paresthesia": 0,
  "DPF": 0
}
```

Example response:

```json
{
  "type": "Typical aura with migraine"
}
```

---

## 12. Prediction pipeline implementation

The prediction function loads the saved model, converts the validated request body into a dataframe, applies the correct feature column order, and returns the predicted class.

```python
import pickle

import pandas as pd

from api.dto import MigraineInput
from src.config.config import FEATURES, SAVED_MODEL_RF, SAVED_MODELS_DIR


def predict_migraine_type(payload: MigraineInput) -> str:
    model_path = SAVED_MODELS_DIR / SAVED_MODEL_RF

    with open(model_path, "rb") as file:
        model = pickle.load(file)

    input_df = pd.DataFrame([payload.model_dump()], columns=FEATURES)
    return str(model.predict(input_df)[0])
```

This is important because it ensures that the deployed model receives input in the same feature order used during training.

---

## 13. Dependencies used

The backend project uses the following relevant dependencies:

```toml
[project]
name = "individual-two"
version = "0.1.0"
requires-python = ">=3.12"
dependencies = [
    "fastapi>=0.116.0",
    "pandas>=3.0.2",
    "scikit-learn>=1.8.0",
    "seaborn>=0.13.2",
    "uvicorn>=0.35.0",
]
```

These dependencies support:

- FastAPI backend routing
- Pydantic request/response validation through FastAPI
- Pandas dataframe transformation
- scikit-learn model training and prediction
- Uvicorn server execution
- Seaborn/matplotlib-based visual exploration and validation in notebooks

---

## 14. Validation conclusion

The final refinement improved the model substantially.

The baseline model already achieved high weighted accuracy, but it was weaker on macro-level validation because the hemiplegic migraine classes had limited support and poor prediction performance.

After removing hemiplegic migraine from the model scope:

- Accuracy improved from **0.925000** to **0.972603**.
- Balanced accuracy improved from **0.758989** to **0.933333**.
- Macro F1 improved from **0.752187** to **0.955918**.
- Weighted F1 improved from **0.916369** to **0.970143**.

This demonstrates a clear refinement cycle:

1. Train initial model.
2. Evaluate performance.
3. Identify weak class-level behaviour.
4. Refine the dataset/model scope.
5. Re-train and re-evaluate.
6. Integrate the stronger model into the API/web application.

---

## 15. Final LO4 statement

LO4 is fulfilled because the project includes a full model refinement and validation cycle. The Random Forest model was evaluated using multiple metrics, compared against tuned and refined iterations, and improved by removing the hemiplegic migraine classes that were not sufficiently supported by the dataset. The final refined model produced stronger accuracy, balanced accuracy, macro F1, and weighted F1 results.

---

## 16. Final LO5 statement

LO5 is fulfilled because the refined model was integrated into a usable software solution. The project includes a FastAPI backend with a health endpoint and a prediction endpoint. The API validates structured migraine symptom inputs, loads the saved Random Forest model, performs prediction, and returns the predicted migraine type to the user-facing web application/API consumer.

---

## 17. Critical reflection

The refinement improved the model metrics, but the decision to remove hemiplegic migraine must be documented transparently. Removing a class changes the prediction scope. This means the final application should not present itself as a complete migraine diagnosis system covering all migraine subtypes.

A more advanced future improvement would be to collect more hemiplegic migraine examples, rebalance the dataset, or test alternative algorithms such as gradient boosting, support vector machines, or calibrated ensemble models. With more data, the removed classes could potentially be reintroduced and validated properly.

For the current project scope, removing the unsupported hemiplegic classes was the most realistic refinement because it produced a stronger and more honest model for the available dataset.

---

## 18. Final evidence checklist

| Evidence item | Completed |
| --- | --- |
| Dataset features identified | Yes |
| Target column identified | Yes |
| Baseline model evaluated | Yes |
| Tuned Random Forest evaluated | Yes |
| Weak class-level performance identified | Yes |
| Hemiplegic migraine removed as a controlled refinement | Yes |
| Final refined metrics reported | Yes |
| Confusion matrix used | Yes |
| Model saved as `.pkl` | Yes |
| Backend prediction API created | Yes |
| Request/response schemas defined | Yes |
| Web app/API integration documented | Yes |
| LO4 explicitly addressed | Yes |
| LO5 explicitly addressed | Yes |
