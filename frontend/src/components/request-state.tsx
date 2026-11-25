import { AlertCircle, LoaderCircle } from "lucide-react";
import { Button } from "#/components/ui/button";
import { ApiError } from "#/lib/api";

export function LoadingState({ label }: { label: string }) {
  return (
    <div
      role="status"
      className="flex min-h-24 items-center gap-3 text-sm font-semibold text-[#416166]"
    >
      <LoaderCircle aria-hidden="true" className="size-5 text-[#2f6a4a] motion-safe:animate-spin" />
      {label}
    </div>
  );
}

export function ErrorState({
  error,
  onRetry,
  title = "Something went wrong",
}: {
  error: unknown;
  onRetry: () => void;
  title?: string;
}) {
  let message = "Check your connection and try again.";
  if (error instanceof ApiError) {
    if (error.status === 401) message = "Sign in to continue.";
    if (error.status === 403) message = "You don’t have access to this resource.";
    if (error.status === 404) message = "The requested resource could not be found.";
  }

  return (
    <div className="space-y-4 py-3">
      <div role="alert" className="flex items-start gap-3">
        <AlertCircle aria-hidden="true" className="mt-0.5 size-5 shrink-0 text-[#a43c2d]" />
        <div>
          <p className="font-bold text-[#173a40]">{title}</p>
          <p className="mt-1 text-sm text-[#416166]">{message}</p>
        </div>
      </div>
      <Button type="button" variant="outline" onClick={onRetry}>
        Try again
      </Button>
    </div>
  );
}
