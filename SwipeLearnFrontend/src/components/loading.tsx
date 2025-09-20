export function Loading() {
  return (
    <main className="bg-tw-background flex min-h-screen w-full flex-col items-center justify-center gap-4">
      <div className="flex flex-col gap-4">
        <div className="mx-auto">
          <img src="/mascot.png" width={200} height={200} />
        </div>
        <h1 className="text-2xl font-semibold">
          Videoların 1-2 dakika içerisinde hazırlanmış olacak.
        </h1>
      </div>

      <p className="max-w-prose text-center">
        O sırada bu ekrandan ayrılabilirsin veya sana özel oluşturduğumuz kısa
        bilgilerle konuya güzel bir başlangıç yapabilirsin.
      </p>

      <div className="bg-tw-secondary h-4 w-4 animate-bounce rounded-full"></div>

      <div className="bg-tw-primary flex min-h-24 w-1/3 items-center justify-center rounded-md">
        <p>İstanbul 1453 yılında feth edilmiştir.</p>
      </div>

      <p className="text-sm text-gray-500">
        Videolar oluşturulduğunda otomatik olarak yönlendirileceksin.
      </p>
    </main>
  );
}
