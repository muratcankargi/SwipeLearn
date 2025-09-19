import { Button } from "@/components/ui/button";
import { ArrowLeft, ArrowRight } from "lucide-react";
import { useState } from "react";
import { useParams } from "react-router";

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

  const swipeLeft = () => {
    setStyles((prevValues) => {
      const newStyles = prevValues.map((prevValue) => ({
        x: prevValue.x - 640,
      }));

      return newStyles;
    });
  };

  const swipeRight = () => {
    setStyles((prevValues) => {
      const newStyles = prevValues.map((prevValue) => ({
        x: prevValue.x + 640,
      }));

      return newStyles;
    });
  };

  return (
    <main className="flex min-h-screen w-full flex-col items-center justify-center gap-4 bg-gray-100">
      <div className="flex gap-4">
        <Button onClick={swipeLeft} size={"icon"}>
          <ArrowLeft />
        </Button>
        <Button onClick={swipeRight} size={"icon"}>
          <ArrowRight />
        </Button>
      </div>

      <div className="flex w-full overflow-x-hidden border border-black">
        <div
          style={{
            translate: styles[0].x,
            width: videoWidth,
            height: videoHeight,
          }}
          className="ease rounded-md bg-black transition-transform"
        ></div>
        <div
          style={{
            translate: styles[1].x,
            width: videoWidth,
            height: videoHeight,
          }}
          className="ease rounded-md bg-black transition-transform"
        ></div>

        <div
          style={{
            translate: styles[2].x,
            width: videoWidth,
            height: videoHeight,
          }}
          className="ease rounded-md bg-black transition-transform"
        ></div>
      </div>
    </main>
  );
}
