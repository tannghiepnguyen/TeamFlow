import { Link, useMatchRoute } from "@tanstack/react-router";
import { Layers3, LayoutGrid } from "lucide-react";
import { useRef, type ReactNode } from "react";

import {
  Sidebar,
  SidebarContent,
  SidebarHeader,
  SidebarInset,
  SidebarMenu,
  SidebarMenuButton,
  SidebarMenuItem,
  SidebarProvider,
  SidebarTrigger,
  useSidebar,
} from "#/components/ui/sidebar";

export function AppShell({ children }: { children: ReactNode }) {
  return (
    <SidebarProvider>
      <AppShellLayout>{children}</AppShellLayout>
    </SidebarProvider>
  );
}

function AppShellLayout({ children }: { children: ReactNode }) {
  const matchRoute = useMatchRoute();
  const { isMobile, open, openMobile, setOpenMobile } = useSidebar();
  const trigger = useRef<HTMLButtonElement>(null);

  function closeMobileSidebar() {
    setOpenMobile(false);
  }

  return (
    <>
      <a
        href="#main-content"
        className="sr-only focus:not-sr-only focus:fixed focus:top-3 focus:left-3 focus:z-50 focus:rounded-md focus:bg-white focus:p-3"
      >
        Skip to main content
      </a>
      <Sidebar
        collapsible="icon"
        className="border-sidebar-border"
        onCloseAutoFocus={(event) => {
          event.preventDefault();
          trigger.current?.focus();
        }}
      >
        <SidebarHeader>
          <SidebarMenu>
            <SidebarMenuItem>
              <SidebarMenuButton
                asChild
                size="lg"
                tooltip="TeamFlow"
                className="text-base font-extrabold tracking-tight text-sidebar-foreground! group-data-[collapsible=icon]:p-0!"
              >
                <Link to="/" onClick={closeMobileSidebar}>
                  <span className="grid size-8 shrink-0 place-items-center rounded-lg bg-sidebar-primary text-sidebar-primary-foreground">
                    <Layers3 aria-hidden="true" className="size-4" />
                  </span>
                  <span className="group-data-[collapsible=icon]:sr-only">TeamFlow</span>
                </Link>
              </SidebarMenuButton>
            </SidebarMenuItem>
          </SidebarMenu>
        </SidebarHeader>
        <SidebarContent>
          <nav aria-label="Main navigation" className="px-2">
            <SidebarMenu>
              <SidebarMenuItem>
                <SidebarMenuButton
                  asChild
                  size="lg"
                  isActive={Boolean(matchRoute({ to: "/" }))}
                  tooltip="Overview"
                  className="h-11 text-sidebar-foreground! group-data-[collapsible=icon]:justify-center"
                >
                  <Link to="/" activeOptions={{ exact: true }} onClick={closeMobileSidebar}>
                    <LayoutGrid aria-hidden="true" />
                    <span className="group-data-[collapsible=icon]:sr-only">Overview</span>
                  </Link>
                </SidebarMenuButton>
              </SidebarMenuItem>
            </SidebarMenu>
          </nav>
        </SidebarContent>
      </Sidebar>
      <SidebarInset className="min-w-0 bg-transparent">
        <div className="flex h-14 items-center gap-3 border-b border-sidebar-border px-4 sm:px-7">
          <SidebarTrigger
            ref={trigger}
            aria-expanded={isMobile ? openMobile : open}
            className="size-10"
          />
          <span className="font-extrabold tracking-tight md:hidden">TeamFlow</span>
        </div>
        <div
          id="main-content"
          tabIndex={-1}
          className="mx-auto w-full max-w-6xl px-5 py-9 sm:px-9 sm:py-14 lg:px-14"
        >
          {children}
        </div>
      </SidebarInset>
    </>
  );
}
