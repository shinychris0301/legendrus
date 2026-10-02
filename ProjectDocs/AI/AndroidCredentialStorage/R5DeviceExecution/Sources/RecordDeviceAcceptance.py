from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
runs=sorted((base/'QA/DeviceRuns').iterdir());assert runs
run=runs[-1];status=json.loads((run/'RunStatus.json').read_text());assert status['requestedPhasesPassed'] and status['installAttempted']
reports={name:json.loads((run/(name+'.json')).read_text()) for name in ['prepare','restart','verify']}
assert [reports[n]['status'] for n in reports]==['awaiting_restart','awaiting_force_stop','passed']
assert all(r['authenticatedSessionTested'] is False for r in reports.values())
normal=json.loads((base/'QA/NormalLaunch.json').read_text());assert normal['processRunning'] and normal['redactedReportUnchanged'] and normal['ownershipDirectoryAbsent'] and normal['invalidStartCallbackErrors']==0
artifact=json.loads((base/'QA/Artifact.json').read_text());artifact.update(installed=True,deviceTested=True,deviceTestScope='Credential storage probe and normal title-screen startup only');(base/'QA/Artifact.json').write_text(json.dumps(artifact,indent=2)+'\n')
apk=json.loads((base/'QA/ApkVerification.json').read_text());apk.update(physicalDeviceTested=True,deviceExecutionEvidence='DeviceAcceptance.json');(base/'QA/ApkVerification.json').write_text(json.dumps(apk,indent=2)+'\n')
evidence={'device':json.loads((base/'QA/Device.json').read_text()),'artifactSha256':artifact['sha256'],'finalRun':str(run.relative_to(base)), 'passedPhases':3,'reportedChecks':sum(len(r['checks']) for r in reports.values()),'actualUnityJniAndroidKeyStoreAtomicFileExecuted':True,'normalStartup':normal,'hardwareBackedKeyAttested':False,'physicalPowerLossTested':False,'authenticatedSessionTested':False,'allAndroidVersionsTested':False}
(base/'QA/DeviceAcceptance.json').write_text(json.dumps(evidence,indent=2)+'\n');print(json.dumps(evidence,indent=2))
