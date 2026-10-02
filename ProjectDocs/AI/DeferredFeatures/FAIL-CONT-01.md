# FAIL-CONT-01 — Paid +5 moves and rewarded continuation

Status: **CONT-01 recovery and CONT-02A local reservation/refund foundation implemented; playable paid/ad continuation remains pending.** See [FailedRoundRecovery/R1](../FailedRoundRecovery/R1/TestResults.md). Existing disabled offer artwork is reused unchanged. Offer UI designs were added in Failure R4. User explicitly requested this feature on 2026-09-15 and placed implementation after UI design completion. This supersedes treating paid/ad continuation as permanently out of scope. Existing retry/chapter-map actions remain functional in the current visual build.

## 2026-09-29 RuStore and transaction update

User selected RuStore and requested our own backend. [CONT-02A results](../ContinuationTransactions/R1/TestResults.md) cover atomic local gem reservation plus unused entitlement, duplicate/stale rejection, decline/refund and separate-process recovery. The internal reservation API is unbound; no offer was enabled. A local RuStore server module verifies invoice contracts and journals wallet credits in tests; Unity is not connected and no real provider purchase was run. This is not a completed playable continuation feature.

Next establish stable backend identity/Unity transport and server round authority, then consume entitlements on the same attempt and integrate real Pay acknowledgement/refund handling. Prices, ad provider and ad reward policy remain unspecified. Existing art and free-retry behavior remain unchanged. The historical “backend changes not part of current header refinement” below applied to that old visual task, not the user's new explicit backend authorization.

## R4 design handoff

The user subsequently requested designing both offer cards during the UI pass. Failure R4 includes separate burgundy paid-moves and sapphire rewarded-ad artwork, native **disabled** `PaidOffer`/`AdOffer` buttons, live RU/EN titles and unavailable labels, an unknown-price dash with a green currency symbol, and an ad play symbol. They have no transaction listeners. Existing retry/chapter-map actions remain usable. No40-gem price or ad availability is implied.

When this task begins, bind these existing fields and replace their unavailable state from real balance/provider/attempt data. Reuse the R4 art and hierarchy; do not regenerate the designs automatically. Preserve the centered goals, corrected booster-label spacing, R5-restored original main panel, title-only side-ornament removal and dark tone. Visual evidence lives under `FailStyleAlignment/OffersAndLayoutR4/`.

## Development slot

First gameplay/monetization milestone after the agreed UI design pass, before release acceptance. See [development schedule](../DevelopmentSchedule.md). Do not request the same implementation authorization again. Balance choices and provider configuration may still need clarification based on what exists at that stage.

## 2026-09-29 foundation update

Normal zero-move failures now persist the settled board, identity, rules/content contract and goal/score state in the existing save journal. Relaunch recovers Failure. Retry or explicit exit finalizes the local loss once; retry creates a new attempt with the existing free-retry policy. Failed writes keep the in-memory decision and usable retry/exit controls. `InitializeRetained` reconstructs an input-locked board without another attempt/energy spend. It does not grant moves or enable offers. Separate-process verification and exact limits are recorded in the linked results.

Step 1 below has a provider-independent implementation. Steps 2–4, playable same-attempt continuation, atomic economic fulfillment, provider callbacks and Android interruption acceptance remain pending. No price or provider activation is inferred.

## Historical starting points before CONT-01

- `ScreenRouter` gameplay failure callback records `GameState.LastFailureResult` then routes to the outcome; current Failure does not retain a resumable board. `LevelFailureResult` copies goal progress/score, not the complete board.
- `ProductionFailView` has existing native retry and chapter-map actions; retry creates a new attempt and does not resume the lost board.
- `GameState.SpendGems` exists but alone does not make a spend plus board resume atomic or recoverable.
- `MonetizationService.TryRewardedContinue` already exists behind optional-ad consent and a runtime monetization port. Reuse it and inspect its durable reward/receipt contract; do not bypass it or equate ad opening/closing with verified completion.
- The fourth existing gameplay booster also adds moves. It remains a separate stock-based gameplay action, not the Failure purchase/ad entitlement.

## Implementation sequence

