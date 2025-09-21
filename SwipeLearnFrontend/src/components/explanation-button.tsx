import type { Answer } from "@/pages/Quiz";
import { Button } from "./ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";
import { usePostQuizExplanation } from "@/data/query";
import { useParams } from "react-router";
import { LoadingIndicator } from "./ui/loading-indicator";

export function ExplanationButton({ answer }: { answer: Answer }) {
  const params = useParams<{ id: string }>();

  const mutation = usePostQuizExplanation();

  const handleClick = () => {
    if (mutation.data) return;

    mutation.mutate({
      body: {
        id: params.id,
        questionIndex: answer.questionIndex,
        optionIndex: answer.optionIndex,
      },
    });
  };

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          onClick={handleClick}
          className="bg-tw-secondary hover:bg-tw-secondary/90"
        >
          Soruyu Açıkla
        </Button>
      </PopoverTrigger>
      <PopoverContent
        className="flex w-96 flex-col gap-4"
        side="top"
        alignOffset={20}
        align="start"
      >
        {mutation.isPending ? (
          <div className="flex items-center justify-center">
            <LoadingIndicator />
          </div>
        ) : (
          <p>{mutation.data?.description}</p>
        )}
      </PopoverContent>
    </Popover>
  );
}
