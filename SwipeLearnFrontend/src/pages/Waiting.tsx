import { WaitingInfos } from "@/components/waiting-infos";

export function Waiting() {
  return (
    <>
      <div className="flex flex-col gap-4">
        <div className="mx-auto">
          <img src="/mascot.png" width={200} height={200} />
        </div>
        <h1 className="text-2xl font-semibold">
          Videoların 1-2 dakika içerisinde hazırlanmış olacak.
        </h1>
      </div>

      <p className="my-2 max-w-prose text-center">
        O sırada bu ekrandan ayrılabilirsin veya sana özel oluşturduğumuz kısa
        bilgilerle konuya güzel bir başlangıç yapabilirsin.
      </p>

      <div className="bg-tw-secondary my-2 h-4 w-4 animate-bounce rounded-full"></div>

      <WaitingInfos />

      <p className="text-sm text-gray-500">
        Videolar oluşturulduğunda otomatik olarak yönlendirileceksin.
      </p>
    </>
  );
}
