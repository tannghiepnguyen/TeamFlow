# Issue board ordering

Status: Open (source ADR-004)  
Recorded: 2026-11-23

## Context

The issue catalog specifies decimal `Position` and a separate optimistic version. The board needs deterministic ordering and safe concurrent moves.

## Options

Numeric ranks fit the existing model and support inserts between items. Repeated inserts into the same gap can require rebalancing. An adjacency list represents neighbors directly and needs a strategy for consistent traversal and concurrent updates.

## Decision criteria

Define the board's ordering scope and stable tie-breaker, how concurrent moves and stale versions behave, and whether and how ranks rebalance. Verify reorder integrity under contention before selecting an approach. [Schema](../../database/schema.md)
