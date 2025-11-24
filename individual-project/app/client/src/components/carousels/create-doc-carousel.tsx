import ColoredCard from "@/components/cards/colored-card";
import {
  Carousel,
  CarouselContent,
  CarouselItem,
} from "@/components/ui/carousel";
import { defaultDocumentColor } from "@/components/ui/consts/consts";
import { ROUTES } from "@/lib/router-paths";
import CreateDocumentCard from "@/routes/(root)/workspace/components/add-document-card";
import type { Document } from "@/types/workspace";
import { formatDistanceToNow } from "date-fns";
import { Clock } from "lucide-react";
import { VisibilityToggle } from "../visibility-toggle";

const CreateDocCarousel = ({ documents }: { documents: Document[] }) => {
  return (
    <Carousel opts={{ align: "start" }} className="w-full">
      <CarouselContent className="gap-3">
        <CarouselItem className="basis-1/3 lg:basis-1/5">
          <CreateDocumentCard variant="grid" />
        </CarouselItem>

        {documents.map((document) => (
          <CarouselItem key={document.id} className="basis-1/3 lg:basis-1/5">
            <ColoredCard
              title={document.title ?? "Untitled"}
              color={document.colorHex ?? defaultDocumentColor}
              titleClassName="truncate max-w-[100px]"
              to={ROUTES.AUTHENTICATED.DOCUMENT(
                document.workspaceId,
                document.id,
                document.kind
              )}
              headerIcon={
                <VisibilityToggle
                  className="size-5"
                  visibility={document.visibility}
                />
              }
            >
              <span className="font-semibold flex items-center gap-2 text-xs text-muted-foreground">
                <Clock className="h-4 w-4" />
                Last edited{" "}
                {formatDistanceToNow(
                  new Date(document.updatedAt || document.createdAt),
                  { addSuffix: true }
                )}
              </span>
            </ColoredCard>
          </CarouselItem>
        ))}
      </CarouselContent>
    </Carousel>
  );
};

export default CreateDocCarousel;
