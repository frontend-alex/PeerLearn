import { toast } from "sonner";
import { Link } from "react-router-dom";
import { useForm } from "react-hook-form";
import { Check, ChevronsUpDown, House, Loader, Plus } from "lucide-react";

import { API } from "@/lib/config";
import { ROUTES } from "@/lib/router-paths";
import { useApiMutation } from "@/hooks/hook";
import { defaultWorkspaceColor } from "@/components/ui/consts/consts";
import { Button } from "@/components/ui/button";
import GlobalDialog from "@/components/dialogs/GlobalDialog";
import { useUserWorkspaces } from "@/routes/(root)/workspace/hooks/use-workspaces";
import { type Workspace, WorkspaceVisibility } from "@/types/workspace";
import { useCurrentWorkspace } from "@/routes/(root)/workspace/hooks/use-current-workspace";
import WorkspaceForm from "@/components/auth/forms/workspace/workspace-form-01";
import type { WorkspaceSchemaType } from "@/utils/schemas/workspace/workspace.schema";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DropdownSkeleton } from "@/components/skeletons/dropdown-skeleton";
import { VisibilityToggle } from "@/components/visibility-toggle";

const WorkspaceDropdown = () => {

  const {
    data: workspaces,
    isLoading: isWorkspacesLoading,
    error,
  } = useUserWorkspaces();

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

  const { currentWorkspaceId, hasWorkspaceContext } = useCurrentWorkspace();

  const handleWorkspace = async (data: WorkspaceSchemaType) =>
    createWorkspace(data);

  if (isWorkspacesLoading) return <DropdownSkeleton />;

  if (error) return null;

  const workspaceList = workspaces?.data || [];
  const currentWorkspace = workspaceList.find(
    (ws: Workspace) => ws.id === currentWorkspaceId
  );

  const colorFirst = currentWorkspace?.colorHex ?? defaultWorkspaceColor;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button
          variant="ghost"
          className="no-ring h-12 w-full flex items-center justify-between gap-3 px-3 data-[state=open]:bg-accent data-[state=open]:text-accent-foreground"
        >
          <div className="flex items-center gap-3">
            <div
              style={{ backgroundColor: colorFirst }}
              className="flex justify-center items-center p-2 rounded-sm"
            >
              <House className="h-4 w-4 text-white" />
            </div>
            <span className="capitalize truncate max-w-[100px]">
              {hasWorkspaceContext
                ? currentWorkspace?.name || "Unknown Workspace"
                : "Select Workspace"}
            </span>
            {hasWorkspaceContext && (
              <VisibilityToggle
                visibility={currentWorkspace?.visibility}
              />
            )}
          </div>
          <ChevronsUpDown className="h-4 w-4" />
        </Button>
      </DropdownMenuTrigger>

      {isWorkspacesLoading ? (
        <Loader />
      ) : (
        <DropdownMenuContent className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg capitalize">
          {workspaceList?.length > 0 ? (
            workspaceList.map((workspace: Workspace) => {
              const colors = workspace.colorHex ?? defaultWorkspaceColor;

              return (
                <Link key={workspace.id} to={ROUTES.AUTHENTICATED.WORKSPACE(workspace.id)}>
                  <DropdownMenuItem
                    className="flex items-center justify-between gap-3 w-full"
                  >
                    <div className="flex items-center gap-3 justify-between w-full">
                      <div className="flex items-center gap-3">
                        <div
                          style={{ backgroundColor: colors }}
                          className="flex justify-center items-center p-1 rounded-sm"
                        >
                          <House className="text-white" />
                        </div>
                        <span className="font-medium text-xs max-w-[130px] truncate">
                          {workspace.name}
                        </span>
                      </div>
                      <VisibilityToggle
                        visibility={workspace.visibility}
                      />
                    </div>

                    {workspace.id === currentWorkspaceId && <Check />}
                  </DropdownMenuItem>
                </Link>
              );
            })
          ) : (
            <DropdownMenuItem>
              <span className="capitalize text-center text-sm">
                No workspaces found.
              </span>
            </DropdownMenuItem>
          )}

          {/* Divider */}
          <DropdownMenuSeparator />

          {/* Create Workspace Button */}
          <GlobalDialog
            content={
              <WorkspaceForm
                workspaceForm={form}
                isPending={isWorkspacePending}
              />
            }
            onConfirm={() => handleWorkspace(form.getValues())}
          >
            <Button
              variant={"ghost"}
              className="flex items-center justify-center w-full"
            >
              <Plus className="h-4 w-4" />
              Create Workspace
            </Button>
          </GlobalDialog>
        </DropdownMenuContent>
      )}
    </DropdownMenu>
  );
};

export default WorkspaceDropdown;
