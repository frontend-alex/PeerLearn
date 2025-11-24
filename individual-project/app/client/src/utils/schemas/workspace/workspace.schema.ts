import { WorkspaceVisibility } from "@/types/workspace";
import z from "zod";

export const workspaceSchema = z.object({
  name: z.string().min(2).max(100),
  visibility: z.nativeEnum(WorkspaceVisibility),
});

export type WorkspaceSchemaType = z.infer<typeof workspaceSchema>;

const hexColorRegex = /^#(?:[0-9a-fA-F]{6}|[0-9a-fA-F]{8})$/;

export const updateWorkspaceSchema = z
  .object({
    name: z.string().min(1, "Name must be at least 1 character").max(128, "Name must be 128 characters or less").optional(),
    description: z.string().max(1000, "Description must be 1000 characters or less").optional(),
    visibility: z.nativeEnum(WorkspaceVisibility).optional(),
    colorHex: z
      .string()
      .regex(hexColorRegex, "Color must be a valid hex value (e.g., #RRGGBB)")
      .optional(),
  })
  .refine(
    (data) =>
      data.name !== undefined ||
      data.description !== undefined ||
      data.visibility !== undefined ||
      data.colorHex !== undefined,
    {
      message: "At least one field must be provided to update",
      path: ["root"],
    }
  );

export type UpdateWorkspaceSchemaType = z.infer<typeof updateWorkspaceSchema>;