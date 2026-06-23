# Second Random Forest Evaluation

This evaluation explains why the random forest scores highly, what risk that creates, and how the model was optimized to give more attention to minority migraine classes.

## Why The Model Scores High

The baseline model scores high because the dataset has strong clinical signal for the target `Type`.

Key reasons:

| Reason | Effect |
| --- | --- |
| Diagnostic features | Columns such as `Duration`, `Frequency`, `Visual`, `Sensory`, `Dysphasia`, `Vertigo`, `Nausea`, `Photophobia`, and `Phonophobia` are directly related to migraine classification. |
| Small dataset | The test split has only 80 rows, so a few predictions can strongly affect the final scores. |
| Class imbalance | The largest class, `Typical aura with migraine`, dominates the test set with 49 of 80 rows. |
| Weighted metrics favor large classes | Weighted F1 stays high when the model performs well on the dominant class, even if rare classes are weaker. |
| Possible label-rule overlap | Some features may reflect the same clinical rules used to assign the migraine type, making the prediction task easier than a noisy real-world deployment. |

The most important warning is that accuracy alone is too optimistic. The baseline accuracy was `0.9250`, but macro F1 was only `0.7698`, showing that minority-class performance is weaker.

## How To Fix The Evaluation Problem

The issue is not that high accuracy is automatically wrong. The issue is that accuracy and weighted F1 can hide poor minority-class performance.

Better evaluation choices:

| Fix | Why It Helps |
| --- | --- |
| Track macro F1 | Gives each class equal importance, regardless of class size. |
| Track macro recall | Shows whether rare classes are being missed. |
| Use per-class precision, recall, and F1 | Makes minority-class failures visible. |
| Use stratified cross-validation during tuning | Preserves class distribution across folds. |
| Tune with `f1_macro` | Optimizes for balanced class performance instead of majority-class performance. |
| Consider `class_weight="balanced"` | Penalizes mistakes on minority classes more heavily. |

## Optimization Run

The optimized model was tuned using 5-fold stratified cross-validation on the training split.

Optimization target:

```text
f1_macro
```

Search space:

| Parameter | Values |
| --- | --- |
| `n_estimators` | `100`, `300` |
| `max_depth` | `None`, `10`, `20` |
| `min_samples_leaf` | `1`, `2`, `4` |
| `max_features` | `sqrt`, `None` |
| `class_weight` | `None`, `balanced`, `balanced_subsample` |

Best parameters:

```python
RandomForestClassifier(
    n_estimators=100,
    max_depth=10,
    max_features="sqrt",
    min_samples_leaf=1,
    class_weight="balanced",
    random_state=42,
)
```

Best cross-validation macro F1:

```text
0.7388
```

## Baseline vs Optimized

Both models were evaluated on the same holdout test split: `test_size=0.2`, `random_state=42`, and no stratification, matching the project training function.

| Metric | Baseline | Optimized | Delta |
| --- | ---: | ---: | ---: |
| Accuracy | 0.9250 | 0.9250 | 0.0000 |
| Precision macro | 0.8086 | 0.8104 | +0.0019 |
| Recall macro | 0.7471 | 0.7828 | +0.0357 |
| F1 macro | 0.7698 | 0.7820 | +0.0122 |
| Precision weighted | 0.9163 | 0.9185 | +0.0022 |
| Recall weighted | 0.9250 | 0.9250 | 0.0000 |
| F1 weighted | 0.9177 | 0.9143 | -0.0034 |
| ROC-AUC OVR weighted | 0.9961 | 0.9955 | -0.0006 |

The optimized model is better if the goal is fairer class-level performance. It improves macro recall and macro F1 while keeping accuracy unchanged. It slightly reduces weighted F1 because it trades a small amount of majority-class performance for better minority-class behavior.

## Per-Class Comparison

| Class | Baseline F1 | Optimized F1 | Change |
| --- | ---: | ---: | ---: |
| Basilar-type aura | 0.8333 | 0.6667 | -0.1666 |
| Familial hemiplegic migraine | 0.8000 | 0.8571 | +0.0571 |
| Migraine without aura | 0.9286 | 1.0000 | +0.0714 |
| Other | 0.8571 | 1.0000 | +0.1429 |
| Sporadic hemiplegic migraine | 0.0000 | 0.0000 | 0.0000 |
| Typical aura with migraine | 0.9697 | 0.9505 | -0.0192 |
| Typical aura without migraine | 1.0000 | 1.0000 | 0.0000 |

The optimized model improved several minority or medium-frequency classes, but it still failed on `Sporadic hemiplegic migraine`. That class only has 2 examples in the test set and 14 rows in the full dataset, so the model has very little data to learn it reliably.

## Final Decision

The optimized model is now used in `src/model/random_forest.py` because it is better aligned with the real evaluation goal: improving balanced class performance rather than only maximizing accuracy.

Current model parameters:

```python
RandomForestClassifier(
    n_estimators=100,
    max_depth=10,
    max_features="sqrt",
    min_samples_leaf=1,
    class_weight="balanced",
    random_state=random_state,
)
```

## Remaining Limitations

The optimized model is not a complete fix.

Main limitations:

| Limitation | Impact |
| --- | --- |
| Very small minority classes | The model still cannot reliably identify `Sporadic hemiplegic migraine`. |
| Small holdout test set | Results may vary significantly with a different split. |
| Possible feature leakage | If symptoms directly encode diagnosis rules, the model may score higher than expected in real-world use. |
| No external validation set | The current score only reflects this dataset. |