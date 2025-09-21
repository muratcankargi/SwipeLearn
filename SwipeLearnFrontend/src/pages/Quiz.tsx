import { ExplanationButton } from "@/components/explanation-button";
import { TakeNotes } from "@/components/take-notes";
import { Button } from "@/components/ui/button";
import { LoadingIndicator } from "@/components/ui/loading-indicator";
import { Progress } from "@/components/ui/progress";
import { useGetTopicQuiz, usePostTopicQuiz } from "@/data/query";
import { cn } from "@/lib/utils";
import {
  ArrowLeft,
  ArrowRight,
  Check,
  ExternalLink,
  MonitorStop,
} from "lucide-react";
import { useState } from "react";
import { Link, useParams } from "react-router";
import { toast } from "sonner";

const indexToLetter: Record<number, string> = {
  0: "A",
  1: "B",
  2: "C",
  3: "D",
};

export type Answer = {
  questionIndex: number;
  optionIndex: number;
  correctOptionIndex: number | undefined;
};

export function Quiz() {
  const params = useParams<{ id: string }>();

  const questionsQuery = useGetTopicQuiz({ query: { id: params.id } });

  const questions = questionsQuery.data?.questions ?? [];

  // currentQuestionIndex
  const [currentIndex, setCurrentIndex] = useState(0);

  const currentQuestion = questions[currentIndex];

  const progress = ((currentIndex + 1) * 100) / questions.length;

  const [answers, setAnswers] = useState<Answer[]>([]);

  const [endQuiz, setEndQuiz] = useState(false);
  const [correctQuestionsCount, setCorrectQuestionsCount] = useState(0);

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
        onSuccess: async (data) => {
          if (optionIndex === data?.correctOptionIndex) {
            const audio = new Audio("/correct.mp3");
            await audio.play();

            setTimeout(() => nextQuestion(), 300);
            setCorrectQuestionsCount((prevValue) => prevValue + 1);
          }

          setAnswers((prevValues) => [
            ...prevValues,
            {
              questionIndex: currentIndex,
              optionIndex: optionIndex,
              correctOptionIndex: data?.correctOptionIndex,
            },
          ]);
        },
        onError: () => {
          toast.error("Bir şeyler ters gitti.");
        },
      },
    );
  };

  const isAnsweredIncorrectly = answers.find(
    (answer) =>
      answer.questionIndex === currentIndex &&
      typeof answer.correctOptionIndex === "number" &&
      answer.correctOptionIndex !== answer.optionIndex,
  );

  const repeatQuiz = () => {
    setAnswers([]);
    setCorrectQuestionsCount(0);
    setCurrentIndex(0);
    setEndQuiz(false);
  };

  if (questionsQuery.isLoading) return <LoadingIndicator />;
  if (questionsQuery.isError || !questionsQuery.data?.questions)
    return <div>Bir şeyler ters gitti.</div>;

  return (
    <>
      {!endQuiz && (
        <div className="mt-24 mb-14 grid w-full items-center gap-4 px-4 sm:mt-0 sm:grid-cols-3">
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

          <div className="flex sm:justify-end">
            <Button
              onClick={() => setEndQuiz(true)}
              className="bg-tw-secondary hover:bg-tw-secondary/90"
            >
              Quizi Bitir
              <Check />
            </Button>
          </div>
        </div>
      )}

      {!endQuiz && (
        <div className="my-4 flex w-full justify-between px-4 sm:w-1/2">
          <Button
            disabled={currentIndex === 0}
            onClick={previousQuestion}
            size={"sm"}
          >
            <ArrowLeft />
            Önceki Soru
          </Button>

          {isAnsweredIncorrectly && (
            <ExplanationButton answer={isAnsweredIncorrectly} />
          )}

          <Button
            disabled={currentIndex + 1 === questions.length}
            onClick={nextQuestion}
            size={"sm"}
          >
            Sonraki Soru
            <ArrowRight />
          </Button>
        </div>
      )}

      {!endQuiz && (
        <div className="bg-tw-primary mb-4 flex h-fit min-h-64 w-[90%] flex-col gap-4 rounded-md p-4 shadow sm:w-1/2">
          <h1 className="font-semibold">{currentIndex + 1}. Soru</h1>

          <p>{currentQuestion.question}</p>
        </div>
      )}

      {!endQuiz && (
        <div className="space-2 mb-24 grid w-2/3 grid-cols-1 gap-4 sm:mb-0 sm:grid-cols-2">
          {currentQuestion.options?.map((option, i) => {
            const answer = answers.find(
              (answer) => answer.questionIndex === currentIndex,
            );

            return (
              <button
                key={`${currentIndex}-${i}`}
                disabled={
                  !!answers.find(
                    (answer) => answer.questionIndex === currentIndex,
                  )
                }
                onClick={() => handleOptionClick(i)}
                className={cn(
                  "bg-tw-primary hover:bg-tw-primary/90 rounded-md p-4 shadow transition-colors",
                  {
                    "bg-green-400 hover:bg-green-400":
                      answer && answer?.correctOptionIndex === i,
                    "bg-red-400 hover:bg-red-400":
                      answer && answer.correctOptionIndex !== i,
                  },
                )}
              >
                {indexToLetter[i]}) {option}
              </button>
            );
          })}
        </div>
      )}

      {endQuiz && (
        <div className="flex flex-col gap-6">
          <h1 className="text-center text-2xl font-bold">
            Toplam {questions.length} sorudan {correctQuestionsCount} doğru
            bildiniz.
          </h1>

          <div className="flex flex-col items-center gap-2">
            <Link to={"/"}>
              <Button
                onClick={() => setEndQuiz(true)}
                className="bg-tw-secondary hover:bg-tw-secondary/90 w-fit"
              >
                Yeni bir konu öğren
                <ExternalLink />
              </Button>
            </Link>

            <Button variant={"ghost"} onClick={repeatQuiz}>
              Quizi Tekrarla
            </Button>
          </div>
        </div>
      )}

      <TakeNotes />
    </>
  );
}
