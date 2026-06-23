from fastapi import FastAPI

from app.server.api.dto import MigraineInput, MigraineOutput
from app.server.api.model import predict_migraine_type


app = FastAPI(title="Individual Two API")


@app.get("/health")
def health() -> dict[str, str]:
    return {"status": "ok"}


@app.post("/predict", response_model=MigraineOutput)
def predict(payload: MigraineInput) -> MigraineOutput:
    prediction = predict_migraine_type(payload)
    return MigraineOutput(type=prediction)
