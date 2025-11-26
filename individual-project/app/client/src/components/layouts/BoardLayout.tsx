import { lazy, Suspense, useMemo } from "react";

import { useLocation } from "react-router-dom";
import { Bell, Pin, Search, UserRoundPlus } from "lucide-react";

import { ROUTES } from "@/lib/router-paths";
import { Button } from "@/components/ui/button";
import CmdMenuBox from "@/components/cmdbox/cmd-search-menu";
import { ButtonSkeleton as ManageWorkspaceDropdownSkeleton } from "@/components/skeletons/button-skeleton";
import { useBoardUI } from "@/contexts/UIContext";

import { BreadCrumpSkeleton } from "../skeletons/breadcrumps-skeleton";
import CmdInviteMenu from "@/components/cmdbox/cmd-invite-menu";

const LazyBreadCrumps = lazy(() => import("@/components/BreadCrumps"));

const LazyManageWorkspaceDropdown = lazy(
  () => import("@/components/dropdowns/worksapces/workspace-dropdown-crud")
);

const LazyManageDocumentDropdown = lazy(
  () => import("@/components/dropdowns/documents/document-dropdown-crud")
);

const BoardLayout = ({ children }: { children: React.ReactNode }) => {
  const location = useLocation();
  const { commandPalette, notificationPopover, inviteCommand } = useBoardUI();

  const { isBoardRoute, isDocumentRoute } = useMemo(() => {
    const path = location.pathname;
    return {
      isBoardRoute: path.startsWith(`${ROUTES.BASE.APP}/workspace`),
      isDocumentRoute:
        path.includes("/document/") || path.includes("/whiteboard/"),
    };
  }, [location.pathname]);

  return (
    <div className="flex flex-col h-full w-full">
      <div className="sticky top-0 z-10 flex flex-row items-center border-b border-accent justify-between bg-background w-full p-2 shrink-0">
        <Suspense fallback={<BreadCrumpSkeleton />}>
          <LazyBreadCrumps />
        </Suspense>
        <div className="flex flex-row items-center gap-3">
          {isBoardRoute && (
            <>
              <Button
                size="icon"
                variant={"secondary"}
                onClick={inviteCommand.toggle}
                aria-pressed={inviteCommand.isOpen}
              >
                <UserRoundPlus className="size-4" />
              </Button>
              <Button size="icon" variant={"secondary"}>
                <Pin className="size-4" />
              </Button>
              <Suspense fallback={<ManageWorkspaceDropdownSkeleton />}>
                {isDocumentRoute ? (
                  <LazyManageDocumentDropdown />
                ) : (
                  <LazyManageWorkspaceDropdown />
                )}
              </Suspense>
              <div className="h-6 w-px bg-accent" />
            </>
          )}
          <div className="relative">
            <Button
              size="icon"
              variant={"secondary"}
              onClick={notificationPopover.toggle}
              aria-pressed={notificationPopover.isOpen}
            >
              <Bell className="size-4" />
            </Button>
            <span className="absolute -top-1 animate-pulse -right-1 bg-red-500 text-white text-xs rounded-full size-3 " />
          </div>
          <Button
            size="icon"
            variant={"secondary"}
            onClick={commandPalette.open}
          >
            <Search className="size-4" />
          </Button>
        </div>
      </div>
      <CmdMenuBox
        open={commandPalette.isOpen}
        onOpenChange={commandPalette.set}
      />
      <CmdInviteMenu
        open={inviteCommand.isOpen}
        onOpenChange={inviteCommand.set}
      />
      <div className={`flex-1 items-center min-h-0`}>{children}</div>
    </div>
  );
};

export default BoardLayout;
