import { Button } from "@/components/ui/button";
import { ArrowLeft, ArrowRight, NotebookPen } from "lucide-react";
import { useEffect, useRef, useState } from "react";
import { Link, useParams } from "react-router";

export function Swipe() {
  const params = useParams<{ id: string }>();

  const centerOfScreen = window.innerWidth / 2 - 160;

  const videoWidth = 320;
  const videoHeight = 600;

  const [styles, setStyles] = useState([
    { x: centerOfScreen },
    { x: centerOfScreen + 320 },
    { x: centerOfScreen + 640 },
  ]);

  const [currentVideoIndex, setCurrentVideoIndex] = useState(0);

  // videolara ref veriyoruz
  const videoRefs = [
    useRef<HTMLVideoElement>(null),
    useRef<HTMLVideoElement>(null),
    useRef<HTMLVideoElement>(null),
  ];

  const nextVideo = () => {
    if (currentVideoIndex === 2) return;

    setCurrentVideoIndex((prevValue) => prevValue + 1);

    setStyles((prevValues) =>
      prevValues.map((prevValue) => ({
        x: prevValue.x - 640,
      })),
    );
  };

  const previousVideo = () => {
    if (currentVideoIndex === 0) return;

    setCurrentVideoIndex((prevValue) => prevValue - 1);

    setStyles((prevValues) =>
      prevValues.map((prevValue) => ({
        x: prevValue.x + 640,
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
        // ref.current.currentTime = 0;
      }
    });
  }, [currentVideoIndex]);

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
            disabled={currentVideoIndex === 2}
          >
            <ArrowRight />
          </Button>
        </div>
        <div className="flex justify-end">
          <Link to={`/ogren/${params.id}`}>
            <Button>
              Quize Geç
              <NotebookPen />
            </Button>
          </Link>
        </div>
      </div>

      <div className="flex w-full overflow-x-hidden">
        <video
          ref={videoRefs[0]}
          src="/video1.mp4"
          style={{
            translate: styles[0].x,
            width: videoWidth,
            height: videoHeight,
            scale: currentVideoIndex === 0 ? 1 : 0.8,
          }}
          className="ease rounded-md bg-black transition-transform"
          controls
          controlsList="nofullscreen" // TODO: Bunu böyle yapmak yerine translate'i sıfırlayalım
        />

        <video
          ref={videoRefs[1]}
          src="/video2.mp4"
          style={{
            translate: styles[1].x,
            width: videoWidth,
            height: videoHeight,
            scale: currentVideoIndex === 1 ? 1 : 0.8,
          }}
          className="ease rounded-md bg-black transition-transform"
          controls
          controlsList="nofullscreen"
        />

        <video
          ref={videoRefs[2]}
          src="/video3.mp4"
          style={{
            translate: styles[2].x,
            width: videoWidth,
            height: videoHeight,
            scale: currentVideoIndex === 2 ? 1 : 0.8,
          }}
          className="ease rounded-md bg-black transition-transform"
          controls
          controlsList="nofullscreen"
        />
      </div>
    </main>
  );
}
