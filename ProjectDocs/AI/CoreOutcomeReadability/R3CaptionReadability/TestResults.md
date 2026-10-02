# Core caption readability R3 — 2026-10-01

Implemented the approved next scheduled Gameplay/Victory caption pass in the actual active prefabs and their existing owners. **Final 19 Edit Mode + 25 Play Mode cases passed, zero failed/skipped (44 distinct).** Six new permanent cases verify repeated construction, bilingual glyph containment, native input and retained save/reward behavior. Unity 6000.5.9f1 was used; these are Editor results, not physical-phone acceptance.

[Standalone portrait review](CoreCaptions_R3_Review.html) displays one actual Unity capture at a time. The 48 measured configurations cover RU/EN at 360×640, 576×1280, 941×1672 and 1080×2400: eight Gameplay presentations and 40 Victory first-clear/no-boosters/replay/unavailable/no-result presentations. The final run retains 105 PNGs including existing regression captures. Inspected the final small Russian Gameplay, Russian first-clear Victory and English no-boosters Victory portraits directly.

## Installed behavior

Gameplay names now occupy native navy lacquer faces above the existing four wooden booster cards. The existing ProductionHomePanel draws each opaque face and its gold frame; no raster asset was generated or cropped. The four existing Text objects, wording and font assignments are retained. The faces forward pointer input through the existing card handlers to the existing Buttons. Board, card, icon, stock badge and Pause geometry are unchanged. The caption fit component records its actual mesh scale so measurements account for optical fitting. Gameplay revision is **23**.

Victory's three headings are larger live text in both languages. The existing blank panel.png replaces the Russian panel variant with baked headings, avoiding duplicate wording. Both source PNGs remain byte-identical. Currency/booster counts and the no-booster caption are enlarged. Refresh reapplies the same caption configuration as construction. Main score, artwork, Continue/double controls, offer availability and reward/save services retain their behavior. Victory revision is **12**.

| At 360×640 | Previous nominal font pixels | R3 nominal font pixels |
| --- | ---: | ---: |
| Gameplay booster names | 5.74 | 8.25 |
| Victory score/rewards/boosters headings, English | 9 / 9.33 / 8.67 | 12 / 12 / 12 |
| Victory currency counts | 11.67 | 14 |
| Victory booster counts | 9.67 | 12.67 |
| Victory no-booster caption | 7.67 | 11.33 |

Gameplay effective font pixels after mesh fitting are **8.15–8.25** at 360×640; across all displays its mesh scale is 0.9874–1.0. Font pixels describe the rendered font scale, not cap height or Android dp. [Measurements](QA/CaptionMeasurements.json) record glyph bounds separately. Previous Russian baked heading typography has no equivalent live-Text measurement; the English baseline numbers must not be attributed to its source font.

## Verification and scope

[EditAccepted](QA/EditAccepted/summary.json): exit 0, 19/19 passed, 25.52s invocation. Includes fresh and installed Gameplay/Victory, repeated owners preserving component/control/font identity, prior Gameplay/Victory alignment and six prior touch cases including ChapterComplete. [PlayAccepted](QA/PlayAccepted/summary.json): exit 0, 25/25 passed, 171.88s invocation. Includes all existing Gameplay/Victory tests plus two new matrix tests. [Coverage](QA/Coverage.json) gives fixture counts; XML results accompany both summaries.

Native checks assert complete visible characters, line counts, glyph containment, effective scale, safe caption placement and native raycast forwarding. Tapping the Gameplay caption arms the existing LivingLight action, then cancellation preserves the board and full save except the existing energy clock refresh. Victory checks round 500 first-clear with maximum Int32 score and booster quantities, round 1 no-booster rewards, replay/unavailable/no-result semantics and retained rewards/save. Gameplay's normal capture uses the fixture's retained round-1 board; this task makes no round-selection repair claim.

Iterations are retained in QA rather than counted as extra passes. Initial new tests selected an incorrect nested Gameplay component and held a stale Victory view after navigation; corrected the fixtures. Actual Gameplay mesh shrink exposed insufficient caption width/height; final text rectangle and a two-unit card gap resolved fitting and floating boundary failures. Two existing Edit assertions still expected revision 11 and the baked Russian panel; updated them to the current revision and blank panel while retaining their other checks. Final complete Edit/Play runs pass after all production changes.

[Scope](QA/Scope.json): 7,788 of 7,798 baseline project inputs unchanged, **10 changed / 4 added / 0 deleted**. Changed files are five runtime owners/components, Gameplay/Victory prefabs, two existing Play fixtures and the existing Victory Edit fixture. Added files are two permanent test sources and their metas. Exact paths and reviewable diff: [TaskChanges.patch](QA/TaskChanges.patch).

[Prefab preservation](QA/PrefabPreservation.json) verifies every existing serialized object ID, font and native targetGraphic reference. Gameplay adds 16 serialized objects for four native caption faces (413→429); Victory retains all 370 objects. Only four Gameplay and ten Victory text rectangles change position/size; all other existing transform geometry is retained. Victory changes exactly one sprite reference to the existing blank panel. All 70 other prefabs, original metadata, source artwork/fonts, approved Splash, Packages, ProjectSettings and Android guards remain byte-identical to task start. Temporary install helper/meta are removed. [Final evidence validation](QA/FinalValidation.json) checks scope, links, captures and current APK identity.

## Limits and next action

Small captions have improved, but this is not a universal physical-phone minimum. Large four-digit Gameplay stock badges remain small; other secondary HUD labels are outside R3. Font provenance remains unresolved and existing font assignments were preserved; no OS/container fonts were copied. Physical-phone readability, human Chapter 1 pacing and Android lifecycle/performance remain open. No new APK, device test, hosted session, deployment or commit was performed.

The existing R2 APK remains **765,773,174 bytes**, SHA256 `c065e5ab23f9404cb960660e4fd460ae57f15c4310a181022a65da094890fdb5`; sidecar matches. [Existing APK evidence](QA/ExistingApk.json). It includes R1 touch/numeric fixes and **does not include R3 captions**. R2's 130 policy checks are historical, not new R3 checks.

Next: preserve the verified R2 APK, refresh through the guarded development build entry with Gameplay/Victory revisions 23/12, rerun current policy/content guards, and compare signing, identity, permissions, ZIP/ELF alignment, credential JNI and packed membership. Preserve source artwork/imports and prior approvals. Phone execution/hosting remain deferred; final gameplay effects stay last.
