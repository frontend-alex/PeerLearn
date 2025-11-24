import { toast } from "sonner";
import { useForm } from "react-hook-form";
import { useApiMutation } from "@/hooks/hook";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import {
  updatePasswordSchema,
  type updatePasswordSchemaType,
} from "@/utils/schemas/auth/auth.schema";
import { LoaderCircle } from "lucide-react";
import { lazy, Suspense } from "react";

import {
  PartialContainer,
  Section,
  SectionLabel,
  SectionHeading,
  SectionDescription,
  SectionContent,
} from "@/components/ui/partial";

const PasswordStrengthChecks = lazy(
  () => import("@/components/PasswordChecker")
);

const ProfilePassword = () => {
  const updatePasswordsForm = useForm({
    resolver: zodResolver(updatePasswordSchema),
    defaultValues: {
      password: "",
      newPassword: "",
      confirmNewPassword: "",
    },
  });

  const { watch } = updatePasswordsForm;

  const { mutateAsync: updatePassword, isPending } =
    useApiMutation<updatePasswordSchemaType>("PUT", "/auth/update-password", {
      onSuccess: (data) => {
        updatePasswordsForm.reset();
        toast.success(data.message);
      },
      onError: (err) => toast.error(err.response?.data.message),
    });

  const handleUpdatePassword = async (data: updatePasswordSchemaType) =>
    await updatePassword(data);

  return (
    <PartialContainer className="mt-5 max-w-3xl">
      <Section>
        {/* Left Column — Label */}
        <SectionLabel>
          <SectionHeading>Password</SectionHeading>
          <SectionDescription>
            Update your account password. Strong passwords keep your account secure.
          </SectionDescription>
        </SectionLabel>

        {/* Right Column — Content */}
        <SectionContent>
          <Form {...updatePasswordsForm}>
            <form
              onSubmit={updatePasswordsForm.handleSubmit(handleUpdatePassword)}
              className="space-y-4"
            >
              {/* Current password */}
              <FormField
                control={updatePasswordsForm.control}
                name="password"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormControl>
                      <Input
                        type="password"
                        placeholder="Current Password"
                        className="input no-ring"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* New password */}
              <FormField
                control={updatePasswordsForm.control}
                name="newPassword"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormControl>
                      <Input
                        type="password"
                        placeholder="New Password"
                        className="input no-ring"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Password strength checker */}
              {watch("newPassword") ? (
                <Suspense fallback={null}>
                  <PasswordStrengthChecks password={watch("newPassword")} />
                </Suspense>
              ) : null}

              {/* Confirm new password */}
              <FormField
                control={updatePasswordsForm.control}
                name="confirmNewPassword"
                render={({ field }) => (
                  <FormItem className="space-y-2">
                    <FormControl>
                      <Input
                        type="password"
                        placeholder="Confirm New Password"
                        className="input no-ring"
                        {...field}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Submit */}
              <Button type="submit" disabled={isPending}>
                {isPending ? (
                  <div className="flex items-center gap-3">
                    <LoaderCircle className="animate-spin" />
                    <p>Saving...</p>
                  </div>
                ) : (
                  "Save"
                )}
              </Button>
            </form>
          </Form>
        </SectionContent>
      </Section>
    </PartialContainer>
  );
};

export default ProfilePassword;
