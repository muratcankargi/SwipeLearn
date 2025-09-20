import { useEffect, useState } from "react";

const DEFAULT_TIMER = 20;

export function WaitingInfos() {
  const infos = [
    "İstanbul 1453 yılında fethedilmiştir.",
    "Fethi gerçekleştiren padişah II. Mehmed, Fatih Sultan Mehmed unvanını almıştır.",
    "Kuşatma 6 Nisan 1453’te başlamış ve 29 Mayıs’ta sona ermiştir.",
    "Fetih sırasında Bizans İmparatoru XI. Konstantinos son ana kadar şehri savunmuştur.",
    "Osmanlı ordusu yaklaşık 80.000 kişiden oluşuyordu.",
    "Fatih Sultan Mehmed, devrin en büyük toplarından olan 'Şahi' toplarını kullandı.",
    "Haliç’e zincir çekilmesine rağmen Osmanlı donanması karadan gemi yürütmüştür.",
    "Fetih ile birlikte Orta Çağ kapanmış, Yeni Çağ başlamıştır.",
    "İstanbul, Osmanlı Devleti’nin başkenti olmuştur.",
    "Ayasofya camiye çevrilerek Osmanlı hâkimiyetinin sembolü haline gelmiştir.",
  ];

  const [timer, setTimer] = useState(DEFAULT_TIMER);
  const [infoIndex, setInfoIndex] = useState(0);

  const currentInfo = infos[infoIndex];

  useEffect(() => {
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
  }, []);

  return (
    <div className="bg-tw-primary relative flex min-h-24 w-1/3 items-center justify-center rounded-md">
      <div className="absolute top-1 left-2 text-sm">{timer}</div>
      <p key={currentInfo} className="fade-in max-w-[80%] text-center">
        {currentInfo}
      </p>
    </div>
  );
}
