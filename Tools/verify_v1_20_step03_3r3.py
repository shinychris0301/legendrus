#!/usr/bin/env python3
from __future__ import annotations
import argparse, hashlib, json, struct
from pathlib import Path

ALLOW_CHANGED={
    'README.md',
    'MILESTONE_VERSION.txt',
    'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
    'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs',
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs',
}
EXPECTED_MILESTONE='v1.20-step03.3R3-prelevel-gameplay-pause-visual-rebuild'
ART_ROOT='Assets/LegendsOfRus/Art/Production/CoreLoop/ReferenceMatchR3'
EXPECTED_REFS={
    'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/PreLevel.png':'32e05c72c56f403ea065a327aea0ee9bdef33b8bb22330807baff117c0b40195',
    'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Gameplay.png':'d8e73bb76006bd0b4c6428196657ddce47193d3056362562a68d571b74c1d800',
    'Assets/LegendsOfRus/Resources/DesignScreens/Pause.png':'31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716',
}
EXPECTED_COMPONENTS={
'prelevel_top_hud.png':(941,145),'prelevel_chapter_band.png':(941,260),
'prelevel_goals_left_scene.png':(190,445),'prelevel_goals_panel.png':(561,445),'prelevel_goals_right_scene.png':(190,445),
'prelevel_boosters_left_scene.png':(105,285),'prelevel_boosters_panel.png':(731,285),'prelevel_boosters_right_scene.png':(105,285),
'prelevel_reward_left_scene.png':(95,300),'prelevel_reward_panel.png':(751,300),'prelevel_reward_right_scene.png':(95,300),
'prelevel_bottom_controls.png':(941,237),
'gameplay_top_shell.png':(941,357),'gameplay_left_environment.png':(79,942),'gameplay_right_environment.png':(76,942),
'gameplay_booster_shell.png':(941,205),'gameplay_score_footer.png':(941,168),
'pause_header.png':(640,150),'pause_left_border.png':(30,925),'pause_goals_panel.png':(580,270),
'pause_boosters_panel.png':(580,200),'pause_buttons_panel.png':(580,455),'pause_right_border.png':(30,925),'pause_footer.png':(640,50),
}

def sha(p:Path)->str: return hashlib.sha256(p.read_bytes()).hexdigest()
def files(root:Path): return {p.relative_to(root).as_posix():p for p in root.rglob('*') if p.is_file()}
def png_size(p:Path):
    data=p.read_bytes()[:24]
    if len(data)<24 or data[:8]!=b'\x89PNG\r\n\x1a\n': raise ValueError('not PNG')
    return struct.unpack('>II',data[16:24])

def csharp_string_ok(text:str)->bool:
    NORMAL,STRING,CHAR,LINE,BLOCK,VERBATIM=range(6); state=NORMAL; i=0
    while i<len(text):
        c=text[i]; n=text[i+1] if i+1<len(text) else ''
        if state==NORMAL:
            if c=='/' and n=='/': state=LINE; i+=1
            elif c=='/' and n=='*': state=BLOCK; i+=1
            elif c=='@' and n=='"': state=VERBATIM; i+=1
            elif c=='"': state=STRING
            elif c=="'": state=CHAR
        elif state==STRING:
            if c=='\\': i+=1
            elif c=='"': state=NORMAL
            elif c in '\r\n': return False
        elif state==CHAR:
            if c=='\\': i+=1
            elif c=="'": state=NORMAL
            elif c in '\r\n': return False
        elif state==VERBATIM:
            if c=='"' and n=='"': i+=1
            elif c=='"': state=NORMAL
        elif state==LINE:
            if c=='\n': state=NORMAL
        elif state==BLOCK:
            if c=='*' and n=='/': state=NORMAL; i+=1
        i+=1
    return state not in (STRING,CHAR)

