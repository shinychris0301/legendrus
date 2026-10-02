# Returning-player story recap R2 — implemented and verified

User approval to continue selects the bounded implementation from R1. Profile History now opens an optional native reader for known acknowledged scenes, using the installed Dialogue art and Russian/English text. The latest eligible scene opens first; Earlier/Later browse, Continue advances, and Back/finish restore the same History page. Empty eligibility is truthful and disabled. Profile owner/view/active prefab/cutover agree on revision **13**.

**Latest distinct Unity coverage: 27 Edit Mode + 22 Play Mode = 49 passed, zero remaining failed/skipped.** This is reconciled coverage across the following isolated runs, including one improved native-entry case rerun; it is not one full-project sweep. Unity version is 6000.5.9f1 (b57deb96f08d).

| Run | Actual result | Coverage / process evidence |
|---|---|---|
| [EditVerified](QA/EditVerified/summary.json) | 27/27 passed | Six new projection cases, three existing Profile owner/binding cases and 18 existing CampaignStory adapter cases. XML completed in 1.83s; Unity test runner announced exit 0. Launcher terminated with 143 during shutdown; summary recovered against completed XML/log marker, invocation duration unavailable. |
| [NativeFinal](QA/NativeFinal/summary.json) | 21/21 passed | Five recap cases, all 12 Profile cases and four adjacent Home/map/Dialogue story-gate cases. Unity exit 0; invocation 118.05s. |
| [QARecap](QA/QARecap/summary.json) | 1/1 passed | Disposable fresh QA reader leaves synthetic normal and QA journals unchanged. Unity exit 0; invocation 26.82s. |
| [NativeEntryVerified](QA/NativeEntryVerified/summary.json) | 1/1 passed, already counted above | Improved latest/browse/return case with real EventSystem pointer entry from Profile and native Earlier. Unity exit 0; invocation 37.46s. Replaces that case's earlier observation in the distinct inventory. |

Projection checks cover all 58 active scenes, existing RU/EN speaker/text localization, narrative ordering, duplicates, sparse/unknown/archived/scoped IDs, unread/future entries, chapter-specific completed-round eligibility and catalogue/save preservation. Existing CampaignStory cases use controlled adapter stores/content; they do not establish Android activation.

Native checks cover a disabled empty action, latest eligible opening, real pointer entry/Earlier/Continue/Back, locale refresh, repeated same-frame Continue, underlying modal blocking, History page/object restoration, unchanged pending normal story position, full save/journal preservation, repeated round-500 finale completion without ChapterComplete acknowledgement, departure and stale callbacks. Existing normal Dialogue cases cover all **57 installed milestones**, their RU/EN lines and established return routes. The recap reader uses synthetic ledgers; no normal player's saved journal was opened. Existing Profile cases include safe-area/input/statistics/page coverage and native owner idempotency.

## Reconciled initial outcomes

- [EditInitial](QA/EditInitial/summary.json): exit 1, compilation errors in the new test fixture (property/method mismatch and copied helper references), no test XML. Fixture corrected. [EditFinal](QA/EditFinal/summary.json) then passed 9/9, exit 0; final 27-case run above adds existing story adapter coverage.
- [NativeInitial](QA/NativeInitial/summary.json): 19 passed, two failed, no skips. New Profile entry began at y672 and overlapped an existing tab's expanded touch area through y673; moved to y679/h61. Exact save comparison also detected ordinary full-cap energy timestamp refresh; fixture now uses energy4 below cap with a current timestamp, retaining exact whole-save comparisons and unchanged energy policy. NativeFinal passes both cases.
- [NativeEntryFinal](QA/NativeEntryFinal/summary.json) and [NativeEntryProbe](QA/NativeEntryProbe/summary.json): improved pointer fixture each failed once, hitting LetterboxBackground before newly rebuilt graphics acquired render depths. The capture helper now renders before raycasting; no product input guard was relaxed. NativeEntryVerified passes real entry and preserves save/journal/story/origin assertions.
- EditVerified launcher termination is preserved explicitly in its recovered summary; completed passing XML and the Unity test-runner success marker are separate evidence from process exit status. No clean launcher exit is claimed for that invocation.

## Scope and visual evidence

[Scope](QA/Scope.json): of 7,825 task-start project inputs, **7,819 unchanged, six changed, eight added, zero deleted**. Six existing changes are Profile view/runtime/router/builder/prefab/cutover. Four new C# sources plus four metadata files provide model/reader/tests. The active Profile prefab changes only Revision12→13; serialized IDs/references preserved. Dialogue and all other original prefabs, approved Splash, artwork/fonts/metadata, packages/settings/backend and Android guards unchanged. Normal StoryService, GameState and campaign progression services remain byte-identical.

**77 final actual Unity PNGs** across NativeFinal and NativeEntryVerified; 60 initial captures retained separately. NativeFinal's `Recap_Latest_RU` was rendered after Earlier, so it shows level1/index2; the standalone review uses the corrected NativeEntryVerified level2/index3 capture. All are Editor renders of the actual active UI at simulated portrait viewports/safe areas, not physical-phone captures or generated previews. Six exact portraits inspected; see [visual checks](VisualSpotChecks.md) and [one-portrait review](StoryRecap_R2_Review.html). Existing secondary typography/physical readability and exact source-font provenance remain open; no new user visual acceptance inferred.

Current R6 APK/sidecar remain **765,764,694 bytes / SHA256 `42b29fca2c6941ed2ac96e5c7dd00f9e6c79aab70dfd5b0892a64d7faa815ce5`** and do not contain R2 recap. Five normal manual recorder exports remain unchanged. Tests use isolated product directories under `/tmp/lor-returning-player-recap-r2/`; only the entitlement location is linked, credentials never copied/printed. No task-new test scenes remain. No APK build, Android/device lifecycle test, install, ADB, hosting, deployment or commit ran.

[Implementation](ImplementationNotes.md) · [Case inventory](QA/CaseInventory.json) · [Capture inventory](QA/CaptureInventory.json) · [Narrow patch](Sources/RuntimeChanges.patch) · [Final validation](QA/FinalValidation.json).

Task-added code/document lines pass whitespace checks; the four new Unity metadata files retain their standard empty serialized fields. Both narrow patches pass read-only reverse-application checks. Whole-HEAD `git diff --check` reports inherited Unity YAML trailing whitespace already present in the task-start snapshot; it was preserved, not reformatted. [Whitespace evidence](QA/WhitespaceChecks.json) records this separately.

**Next:** Package the verified native recap/Profile13 through the existing guarded development APK path, first preserving the R6 APK/sidecar. Rerun current Android policy/content checks and review identity/signing/permissions, ZIP/ELF/JNI and expected packed membership changes. Phone execution and hosting remain deferred. Human201–210 recording remains parallel, following the existing gap matrix before tuning; final gameplay effects stay last.
