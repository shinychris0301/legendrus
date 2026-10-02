"""Two authorized Scenario source-alpha jobs. Atomic markers prevent duplicate paid submissions."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/EdgesAndRewardsR2';name,action=sys.argv[1:];assert name in ('header','foreground')
trial.OUT=base/'Sources/Scenario'/name;trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_pixa-background-removal'
if action=='submit':
 p=base/'Sources/Crops'/f'{name}.png'
 trial.save('Request.json',{'modelId':trial.MODEL,'source':str(p.relative_to(r)),'sourceSha256':hashlib.sha256(p.read_bytes()).hexdigest(),'outputFormat':'rgba','use':'alpha only with original source RGB'})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(p.read_bytes()).decode(),'outputFormat':'rgba'})
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover accepted job; do not resubmit'
 trial.save('Job.json',{'id':jid,'status':job.get('status')});print(name,'accepted',flush=True)
elif action=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError(action)
