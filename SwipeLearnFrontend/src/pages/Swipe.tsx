import { TakeNotes } from "@/components/take-notes";
import { Button } from "@/components/ui/button";
import { ArrowLeft, ArrowRight, NotebookPen } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { Link, useParams } from "react-router";

const CENTER_OF_SCREEN = window.innerWidth / 2 - 160;
const VIDEO_WIDTH = 320;
const VIDEO_HEIGHT = 600;

export function Swipe({ videoUrls }: { videoUrls: string[] }) {
  const params = useParams<{ id: string }>();

  const { nextVideo, previousVideo, currentVideoIndex, positions, videoRefs } =
    useHandleVideoChanges({ videoUrls });

  return (
    <>
      <div className="my-4 mt-24 grid w-full grid-cols-3 px-4 sm:mt-0">
        <Link className="flex items-center gap-x-2" to={`/`}>
          <ArrowLeft className="mt-1" />
          Ana Sayfaya Dön
        </Link>

        <div className="flex justify-center gap-4">
          <button
            className="bg-primary text-primary-foreground hover:bg-primary/90 rounded-md px-2 transition-transform hover:scale-110 disabled:opacity-50"
            onClick={previousVideo}
            disabled={currentVideoIndex === 0}
          >
            <ArrowLeft className="h-7 w-7" />
          </button>
          <button
            className="bg-primary hover:bg-primary/90 text-primary-foreground rounded-md px-2 transition-transform hover:scale-110 disabled:opacity-50"
            onClick={nextVideo}
            disabled={currentVideoIndex === videoUrls.length - 1}
          >
            <ArrowRight className="h-7 w-7" />
          </button>
        </div>
        <div className="flex justify-end">
          <Link to={`/quiz/${params.id}`}>
            <Button className="bg-tw-secondary hover:bg-tw-secondary/90">
              Quize Geç
              <NotebookPen />
            </Button>
          </Link>
        </div>
      </div>

      <div className="relative mb-24 flex w-full overflow-x-hidden sm:mb-0">
        {videoUrls.map((videoUrl, i) => (
          <video
            key={videoUrl}
            ref={videoRefs[i]}
            src={`${import.meta.env.VITE_BACKEND_URL}${videoUrl}`}
            style={{
              translate: positions[i].x,
              width: VIDEO_WIDTH,
              height: VIDEO_HEIGHT,
              scale: currentVideoIndex === i ? 1 : 0.8,
            }}
            className="ease rounded-md bg-black transition-transform"
            controls
            controlsList="nofullscreen" // TODO: Bunu böyle yapmak yerine translate'i sıfırlayalım
          />
        ))}
      </div>

      <TakeNotes />
    </>
  );
}

function useHandleVideoChanges({ videoUrls }: { videoUrls: string[] }) {
  const initialPositions = videoUrls.map((_, i) => {
    return { x: CENTER_OF_SCREEN + i * VIDEO_WIDTH };
  });

  const [positions, setPositions] = useState(initialPositions);
  const [currentVideoIndex, setCurrentVideoIndex] = useState(0);

  const videoRefs = videoUrls.map((_) => useRef<HTMLVideoElement>(null));

  const nextVideo = () => {
    if (currentVideoIndex === videoUrls.length - 1) return;

    setCurrentVideoIndex((prevValue) => prevValue + 1);

    setPositions((prevValues) =>
      prevValues.map((prevValue) => ({
        x: prevValue.x - VIDEO_WIDTH * 2,
      })),
    );
  };

  const previousVideo = () => {
    if (currentVideoIndex === 0) return;

    setCurrentVideoIndex((prevValue) => prevValue - 1);

    setPositions((prevValues) =>
      prevValues.map((prevValue) => ({
        x: prevValue.x + VIDEO_WIDTH * 2,
      })),
    );
  };

  useEffect(() => {
    videoRefs.forEach((ref, index) => {
      if (!ref.current) return;

      if (index === currentVideoIndex) {
        ref.current.play();
      } else {
        ref.current.pause();
      }
    });
  }, [currentVideoIndex]);

  return {
    nextVideo,
    previousVideo,
    positions,
    currentVideoIndex,
    videoUrls,
    videoRefs,
  };
}
