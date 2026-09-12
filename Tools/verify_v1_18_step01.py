#!/usr/bin/env python3
import argparse, hashlib, json, re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def read(rel):
    return (ROOT/rel).read_text(encoding='utf-8', errors='strict')

def sha256(path):
    h=hashlib.sha256()
    with path.open('rb') as f:
        for chunk in iter(lambda:f.read(1024*1024), b''):
            h.update(chunk)
    return h.hexdigest()

def rel_files(root):
    skip={'Library','Temp','Logs','obj','Build','Builds','.git'}
    out={}
    for p in root.rglob('*'):
        if not p.is_file(): continue
        rel=p.relative_to(root).as_posix()
        if rel.split('/')[0] in skip: continue
        out[rel]=p
    return out

def parse_jsons(root):
    failures=[]
    for p in root.rglob('*.json'):
        if any(x in p.parts for x in ('Library','Temp','Logs')): continue
        try: json.loads(p.read_text(encoding='utf-8'))
        except Exception as e: failures.append(f'{p.relative_to(root)}: {e}')
    return failures

def count_pngs(root):
    return len(list((root/'Assets/LegendsOfRus').rglob('*.png')))

def brace_balance(path):
    text=path.read_text(encoding='utf-8',errors='ignore')
    stack=[]; i=0; state='code'; verbatim=False
    pairs={'}':'{',']':'[',')':'('}
    while i<len(text):
        c=text[i]; n=text[i+1] if i+1<len(text) else ''
        if state=='code':
            if c=='/' and n=='/': state='line'; i+=2; continue
            if c=='/' and n=='*': state='block'; i+=2; continue
            if c=='@' and n=='"': state='string'; verbatim=True; i+=2; continue
            if c=='"': state='string'; verbatim=False; i+=1; continue
            if c=="'": state='char'; i+=1; continue
            if c in '([{': stack.append(c)
            elif c in ')]}':
                if not stack or stack[-1]!=pairs[c]: return False
                stack.pop()
            i+=1; continue
        if state=='line':
            if c=='\n': state='code'
            i+=1; continue
        if state=='block':
            if c=='*' and n=='/': state='code'; i+=2; continue
            i+=1; continue
        if state=='string':
            if verbatim:
                if c=='"' and n=='"': i+=2; continue
                if c=='"': state='code'; i+=1; continue
                i+=1; continue
            if c=='\\': i+=2; continue
            if c=='"': state='code'
            i+=1; continue
        if state=='char':
            if c=='\\': i+=2; continue
            if c=="'": state='code'
            i+=1; continue
    return not stack and state in ('code','line')

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--baseline', type=Path)
    ap.add_argument('--report', type=Path)
    args=ap.parse_args()
    failures=[]; checks={}
    def check(name,value,detail=''):
        checks[name]=bool(value)
        if not value: failures.append(name + (': '+detail if detail else ''))

    check('milestone_step01', read('MILESTONE_VERSION.txt').strip()==
          'v1.18-step01-device-matrix-closed-test-preflight-foundation')
    check('unity_version_pin', 'm_EditorVersion: 6000.5.9f1' in read('ProjectSettings/ProjectVersion.txt'))
    manifest=json.loads(read('Packages/manifest.json'))
    check('addressables_2_8_1_retained', manifest.get('dependencies',{}).get('com.unity.addressables')=='2.8.1')

    runtime_asm=json.loads(read('Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef'))
    check('runtime_refs_qa_core', 'LegendsOfRus.QAReadinessCore' in runtime_asm.get('references',[]))
    qa_asm=json.loads(read('Assets/LegendsOfRus/Scripts/QAReadinessCore/LegendsOfRus.QAReadinessCore.asmdef'))
    check('qa_core_engine_free', qa_asm.get('noEngineReferences') is True and not qa_asm.get('references'))
    check('qa_service_bootstrap_wired', 'FindOrAdd<ClosedTestQaService>(root);' in
          read('Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs'))

    cfg=json.loads(read('Assets/LegendsOfRus/Resources/Config/closed_test_qa_activation.json'))
    check('local_device_capture_approved', cfg.get('localDeviceProfileCaptureApproved') is True)
    check('local_observation_buffer_approved', cfg.get('localObservationBufferApproved') is True)
    for key in ['remoteQaUploadApproved','productionQaProviderApproved','privacyDataSafetyApproved',
                'storeListingAssetsApproved','productionSigningApproved','controlledTestBuildAuthorized',
                'storePublishingAuthorized']:
        check('blocked_'+key, cfg.get(key) is False)

    act=read('Assets/LegendsOfRus/Scripts/Runtime/Services/ClosedTestQaActivation.cs')
    for token in ['RemoteQaTransportCompiled = false','ProductionQaProviderCompiled = false',
                  'ProductionSigningCompiled = false','StorePublishingCompiled = false']:
        check('compiled_gate_'+re.sub(r'\W+','_',token).strip('_'), token in act)

    service=read('Assets/LegendsOfRus/Scripts/Runtime/Services/ClosedTestQaService.cs')
    core_sources='\n'.join(p.read_text(encoding='utf-8',errors='ignore') for p in
                           (ROOT/'Assets/LegendsOfRus/Scripts/QAReadinessCore').glob('*.cs'))
    new_qa_source=service+'\n'+core_sources
    for token in ['deviceUniqueIdentifier','advertisingIdentifier','SystemInfo.deviceModel','SystemInfo.deviceName']:
        check('forbidden_identifier_absent_'+re.sub(r'\W+','_',token), token not in new_qa_source)
    check('qa_service_no_persistence_api', all(x not in service for x in ['PlayerPrefs','File.Write','FileStream','HttpClient','UnityWebRequest']))
    obs=read('Assets/LegendsOfRus/Scripts/QAReadinessCore/ClosedTestObservationBuffer.cs')
    check('observation_buffer_capacity_64', 'DefaultCapacity = 64' in obs)
    check('observation_buffer_max_256', 'capacity > 256' in obs)

    matrix=json.loads(read('Assets/LegendsOfRus/Documentation/V1_18_STEP01_DEVICE_MATRIX.json'))
    check('device_matrix_count_5', matrix.get('requiredProfileCount')==5 and len(matrix.get('profiles',[]))==5)
    aspect=set(x.get('expectedAspectBucket') for x in matrix.get('profiles',[]))
    capability=set(x.get('expectedCapabilityBand') for x in matrix.get('profiles',[]))
    check('device_matrix_aspect_coverage', {'Standard16By9','Tall18By9','Tall19_5By9','Tall20By9'} <= aspect)
    check('device_matrix_capability_coverage', {'Low','Mid','High'} <= capability)
    check('physical_device_evidence_not_faked', matrix.get('physicalDevicePassCount')==0 and
          all(x.get('physicalDeviceEvidence')=='NOT_RETAINED' for x in matrix.get('profiles',[])))

    preflight=read('Assets/LegendsOfRus/Scripts/QAReadinessCore/ClosedTestPreflight.cs')
    for code in ['QA_UNITY_COMPILE_EVIDENCE_MISSING','QA_DEVICE_MATRIX_INCOMPLETE',
                 'QA_PRODUCTION_SIGNING_NOT_APPROVED','QA_CONTROLLED_TEST_BUILD_NOT_AUTHORIZED',
                 'QA_PUBLISHING_NOT_AUTHORIZED']:
        check('preflight_code_'+code.lower(), code in preflight)

    check('save_schema_v6_retained', 'CurrentPayloadVersion = 6' in
          read('Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs') and
          'public int saveVersion = 6;' in read('Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs'))
    check('step4_nunit_hotfix_retained', 'Array.IndexOf(parameterTypes, typeof(string))' in
          read('Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'))
    play_asm=json.loads(read('Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    check('step5_playmode_content_core_ref_retained', 'LegendsOfRus.ContentDeliveryCore' in play_asm.get('references',[]))
    compliance=json.loads(read('Assets/LegendsOfRus/Resources/Config/privacy_support_activation.json'))
    check('final_legal_still_unapproved', compliance.get('finalLegalDocumentsApproved') is False)
    content=json.loads(read('Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json'))
    check('remote_content_still_blocked', all(content.get(k) is False for k in
          ['remoteCatalogActivationApproved','remoteContentActivationApproved','catalogUpdatesActivationApproved']))
    economy=json.loads(read('Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json'))
    commercial=json.loads(read('Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json'))
    check('economy_production_still_blocked', economy.get('runtimeActivationApproved') is False)
    check('commercial_production_still_blocked', all(commercial.get(k) is False for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']))
    check('approved_png_count_65', count_pngs(ROOT)==65, str(count_pngs(ROOT)))
    json_fail=parse_jsons(ROOT)
    check('all_json_valid', not json_fail, str(json_fail[:3]))

    new_cs=list((ROOT/'Assets/LegendsOfRus/Scripts/QAReadinessCore').glob('*.cs'))+[
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/ClosedTestQaActivation.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/ClosedTestQaService.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Editor/Step18DeviceMatrixClosedTestPreflightSelfTest.cs',
        ROOT/'Assets/LegendsOfRus/Tests/QAReadinessEditMode/DeviceMatrixClosedTestPreflightEditModeTests.cs',
        ROOT/'Assets/LegendsOfRus/Tests/QAReadinessPlayMode/ClosedTestQaFoundationPlayModeTests.cs']
    check('new_csharp_delimiters_balanced', all(brace_balance(p) for p in new_cs),
          ','.join(p.name for p in new_cs if not brace_balance(p)))
    selftest=read('Assets/LegendsOfRus/Scripts/Editor/Step18DeviceMatrixClosedTestPreflightSelfTest.cs')
    self_count=len(re.findall(r'\bCheck\("',selftest))
    check('self_test_check_count_22', self_count==22, str(self_count))
    new_tests='\n'.join(p.read_text(encoding='utf-8',errors='ignore') for d in
        ['Assets/LegendsOfRus/Tests/QAReadinessEditMode','Assets/LegendsOfRus/Tests/QAReadinessPlayMode']
        for p in (ROOT/d).glob('*.cs'))
    test_decls=len(re.findall(r'\[(?:Test|TestCase|UnityTest)\b',new_tests))
    check('new_test_declarations_at_least_18', test_decls>=18, str(test_decls))

    protected={}; changed=[]; added=[]; deleted=[]
    if args.baseline:
        base=args.baseline.resolve(); base_files=rel_files(base); cur_files=rel_files(ROOT)
        for rel,bp in base_files.items():
            cp=cur_files.get(rel)
            if cp is None: deleted.append(rel)
            elif sha256(bp)!=sha256(cp): changed.append(rel)
        added=sorted(set(cur_files)-set(base_files)); changed.sort(); deleted.sort()
        pngs=[rel for rel in base_files if rel.lower().endswith('.png') and rel.startswith('Assets/LegendsOfRus/')]
        protected['png_count']=len(pngs)
        protected['png_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in pngs)
        check('baseline_pngs_identical', protected['png_identical'])
        ps=[rel for rel in base_files if rel.startswith('ProjectSettings/')]
        protected['project_settings_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in ps)
        check('project_settings_identical', protected['project_settings_identical'])
        pk=[rel for rel in base_files if rel.startswith('Packages/')]
        protected['packages_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in pk)
        check('packages_identical', protected['packages_identical'])
        save=[rel for rel in base_files if rel.startswith('Assets/LegendsOfRus/Scripts/SaveCore/') or rel in [
            'Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs',
            'Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerSavePayloadCodec.cs',
            'Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs',
            'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs']]
        protected['save_boundary_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in save)
        check('save_boundary_identical', protected['save_boundary_identical'])
        inherited_tests=[rel for rel in base_files if rel.startswith('Assets/LegendsOfRus/Tests/')]
        protected['inherited_tests_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in inherited_tests)
        check('inherited_tests_identical', protected['inherited_tests_identical'])
        compliance_boundary=[rel for rel in base_files if rel.startswith('Assets/LegendsOfRus/Scripts/ComplianceCore/') or rel in [
            'Assets/LegendsOfRus/Resources/Config/privacy_support_activation.json',
            'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportActivation.cs',
            'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportService.cs',
            'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacyAwareAnalytics.cs']]
        protected['compliance_boundary_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in compliance_boundary)
        check('compliance_boundary_identical', protected['compliance_boundary_identical'])
        check('no_baseline_files_deleted', not deleted, ','.join(deleted[:10]))
        allowed_changed={
            'MILESTONE_VERSION.txt','README.md',
            'Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json',
            'Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md',
            'Assets/LegendsOfRus/Documentation/README_FIRST.md',
            'Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef',
            'Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs'}
        unexpected=[x for x in changed if x not in allowed_changed]
        check('inherited_changes_allowlisted', not unexpected, ','.join(unexpected[:10]))

    report={
      'version':'v1.18-step01',
      'milestone':'v1.18-step01-device-matrix-closed-test-preflight-foundation',
      'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
      'passed':not failures,
      'unityVersion':'6000.5.9f1',
      'addressablesPackage':'com.unity.addressables@2.8.1',
      'unityExecutionEvidence':'NOT_RETAINED_FOR_STEP01_BYTES',
      'unityCompileStatus':'PENDING_USER_EXECUTION',
      'editorSelfTestExpected':'22/22',
      'newTestDeclarationsInformational':test_decls,
      'deviceMatrixProfiles':5,
      'physicalDeviceEvidenceRetained':False,
      'remoteQaTransportCompiled':False,
      'productionSigningCompiled':False,
      'controlledTestBuildAuthorized':False,
      'publishingAuthorized':False,
      'checks':checks,
      'protectedBaseline':protected,
      'changedPathsVsBaseline':changed,
      'addedPathsVsBaseline':added,
      'deletedPathsVsBaseline':deleted,
      'failures':failures}
    out=args.report or ROOT/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_18_STEP01.json'
    out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(report,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    print(json.dumps({'passed':not failures,'failures':failures,'changed':len(changed),'added':len(added),'deleted':len(deleted),'selfTests':self_count,'newTestDeclarations':test_decls},indent=2))
    return 0 if not failures else 1

if __name__=='__main__':
    raise SystemExit(main())
