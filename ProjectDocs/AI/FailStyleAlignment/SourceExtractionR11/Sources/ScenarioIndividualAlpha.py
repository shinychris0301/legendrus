"""Bounded original-pixel cutouts. One request per named crop; never auto-resubmit."""
import sys,json,time,base64,urllib.error
from pathlib import Path
from ScenarioExtract import trial,BASE,safe_messages
name=sys.argv[1];action=sys.argv[2]
assert name in ['retry','paid','ad','nameplate','goals','heading','mace','firebird','shield','potion']
trial.MODEL='model_pixa-background-removal'
trial.OUT=BASE/'Sources/Scenario'/('Alpha_'+name);trial.OUT.mkdir(parents=True,exist_ok=True)
if action=='submit':
    if (trial.OUT/'Job.json').exists():raise SystemExit('Already accepted; collect only.')
    with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticRetry':False},f)
    p=BASE/'Sources/Crops'/f'{name}.png'
    trial.save('Request.json',{'modelId':trial.MODEL,'imageFile':str(p.relative_to(BASE)),'outputFormat':'rgba','purpose':'Use alpha only with exact original RGB; clean isolated runtime element after label repair and QA.'})
    try:r=trial.api('/generate/custom/'+trial.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(p.read_bytes()).decode(),'outputFormat':'rgba'})
    except urllib.error.HTTPError as e:
        try:payload=json.loads(e.read(16384))
        except ValueError:payload={}
        trial.save('AccessResult.json',{'httpStatus':e.code,'messages':safe_messages(payload)})
        raise SystemExit(f'Rejected HTTP {e.code}; no retry.')
    j=r.get('job',r);jid=j.get('jobId') or j.get('id')
    assert jid,'Unknown acceptance: retain marker, no retry'
    trial.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'accepted')
elif action=='collect':
    trial.poll()
    if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError(action)
