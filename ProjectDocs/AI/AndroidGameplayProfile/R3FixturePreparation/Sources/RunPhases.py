from pathlib import Path
import json
import subprocess
import sys

sources=Path(__file__).resolve().parent
plan=json.loads((sources/'PhasePlan.json').read_text())
for phase in plan:
    subprocess.run([sys.executable,str(sources/'Verify.py'),phase['name'],'PlayMode',phase['filter'],phase['options']],check=True)
    summary=json.loads((sources.parent/'QA'/phase['name']/'summary.json').read_text())
    assert summary['exitCode']==0 and not summary['nonPassing'] and int(summary['result']['passed'])==1
print('All fourteen separate fixture/normal-profile processes passed.',flush=True)
