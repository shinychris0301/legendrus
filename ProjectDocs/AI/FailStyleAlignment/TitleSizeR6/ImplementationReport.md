# Failure R6 — reference-sized title

Resized the title plaque from861px wide to a centered **691×240px** rectangle atx125,y527 on the941×1672 design canvas. This includes the crest and follows the reference's visually estimated bounds. [Unity R6/R5/reference viewer](Fail_TitleSize_R6_Review.html) · [Comparison and measurements](ReferenceComparison.md).

The existing panel mesh now supports separate protected top/bottom fractions. Only the title uses35%/7%, retaining ornament scale while the central face fits the requested height. Other panels keep their previous25%/25% settings. Main panel, text rectangles, colors, goals, boosters, offers and routes remain unchanged. Canonical prefab/owner/runtime revision6. No raster edits, generation, new assets or GUID changes.

## Performed

- Read project instructions/handoff/current task; inspected local changes, reference and active source. Verified Unity6000.5.9f1, all1,827 R5 production hashes and backed up sources/prefabs.
- Unity compilation/import and Failure-only prefab rebuild/readback at revision6 succeeded, exit0. **10/10 EditMode and18/18 PlayMode tests passed**, zero failures/skips. Existing layout/input/data/RU/EN/safe-area and Victory/Pre-Level checks retained.
- Visually inspected final941×1672 RU and three-goal576×1280 EN captures against reference. Saved9 Failure+12 Victory+20 Pre-Level PNG/JSON pairs and Unity logs/commands/XML; not every capture individually inspected.
- Exact pixel comparison of R5/R6 reference-size RU:143,493 changed pixels, **zero outside the union of old/new title regions** (x35..906,y520..790). Main-panel and all other visible regions are unchanged in this capture. See [PixelScope.json](QA/PixelScope.json).
- Source audit:1,821 unchanged baseline entries;6 existing production files changed, no new files. All944 metas/GUIDs,499 objectIDs and97 R5 evidence hashes preserved. Exact paths in [SourceAudit.json](QA/SourceAudit.json). C# whitespace and runner Python syntax checks passed.
- All11 viewer image choices and4 links resolve; final1,827 production and retained evidence hashes verified. CurrentTask/SessionLog updated.

## Not performed / limits

No browser render/dropdown interaction, APK/AAB/device test, full suite, manual loss/animation/prolonged-play/profiling, calibrated color/blur metrics, exact-font identification or live monetization test. Reference bounds are visual estimates rather than an exact mask of the painted plaque. This matches its approximate footprint; ornament/font contours remain different. Earlier documented lower chapter-map/border overlap remains unchanged. No other pages/settings/packages/signing/Android guards changed; no approval rejection or subagents.
