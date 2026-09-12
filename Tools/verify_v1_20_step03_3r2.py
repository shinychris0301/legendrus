#!/usr/bin/env python3
from __future__ import annotations
import argparse
import hashlib
import json
from pathlib import Path
import struct
import sys

ALLOW_CHANGED = {
    "README.md",
    "MILESTONE_VERSION.txt",
    "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
    "Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs",
}
EXPECTED_MILESTONE = "v1.20-step03.3R2-home-world-map-chapter-map-visual-rebuild"
ART_ROOT = "Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatch"
REFERENCE_ROOT = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens"
EXPECTED_REFS = {
    "Home.png": "7509bdd1fee910207eb5287a1eb69700c6ac44f816fd4f209e524d56be2b64e8",
    "WorldMap.png": "b13e01a68e3ef8641e6094abefabe34faa3c1a0d2e2cec9f7fb2a5afc7993e19",
    "ChapterMap.png": "e69143c5ec1d2122fed9e177c8b0cc48bee0cdc657d50080a2cf7113023122bf",
}
EXPECTED_COMPONENTS = {
    "home_top_hud.png": (941,150),
    "home_logo_scene.png": (941,350),
    "home_hero_scene.png": (941,500),
    "home_play_component.png": (941,210),
    "home_navigation_component.png": (941,240),
    "home_quest_component.png": (941,222),
    "worldmap_top_hud.png": (941,135),
    "worldmap_title_component.png": (941,150),
    "worldmap_painted_map_plate.png": (941,1055),
    "worldmap_bottom_navigation.png": (941,332),
    "chaptermap_top_hud.png": (941,140),
    "chaptermap_ribbon_component.png": (941,250),
    "chaptermap_painted_level_path.png": (941,1060),
    "chaptermap_bottom_controls.png": (941,222),
}


def sha(path: Path) -> str:
    return hashlib.sha256(path.read_bytes()).hexdigest()


def files(root: Path):
    return {p.relative_to(root).as_posix(): p for p in root.rglob("*") if p.is_file()}


def png_size(path: Path):
    data = path.read_bytes()[:24]
    if len(data) < 24 or data[:8] != b"\x89PNG\r\n\x1a\n":
        raise ValueError("not PNG")
    return struct.unpack(">II", data[16:24])


def check_no_newline_in_normal_csharp_string(text: str) -> bool:
    # Lightweight scanner intended to catch the Step18 regression pattern.
    NORMAL, STRING, CHAR, LINE, BLOCK, VERBATIM = range(6)
    state = NORMAL
    i = 0
    while i < len(text):
        c = text[i]
        n = text[i+1] if i+1 < len(text) else ""
        if state == NORMAL:
            if c == '/' and n == '/': state = LINE; i += 1
            elif c == '/' and n == '*': state = BLOCK; i += 1
            elif c == '@' and n == '"': state = VERBATIM; i += 1
            elif c == '"': state = STRING
            elif c == "'": state = CHAR
        elif state == STRING:
            if c == '\\': i += 1
            elif c == '"': state = NORMAL
            elif c in '\r\n': return False
        elif state == CHAR:
            if c == '\\': i += 1
            elif c == "'": state = NORMAL
            elif c in '\r\n': return False
        elif state == VERBATIM:
            if c == '"' and n == '"': i += 1
            elif c == '"': state = NORMAL
        elif state == LINE:
            if c == '\n': state = NORMAL
        elif state == BLOCK:
            if c == '*' and n == '/': state = NORMAL; i += 1
        i += 1
    return state not in (STRING, CHAR)


