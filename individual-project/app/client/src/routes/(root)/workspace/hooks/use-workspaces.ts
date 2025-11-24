import { toast } from "sonner";
import { API } from "@/lib/config";
import type { Workspace } from "@/types/workspace";
import type { ApiSuccessResponse } from "@/types/api";
import { useApiMutation, useApiQuery } from "@/hooks/hook";
import type { UpdateWorkspaceSchemaType } from "@/utils/schemas/workspace/workspace.schema";
import type { NavigateFunction } from "react-router-dom";
import { ROUTES } from "@/lib/router-paths";

export type UpdateWorkspaceFn = (data: UpdateWorkspaceSchemaType) => Promise<ApiSuccessResponse<Workspace>>;

export const useUserWorkspaces = () => {
  return useApiQuery<Workspace[]>(
    ["user-workspaces"],
    API.ENDPOINTS.WORKSPACE.USER_WORKSPACES,
    {
      staleTime: 2 * 60 * 1000, // 2 minutes
    }
  );
};

export const useWorkspace = (workspaceId: number | undefined) => {
  return useApiQuery<Workspace>(
    ["workspace", workspaceId],
    API.ENDPOINTS.WORKSPACE.Id(workspaceId),
    {
      staleTime: 2 * 60 * 1000, // 2 minutes
      enabled: !!workspaceId,
    }
  );
};

export const useUpdateWorkspace = (workspaceId: number) => {
  return useApiMutation<Workspace>(
    "PUT",
    API.ENDPOINTS.WORKSPACE.Id(workspaceId),
    {
      invalidateQueries: [["workspace", workspaceId], ["user-workspaces"]],
      onSuccess: () => {
      },
      onError: (error) => {
        toast.error(error.response?.data.message);
      },
    }
  );  
};


export const useDeleteWorkspace = (workspaceId: number, navigate: NavigateFunction) => {
  return useApiMutation<null>(
    "DELETE",
    API.ENDPOINTS.WORKSPACE.Id(workspaceId),
    {
      invalidateQueries: [["workspace", workspaceId], ["user-workspaces"]],
      onSuccess: (data) => {
        toast.success(data.message);
        navigate(ROUTES.AUTHENTICATED.DASHBOARD)
      },
      onError: (error) => {
        toast.error(error.response?.data.message);
      },
    }
  )
}