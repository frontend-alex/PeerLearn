import { useApiQuery, useApiMutation } from "@/hooks/hook";
import { API } from "@/lib/config";
import { ROUTES } from "@/lib/router-paths";
import type { ApiSuccessResponse } from "@/types/api";
import type { Document } from "@/types/workspace";
import type { UpdateDocumentSchemaType } from "@/utils/schemas/document/document.schema";
import type { NavigateFunction } from "react-router-dom";
import { toast } from "sonner";
import { useCurrentWorkspace } from "../../workspace/hooks/use-current-workspace";


export const useDocument = (documentId: string) => {
  return useApiQuery<Document>(
    ["document", documentId],
    API.ENDPOINTS.DOCUMENTS.Id(Number(documentId)),
    {
      staleTime: 2 * 60 * 1000,
      enabled: !!documentId,
    }
  );
};


export type UpdateDocumentFn = (data: UpdateDocumentSchemaType) => Promise<ApiSuccessResponse<Document>>;

export const useUpdateDocument = (documentId: string) => {
  return useApiMutation<Document>(
    "PUT",
    API.ENDPOINTS.DOCUMENTS.Id(Number(documentId)),
    {
      invalidateQueries: [["document", documentId], ["workspace"]],
    }
  );
};

export type DeleteDocumentFn = (args: null) => Promise<ApiSuccessResponse<unknown>>

export const useDeleteDocument = (documentId: string, navigate: NavigateFunction) => {

  const { currentWorkspaceId } = useCurrentWorkspace();

  return useApiMutation<Document>(
    "DELETE",
    API.ENDPOINTS.DOCUMENTS.Id(Number(documentId)),
    {
      invalidateQueries: [["document", documentId], ["workspace"]],
      onSuccess: (data) => {
        toast.success(data.message);
        navigate(ROUTES.AUTHENTICATED.WORKSPACE(currentWorkspaceId));
      }
    }
  );
};



