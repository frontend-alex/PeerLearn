import { z } from "zod";

const featureValue = z.coerce.number().int().nonnegative();

export const migraineInputSchema = z.object({
  Age: featureValue,
  Duration: featureValue,
  Frequency: featureValue,
  Location: featureValue,
  Character: featureValue,
  Intensity: featureValue,
  Nausea: featureValue,
  Vomit: featureValue,
  Phonophobia: featureValue,
  Photophobia: featureValue,
  Visual: featureValue,
  Sensory: featureValue,
  Dysphasia: featureValue,
  Dysarthria: featureValue,
  Vertigo: featureValue,
  Tinnitus: featureValue,
  Hypoacusis: featureValue,
  Diplopia: featureValue,
  Defect: featureValue,
  Ataxia: featureValue,
  Conscience: featureValue,
  Paresthesia: featureValue,
  DPF: featureValue,
});

export const migraineOutputSchema = z.object({
  type: z.string(),
});

export type MigraineInput = z.infer<typeof migraineInputSchema>;
export type MigraineOutput = z.infer<typeof migraineOutputSchema>;

export async function predictMigraine(input: MigraineInput): Promise<MigraineOutput> {
  const payload = migraineInputSchema.parse(input);

  const response = await fetch("/api/migraine", {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    throw new Error("Failed to predict migraine type");
  }

  return migraineOutputSchema.parse(await response.json());
}
