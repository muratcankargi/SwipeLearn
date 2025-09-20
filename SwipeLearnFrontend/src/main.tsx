import ReactDOM from "react-dom/client";
import { createBrowserRouter } from "react-router";
import { RouterProvider } from "react-router/dom";
import { App } from "./pages/App";
import { Swipe } from "./pages/Swipe";
import { Quiz } from "./pages/Quiz";
import { Waiting } from "./pages/Waiting";
import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import { Toaster } from "sonner";
import { Layout } from "./pages/Layout";

const router = createBrowserRouter([
  {
    path: "/",
    element: <Layout />, // tüm sayfaları sarmalayan layout
    children: [
      {
        index: true, // "/" route
        element: <App />,
      },
      {
        path: "hazirlanma/:id",
        element: <Waiting />,
      },
      {
        path: "kaydir/:id",
        element: <Swipe />,
      },
      {
        path: "quiz/:id",
        element: <Quiz />,
      },
    ],
  },
]);

const root = document.getElementById("root")!;

const queryClient = new QueryClient();

ReactDOM.createRoot(root).render(
  <QueryClientProvider client={queryClient}>
    <Toaster richColors />
    <RouterProvider router={router} />
  </QueryClientProvider>,
);
