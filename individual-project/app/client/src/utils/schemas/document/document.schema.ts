import { DocumentKind, WorkspaceVisibility } from "@/types/workspace";
import z from "zod";
import { hexColorRegex } from "../workspace/workspace.schema";

export const documentSchema = z.object({
  workspaceId: z.number().int().positive(),
  title: z.string().min(1, "Title is required").max(256, "Title must be 256 characters or less"),
  kind: z.nativeEnum(DocumentKind),
  visibility: z.nativeEnum(WorkspaceVisibility).optional(),
});

export type DocumentSchemaType = z.infer<typeof documentSchema>;

export const updateDocumentSchema = z
  .object({
    title: z.string().min(1, "Title must be at least 1 character").max(128, "Title must be 128 characters or less").optional(),
    visibility: z.nativeEnum(WorkspaceVisibility).optional(),
    colorHex: z
      .string()
      .regex(hexColorRegex, "Color must be a valid hex value (e.g., #RRGGBB)")
      .optional(),
  })
  .refine(
    (data) =>
      data.title !== undefined ||
      data.visibility !== undefined ||
      data.colorHex !== undefined,
    {
      message: "At least one field must be provided to update",
      path: ["root"],
    }
  );

export type UpdateDocumentSchemaType = z.infer<typeof updateDocumentSchema>;