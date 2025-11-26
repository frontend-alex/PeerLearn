import { useMemo, useState } from "react";

import { Loader2, MailPlus } from "lucide-react";

import { API } from "@/lib/config";
import type { User } from "@/types/user";
import { useApiQuery } from "@/hooks/hook";
import { getUserInitials } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { useDebounce } from "@/hooks/use-debounce";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import {
  CommandDialog,
  CommandEmpty,
  CommandGroup,
  CommandInput,
  CommandItem,
  CommandList,
} from "@/components/ui/command";

interface CmdInviteMenuProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  workspaceId?: number; // optional if needed for invite logic
}

const MIN_QUERY = 2;
const LIMIT = 8;

const CmdInviteMenu = ({
  open,
  onOpenChange,
  workspaceId,
}: CmdInviteMenuProps) => {
  const [query, setQuery] = useState("");
  const debounced = useDebounce(query, 250);

  const enabled = debounced.length >= MIN_QUERY;

  // Backend search
  const { data: users = [], isPending } = useApiQuery<User[], User[]>(
    ["invite-users", debounced],
    API.ENDPOINTS.USER.SEARCH(debounced, LIMIT),
    {
      enabled,
      staleTime: 0,
      select: (res) => res.data ?? [],
    }
  );

  // Invite mutation
  //   const { mutateAsync: inviteUser } = useApiMutation(
  //     "POST",
  //     API.ENDPOINTS.WORKSPACE.INVITE(workspaceId),
  //     {}
  //   );

  const emptyMessage = useMemo(() => {
    if (query.length === 0) return "Start typing to search.";
    if (query.length < MIN_QUERY) return "Keep typing…";
    if (isPending) return "Searching users…";
    if (users.length === 0) return "No matching users.";
    return null;
  }, [query, isPending, users]);

  const handleInvite = async (user: User) => {
    // await inviteUser({ userId: user.id });
    onOpenChange(false);
  };

  return (
    <CommandDialog open={open} onOpenChange={onOpenChange}>
      <CommandInput
        placeholder="Search by name, username, or email..."
        value={query}
        onValueChange={setQuery}
      />

      <CommandList>
        <CommandEmpty>{emptyMessage}</CommandEmpty>

        {users.length > 0 && (
          <CommandGroup heading="Invite users">
            <div className="flex flex-col gap-1">
              {users.map((user) => {
                return (
                  <CommandItem
                    key={user.id}
                    value={debounced}
                    onSelect={() => handleInvite(user)}
                  >
                    {/* Avatar */}
                    <Avatar className="flex h-8 w-8 items-center justify-center rounded-full bg-muted text-sm font-semibold uppercase">
                      <AvatarFallback>
                        {getUserInitials(user.username)}
                      </AvatarFallback>
                      <AvatarImage src={user.profilePicture ?? ""} />
                    </Avatar>

                    {/* User Info */}
                    <div className="flex flex-col">
                      <span className="text-sm font-medium capitalize">
                        {user.firstName} {user.lastName}
                      </span>
                      <span className="text-xs text-muted-foreground">
                        @{user.username} · {user.email}
                      </span>
                    </div>

                    {/* Invite button indicator */}
                    <Button
                      tabIndex={-1}
                      className="ml-auto pointer-events-auto flex items-center gap-1 text-xs"
                      onClick={(e) => {
                        e.stopPropagation();
                        handleInvite(user);
                      }}
                    >
                      {isPending ? (
                        <Loader2 className="h-4 w-4 animate-spin text-black" />
                      ) : (
                        <MailPlus className="h-4 w-4 text-black" />
                      )}
                      Invite
                    </Button>
                  </CommandItem>
                );
              })}
            </div>
          </CommandGroup>
        )}
      </CommandList>
    </CommandDialog>
  );
};

export default CmdInviteMenu;
