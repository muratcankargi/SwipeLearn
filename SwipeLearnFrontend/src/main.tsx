import ReactDOM from "react-dom/client";
import { createBrowserRouter } from "react-router";
import { RouterProvider } from "react-router/dom";
import { App } from "./pages/App";
import { Swipe } from "./pages/Swipe";
import { Learn } from "./pages/Learn";

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
  },
  {
    path: "/kaydir/:id",
    element: <Swipe />,
  },
  {
    path: "/ogren/:id",
    element: <Learn />,
  },
]);

const root = document.getElementById("root")!;

ReactDOM.createRoot(root).render(<RouterProvider router={router} />);
