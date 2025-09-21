import { useGetExplore } from "@/data/query";
import { TopicContent } from "./topic-content";
import { useNavigate } from "react-router";

export function Explore() {
  const topics = useGetExplore();

  const navigate = useNavigate();

  return (
    <div
      id="kesfet"
      className="my-24 flex w-full flex-col items-center justify-center gap-12 px-4"
    >
      <div className="space-y-2 text-center">
        <h2 className="text-2xl font-extrabold">Keşfet</h2>

        <p className="max-w-prose">
          Bu alandan başka kullanıcılar tarafından oluşturulmuş konuları
          inceleyebilirsin.
        </p>
      </div>

      <div className="xs:grid-cols-2 grid w-full max-w-3/4 grid-cols-1 justify-items-center gap-12 sm:grid-cols-4">
        {topics.data?.list?.map((topic) => (
          <div
            key={topic.id}
            className="relative h-96 w-52 rounded-md bg-gray-400"
          >
            <button
              className="h-full w-full"
              onClick={() => navigate(`/kaydir/${topic.id}`)}
            >
              <img
                className="h-full w-full rounded-md transition-opacity hover:opacity-80"
                loading="lazy"
                // @ts-ignore
                src={`${import.meta.env.VITE_BACKEND_URL}/images/${topic?.imageUrl}`}
              />
            </button>

            <TopicContent description={topic.description} id={topic.id ?? ""} />
          </div>
        ))}
      </div>
    </div>
  );
}
