import { Link } from "react-router";
import { Button } from "./ui/button";
import { Popover, PopoverTrigger } from "./ui/popover";
import { PopoverContent } from "@radix-ui/react-popover";
import { ExternalLink } from "lucide-react";

export function TopicContent({
  description,
  id,
}: {
  description: string;
  id: string;
}) {
  return (
    <Popover>
      <PopoverTrigger
        asChild
        className="absolute bottom-4 left-1/2 -translate-x-1/2"
      >
        <Button>Açıklamayı Gör</Button>
      </PopoverTrigger>
      <PopoverContent
        alignOffset={20}
        className="z-[999] flex w-96 flex-col gap-4 rounded-md border bg-white p-4 shadow"
      >
        <Link
          to={`/kaydir/${id}`}
          className="my-2 flex items-center gap-x-1 font-semibold"
        >
          İçeriğe Git <ExternalLink className="h-4 w-4" />
        </Link>

        <p>{description}</p>
      </PopoverContent>
    </Popover>
  );
}
