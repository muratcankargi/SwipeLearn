import { Button } from "@/components/ui/button";
import { ArrowLeft, ArrowRight, NotebookPen } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { Link, useParams } from "react-router";

const CENTER_OF_SCREEN = window.innerWidth / 2 - 160;
const VIDEO_WIDTH = 320;
const VIDEO_HEIGHT = 600;

export function Swipe() {
  const params = useParams<{ id: string }>();

  const {
    nextVideo,
    previousVideo,
    currentVideoIndex,
    positions,
    videoUrls,
    videoRefs,
  } = useHandleVideoChanges();

  return (
    <main className="flex min-h-screen w-full flex-col items-center justify-center gap-4 bg-gray-100">
      <div className="grid w-full grid-cols-3 px-4">
        <div></div>
        <div className="flex justify-center gap-4">
          <Button
            onClick={previousVideo}
            size="icon"
            disabled={currentVideoIndex === 0}
          >
            <ArrowLeft />
          </Button>
          <Button
            onClick={nextVideo}
            size="icon"
            disabled={currentVideoIndex === videoUrls.length - 1}
          >
            <ArrowRight />
          </Button>
        </div>
        <div className="flex justify-end">
          <Link to={`/quiz/${params.id}`}>
            <Button>
              Quize Geç
              <NotebookPen />
            </Button>
          </Link>
        </div>
      </div>

      <div className="flex w-full overflow-x-hidden">
        {videoUrls.map((videoUrl, i) => (
          <video
            key={videoUrl}
            ref={videoRefs[i]}
            src={videoUrl}
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
    </main>
  );
}

function useHandleVideoChanges() {
  const [videoUrls] = useState(["/video1.mp4", "/video2.mp4", "/video3.mp4"]);

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
