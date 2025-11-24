import { Lock, LockOpen } from "lucide-react";
import {
  WorkspaceVisibility,
  type Document,
  type Workspace,
} from "@/types/workspace";
import { Tooltip, TooltipContent, TooltipTrigger } from "./ui/tooltip";
import { toast } from "sonner";
import { useCallback, type ReactNode } from "react";
import type { ApiSuccessResponse } from "@/types/api";

interface VisibilityToggleProps {
  /** Enum value: WorkspaceVisibility.PRIVATE or PUBLIC */
  visibility: WorkspaceVisibility | undefined;

  /** Called when toggling. You decide what it updates (doc or workspace). */
  onChange?: (
    newVis: WorkspaceVisibility
  ) =>
    | Promise<ApiSuccessResponse<Document>>
    | Promise<ApiSuccessResponse<Workspace>>;

  /** Optional: override the default icon-only trigger */
  children?:
    | ReactNode
    | ((Icon: React.ComponentType<{ className?: string }>) => ReactNode);

  /** Tailwind classes for the icon */
  className?: string;

  /** Enable pointer events if no onChange is provided */
  clickable?: boolean;
}

export const VisibilityToggle = ({
  visibility,
  onChange,
  className = "h-4 w-4",
  clickable = false,
  children,
}: VisibilityToggleProps) => {
  const current =
    String(visibility).toUpperCase() === WorkspaceVisibility.PRIVATE
      ? WorkspaceVisibility.PRIVATE
      : WorkspaceVisibility.PUBLIC;

  const isPrivate = current === WorkspaceVisibility.PRIVATE;
  const Icon = isPrivate ? Lock : LockOpen;

  const handleToggle = useCallback(async () => {
    if (!onChange) return;

    const next =
      current === WorkspaceVisibility.PRIVATE
        ? WorkspaceVisibility.PUBLIC
        : WorkspaceVisibility.PRIVATE;

    try {
      await onChange(next);
    } catch {
      toast.error("Failed to update visibility");
    }
  }, [onChange, current]);

  const isClickable = clickable || !!onChange;

  //
  // Build trigger node
  //
  let trigger;

  if (typeof children === "function") {
    trigger = (
      <div
        onClick={handleToggle}
        className={isClickable ? "cursor-pointer" : ""}
      >
        {children(Icon)}
      </div>
    );
  } else if (children) {
    trigger = (
      <div
        onClick={handleToggle}
        className={isClickable ? "cursor-pointer" : ""}
      >
        {children}
      </div>
    );
  } else {
    trigger = (
      <Icon
        className={`${className} ${
          isClickable ? "cursor-pointer hover:opacity-70" : ""
        }`}
        onClick={handleToggle}
      />
    );
  }

  return (
    <Tooltip>
      <TooltipTrigger asChild>{trigger}</TooltipTrigger>
      <TooltipContent>
        {isPrivate
          ? "Only invited members can access this."
          : "Anyone with a link can view this."}
      </TooltipContent>
    </Tooltip>
  );
};
