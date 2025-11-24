import DeleteCard from "@/components/cards/delete-card";
import { useDeleteWorkspace } from "../hooks/use-workspaces";
import { useNavigate } from "react-router-dom";

const ProfileSettings = ({
  currentWorkspaceId,
}: {
  currentWorkspaceId: number;
}) => {
  const navigate = useNavigate();
  const { mutateAsync: del } = useDeleteWorkspace(currentWorkspaceId, navigate);

  return (
    <DeleteCard
      fn={async () => await del(null)}
      button="Delete Workspace"
      description="Deleting your workspace is permanent and will remove all your data."
      dialogDescription="You're about to permanently delete your workspace. This action cannot be undone."
    />
  );
};

export default ProfileSettings;
