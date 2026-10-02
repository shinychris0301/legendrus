"""Record verified package results and append the project handoff once."""
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
AI = ROOT / 'ProjectDocs/AI'
artifact = json.loads((QA / 'Artifact.json').read_text())
comparison = json.loads((QA / 'ArtifactComparison.json').read_text())
build = json.loads((QA / 'Build/summary.json').read_text())
review = json.loads((QA / 'PayloadReview.json').read_text())
assert build['exitCode'] == 0 and review['changedPayloadCount'] == 6
assert artifact['bytes'] == 765773174 and comparison['savedBytes'] == -22968
assert json.loads((QA / 'Scope.json').read_text())['unchanged'] == 7798
next_action = (
    "Address R1's measured small Gameplay booster captions and Victory secondary headings/counts "
    "in a bounded RU/EN Unity pass. Use live text and current font assignments; preserve source "
    "artwork, controls, save/reward behavior and unavailable-offer semantics. Keep large inventory "
    "badge limits explicit. Phone execution and hosting remain deferred; human Chapter 1 pacing, "
    "font provenance and Android lifecycle/performance remain open. Final gameplay effects stay last."
)
report = f"""# Core outcome Android refresh R2 — 2026-10-01

Built and verified the approved next scheduled development APK with the installed [R1 Gameplay, Victory and ChapterComplete fixes](../R1/TestResults.md). The package now contains the touch-target script, three updated page prefabs and compiled numeric/touch owner behavior. All **130 Android policy/content Edit Mode checks passed**, with zero failures/skips. Unity 6000.5.9f1 exited 0 and its actual build report returned Succeeded. No production source, artwork, prefab, package or project setting was changed during this packaging task.

## Delivered APK

| Measurement | Preserved R17 | R2 refresh |
| --- | ---: | ---: |
| APK bytes | 765,750,206 | 765,773,174 |
| Packed object rows | 29,322 | 29,347 |
| ZIP paths | 403 | 403 |

Size increase: **22,968 bytes (0.003%)**, not a new footprint saving. [Artifact](QA/Artifact.json) · [Exact package comparison](QA/ArtifactComparison.json).

APK: [LegendsOfRus-v1.17-step01-development.apk](../../../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk).

SHA256: `{artifact['sha256']}`. The adjacent current checksum sidecar matches. R17 and its verified inherited sidecar are preserved under `Builds/Verification/v1_17_step01/BeforeCoreOutcomeUiRefreshR2/`; [archive record](QA/PreviousArtifact.json).

The existing identity remains `com.example.legendsofrus.v117debug`, versionCode1 / versionName0.0.0, ARM64, API26–36, portrait, debuggable and Android Debug signed. No release/custom signing, version bump, installation, hosting, deployment or commit was performed.

## Build and checks actually run

- Current Android policy/content suite: 130 distinct Edit Mode cases passed in {json.loads((QA/'Policy/summary.json').read_text())['durationSeconds']:.2f}s. Includes existing policy, guard, resolved-package, request/options, credential-plugin and content-delivery cases. Isolated test configuration/storage was used. [Summary](QA/Policy/summary.json) · [Actual XML](QA/Policy/results.xml).
- Existing AndroidDevelopmentBuildCommand.BuildDevelopmentApk ran through its normal approval/fingerprint, options, Addressables, player, IL2CPP, linker and Gradle path. Unity process exited 0 in {build['durationSeconds']:.2f}s; the temporary report helper required a Succeeded report and nonempty packed inventory. Existing guards remained active. [Build invocation](QA/Build/summary.json) · [Actual packed report](QA/PackedAssets.json).
- Signing and same-certificate comparison against archived R17, manifest identity/ABI/API/debug/portrait, unchanged permissions, all403 ZIP CRCs, exact ZIP accounting with zero unindexed bytes, 16 KiB ZIP alignment and all six ARM64 ELF load-segment alignments passed. IL2CPP metadata contains the new touch class and retained profile/runtime markers. [APK inspection](QA/ApkVerification.json).
- Nine compiled credential DEX classes, JNI method definitions/operation fields and all four Java source/meta pins passed. No player credentials or signing secrets were read or printed by these audits. [DEX definitions](QA/CredentialDex.json) · [Pinned source inventory](QA/PinnedPluginInventory.json).

The 17 Edit Mode and 43 Play Mode UI checks and 129 Unity captures belong to the preceding R1 task; they were not rerun or added to this task's 130 passes. No new Play Mode test, visual capture, full R5 regression, human pacing study or physical-device execution ran. A successful build/package inspection does not establish Android touch/readability, lifecycle or performance acceptance.

Unity launches used reviewed permissions for local Package Manager IPC. Raw logs are in `/tmp/lor-core-outcome-android-refresh-r2/`. Prior APK/checksum were verified before archival. Only the three generated launcher packaging directories were isolated; player assets, IL2CPP project, signing configuration and settings were retained. [Packaging isolation](QA/PackagingIsolation.json).

## Reviewed package differences

Exactly 24 page objects and one MonoScript were added to packed membership. Gameplay405→413: one GameObject, RectTransform and CanvasRenderer plus five MonoBehaviours (touch target and four numeric effects). Victory362→370 and ChapterComplete188→196 each add two native touch children. The new MonoScript GUID matches the installed ProductionControlTouchTarget metadata. All other packed path/GUID/type membership is identical. Packed object bytes are separate from compressed APK size and device-memory measurements. [Strict packed comparison](QA/PackedComparison.json).

All403 ZIP paths remain; **397 payloads are byte-identical and six differences are reviewed**: player data, managed metadata, libil2cpp.so, libunity.so, boot.config and unity_app_guid. Stable boot settings are identical; only build/player-connection identifiers change. [Payload review](QA/PayloadReview.json).

The sixth difference, libunity.so (+1,288 uncompressed bytes), was examined before acceptance. Its UnityEngine binding inventory grows1,159→1,160 with exactly `UnityEngine.CanvasRenderer::set_cullTransparentMesh_Injected` added and no bindings removed. The new touch Graphic calls this setter. Dynamic symbol definitions remain identical, and the actual build log records Unity's normal engine link/strip steps. This supports the interpretation that the added native call caused the stripped engine regeneration; it is not a full binary-identity or device-runtime claim. [Native engine comparison](QA/NativeEngineComparison.json).

## Preserved scope and next action

All **7,798 task-start Assets/Packages/ProjectSettings inputs are byte-identical: zero changed, added or deleted**. Source art, fonts, existing/new GUIDs, approved Splash, all prefabs and their installed R1 modifications, prior user edits, package locks, project settings, guards and plugin pins are retained. [Scope](QA/Scope.json). Only two newly generated Addressables linker files were removed after building; existing generated metadata was protected. Temporary build helper/meta removed normally, with no recovery needed. Caches/backups/logs remain outside the authored change set. [Generated cleanup](QA/GeneratedFileCleanup.json).

The authored additions are this task's scripts, JSON/XML/package evidence and report, plus CurrentTask/SessionLog/Schedule/Handoff updates. No production task patch is needed. [Final evidence validation](QA/FinalValidation.json) rechecks artifact/sidecars, project hashes, test counts, package evidence and new report links.

{next_action}
"""
(BASE / 'TestResults.md').write_text(report)
heading = '# 2026-10-01 — Core outcome Android refresh R2 complete'
entry = f"""{heading}

The user approved the next scheduled APK refresh. Built the installed R1 Gameplay/Victory/ChapterComplete touch/numeric fixes through the unchanged guarded development entry. **130 current Android policy/content Edit Mode cases passed**, zero failed/skipped. Unity exited0; actual Succeeded build report; {build['durationSeconds']:.2f}s. Prior R1's 60 UI checks/129 captures remain historical, not new runtime or phone acceptance.

**APK765,773,174 bytes, +22,968 bytes (0.003%) from preserved R17.** SHA256 `{artifact['sha256']}`. Current checksum matches. R17 APK765,750,206 bytes/SHA256 c83940412715c9ebed87eb3e47b12bdd971cb8aae8a48735f015dd9ac9279fbe and verified sidecar archived BeforeCoreOutcomeUiRefreshR2. Identity/signing/permissions/ABI/API/portrait/debug retained. ZIP CRC/exact accounting/no unused gaps, 16KiB ZIP/six ELF alignment, runtime markers, nine credential DEX/JNI definitions and four Java source/meta pins pass.

Exactly24 UI packed objects and one MonoScript added (29,322→29,347); all other path/GUID/type membership unchanged. All403 ZIP paths retained,397 payloads identical; six reviewed data/code/build-ID changes. Regenerated libunity.so examined: exactly one added CanvasRenderer cullTransparentMesh native setter binding, no removed bindings, identical dynamic symbol definitions, regular Unity link/strip steps. Stable boot settings unchanged.

[Results](CoreOutcomeReadability/R2AndroidRefresh/TestResults.md) · [APK](../../Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk) · [Scope](CoreOutcomeReadability/R2AndroidRefresh/QA/Scope.json). All7,798 project inputs unchanged; zero source/prefab/art/settings/packages/GUID/guard changes. Only two new generated linker files cleaned; temporary helper/meta removed normally. Added task scripts/evidence/docs only. No new Play Mode/capture, phone execution, hosting, deployment or commit.

**Next:** {next_action}

"""
current = AI / 'CurrentTask.md'
assert not current.read_text().startswith(heading)
current.write_text(entry + current.read_text())
log = AI / 'SessionLog.md'
assert heading not in log.read_text()
with log.open('a') as stream:
    stream.write('\n' + entry)
