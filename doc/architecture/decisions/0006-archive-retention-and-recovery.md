# Archive retention and recovery

Status: Open (source ADR-006)  
Recorded: 2026-11-23

## Context

The specification proposes workspace/project archival, soft deletion for comments and attachments, and restrictions on hard deletion of core records. Recovery must account for Identity, business data, events and external objects.

## Decision criteria

Specify archive and restore permissions, write restrictions on archived workspaces, historical read access, retention and purge timelines, and how audit data and object storage participate. Set RPO and RTO and verify isolated restoration before production rollout.

## Consequences

Retention choices affect schema relationships, user visibility, storage costs and authorization for recovery. Document release and restore procedures before enabling purge or production recovery. [Backup and restore](../../operations/backup-and-restore.md)
