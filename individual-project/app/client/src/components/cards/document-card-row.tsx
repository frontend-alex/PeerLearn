import { ROUTES } from "@/lib/router-paths";
import type { Document } from "@/types/workspace";
import { formatDistanceToNow } from "date-fns";
import { Clock } from "lucide-react";
import { Link } from "react-router-dom";
import { VisibilityToggle } from "../visibility-toggle";

const DocumentCardRow = ({ document }: { document: Document }) => {
  return (
    <Link 
      to={ROUTES.AUTHENTICATED.DOCUMENT(document.workspaceId, document.id, document.kind)} 
      style={{  "--dynamic-bg": document.colorHex } as React.CSSProperties}
      className="flex items-center gap-3 justify-between hover:bg-[var(--dynamic-bg)]/10 pr-2 cursor-pointer rounded-md trnansition-colors"
    >
      <div className="flex items-center gap-3">
        <span
          style={{ backgroundColor: document.colorHex }}
          className="w-1 h-11"
        />
        <h1>{document.title}</h1>
      </div>

      <div className="flex items-center gap-3">
        <span className="font-semibold flex items-center gap-2 text-xs text-muted-foreground">
          <Clock className="h-4 w-4" />
          Last edited{" "}
          {formatDistanceToNow(
            new Date(document.updatedAt || document.createdAt),
            { addSuffix: true }
          )}
        </span>
        <VisibilityToggle  className="size-5" visibility={document.visibility}/>
      </div>
    </Link>
  );
};

export default DocumentCardRow;
