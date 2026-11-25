import { z } from "zod";

const safeMethods = new Set(["GET", "HEAD", "OPTIONS", "TRACE"]);
const tokenPath = "/api/antiforgery/token";
const tokenResponse = z.object({ requestToken: z.string().min(1) });
const problemDetails = z.object({
  title: z.string().optional(),
  detail: z.string().optional(),
  errors: z.record(z.string(), z.array(z.string())).optional(),
});

export class ApiError extends Error {
  constructor(
    readonly status: number,
    readonly problem?: z.infer<typeof problemDetails>,
  ) {
    super(problem?.title ?? `Request failed (${status})`);
    this.name = "ApiError";
  }
}

type ApiPath = `/api/${string}`;

async function send(path: ApiPath, options: RequestInit = {}): Promise<Response> {
  const headers = new Headers(options.headers);
  headers.set("Accept", "application/json, application/problem+json");

  const response = await fetch(path, {
    ...options,
    headers,
    credentials: "same-origin",
    redirect: "error",
  });

  if (!response.ok) {
    const contentType = response.headers.get("Content-Type") ?? "";
    const body: unknown = contentType.includes("json")
      ? await response.json().catch(() => undefined)
      : undefined;
    throw new ApiError(response.status, problemDetails.safeParse(body).data);
  }

  return response;
}

// Concurrent writes share the token request that establishes the antiforgery cookie.
// Once it settles, later writes obtain a token for the current session.
let pendingToken: Promise<string> | undefined;

function getToken(): Promise<string> {
  pendingToken ??= send(tokenPath, { cache: "no-store" })
    .then(async (response) => tokenResponse.parse(await response.json()).requestToken)
    .finally(() => {
      pendingToken = undefined;
    });
  return pendingToken;
}

export async function apiRequest(path: ApiPath, options: RequestInit = {}): Promise<Response> {
  if (typeof window === "undefined") {
    throw new Error("The browser API client requires a browser context.");
  }

  const url = new URL(path, window.location.origin);
  if (url.origin !== window.location.origin || !url.pathname.startsWith("/api/")) {
    throw new Error("API requests must target the same-origin /api/ prefix.");
  }

  const headers = new Headers(options.headers);
  if (!safeMethods.has((options.method ?? "GET").toUpperCase())) {
    options.signal?.throwIfAborted();
    headers.set("X-CSRF-TOKEN", await getToken());
  }

  return send(path, { ...options, headers });
}

export async function apiJson<T>(path: ApiPath, schema: z.ZodType<T>, options?: RequestInit) {
  const response = await apiRequest(path, options);
  return schema.parse(await response.json());
}
