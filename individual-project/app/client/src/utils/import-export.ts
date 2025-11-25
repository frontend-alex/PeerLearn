import { useCallback, useRef } from "react";
import { toast } from "sonner";
import { useDocument, useUpdateDocument } from "@/routes/(root)/document/hooks/use-document";
import { DocumentKind } from "@/types/workspace";
import {
  exportWhiteboardToPDF,
  exportDocumentToPDF,
  exportDocumentToWord,
} from "@/utils/export";
import { importFileToEditorJS } from "@/utils/import";

interface UseDocumentImportExportOptions {
  documentId: string;
  onImportSuccess?: () => void;
  onExportSuccess?: () => void;
}

/**
 * Hook for document import and export functionality
 * Provides clean, reusable methods for importing and exporting documents
 */
export function useDocumentImportExport({
  documentId,
  onImportSuccess,
  onExportSuccess,
}: UseDocumentImportExportOptions) {
  const { data: documentResponse } = useDocument(documentId);
  const document = documentResponse?.data;
  const { mutateAsync: updateDocument } = useUpdateDocument(documentId);
  const fileInputRef = useRef<HTMLInputElement>(null);

  /**
   * Handle file import
   */
  const handleImport = useCallback(
    async (file: File) => {
      if (!document || document.kind !== DocumentKind.DOCUMENT) {
        toast.error("Import is only available for documents");
        return;
      }

      const toastId = `import-file-${Date.now()}`;

      try {
        toast.loading("Importing file...", { id: toastId });

        // Parse file to EditorJS format
        const editorJSData = await importFileToEditorJS(file);

        // Convert to JSON string
        const contentString = JSON.stringify(editorJSData);

        // Update document
        await updateDocument({ content: contentString });

        toast.success("File imported successfully", { id: toastId });
        onImportSuccess?.();

        return true;
      } catch (error) {
        console.error("Import error:", error);
        toast.error(
          error instanceof Error
            ? error.message
            : "Failed to import file",
          { id: toastId }
        );
        return false;
      }
    },
    [document, updateDocument, onImportSuccess]
  );

  /**
   * Trigger file input click
   */
  const triggerImport = useCallback(() => {
    fileInputRef.current?.click();
  }, []);

  /**
   * Handle file input change
   */
  const handleFileInputChange = useCallback(
    async (e: React.ChangeEvent<HTMLInputElement>) => {
      const file = e.target.files?.[0];
      if (!file) return;

      await handleImport(file);

      // Reset file input
      if (fileInputRef.current) {
        fileInputRef.current.value = "";
      }
    },
    [handleImport]
  );

  /**
   * Export whiteboard to PDF
   */
  const exportWhiteboardPDF = useCallback(
    async (filename?: string) => {
      if (!document || document.kind !== DocumentKind.WHITEBOARD) {
        toast.error("Export is only available for whiteboards");
        return false;
      }

      try {
        const canvasElement = window.document.querySelector(".react-flow");
        if (!canvasElement) {
          toast.error("Whiteboard canvas not found");
          return false;
        }

        const exportFilename = filename || `${document.title || "whiteboard"}.pdf`;
        await exportWhiteboardToPDF(
          canvasElement as HTMLElement,
          exportFilename
        );

        toast.success("Whiteboard exported successfully");
        onExportSuccess?.();
        return true;
      } catch (error) {
        console.error("Export error:", error);
        toast.error("Failed to export whiteboard");
        return false;
      }
    },
    [document, onExportSuccess]
  );

  /**
   * Export document to PDF
   */
  const exportDocumentPDF = useCallback(
    async (filename?: string) => {
      if (!document || document.kind !== DocumentKind.DOCUMENT) {
        toast.error("Export is only available for documents");
        return false;
      }

      if (!document.content) {
        toast.error("Document has no content to export");
        return false;
      }

      try {
        const exportFilename = filename || `${document.title || "document"}.pdf`;
        await exportDocumentToPDF(
          document.content,
          document.title || "Document",
          exportFilename
        );

        toast.success("Document exported to PDF successfully");
        onExportSuccess?.();
        return true;
      } catch (error) {
        console.error("Export error:", error);
        toast.error("Failed to export document to PDF");
        return false;
      }
    },
    [document, onExportSuccess]
  );

  /**
   * Export document to Word
   */
  const exportDocumentWord = useCallback(
    async (filename?: string) => {
      if (!document || document.kind !== DocumentKind.DOCUMENT) {
        toast.error("Export is only available for documents");
        return false;
      }

      if (!document.content) {
        toast.error("Document has no content to export");
        return false;
      }

      try {
        const exportFilename = filename || `${document.title || "document"}.docx`;
        await exportDocumentToWord(
          document.content,
          document.title || "Document",
          exportFilename
        );

        toast.success("Document exported to Word successfully");
        onExportSuccess?.();
        return true;
      } catch (error) {
        console.error("Export error:", error);
        toast.error("Failed to export document to Word");
        return false;
      }
    },
    [document, onExportSuccess]
  );

  return {
    // Import
    fileInputRef,
    triggerImport,
    handleFileInputChange,
    handleImport,
    canImport: document?.kind === DocumentKind.DOCUMENT,

    // Export
    exportWhiteboardPDF,
    exportDocumentPDF,
    exportDocumentWord,
    canExportWhiteboard: document?.kind === DocumentKind.WHITEBOARD,
    canExportDocument: document?.kind === DocumentKind.DOCUMENT && !!document.content,

    // Document info
    document,
  };
}

