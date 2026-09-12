#!/usr/bin/env python3
import argparse
import hashlib
import json
import os
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]

def sha256(path):
    h=hashlib.sha256()
    with open(path,'rb') as f:
        for chunk in iter(lambda:f.read(1024*1024),b''):
            h.update(chunk)
    return h.hexdigest()

def rel_files(root):
    out={}
    for p in root.rglob('*'):
        if p.is_file():
            out[p.relative_to(root).as_posix()]=p
    return out

def count_pngs(root):
    return sum(1 for p in (root/'Assets/LegendsOfRus').rglob('*.png'))

def read(rel):
    return (ROOT/rel).read_text(encoding='utf-8')

def parse_jsons(root):
    bad=[]
    for p in root.rglob('*.json'):
        try:
            json.loads(p.read_text(encoding='utf-8'))
        except Exception as exc:
            bad.append((p.relative_to(root).as_posix(),str(exc)))
    return bad

def brace_balance(path):
    s=path.read_text(encoding='utf-8')
    stack=[]
    i=0
    state='code'
    verbatim=False
    while i<len(s):
        c=s[i]
        n=s[i+1] if i+1<len(s) else ''
        if state=='code':
            if c=='/' and n=='/': state='line'; i+=2; continue
            if c=='/' and n=='*': state='block'; i+=2; continue
            if c=='@' and n=='"': state='string'; verbatim=True; i+=2; continue
            if c=='"': state='string'; verbatim=False; i+=1; continue
            if c=="'": state='char'; i+=1; continue
            if c in '{[(':
                stack.append(c)
            elif c in '}])':
                match={'}':'{',']':'[',')':'('}[c]
                if not stack or stack[-1]!=match: return False
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
    failures=[]
    checks={}

    def check(name, value, detail=''):
        checks[name]=bool(value)
        if not value:
            failures.append(name + (': '+detail if detail else ''))

    check('milestone_step06', read('MILESTONE_VERSION.txt').strip()==
          'v1.17-step06-privacy-terms-support-account-recovery-foundation')
    manifest=json.loads(read('Packages/manifest.json'))
    check('addressables_2_8_1', manifest.get('dependencies',{}).get('com.unity.addressables')=='2.8.1')
    runtime_asm=json.loads(read('Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef'))
    check('runtime_refs_compliance_core', 'LegendsOfRus.ComplianceCore' in runtime_asm.get('references',[]))
    core_asm=json.loads(read('Assets/LegendsOfRus/Scripts/ComplianceCore/LegendsOfRus.ComplianceCore.asmdef'))
    check('compliance_core_engine_free', core_asm.get('noEngineReferences') is True and not core_asm.get('references'))
    play_asm=json.loads(read('Assets/LegendsOfRus/Tests/PlayMode/LegendsOfRus.Runtime.PlayModeTests.asmdef'))
    check('step5_playmode_direct_content_core_ref', 'LegendsOfRus.ContentDeliveryCore' in play_asm.get('references',[]))

    cfg=json.loads(read('Assets/LegendsOfRus/Resources/Config/privacy_support_activation.json'))
    check('local_compliance_persistence_approved', cfg.get('localCompliancePersistenceApproved') is True)
    check('local_support_export_approved', cfg.get('localSupportExportApproved') is True)
    for key in ['finalLegalDocumentsApproved','legalAcceptanceUiApproved','supportUiApproved',
                'accountRecoveryUiApproved','accountRecoveryBackendApproved','dataDeletionBackendApproved',
                'productionAccountServicesApproved']:
        check('blocked_'+key, cfg.get(key) is False)

    ps=read('Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportService.cs')
    check('support_allowlist_dto', 'sealed class SupportExportDto' in ps and 'TryBuildSupportExport' in ps)
    check('no_device_unique_identifier', 'deviceUniqueIdentifier' not in ps)
    check('no_advertising_identifier', 'advertisingIdentifier' not in ps)
    store=read('Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerPrefsComplianceStateStore.cs')
    check('recovery_not_in_store', all(x not in store for x in ['candidateCode','recoveryCode','password','email']))
    recovery=read('Assets/LegendsOfRus/Scripts/ComplianceCore/AccountRecoveryPolicy.cs')
    check('recovery_persistence_false', 'CandidatePersistenceAllowed = false' in recovery)
    check('recovery_backend_compiled_false', 'RecoveryBackendCompiled = false' in recovery)
    check('deletion_backend_compiled_false', 'AccountDeletionBackendCompiled = false' in recovery)

    wrapper=read('Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacyAwareAnalytics.cs')
    check('analytics_consent_gate', 'PrivacySupportService.Instance.MayUseAnalytics' in wrapper and
          'AnalyticsReasonCodes.CaptureNotAllowed' in wrapper)
    prod_calls=[
        'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs',
        'Assets/LegendsOfRus/Scripts/Runtime/UI/LevelTutorialPresenter.cs',
        'Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs',
        'Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs']
    check('no_direct_runtime_analytics_calls', all('AnalyticsService.Try' not in read(x) for x in prod_calls))
    monet=read('Assets/LegendsOfRus/Scripts/Runtime/Services/MonetizationService.cs')
    check('rewarded_ads_consent_gate', 'PrivacySupportService.Instance.MayUseAds' in monet and
          'MONETIZATION_AD_CONSENT_REQUIRED' in monet)

    check('save_schema_v6_retained', 'CurrentPayloadVersion = 6' in
          read('Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs') and
          'public int saveVersion = 6;' in read('Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs'))
    check('step4_nunit_hotfix_retained', 'Array.IndexOf(parameterTypes, typeof(string))' in
          read('Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs'))
    check('old_nunit_type_constraint_absent', 'Does.Not.Contain(typeof(' not in ''.join(
        p.read_text(encoding='utf-8',errors='ignore') for p in (ROOT/'Assets/LegendsOfRus/Tests').rglob('*.cs')))
    check('approved_png_count_65', count_pngs(ROOT)==65, str(count_pngs(ROOT)))
    check('all_json_valid', not parse_jsons(ROOT), str(parse_jsons(ROOT)[:3]))

    new_cs=list((ROOT/'Assets/LegendsOfRus/Scripts/ComplianceCore').glob('*.cs'))+[
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportActivation.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerPrefsComplianceStateStore.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacySupportService.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/PrivacyAwareAnalytics.cs',
        ROOT/'Assets/LegendsOfRus/Scripts/Editor/Step17PrivacyTermsSupportRecoveryFoundationSelfTest.cs',
        ROOT/'Assets/LegendsOfRus/Tests/ComplianceEditMode/ComplianceFoundationEditModeTests.cs',
        ROOT/'Assets/LegendsOfRus/Tests/CompliancePlayMode/PrivacySupportFoundationPlayModeTests.cs']
    check('new_csharp_delimiters_balanced', all(brace_balance(p) for p in new_cs),
          ','.join(p.name for p in new_cs if not brace_balance(p)))

    protected={}
    changed=[]
    added=[]
    deleted=[]
    if args.baseline:
        base=args.baseline.resolve()
        base_files=rel_files(base)
        cur_files=rel_files(ROOT)
        for rel,bp in base_files.items():
            cp=cur_files.get(rel)
            if cp is None:
                deleted.append(rel)
            elif sha256(bp)!=sha256(cp):
                changed.append(rel)
        added=sorted(set(cur_files)-set(base_files))
        changed.sort(); deleted.sort()
        # Protected baseline inventories.
        pngs=[rel for rel in base_files if rel.lower().endswith('.png') and rel.startswith('Assets/LegendsOfRus/')]
        protected['png_count']=len(pngs)
        protected['png_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in pngs)
        check('baseline_pngs_identical', protected['png_identical'])
        ps_files=[rel for rel in base_files if rel.startswith('ProjectSettings/')]
        protected['project_settings_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in ps_files)
        check('project_settings_identical', protected['project_settings_identical'])
        save_files=[rel for rel in base_files if rel.startswith('Assets/LegendsOfRus/Scripts/SaveCore/') or
                    rel in ['Assets/LegendsOfRus/Scripts/Runtime/Services/SaveService.cs',
                            'Assets/LegendsOfRus/Scripts/Runtime/Services/PlayerSavePayloadCodec.cs',
                            'Assets/LegendsOfRus/Scripts/Runtime/Core/PlayerSaveMigrationRegistry.cs',
                            'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs']]
        protected['save_boundary_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in save_files)
        check('save_boundary_identical', protected['save_boundary_identical'])
        inherited_tests=[rel for rel in base_files if rel.startswith('Assets/LegendsOfRus/Tests/')]
        protected['inherited_tests_identical']=all(rel in cur_files and sha256(base_files[rel])==sha256(cur_files[rel]) for rel in inherited_tests)
        check('inherited_tests_identical', protected['inherited_tests_identical'])
        check('no_baseline_files_deleted', not deleted, ','.join(deleted[:10]))

    report={
        'version':'v1.17-step06',
        'milestone':'v1.17-step06-privacy-terms-support-account-recovery-foundation',
        'status':'PASS_STATIC_SOURCE_VERIFICATION' if not failures else 'FAIL_STATIC_SOURCE_VERIFICATION',
        'passed':not failures,
        'unityVersion':'6000.5.9f1',
        'addressablesPackage':'com.unity.addressables@2.8.1',
        'unityExecutionEvidence':'NOT_RETAINED_FOR_STEP06_BYTES',
        'unityCompileStatus':'PENDING_USER_EXECUTION',
        'editorSelfTestExpected':'20/20',
        'finalLegalDocumentsApproved':False,
        'productionAccountTransportCompiled':False,
        'productionBuildAuthorized':False,
        'checks':checks,
        'protectedBaseline':protected,
        'changedPathsVsBaseline':changed,
        'addedPathsVsBaseline':added,
        'deletedPathsVsBaseline':deleted,
        'failures':failures
    }
    out=args.report or ROOT/'Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP06.json'
    out.parent.mkdir(parents=True,exist_ok=True)
    out.write_text(json.dumps(report,indent=2,ensure_ascii=False)+'\n',encoding='utf-8')
    print(json.dumps({'passed':not failures,'failures':failures,'changed':len(changed),'added':len(added),'deleted':len(deleted)},indent=2))
    return 0 if not failures else 1

if __name__=='__main__':
    raise SystemExit(main())
