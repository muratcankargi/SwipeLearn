import { useState } from "react";
import { Button } from "./ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";
import { Textarea } from "./ui/textarea";
import { useParams } from "react-router";
import { Pencil } from "lucide-react";

export function TakeNotes() {
  const params = useParams<{ id: string }>();

  const [note, setNote] = useState(
    localStorage.getItem(`note_${params.id}`) ?? "",
  );

  return (
    <Popover>
      <PopoverTrigger asChild className="absolute bottom-4 left-4">
        <Button className="bg-tw-secondary hover:bg-tw-secondary/90">
          Not Al
          <Pencil />
        </Button>
      </PopoverTrigger>
      <PopoverContent
        className="flex w-96 flex-col gap-4"
        side="top"
        alignOffset={20}
        align="start"
      >
        <p className="text-gray-600">
          Bu alanda notlar alabilir, istersen quiz zamanı inceleyebilirsin.
        </p>
        <Textarea
          className="h-48 w-full"
          value={note}
          onChange={(e) => {
            setNote(e.target.value);

            localStorage.setItem(`note_${params.id}`, note);
          }}
        />
      </PopoverContent>
    </Popover>
  );
}
