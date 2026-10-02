# Detailed visual recheck — 2026-09-23

**Verdict: recognizable and stylistically consistent, but not yet a faithful overall match.** The restored main title is the closest element. No percentage score is assigned: differing live values, language/content states and generated scenery make a whole-screen pixel score misleading.

Compared the original and the final Verify04 **941×1672 Russian Unity capture at the same dimensions**, then checked current layout code and the active prefab. The prefab predates this capture. This is a review of existing runtime evidence, not a newly captured session.

| Priority / area | Visible difference | Recommended correction |
| --- | --- | --- |
| High — card frames and image joins | The implementation has thinner, straighter-looking rails and angular corner joins. Rectangular dark notches are visible near the upper-left artwork corners. Scene-to-panel transitions are harder than in the reference, and the visible gaps between frames look larger. | Fit the painted window to the actual inner frame silhouette and repair corner joins. Inspect source-frame alpha and slicing together; avoid simply enlarging the whole card. |
| High — reward scale | Especially in the Firebird card, the red gem/chest/medallion strip is smaller and less prominent than the original. More unused space appears above it. | Size icons individually from their visible bounds and align the reward row with the action button, retaining the “Reward examples” label. |
| High — scenery and Firebird | The header city is warmer, buildings are in different places, and the gaps/footer expose leafy ground rather than the reference's darker ornamental surround. The Firebird is smaller in its composition and lacks much of the atmospheric glow and sparks. | Preserve original extracted foregrounds; match scene framing and add a separate glow layer. Use Scenario only for scenery or light not recoverable from the source. |
| Medium — tabs and typography | The selected tab has a flat, inset blue face instead of the textured sapphire surface. Live card names/action text have different weight and embossing; the shared player-name/level lettering is heavier than in this reference. | Restore a textured tab face and adjust local caption size/placement. Keep shared header decisions consistent across pages; the exact source typeface remains unverified. |
| Medium — footer | The original has a continuous carved/gold lower surround. The implementation has a smaller separate ornament, a Back control and an availability note over visible ground. | Rebuild the decorative surround around the required native Back control and honest availability note. |

## Elements that now match well

The Russian main title's letterforms, gold shading and overall silhouette come from the original. The full crest is present. A fresh read-only pixel check confirms **62,857 visible source-RGB pixels with zero RGB mismatches** in the title sprite; this is not a claim that its alpha or the entire page is pixel-identical. The overall four-event order and two-column card structure are close. Source characters and reward subjects are recognizable and retained.

The shared Home header is correctly used and includes live energy. Different saved balances, level and XP presentation are not extraction defects.

## Intentional content differences

Upcoming selection, unconfirmed dates, example-reward labels, Preview/Coming Soon actions, and omission of the active alert/premium claim reflect the current unimplemented event service. Do not copy illustrative countdowns or participation states to make the screenshot appear exact.

## Recommended next pass

**Events R2 — Card Frames and Reward Scale.** Repair corner/artwork joins and visible frame spacing first, then restore reward-icon prominence. Keep the original main title and shared header. Follow with scenery/Firebird glow and tab/footer ornament refinement.

This audit changed documentation only. No runtime/prefab/assets were edited, no new Scenario job was requested, and no Unity tests were rerun. The prior 17 passing checks establish functional behavior, not reference fidelity. [Read-only verification](FidelityRecheck.json).

---

# Events reference fidelity — Reference Rebuild R1

Compared the original `DesignScreens/Events.png`, the historical SourceExtractionR3 Unity capture and the current native page.

## Corrected

- Events now uses `ProductionPlayerHeader`, the same presentation/artwork and live balance handling as Home. Coins, gems, energy, profile and settings retain their native routes.
- Original Russian **СОБЫТИЯ** lettering and the complete upper crest are restored. The old 533×182 crop cut through the crest; the new 549×207 extraction includes its full silhouette. Scenario supplies alpha; visible RGB comes directly from the original.
- The English title stays live on a blank plaque with the same complete silhouette. Localized card captions use double raster density. Original font identity remains unverified.
- The environment and title scenery have a cooler, darker tint to reduce competing warm scenery between cards.
- The card stack starts beneath the shared header; Back/footer follow the safe bottom edge on taller screens. Original sprite proportions are preserved.

## Deliberate content differences

Upcoming is selected because no authoritative event schedule is connected. Dates remain “to be announced”; reward rows say “Reward examples”. Only Firebird opens the existing preview; other actions are unavailable. The source's active-event alert, premium ribbon, countdowns and participation claims are not presented as working features. Header values come from the save.

## Remaining visual differences

This is **not a pixel-identical reproduction**. Previously generated clean scenery differs from the original architecture, lighting and foliage. The isolated Firebird lacks some original atmospheric glow. Card corner ornaments reuse an existing source frame rather than four independently extracted silhouettes. The selected tab uses a native blue face, and live labels do not reproduce original embossed letterforms. The shared Home portrait/header intentionally follows the approved common component rather than a page-specific redraw. The bottom Back control and availability note differ from the illustrative reference footer.

No full-screen reference or screenshot hotspot is used in the native page. The source image appears only in the review's comparison option.
