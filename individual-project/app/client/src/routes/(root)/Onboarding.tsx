import WorkspaceForm from "@/components/auth/forms/workspace/workspace-form-01";

import { API } from "@/lib/config";
import { useForm } from "react-hook-form";
import { ROUTES } from "@/lib/router-paths";
import { getRandomColor } from "@/lib/utils";
import { useApiMutation } from "@/hooks/hook";
import { useNavigate } from "react-router-dom";
import { useAuth } from "@/contexts/AuthContext";
import { useTheme } from "@/contexts/ThemeContext";
import { useCallback, useMemo, useState } from "react";
import { ChevronRight, LoaderCircle } from "lucide-react";
import { motion, AnimatePresence } from "motion/react";
import { questionnaireSteps, randomColors } from "@/components/ui/consts/consts";
import { WorkspaceVisibility, type Workspace } from "@/types/workspace";
import type { WorkspaceSchemaType } from "@/utils/schemas/workspace/workspace.schema";



const OnboardingPage = () => {
  const navigate = useNavigate();

  const { user, refetch } = useAuth();
  const { theme } = useTheme();

  const form = useForm<WorkspaceSchemaType>({
    defaultValues: {
      name: `${user?.username}'s Workspace` || "",
      visibility: WorkspaceVisibility.PUBLIC,
    },
  });

  const [currentStep, setCurrentStep] = useState(0);
  const [answers, setAnswers] = useState<Record<number, string>>({});

  const handleOptionSelect = useCallback(
    (optionId: string) => {
      setAnswers((prev) => ({ ...prev, [currentStep + 1]: optionId }));
      if (currentStep < questionnaireSteps.length - 1) {
        setTimeout(() => {
          setCurrentStep((step) => step + 1);
        }, 300);
      }
    },
    [currentStep]
  );

  const prevStep = useCallback(() => {
    setCurrentStep((step) => (step > 0 ? step - 1 : step));
  }, []);

  const { mutateAsync: createWorkspace, isPending: isWorkspacePending } =
    useApiMutation<Workspace>(
      "POST",
      API.ENDPOINTS.WORKSPACE.WORKSPACE,
      {
        onSuccess: (data) => {
          if (data.success && data.data) {
            localStorage.setItem("currentWorkspaceId", data.data.id.toString());
            refetch();
            navigate(ROUTES.AUTHENTICATED.DASHBOARD);
          }
        },
      }
    );

  const handleWorkspace = useCallback(
    async (data: WorkspaceSchemaType) => createWorkspace(data),
    [createWorkspace]
  );

  const currentQuestion = questionnaireSteps[currentStep];

  const optionColors = useMemo(() => {
    const options = questionnaireSteps[currentStep].options;
    if (!options) return [] as string[];
    return options.map(() => getRandomColor(randomColors, theme));
  }, [currentStep, theme]);

  return (
    <div className="h-dvh flex flex-col">
      {/* Flat Progress Indicator */}
      <div className="flex">
        {questionnaireSteps.map((_, index) => (
          <motion.div
            key={index}
            className={`h-1 flex-1  ${
              index <= currentStep ? "bg-primary" : "bg-muted"
            }`}
            initial={{ scaleX: 0 }}
            animate={{ scaleX: index <= currentStep ? 1 : 0 }}
            transition={{ duration: 0.3, delay: index * 0.1 }}
          />
        ))}
      </div>

      {/* Main Content */}
      <div className="flex flex-1 items-center justify-center">
        <div className="w-full max-w-2xl">
          <AnimatePresence mode="wait">
            <motion.div
              key={currentStep}
              initial={{ opacity: 0, x: 50 }}
              animate={{ opacity: 1, x: 0 }}
              exit={{ opacity: 0, x: -50 }}
              transition={{ duration: 0.5, ease: "easeInOut" }}
              className="text-center"
            >
              <motion.h1
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.2, duration: 0.5 }}
                className="text-3xl font-bold mb-4"
              >
                {currentQuestion.question}
              </motion.h1>

              <motion.p
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.3, duration: 0.5 }}
                className="text-lg text-muted-foreground mb-8"
              >
                {currentQuestion.description}
              </motion.p>

              {/* Options Grid or Form */}
              {currentQuestion.isForm ? (
                <motion.div
                  initial={{ y: 20, opacity: 0 }}
                  animate={{ y: 0, opacity: 1 }}
                  transition={{ delay: 0.4, duration: 0.5 }}
                  className="max-w-md mx-auto mb-8"
                >
                  <WorkspaceForm
                    workspaceForm={form}
                    isPending={isWorkspacePending}
                  />
                </motion.div>
              ) : (
                <motion.div
                  initial={{ y: 20, opacity: 0 }}
                  animate={{ y: 0, opacity: 1 }}
                  transition={{ delay: 0.4, duration: 0.5 }}
                  className="grid grid-cols-1 md:grid-cols-2 gap-4 mb-8"
                >
                  {currentQuestion.options?.map((option, index) => {
                    const isSelected = answers[currentStep + 1] === option.id;
                    const Icon = option.icon;
                    const color = optionColors[index];
                    return (
                      <motion.div
                        key={option.id}
                        whileHover={{ scale: 1.02 }}
                        whileTap={{ scale: 0.98 }}
                        onClick={() => handleOptionSelect(option.id)}
                        style={{ "--dynamic-bg": color } as React.CSSProperties}
                        className={`
                            relative p-6 rounded-xl border-2 transition-all duration-300 cursor-pointer
                            ${
                              isSelected
                                ? "border-[var(--dynamic-bg)] bg-[var(--dynamic-bg)]/5 shadow-md"
                                : "border-accent hover:border-[var(--dynamic-bg)] hover:bg-accent/5"
                            }
                          `}
                      >
                        <div className="flex items-center gap-4">
                          <div
                            style={
                              { "--dynamic-bg": color } as React.CSSProperties
                            }
                            className={`bg-[var(--dynamic-bg)] p-3 rounded-lg`}
                          >
                            <Icon className="size-6" />
                          </div>
                          <span className={`text-lg font-medium`}>
                            {option.label}
                          </span>
                        </div>
                      </motion.div>
                    );
                  })}
                </motion.div>
              )}

              {/* Navigation Buttons */}
              <motion.div
                initial={{ y: 20, opacity: 0 }}
                animate={{ y: 0, opacity: 1 }}
                transition={{ delay: 0.5, duration: 0.5 }}
                className="flex justify-center gap-4"
              >
                {currentStep > 0 && (
                  <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    onClick={prevStep}
                    disabled={currentStep === 0 || isWorkspacePending}
                    className="px-6 py-2 border border-muted rounded-lg hover:bg-muted transition-colors"
                  >
                    Previous
                  </motion.button>
                )}

                {currentStep < questionnaireSteps.length - 1 ? (
                  ""
                ) : (
                  <motion.button
                    whileHover={{ scale: 1.05 }}
                    whileTap={{ scale: 0.95 }}
                    disabled={isWorkspacePending}
                    onClick={() => handleWorkspace(form.getValues())}
                    // onClick={() => form.handleSubmit(handleWorkspace)()}
                    className={`px-6 py-2 rounded-lg transition-colors flex items-center gap-2 ${
                      !isWorkspacePending
                        ? "bg-primary text-primary-foreground hover:bg-primary/90"
                        : "bg-muted text-muted-foreground cursor-not-allowed"
                    }`}
                  >
                    {isWorkspacePending ? <LoaderCircle /> : ""}{" "}
                    {isWorkspacePending ? "Creating..." : "Complete Setup"}
                    <ChevronRight className="size-4" />
                  </motion.button>
                )}
              </motion.div>
            </motion.div>
          </AnimatePresence>
        </div>
      </div>
    </div>
  );
};

export default OnboardingPage;