schedule = AI / 'DevelopmentSchedule.md'
lines = schedule.read_text().splitlines(True)
indices = [i for i, line in enumerate(lines) if line.startswith('**Immediate next action:**')]
assert len(indices) == 1
index = indices[0]
lines[index] = '**Immediate next action:** ' + next_action + '\n'
lines[index:index] = [f"**2026-10-01 core outcome APK refresh completed:** [R2](CoreOutcomeReadability/R2AndroidRefresh/TestResults.md) builds installed R1 touch/numeric fixes through normal guards.130 current policy/content checks passed; Unity exit0/Succeeded. APK765,773,174bytes (+22,968), same signing/identity/permissions; ZIP/ELF/JNI checks pass.24 UI objects+one MonoScript added; unrelated packed membership retained.397/403 payloads identical; regenerated native engine's added CanvasRenderer setter binding reviewed. All7,798 source/config inputs unchanged. Prior UI results remain historical; phone execution remains deferred.\n", '\n']
schedule.write_text(''.join(lines))
handoff = AI / 'ConversationHandoff.md'
text = handoff.read_text()
start = text.index('Latest completed task:')
end = text.index('\n\nPrepared 2026-09-12.', start)
orientation = f"""Latest completed task: [Core outcome Android refresh R2](CoreOutcomeReadability/R2AndroidRefresh/TestResults.md). The current APK now includes R1's installed Gameplay/Victory/ChapterComplete touch and numeric changes. Current 130 Android policy/content Edit Mode cases passed; Unity exited 0 and the actual guarded build report Succeeded. APK 765,773,174 bytes (+22,968 from R17), SHA256 `{artifact['sha256']}`. Current sidecar verified; prior R17 and sidecar preserved under BeforeCoreOutcomeUiRefreshR2. Signing, identity, permissions, ZIP/ELF alignment and credential JNI checks pass. Exactly 24 UI packed objects and one MonoScript added; unrelated membership unchanged. All 403 ZIP paths retained, 397 payloads identical; six reviewed changes include the regenerated native engine's one added CanvasRenderer setter binding. All 7,798 task-start project inputs unchanged. No phone execution, hosting, deployment or commit.

The preceding [R1 UI audit](CoreOutcomeReadability/R1/TestResults.md) passed 17 Edit and 43 Play cases and retained 129 Unity captures across 56 RU/EN configurations. Those results remain historical; Android readability/lifecycle/performance acceptance is separate. Small secondary captions and large badge quantities remain limited. Approved art, font assignments, GUIDs, control identities, saves/rewards and prior acceptance decisions are preserved.

Next: {next_action} Preserve historical decisions below."""
handoff.write_text(text[:start] + orientation + text[end:])
print('Wrote actual R2 results and updated the latest project handoff; historical entries retained.')
