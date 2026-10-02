# Failure R5 — restore the main panel

R4 changed too much: the user's correction is to preserve the large main Game Over panel and remove only the decorations attached to the title's sides. Restored the original `panel_r2` artwork, white tint and891px width. Kept the R4 title without projecting side scrolls and the corrected goals/boosters/disabled offers. Current1005px outer-panel height remains to fit the offer/return layout (20px more than before offers).

[Actual Unity R5, R4 and reference](Fail_MainPanelRestore_R5_Review.html) · [Reference comparison](ReferenceComparison.md).

The canonical Failure prefab, owning builder and runtime are revision5. Existing EditMode assertions distinguish the restored main sprite from the separate clean-sided title sprite. No new generation, raster modification, asset or GUID change. FAIL-CONT-01's design handoff was corrected to preserve the restored main panel during future continuation implementation. Payment/ad behavior remains deferred.

## Performed

- Read instructions/handoff/current task, inspected local changes and recorded Unity6000.5.9f1. Verified all1,827 R4 production hashes before edits; backed up source/prefabs.
- Compared original full reference and final actual Unity RU941×1672 and three-goal EN576×1280 captures.
- Unity compile/import and Failure-only prefab rebuild/readback at revision5 succeeded, exit0. **10/10 EditMode and18/18 PlayMode** tests passed, zero failures/skips. Existing native layout/input/state/disabled-offer, RU/EN/safe-area and Victory/Pre-Level regression checks retained.
- Retained9 Failure+12 Victory+20 Pre-Level PNG/JSON pairs, logs/commands/XML. Other captures were generated but not all individually reviewed. Saves were isolated.
- Source audit:1,822 unchanged baseline files,5 existing production files changed, no new production files. All944 metas/GUIDs,499 prefab objectIDs and126 R4 evidence hashes preserved. Exact paths in [QA/SourceAudit.json](QA/SourceAudit.json). Changed C# whitespace check passed.
- Review's11 image choices and4 links resolve. Runner Python syntax, final1,827 production hashes and retained evidence hashes verified. CurrentTask/SessionLog updated.

## Not performed / limits

No new browser rendering/dropdown interaction, APK/AAB/device test, full suite, manual loss/animation inspection, prolonged play/profiling, calibrated color/blur metrics, exact-font identification or live monetization tests. Other pages/settings/packages/signing/Android guards and unrelated local work preserved. No approval rejection or subagents.

The outer-panel artwork is restored, but the page is not pixel-identical to the reference. Generated detail, actual game data, unavailable offers and the retained chapter-map action remain different. That return button overlaps the lower center border in the current offer layout; no further layout redesign was made in this narrowly scoped correction. Visual acceptance remains with the user.
