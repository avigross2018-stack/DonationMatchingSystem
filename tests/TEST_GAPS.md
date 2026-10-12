# Specification and implementation gaps

This file separates passing characterization/regression coverage for the
implemented API from requirements in `docs/specification/donation-system-v3.md`.
No production business behavior was changed to hide these gaps.

## High severity

### Pending matches can overbook a donation or recipient

- **Evidence:** `DonationMatchingService.CalculateMatchingAmountAsync` subtracts
  only `COMPLETED` redemptions from the per-recipient cap. Creating a `PENDING`
  redemption does not reserve or decrement `Donation.RemainingAmount` or
  `Recipient.RemainingEligibility`.
- **Coverage:** matching integration tests prove the calculation and worker
  creation behavior for one recipient. The risk remains when multiple
  recipients are matched before completion.
- **Recommendation:** define a reservation invariant and enforce it atomically
  under row locks, or include pending commitments in all availability sums.

### Registration uniqueness has a check-then-insert race

- **Evidence:** `AuthService` queries by phone before inserting. `Users.Phone`
  is unique, but a concurrent losing request can surface an unhandled
  `DbUpdateException` as HTTP 500 instead of a controlled 409.
- **Coverage:** sequential duplicate registration is covered as 409. A safe
  concurrency contract is not treated as implemented behavior.
- **Recommendation:** translate the MySQL unique-key violation to the same
  conflict result, retaining the database constraint as the final authority.

### Inactive donors can still read their donation list

- **Evidence:** `DonationService.GetMyDonationsAsync` verifies only that a donor
  profile exists; unlike create/update/cancel it does not check
  `donor.User.IsActive`.
- **Coverage:** authorization and ownership filtering are covered. The inactive
  behavior is documented rather than made a desirable regression contract.
- **Recommendation:** enforce account activity consistently on sensitive reads.

### No redemption completion endpoint or atomic balance update

- **Evidence:** the implemented controllers expose auth, donation CRUD/cancel,
  and role probes only. The specification requires completing a redemption and
  decrementing both balances atomically.
- **Coverage:** not runnable against the current API; specification gap.
- **Recommendation:** implement an idempotent admin completion workflow with
  row locks and transaction-level invariant checks.

## Medium severity

### `MaxAmountPerRecipient` may exceed `TotalAmount`

- **Evidence:** DTO ranges and `DonationService` require both values to be
  positive, but do not compare them. The database check constraint also omits
  this relationship.
- **Coverage:** positive/range validation is covered; the specification-only
  comparison is not asserted as current behavior.
- **Recommendation:** add service validation and an aligned database invariant.

### `REJECTED` blocks future matching while `CANCELLED` does not

- **Evidence:** `RedemptionRepo.HasBlockedMatchAsync` checks `PENDING` and
  `REJECTED`; it ignores `CANCELLED` and `COMPLETED` (completed amounts only
  reduce the cap).
- **Coverage:** `TryCreateMatch_ExistingStatus_FollowsCurrentBlockingRules`
  characterizes all four statuses.
- **Recommendation:** obtain a product decision and name the policy explicitly.

### Worker failures have no retry or dead-letter mechanism

- **Evidence:** `DonationMatchingWorker` logs and consumes exceptions, then
  moves to the next in-memory channel event. Restart loses queued events.
- **Coverage:** normal `DonationCreated` and `RecipientRegistered` processing is
  covered with polling rather than fixed sleeps.
- **Recommendation:** use durable delivery or reconciliation/full-scan retry.

### Error responses are not uniform

- **Evidence:** the global handler emits Problem Details with `traceId`; model
  validation emits `ValidationProblemDetails`; login and several controller
  catch blocks return anonymous `{ message }` bodies; authentication middleware
  returns empty 401/403 responses.
- **Coverage:** each relevant current shape is asserted without pretending it
  is uniform.
- **Recommendation:** centralize safe error translation for API errors.

### Error contract lacks specification fields

- **Evidence:** the specification requires stable business `code` and
  `correlationId`; `GlobalExceptionHandler` emits `traceId` and no `code`.
- **Coverage:** current Problem Details fields are verified.
- **Recommendation:** define an error-code catalog and correlation propagation.

## Broader specification gaps

- The specification lists a broad `/api/v1` surface for users, profiles,
  recipients, population types, redemptions, and admin operations. None of
  those future routes are present; the regression suite targets implemented
  `/api/Auth`, `/api/donations`, and `/api/test` routes.
- The specification says the eligibility formula requires approval, while
  `EligibilityService` already fixes the formula to
  `max(0, base + children*500 - income*0.20)`, rounded AwayFromZero.
- There is no public population-type API, recipient profile API, audit API,
  health endpoint, password-change flow, or account-activity administration.
- The in-memory bounded event channel is process-local and provides no durable
  delivery guarantee.
