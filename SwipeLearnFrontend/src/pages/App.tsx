import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { usePostTopic } from "@/data/query";
import { zodResolver } from "@hookform/resolvers/zod";
import { ArrowRight } from "lucide-react";
import { useForm } from "react-hook-form";
import { useNavigate } from "react-router";
import z from "zod";
import { toast } from "sonner";

const schema = z.object({
  topic: z
    .string()
    .min(1, "Bu alan zorunludur.")
    .max(512, "Maksimum 512 karakter girebilirsiniz."),
});

type FormData = z.infer<typeof schema>;

export function App() {
  const methods = useForm({ resolver: zodResolver(schema) });

  const navigate = useNavigate();

  const mutation = usePostTopic();

  const onSubmit = (data: FormData) => {
    console.log("Data: ", data);

    mutation.mutate(
      { body: { description: data.topic } },
      {
        onSuccess: (data) => {
          navigate(`/hazirlanma/${data.id}`);
        },
        onError: () => {
          toast.error("Bir şeyler ters gitti.");
        },
      },
    );
  };

  return (
    <div className="flex w-full flex-col items-center gap-8">
      <div className="flex flex-col gap-4">
        <div className="mx-auto">
          <img src="/mascot.png" width={200} height={200} />
        </div>
        <h1 className="text-4xl font-extrabold">Ne öğrenmek istiyorsun?</h1>
      </div>

      <form
        className="flex w-1/2 flex-col items-center justify-center gap-4"
        onSubmit={methods.handleSubmit(onSubmit)}
      >
        <div className="flex min-h-36 w-full flex-col">
          <Textarea
            required
            placeholder="İstanbul'un fethi"
            className="bg-tw-primary flex-1"
            {...methods.register("topic")}
          />
          {methods.formState.errors.topic && (
            <p className="mt-1 text-sm text-red-500">
              {methods.formState.errors.topic.message}
            </p>
          )}
        </div>
        <Button
          disabled={methods.watch("topic")?.length === 0 || mutation.isPending}
          type="submit"
          className="bg-tw-secondary hover:bg-tw-secondary/90 ml-auto"
          size={"lg"}
        >
          İlerle
          <ArrowRight />
        </Button>
      </form>
    </div>
  );
}
