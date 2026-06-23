import { motion } from "framer-motion";

import { questionImages, type Question } from "@/const/migraine-questions";

const MigraineIllustration = ({ visibleStep, slides }: { visibleStep: number; slides: (Question | null)[] }) => {
  return (
    <section className="hidden lg:flex h-screen w-full items-center justify-center overflow-hidden">
      <div className="relative flex h-full w-full items-center justify-center">
        {slides.map((slide, index) => {
          if (!slide) return null;

          const isVisible = index === visibleStep;

          return (
            <motion.div
              key={slide.field}
              className="absolute"
              initial={false}
              animate={
                isVisible
                  ? {
                      opacity: 1,
                    }
                  : {
                      opacity: 0,
                    }
              }
              transition={{ duration: 0.2 }}
            >
              <div className="flex h-[520px] w-[520px] items-center justify-center">
                <img
                  src={questionImages[slide.field]}
                  alt=""
                  className="h-full w-full object-contain"
                  draggable={false}
                />
              </div>
            </motion.div>
          );
        })}
      </div>
    </section>
  );
}

export default MigraineIllustration;
