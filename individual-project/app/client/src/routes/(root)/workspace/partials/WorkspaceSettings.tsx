import DeleteCard from "@/components/cards/delete-card";
import type { DeleteDocumentFn } from "../../document/hooks/use-document";

const ProfileSettings = ({
  deleteWorkspace,
}: {
  deleteWorkspace: DeleteDocumentFn;
}) => {
  return (
    <DeleteCard
      fn={async () => await deleteWorkspace(null)}
      button="Delete Workspace"
      description="Deleting your workspace is permanent and will remove all your data."
      dialogDescription="You're about to permanently delete your workspace. This action cannot be undone."
    />
  );
};

export default ProfileSettings;
