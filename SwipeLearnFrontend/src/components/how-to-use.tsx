import { BookOpen, MonitorStop, NotebookPen, Repeat2 } from "lucide-react";

const steps = [
  {
    icon: BookOpen,
    text: "İlgini çeken herhangi bir konuyu yaz ve öğrenmeye başla.",
  },
  {
    icon: MonitorStop,
    text: "Girdiğin konuya özel kısa ve anlaşılır videolar hazırlanır.",
  },
  {
    icon: NotebookPen,
    text: "Videoları izledikten sonra soruları çözerek kendini test et.",
  },
  {
    icon: Repeat2,
    text: "Öğrenmeni pekiştirmek için bu adımları istediğin kadar tekrarla.",
  },
];

export function HowToUse() {
  return (
    <div
      className="max-w-8xl mx-auto mt-24 mb-24 grid grid-cols-2 gap-24"
      id="nasil-kullanilir"
    >
      {steps.map((step, index) => {
        const Icon = step.icon;
        return (
          <div
            key={index}
            className="relative flex flex-col items-center gap-4"
          >
            <div className="bg-tw-secondary absolute -top-6 left-0 flex h-7 w-7 items-center justify-center rounded-full text-white">
              {index + 1}
            </div>
            <Icon className="h-12 w-12" />
            <p className="max-w-64 text-center text-lg">{step.text}</p>
          </div>
        );
      })}
    </div>
  );
}
