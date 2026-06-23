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
