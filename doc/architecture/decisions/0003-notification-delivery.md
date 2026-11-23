# Notification delivery

Status: Proposed (source ADR-005)  
Recorded: 2026-11-23

## Context

Issue changes need durable asynchronous delivery, and a Worker running in another process has no direct access to SignalR connections held by the API.

## Proposed decision

Persist a versioned outbox event in the business transaction. Claim work with leases or database locking, retry with capped backoff, and deduplicate database effects. Persist notifications as the read model. Use polling as a baseline and define an authenticated Worker-to-API path or suitable fan-out when adding live hints. Define retry limits, retention, failure handling and external email deduplication before release.

## Consequences

Issue writes can proceed when the Worker is offline. Recipients eventually see notifications after recovery. Consumers and operational tooling need to handle repeat delivery, stale leases and events from older deployments. [Outbox and notifications](../outbox-and-notifications.md)
