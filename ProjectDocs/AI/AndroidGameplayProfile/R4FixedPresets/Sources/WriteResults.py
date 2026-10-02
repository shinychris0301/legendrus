from pathlib import Path
import json
BASE=Path(__file__).resolve().parent.parent
r=json.loads((BASE/'QA/Reconciliation.json').read_text())
v=json.loads((BASE/'QA/VisualInspection.json').read_text())
assert r['result']=='Passed'
text=f'''# Fixed disposable presets R4 — test results, 2026-10-02

Implemented six bounded selectors in the existing development profile and a pure validated-content factory. Actual cold launches produce all six complete independent R3 reference payloads without test seeding; separate resume processes restore them exactly. Existing commands and normal saves remain isolated. [Implementation](ImplementationNotes.md) describes ownership/commit order and known maximum-stock limits.

| Current check | Passed | Failed/skipped | Evidence |
|---|---:|---|---|
| Unity Edit Mode: 23 new factory + 27 existing recap/Profile/story cases | 50 | 0/0 | [Summary](QA/EditVerified/summary.json), [XML](QA/EditVerified/results.xml) |
| Unity native Play Mode: six recap + two exact-stock/input cases | 8 | 0/0 | [Summary](QA/NativeVerified/summary.json), [XML](QA/NativeVerified/results.xml) |
| Separate cold process phases: six preset writer/read pairs, five original QA phases, normal before/after | 19 | 0/0 | [Phase plan](Sources/PhasePlan.json), [Reconciliation](QA/Reconciliation.json) |
| Actual profile/factory compiled with development/release defines, fake JNI/content/preferences | 120 | 0 | [Guard summary](QA/EntryGuards/summary.json) |

There are **{r['unityDistinctTestCasesPassed']} distinct Unity test cases and77 successful case executions across21 final processes**, all launchers exit0. The19 cold phases execute two staged methods, not19 distinct test names. This is targeted current coverage, not another full sweep; R7's1,377 broader cases remain historical. The120 compiled checks are separate from Unity and use a simulated Android boundary; no actual Android intent, release build, device or lifecycle verification occurred.

Initial [EditFixtures](QA/EditFixtures/summary.json) completed49passes/onefailure, exit2: Unity/Mono surfaced `OverflowException` directly through reflection, while the test expected a wrapper. The factory correctly rejected overflow. The fixture now normalizes optional `TargetInvocationException` at its reflection helper and asserts the exact underlying exception. [Initial source](Sources/EditFixtureInitial.cs), XML and log are retained; the corrected complete50-case suite passed. No product correction or weakened negative assertion was required.

The six actual writer DTOs match [R3 independent service references](../R3FixturePreparation/FixturePlan.json) field-for-field except the fresh energy timestamp; all six cold readers restore their exact written DTO. Early story has eight completions/24stars/three recap scenes and round9selected; finale has500completions/1500stars/58scenes and Chapter1acknowledged, round500selected. Chapter2 remains unavailable. Each story reader returns from actual Dialogue to the same History page2 with full Save/journal unchanged; replay rejects duplicate gold/gems/four booster grants. Five synthetic normal preference fingerprints survive every phase and final no-command restoration.

Native current tests cover48 RU/EN large-stock configurations and exact text fit, visible count/card pointer forwarding, busy duplicate consumption,1 000→999 and+5moves with unchanged board hash. Existing recap tests cover truthful empty eligibility, native Earlier/Later/Continue/Back, repeated taps, finale reentry/finish, retirement and QA/normal journal isolation. Stock rendering tests establish Editor layout and transaction behavior, not physical ten-digit legibility. `stock-max` is display/consumption only because ordinary booster reward additions have no overflow guard.

**{r['nativeUnityPngCount']} actual Unity PNGs** recorded in the native run; [capture inventory](QA/Captures.json) pins hashes. {len(v['inspected'])} standalone simulated portrait pages were visually inspected (Gameplay stock and Profile recap); [inspection record](QA/VisualInspection.json) distinguishes Editor capture from phone evidence. Approved Splash and all visual assets/prefabs/builders remain byte-identical; no redesign or new visual acceptance is claimed.

[Scope](QA/Scope.json) and [final validation](QA/FinalValidation.json) reconcile the inherited tree:7,832of7,833original project inputs unchanged; one existing profile source changed, three C# source/meta pairs added, no deletion. New GUIDs unique; all original metadata/art/fonts/prefabs/settings/packages/backend/other services/tests and inherited changes preserved. [Changed files](QA/ChangedFiles.json), [reviewable patch](Sources/TaskChanges.patch). Six continuity/usage/checklist documents updated, prior acceptance retained. Five normal manual recorder exports unchanged; test stores are isolated under `/tmp/lor-gameplay-profile-r1/fixture-r4/`, not normal personal save journals.

Current R3 APK/sidecar stays766,017,314bytes, SHA256`2302b069bddf4b0261513fa5696bfa60dc9940f1c9bd61bc34a54c063079335e` and **does not include the new presets**. No Android build/device/ADB/install/hosting/deployment/commit. All18physical rows remain pending. Next is a separately guarded APK refresh with current policy/content, signing/identity/permissions/ZIP/ELF/JNI/compiled inclusion and membership inspection. Human201–210 recording remains parallel; phone/hosting remain deferred and final gameplay effects stay last.
'''
(BASE/'TestResults.md').write_text(text)
