export function Navbar() {
  return (
    <nav className="absolute top-6 right-4">
      <ul className="flex gap-x-6">
        <li>
          <button
            onClick={() => {
              const element = document.getElementById("nasil-kullanilir");
              element?.scrollIntoView({
                behavior: "smooth",
              });
            }}
          >
            Nasıl Kullanılır?
          </button>
        </li>
        <li>
          <button
            onClick={() => {
              const element = document.getElementById("kesfet");
              element?.scrollIntoView({
                behavior: "smooth",
              });
            }}
          >
            Keşfet
          </button>
        </li>
      </ul>
    </nav>
  );
}
