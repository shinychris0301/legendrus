#!/usr/bin/env python3
import argparse, hashlib, json, pathlib, sys

def sha(path):
    h=hashlib.sha256()
    with path.open('rb') as f:
        for chunk in iter(lambda:f.read(1024*1024), b''): h.update(chunk)
    return h.hexdigest()

def files(root, pattern='*'):
    return [p for p in root.rglob(pattern) if p.is_file()]

def main():
    ap=argparse.ArgumentParser()
    ap.add_argument('--project', required=True)
    ap.add_argument('--baseline', required=True)
    ap.add_argument('--output')
    a=ap.parse_args()
    root=pathlib.Path(a.project).resolve(); base=pathlib.Path(a.baseline).resolve()
    failures=[]
    # JSON parse
    json_count=0
    for p in files(root,'*.json'):
        json_count+=1
        try: json.loads(p.read_text())
        except Exception as e: failures.append(f'invalid json {p.relative_to(root)}: {e}')
    # protected approved art
    pngs=[p for p in files(base,'*.png') if 'Assets/LegendsOfRus/' in p.as_posix()]
    for p in pngs:
        rel=p.relative_to(base); q=root/rel
        if not q.exists() or sha(p)!=sha(q): failures.append(f'approved png changed: {rel}')
    # ProjectSettings unchanged
    for p in files(base/'ProjectSettings'):
        rel=p.relative_to(base); q=root/rel
        if not q.exists() or sha(p)!=sha(q): failures.append(f'ProjectSettings changed: {rel}')
    # Existing Resources unchanged; Step 5 may only add two config files.
    allowed_new={
      pathlib.Path('Assets/LegendsOfRus/Resources/Config/content_delivery_manifest.json'),
      pathlib.Path('Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json')
    }
    for p in files(base/'Assets/LegendsOfRus/Resources'):
        rel=p.relative_to(base); q=root/rel
        if not q.exists() or sha(p)!=sha(q): failures.append(f'inherited Resources changed: {rel}')
    for q in files(root/'Assets/LegendsOfRus/Resources'):
        rel=q.relative_to(root)
        if not (base/rel).exists() and rel not in allowed_new:
            failures.append(f'unexpected new Resources file: {rel}')
    manifest=json.loads((root/'Packages/manifest.json').read_text())
    if manifest.get('dependencies',{}).get('com.unity.addressables')!='2.8.1':
        failures.append('Addressables package not pinned to 2.8.1')
    cd=json.loads((root/'Assets/LegendsOfRus/Resources/Config/content_delivery_manifest.json').read_text())
    entries=cd.get('entries',[])
    if len(entries)!=31: failures.append(f'content manifest count {len(entries)} != 31')
    ready=[e for e in entries if e.get('addressableReady') is True]
    if [e.get('address') for e in ready] != ['lor.validation.local-probe']:
        failures.append('only validation probe may be addressable-ready')
    act=json.loads((root/'Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json').read_text())
    for k in ['localAddressablesActivationApproved','remoteCatalogActivationApproved','remoteContentActivationApproved','catalogUpdatesActivationApproved','finalRuntimeActivationApproved']:
        if act.get(k) is not False: failures.append(f'activation must remain false: {k}')
    econ=(root/'Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json').read_text()
    comm=(root/'Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json').read_text()
    if '"runtimeActivationApproved": false' not in econ: failures.append('economy activation changed')
    for k in ['analyticsActivationApproved','iapActivationApproved','rewardedAdsActivationApproved','finalRuntimeActivationApproved']:
        if f'"{k}": false' not in comm: failures.append(f'commercial activation changed: {k}')
    testhot=(root/'Assets/LegendsOfRus/Tests/DiagnosticsEditMode/ErrorReportFoundationEditModeTests.cs').read_text()
    if 'Array.IndexOf(parameterTypes, typeof(string))' not in testhot or 'Does.Not.Contain(typeof(string))' in testhot:
        failures.append('Step 4 NUnit compile hotfix missing')
    service=(root/'Assets/LegendsOfRus/Scripts/Runtime/Services/ContentDeliveryService.cs').read_text()
    if 'UnityWebRequest' in service or 'HttpClient' in service or 'DownloadDependenciesAsync' in service or 'CheckForCatalogUpdates' in service:
        failures.append('remote/network content API found in runtime service')
    report={
      'version':'v1.17-step05',
      'milestone':'addressables-content-delivery-foundation',
      'passed':not failures,
      'status':'PASS_STATIC_SOURCE_AND_PROTECTED_BASELINE_VERIFICATION' if not failures else 'FAIL',
      'unityVersion':'6000.5.9f1',
      'addressablesPackage':'2.8.1',
      'approvedPngFiles':len(pngs),
      'contentManifestEntries':len(entries),
      'productionEntriesAddressableReady':len([e for e in ready if e.get('tier')!='ValidationLocal']),
      'validationEntriesAddressableReady':len([e for e in ready if e.get('tier')=='ValidationLocal']),
      'remoteDeliveryAuthorized':False,
      'unityExecutionEvidence':'NOT_RETAINED',
      'addressablesBuildEvidence':'NOT_RETAINED',
      'representativeDeviceEvidence':'NOT_RETAINED',
      'jsonFilesParsed':json_count,
      'failures':failures
    }
    text=json.dumps(report,indent=2)+"\n"
    if a.output: pathlib.Path(a.output).write_text(text)
    print(text,end='')
    sys.exit(0 if not failures else 1)
if __name__=='__main__': main()
