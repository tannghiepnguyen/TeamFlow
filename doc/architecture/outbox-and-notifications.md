# Outbox and notifications

P1 adds durable event delivery for issue assignment and status changes, with in-app notifications for assignment and comments. The repository contains `OutboxMessage`, `ProcessedEvent`, `Notification`, and a Worker scaffold. Processing and delivery remain planned.

## Write and delivery flow

1. A use case validates membership, the mutation, and the expected issue version where applicable.
2. Save the business row, any related issue activity entry, and the outbox record in the same PostgreSQL transaction.
3. The Worker claims available records in bounded batches using a lease or equivalent database locking. Track attempts, sanitized last error, and the next scheduled retry. Use capped exponential backoff and account for an expired worker lease.
4. The consumer writes its database side effect and the `(ConsumerName, EventId)` ledger entry in one transaction. A notification uniqueness constraint can also enforce one notification per recipient, event and type.
5. Complete the outbox record after successful handling. Duplicate delivery can occur if processing stops between the side effect and the completion update. Reprocessing must preserve one logical notification.

Use stable versioned event names such as `IssueAssigned.v1` and backward-compatible readers for messages already in storage. Keep payloads minimal and tenant-scoped. External email requires its own delivery and deduplication strategy.

## Client delivery

Persist notifications for recipient-only listing, unread count and mark-read operations. Polling provides a baseline. P1 proposes authorized SignalR hints, with the HTTP read model remaining authoritative. A separate Worker needs a delivery channel to reach connections held by the API. [Notification delivery decision](decisions/0003-notification-delivery.md)

P2 adds optional email, channel preferences, and operational handling of permanently failed events. Define preference defaults, the retry limit, failed-event inspection/replay, and retention before enabling these paths. Account-security email follows its separate policy.

Verify rollback of the business write also removes the event, retries survive a Worker restart, and duplicate processing produces one logical notification. Track queue age, attempts and failures without putting tenant IDs or payload data in metric labels.
