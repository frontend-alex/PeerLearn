import React from "react";

import { randomColors } from "@/components/ui/consts/consts";
import type { Document } from "@/types/workspace";
import type { ApiSuccessResponse } from "@/types/api";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import type { UpdateWorkspaceFn } from "@/routes/(root)/workspace/hooks/use-workspaces";


type UpdateDocumentFn = (data: any) => Promise<ApiSuccessResponse<Document>>;

type Props = {
  hexColor: string;
  updateWorkspace?: UpdateWorkspaceFn;
  updateDocument?: UpdateDocumentFn;
};

const ColorPopover: React.FC<Props> = ({
  hexColor,
  updateWorkspace,
  updateDocument,
}) => {
  const handler = updateWorkspace || updateDocument;

  const handleColorSelect = async (color: string) => {
    if (!handler) return;

    handler({ colorHex: color });
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <button
          className="size-10 rounded-lg border border-border"
          style={{ backgroundColor: hexColor }}
        />
      </PopoverTrigger>

      <PopoverContent className="w-fit p-3">
        <div className="grid grid-cols-5 gap-2">
          {randomColors.map((color) => (
            <button
              key={color}
              onClick={() => handleColorSelect(color)}
              className="size-6 rounded-lg border border-border transition-colors hover:opacity-80"
              style={{ backgroundColor: color }}
            />
          ))}
        </div>
      </PopoverContent>
    </Popover>
  );
};

export default ColorPopover;
