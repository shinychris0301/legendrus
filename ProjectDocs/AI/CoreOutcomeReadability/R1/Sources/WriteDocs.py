"""Publish local evidence and update the existing development handoff without erasing history."""
from pathlib import Path
import json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';DOCS=ROOT/'ProjectDocs/AI'
def read(p):return json.loads((QA/p).read_text())
assert read('EditVerified/summary.json')['result']['passed']=='17' and read('PlayFinal/summary.json')['result']['passed']=='43'
scope=read('Scope.json');compare=read('CaptureComparison.json');pixels=read('MapPixelComparison.json')['rows'];apk=read('ExistingApk.json');measurements=read('Measurements.json');assert len(measurements)==56
next_action='Refresh the guarded development APK with the installed Gameplay/Victory/ChapterComplete touch and numeric changes. Preserve the verified R17 artifact first; rerun Android policy/content guards and compare signing, identity, permissions, ZIP/ELF alignment, credential JNI definitions and packed membership. Source artwork/imports must remain unchanged. Phone execution and hosting remain deferred. Small secondary captions, human Chapter 1 pacing, font provenance and Android lifecycle/performance remain open; final gameplay effects stay last.'
report='''# Core outcome readability and input R1 — 2026-10-01

Implemented the approved next scheduled audit of Gameplay, Victory and ChapterComplete in Russian and English. Added safe-area-constrained native touch padding to undersized controls and kept Gameplay stock quantities on one line. **Final: 17 Edit Mode + 43 Play Mode checks passed, zero failed/skipped (60 distinct).** [One-portrait Unity review](CoreOutcome_R1_Review.html).

## Demonstrated defects and installed correction

At the simulated 360×640 viewport with safe area (0,20,360,600), Gameplay Pause was 37.32×36.60 rendered pixels, Victory Continue was 209.67×40.33 and ChapterComplete actions were approximately 140/137×33.73. Their artwork and button identities are retained; a transparent child Graphic expands input to at least 44 rendered pixels on each undersized axis, constrained by the existing safe-area bounds. Pointer events reach the existing Button; its targetGraphic, listeners, route guards and action ownership remain unchanged. This is a rendered-pixel criterion in the sampled Editor viewports, not a physical-device dp or touch-accuracy acceptance.

| Control | Previous height at 360×640 | Verified input height |
| --- | ---: | ---: |
| Gameplay Pause | 36.60 px | 44.00 px; width also 44.00 px |
| Victory Continue | 40.33 px | 44.00 px |
| ChapterComplete Continue and Map | 33.73 px | 44.00 px |

Victory's video control already exceeds 44 pixels in the small view; it uses the same owner-bound target without growing its art. No Button, Canvas, EventSystem, timer, duplicate action or image asset was introduced. The shared touch Graphic emits a transparent quad for native raycast ordering. All five targets stay within the measured safe areas.

Gameplay's synthetic `9 999` stock quantity wrapped into two lines in the circular badge. The owning source controls now use one line, allow a lower best-fit bound and apply the existing numeric mesh containment effect. All five characters remain visible inside the existing label. The actual inventory and formatting service are unchanged. Large values still become small inside the fixed badge: this corrects numeric wrapping and containment, not universal large-stock readability.

Owning APIs install the same behavior in fresh construction and repeated current construction. The three real active prefabs were updated through the precise new owner helpers in an idle Editor; no full page/art/import rebuild. Gameplay revision22, Victory11 and ChapterComplete9 remain in agreement with their owners. All existing serialized object IDs, sprite/font/target references and transform geometry are preserved; each prefab adds eight serialized objects (Gameplay touch child plus four numeric effects, or two touch children for each outcome page). Other 69 prefabs are byte-identical. [Identity proof](QA/PrefabIdentities.json) · [Art/binding/geometry proof](QA/ArtworkAndBindingPreservation.json).

## Executed Unity checks

Unity 6000.5.9f1 (b57deb96f08d); isolated product storage/configuration and process-only Vulkan. Existing bootstrap/router/services used. Unity launches used reviewed permissions for local Package Manager IPC.

- Baseline: seven existing native capture/layout checks passed; all 7,792 task-start project inputs verified unchanged before adding the disposable probe.
- Diagnostic matrix: three temporary cases measured 56 unique configurations across both languages and 360×640, 576×1280, 941×1672 and 1080×2400. Gameplay normal/large values/targeting; Victory round500 first-clear with Int32.MaxValue score, replay and unavailable feedback; ChapterComplete actual installed 500/500 rounds and 1500/1500 stars. Captures and measurement rows retained. These observations are not blanket readability acceptance.
- Final Edit Mode: 17 passed. Six new fresh/installed owner cases verify retained object/component/control/target/art identity and no duplicate touch children on repeated calls. Existing Gameplay6, Victory3 and ChapterComplete2 serialized checks pass.
- Final Play Mode: 43 passed. Gameplay11, Victory12, ChapterComplete5, finale12 and matrix3. Three new regressions raycast in the additional small-view padding and execute the existing native pointer path: Pause preserves board/save; Victory reaches the existing story/map route without duplicate grants; ChapterComplete acknowledges once and navigates to WorldMap without duplicate rewards. Large score stays exact, stock glyphs stay on one line/in their label, localization and small/tall safe areas pass. Existing board/booster/pause, score/replay/video-unavailability, chapter states and finale behavior remain covered.

Final counts include the three diagnostic cases, whose temporary source/meta were removed after evidence collection. Baseline and intermediate repeats are not added to the 60 distinct final checks. The finale suite uses its established legacy test catalog; the matrix and current ChapterComplete cases separately cover the installed 500-round chapter. This is targeted coverage, not a new full R5 gameplay sweep or human campaign pacing study.

[Final Edit summary](QA/EditVerified/summary.json) · [Final Play summary](QA/PlayFinal/summary.json) · [Coverage](QA/Coverage.json) · [56 observations](QA/Measurements.json) · [Touch dimensions](QA/TouchMeasurements.json).

## Iterations and visual evidence

Initial Edit run passed15/17; two new fresh-owner assertions incorrectly required component traversal order to remain fixed. Existing owners reorder retained siblings. Corrected only that assertion to compare component identities as a set; control/target/art checks remain exact. Complete17/17 rerun passed; no production correction was needed. The initial diagnostic fixture accumulated prior observations and reported best-fit font sizes before correcting for UGUI pixelsPerUnit. Cleared each case's observation buffer and used the installed UGUI Text scaling contract for the final metadata; actual mesh bounds remain the evidence for fitted numeric glyph size.

Retained 129 final actual Unity PNGs, including all56 matrix pages. Standalone small portraits inspected: [Gameplay](QA/PlayFinal/Gameplay_Normal_ru_360x640.png), [Victory](QA/PlayFinal/Victory_FinaleFirstClear_en_360x640.png), [ChapterComplete](QA/PlayFinal/ChapterComplete_Finale_ru_360x640.png). The review presents one portrait at a time and supports before/after, language, state and viewport selection.

'''
report+=f'''Of {compare['pairedCount']} baseline capture pairs, {compare['pixelIdentityProvenCount']} are pixel-identical. Gameplay differences include the intended stock glyph containment changes; non-identical pairs have 21–187 changed pixels in their captures, maximum channel delta89. Eight Victory pairs differ at 1–72 pixels, maximum delta1/255; other Victory pairs and all ChapterComplete pairs are identical. Layout JSON is unchanged for every paired capture. No full-image identity claim for unequal pairs. [Capture comparison](QA/CaptureComparison.json) · [Decoded differences](QA/MapPixelComparison.json).

Small Gameplay booster captions, several Victory secondary headings/counts and large badge quantities remain below a broad readability target. ChapterComplete's prior larger caption checks still pass. No artwork/text redesign was attempted to force every caption into a universal minimum. Existing typography fallback/provenance warning remains; font assignments/assets were preserved and no OS font files distributed.

## Scope, artifact and next action

Of {scope['baselineFiles']:,} task-start Assets/Packages/ProjectSettings files, **{scope['unchanged']:,} unchanged, 9 changed, 6 added, 0 deleted**. Changed: three canonical prefabs, three owning runtime/source files and three existing Play Mode fixtures. Added: shared touch Graphic, native pointer assertion helper and six-case Edit fixture, each with new metadata. All original source art/import bytes, existing metadata/GUIDs, fonts, packages/settings, Splash and earlier user edits are preserved. Temporary editor helper and diagnostic fixture/meta removed. [Scope](QA/Scope.json) · [Task-only patch](QA/TaskChanges.patch). Generated caches/logs are outside the authored change set; raw logs in `/tmp/lor-core-outcome-readability-r1/`.

No Android build, installation, phone execution, hosting, deployment or commit ran. The verified R17 APK remains {apk['bytes']:,} bytes, SHA256 `{apk['sha256']}`, and **does not yet contain these UI changes**: [artifact check](QA/ExistingApk.json).

{next_action}
'''
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-01 — Core outcome readability and input R1 complete

