# Monorepo and local development topology

Status: Current baseline, with Compose scope open  
Recorded: 2026-11-23

## Context

The repository contains the API, Worker, frontend and a combined development launcher. The structure guide proposes native app processes and a PostgreSQL-only container. NFR-D01 expects one-command local Compose startup.

## Decision

Use one repository and the existing native launcher for the current scaffold. Keep Aspire and additional mandatory services outside the baseline. Evaluate a database-only default and an opt-in full-stack profile when PostgreSQL and the application images exist.

## Consequences

Contributors can start the current API and frontend with one command and no database setup. The database topology and full-stack Compose release check still require a decision. [Local development](../../development/setup.md)
