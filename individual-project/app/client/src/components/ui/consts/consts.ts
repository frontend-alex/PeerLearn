import { User, Users, BookOpen } from "lucide-react";

export const questionnaireSteps = [
  {
    id: 1,
    question: "What brings you here today?",
    description: "Help us understand your learning goals",
    options: [
      { id: "academic", label: "Academic studies", icon: BookOpen },
      { id: "professional", label: "Professional development", icon: User },
      { id: "personal", label: "Personal interest", icon: BookOpen },
      { id: "other", label: "Something else", icon: User },
    ],
  },
  {
    id: 2,
    question: "How do you prefer to learn?",
    description: "Choose your learning style",
    options: [
      { id: "solo", label: "Solo learning", icon: User },
      { id: "group", label: "Group study", icon: Users },
      { id: "both", label: "Both, depending on the topic", icon: Users },
    ],
  },
  {
    id: 3,
    question: "Will you be creating content?",
    description: "Let us know if you plan to share knowledge",
    options: [
      {
        id: "yes",
        label: "Yes, I'll create quizzes and content",
        icon: BookOpen,
      },
      { id: "maybe", label: "Maybe, I'm not sure yet", icon: User },
      { id: "no", label: "No, I'm here to learn only", icon: BookOpen },
    ],
  },
  {
    id: 4,
    question: "What should we call your workspace?",
    isForm: true,
  },
];

export const randomColors = [
  "#ef4444",
  "#3b82f6",
  "#22c55e",
  "#eab308",
  "#a855f7",
  "#ec4899",
  "#6366f1",
  "#14b8a6",
  "#f97316",
];

export const randomColorsDark = [
  "#ef4444", // red-500
  "#3b82f6", // blue-500
  "#22c55e", // green-500
  "#eab308", // yellow-500
  "#a855f7", // purple-500
  "#ec4899", // pink-500
  "#6366f1", // indigo-500
  "#14b8a6", // teal-500
  "#f97316", // orange-500
];

export const defaultWorkspaceColor = "#6366f1";
export const defaultDocumentColor = "#3b82f6";
