import { useForm } from "react-hook-form";
import { LoaderCircle } from "lucide-react";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { zodResolver } from "@hookform/resolvers/zod";
import ColorPopover from "@/components/dropdowns/color-dropdown";
import { defaultWorkspaceColor } from "@/components/ui/consts/consts";
import { type Workspace, WorkspaceVisibility } from "@/types/workspace";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import {
  type UpdateWorkspaceSchemaType,
  updateWorkspaceSchema,
} from "@/utils/schemas/workspace/workspace.schema";
import {
  PartialContainer,
  PartialHeader,
  PartialSeparator,
  Section,
  SectionContent,
  SectionDescription,
  SectionHeading,
  SectionLabel,
} from "@/components/ui/partial";

import type { UpdateWorkspaceFn } from "../hooks/use-workspaces";
import { VisibilityToggle } from "@/components/visibility-toggle";

type PartialProps = {
  workspace: Workspace;
  isWorkspaceUpdating: boolean;
  updateWorkspace: UpdateWorkspaceFn;
};

const WorkspaceInfo: React.FC<PartialProps> = ({
  workspace,
  updateWorkspace,
  isWorkspaceUpdating,
}) => {
  const form = useForm<UpdateWorkspaceSchemaType>({
    resolver: zodResolver(updateWorkspaceSchema),
    defaultValues: {
      description: workspace.description,
      colorHex: workspace.colorHex,
      name: workspace.name,
      visibility: WorkspaceVisibility.PUBLIC,
    },
  });

  return (
    <PartialContainer>
      <PartialHeader
        title={workspace.name}
        description={workspace.description}
        titleClassName="text-2xl font-bold"
        descriptionClassName="text-muted-foreground text-balance"
      />

      <PartialSeparator />

      <Form {...form}>
        <form
          onSubmit={form.handleSubmit(updateWorkspace)}
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
                updateWorkspace={updateWorkspace}
                hexColor={workspace.colorHex || defaultWorkspaceColor}
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
                  updateWorkspace({
                    visibility:
                      newVis === WorkspaceVisibility.PRIVATE
                        ? WorkspaceVisibility.PRIVATE
                        : WorkspaceVisibility.PUBLIC,
                  })
                }
                visibility={workspace.visibility}
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
                name="name"
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

          <Section>
            <SectionLabel>
              <SectionHeading>Description</SectionHeading>
              <SectionDescription>
                Provide a short summary of what this workspace is about.
              </SectionDescription>
            </SectionLabel>

            <SectionContent>
              <FormField
                control={form.control}
                name="description"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormControl>
                      <Input
                        type="text"
                        placeholder="Workspace description"
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
            <Button type="submit" disabled={isWorkspaceUpdating}>
              {isWorkspaceUpdating ? (
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

export default WorkspaceInfo;
