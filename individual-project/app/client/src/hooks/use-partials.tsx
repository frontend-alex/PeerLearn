import { type User } from "@/types/user";
import { useEffect, useMemo, useState } from "react";
import type React from "react";

type PartialModuleProps = {
  user: User;
  refetchUser: () => void;
} & Record<string, unknown>;

export type ModuleLoader<T extends PartialModuleProps = PartialModuleProps> =
  Record<string, () => Promise<{ default: React.FC<T> }>>;

type FilterFn = (filename: string, user: User) => boolean;

type UseDynamicPartialsOptions<T extends Record<string, unknown> = {}> = {
  partialModules: ModuleLoader<PartialModuleProps & T>;
  user: User | null | undefined;

  // kept for API compatibility but NOT used inside the hook
  refetchUser?: () => void;
  extraProps?: T;
  border?: boolean;

  filterFn?: FilterFn;
  reverseOrder?: boolean;
};

export type DynamicPartial<TExtra extends Record<string, unknown> = {}> = {
  key: string;
  Component: React.FC<PartialModuleProps & TExtra>;
};

/**
 * useDynamicPartials
 *
 * Loads dynamic partial modules once (per user / filter / order),
 * and returns their component definitions. The caller is responsible
 * for rendering them with props (user, refetchUser, extraProps).
 */
export function useDynamicPartials<T extends Record<string, unknown> = {}>({
  partialModules,
  user,
  filterFn,
  reverseOrder = false,
}: UseDynamicPartialsOptions<T>): DynamicPartial<T>[] {
  const [partials, setPartials] = useState<DynamicPartial<T>[]>([]);

  // stable list of module entries
  const moduleEntries = useMemo(
    () => Object.entries(partialModules),
    [partialModules]
  );

  useEffect(() => {
    if (!user) {
      setPartials([]);
      return;
    }

    let cancelled = false;

    const load = async () => {
      let entries = [...moduleEntries];
      entries.sort(([a], [b]) => a.localeCompare(b));
      if (reverseOrder) entries.reverse();

      const filtered = entries.filter(([path]) => {
        const fileName =
          path.split("/").pop()?.replace(".tsx", "").toLowerCase() || "";

        if (filterFn) return filterFn(fileName, user);

        // default filtering logic – keep it if you still need it
        const hasPassword = true;
        if (fileName === "changecredentialstwo" && hasPassword) return false;
        if (fileName === "changecredentials" && !hasPassword) return false;
        return true;
      });

      const loaded = await Promise.all(
        filtered.map(async ([path, loader]) => {
          const mod = await loader();
          const Component = mod.default as React.FC<
            PartialModuleProps & T
          >;

          return {
            key: path,
            Component,
          };
        })
      );

      if (!cancelled) setPartials(loaded);
    };

    load();

    return () => {
      cancelled = true;
    };
  }, [user, moduleEntries, filterFn, reverseOrder]);

  return partials;
}
