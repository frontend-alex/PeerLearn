import { MigraineInput } from "@/api/migraine";

export type Field = keyof MigraineInput;

export type Question =
  | {
      field: Field;
      title: string;
      helper: string;
      type: "number";
      min: number;
      max: number;
    }
  | {
      field: Field;
      title: string;
      helper: string;
      type: "choice";
      options: { label: string; value: number; description?: string }[];
    };

const yesNo = [
  { label: "No", value: 0 },
  { label: "Yes", value: 1 },
];

export const questions: Question[] = [
  {
    field: "Age",
    title: "How old are you?",
    helper: "Age helps the model compare your symptoms against the training data.",
    type: "number",
    min: 1,
    max: 120,
  },
  {
    field: "Duration",
    title: "How long does a typical headache episode last?",
    helper: "Choose the closest duration pattern.",
    type: "choice",
    options: [
      { label: "Short", value: 1, description: "A shorter episode" },
      { label: "Medium", value: 2, description: "A moderate-length episode" },
      { label: "Long", value: 3, description: "A long-lasting episode" },
    ],
  },
  {
    field: "Frequency",
    title: "How often do these episodes happen?",
    helper: "Pick the closest frequency level from low to high.",
    type: "choice",
    options: Array.from({ length: 8 }, (_, index) => ({
      label: `Level ${index + 1}`,
      value: index + 1,
      description: index === 0 ? "Least frequent" : index === 7 ? "Most frequent" : undefined,
    })),
  },
  {
    field: "Location",
    title: "Where is the pain usually located?",
    helper: "Choose the pattern that best matches your headache location.",
    type: "choice",
    options: [
      { label: "Not specific", value: 0 },
      { label: "One side", value: 1 },
      { label: "Both sides or shifting", value: 2 },
    ],
  },
  {
    field: "Character",
    title: "How would you describe the pain?",
    helper: "This captures the main character of the headache.",
    type: "choice",
    options: [
      { label: "Other or unclear", value: 0 },
      { label: "Throbbing or pulsating", value: 1 },
      { label: "Pressure or tightness", value: 2 },
    ],
  },
  {
    field: "Intensity",
    title: "How intense is the pain?",
    helper: "Use the level that best describes a typical episode.",
    type: "choice",
    options: [
      { label: "None", value: 0 },
      { label: "Mild", value: 1 },
      { label: "Moderate", value: 2 },
      { label: "Severe", value: 3 },
    ],
  },
  { field: "Nausea", title: "Do you feel nauseous during episodes?", helper: "Nausea is common in migraine patterns.", type: "choice", options: yesNo },
  { field: "Vomit", title: "Do you vomit during episodes?", helper: "Answer yes if this happens with the headache.", type: "choice", options: yesNo },
  { field: "Phonophobia", title: "Are you sensitive to sound?", helper: "For example, normal sounds feel painful or overwhelming.", type: "choice", options: yesNo },
  { field: "Photophobia", title: "Are you sensitive to light?", helper: "For example, bright light makes symptoms worse.", type: "choice", options: yesNo },
  {
    field: "Visual",
    title: "Do you notice visual symptoms before or during the headache?",
    helper: "Choose the closest option for visual aura symptoms.",
    type: "choice",
    options: [
      { label: "No visual symptoms", value: 0 },
      { label: "Mild visual changes", value: 1 },
      { label: "Flashes, spots, or zigzags", value: 2 },
      { label: "Blurred or partial vision loss", value: 3 },
      { label: "Strong visual aura", value: 4 },
    ],
  },
  {
    field: "Sensory",
    title: "Do you feel sensory changes?",
    helper: "For example numbness, tingling, or altered sensation.",
    type: "choice",
    options: [
      { label: "No sensory symptoms", value: 0 },
      { label: "Mild sensory symptoms", value: 1 },
      { label: "Strong sensory symptoms", value: 2 },
    ],
  },
  { field: "Dysphasia", title: "Do you have trouble finding or understanding words?", helper: "This means language difficulty during an episode.", type: "choice", options: yesNo },
  { field: "Dysarthria", title: "Does your speech become slurred?", helper: "Answer yes if articulation becomes difficult.", type: "choice", options: yesNo },
  { field: "Vertigo", title: "Do you experience vertigo or spinning sensation?", helper: "This is different from mild dizziness.", type: "choice", options: yesNo },
  { field: "Tinnitus", title: "Do you hear ringing or buzzing in your ears?", helper: "Answer yes if it appears with the episode.", type: "choice", options: yesNo },
  { field: "Hypoacusis", title: "Does your hearing feel reduced?", helper: "This means temporary reduced hearing during symptoms.", type: "choice", options: yesNo },
  { field: "Diplopia", title: "Do you see double?", helper: "Double vision can be relevant for some migraine types.", type: "choice", options: yesNo },
  { field: "Defect", title: "Do you notice a visual field defect?", helper: "For example, a missing area in your vision.", type: "choice", options: yesNo },
  { field: "Ataxia", title: "Do you feel loss of coordination?", helper: "For example, trouble walking steadily or controlling movement.", type: "choice", options: yesNo },
  { field: "Conscience", title: "Do you feel confused or less alert?", helper: "Answer yes for altered awareness during episodes.", type: "choice", options: yesNo },
  { field: "Paresthesia", title: "Do you feel pins and needles?", helper: "This is tingling, prickling, or crawling sensations.", type: "choice", options: yesNo },
  { field: "DPF", title: "Do symptoms worsen with daily physical activity?", helper: "For example walking stairs or moving around makes it worse.", type: "choice", options: yesNo },
];

export const questionImages: Record<Field, string> = {
  Age: "/migraine/onboarding/age.png",
  Duration: "/migraine/onboarding/duration.png",
  Frequency: "/migraine/onboarding/frequency.png",
  Location: "/migraine/onboarding/location.png",
  Character: "/migraine/onboarding/character.png",
  Intensity: "/migraine/onboarding/intensity.png",
  Nausea: "/migraine/onboarding/nausea.png",
  Vomit: "/migraine/onboarding/vomit.png",
  Phonophobia: "/migraine/onboarding/phonophobia.png",
  Photophobia: "/migraine/onboarding/photophobia.png",
  Visual: "/migraine/onboarding/visual.png",
  Sensory: "/migraine/onboarding/sensory.png",
  Dysphasia: "/migraine/onboarding/dysphasia.png",
  Dysarthria: "/migraine/onboarding/dysarthria.png",
  Vertigo: "/migraine/onboarding/vertigo.png",
  Tinnitus: "/migraine/onboarding/tinnitus.png",
  Hypoacusis: "/migraine/onboarding/hypoacusis.png",
  Diplopia: "/migraine/onboarding/diplopia.png",
  Defect: "/migraine/onboarding/defect.png",
  Ataxia: "/migraine/onboarding/ataxia.png",
  Conscience: "/migraine/onboarding/conscience.png",
  Paresthesia: "/migraine/onboarding/paresthesia.png",
  DPF: "/migraine/onboarding/dpf.png",
};
