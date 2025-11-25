export type UpdateUserPayload = {
  username?: string;
  email?: string;
  profilePicture?: File;
};

export const buildUserUpdateFormData = (payload: UpdateUserPayload) => {
  const formData = new FormData();

  if (payload.username) {
    formData.append("username", payload.username);
  }

  if (payload.email) {
    formData.append("email", payload.email);
  }

  if (payload.profilePicture) {
    formData.append("image", payload.profilePicture);
  }

  return formData;
};