def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--candidate', default=str(Path(__file__).resolve().parents[1]))
    ap.add_argument('--baseline', required=True)
    ap.add_argument('--output', default='')
    args=ap.parse_args()
    candidate=Path(args.candidate).resolve(); baseline=Path(args.baseline).resolve()
    failures=[]
    bm=files(baseline); cm=files(candidate)
    added=sorted(set(cm)-set(bm)); deleted=sorted(set(bm)-set(cm))
    changed=[]
    for rel in sorted(set(bm)&set(cm)):
        if sha(bm[rel]) != sha(cm[rel]): changed.append(rel)

    if deleted: failures.append(f"deleted baseline files: {deleted}")
    unexpected=[x for x in changed if x not in ALLOW_CHANGED]
    if unexpected: failures.append(f"unexpected inherited changes: {unexpected}")
    missing_allow=[x for x in ALLOW_CHANGED if x not in changed]
    if missing_allow: failures.append(f"expected allowlisted changes missing: {missing_allow}")

    # Every inherited PNG remains byte-identical; new R2 PNGs are additive only.
    baseline_png=[r for r in bm if r.lower().endswith('.png')]
    altered_png=[r for r in baseline_png if r not in cm or sha(bm[r]) != sha(cm[r])]
    if altered_png: failures.append(f"inherited PNG changed: {altered_png}")

    for prefix in ("Packages/", "ProjectSettings/"):
        altered=[r for r in bm if r.startswith(prefix) and (r not in cm or sha(bm[r]) != sha(cm[r]))]
        if altered: failures.append(f"protected {prefix} changed: {altered}")

    baseline_tests=[r for r in bm if r.startswith('Assets/LegendsOfRus/Tests/')]
    altered_tests=[r for r in baseline_tests if r not in cm or sha(bm[r]) != sha(cm[r])]
    if altered_tests: failures.append(f"inherited tests changed: {altered_tests}")

    baseline_runtime=[r for r in bm if r.startswith('Assets/LegendsOfRus/Scripts/Runtime/')]
    altered_runtime=[r for r in baseline_runtime if r not in cm or sha(bm[r]) != sha(cm[r])]
    if altered_runtime: failures.append(f"inherited runtime code changed: {altered_runtime}")

    for rel in [
        'Assets/LegendsOfRus/Resources/Config/chapter01_levels.json',
        'Assets/LegendsOfRus/Resources/Config/chapter01_story.json',
        'Assets/LegendsOfRus/Resources/Localization/story_ru.json',
    ]:
        if rel in bm and sha(bm[rel]) != sha(cm[rel]): failures.append(rel + ' changed')

    milestone=(candidate/'MILESTONE_VERSION.txt').read_text().strip()
    if milestone != EXPECTED_MILESTONE: failures.append('milestone mismatch: '+milestone)

    for name, expected in EXPECTED_REFS.items():
        p=candidate/REFERENCE_ROOT/name
        if not p.is_file() or sha(p) != expected: failures.append('approved reference hash mismatch: '+name)

    actual_components=sorted(p.name for p in (candidate/ART_ROOT).glob('*.png'))
    if actual_components != sorted(EXPECTED_COMPONENTS):
        failures.append('component inventory mismatch')
    for name,size in EXPECTED_COMPONENTS.items():
        p=candidate/ART_ROOT/name
        if not p.is_file(): failures.append('missing component: '+name); continue
        actual=png_size(p)
        if actual != size: failures.append(f'{name} dimensions {actual} != {size}')
        if actual == (941,1672): failures.append('full-screen production screenshot forbidden: '+name)

    proof_path=candidate/'ProjectDocs/UIReferenceMatch/v1_20_step03_3r2_pixel_proof.json'
    proof=json.loads(proof_path.read_text())
    for page in ('Home','WorldMap','ChapterMap'):
        p=proof.get(page,{})
        if not p.get('exactPixelReconstruction') or p.get('differentPixels') != 0 or p.get('maxChannelError') != 0:
            failures.append('static pixel proof failed: '+page)

    activation=json.loads((candidate/'Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r2.json').read_text())
    true_flags=['referenceMatchRebuildAuthorized','referenceDerivedProductionComponentsAuthorized','homeReferenceMatchCutoverAuthorized','worldMapReferenceMatchCutoverAuthorized','chapterMapReferenceMatchCutoverAuthorized','staticComponentPixelReconstructionVerified','manualUnityRenderReviewRequired']
    false_flags=['finalReferenceMatchSignoffGranted','fullScreenScreenshotRuntimeUseAuthorized','screenshotDeletionAuthorized','lockedReferenceOverwriteAuthorized','automaticArtGenerationAuthorized','genericStyleSubstitutionAllowed','screenRouterGameplayBehaviorMutationAuthorized','saveEconomyGameplayMutationAuthorized','productionContentPublishAuthorized','playConsoleActionEnabled']
    for k in true_flags:
        if activation.get(k) is not True: failures.append(f'activation expected true: {k}')
    for k in false_flags:
        if activation.get(k) is not False: failures.append(f'activation expected false: {k}')

    builder=(candidate/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs').read_text()
    if 'Art/Production/CoreLoop/ReferenceMatch' not in builder: failures.append('builder missing production art root')
    if 'ApprovedScreens/' in builder: failures.append('builder directly references approved screenshot path')
    if 'Resources.Load<Sprite>("DesignScreens/' in builder: failures.append('builder loads DesignScreens')
    base_builder=(candidate/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs').read_text()
    if 'Step20CoreLoopReferenceMatchBuilder.BuildAll(false);' not in base_builder: failures.append('base builder does not reapply R2')

    new_cs=[
        candidate/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchBuilder.cs',
        candidate/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchSelfTest.cs',
        candidate/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionReferenceMatchPageMarker.cs',
        candidate/'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ReferenceMatchCoreLoopUiEditModeTests.cs',
    ]
    banned=['BuildPipeline.BuildPlayer','UnityWebRequest','HttpClient','PlayerPrefs.Set','Addressables.LoadAssetAsync','RuntimeEconomyTransactionEngine','GrantCurrency','GrantReward','SetBalance']
    for p in new_cs:
        text=p.read_text()
        if not check_no_newline_in_normal_csharp_string(text): failures.append('malformed C# string literal: '+p.name)
        for token in banned:
            if token in text: failures.append(f'banned token {token} in {p.name}')

    # Catch prior Step18 malformed literal regression across all C# sources.
    bad_csharp=[]
    for p in candidate.rglob('*.cs'):
        try: text=p.read_text(encoding='utf-8')
        except UnicodeDecodeError: continue
        if not check_no_newline_in_normal_csharp_string(text): bad_csharp.append(p.relative_to(candidate).as_posix())
    if bad_csharp: failures.append('C# normal-string newline regression: '+str(bad_csharp))

    # All JSON must parse.
    bad_json=[]
    for p in candidate.rglob('*.json'):
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: bad_json.append(f'{p.relative_to(candidate).as_posix()}: {e}')
    if bad_json: failures.append('invalid JSON: '+str(bad_json[:10]))

    manifest=json.loads((candidate/'Packages/manifest.json').read_text())
    if manifest.get('dependencies',{}).get('com.unity.addressables') != '2.8.1': failures.append('Addressables is not pinned to 2.8.1')
    save_registry=(candidate/'Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs').read_text()
    if 'CurrentPayloadVersion = 6' not in save_registry: failures.append('save-v6 pin missing')

    hotfix05a=(candidate/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs').read_text()
    hotfix05b=(candidate/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs').read_text()
    if 'JsonUtility.ToJson(BuildSnapshot(), true) + "\\n"' not in hotfix05a: failures.append('Compile Hotfix 05 preflight literal fix missing')
    if 'Contains("\\\"packageLockReviewed\\\": false")' not in hotfix05b: failures.append('Compile Hotfix 05 JSON escaping fix missing')

    tests=(candidate/'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ReferenceMatchCoreLoopUiEditModeTests.cs').read_text()
    test_declarations=tests.count('[Test]') + tests.count('[TestCase(')
    selftest=(candidate/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchSelfTest.cs').read_text()
    if 'ExpectedChecks = 85' not in selftest: failures.append('self-test expected count is not 85')

    report={
        'milestone': EXPECTED_MILESTONE,
        'status': 'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
        'candidate_files': len(cm),
        'baseline_files': len(bm),
        'changed_inherited_paths': changed,
        'added_path_count': len(added),
        'added_paths': added,
        'deleted_path_count': len(deleted),
        'inherited_png_count': len(baseline_png),
        'inherited_pngs_byte_identical': len(altered_png)==0,
        'production_reference_match_component_count': len(actual_components),
        'production_full_screen_screenshot_count': sum(1 for n in actual_components if png_size(candidate/ART_ROOT/n)==(941,1672)),
        'static_pixel_reference_match_pages': 3,
        'reference_match_test_attribute_count': test_declarations,
        'editor_self_test_expected_checks': 85,
        'packages_identical': not any(r.startswith('Packages/') for r in changed),
        'project_settings_identical': not any(r.startswith('ProjectSettings/') for r in changed),
        'inherited_tests_identical': len(altered_tests)==0,
        'inherited_runtime_code_identical': len(altered_runtime)==0,
        'addressables_version':'2.8.1',
        'save_payload_version':6,
        'unity_compile_test_evidence':'PENDING_USER_UNITY_EXECUTION',
        'final_reference_match_signoff_granted':False,
        'failures':failures,
    }
    out=Path(args.output) if args.output else candidate/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP03_3R2.json'
    out.write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,ensure_ascii=False,indent=2))
    return 0 if not failures else 1

if __name__=='__main__':
    raise SystemExit(main())
