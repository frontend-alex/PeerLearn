import z from "zod";
import { Link } from "react-router-dom";
import { type UseFormReturn } from "react-hook-form";
import { CircleAlert, LoaderCircle, Pen } from "lucide-react";

import { getUserInitials, makeForm } from "@/lib/utils";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/contexts/AuthContext";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { emailSchema, usernameSchema } from "@/utils/schemas/user/user.schema";
import {
  Form,
  FormControl,
  FormField,
  FormItem,
  FormMessage,
} from "@/components/ui/form";
import {
  PartialContainer,
  PartialHeader,
  PartialSeparator,
  Section,
  SectionContent,
  SectionDescription,
  SectionHeading,
  SectionLabel,
} from "@/components/ui/partial";

const ProfileData = () => {
  const { user, update, isUserUpdating } = useAuth();

  if (!user) return null;

  // Schemas
  const usernameSchemaObject = z.object({ username: usernameSchema });
  const emailSchemaObject = z.object({ email: emailSchema });

  const profilePictureSchemaObject = z.object({
    image: z
      .instanceof(File)
      .refine((file) => file.size > 0, "Please select an image")
      .refine(
        (file) => ["image/jpeg", "image/png", "image/webp"].includes(file.type),
        "Only JPG, PNG, WebP images are allowed"
      ),
  });

  // Forms
  const usernameForm = makeForm(usernameSchemaObject, {
    username: user?.username ?? "",
  });

  const emailForm = makeForm(emailSchemaObject, {
    email: user?.email ?? "",
  });

  const pictureForm = makeForm(profilePictureSchemaObject, {
    image: undefined as any,
  });

  type FormSectionProps<T extends z.ZodTypeAny> = {
    title: string;
    description: string;
    form: UseFormReturn<z.infer<T>>;
    name: keyof z.infer<T>;
    type: string;
    placeholder?: string;
    onSubmit: (data: z.infer<T>) => void;
    footer?: React.ReactNode;
  };

  // Shared renderer
  const renderFormSection = <T extends z.ZodTypeAny>({
    title,
    description,
    form,
    name,
    type,
    placeholder,
    onSubmit,
    footer,
  }: FormSectionProps<T>) => (
    <Section>
      <SectionLabel>
        <SectionHeading>{title}</SectionHeading>
        <SectionDescription>{description}</SectionDescription>
        {footer}
      </SectionLabel>

      <SectionContent>
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
            <FormField
              control={form.control}
              name={name as any}
              render={({ field }) => (
                <FormItem className="space-y-2">
                  <FormControl>
                    {type === "file" ? (
                      <div className="flex justify-end relative">
                        <Input
                          type="file"
                          accept="image/*"
                          className="no-ring  cursor-pointer size-[100px] opacity-0 z-[10] rounded-full"
                          onChange={async (e) => {
                            const file = e.target.files?.[0];
                            if (!file) return;

                            field.onChange(file);

                            await form.handleSubmit(onSubmit)();
                          }}
                        />

                        <Avatar className="rounded-full size-[100px] absolute">
                          <AvatarFallback>
                            {getUserInitials(user.username)}
                          </AvatarFallback>
                          <AvatarImage src={user.profilePicture || undefined} />
                        </Avatar>

                        <div className="bg-primary size-6 rounded-full flex items-center justify-center absolute bottom-0">
                          <Pen size={15} />
                        </div>
                      </div>
                    ) : (
                      <Input
                        type={type}
                        placeholder={placeholder}
                        className="input no-ring"
                        {...field}
                      />
                    )}
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            {type !== "file" && (
              <Button type="submit" disabled={isUserUpdating}>
                {isUserUpdating ? (
                  <div className="flex items-center gap-3">
                    <LoaderCircle className="animate-spin" />
                    <p>Saving...</p>
                  </div>
                ) : (
                  "Save"
                )}
              </Button>
            )}
          </form>
        </Form>
      </SectionContent>
    </Section>
  );

  return (
    <PartialContainer className="max-w-3xl">
      <PartialHeader
        title="Account Information"
        description="Update your username, email, and profile picture here."
      />

      <PartialSeparator />

      {/* Profile Picture */}
      {renderFormSection({
        title: "Profile Picture",
        description: "Upload a new profile image.",
        form: pictureForm,
        name: "image",
        type: "file",
        onSubmit: async (data) => {
          await update({ profilePicture: data.image });
        },
        footer: (
          <p className="text-xs text-muted-foreground pt-1">
            Supported formats: JPG, PNG, WebP
          </p>
        ),
      })}

      {/* Username */}
      {renderFormSection({
        title: "Username",
        description: "This will be your unique identifier.",
        form: usernameForm,
        name: "username",
        type: "text",
        placeholder: "John Doe",
        onSubmit: (data) => {
          if (data.username === user?.username) return;
          update(data);
        },
      })}

      {/* Email */}
      {renderFormSection({
        title: "Email address",
        description: "This will be used for notifications and login.",
        form: emailForm,
        name: "email",
        type: "email",
        placeholder: "m@example.com",
        onSubmit: (data) => {
          if (data.email === user?.email) return;
          update(data);
        },
        footer: !user?.emailVerified ? (
          <div className="flex items-center gap-2 text-yellow-500 pt-1">
            <CircleAlert size={15} />
            <p className="text-xs">Email not verified</p>
            <Link
              to={`/verify-email?email=${user?.email}`}
              className="text-xs underline underline-offset-4"
            >
              Verify now
            </Link>
          </div>
        ) : null,
      })}
    </PartialContainer>
  );
};

export default ProfileData;
