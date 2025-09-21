export function Explore() {
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

      <div className="grid w-full max-w-3/4 grid-cols-1 justify-items-center gap-12 sm:grid-cols-4">
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
        <div className="h-96 w-48 rounded-md bg-gray-400"></div>
      </div>
    </div>
  );
}
