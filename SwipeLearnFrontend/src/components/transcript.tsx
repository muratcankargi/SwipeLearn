import { Button } from "./ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";

export function Transcript({ transcript }: { transcript: string }) {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button>Transkript</Button>
      </PopoverTrigger>
      <PopoverContent
        className="flex w-96 flex-col gap-4"
        side="bottom"
        align="start"
      >
        <p className="text-gray-600">{transcript}</p>
      </PopoverContent>
    </Popover>
  );
}
