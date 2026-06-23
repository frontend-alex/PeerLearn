# Random Forest Model Evaluation

This document records the baseline evaluation for `src/model/random_forest.py` and the first grid-search tuning run.

## Dataset

The model was evaluated on `data/raw/raw.csv`.

| Item | Value |
| --- | ---: |
| Rows | 400 |
| Feature columns | 23 |
| Target column | `Type` |
| Train rows | 320 |
| Test rows | 80 |

The train/test split matches the current `train_rf` implementation: `test_size=0.2`, `random_state=42`, and no stratification.

Test target distribution:

| Class | Count |
| --- | ---: |
| Typical aura with migraine | 49 |
| Migraine without aura | 13 |
| Basilar-type aura | 6 |
| Other | 4 |
| Familial hemiplegic migraine | 3 |
| Typical aura without migraine | 3 |
| Sporadic hemiplegic migraine | 2 |

## Baseline Model

Baseline estimator:

```python
RandomForestClassifier(random_state=42)
```

Overall baseline metrics:

| Metric | Score |
| --- | ---: |
| Accuracy | 0.9250 |
| Precision macro | 0.8086 |
| Recall macro | 0.7471 |
| F1 macro | 0.7698 |
| Precision weighted | 0.9163 |
| Recall weighted | 0.9250 |
| F1 weighted | 0.9177 |

Per-class baseline report:

| Class | Precision | Recall | F1-score | Support |
| --- | ---: | ---: | ---: | ---: |
| Basilar-type aura | 0.8333 | 0.8333 | 0.8333 | 6 |
| Familial hemiplegic migraine | 1.0000 | 0.6667 | 0.8000 | 3 |
| Migraine without aura | 0.8667 | 1.0000 | 0.9286 | 13 |
| Other | 1.0000 | 0.7500 | 0.8571 | 4 |
| Sporadic hemiplegic migraine | 0.0000 | 0.0000 | 0.0000 | 2 |
| Typical aura with migraine | 0.9600 | 0.9796 | 0.9697 | 49 |
| Typical aura without migraine | 1.0000 | 1.0000 | 1.0000 | 3 |

The model performs strongly overall, but the macro scores are lower than weighted scores because the test set is imbalanced and the model missed both `Sporadic hemiplegic migraine` examples.

## Grid Search

Notebook: `src/notebook/random-forest-grid-search.ipynb`

The grid search optimizes `f1_weighted` using 5-fold stratified cross-validation on the training split.

Parameter grid:

| Parameter | Values |
| --- | --- |
| `n_estimators` | `100`, `200`, `300` |
| `max_depth` | `None`, `5`, `10`, `20` |
| `min_samples_split` | `2`, `5`, `10` |
| `min_samples_leaf` | `1`, `2`, `4` |
| `max_features` | `sqrt`, `log2` |

Best parameters found:

```python
{
    "max_depth": None,
    "max_features": "sqrt",
    "min_samples_leaf": 1,
    "min_samples_split": 2,
    "n_estimators": 100,
}
```

Best cross-validation weighted F1: `0.8706`.

## Baseline vs Tuned Model

The best grid-search parameters match the effective defaults used by `RandomForestClassifier(random_state=42)`, so the tuned model produced the same holdout results as the baseline.

| Metric | Baseline | Tuned | Delta |
| --- | ---: | ---: | ---: |
| Accuracy | 0.9250 | 0.9250 | 0.0000 |
| Precision macro | 0.8086 | 0.8086 | 0.0000 |
| Recall macro | 0.7471 | 0.7471 | 0.0000 |
| F1 macro | 0.7698 | 0.7698 | 0.0000 |
| Precision weighted | 0.9163 | 0.9163 | 0.0000 |
| Recall weighted | 0.9250 | 0.9250 | 0.0000 |
| F1 weighted | 0.9177 | 0.9177 | 0.0000 |


