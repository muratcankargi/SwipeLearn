import { Logo } from "@/components/logo";
import { Outlet } from "react-router";

export function Layout() {
  return (
    <main className="bg-tw-background relative flex min-h-screen w-full flex-col items-center justify-center">
      <Logo />

      <Outlet />
    </main>
  );
}
