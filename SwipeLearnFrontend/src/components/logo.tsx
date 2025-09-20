import { Link } from "react-router";

export function Logo() {
  return (
    <Link to={"/"} className="absolute top-4 left-4 flex items-center gap-x-1">
      <img src="/mascot.png" width={50} height={50} />
      <div className="-space-y-2 text-center font-extrabold">
        <div className="text-tw-secondary">Swipe</div>
        <div>Learn</div>
      </div>
    </Link>
  );
}
