# Returning-player flow review R1 — 2026-10-02

The scheduled review is complete. **Saved level selection and pending-story recovery already exist; optional rereading of acknowledged story scenes is missing.** Recommend a bounded read-only story recap from existing Profile History, using the installed dialogue art and text. [Next implementation scope](ImplementationScope.md). No recap UI or reader was implemented during this review, and usefulness is an inference from the access gap, not measured returning-player feedback.

## Installed behavior

| Returning-player need | Finding | Evidence / implication |
|---|---|---|
| Find the saved round | Home Play uses the existing chapter gate. The map's Current control centers `selectedLevel`; header level represents the unlocked frontier. | Saved replay selection may differ from the frontier intentionally. Preserve it; do not add an automatic overwrite. Home/map selection and gates pass in the targeted native run. |
| Continue unread eligible story | Normal story service finds unacknowledged milestones whose rounds are completed; Home/map gates offer them before further progression. | Existing intro, pending milestone and final chapter presentation must remain the progression path. Native tests cover all 57 installed milestones and the current finale. |
| Recover interrupted story | Normal active sequence is transient; resetting it allows an eligible unread scene to be offered again. The campaign adapter derives pending scenes from its durable ledger. | Three focused Play Mode reset/suppression/conflict checks use the explicit legacy fixture; 18 Edit Mode adapter checks cover acknowledgement, write-fault recovery and scoped legacy/expanded content. These are not physical process-restart evidence. |
| Reread an acknowledged scene | Normal `TryBeginScene` returns false for completed scenes. Profile History builds only level stars/score/attempt statistics. No recap entry or route is installed. | A separate read-only sequence is needed; loosening the normal completion guard would change progression behavior. Source review plus native history/gate tests support this access-gap finding. |
| Reuse story wording and art | Active Chapter 1 supplies 58 scenes / 70 lines, with complete speaker/text keys in RU and EN. Existing Dialogue has explicit providers. | No generated recap prose or replacement artwork is needed. Its normal completion callback still writes state and must not be used for recap. |
| Trigger a welcome after absence | No dedicated narrative acknowledgement timestamp or absence policy was found in the reviewed save/story/UI owners. | An energy timestamp is not a narrative inactivity metric. Start with an optional action; no forced welcome or timestamp/save migration proposed. |

Source anchors and actual hashes: [reviewed owners](QA/SourceAnchors.json). The active loader reads CampaignActive resources. Current native save scene IDs and separate development campaign scoped keys have different contracts; the proposed first delivery keeps those boundaries intact.

## Content and current Unity checks

Read-only [installed inventory](QA/InstalledStoryInventory.json) validates 500 round IDs, 57 milestone/trigger associations and 58 unique scenes in narrative order. RU/EN each contain 1,569 unique localization entries; every scene's speaker/body key resolves to nonempty text. Scene lengths: 50 one-line, six two-line, one three-line intro and one five-line finale. No scene-summary field is installed. Finale: `campaign.chapter01.village_departure`, after round 500; exactly one normal scene routes to ChapterComplete. The archived eight-scene ten-round resource remains a historical/migration fixture, not current recap content.

**18 Edit Mode + 15 Play Mode = 33 current Unity checks passed**, zero failed/skipped/inconclusive. Unity 6000.5.9f1 exited 0 in both processes: 26.61-second Edit invocation and 77.10-second Play invocation. [Edit summary](QA/StoryPersistence/summary.json), [cases](QA/StoryPersistence/results.xml), [Play summary](QA/NativeFlow/summary.json), [cases](QA/NativeFlow/results.xml).

Play coverage comprises seven current native Dialogue cases, two map cases, one Home gate case, two Profile History cases and three explicit legacy story-service fixture cases. The all-milestone native case renders existing RU/EN lines and routes through all 57 milestones to the round-500 finale. Profile History tests verify bounded paging and unchanged save state; they do not implement narrative browsing. Adapter tests use controlled in-memory stores/content fixtures. No new tests or product sources were authored for this review.

The existing tests produced **28 actual Unity PNGs**. Inspected [Profile History at 576×1280 EN](QA/NativeFlow/Captures/LOR_PROFILE_CAPTURE_DIR/Profile_HistoryPage2_safe_EN.png) and [current finale at 576×1280 EN](QA/NativeFlow/Captures/LOR_DIALOGUE_CAPTURE_DIR/Dialogue_Finale_576x1280_safe_EN.png): the former displays saved level rows; the latter uses the existing live text and native Continue. These are captures of existing pages, not a recap implementation, new design approval or phone acceptance. Small secondary statistics remain outside universal device readability acceptance.

## Preservation and delivery

[Scope](QA/Scope.json) and [final validation](QA/FinalValidation.json) verify all **7,825 project inputs unchanged**, five unchanged normal manual recordings, and the unchanged R6 APK/sidecar (765,764,694 bytes, SHA256 `42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5`). Existing artwork/fonts/prefabs/GUIDs/metas, accepted Splash, settings/packages/backend/Android guards and prior user edits were preserved. Test product storage was isolated under `/tmp/lor-returning-player-r1/`; no personal save journal was opened. Existing machine entitlement was used without reading/copying licensing contents. Graphics override applied to the test process only.

Authored files: this report, implementation scope, inspection/runner/final-validation sources, task evidence and four continuity-document updates. Existing sources/tests/prefabs untouched. Raw Unity/launcher logs stay in the task's `/tmp` folders; generated caches are excluded from the authored delivery. No temporary helper was installed; both test processes' generated-scene cleanup lists are empty. No new APK, device/ADB/install/hosting/deployment/commit work or human play results.

## Next action

Implement the optional read-only story recap within [the bounded scope](ImplementationScope.md), then verify native input, state preservation and one-portrait presentation before packaging it. Human 201–210 recording remains the parallel Chapter 1 acceptance action, followed by the existing gap queue before tuning. Phone execution/hosting remain deferred; large-stock fixture/readability, exact font provenance and physical lifecycle/performance remain open; final gameplay effects stay last.
