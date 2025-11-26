import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
} from "react";

type ToggleController = {
  isOpen: boolean;
  open: () => void;
  close: () => void;
  toggle: () => void;
  set: (open: boolean) => void;
};

type BoardUIContextType = {
  commandPalette: ToggleController;
  notificationPopover: ToggleController;
  inviteCommand: ToggleController;
};

const BoardUIContext = createContext<BoardUIContextType | undefined>(undefined);

const useToggleController = (
  initialState = false
): [boolean, ToggleController] => {
  const [isOpen, setIsOpen] = useState(initialState);

  const open = useCallback(() => setIsOpen(true), []);
  const close = useCallback(() => setIsOpen(false), []);
  const toggle = useCallback(() => setIsOpen((prev) => !prev), []);
  const set = useCallback((open: boolean) => setIsOpen(open), []);

  const controller = useMemo<ToggleController>(
    () => ({
      isOpen,
      open,
      close,
      toggle,
      set,
    }),
    [close, isOpen, open, set, toggle]
  );

  return [isOpen, controller];
};

export const BoardUIProvider = ({
  children,
}: {
  children: React.ReactNode;
}) => {
  const [, commandPalette] = useToggleController();
  const [, notificationPopover] = useToggleController();
  const [, inviteCommand] = useToggleController();

  useEffect(() => {
    const handleShortcut = (event: KeyboardEvent) => {
      const key = event.key.toLowerCase();

      if ((event.metaKey || event.ctrlKey) && key === "k") {
        event.preventDefault();
        commandPalette.toggle();
        return;
      }

      if (event.shiftKey && key === "n") {
        event.preventDefault();
        notificationPopover.toggle();
        return;
      }

      if ((event.metaKey || event.ctrlKey) && key === "i") {
        event.preventDefault();
        inviteCommand.toggle();
      }
    };

    window.addEventListener("keydown", handleShortcut);
    return () => window.removeEventListener("keydown", handleShortcut);
  }, [commandPalette, notificationPopover, inviteCommand]);

  const value = useMemo<BoardUIContextType>(() => {
    return {
      commandPalette,
      notificationPopover,
      inviteCommand,
    };
  }, [commandPalette, notificationPopover, inviteCommand]);

  return (
    <BoardUIContext.Provider value={value}>
      {children}
    </BoardUIContext.Provider>
  );
};

export const useBoardUI = () => {
  const context = useContext(BoardUIContext);

  if (!context) {
    throw new Error("useBoardUI must be used within a BoardUIProvider.");
  }

  return context;
};

