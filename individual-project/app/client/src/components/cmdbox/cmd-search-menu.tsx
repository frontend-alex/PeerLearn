import * as React from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
  CommandSeparator,
} from "@/components/ui/command";
import {
  LayoutDashboard,
  Folder,
  FileText,
  Settings,
  User,
  Palette,
} from "lucide-react";
import { ROUTES } from "@/lib/router-paths";
import { useUserWorkspaces } from "@/routes/(root)/workspace/hooks/use-workspaces";
import { DocumentKind } from "@/types/workspace";

interface CommandMenuProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

const CmdMenuBox = ({ open, onOpenChange }: CommandMenuProps) => {
  const navigate = useNavigate();
  const location = useLocation();
  const { data: workspacesResponse } = useUserWorkspaces();
  const workspaces = workspacesResponse?.data || [];

  const allDocuments = React.useMemo(() => {
    return workspaces.flatMap((workspace) =>
      (workspace.documents || [])
        .filter((doc) => doc.title)
        .map((doc) => ({
          id: doc.id,
          title: doc.title!,
          workspaceId: workspace.id,
          workspaceName: workspace.name,
          kind: doc.kind,
          colorHex: workspace.colorHex,
          url: ROUTES.AUTHENTICATED.DOCUMENT(workspace.id, doc.id, doc.kind),
        }))
    );
  }, [workspaces]);

  const runCommand = React.useCallback(
    (url: string) => {
      navigate(url);
      onOpenChange(false);
    },
    [navigate, onOpenChange]
  );

  return (
    <CommandDialog className="w-[--radix-dropdown-menu-trigger-width] min-w-3xl rounded-lg" open={open} onOpenChange={onOpenChange}>
      <CommandInput placeholder="Type a command or search..." />
      <CommandList>
        <CommandEmpty>No results found.</CommandEmpty>

        {/* General Navigation */}
        <CommandGroup heading="General">
          <div className="flex flex-col gap-1">

          <CommandItem 
            data-active={location.pathname === ROUTES.AUTHENTICATED.DASHBOARD}
            onSelect={() => runCommand(ROUTES.AUTHENTICATED.DASHBOARD)}
          >
            <LayoutDashboard className="h-4 w-4" />
            <span>Dashboard</span>
          </CommandItem>  
          <CommandItem 
            data-active={location.pathname === ROUTES.AUTHENTICATED.PROFILE}
            onSelect={() => runCommand(ROUTES.AUTHENTICATED.PROFILE)}
          >
            <User className="h-4 w-4" />
            <span>Profile</span>
          </CommandItem>
          <CommandItem 
            data-active={location.pathname === ROUTES.AUTHENTICATED.SETTINGS}
            onSelect={() => runCommand(ROUTES.AUTHENTICATED.SETTINGS)}
          >
            <Settings className="h-4 w-4" />
            <span>Settings</span>
          </CommandItem>
          </div>
        </CommandGroup>

        {/* Workspaces */}
        {workspaces.length > 0 && (
          <>
            <CommandSeparator />
            <CommandGroup heading="Workspaces">
             <div className="flex flex-col gap-1">
               {workspaces.map((workspace) => {
                const workspaceUrl = ROUTES.AUTHENTICATED.WORKSPACE(workspace.id);
                const isActive = location.pathname === workspaceUrl;
                return (
                  <CommandItem
                    key={workspace.id}
                    data-active={isActive}
                    onSelect={() => runCommand(workspaceUrl)}
                  >
                    <Folder className=" h-4 w-4" style={{ color: workspace.colorHex }} />
                    <span>{workspace.name}</span>
                  </CommandItem>
                );
              })}
             </div>
            </CommandGroup>
          </>
        )}

        {/* Documents */}
        {allDocuments.length > 0 && (
          <>
            <CommandSeparator />
            <CommandGroup heading="Documents">
              <div className="flex flex-col gap-1">
                {allDocuments.map((doc) => {
                const isActive = location.pathname === doc.url;
                return (
                  <CommandItem 
                    key={doc.id} 
                    data-active={isActive}
                    onSelect={() => runCommand(doc.url)}
                  >
                    {doc.kind === DocumentKind.WHITEBOARD ? (
                      <Palette style={{ color: doc.colorHex }} className="h-4 w-4" />
                    ) : (
                      <FileText style={{ color: doc.colorHex }} className="h-4 w-4" />
                    )}
                    <span>{doc.title}</span>
                    <span className=" text-xs text-muted-foreground">
                      in {doc.workspaceName}
                    </span>
                  </CommandItem>
                );
              })}
              </div>
            </CommandGroup>
          </>
        )}
      </CommandList>
    </CommandDialog>
  );
}

export default CmdMenuBox