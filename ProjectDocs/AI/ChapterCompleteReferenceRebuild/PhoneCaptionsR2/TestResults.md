# Chapter Complete phone captions R2 — 2026-09-30

**Implemented and verified: 18 targeted Unity tests passed, 0 failed, 0 skipped.** [Portrait review](ChapterComplete_PhoneCaptions_R2_Review.html).

## Installed change

The canonical Chapter Complete prefab and its owning builder/runtime now share revision 9. Reward counts increased from 30 to 40 reference units; the summary from 25 to 40 and the next-chapter caption to 38. The summary uses two lines. Russian teaser lettering now uses the existing blank parchment sprite plus live localized text, retaining the source wording; English uses the same layout. The description is a larger live caption below the teaser, inset above the bottom ornament. Existing font assignments are retained, with no claim of exact source-font identity.

The existing hero art is scaled proportionally to make room. The existing footer uses an 80-unit nine-slice border to preserve corners. Reward icons and values have separate bounds, with an explicit lower decorative margin. No new image assets were created, and no source image pixels changed. Gameplay, rewards and chapter unlock rules are unchanged; Chapter 2 remains unavailable.

## Executed verification

| Check | Result |
|---|---|
| Specific owning-builder rebuild, Unity 6000.5.9f1 | Build3 exit 0; active prefab rebuilt |
| Final Play Mode: ChapterCompleteAlignment + ChapterFinaleCompletion | 16 passed, 0 failed/skipped |
| Final Edit Mode: ChapterCompleteAlignment | 2 passed, 0 failed/skipped |
| Source runner syntax | Passed |
| Baseline hash scope | 8 existing files changed; 7,777 unchanged; 0 added/deleted |

Evidence: [runtime summary](QA/FinalPlayMode/summary.json), [serialized-prefab summary](QA/FinalEditMode2/summary.json), [build result](QA/Build3/result.json), [file scope](QA/Scope.json).

The new active-catalog case renders all 500 completed rounds in Russian and English at 360×640, 576×1280 and 1080×2400 with simulated safe areas. It verifies full totals (500/500 and 1500/1500), text height, at least 13.5 effective render pixels for the targeted captions, safe-area containment, separated text/art bounds, footer/card decoration margins, button raycasts and unchanged saved state. Existing cases cover incomplete-state presentation, repeat acknowledgment without duplicate grants and the current finale navigation. These are geometry and behavior checks, not proof of readability for every device or user.

Manually inspected final actual captures: Russian and English at 360×640, English at 576×1280 and Russian at 1080×2400. The description clears the bottom ornament and counts clear the lower card frames. Captures are under `QA/FinalPlayMode/Captures/LOR_CHAPTER_COMPLETE_CAPTURE_DIR/`; the review displays one portrait at a time.

## Iterations and limits

Initial build failed because TextureImporter does not expose the attempted spriteMeshType property; removed that assignment, retaining the existing FullRect configuration through TextureImporterSettings. Build2 succeeded. Initial 16 runtime tests passed, but visual inspection found caption/frame contact; spacing was corrected and margin assertions added before the final passing run. One final Edit Mode command used the wrong namespace and selected zero cases; it is not counted. Corrected FinalEditMode2 selected and passed both cases. The verification runner now rejects zero-case runs.

Tests use isolated product storage and the previously authorized process-only Vulkan option. No project graphics/backend/package settings changed. No full-project sweep, Android build, installation, actual phone test or human difficulty study ran. Phone work remains postponed; hosting/online activation and animation/effect redesign remain deferred.

## Changed files

- ProductionChapterCompleteView.cs and ProductionChapterCompleteRuntime.cs: localized captions and layout.
- LockedDesignImporter.cs and Step20ProductionUiFix01SerializedPrefabBuilder.cs: footer slicing and canonical rebuild validation.
- ProductionUI/Pages/ChapterComplete.prefab and ChapterCompleteAligned/footer.png.meta: installed layout and sprite borders; existing GUID retained.
- ChapterCompleteAlignmentPlayModeTests.cs and ChapterCompleteAlignmentEditModeTests.cs: current revision and readability/frame checks.

`Sources/Changes.patch` compares the seven snapshotted files against the task baseline; footer metadata is covered by the baseline hash inventory and current source, not a reconstructed before-file. Existing unrelated working-tree edits are preserved.

**Next:** audit Fail and Out of Energy captions and controls at small portrait sizes in both languages, fixing demonstrated layout/readability issues. Resume physical-phone acceptance separately when requested. Human rounds 31–500 difficulty/pacing, optional catch-up recap, device lifecycle/performance and a single clean post-correction broad runtime sweep remain open.
