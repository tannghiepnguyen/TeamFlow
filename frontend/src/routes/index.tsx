import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/")({ component: Home });

function Home() {
  return (
    <div className="space-y-3">
      <h1 className="display-title text-4xl font-medium tracking-tight text-[#173a40] sm:text-5xl">
        Overview
      </h1>
      <p className="text-sm text-[#416166]">Workspace views are still in development.</p>
    </div>
  );
}
