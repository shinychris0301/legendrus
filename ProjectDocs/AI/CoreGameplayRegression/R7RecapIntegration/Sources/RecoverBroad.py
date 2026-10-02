"""Recover completed broad XML/log evidence after loss of the launcher session."""
from pathlib import Path
import json
import xml.etree.ElementTree as ET

BASE=Path(__file__).resolve().parent.parent
out=BASE/'QA/Broad'
assert not (out/'summary.json').exists()
tree=ET.parse(out/'results.xml')
result=tree.getroot().attrib
assert int(result['total'])==1353 and int(result['passed'])==1339
assert int(result['failed'])==0 and int(result['skipped'])==14
log=(out/'unity.log').read_text()
marker='Test run completed. Exiting with code 0 (Ok). Run completed.'
assert marker in log
options=json.loads((BASE/'Sources/BroadOptions.json').read_text())
record={'exitCode':None,'launcherExitStatusUnavailableAfterInterruption':True,
        'reportedUnityExitCode':0,'unityCompletionMarker':marker,
        'durationSeconds':None,'testDurationSeconds':float(result['duration']),
        'isolatedConfig':options['XDG_CONFIG_HOME'],'processOnlyVulkan':True,
        'phaseOverrides':options,'result':result,
        'nonPassing':[{'name':c.get('fullname'),'result':c.get('result'),
                       'message':c.findtext('.//message')} for c in tree.findall('.//test-case') if c.get('result')!='Passed'],
        'recoveryBasis':'Completed XML plus explicit Unity exit0 marker; launcher status is not inferred.'}
(out/'summary.json').write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps({k:record[k] for k in ['exitCode','reportedUnityExitCode','testDurationSeconds','result']},indent=2))
