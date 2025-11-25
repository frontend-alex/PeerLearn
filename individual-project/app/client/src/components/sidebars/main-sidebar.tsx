import { lazy, Suspense, useMemo } from "react";

import { useLocation, useParams } from "react-router-dom";
import { FileText, Folder, LayoutDashboard, Settings, Square, User } from "lucide-react";

import { ROUTES } from "@/lib/router-paths";
import { DocumentKind } from "@/types/workspace";
import { defaultWorkspaceColor } from "@/components/ui/consts/consts";
import { useUserWorkspaces, useWorkspace } from "@/routes/(root)/workspace/hooks/use-workspaces";
import { DropdownSkeleton as UserDropdownSkeleton } from "@/components/skeletons/dropdown-skeleton";
import { ButtonSkeleton as ManageWorkspaceDropdownSkeleton } from "@/components/skeletons/button-skeleton";
import { Sidebar, SidebarContent, SidebarFooter, SidebarHeader, SidebarMenu, SidebarMenuButton, SidebarMenuItem } from "@/components/ui/sidebar";

import { SidebarGroupRenderer } from "./flexible-sidebar-link";

const LazyUserDropdown = lazy(
  () => import("@/components/dropdowns/user-dropdown")
);

const LazyWorkspaceDropdown = lazy(
  () => import("@/components/dropdowns/worksapces/workspace-dropdown")
);

const LazyWorspaceCrudDropdown = lazy(
  () => import("@/components/dropdowns/worksapces/workspace-dropdown-crud")
);

export function AppSidebar({ ...props }: React.ComponentProps<typeof Sidebar>) {
  const { workspaceId } = useParams<{ workspaceId?: string }>();
  const location = useLocation();
  const pathname = location.pathname;

  const isWorkspaceRoute = pathname.startsWith(`${ROUTES.BASE.APP}/workspace`);
  const showAllWorkspaces = !isWorkspaceRoute;

  const { data: workspaceResponse } = useWorkspace(
    workspaceId ? Number(workspaceId) : undefined
  );
  const { data: allWorkspacesResponse } = useUserWorkspaces();

  const workspace = workspaceResponse?.data;
  const allWorkspaces = allWorkspacesResponse?.data || [];


  const generalGroup = useMemo(
    () => ({
      label: "General",
      items: [
        {
          title: "Dashboard",
          url: ROUTES.AUTHENTICATED.DASHBOARD,
          icon: LayoutDashboard,
          isActive: pathname === ROUTES.AUTHENTICATED.DASHBOARD,
        },
        {
          title: "Profile",
          url: ROUTES.AUTHENTICATED.PROFILE,
          icon: User,
          isActive: pathname === ROUTES.AUTHENTICATED.PROFILE,
        },
        {
          title: "Settings",
          url: ROUTES.AUTHENTICATED.SETTINGS,
          icon: Settings,
          isActive: pathname === ROUTES.AUTHENTICATED.SETTINGS,
        },
      ],
    }),
    [pathname]
  );

  const workspaceItems = useMemo(() => {
    if (!showAllWorkspaces) return [];

    return allWorkspaces.map((ws) => {
      const documentItems = (ws.documents || [])
        .filter((doc) => doc.title)
        .map((doc) => {
          const documentUrl = ROUTES.AUTHENTICATED.DOCUMENT(ws.id, doc.id, doc.kind);
          return {
            title: doc.title!,
            url: documentUrl,
            isActive: pathname === documentUrl,
          };
        });

      return {
        title: ws.name,
        icon: Folder,
        items: documentItems,
        isActive: documentItems.some((item) => item.isActive),
        colorHex: ws.colorHex ?? defaultWorkspaceColor,
      };
    });
  }, [allWorkspaces, showAllWorkspaces, pathname]);

  const documentItems = useMemo(() => {
    if (showAllWorkspaces || !workspace?.documents) return [];
    return workspace.documents
      .filter(
        (document) => document.title && document.kind === DocumentKind.DOCUMENT
      )
      .map((document) => {
        const documentUrl = ROUTES.AUTHENTICATED.DOCUMENT(workspace.id, document.id, document.kind);
        return {
          title: document.title!,
          url: documentUrl,
          isActive: pathname === documentUrl,
        };
      });
  }, [workspace, pathname, showAllWorkspaces]);

  const whiteboardItems = useMemo(() => {
    if (showAllWorkspaces || !workspace?.documents) return [];
    return workspace.documents
      .filter(
        (document) =>
          document.title && document.kind === DocumentKind.WHITEBOARD
      )
      .map((document) => {
        const whiteboardUrl = ROUTES.AUTHENTICATED.DOCUMENT(workspace.id, document.id, document.kind);
        return {
          title: document.title!,
          url: whiteboardUrl,
          isActive: pathname === whiteboardUrl,
        };
      });
  }, [workspace, pathname, showAllWorkspaces]);

  const sidebarGroups = [
    generalGroup,
    ...(showAllWorkspaces
      ? [
          {
            label: "Workspaces",
            items: workspaceItems,
          },
        ]
      : [
          {
            label: workspace ? (
              <div className="flex items-center justify-between w-full">
                <div
                  className={`flex items-center ${
                    workspace.name.length <= 14 ? "gap-1" : ""
                  }`}
                >
                  <span className="max-w-[100px] truncate">
                    {workspace.name}
                  </span>
                </div>
                <Suspense fallback={<ManageWorkspaceDropdownSkeleton />}>
                  <LazyWorspaceCrudDropdown
                    className="size-4"
                    variant="ghost"
                  />
                </Suspense>
              </div>
            ) : (
              "Documents"
            ),
            items: [
              ...(documentItems.length > 0
                ? [
                    {
                      title: "Documents",
                      icon: FileText,
                      items: documentItems,
                      isActive: documentItems.some((item) => item.isActive),
                      colorHex: workspace?.colorHex ?? defaultWorkspaceColor,
                    },
                  ]
                : []),
              ...(whiteboardItems.length > 0
                ? [
                    {
                      title: "Whiteboards",
                      icon: Square,
                      items: whiteboardItems,
                      isActive: whiteboardItems.some((item) => item.isActive),
                      colorHex: workspace?.colorHex ?? defaultWorkspaceColor,
                    },
                  ]
                : []),
            ],
          },
        ]),
  ];

  return (
    <Sidebar
      variant="inset"
      {...props}
      className="bg-background border-r border-accent select-none"
    >
      <SidebarHeader className="bg-background">
        <SidebarMenu>
          <SidebarMenuItem>
            <SidebarMenuButton size="lg" asChild>
              <Suspense fallback={<UserDropdownSkeleton />}>
                <LazyWorkspaceDropdown />
              </Suspense>
            </SidebarMenuButton>
          </SidebarMenuItem>
        </SidebarMenu>
      </SidebarHeader>
      <SidebarContent className="bg-background">
        {sidebarGroups.map((group, index) => (
          <SidebarGroupRenderer
            key={
              typeof group.label === "string" ? group.label : `group-${index}`
            }
            group={group}
          />
        ))}
      </SidebarContent>
      <SidebarFooter className="bg-background">
        <Suspense fallback={<UserDropdownSkeleton />}>
          <LazyUserDropdown />
        </Suspense>
      </SidebarFooter>
    </Sidebar>
  );
}
