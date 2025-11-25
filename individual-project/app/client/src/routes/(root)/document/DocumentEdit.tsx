import { useMemo } from "react";

import { useNavigate, useParams } from "react-router-dom";

import Loading from "@/components/Loading";
import { useAuth } from "@/contexts/AuthContext";
import { type ModuleLoader, useDynamicPartials } from "@/hooks/use-partials";
import type { UpdateDocumentSchemaType } from "@/utils/schemas/document/document.schema";

import { useDeleteDocument, useDocument, useUpdateDocument } from "./hooks/use-document";

const partialModules = import.meta.glob(
  "./partials/*.tsx"
) as ModuleLoader;

const WorkspaceEdit = () => {

  const navigate = useNavigate();

  const { documentId } = useParams<{ documentId: string }>()

  const { user, isLoading, refetch } = useAuth();

  const { data: document, isPending: isDocumentPending } = useDocument(documentId ?? "");

  const { mutateAsync: update, isPending: isDocumentUpdating } = useUpdateDocument(documentId ?? "");

  const { mutateAsync: del, isPending: isDocumentDeleting } = useDeleteDocument(documentId ?? "", navigate);

  const updateDocument = async (data: UpdateDocumentSchemaType) => await update(data);
  const deleteDocument = async () => await del(null);

  const partials = useDynamicPartials({
    partialModules,
    user,
    reverseOrder: false,
  });

    const extraProps = useMemo(
      () => ({
        document: document?.data,
        updateDocument,
        deleteDocument,
        isDocumentUpdating,
        isDocumentDeleting
      }),
      [
        document,
        updateDocument,
        deleteDocument,
        isDocumentUpdating,
        isDocumentDeleting
      ]
    );

  if (isLoading || !user || isDocumentPending) {
    return <Loading />;
  }

  return (
    <div className="flex flex-col gap-10 max-w-3xl">
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
