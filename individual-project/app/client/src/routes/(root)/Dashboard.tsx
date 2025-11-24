import { toast } from "sonner";
import { Plus } from "lucide-react";
import { useForm } from "react-hook-form";

import { API } from "@/lib/config";
import { ROUTES } from "@/lib/router-paths";
import { useApiMutation } from "@/hooks/hook";
import { Button } from "@/components/ui/button";
import { defaultWorkspaceColor } from "@/components/ui/consts/consts";
import GlobalDialog from "@/components/dialogs/GlobalDialog";
import { useUserWorkspaces } from "@/routes/(root)/workspace/hooks/use-workspaces";
import { type Workspace, WorkspaceVisibility } from "@/types/workspace";
import WorkspaceForm from "@/components/auth/forms/workspace/workspace-form-01";
import type { WorkspaceSchemaType } from "@/utils/schemas/workspace/workspace.schema";
import ColoredCard, { ColoredCardSkeleton } from "@/components/cards/colored-card";
import {
  Accordion,
  AccordionContent,
  AccordionItem,
  AccordionTrigger,
} from "@/components/ui/accordion";
import { VisibilityToggle } from "@/components/visibility-toggle";

const Dashboard = () => {
  const { data: workspacesResponse, isLoading } = useUserWorkspaces();

  const workspaces = workspacesResponse?.data || [];

  const form = useForm<WorkspaceSchemaType>({
    defaultValues: {
      name: "",
      visibility: WorkspaceVisibility.PUBLIC,
    },
  });

  const { mutateAsync: createWorkspace, isPending: isWorkspacePending } =
    useApiMutation<Workspace>("POST", API.ENDPOINTS.WORKSPACE.WORKSPACE, {
      invalidateQueries: [["user-workspaces"]],
      onSuccess: (data) => {
        if (data.success && data.data) {
          toast.success(data.message);
        }
      },
    });

  const handleWorkspace = async (data: WorkspaceSchemaType) =>
    createWorkspace(data);

  return (
    <div className="h-[50dvh] flex flex-col gap-10 items-center justify-center">
      <h1 className="text-xl">Hello There!</h1>

      <Accordion
        type="single"
        collapsible
        className="w-full"
        defaultValue="workspaces"
      >
        <AccordionItem value="workspaces">
          <div className="flex items-center justify-between">
            <AccordionTrigger className="flex-1">
              <span>Your Workspace(s): {workspaces.length}</span>
            </AccordionTrigger>
            <GlobalDialog
              content={
                <WorkspaceForm
                  workspaceForm={form}
                  isPending={isWorkspacePending}
                />
              }
              onConfirm={() => handleWorkspace(form.getValues())}
              hideFooter={false}
            >
              <Button
                variant={"ghost"}
                size="sm"
                onClick={(e) => e.stopPropagation()}
              >
                <Plus className="h-4 w-4" />
              </Button>
            </GlobalDialog>
          </div>
            <AccordionContent className="w-full">
              <div className="grid grid-cols-3 lg:grid-cols-5 gap-4 w-full">
                {isLoading ? (
                  Array.from({ length: 3 }).map((_, index) => (
                    <ColoredCardSkeleton key={index} />
                  ))
                ) : workspaces.length === 0 ? (
                  <span className="text-center text-sm col-span-full text-muted-foreground">
                    No workspaces yet.
                  </span>
                ) : (
                  workspaces.map((workspace) => (
                    <ColoredCard
                      key={workspace.id}
                      title={workspace.name}
                      color={workspace.colorHex ?? defaultWorkspaceColor}
                      to={ROUTES.AUTHENTICATED.WORKSPACE(workspace.id)}
                      titleClassName="truncate max-w-[100px]"
                      className="hover:bg-muted"
                      headerIcon={
                        <VisibilityToggle
                          visibility={workspace.visibility}
                          className="size-5"
                        />
                      }
                    />
                  ))
                )}
              </div>
            </AccordionContent>
        </AccordionItem>
      </Accordion>
    </div>
  );
};

export default Dashboard;