def main():
    ap=argparse.ArgumentParser(); ap.add_argument('--candidate',default=str(Path(__file__).resolve().parents[1])); ap.add_argument('--baseline',required=True); ap.add_argument('--output',default=''); a=ap.parse_args()
    c=Path(a.candidate).resolve(); b=Path(a.baseline).resolve(); failures=[]
    cm=files(c); bm=files(b)
    added=sorted(set(cm)-set(bm)); deleted=sorted(set(bm)-set(cm)); changed=[]
    for rel in sorted(set(cm)&set(bm)):
        if sha(cm[rel])!=sha(bm[rel]): changed.append(rel)
    if deleted: failures.append('deleted baseline files: '+str(deleted))
    unexpected=[x for x in changed if x not in ALLOW_CHANGED]
    if unexpected: failures.append('unexpected inherited changes: '+str(unexpected))
    missing=[x for x in ALLOW_CHANGED if x not in changed]
    if missing: failures.append('expected allowlisted changes missing: '+str(missing))

    inherited_png=[r for r in bm if r.lower().endswith('.png')]
    altered_png=[r for r in inherited_png if r not in cm or sha(bm[r])!=sha(cm[r])]
    if altered_png: failures.append('inherited PNG changed: '+str(altered_png))
    for prefix in ('Packages/','ProjectSettings/'):
        altered=[r for r in bm if r.startswith(prefix) and (r not in cm or sha(bm[r])!=sha(cm[r]))]
        if altered: failures.append('protected '+prefix+' changed: '+str(altered))
    inherited_tests=[r for r in bm if r.startswith('Assets/LegendsOfRus/Tests/')]
    altered_tests=[r for r in inherited_tests if r not in cm or sha(bm[r])!=sha(cm[r])]
    if altered_tests: failures.append('inherited tests changed: '+str(altered_tests))
    inherited_runtime=[r for r in bm if r.startswith('Assets/LegendsOfRus/Scripts/Runtime/')]
    altered_runtime=[r for r in inherited_runtime if r not in cm or sha(bm[r])!=sha(cm[r])]
    if altered_runtime != ['Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs']:
        failures.append('unexpected inherited runtime changes: '+str(altered_runtime))

    for rel in ['Assets/LegendsOfRus/Resources/Config/chapter01_levels.json','Assets/LegendsOfRus/Resources/Config/chapter01_story.json','Assets/LegendsOfRus/Resources/Localization/story_ru.json']:
        if rel in bm and sha(bm[rel])!=sha(cm[rel]): failures.append(rel+' changed')
    if (c/'MILESTONE_VERSION.txt').read_text().strip()!=EXPECTED_MILESTONE: failures.append('milestone mismatch')
    for rel,h in EXPECTED_REFS.items():
        p=c/rel
        if not p.is_file() or sha(p)!=h: failures.append('reference hash mismatch: '+rel)

    actual=sorted(p.name for p in (c/ART_ROOT).glob('*.png'))
    if actual!=sorted(EXPECTED_COMPONENTS): failures.append('R3 component inventory mismatch')
    for name,size in EXPECTED_COMPONENTS.items():
        p=c/ART_ROOT/name
        if not p.is_file(): failures.append('missing component: '+name); continue
        s=png_size(p)
        if s!=size: failures.append(f'{name} dimensions {s} != {size}')
        if s==(941,1672): failures.append('full-screen production screenshot forbidden: '+name)

    proof=json.loads((c/'ProjectDocs/UIReferenceMatch/v1_20_step03_3r3_pixel_proof.json').read_text())
    pre=proof.get('PreLevel',{}); gp=proof.get('Gameplay',{}); pa=proof.get('Pause',{})
    if not pre.get('exactPixelReconstruction') or pre.get('differentPixels')!=0 or pre.get('maxChannelError')!=0: failures.append('PreLevel pixel proof failed')
    if not gp.get('exactShellPixelReconstruction') or gp.get('differentPixelsWithinScope')!=0 or gp.get('maxChannelErrorWithinScope')!=0: failures.append('Gameplay shell pixel proof failed')
    if not pa.get('exactModalPixelReconstruction') or pa.get('differentPixelsWithinScope')!=0 or pa.get('maxChannelErrorWithinScope')!=0: failures.append('Pause modal pixel proof failed')
    if gp.get('dynamicBoardPixelCount',0)<=0: failures.append('Gameplay proof does not preserve dynamic board scope')
    if pa.get('backgroundMode')!='live_gameplay_plus_runtime_dim_overlay': failures.append('Pause proof does not preserve live gameplay background')

    activation=json.loads((c/'Assets/LegendsOfRus/Resources/Config/production_ui_reference_match_v120_step03_3r3.json').read_text())
    true_flags=['referenceMatchRebuildAuthorized','referenceDerivedProductionComponentsAuthorized','preLevelReferenceMatchCutoverAuthorized','gameplayShellReferenceMatchCutoverAuthorized','pauseModalReferenceMatchCutoverAuthorized','liveGameplayBoardPreservationRequired','liveGameplayPauseBackdropRequired','staticComponentPixelReconstructionVerified','manualUnityRenderReviewRequired']
    false_flags=['finalReferenceMatchSignoffGranted','fullScreenScreenshotRuntimeUseAuthorized','screenshotDeletionAuthorized','lockedReferenceOverwriteAuthorized','automaticArtGenerationAuthorized','genericStyleSubstitutionAllowed','gameplayBoardReplacementAuthorized','gameplayRulesMutationAuthorized','saveEconomyRewardMutationAuthorized','productionContentPublishAuthorized','playConsoleActionEnabled']
    for k in true_flags:
        if activation.get(k) is not True: failures.append('activation expected true: '+k)
    for k in false_flags:
        if activation.get(k) is not False: failures.append('activation expected false: '+k)

    builder=(c/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3Builder.cs').read_text()
    if 'ReferenceMatchR3' not in builder: failures.append('R3 builder art root missing')
    if 'ApprovedScreens/' in builder or 'Resources.Load<Sprite>("DesignScreens/' in builder: failures.append('R3 builder directly loads QA capture')
    base=(c/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopPageBuilder.cs').read_text()
    if 'Step20CoreLoopReferenceMatchBuilder.BuildAll(false);' not in base or 'Step20CoreLoopReferenceMatchR3Builder.BuildAll(false);' not in base: failures.append('base builder does not reapply R2 + R3')
    router=(c/'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs').read_text()
    for needle in [
        'Hotspot(parent, "Boosters", 110, 850, 720, 285',
        'Hotspot(parent, "StartLevel", 205, 1435, 535, 190',
        'Hotspot(parent, "Resume", 235, 910, 470, 105',
        'Hotspot(parent, "Restart", 235, 1025, 470, 105',
        'Hotspot(parent, "Settings", 235, 1140, 470, 105',
        'Hotspot(parent, "Exit", 235, 1255, 470, 105',
        'SetPixelRect(rt, 79, 357, 786, 942);']:
        if needle not in router: failures.append('runtime geometry pin missing: '+needle)

    bad=[]
    for p in c.rglob('*.cs'):
        try: text=p.read_text(encoding='utf-8')
        except UnicodeDecodeError: continue
        if not csharp_string_ok(text): bad.append(p.relative_to(c).as_posix())
    if bad: failures.append('C# normal-string newline regression: '+str(bad))
    bad_json=[]
    for p in c.rglob('*.json'):
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: bad_json.append(f'{p.relative_to(c).as_posix()}: {e}')
    if bad_json: failures.append('invalid JSON: '+str(bad_json[:10]))

    manifest=json.loads((c/'Packages/manifest.json').read_text())
    if manifest.get('dependencies',{}).get('com.unity.addressables')!='2.8.1': failures.append('Addressables is not 2.8.1')
    if 'CurrentPayloadVersion = 6' not in (c/'Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs').read_text(): failures.append('save-v6 pin missing')
    if 'JsonUtility.ToJson(BuildSnapshot(), true) + "\\n"' not in (c/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildPreflightRunner.cs').read_text(): failures.append('Compile Hotfix 05 preflight fix missing')
    if 'Contains("\\\"packageLockReviewed\\\": false")' not in (c/'Assets/LegendsOfRus/Scripts/Editor/Step18ControlledTestBuildReleaseCandidateSelfTest.cs').read_text(): failures.append('Compile Hotfix 05 JSON escaping fix missing')

    tests=(c/'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode/ReferenceMatchR3CoreLoopUiEditModeTests.cs').read_text()
    test_attributes=tests.count('[Test]')+tests.count('[TestCase(')
    selftest=(c/'Assets/LegendsOfRus/Scripts/Editor/Step20CoreLoopReferenceMatchR3SelfTest.cs').read_text()
    if 'ExpectedChecks = 137' not in selftest: failures.append('R3 self-test count is not 137')

    report={
        'milestone':EXPECTED_MILESTONE,
        'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
        'candidate_files':len(cm),'baseline_files':len(bm),'changed_inherited_paths':changed,
        'added_path_count':len(added),'added_paths':added,'deleted_path_count':len(deleted),
        'inherited_png_count':len(inherited_png),'inherited_pngs_byte_identical':len(altered_png)==0,
        'r3_component_count':len(actual),'production_full_screen_screenshot_count':sum(1 for n in actual if png_size(c/ART_ROOT/n)==(941,1672)),
        'prelevel_full_reference_static_match':pre.get('exactPixelReconstruction') is True,
        'gameplay_fixed_shell_static_match':gp.get('exactShellPixelReconstruction') is True,
        'pause_modal_static_match':pa.get('exactModalPixelReconstruction') is True,
        'gameplay_live_board_preserved':True,'pause_live_gameplay_background_preserved':True,
        'r3_test_attribute_count':test_attributes,'editor_self_test_expected_checks':137,
        'packages_identical':not any(x.startswith('Packages/') for x in changed),
        'project_settings_identical':not any(x.startswith('ProjectSettings/') for x in changed),
        'inherited_tests_identical':len(altered_tests)==0,
        'allowed_runtime_geometry_change_only':altered_runtime==['Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs'],
        'addressables_version':'2.8.1','save_payload_version':6,
        'unity_compile_test_evidence':'PENDING_USER_UNITY_EXECUTION',
        'final_reference_match_signoff_granted':False,
        'failures':failures,
    }
    out=Path(a.output) if a.output else c/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_20_STEP03_3R3.json'
    out.write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n',encoding='utf-8')
    print(json.dumps(report,ensure_ascii=False,indent=2))
    return 0 if not failures else 1
if __name__=='__main__': raise SystemExit(main())
