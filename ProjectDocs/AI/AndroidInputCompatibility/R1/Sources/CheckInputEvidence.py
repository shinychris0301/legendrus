from pathlib import Path
import json,re,ast,xml.etree.ElementTree as ET
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
old=(base/'Sources/Before/ProjectSettings/ProjectSettings.asset').read_text();new=(root/'ProjectSettings/ProjectSettings.asset').read_text()
assert new==old.replace('  activeInputHandler: 2','  activeInputHandler: 1'),'Unexpected PlayerSettings changes'
log=(base/'QA/Build/unity.log').read_text()
assert 'Active Input Handling is set to Both' not in log,'Android input warning remains'
assert 'Build Fix04 APK created at the existing policy output' in log,'Guarded build success missing'
checks=[]
for run,expected in [('Compatibility',18),('PolicyTests',120)]:
 s=json.loads((base/'QA'/run/'summary.json').read_text());r=s['result']
 assert s['exitCode']==0 and int(r['passed'])==expected and int(r['failed'])==0 and int(r['skipped'])==0
 checks.append({'run':run,'passed':expected,'failed':0,'skipped':0})
for p in (base/'Sources').glob('*.py'):ast.parse(p.read_text())
report={'playerSettingsOnlyChange':'activeInputHandler: 2 -> 1','androidBothWarningAbsent':True,'guardedApkSucceeded':True,'finalRuns':checks,'unityPassed':138,'physicalTouchOrAndroidBackTested':False,'note':'Queued virtual touch/mouse/keyboard and real UI handlers tested in Editor. No phone execution.'}
(base/'QA/InputConfigurationEvidence.json').write_text(json.dumps(report,indent=2))
print(json.dumps(report,indent=2))