Completed the approved Gameplay/Victory/ChapterComplete native RU/EN audit. Fixed small-view touch targets with transparent safe-area-constrained child Graphics that reach the existing Buttons; artwork, targetGraphics, actions and geometry remain unchanged. At360×640, Pause37.32×36.60→44×44pixels, VictoryContinue height40.33→44 and both ChapterComplete actions33.73→44. Gameplay four-digit stock now stays on one line with all characters contained. Very small secondary captions and large badge quantities remain a documented limit.

**Final17 Edit Mode +43 Play Mode passed, zero failed/skipped (60 distinct).** Six new fresh/installed repeated-owner identity cases and three new native padding pointer cases verify board/save preservation, existing story routing and one-time chapter acknowledgment/rewards. Matrix covers56 configurations: both languages, four viewports, gameplay normal/large/targeting, round500 first-clear/max score/replay/unavailable and current500-round chapter completion. Finale12 retain the established legacy fixture; installed500-round coverage is separate. InitialEdit15/17 failed only on new order-sensitive identity assertions; corrected to identity-set comparison and complete17/17 passed. Diagnostic buffer/best-fit scaling metadata also corrected before final evidence.

129 final actual Unity captures retained; one-portrait review supplied.16/36 baseline pairs identical, small intended Gameplay stock differences and bounded Victory1/255 differences recorded; all ChapterComplete pairs identical. Every paired layout JSON unchanged. Source art/fonts/import bytes and all existing prefab IDs/references/geometry/revisions22/11/9 preserved; other69prefabs unchanged. Each touched prefab adds8 serialized objects.

