import { Button } from "@/components/ui/button";
import { Progress } from "@/components/ui/progress";
import { ArrowLeft, ArrowRight } from "lucide-react";
import { useState } from "react";

type Question = {
  id: number;
  question: string;
  options: string[];
};

const dummyQuestions: Question[] = [
  {
    id: 1,
    question: "İstanbul hangi yılda fethedildi?",
    options: ["1453", "1923", "1071", "1402"],
  },
  {
    id: 2,
    question: "Cumhuriyet hangi yıl ilan edildi?",
    options: ["1919", "1920", "1923", "1938"],
  },
];

const indexToLetter: Record<number, string> = {
  0: "A",
  1: "B",
  2: "C",
  3: "D",
};

export function Learn() {
  const [questions] = useState<Question[]>(dummyQuestions);
  const [currentIndex, setCurrentIndex] = useState(0);

  const currentQuestion = questions[currentIndex];

  const progress = ((currentIndex + 1) * 100) / questions.length;

  const nextQuestion = () => {
    if (currentIndex + 1 === questions.length) return;

    setCurrentIndex((prevValue) => prevValue + 1);
  };

  const previousQuestion = () => {
    if (currentIndex === 0) return;

    setCurrentIndex((prevValue) => prevValue - 1);
  };

  return (
    <main className="flex min-h-screen w-full flex-col items-center justify-center gap-4 bg-gray-100">
      <div className="mb-18 w-1/3">
        <div className="my-2 flex w-full justify-center">İlerleme</div>
        <Progress value={progress} className="w-full" />
      </div>

      <div className="flex w-1/2 justify-between">
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

      <div className="flex h-fit min-h-64 w-1/2 flex-col gap-4 rounded-md bg-white p-4 shadow">
        <h1 className="font-semibold">{currentIndex + 1}. Soru</h1>

        <p>{currentQuestion.question}</p>
      </div>

      <div className="grid w-2/3 grid-cols-2 gap-4">
        {currentQuestion.options.map((option, i) => (
          <button className="rounded-md bg-white p-4 shadow hover:bg-gray-100">
            {indexToLetter[i]}) {option}
          </button>
        ))}
      </div>
    </main>
  );
}
