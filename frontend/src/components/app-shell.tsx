import { Link } from "@tanstack/react-router";
import { Layers3, LayoutGrid } from "lucide-react";
import type { ReactNode } from "react";

export function AppShell({ children }: { children: ReactNode }) {
  return (
    <div className="min-h-dvh lg:grid lg:grid-cols-[15rem_minmax(0,1fr)]">
      <a
        href="#main-content"
        className="sr-only focus:not-sr-only focus:fixed focus:top-3 focus:left-3 focus:z-50 focus:rounded-md focus:bg-white focus:p-3"
      >
        Skip to main content
      </a>
      <header className="border-b border-[#dbe8df] bg-[#f9fcfa] px-5 py-4 sm:px-7 lg:border-r lg:border-b-0 lg:py-8">
        <div className="flex items-center justify-between gap-4 lg:flex-col lg:items-stretch lg:gap-12">
          <Link
            to="/"
            className="flex w-fit items-center gap-2.5 rounded-md text-lg font-extrabold tracking-tight text-[#173a40]! focus-visible:outline-2 focus-visible:outline-offset-4 focus-visible:outline-[#2f6a4a]"
          >
            <span className="grid size-9 place-items-center rounded-xl bg-[#194539] text-white">
              <Layers3 aria-hidden="true" className="size-[1.125rem]" />
            </span>
            TeamFlow
          </Link>
          <nav aria-label="Main navigation">
            <Link
              to="/"
              activeOptions={{ exact: true }}
              className="flex min-h-11 items-center gap-2.5 rounded-lg px-3 text-sm font-bold text-[#194539]! hover:bg-[#eaf3ee] focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-[#2f6a4a]"
              activeProps={{ className: "bg-[#e5f1e9]" }}
            >
              <LayoutGrid aria-hidden="true" className="size-4" />
              Overview
            </Link>
          </nav>
        </div>
      </header>
      <main
        id="main-content"
        tabIndex={-1}
        className="mx-auto w-full max-w-6xl px-5 py-9 sm:px-9 sm:py-14 lg:px-14"
      >
        {children}
      </main>
    </div>
  );
}
