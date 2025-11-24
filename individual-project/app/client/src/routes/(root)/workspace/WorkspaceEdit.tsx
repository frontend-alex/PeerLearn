import { useMemo } from "react";
import Loading from "@/components/Loading";
import { useAuth } from "@/contexts/AuthContext";
import {
  useDeleteWorkspace,
  useUpdateWorkspace,
  useWorkspace,
} from "./hooks/use-workspaces";
import { useCurrentWorkspace } from "./hooks/use-current-workspace";
import {
  useDynamicPartials,
  type ModuleLoader,
} from "@/hooks/use-partials";
import type { UpdateWorkspaceSchemaType } from "@/utils/schemas/workspace/workspace.schema";

const partialModules = import.meta.glob(
  "./partials/*.tsx"
) as ModuleLoader;

const WorkspaceEdit = () => {
  const { user, isLoading, refetch } = useAuth();
  const { currentWorkspaceId } = useCurrentWorkspace();

  const { data: workspace, isPending: isWorkspacePending } =
    useWorkspace(currentWorkspaceId);

  const { mutateAsync: update, isPending: isWorkspaceUpdating } =
    useUpdateWorkspace(currentWorkspaceId);

  const { mutateAsync: del, isPending: isWorkspaceDeleting } =
    useDeleteWorkspace(currentWorkspaceId);

  const updateWorkspace = async (data: UpdateWorkspaceSchemaType) =>
    await update(data);

  const deleteWorkspace = async () => await del(null);

  const extraProps = useMemo(
    () => ({
      workspace: workspace?.data,
      updateWorkspace,
      deleteWorkspace,
      currentWorkspaceId,
      isWorkspaceDeleting,
      isWorkspaceUpdating,
    }),
    [
      workspace,
      updateWorkspace,
      deleteWorkspace,
      currentWorkspaceId,
      isWorkspaceDeleting,
      isWorkspaceUpdating,
    ]
  );

  const partials = useDynamicPartials({
    partialModules,
    user,
    reverseOrder: false,
  });

  if (isLoading || isWorkspacePending || !user || !workspace) {
    return <Loading />;
  }

  console.log('first')

  return (
    <div className="flex flex-col gap-3 max-w-3xl">
      {partials.map(({ key, Component }) => (
        <div key={key}>
          <Component
            user={user}
            refetchUser={refetch}
            {...extraProps}
          />
        </div>
      ))}
    </div>
  );
};

export default WorkspaceEdit;
