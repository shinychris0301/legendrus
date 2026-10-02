"""Record verified R17 results and preserve all earlier acceptance decisions."""
from pathlib import Path
import json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';DOCS=ROOT/'ProjectDocs/AI'
def read(name):return json.loads((QA/name).read_text())
a=read('Artifact.json');c=read('ArtifactComparison.json');packed=read('PackedComparison.json');scope=read('Scope.json');prefab=read('PrefabPreservation.json');rows=read('CandidatesBefore.json')
assert read('Build/summary.json')['exitCode']==0 and read('PackedAssets.json')['result']=='Succeeded'
assert read('EditFinal/summary.json')['result']['passed']=='211' and read('PlayFinal/summary.json')['result']['passed']=='44'
next_action='Return to the core UI acceptance schedule with a bounded native RU/EN readability and input audit of Gameplay, Victory and ChapterComplete at small and tall portrait sizes, including long values and chapter-final states. Reuse the current router/services and preserve rewards, saves, artwork, fonts and existing acceptance. Measure actual glyph clipping, touch targets and recovery/navigation before proposing any fixes. Retain all nine live/shared Home resources. Physical-phone execution and hosting remain deferred; human Chapter1 pacing, font provenance and Android lifecycle/performance remain open; final gameplay effects stay last.'
report=f'''# Home dormant artwork cleanup R17 — 2026-10-01

The user approved the scheduled cleanup. Removed nine dormant Home resource loads and assignments, cleared exactly nine disabled Image sprite fields in the active Home prefab, and archived each PNG/meta pair outside Resources. Current Home controls, source artwork, localizable labels, loading services, routes and revisions are preserved. This completes the R14 dormant Home removal candidates; nine live/shared resources remain.

ProductionHomeRuntime now requests nine retained sprites in fresh/legacy construction and zero in current migration. Retired nav/medallion/quest/foliage placeholders start empty; gift geometry is selected by the existing button identity rather than a retired sprite name. Current runtime and source owners clear reinjected dormant sprites after live targets are assigned. Control targets and enabled field Images are protected. The historical backdrop and lighting diagnostics validate current artwork and inactive historical overlays/effects. Lighting components/materials and serialized objects are retained.

## Resources and preservation

| Archived HomeAligned resource | Dimensions | Preserved GUID |
| --- | --- | --- |
'''
for r in rows:report+=f"| {r['name']} | {r['width']} × {r['height']} | `{r['guid']}` |\n"
report+='''
All nine sprites retain RGBA32, 11 mip levels, trilinear filtering, Read/Write disabled and the existing Android RGBA32/4096 override. LockedDesignImporter explicitly owns these nine exact archive paths; no blanket archive-folder rule. [PNG/meta byte proof](QA/ArchiveByteVerification.json) and [Unity import/dependency evidence](QA/EditFinal/nav_map_generated.json) are retained. Each archive has zero consumers across 99 recursive serialized dependency roots. Current source/GUID tracing found one historical Home Image per resource before the edit, no other GUID consumers: [inventory](QA/CandidatesBefore.json).

The nine retained HomeAligned resources are home_logo, avatar_portrait, avatar_ring, resource_capsule, resource_plus, settings_medallion, coin_icon, gems_icon and energy_icon. Login logo, Gameplay portrait and PreLevel currency dependency checks pass. Current clean_environment/source_hero/source_logo/source_header, navigation, mail/gift/footer and quest artwork remain live.

'''
report+=f'''Home keeps all {prefab['homeSerializedObjectCount']} serialized object headers. Its only byte changes are the nine sprite fields; other references, IDs, geometry and revisions are identical. All other 71 prefabs are byte-identical: [proof](QA/PrefabPreservation.json).

## Unity verification

Unity 6000.5.9f1 (b57deb96f08d). Isolated test storage/configuration, existing bootstrap/router/Canvas and process-only Vulkan used. Unity launches used reviewed permissions for local Package Manager IPC.

- Baseline: four native capture tests passed before implementation; all 7,790 task-start project inputs verified unchanged before editing. These four methods repeat in final coverage and are not counted twice.
- Final Edit Mode: **211 passed, zero failed/skipped**, including 130 Android policy/content checks and 20 new permanent cleanup cases. Archives/GUIDs/imports/dependencies, fresh/legacy/current loads9/9/0, repeated stale-sprite clearing with object/component/live-target preservation, live-target protection, historical diagnostics, fresh/installed cloned-header pages and retained graphs verified. Updated two prior fixtures' expected load count18→9, removed their obsolete retained quest_reward_frame expectation, and moved the missing-unrelated-resource probe to retained resource_capsule before running. No Unity compile/test failure or production correction was required.
- Final Play Mode: **44 passed, zero failed/skipped**. Home13, WorldMap7, ChapterMap5, PreLevel6, Home preparation7, Loading4, Splash2. Native controls/navigation, inventory, RU/EN, portraits, measured progress/retry/cancellation covered. **255 distinct final passes**; no new broad R5/full-game or device acceptance claim.

[Edit results](QA/EditFinal/summary.json) · [Play results](QA/PlayFinal/summary.json) · [Coverage](QA/TestCoverage.json). A local preservation checker initially filtered only 36 /Pages/ prefabs while expecting all72; corrected that audit filter to include all prefab files. Production and Unity tests were unaffected.

Retained **100 final actual Unity PNG captures**, compared 35 task-start pairs. All14Home/5ChapterMap/7PreLevel and7WorldMap pairs are pixel-identical (33/35). Two tall WorldMap pairs differ9/17pixels, maximum channel delta1/255, identical layout JSON; the same recorded pattern as R15/R16. These two pairs are not claimed identical. [Pixel comparison](QA/CaptureComparison.json) · [Decoded differences](QA/MapPixelComparison.json).

Standalone actual Unity pages inspected: [Home](QA/PlayFinal/HOME/Home_941x1672_RU.png), [WorldMap](QA/PlayFinal/WORLDMAP/WorldMap_941x1672_RU.png), [PreLevel](QA/PlayFinal/PRELEVEL/PreLevel_941x1672_RU.png). No offline/generated preview or physical-phone acceptance is claimed.

## Guarded Android package

Existing AndroidDevelopmentBuildCommand ran with its normal approval/fingerprint/options/plugin guards. Unity process exited0; actual build report Succeeded; {read('Build/summary.json')['durationSeconds']:.2f}s invocation. Prior R16 APK and inherited sidecar preserved under BeforeHomeDormantArtworkCleanupR17; its SHA verified before archival. Only three generated launcher packaging directories were isolated, leaving signing/settings/player asset build paths intact. Temporary audit helper/meta removed normally; no wrapper recovery was needed.

| Measurement | R16 | R17 |
| --- | ---: | ---: |
| APK bytes | {c['old']['bytes']:,} | {a['bytes']:,} |
| Packed rows | {packed['beforeRows']:,} | {packed['afterRows']:,} |
| ZIP paths | 403 | 403 |

**Measured saving: {c['savedBytes']:,} bytes ({c['savedPercent']:.3f}%).** Exactly27 packed rows for the nine candidates removed; all other path/GUID/type membership unchanged. Removed packed-object bytes {packed['removedPackedObjectBytes']:,} are a separate metric, not compressed APK or device-memory savings. All18 retired artwork GUIDs absent. [Packed comparison](QA/PackedComparison.json) · [APK comparison](QA/ArtifactComparison.json).

APK: `{a['path']}`. SHA256 `{a['sha256']}`. Current SHA sidecar matches.

Signing/certificate, manifest identity com.example.legendsofrus.v117debug version1/0.0.0, ARM64 API26→36, portrait/debug, unchanged permissions, ZIP CRC, zero unindexed bytes, 16KiB ZIP and six ELF alignment checks passed. All403ZIPnames retained;398payloads byte-identical. Five expected asset/code/build-ID payload changes checked; boot settings differ only in build/player-connection identifiers. Nine credential DEX classes/JNI methods/operation fields and four Java source/meta pins pass. [APK checks](QA/ApkVerification.json) · [Payload review](QA/PayloadReview.json) · [DEX evidence](QA/CredentialDexDefinitions.json).

## Scope and next action

Of7,790task-start Assets/Packages/ProjectSettings inputs, **{scope['unchanged']:,}unchanged / {len(scope['changed'])}changed / {len(scope['added'])}added / {len(scope['deleted'])}removed**. Changes: active Home prefab, three Home runtime/view/composition owners, importer and two existing regressions. Additions/removals include18byte-identical moved paths and a new permanent test source/meta. [Exact scope](QA/Scope.json) · [Task-only patch](QA/TaskChanges.patch). Only two newly generated linker files cleaned; prior user edits, packages/settings, fonts, approved Splash and Android guards preserved. Caches/logs/backups are outside the authored patch. No commit, deployment or phone execution. Raw local logs: `/tmp/lor-home-dormant-artwork-cleanup-r17/`.

{next_action}
'''
# Use the actual exported DEX filename.
for p in QA.glob('*Dex*.json'):
 if p.name!='CredentialDexDefinitions.json':report=report.replace('QA/CredentialDexDefinitions.json','QA/'+p.name)
