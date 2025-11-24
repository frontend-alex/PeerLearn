import { useEffect, useState, useMemo } from "react";

import { Link } from "react-router-dom";
import { ChevronLeft, ChevronRight } from "lucide-react";
import { useLocation, useNavigate, useParams } from "react-router-dom";

import { WorkspaceVisibility } from "@/types/workspace";
import { ROUTES } from "@/lib/router-paths";
import { useWorkspace } from "@/routes/(root)/workspace/hooks/use-workspaces";
import { useUpdateDocument } from "@/routes/(root)/document/hooks/use-document";

import { Input } from "./ui/input";
import { Button } from "./ui/button";
import { BreadCrumpSkeleton } from "./skeletons/breadcrumps-skeleton";
import { VisibilityToggle } from "./visibility-toggle";

const ROUTE_SEGMENT_NAMES: Record<string, string> = {
  edit: "Edit",
  billing: "Billing",
  settings: "Settings",
  profile: "Profile",
  members: "Members",
  documents: "Documents",
  whiteboard: "Whiteboard",
  document: "Document",
  dashboard: "Dashboard",
  workspace: "Workspace",
  theme: "Theme",
  account: "Account",
  security: "Security",
  notifications: "Notifications",
};

// Helper to get display name for a route segment
const getSegmentDisplayName = (segment: string): string => {
  return (
    ROUTE_SEGMENT_NAMES[segment.toLowerCase()] ||
    segment.charAt(0).toUpperCase() + segment.slice(1)
  );
};