1. Retain or persist a resumable round when moves run out: full tile/special/obstacle state, goal progress, score, move count, selected level, attempt identity, booster state and any RNG/state needed for coherent continuation. Suspend input and clear pending animations at a stable point. Finalize loss/analytics only when the player declines/exits or the attempt terminates; avoid duplicate completion rewards and duplicate attempt events.
2. Add an idempotent continuation operation tied to a specific suspended attempt. Paid continuation spends configured gems and grants exactly five moves to that same board once. Handle insufficient balance, double taps, stale attempts, persistence failures, app interruption and retry safely; no charge without a recoverable grant.
3. Wire the existing rewarded continuation provider/consent/placement contract. Grant the configured continuation only after verified durable completion for the active attempt; cancellation, unavailable ads, timeout and provider failure do not resume or charge. Duplicate/late callbacks cannot grant twice or affect a different round. Never ship a success-simulating mock.
4. Complete integration of the R4 red paid-offer and blue rewarded-offer cards (independent assets/live text/native controls already prepared during UI design). Preserve retry and return behavior in an explicit, usable layout. Show balance, price and availability truthfully; localize RU/EN. Reference **40 gems** is a proposed balance setting, not an existing approved production price. Ad move count/limits must come from the chosen balance configuration; do not infer unlimited continuation.
5. Run the acceptance checks below and update CurrentTask/SessionLog with actual outcomes and remaining provider/device limitations.

## Acceptance checks

- Same board, score and goals survive resume; paid grant adds exactly five moves with one spend. No resetting the round, consuming another retry charge or granting completion rewards early.
- Exact balance, insufficient balance, repeated taps, multiple sequential continues and configured limits.
- Verified ad success, rejection/cancel/unavailable/timeout, duplicate callback, stale callback and navigation away; consent remains respected.
- App background/relaunch and interrupted persistence/transaction recovery; no orphaned charge or double grant.
- Matching/cascades/dead-board reshuffle after resume, win after resume, subsequent loss, retry/exit and analytics/reward idempotency.
- Live RU/EN offer UI, safe areas, readable price/move counts and actual Android touches.
- Unit and Unity lifecycle tests, provider sandbox integration and physical Android smoke test. Record any unavailable provider/device verification explicitly.

Not part of the current header refinement: new ad SDK installation, signing/package changes, production monetization rollout, backend changes or release distribution. Inspect applicable existing guards and configuration when integration begins.

## R7 layout handoff (2026-09-15)

Whole paid/ad sprites now use native proportions and aligned painted faces. Existing serialized offer controls and unavailable state remain. Chapter-map action is beside the booster heading, preserving the exposed lower main-panel ornament. Preserve this layout and the R6 title size when connecting continuation behavior. Latest visual evidence: `FailStyleAlignment/LayoutAndReadabilityR7/Fail_LayoutAndReadability_R7_Review.html`.

## R9 offer-art handoff (2026-09-15)

Latest canonical design uses paid_r9/ad_r9 with protected mesh fitting on the existing offer buttons. The full green faces and corners preserve uniform scale; plain upper spans fit shared324×140 bounds. Currency, price and play remain separate live objects with larger bounds. R8 lettering and the R7 return action are preserved. Integrate behavior into these serialized controls after UI completion; do not restore the earlier shallow-face art. Offers remain unavailable and no40-gem price is approved. Evidence: `FailStyleAlignment/OfferCardsR9/Fail_OfferCards_R9_Review.html`.

## R11 source-extraction handoff (2026-09-16)

Latest Failure UI uses original-reference paid_r11/ad_r11 isolated through Scenario, with native aspect ratios. Keep the existing serialized PaidOffer/AdOffer controls and live titles/status/price. The original gem, lightning and play triangle are now embedded in these isolated sprites; the old Currency/MovesSymbol images and PlaySymbol text are disabled to avoid duplicates. Re-enable only if the corresponding artwork is removed from the source sprite. Price remains a live dash; no40-gem price is approved. Offers stay disabled until the resumable-attempt/transaction/provider work above is complete. Reference captions and booster stock/functions remain. Review: `FailStyleAlignment/SourceExtractionR11/Fail_SourceExtraction_R11_Review.html`.