[Results](CoreOutcomeReadability/R1/TestResults.md) · [Review](CoreOutcomeReadability/R1/CoreOutcome_R1_Review.html) · [Scope](CoreOutcomeReadability/R1/QA/Scope.json). Of7,792baseline inputs,7,783unchanged/9changed/6added/0deleted. Temporary probe/install helper removed; prior user changes, approved Splash, packages/settings/guards retained. No Android build/device test/deployment/commit. R17 APK unchanged at765,750,206bytes/SHA256{apk['sha256']}; it does not include this task's UI fixes.

**Next:** {next_action}

'''
# Keep new recovery instructions concise and preserve every historical entry.
for old,new in {'At360':'At 360','Pause37':'Pause 37','VictoryContinue':'Victory Continue','Final17':'Final 17','+43':' + 43','covers56':'covers 56','current500':'current 500','Finale12':'Finale 12','installed500':'installed 500','InitialEdit15/17':'Initial Edit 15/17','complete17/17':'complete 17/17','129 final':'129 final','supplied.16/36':'supplied. 16/36','Victory1/255':'Victory 1/255','revisions22/11/9':'revisions 22/11/9','other69prefabs':'other 69 prefabs','adds8':'adds 8','Of7,792baseline inputs,7,783unchanged/9changed/6added/0deleted':'Of 7,792 baseline inputs, 7,783 unchanged/9 changed/6 added/0 deleted','at765,750,206bytes/SHA256':'at 765,750,206 bytes/SHA256 '}.items():entry=entry.replace(old,new)
p=DOCS/'CurrentTask.md';p.write_text(entry+p.read_text());p=DOCS/'SessionLog.md';p.write_text(p.read_text().rstrip()+'\n\n'+entry)
p=DOCS/'DevelopmentSchedule.md';s=p.read_text();marker='**Immediate next action:** ';start=s.index(marker);end=s.index('\n',start);s=s[:start]+f'''**2026-10-01 core outcome input/readability completed:** [R1](CoreOutcomeReadability/R1/TestResults.md) installs safe-area touch padding on three pages and single-line Gameplay stock. Final 17 Edit + 43 Play passed (60 distinct); 56 bilingual/state/display observations and 129 Unity captures retained. Native pointer routes, board/save and reward preservation pass. Existing art/geometry/GUIDs/revisions and other69prefabs retained. R17 APK unchanged and excludes these new UI fixes; secondary captions/phone acceptance remain open.

{marker}{next_action}'''+s[end:];p.write_text(s)
p=DOCS/'ConversationHandoff.md';s=p.read_text();start=s.index('Latest completed task:');end=s.index('\n\nPrepared ',start);s=s[:start]+f'''Latest completed task: [Core outcome readability/input R1](CoreOutcomeReadability/R1/TestResults.md). Added safe-area touch padding on Gameplay/Victory/ChapterComplete without changing painted artwork, existing control identities or targetGraphics; Gameplay quantities no longer wrap. Final17 Edit+43 Play passed (60distinct),56 native RU/EN configurations and129captures. Small secondary captions remain limited; current chapter500 totals and native save/reward/navigation behavior verified. Existing prefab IDs/geometry/revisions22/11/9 and all source art/font/import bytes retained; other69prefabs unchanged. Scope7,783unchanged/9changed/6added/0deleted. R17 APK765,750,206bytes/SHA256{apk['sha256']} remains unchanged and does not include R1 UI changes. Next: {next_action} Preserve historical decisions below.'''+s[end:];p.write_text(s)
print('Report/review and development handoff updated with verified results and APK status.')
