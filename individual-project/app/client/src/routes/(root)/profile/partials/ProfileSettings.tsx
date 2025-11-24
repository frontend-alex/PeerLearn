import DeleteCard from "@/components/cards/delete-card";

import { toast } from "sonner";
import { useApiMutation } from "@/hooks/hook";

const ProfileSettings = () => {
  const { mutateAsync: deleteUser } = useApiMutation("DELETE", "/auth/delete", {
    invalidateQueries: [["auth", "me"]],
    onSuccess: (data) => toast.success(data.message),
    onError: (err) => toast.error(err.response?.data.message),
  });

  return (
    <DeleteCard
      fn={() => deleteUser(null)}
      description="Deleting your account is permanent and will remove all your data."
      button="Delete Account"
      dialogDescription="You're about to permanently delete your workspace. This action cannot be undone."
    />
  );
};

export default ProfileSettings;
