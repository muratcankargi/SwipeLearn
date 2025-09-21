import { TakeNotes } from "@/components/take-notes";
import { Button } from "@/components/ui/button";
import { LoadingIndicator } from "@/components/ui/loading-indicator";
import { Progress } from "@/components/ui/progress";
import { useGetTopicQuiz, usePostTopicQuiz } from "@/data/query";
import { cn } from "@/lib/utils";
import { ArrowLeft, ArrowRight, MonitorStop } from "lucide-react";
import { useState } from "react";
import { Link, useParams } from "react-router";
import { toast } from "sonner";

const indexToLetter: Record<number, string> = {
  0: "A",
  1: "B",
  2: "C",
  3: "D",
};

export function Quiz() {
  const params = useParams<{ id: string }>();

  const questionsQuery = useGetTopicQuiz({ query: { id: params.id } });

  const questions = questionsQuery.data?.questions ?? [];

  // currentQuestionIndex
  const [currentIndex, setCurrentIndex] = useState(0);

  const currentQuestion = questions[currentIndex];

  const progress = ((currentIndex + 1) * 100) / questions.length;

  const [answers, setAnswers] = useState<
    {
      questionIndex: number;
      optionIndex: number;
      correctOptionIndex: number | undefined;
    }[]
  >([]);

  const nextQuestion = () => {
    if (currentIndex + 1 === questions.length) return;

    setCurrentIndex((prevValue) => prevValue + 1);
  };

  const previousQuestion = () => {
    if (currentIndex === 0) return;

    setCurrentIndex((prevValue) => prevValue - 1);
  };

  const mutation = usePostTopicQuiz();

  const handleOptionClick = (optionIndex: number) => {
    mutation.mutate(
      {
        body: {
          id: params.id,
          questionIndex: currentIndex,
          optionIndex: optionIndex,
        },
      },
      {
        onSuccess: (data) => {
          setAnswers((prevValues) => [
            ...prevValues,
            {
              questionIndex: currentIndex,
              optionIndex: optionIndex,
              correctOptionIndex: data.isCorrect ? optionIndex : undefined, // burası değişecek
            },
          ]);

          if (!data.isCorrect) {
            toast.info("Yanlış cevap.");
          }
        },
        onError: () => {
          toast.error("Bir şeyler ters gitti.");
        },
      },
    );
  };

  if (questionsQuery.isLoading) return <LoadingIndicator />;
  if (questionsQuery.isError || !questionsQuery.data?.questions)
    return <div>Bir şeyler ters gitti.</div>;

  return (
    <>
      <div className="mb-14 grid w-full grid-cols-3 items-center px-4">
        <div className="flex justify-start">
          <Link to={`/kaydir/${params.id}`}>
            <Button className="bg-tw-secondary hover:bg-tw-secondary/90">
              Videolara Dön
              <MonitorStop />
            </Button>
          </Link>
        </div>
        <div>
          <div className="mb-2 flex w-full justify-center">İlerleme</div>
          <Progress
            value={progress}
            className="[&_[data-slot=progress-indicator]]:bg-tw-secondary w-full"
          />
        </div>
        <div></div>
      </div>

      <div className="my-4 flex w-1/2 justify-between">
        <Button
          disabled={currentIndex === 0}
          onClick={previousQuestion}
          size={"sm"}
        >
          <ArrowLeft />
          Önceki Soru
        </Button>
        <Button
          disabled={currentIndex + 1 === questions.length}
          onClick={nextQuestion}
          size={"sm"}
        >
          Sonraki Soru
          <ArrowRight />
        </Button>
      </div>

      <div className="bg-tw-primary mb-4 flex h-fit min-h-64 w-1/2 flex-col gap-4 rounded-md p-4 shadow">
        <h1 className="font-semibold">{currentIndex + 1}. Soru</h1>

        <p>{currentQuestion.question}</p>
      </div>

      <div className="space-2 grid w-2/3 grid-cols-2 gap-4">
        {currentQuestion.options?.map((option, i) => (
          <button
            key={`${currentIndex}-${i}`}
            disabled={
              !!answers.find((answer) => answer.questionIndex === currentIndex)
            }
            onClick={() => handleOptionClick(i)}
            className={cn(
              "bg-tw-primary hover:bg-tw-primary/90 rounded-md p-4 shadow transition-colors",
              {
                "hover:bg-tw-green-400/90 bg-green-400": answers.find(
                  (answer) =>
                    answer.questionIndex === currentIndex &&
                    answer.correctOptionIndex === i,
                ),
              },
            )}
          >
            {indexToLetter[i]}) {option}
          </button>
        ))}
      </div>

      <TakeNotes />
    </>
  );
}
