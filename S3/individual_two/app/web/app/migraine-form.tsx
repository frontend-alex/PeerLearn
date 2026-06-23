"use client";
import { motion, type Transition } from "framer-motion";
import { useState } from "react";

import MigraineIllustration  from "@/components/migrane-illustration";

import { Button } from "@/components/ui/button";
import { CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Progress } from "@/components/ui/progress";
import { cn } from "@/lib/utils";
import {
  canContinue,
  getProgress,
  handleBack,
  handleNext,
  handleRestart,
  initialMigraineFormState,
  questions,
  updateAgeInput,
  updateAnswer,
} from "@/utils/migraine-form";
import { ArrowLeft } from "lucide-react";

const slideHeight = 288;
const questionTransition: Transition = {
  duration: 0.85,
  ease: [0.22, 1, 0.36, 1],
};

export function MigraineForm() {
  const [form, setForm] = useState(initialMigraineFormState);

  const isLastStep = form.step === questions.length - 1;
  const progress = getProgress(form);
  const isContinueDisabled = form.isSubmitting || !canContinue(form);
  const visibleStep = form.result ? questions.length : form.step;
  const slides = [...questions, null];

  return (
    <main className="min-h-screen flex items-center ">
      <section className="w-full flex items-center h-screen">
        <div className="flex flex-col gap-10 max-w-xl mx-auto px-5">
          <div className="absolute bottom-5 left-5 flex flex-col gap-3">
            <p className="text-sm">
              Steps {form.step + 1} of {questions.length}
            </p>
            <Progress value={progress} className="h-1" />
          </div>

          <div className="relative h-72 overflow-hidden">
            <motion.div
              animate={{ y: -visibleStep * slideHeight }}
              transition={questionTransition}
            >
              {slides.map((slide, index) => {
                const isVisible = index === visibleStep;

                return (
                  <div
                    key={slide?.field ?? "result"}
                    aria-hidden={!isVisible}
                    className={cn(
                      "flex h-72 flex-col justify-center space-y-4",
                      !isVisible && "pointer-events-none",
                    )}
                  >
                    {slide ? (
                      <>
                        <div className="space-y-2">
                          <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                            {slide.field}
                          </p>
                          <CardTitle className="text-xl leading-tight">
                            {slide.title}
                          </CardTitle>
                          <p className="text-sm leading-6 text-muted-foreground">
                            {slide.helper}
                          </p>
                        </div>

                        {slide.type === "number" ? (
                          <Input
                            type="number"
                            min={slide.min}
                            max={slide.max}
                            value={form.ageInput}
                            disabled={!isVisible}
                            onChange={(event) =>
                              updateAgeInput(setForm, event.target.value)
                            }
                            className="h-9 w-full text-sm [appearance:textfield] [&::-webkit-inner-spin-button]:appearance-none [&::-webkit-outer-spin-button]:appearance-none"
                          />
                        ) : (
                          <div className="flex flex-wrap items-center gap-2">
                            {slide.options.map((option) => {
                              const isSelected =
                                form.answers[slide.field] === option.value;

                              return (
                                <Button
                                  key={option.value}
                                  type="button"
                                  variant={isSelected ? "default" : "outline"}
                                  size="sm"
                                  disabled={!isVisible}
                                  onClick={() =>
                                    updateAnswer(
                                      setForm,
                                      slide.field,
                                      option.value,
                                    )
                                  }
                                  className={cn(
                                    "h-auto w-max justify-start rounded-xl px-3 py-2 text-left",
                                    !isSelected && "bg-background font-medium",
                                  )}
                                >
                                  {option.label}
                                </Button>
                              );
                            })}
                          </div>
                        )}

                        {index === form.step && form.error ? (
                          <div className="rounded-md border px-3 py-2 text-sm text-muted-foreground">
                            {form.error}
                          </div>
                        ) : null}
                      </>
                    ) : (
                      <div className="space-y-4">
                        <div className="space-y-2">
                          <p className="text-xs font-medium uppercase tracking-wider text-muted-foreground">
                            Prediction ready
                          </p>
                          <CardTitle className="text-xl">
                            {form.result?.type}
                          </CardTitle>
                          <p className="text-sm leading-6 text-muted-foreground">
                            This is a model prediction based on your answers. It
                            is not a medical diagnosis.
                          </p>
                        </div>
                        <Button
                          type="button"
                          size="sm"
                          disabled={!isVisible}
                          onClick={() => handleRestart(setForm)}
                        >
                          Start again
                        </Button>
                      </div>
                    )}
                  </div>
                );
              })}
            </motion.div>
            <div className="pointer-events-none absolute inset-x-0 top-0 h-10 bg-gradient-to-b from-bg-background to-transparent backdrop-blur-[2px] [mask-image:linear-gradient(to_bottom,black,transparent)]" />
            <div className="pointer-events-none absolute inset-x-0 bottom-0 h-10 bg-gradient-to-t from-bg-background to-transparent backdrop-blur-[2px] [mask-image:linear-gradient(to_top,black,transparent)]" />
          </div>
          {!form.result ? (
            <div className="flex flex-wrap gap-2 items-center justify-end">
              {!form.result && form.step > 0 ? (
                <Button
                  type="button"
                  variant="ghost"
                  size="sm"
                  onClick={() => handleBack(setForm)}
                  disabled={form.isSubmitting}
                  className="w-max py-1"
                >
                  <ArrowLeft /> Back
                </Button>
              ) : null}

              <Button
                type="button"
                size="sm"
                onClick={() => handleNext(form, setForm)}
                disabled={isContinueDisabled}
              >
                {form.isSubmitting
                  ? "Predicting..."
                  : isLastStep
                    ? "Show result"
                    : "Continue"}
              </Button>
            </div>
          ) : null}
        </div>
      </section>
      {/* <MigraineIllustration visibleStep={visibleStep} slides={slides} /> */}
      
    </main>
  );
}