(BASE/'TestResults.md').write_text(report)
entry=f'''# 2026-10-01 — Home dormant artwork cleanup R17 complete

Removed nine remaining dormant Home-only resource loads/assignments and exactly nine disabled active Home prefab sprite fields; runtime/source owners clear stale sprites while protecting current targets. Historical backdrop/lighting diagnostics track current art and inactive overlays/effects. All480serialized Home object headers and otherbytes/references/revisions preserved; other71prefabs byte-identical. Nine PNG/meta pairs archived outside Resources with exact bytes/GUIDs and RGBA32/11mips/trilinear/Android4096 imports. Nine live/shared resources retained, including Login logo, Gameplay portrait and currency.

**211 Edit Mode +44 Play Mode passed, zero failed/skipped (255 distinct).** Twenty new regressions cover archives99dependencyroots each, fresh/legacy/current loads9/9/0, stale sprite reinjection/component/target preservation, live protection, diagnostics, cloned headers and retained graphs. Two existing fixtures updated18→9; unrelated missing-key probe now uses retained resource_capsule. No Unity failure or production correction. A source preservation checker initially counted /Pages/36instead of all72prefabs; corrected only that filter.

**33/35 paired captures pixel-identical**, including all14Home/5ChapterMap/7PreLevel. Two tall WorldMap pairs differ9/17pixels max1/255, same layout JSON, same R15/R16 pattern.100actual Unity captures retained; standalone Home/WorldMap/PreLevel inspected. No phone acceptance claim.

**Guarded APK {a['bytes']:,} bytes, down {c['savedBytes']:,} bytes ({c['savedPercent']:.3f}%) from R16.** SHA256 `{a['sha256']}`. Unity exit0 and actual Succeededreport; no recovery needed. Exactly27packed rows removed ({packed['removedPackedObjectBytes']:,}objectbytes, distinct from APK saving); othermembership unchanged and18retiredGUIDs absent from29,322rows.403ZIPpaths retained/398payloads identical; five expected payload changes and bootidentifiers checked. Signing/identity/permissions/CRC/ZIP/sixELF alignment/JNI/fourJava source/meta pins passed. Prior APK archived BeforeHomeDormantArtworkCleanupR17.

[Results](AndroidPackageFootprint/R17HomeDormantArtworkCleanup/TestResults.md) · [Scope](AndroidPackageFootprint/R17HomeDormantArtworkCleanup/QA/Scope.json) · [Package comparison](AndroidPackageFootprint/R17HomeDormantArtworkCleanup/QA/ArtifactComparison.json). Of7,790baselineinputs,7,765unchanged/7changed/20added/18removed includes18exactmove paths and newtest/meta. Temporary helper removed; only new linker files cleaned. Previous user changes, approved Splash, fonts/settings/packages/guards preserved. No commit, deployment or device run.

**Next:** {next_action}

'''
p=DOCS/'CurrentTask.md';p.write_text(entry+p.read_text())
p=DOCS/'SessionLog.md';p.write_text(p.read_text().rstrip()+'\n\n'+entry)
p=DOCS/'DevelopmentSchedule.md';s=p.read_text();marker='**Immediate next action:** ';start=s.index(marker);end=s.index('\n',start);s=s[:start]+f'''**2026-10-01 Home dormant artwork cleanup completed:** [R17](AndroidPackageFootprint/R17HomeDormantArtworkCleanup/TestResults.md): nine temporary dependencies and nine disabled Home prefab sprite fields removed with owners/diagnostics aligned. Nine exact PNG/meta archives; other71prefabs/current controls retained. Final211Edit+44Playpassed (255distinct).33/35captures identical; two tiny WorldMap differences with unchanged layouts. Guarded APK{a['bytes']:,}bytes, down{c['savedBytes']:,}bytes ({c['savedPercent']:.3f}%),27packedrowsremoved. Unityexit0, signing/package/JNIchecks passed. Nine live/sharedresources retained; no phone run.

{marker}{next_action}'''+s[end:];p.write_text(s)
p=DOCS/'ConversationHandoff.md';s=p.read_text();start=s.index('Latest completed task:');end=s.index('\n\nPrepared ',start);s=s[:start]+f'''Latest completed task: [Home dormant artwork cleanup R17](AndroidPackageFootprint/R17HomeDormantArtworkCleanup/TestResults.md). Nine Home-only loads/assignments and nine disabled prefab sprite fields removed; owners and historical diagnostics aligned. Exact PNG/meta archives/imports; all480Home object headers/otherbytes retained, other71prefabs unchanged. Final211Edit+44Playpassed (255distinct),33/35captures identical, two prior-pattern WorldMap differences9/17pixels max1/255/samegeometry. Guarded APK{a['bytes']:,}bytes, down{c['savedBytes']:,}bytes ({c['savedPercent']:.3f}%), SHA256{a['sha256']};Unityexit0/Succeededreport,27packedrowsremoved,all18retiredGUIDsexcluded, signing/package/JNIchecks pass. Scope7,765unchanged/7changed/20added/18removed includes18exactmoves/newtest. Nine live/sharedHome resources retained. Next: {next_action} Preserve historical decisions below.'''+s[end:];p.write_text(s)
print('Report, CurrentTask, SessionLog, schedule and handoff updated from actual evidence.')