const BreadCrumps = () => {
  const navigate = useNavigate();
  const location = useLocation();

  const [isEditing, setIsEditing] = useState(false);
  const [titleInput, setTitleInput] = useState("");

  const { workspaceId, documentId } = useParams<{
    workspaceId: string;
    documentId: string;
  }>();

  const { data: workspaceResponse, isLoading } = useWorkspace(
    workspaceId ? Number(workspaceId) : undefined
  );

  const { mutateAsync: updateDocument } = useUpdateDocument(Number(documentId));

  const currentDocument = workspaceResponse?.data?.documents?.find(
    (document) => document.id === Number(documentId)
  );

  const workspace = workspaceResponse?.data;

  // Parse breadcrumb items from pathname
  const breadcrumbItems = useMemo(() => {
    const items: Array<{ label: string; path: string; isDocument?: boolean }> =
      [];
    const pathname = location.pathname;

    // Parse path segments: /app/v1/workspace/:workspaceId/...
    const segments = pathname.split("/").filter(Boolean);
    const appIndex = segments.indexOf("app");
    const v1Index = segments.indexOf("v1");

    if (appIndex !== -1 && v1Index !== -1) {
      const routeSegments = segments.slice(v1Index + 1); // Everything after /app/v1

      // Handle top-level routes (profile, settings, dashboard) - these don't need workspace context
      if (routeSegments.length > 0) {
        const firstSegment = routeSegments[0];
        const topLevelRoutes = ["profile", "settings", "dashboard"];

        if (topLevelRoutes.includes(firstSegment.toLowerCase())) {
          // For top-level routes, show Dashboard > RouteName (or just RouteName if it's dashboard)
          if (firstSegment.toLowerCase() !== "dashboard") {
            items.push({
              label: "Dashboard",
              path: ROUTES.AUTHENTICATED.DASHBOARD,
            });
          }

          // Add the top-level route
          const routePath = `${ROUTES.BASE.APP}/${firstSegment}`;
          items.push({
            label: getSegmentDisplayName(firstSegment),
            path: routePath,
          });

          // Handle sub-routes if any (e.g., /settings/theme, /profile/edit)
          if (routeSegments.length > 1) {
            let currentPath = routePath;
            for (let i = 1; i < routeSegments.length; i++) {
              const segment = routeSegments[i];
              currentPath = `${currentPath}/${segment}`;
              items.push({
                label: getSegmentDisplayName(segment),
                path: currentPath,
              });
            }
          }

          return items;
        }
      }

      // For workspace routes, start with Dashboard
      items.push({
        label: "Dashboard",
        path: ROUTES.AUTHENTICATED.DASHBOARD,
      });

      let currentPath: string = ROUTES.BASE.APP;
      let i = 0;
      let hasWorkspace = false;
      let hasDocument = false;

      while (i < routeSegments.length) {
        const segment = routeSegments[i];

        // Handle workspace
        if (segment === "workspace" && i + 1 < routeSegments.length) {
          const wsId = routeSegments[i + 1];
          currentPath = `${currentPath}/workspace/${wsId}`;
          hasWorkspace = true;

          // Add workspace to breadcrumbs (use name if available, otherwise use "Workspace")
          items.push({
            label: workspace?.name || "Workspace",
            path: currentPath,
          });

          i += 2; // Skip "workspace" and workspaceId
          continue;
        }

        // Handle document/whiteboard
        if (
          (segment === "document" || segment === "whiteboard") &&
          i + 1 < routeSegments.length
        ) {
          const docId = routeSegments[i + 1];
          currentPath = `${currentPath}/${segment}/${docId}`;
          hasDocument = true;

          // Find document in workspace
          const doc = workspaceResponse?.data?.documents?.find(
            (d) => d.id === Number(docId)
          );

          if (doc) {
            items.push({
              label: doc.title || getSegmentDisplayName(segment),
              path: currentPath,
              isDocument: true,
            });
          } else {
            items.push({
              label: getSegmentDisplayName(segment),
              path: currentPath,
              isDocument: true,
            });
          }

          i += 2;
          continue;
        }

        // Handle other segments (edit, billing, etc.) - these come after workspace or document
        if (hasWorkspace || hasDocument) {
          currentPath = `${currentPath}/${segment}`;
          items.push({
            label: getSegmentDisplayName(segment),
            path: currentPath,
          });
        }

        i++;
      }
    }

    // If no items were added (fallback), default to Dashboard
    if (items.length === 0) {
      items.push({
        label: "Dashboard",
        path: ROUTES.AUTHENTICATED.DASHBOARD,
      });
    }

    return items;
  }, [location.pathname, workspace, workspaceResponse?.data?.documents]);

  useEffect(() => {
    if (currentDocument?.title) {
      setTitleInput(currentDocument.title);
    }
  }, [currentDocument?.title]);

  const handleTitleUpdate = async () => {
    if (!documentId || !titleInput.trim()) {
      setIsEditing(false);
      return;
    }

    try {
      await updateDocument({
        title: titleInput.trim(),
      });
      setIsEditing(false);
    } catch (error) {
      setIsEditing(false);
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent<HTMLInputElement>) => {
    if (e.key === "Enter") {
      handleTitleUpdate();
    } else if (e.key === "Escape") {
      setTitleInput(currentDocument?.title || "");
      setIsEditing(false);
    }
  };

  if (isLoading) return <BreadCrumpSkeleton />;

  return (
    <div className="flex items-center gap-3 select-none">
      {/* Back/Forward Navigation */}
      <div className="flex items-center gap-1">
        <Button
          size="icon"
          variant="ghost"
          className="h-8 w-8"
          onClick={() => navigate(-1)}
        >
          <ChevronLeft className="h-4 w-4" />
        </Button>
        <Button
          size="icon"
          variant="ghost"
          className="h-8 w-8"
          onClick={() => navigate(1)}
        >
          <ChevronRight className="h-4 w-4" />
        </Button>
      </div>

      {/* Breadcrumb */}
      <nav
        className="hidden md:flex items-center gap-0 text-sm"
        aria-label="Breadcrumb"
      >
        {breadcrumbItems.map((item, index) => {
          const isLast = index === breadcrumbItems.length - 1;
          const isDocumentItem =
            item.isDocument && documentId && currentDocument;

          return (
            <div
              key={`${item.path}-${index}`}
              className="flex items-center gap-0"
            >
              {index > 0 && <span className="text-muted-foreground">/</span>}

              {isDocumentItem ? (
                <>
                  {isEditing ? (
                    <Input
                      type="text"
                      value={titleInput}
                      onChange={(e) => setTitleInput(e.target.value)}
                      onBlur={handleTitleUpdate}
                      onKeyDown={handleKeyDown}
                      autoFocus
                      className="h-8 w-[150px] no-ring border-none font-medium"
                    />
                  ) : (
                    <Button
                      onDoubleClick={() => setIsEditing(true)}
                      variant="ghost"
                      className="font-medium text-foreground px-2"
                    >
                      {item.label}
                    </Button>
                  )}
                  <VisibilityToggle
                    className="size-5 ml-1"
                    visibility={currentDocument?.visibility}
                    onChange={(newVis) =>
                      updateDocument({
                        visibility:
                          newVis === WorkspaceVisibility.PRIVATE
                            ? WorkspaceVisibility.PRIVATE
                            : WorkspaceVisibility.PUBLIC,
                      })
                    }
                    clickable={true}
                  />
                </>
              ) : isLast ? (
                // Last item - not clickable, just text
                <span className="font-medium text-foreground px-2">
                  {item.label}
                </span>
              ) : (
                // Clickable breadcrumb item
                <Link to={item.path}>
                  <Button
                    variant="ghost"
                    className="font-medium text-foreground px-2 hover:underline"
                  >
                    {item.label}
                  </Button>
                </Link>
              )}
            </div>
          );
        })}
      </nav>
    </div>
  );
};

export default BreadCrumps;
