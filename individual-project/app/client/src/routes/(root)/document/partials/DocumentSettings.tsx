import DeleteCard from "@/components/cards/delete-card";
import type { DeleteDocumentFn } from "../hooks/use-document";

const ProfileSettings = ({
  deleteDocument,
}: {
  deleteDocument: DeleteDocumentFn;
}) => {
  return (
    <DeleteCard
      fn={async () => await deleteDocument(null)}
      button="Delete Document"
      description="Deleting your document is permanent and will remove all your data."
      dialogDescription="You're about to permanently delete your document. This action cannot be undone."
    />
  );
};

export default ProfileSettings;
