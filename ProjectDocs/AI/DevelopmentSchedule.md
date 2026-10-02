# Development schedule

This milestone schedule records the user's 2026-09-15 sequencing decision. It is not a calendar reminder or a claim that pending features are implemented.

| Order | Stage | Status / exit condition |
|---|---|---|
| 1 | Finish and review the UI design pass | Active. Failure R12 delivered; Pause R1 delivered; Chapter Complete R7, Shop R1 and Special Offer R3 and Daily Reward R1 implemented and verified, awaiting visual review; retain page-by-page visual approvals. Complete remaining agreed page designs and responsive/localization review before continuation implementation. |
| 2 | Failure continuation: preserve and resume a round | Scheduled immediately after UI design completion, before monetization acceptance and release build. Implement retained/snapshotted board lifecycle and terminal-failure handling first. |
| 3 | Paid +5 moves and rewarded-ad continuation | Same post-UI milestone, dependent on stage 2. Integrate configured gem spending and existing verified rewarded provider; connect the current Failure R12 disabled offer-card designs to real availability states. |
| 4 | Continuation regression and Android acceptance | Verify transactions, cancellation/recovery, provider sandbox and actual device behavior before declaring the features complete. |

Detailed task, dependencies and acceptance checks: [FAIL-CONT-01](DeferredFeatures/FAIL-CONT-01.md). Owner: next gameplay/monetization development pass. No fixed date was supplied. Revisit this task when UI design is approved; do not silently omit it or replace it with a new-round retry.

## Additional post-UI feature

[CHAPTER-SMS-01](DeferredFeatures/CHAPTER-SMS-01.md): real phone-number verification via SMS associated with Chapter Complete, explicitly clarified by the user on 2026-09-16. Schedule after UI design completion; provider/backend and exact trigger/optional behavior to be defined before implementation. Separate from the still-pending physical-phone visual checks.

## Daily Reward follow-up

DailyReward R1 includes the explicitly approved seven-day schedule, saved claims and real monthly claim count. Monthly milestone payouts at7/14/21/28days remain unconfigured; define their reward policy and implement claim/grant behavior in a later feature pass. Reference previews must not imply an active payout. Scenario art R1 awaits visual review; remaining art differences are recorded in [ReferenceComparison](DailyRewardStyleAlignment/R1/ReferenceComparison.md).

## Login / Account follow-up

[AUTH-ACCOUNT-01](DeferredFeatures/AUTH-ACCOUNT-01.md): after UI completion, define and integrate real sign-in/providers, linking and progress reconciliation, recovery and published legal/consent destinations. Login R1 provides native presentation and local guest entry only. Coordinate with CHAPTER-SMS-01; no provider/compliance guard activation is included in the visual pass.

## Characters / Hero Collection follow-up

[HERO-COLLECTION-01](DeferredFeatures/HERO-COLLECTION-01.md): Page36 provides a native collection preview, preserving Ivan as current. Hero ownership, selection, combat stats, upgrades and team bonuses require a separate rules/service pass after UI review; no preview browsing grants or spends resources.
