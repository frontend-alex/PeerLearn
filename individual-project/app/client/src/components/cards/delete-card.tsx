import DeleteDialog from "@/components/dialogs/DeleteDialog";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import type { ApiSuccessResponse } from "@/types/api";

type DeleteCardProps = {
  fn: (val: null) => Promise<ApiSuccessResponse<unknown>>
  title ?: string;
  description: string;
  button: string
  dialogDescription: string
}


const DeleteCard = ({ fn, title = "Danger Zone", description, button, dialogDescription }: DeleteCardProps) => {
  return (
    <Card className="bg-red-600/10 dark:bg-destructive/20 border-none shadow-none">
      <CardContent>
        <div>
          <h3 className="font-medium text-lg text-red-600/50 dark:text-destructive/50">
            {title}
          </h3>
          <p className="text-sm mt-2 text-stone-400 max-w-md">
            {description}
           
          </p>
          <DeleteDialog
            description={dialogDescription}
            onConfirm={() => fn(null)}
          >
            <Button variant="destructive" className="mt-5">
              {button}
            </Button>
          </DeleteDialog>
        </div>
      </CardContent>
    </Card>
  );
};

export default DeleteCard;
