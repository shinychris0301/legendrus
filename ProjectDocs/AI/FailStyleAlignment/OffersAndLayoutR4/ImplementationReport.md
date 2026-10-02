# Failure R4 — offer designs and panel alignment

Implemented the user's requested layout corrections and paid/ad offer **designs** in the active Failure prefab and owning builder/runtime, revision4. [Actual Unity portraits, R3 and original reference](Fail_OffersAndLayout_R4_Review.html) · [Issue-by-issue comparison](ReferenceComparison.md).

The paid +5 moves and rewarded continuation **behavior is still deferred until UI design completion**, as explicitly requested. [DevelopmentSchedule.md](../../DevelopmentSchedule.md) and [FAIL-CONT-01](../../DeferredFeatures/FAIL-CONT-01.md) now hand off these existing disabled controls for later integration. No purchase or ad can be initiated from this build. Current retry and chapter-map navigation remain functional; chapter-map return sits below the booster row.

## Design changes

- Goal icons reduced from100px maximum to88px and moved lower; live counts form a balanced group within the existing goal inset. No additional per-gem panel or screenshot region was introduced. Two/three-goal data still drives the layout.
- Booster heading moved below the top frame; the individual name labels sit lower within their plates. Medallions and artwork are slightly smaller and locally darkened; real stock, established names and artwork identities remain.
- New straight-sided title plaque retains only restrained top ornament/sapphire. The outer panel has a thin border, without the former large scrolls. Its top corners are covered by the wider title. Final panels use dark local tints.
- New dark burgundy paid card and sapphire ad card, each with an attached emerald button face. Live RU/EN titles, unavailable captions, lightning/currency/play symbols; paid price is a dash because no price is approved. Thirteen native buttons now exist; the two new offer buttons remain non-interactable and have no transaction listeners.

Four independently generated Scenario assets, one output each, medium quality, **48CU total**. Original output dimensions were1536×512 title,896×1024 outer panel and1024×448 offers. Alpha cleanup/transparent-padding trim produced1511×435,740×1024,1015×370 and1024×419 respectively. Source requests and returned image sizes differ for the title; recorded originals are authoritative. Original RGB within retained bounds is unchanged. Existing authorization covered local transparency cleanup; only alpha thresholds/padding were altered. Inspected all four sprites over black, white and magenta. Original full Failure image was style guidance only, never runtime art or a crop.

## Verification performed

- Read project instructions/handoff/current task/reference provenance and inspected local modifications, active code/prefab and current/reference images. Verified Unity6000.5.9f1 and all1,819 R3 production hashes before edits; backed up source/prefabs.
- Four Scenario submit/status/billing/download flows completed. Originals, requests, job IDs, billing and alpha diagnostics are retained. No replacement/duplicate generations, remote background removal or upscale.
- Two isolated Unity6000.5.9f1 runs: compilation/import, Failure-only prefab rebuild and revision4 readback all succeeded. Each passed **10/10 EditMode and18/18 PlayMode**, zero failures/skips. Final run followed the ad-label/outer-corner/tone corrections; no subsequent production edits.
- Existing Failure tests now check13 buttons, disabled offers and unchanged saved state when their empty callbacks are invoked; goal icon clearance below the inner border, count clearance, booster-heading clearance, nonoverlapping offers/retry/exit, readable text bounds and native hit targets. Existing goal snapshots, RU/EN/simulated safe areas, real stock, retry/exit/settings/booster-guide behavior and Victory/Pre-Level regression fixtures passed.
- Saved9 Failure,12 Victory and20 Pre-Level final PNG/JSON pairs. Visually inspected final RU941×1672 and three-goal EN576×1280 plus first-run RU. These are actual Unity captures with simulated displays/safe areas and isolated saves. Other captures were not all individually reviewed.
- Headless Chrome390×1100 default review rendered and was visually inspected; all11 image choices and4 links resolve. Browser dropdown interaction was not exercised. Three R4 Python helper/preparation/runner syntax checks passed.
- Preservation audit:1,813 baseline files unchanged; six existing production files changed, plus four new PNG/meta pairs. All940 old metas/GUIDs,439 old prefab objectIDs and102 R3 evidence hashes preserved. Final prefab has499 IDs. Exact paths in [SourceAudit.json](QA/SourceAudit.json). C# trailing-whitespace check passed; final1,827 production hashes and retained evidence hashes verified.

## Not performed / limits

No APK/AAB or physical Android test, full-project suite, manually played loss/animation inspection, extended-play/profiling, calibrated color/blur metrics or exact-font identification. Failure callback tests invoke the registered delegate rather than play a loss. No actual purchase, ad, board-resume or provider integration was implemented/tested; those remain the scheduled feature phase. Existing profile/resource routes were preserved, without separately adding manual route tests for each header control.

The result is not pixel-identical. Generated ornaments/materials, real game values/goals/booster names, unavailable offer states and retained chapter-map action differ. Availability captions and long names are small at narrow phone widths. User visual acceptance remains pending. Other pages, fonts, packages/settings/signing/Android guards and unrelated working changes were preserved. No automatic approval rejection and no subagents.
