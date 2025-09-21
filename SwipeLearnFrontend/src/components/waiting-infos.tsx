import { useGetTopicShortInfo } from "@/data/query";
import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { LoadingIndicator } from "./ui/loading-indicator";

const DEFAULT_TIMER = 10;

export function WaitingInfos() {
  const params = useParams<{ id: string }>();

  const infosQuery = useGetTopicShortInfo({ query: { id: params.id } });

  const infos = infosQuery.data?.info ?? [];

  const [timer, setTimer] = useState(DEFAULT_TIMER);
  const [infoIndex, setInfoIndex] = useState(0);

  useEffect(() => {
    if (!infosQuery.data) return;

    const interval = setInterval(() => {
      setTimer((prevValue) => {
        if (prevValue === 1) {
          setInfoIndex((prevIndex) =>
            prevIndex === infos.length - 1 ? 0 : prevIndex + 1,
          );
          return DEFAULT_TIMER;
        }
        return prevValue - 1;
      });
    }, 1000);

    return () => clearInterval(interval);
  }, [infosQuery.data]);

  const currentInfo = infos[infoIndex] ?? undefined;

  return (
    <div className="bg-tw-primary relative my-2 flex min-h-24 w-1/3 items-center justify-center rounded-md">
      {currentInfo && (
        <div className="absolute top-1 left-2 text-sm">{timer}</div>
      )}

      <p key={currentInfo} className="fade-in max-w-[80%] text-center">
        {currentInfo}
      </p>

      {infosQuery.isLoading && <LoadingIndicator />}
    </div>
  );
}
