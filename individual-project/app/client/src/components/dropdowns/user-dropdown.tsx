import { Link } from "react-router-dom";
import {
  ChevronsUpDown,
  Layout,
  LogOut,
  Settings,
  Sparkles,
  User,
} from "lucide-react";

import { ROUTES } from "@/lib/router-paths";
import { getUserInitials } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { useAuth } from "@/contexts/AuthContext";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { useCurrentWorkspace } from "@/routes/(root)/workspace/hooks/use-current-workspace";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuGroup,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { DropdownSkeleton as UserDropdownSkeleton } from "../skeletons/dropdown-skeleton";

const UserDropdown = ({
  align = "end",
  side = "bottom",
}: {
  align?: "start" | "center" | "end";
  side?: "top" | "right" | "bottom" | "left";
}) => {
  const { currentWorkspaceId, hasWorkspaceContext } = useCurrentWorkspace();

  const dropdownLinks = [
    {
      name: "Workspace Board",
      url: hasWorkspaceContext
        ? ROUTES.HELPERS.getBoardRoute(currentWorkspaceId)
        : ROUTES.AUTHENTICATED.DASHBOARD,
      icon: Layout,
    },
    {
      name: "Profile",
      url: ROUTES.AUTHENTICATED.PROFILE,
      icon: User,
    },
    {
      name: "Settings",
      url: ROUTES.AUTHENTICATED.SETTINGS,
      icon: Settings,
    },
  ];

  const { logout, user, isLoading } = useAuth();

  if (isLoading) return <UserDropdownSkeleton />;
  if (!user) return null;

  return (
    <DropdownMenu>
      <DropdownMenuTrigger asChild>
        <div>
          <Button
            variant="ghost"
            className="no-ring h-12 w-full justify-start gap-3 px-3 data-[state=open]:bg-accent data-[state=open]:text-accent-foreground"
          >
            <Avatar className="h-8 w-8 rounded-lg">
              {/* <AvatarImage src={user.avatar || "/placeholder.svg"} alt={user.name} /> */}
              <AvatarFallback className="rounded-full">
                {getUserInitials(user.username)}
              </AvatarFallback>
            </Avatar>
            <div className="grid flex-1 text-left text-sm leading-tight">
              <span className="truncate font-semibold">{user.username}</span>
              <span className="truncate text-xs text-muted-foreground">
                {user?.email}
              </span>
            </div>
            <ChevronsUpDown className="ml-auto size-4" />
          </Button>
        </div>
      </DropdownMenuTrigger>
      <DropdownMenuContent
        className="w-[--radix-dropdown-menu-trigger-width] min-w-56 rounded-lg"
        side={side}
        align={align}
        sideOffset={4}
      >
        <DropdownMenuLabel className="p-0 font-normal">
          <div className="flex items-center gap-2 px-1 py-1.5 text-left text-sm">
            <Avatar className="h-8 w-8 rounded-full">
              {/* <AvatarImage src={user.avatar || "/placeholder.svg"} alt={user.name} /> */}
              <AvatarFallback className="rounded-lg">
                {getUserInitials(user.username)}
              </AvatarFallback>
            </Avatar>
            <div className="grid flex-1 text-left text-sm leading-tight">
              <span className="truncate font-semibold">{user.username}</span>
              <span className="truncate text-xs text-muted-foreground">
                {user.email}
              </span>
            </div>
          </div>
        </DropdownMenuLabel>
        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          <DropdownMenuItem>
            <Sparkles />
            Upgrade to Pro
          </DropdownMenuItem>
        </DropdownMenuGroup>
        <DropdownMenuSeparator />
        <DropdownMenuGroup>
          {dropdownLinks.map((link) => (
            <Link to={link.url}>
              <DropdownMenuItem>
                <link.icon />
                {link.name}
              </DropdownMenuItem>
            </Link>
          ))}
        </DropdownMenuGroup>
        <DropdownMenuSeparator />
        <DropdownMenuItem
          className="text-destructive cursor-pointer"
          onClick={logout}
        >
          <LogOut className="text-destructive" />
          Log out
        </DropdownMenuItem>
      </DropdownMenuContent>
    </DropdownMenu>
  );
};

export default UserDropdown;
