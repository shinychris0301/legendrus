# v1.18 Step 2 — Balance Telemetry and Tutorial Funnel Review Foundation

## Status

Implemented in source for Unity 6000.5.9f1 verification.

This checkpoint adds a closed-test/local review path. It does **not** enable a
production analytics provider, network telemetry upload, automatic difficulty
changes, tutorial experiments, monetization activation, production signing, or
Google Play publishing.

## Scope implemented

- Engine-free `LegendsOfRus.BalanceReviewCore` assembly.
- Fixed-capacity process-memory observation buffer (128 default, 256 hard max).
- Coarse level lifecycle observations: start, win, fail.
- Level aggregation: attempts, wins, failures, retries, win/failure rate,
  average winning moves remaining, winning move margin, and average winning score.
- Tutorial funnel aggregation: shown -> completed using a deterministic integer
  key derived only from authored tutorial IDs.
- Review-only issue codes for:
  - insufficient samples,
  - high failure rate,
  - near-perfect win rate,
  - low winning move margin,
  - tutorial completion drop-off.
- Runtime wiring beside the existing consent-gated analytics path.
- Editor command to print the current process-local report while in Play Mode.
- Dedicated EditMode/PlayMode regression tests and an Editor self-test.

## Review policy

The current review rules are deliberately conservative and informational:

- Level sample minimum: 5 starts.
- High failure review: failure rate >= 65%.
- Near-perfect win review: win rate >= 95%.
- Low winning move-margin review: at least 3 wins and average winning remaining
  moves <= 8% of the authored move limit.
- Tutorial sample minimum: 3 shown events.
- Tutorial drop-off review: completion rate < 80%.

These thresholds create review flags only. They never mutate level data.

## Data-minimization boundary

The Step 2 buffer stores only enums and integers. It does not store:

- account IDs,
- email/password/recovery values,
- device unique identifiers,
- advertising identifiers,
- device model/name,
- free-form player text,
- IP/network metadata.

The buffer is not persisted and has no upload implementation.

## Runtime wiring

`RuntimeBootstrap` creates one `BalanceTelemetryReviewService`. `ScreenRouter`
records level start/win/fail observations, and `LevelTutorialPresenter` records
shown/completed tutorial observations. Production analytics remains routed
through `PrivacyAwareAnalytics` and is not activated by this foundation.

## Unity verification

1. Open in Unity 6000.5.9f1 and wait for compilation.
2. Run `Legends of Rus > Tests > Run v1.18 Step 2 Self-Tests`.
3. Run complete EditMode and PlayMode suites.
4. In Play Mode, exercise tutorial levels and several wins/fails/retries.
5. Run `Legends of Rus > QA > Print v1.18 Step 2 Local Balance Review`.
6. Review output for sanity; do not treat synthetic/local samples as production balance evidence.

## Exit boundary

Step 2 is eligible to close only after the exact candidate has retained Unity
compile, self-test, EditMode, PlayMode, and representative closed-test review
evidence. Production analytics transport and automatic balance changes remain
separate explicit activation decisions.
