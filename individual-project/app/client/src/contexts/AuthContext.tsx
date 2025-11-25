import type { User } from "@/types/user";
import type { ApiError } from "@/types/api";

import { toast } from "sonner";
import { API } from "@/lib/config";
import { useNavigate } from "react-router-dom";
import { useQueryClient } from "@tanstack/react-query";
import { useApiMutation, useApiQuery } from "@/hooks/hook";
import { createContext, useCallback, useContext, useMemo, useEffect, useRef } from "react";
import { buildUserUpdateFormData, type UpdateUserPayload } from "@/utils/user/update-form-data";
import { startSignalRConnection, stopSignalRConnection } from "@/lib/signalr";

type AuthContextType = {
  user: User | null;
  isLoading: boolean;
  error: ApiError | null;
  isAuthenticated: boolean;
  isUserUpdating: boolean;
  logout: () => void;
  refetch: () => void;
  update: (data: UpdateUserPayload) => Promise<void>;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: React.ReactNode }) => {
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const signalRConnectedRef = useRef(false);

  const { data, isLoading, error, refetch } = useApiQuery<User>(
    ["auth", "me"],
    API.ENDPOINTS.USER.ME,
    {
      staleTime: 1000 * 60 * 10,
      cacheTime: 1000 * 60 * 15,
      enabled: true,
    }
  );

  // Connect to SignalR when user is authenticated
  useEffect(() => {
    const connectSignalR = async () => {
      const isAuthenticated = Boolean(data?.data) && !error;
      
      if (isAuthenticated && !signalRConnectedRef.current) {
        try {
          await startSignalRConnection();
          signalRConnectedRef.current = true;
        } catch (error) {
          signalRConnectedRef.current = false;
        }
      } else if (!isAuthenticated && signalRConnectedRef.current) {
        try {
          await stopSignalRConnection();
          signalRConnectedRef.current = false;
        } catch (error) {
        }
      }
    };

    if (!isLoading) {
      connectSignalR();
    }

    return () => {
      if (signalRConnectedRef.current) {
        stopSignalRConnection().catch(toast.error);
        signalRConnectedRef.current = false;
      }
    };
  }, [data, error, isLoading]);

  const { mutateAsync: logoutMutation } = useApiMutation(
    "POST",
    API.ENDPOINTS.AUTH.LOGOUT,
    {
      onSuccess: (data) => {
        toast.success(data.message);
        queryClient.setQueryData(["auth", "me"], null);
        navigate("/", { replace: true });
      },
    }
  );


  const { mutateAsync: updateMutation, isPending: isUserUpdating } = useApiMutation<
    void,
    FormData
  >("PUT", API.ENDPOINTS.USER.UPDATE, {
    invalidateQueries: [["auth", "me"]],
    onSuccess: (data) => {
      toast.success(data.message)
    },
  });

  const logout = async () => {
    await logoutMutation(undefined);
  };

  const update = useCallback(
    async (payload: UpdateUserPayload) => {
      const formData = buildUserUpdateFormData(payload);
      const hasEntries = [...formData.keys()].length > 0;

      if (!hasEntries) return;

      await updateMutation(formData);
    },
    [updateMutation]
  );


  const contextValue = useMemo<AuthContextType>(() => {
    return {
      user: data?.data ?? null,
      isLoading,
      error,
      isUserUpdating,
      isAuthenticated: Boolean(data?.data) && !error,
      logout,
      refetch,
      update,
    };
  }, [data, isLoading, error, refetch, logout, update, isUserUpdating]);

  return (
    <AuthContext.Provider value={contextValue}>{children}</AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth must be used within an AuthProvider.");
  }
  return context;
};
