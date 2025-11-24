import * as React from "react";
import { Separator } from "@/components/ui/separator";
import { cn } from "@/lib/utils";

/* -----------------------------------------------------
 * PARTIAL CONTAINER
 * --------------------------------------------------- */
interface PartialContainerProps extends React.HTMLAttributes<HTMLDivElement> {
  children: React.ReactNode;
}

const PartialContainer = React.forwardRef<HTMLDivElement, PartialContainerProps>(
  ({ className, children, ...props }, ref) => {
    return (
      <div ref={ref} className={cn("flex flex-col gap-5", className)} {...props}>
        {children}
      </div>
    );
  }
);
PartialContainer.displayName = "PartialContainer";

/* -----------------------------------------------------
 * PARTIAL HEADER
 * --------------------------------------------------- */
interface PartialHeaderProps extends React.HTMLAttributes<HTMLDivElement> {
  title: string;
  description?: string;
  titleClassName?: string;
  descriptionClassName?: string;
}

const PartialHeader = React.forwardRef<HTMLDivElement, PartialHeaderProps>(
  (
    {
      className,
      title,
      description,
      titleClassName,
      descriptionClassName,
      ...props
    },
    ref
  ) => {
    return (
      <div ref={ref} className={cn("", className)} {...props}>
        <h2 className={cn("text-xl font-semibold", titleClassName)}>
          {title}
        </h2>

        {description && (
          <p
            className={cn(
              "text-sm text-stone-400 mt-1",
              descriptionClassName
            )}
          >
            {description}
          </p>
        )}
      </div>
    );
  }
);
PartialHeader.displayName = "PartialHeader";

/* -----------------------------------------------------
 * PARTIAL SEPARATOR
 * --------------------------------------------------- */
const PartialSeparator = React.forwardRef<
  React.ElementRef<typeof Separator>,
  React.ComponentProps<typeof Separator>
>(({ className, ...props }, ref) => {
  return (
    <Separator
      ref={ref}
      className={cn("bg-accent mt-2 mb-2", className)}
      {...props}
    />
  );
});
PartialSeparator.displayName = "PartialSeparator";

/* -----------------------------------------------------
 * NEXT-GEN SECTION SYSTEM
 * --------------------------------------------------- */

// A semantic wrapper representing a single section block
const Section = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div ref={ref} className={cn("grid grid-cols-3 gap-8", className)} {...props} />
));
Section.displayName = "Section";

// Column: section label
const SectionLabel = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div
    ref={ref}
    className={cn("space-y-1 col-span-1", className)}
    {...props}
  />
));
SectionLabel.displayName = "SectionLabel";

// Label heading
const SectionHeading = React.forwardRef<
  HTMLHeadingElement,
  React.HTMLAttributes<HTMLHeadingElement>
>(({ className, ...props }, ref) => (
  <h3 ref={ref} className={cn("text-base font-medium", className)} {...props} />
));
SectionHeading.displayName = "SectionHeading";

// Label description
const SectionDescription = React.forwardRef<
  HTMLParagraphElement,
  React.HTMLAttributes<HTMLParagraphElement>
>(({ className, ...props }, ref) => (
  <p
    ref={ref}
    className={cn("text-sm text-muted-foreground", className)}
    {...props}
  />
));
SectionDescription.displayName = "SectionDescription";

// Column: section content
const SectionContent = React.forwardRef<
  HTMLDivElement,
  React.HTMLAttributes<HTMLDivElement>
>(({ className, ...props }, ref) => (
  <div ref={ref} className={cn("col-span-2 space-y-8", className)} {...props} />
));
SectionContent.displayName = "SectionContent";

/* -----------------------------------------------------
 * EXPORTS
 * --------------------------------------------------- */
export {
  PartialContainer,
  PartialHeader,
  PartialSeparator,
  Section,
  SectionLabel,
  SectionHeading,
  SectionDescription,
  SectionContent,
};
