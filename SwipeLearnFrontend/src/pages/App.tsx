import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowRight, Brain } from "lucide-react";
import { useForm } from "react-hook-form";
import z from "zod";

const schema = z.object({
  topic: z
    .string()
    .min(1, "Bu alan zorunludur.")
    .max(512, "Maksimum 512 karakter girebilirsiniz."),
});

type FormData = z.infer<typeof schema>;

export function App() {
  const methods = useForm({ resolver: zodResolver(schema) });

  const onSubmit = (data: FormData) => {
    console.log("Data: ", data);
  };

  return (
    <main className="flex min-h-screen w-full items-center justify-center bg-gray-100">
      <div className="flex w-full flex-col items-center gap-12">
        <div className="flex flex-col gap-4">
          <div className="mx-auto">
            <Brain className="h-20 w-20" />
          </div>
          <h1 className="text-4xl font-extrabold">Ne öğrenmek istiyorsun?</h1>
        </div>

        <form
          className="flex w-1/2 flex-col items-center justify-center gap-4"
          onSubmit={methods.handleSubmit(onSubmit)}
        >
          <Textarea
            placeholder="İstanbul'un fethi"
            className="h-36 bg-gray-200"
            {...methods.register("topic")}
          />
          <Button type="submit" className="ml-auto" size={"lg"}>
            İlerle
            <ArrowRight />
          </Button>
        </form>
      </div>
    </main>
  );
}
