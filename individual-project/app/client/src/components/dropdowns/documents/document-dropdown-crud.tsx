import { toast } from "sonner";
import { useNavigate, useParams, Link, useLocation } from "react-router-dom";
import {
  Download,
  EllipsisVerticalIcon,
  FileText,
  FileType,
  Settings,
  Trash,
  Upload,
} from "lucide-react";

import { cn } from "@/lib/utils";
import { API } from "@/lib/config";
import { ROUTES } from "@/lib/router-paths";
import { useApiMutation } from "@/hooks/hook";
import { Button } from "@/components/ui/button";
import { DocumentKind, type Workspace } from "@/types/workspace";
import DeleteDialog from "@/components/dialogs/DeleteDialog";
import { useDocumentImportExport } from "@/utils/import-export";
import { useCurrentWorkspace } from "@/routes/(root)/workspace/hooks/use-current-workspace";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuSub,
  DropdownMenuSubContent,
  DropdownMenuSubTrigger,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";

const ManageDocumentDropdown = ({
  className,
  variant = "secondary",
}: {
  className?: string;
  variant?:
    | "default"
    | "secondary"
    | "ghost"
    | "link"
    | "destructive"
    | "outline";
}) => {
  const navigate = useNavigate();

  const { documentId } = useParams<{ documentId: string }>();
  const { currentWorkspaceId } = useCurrentWorkspace();
  const {
    fileInputRef,
    triggerImport,
    handleFileInputChange,
    exportWhiteboardPDF,
    exportDocumentPDF,
    exportDocumentWord,
    canImport,
    canExportWhiteboard,
    canExportDocument,
  } = useDocumentImportExport({ documentId: documentId ?? "" });

  const { mutateAsync: deleteWorkspace, isPending: isDeletingWorkspace } =
    useApiMutation<Workspace>(
      "DELETE",
      API.ENDPOINTS.DOCUMENTS.Id(Number(documentId)),
      {
        invalidateQueries: [
          ["workspace", currentWorkspaceId],
          ["user-workspaces"],
        ],
        onSuccess: (data) => {
          toast.success(data.message);
          navigate(ROUTES.AUTHENTICATED.WORKSPACE(currentWorkspaceId));
        },
        onError: (error) => {
          toast.error(error.response?.data.message);
        },
      }
    );

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <Button size="icon" variant={variant} className={cn(className, "")}>
          <EllipsisVerticalIcon className="size-4" />
        </Button>
      </DropdownMenuTrigger>
      <DropdownMenuContent>
        <DropdownMenuLabel>Manage Document</DropdownMenuLabel>

        <DropdownMenuSeparator />

        <Link
          to={`${ROUTES.AUTHENTICATED.DOCUMENT(
            currentWorkspaceId,
            documentId ?? "",
            DocumentKind.DOCUMENT
          )}/edit`}
        >
          <DropdownMenuItem>
            <Settings />
            <span>Settings</span>
          </DropdownMenuItem>
        </Link>
        <DropdownMenuSeparator />
        {canImport && (
          <>
            <DropdownMenuItem
              onSelect={(e) => {
                e.preventDefault();
                triggerImport();
              }}
            >
              <Upload className="text-stone-400 size-5" />
              <span>Import</span>
            </DropdownMenuItem>
            <input
              ref={fileInputRef}
              type="file"
              accept=".pdf,.docx,.doc,.md,.markdown,.txt,.readme"
              className="hidden"
              onChange={handleFileInputChange}
            />
          </>
        )}
        <DropdownMenuSub>
          <DropdownMenuSubTrigger className="w-full gap-2">
            <Download className="text-stone-400 size-5" />
            <span>Export</span>
          </DropdownMenuSubTrigger>
          <DropdownMenuSubContent>
            {canExportWhiteboard ? (
              <DropdownMenuItem onClick={() => exportWhiteboardPDF()}>
                <FileText />
                <span>PDF</span>
              </DropdownMenuItem>
            ) : canExportDocument ? (
              <>
                <DropdownMenuItem onClick={() => exportDocumentPDF()}>
                  <FileText />
                  <span>PDF</span>
                </DropdownMenuItem>
                <DropdownMenuItem onClick={() => exportDocumentWord()}>
                  <FileType />
                  <span>Word</span>
                </DropdownMenuItem>
              </>
            ) : null}
          </DropdownMenuSubContent>
        </DropdownMenuSub>
        <DropdownMenuSeparator />
        <DropdownMenuItem>
          <DeleteDialog
            isLoading={isDeletingWorkspace}
            onConfirm={async () => await deleteWorkspace(undefined)}
          >
            <Button
              variant={"ghost"}
              className="flex items-center justify-start w-max text-destructive hover:text-destructive p-0 m-0 h-[20px]"
            >
              <Trash className="h-4 w-4 text-destructive hover:text-destructive" />
              Delete Document
            </Button>
          </DeleteDialog>
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default ManageDocumentDropdown;
