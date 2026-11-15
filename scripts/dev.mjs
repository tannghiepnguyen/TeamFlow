import { spawn } from "node:child_process";
import { fileURLToPath } from "node:url";

const root = fileURLToPath(new URL("../", import.meta.url));
const windows = process.platform === "win32";
const children = [];
let stopping = false;

async function stop(code = 0) {
  if (stopping) return;
  stopping = true;
  await Promise.all(children.map((child) => new Promise((resolve) => {
    if (!child.pid || child.exitCode !== null) return resolve();
    if (windows) {
      const killer = spawn("taskkill", ["/pid", String(child.pid), "/T", "/F"], {
        stdio: "ignore",
        windowsHide: true,
      });
      killer.once("error", resolve);
      killer.once("exit", resolve);
    } else {
      try { process.kill(-child.pid, "SIGTERM"); } catch {}
      resolve();
    }
  })));
  process.exit(code);
}

function start(name, command, args, cwd) {
  const child = spawn(command, args, {
    cwd,
    stdio: "inherit",
    detached: !windows,
    windowsHide: true,
  });
  children.push(child);
  child.once("error", (error) => {
    console.error(`Could not start ${name}: ${error.message}`);
    void stop(1);
  });
  child.once("exit", (code) => {
    if (!stopping) {
      console.log(`${name} stopped. Shutting down both apps.`);
      void stop(code ?? 1);
    }
  });
}

process.on("SIGINT", () => void stop());
process.on("SIGTERM", () => void stop());

console.log("Starting frontend (http://localhost:3000) and backend (https://localhost:7045).");
console.log("Press Ctrl+C to stop both apps.");
start("backend", "dotnet", ["watch", "run", "--project", "backend/src/TeamFlow.API", "--launch-profile", "https"], root);
start("frontend", windows ? "vp.exe" : "vp", ["run", "dev"], fileURLToPath(new URL("../frontend/", import.meta.url)));
