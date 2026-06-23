import { Dispatch, SetStateAction } from "react";
import { z } from "zod";

import { MigraineInput, migraineInputSchema, migraineOutputSchema, predictMigraine } from "@/api/migraine";
import { Field, Question, questions } from "@/const/migraine-questions";

export { questions } from "@/const/migraine-questions";

const initialAnswers: Partial<MigraineInput> = {};

export const migraineFormStateSchema = z.object({
  answers: migraineInputSchema.partial(),
  ageInput: z.string(),
  step: z.number().int().min(0).max(questions.length - 1),
  result: migraineOutputSchema.nullable(),
  error: z.string().nullable(),
  isSubmitting: z.boolean(),
});

export type MigraineFormState = z.infer<typeof migraineFormStateSchema>;
export type SetMigraineFormState = Dispatch<SetStateAction<MigraineFormState>>;

export const initialMigraineFormState = migraineFormStateSchema.parse({
  answers: initialAnswers,
  ageInput: "",
  step: 0,
  result: null,
  error: null,
  isSubmitting: false,
});

export function getCurrentQuestion(form: MigraineFormState): Question {
  return questions[form.step];
}

export function getProgress(form: MigraineFormState): number {
  return Math.round(((form.step + 1) / questions.length) * 100);
}

export function canContinue(form: MigraineFormState): boolean {
  const question = getCurrentQuestion(form);

  if (question.type === "number") {
    const age = Number(form.ageInput);
    return Boolean(form.ageInput) && Number.isInteger(age) && age >= question.min && age <= question.max;
  }

  return form.answers[question.field] !== undefined;
}

export function updateAnswer(setForm: SetMigraineFormState, field: Field, value: number) {
  setForm((current) =>
    migraineFormStateSchema.parse({
      ...current,
      answers: { ...current.answers, [field]: value },
      error: null,
    }),
  );
}

export function updateAgeInput(setForm: SetMigraineFormState, value: string) {
  setForm((current) => migraineFormStateSchema.parse({ ...current, ageInput: value, error: null }));
}

export function handleBack(setForm: SetMigraineFormState) {
  setForm((current) =>
    migraineFormStateSchema.parse({
      ...current,
      step: Math.max(0, current.step - 1),
      result: null,
      error: null,
    }),
  );
}

export function handleRestart(setForm: SetMigraineFormState) {
  setForm(initialMigraineFormState);
}

export async function handleNext(form: MigraineFormState, setForm: SetMigraineFormState) {
  const question = getCurrentQuestion(form);
  const isLastStep = form.step === questions.length - 1;
  let answers = form.answers;

  if (question.type === "number") {
    const age = Number(form.ageInput);

    if (!form.ageInput || !Number.isInteger(age) || age < question.min || age > question.max) {
      setForm((current) =>
        migraineFormStateSchema.parse({
          ...current,
          error: `Enter an age between ${question.min} and ${question.max}.`,
          result: null,
        }),
      );
      return;
    }

    answers = { ...answers, [question.field]: age };
  } else if (answers[question.field] === undefined) {
    setForm((current) =>
      migraineFormStateSchema.parse({
        ...current,
        error: "Choose an answer before continuing.",
        result: null,
      }),
    );
    return;
  }

  if (!isLastStep) {
    setForm((current) =>
      migraineFormStateSchema.parse({
        ...current,
        answers,
        step: current.step + 1,
        result: null,
        error: null,
      }),
    );
    return;
  }

  const parsedInput = migraineInputSchema.safeParse(answers);

  if (!parsedInput.success) {
    setForm((current) =>
      migraineFormStateSchema.parse({
        ...current,
        error: parsedInput.error.issues[0]?.message ?? "Please check your answers.",
        result: null,
      }),
    );
    return;
  }

  setForm((current) => migraineFormStateSchema.parse({ ...current, answers, isSubmitting: true, result: null, error: null }));

  try {
    const result = await predictMigraine(parsedInput.data);
    setForm((current) => migraineFormStateSchema.parse({ ...current, result, isSubmitting: false }));
  } catch (unknownError) {
    setForm((current) =>
      migraineFormStateSchema.parse({
        ...current,
        error: unknownError instanceof Error ? unknownError.message : "Something went wrong",
        isSubmitting: false,
      }),
    );
  }
}
