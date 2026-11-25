import { defineConfig, lazyPlugins, loadEnv } from "vite-plus";
import { devtools } from "@tanstack/devtools-vite";

import { tanstackStart } from "@tanstack/react-start/plugin/vite";

import viteReact from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import { nitro } from "nitro/vite";

const config = defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "API_PROXY_TARGET");

  return {
    fmt: {},
    lint: {
      jsPlugins: [{ name: "vite-plus", specifier: "vite-plus/oxlint-plugin" }],
      rules: { "vite-plus/prefer-vite-plus-imports": "error" },
      options: { typeAware: true, typeCheck: true },
    },
    resolve: { tsconfigPaths: true },
    plugins: lazyPlugins(() => [
      devtools(),
      nitro({
        devProxy: {
          "/api/**": {
            target: env.API_PROXY_TARGET || "https://localhost:7045",
            changeOrigin: true,
            secure: false,
          },
        },
        rollupConfig: { external: [/^@sentry\//] },
      }),
      tailwindcss(),
      tanstackStart(),
      viteReact(),
    ]),
  };
});

export default config;
