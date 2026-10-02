> **Reference discovery correction — 2026-09-23:** A dedicated Firebird Festival/Event Detail original has now been found in `match3final/ChatGPT Image Aug 25, 2026, 08_59_50 PM (7).png`. [View the original](Sources/OriginalEventDetailReference.png). The R1 implementation predates this discovery and adapts Events artwork; earlier statements that no dedicated original exists are superseded. No runtime change was made during this reference check.

# Visual Page 19 — Event Detail Reference Rebuild R1

Rebuilt the existing native Firebird Festival preview to share the accepted Events presentation. The user previously approved using Events artwork in the absence of a dedicated Event Detail original. This is a design adaptation, not a pixel-exact reproduction of an unseen reference.

## Implemented

- Actual `ProductionPlayerHeader` integration, including saved coins, gems, energy, player name/level, Profile, Settings and resource routes. Legacy serialized button/text bindings are preserved.
- Exact Home clean environment, white tint and centered-cover behavior, consistent with Events R4.
- Complete blank source title plaque with localized live “СОБЫТИЕ” / “EVENT”.
- Updated Events city scenery, source Firebird, separate halo/sparks and rounded stencil. The foreground layers share bounds; the landscape scenery independently covers the wider detail composition.
- Repaired source frame ornaments and individually sized original reward icons.
- Native text rendered at twice its displayed raster size; no new font installed or exact font claim.
- Original carved navy/gold footer and intact centre jewel. Header aligns to safe top; Back, footer note and ornament remain near safe bottom on taller displays.

Participation stays disabled. The date is unconfirmed, rewards are clearly labelled examples, no quantities are invented, and preview interactions cannot grant rewards or alter the old demo-claim flag.

## Asset workflow

**0 new Scenario requests / 0 new credits / 0 raster edits.** Reused the accepted source extractions and Scenario outputs already installed for Events R2–R4. No new generation was necessary. `Sources/AssetProvenance.json` records hashes and provenance for the reused materials. No existing artwork, metadata or other page prefab changed.

## Changed files

- `ProductionEventDetailRuntime.cs`: migration, shared composition, source layers and text density; fresh builds clone the current Shop shared header rather than searching for its removed legacy children.
- `ProductionEventDetailView.cs`: revision2, shared header presentation and live value refresh.
- `ProductionEventDetailLayout.cs`: safe-top composition and safe-bottom footer controls.
- `ProductionPlayerHeader.cs`: EventDetail attach overload; existing page attach methods unchanged.
- `ProductionEventDetailPrefabBuilder.cs`: revision2 cutover/readback.
- Active `Pages/EventDetail.prefab` and `CutoverState/EventDetailCutoverState.txt`.
- Existing EventDetail Edit/Play Mode checks extended for background/header parity, reward sprites, glow registration, rounded mask, safe top and footer.

**9 existing Assets files modified, no additions or deletions.** Runtime revision2, builder, active prefab and cutover agree. Hierarchy/binding preservation and repeated-build idempotence passed. Scope evidence: `QA/ScopeAndVerification.json`.

## Verification

Unity **6000.5.9f1**, isolated test save, final **Verify02**:

- Prefab rebuild, script compilation and serialized readback passed.
- **4 Edit Mode tests passed**, 0 failed/skipped: EventDetail and Events.
- **10 Play Mode tests passed**, 0 failed/skipped: EventDetail and Events.
- Verified Events→EventDetail→Back, double-click/rebinding guards, shared header routes, real/max balances, localization, text fit, native raycasts, unavailable participation, unchanged save/reward state and responsive safe areas.
- Six actual EventDetail captures plus eight Events regression captures. Final Russian941×1672 and English576×1280 inspected visually.
- Review browser: nine selectable views and previous-version toggle passed; no script errors or horizontal overflow.

Verify01 passed import/Edit Mode but failed two capture tests because the legacy `Rect.Contains` assertion excludes the maximum edge touched by the new shared header. Replaced it with explicit four-edge checks using the same half-pixel tolerance as Events. Final captures pass those checks. No layout error was hidden by removing a check.

No Android build, physical-device test, full project suite, new Scenario access check or performance profile was run.

## Next

Review the native page. Visual Page20 — Individual Ranking is next in sequence. Actual event objectives, scheduling, progress and reward validation remain feature-phase work; this visual rebuild does not implement them.
