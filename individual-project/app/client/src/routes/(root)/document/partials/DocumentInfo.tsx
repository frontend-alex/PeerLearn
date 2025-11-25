import { useForm } from "react-hook-form";
import { LoaderCircle } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { zodResolver } from "@hookform/resolvers/zod";
import ColorPopover from "@/components/dropdowns/color-dropdown";
import { VisibilityToggle } from "@/components/visibility-toggle";
import { defaultWorkspaceColor } from "@/components/ui/consts/consts";
import { type Document, WorkspaceVisibility } from "@/types/workspace";
import { Form, FormControl, FormField, FormItem, FormMessage } from "@/components/ui/form";
import { type UpdateDocumentSchemaType, updateDocumentSchema } from "@/utils/schemas/document/document.schema";
import { PartialContainer, PartialHeader, PartialSeparator, Section, SectionContent, SectionDescription, SectionHeading, SectionLabel } from "@/components/ui/partial";

import type { UpdateDocumentFn } from "../hooks/use-document";

type PartialProps = {
  document: Document;
  isDocumentUpdating: boolean;
  updateDocument: UpdateDocumentFn;
};

const DocumentInfo: React.FC<PartialProps> = ({
  document,
  updateDocument,
  isDocumentUpdating,
}) => {
  const form = useForm<UpdateDocumentSchemaType>({
    resolver: zodResolver(updateDocumentSchema),
    defaultValues: {
      colorHex: document.colorHex,
      title: document.title,
      visibility: document.visibility,
    },
  });

  return (
    <PartialContainer>
      <PartialHeader
        title={document.title}
        titleClassName="text-2xl font-bold"
        descriptionClassName="text-muted-foreground text-balance"
      />

      <PartialSeparator />

      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(updateDocument)}
          className="space-y-10"
        >
          <Section>
            <SectionLabel>
              <SectionHeading>Appearance</SectionHeading>
              <SectionDescription>
                Customize the visual identity of your workspace.
              </SectionDescription>
            </SectionLabel>

            <SectionContent className="flex justify-end">
              <ColorPopover
                updateDocument={updateDocument}
                hexColor={document.colorHex || defaultWorkspaceColor}
              />
            </SectionContent>
          </Section>

          <Section>
            <SectionLabel>
              <SectionHeading>Visibility</SectionHeading>
              <SectionDescription>
                Control who can access and view your workspace.
              </SectionDescription>
            </SectionLabel>

            <SectionContent className="flex justify-end">
              <VisibilityToggle
                onChange={(newVis) =>
                  updateDocument({
                    visibility:
                      newVis === WorkspaceVisibility.PRIVATE
                        ? WorkspaceVisibility.PRIVATE
                        : WorkspaceVisibility.PUBLIC,
                  })
                }
                visibility={document.visibility}
              >
                {(Icon) => (
                  <Button disabled={true}>
                    <Icon className="mr-2 h-4 w-4" />
                    Change visibility
                  </Button>
                )}
              </VisibilityToggle>
            </SectionContent>
          </Section>

          <Section>
            <SectionLabel>
              <SectionHeading>Name</SectionHeading>
              <SectionDescription>
                Set a unique, recognizable name for your workspace.
              </SectionDescription>
            </SectionLabel>

            <SectionContent>
              <FormField
                control={form.control}
                name="title"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormControl>
                      <Input
                        type="text"
                        placeholder="Workspace name"
                        className="input no-ring"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </SectionContent>
          </Section>

          <div className="flex justify-end">
            <Button type="submit" disabled={isDocumentUpdating}>
              {isDocumentUpdating ? (
                <div className="flex items-center gap-3">
                  <LoaderCircle className="animate-spin" />
                  <p>Saving...</p>
                </div>
              ) : (
                "Save"
              )}
            </Button>
          </div>
        </form>
      </Form>
    </PartialContainer>
  );
};

export default DocumentInfo;
