"""Bounded Scenario alpha extraction; preserve source RGB, never resubmit accepted jobs."""
from pathlib import Path
import sys,json,time,base64,urllib.error
r=Path.cwd();sys.path.insert(0,str(r/'Tools'))
import scenario_trial as trial
base=r/'ProjectDocs/AI/PauseStyleAlignment/R1'
name,action=sys.argv[1:];assert name in ('title','resume','restart','settings','exit')
trial.MODEL='model_pixa-background-removal';trial.OUT=base/'Sources/Scenario'/name;trial.OUT.mkdir(parents=True,exist_ok=True)
if action=='submit':
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticRetry':False},f)
 source=base/'Sources/Crops'/f'{name}.png'
 trial.save('Request.json',{'modelId':trial.MODEL,'source':str(source.relative_to(r)),'outputFormat':'rgba','use':'alpha only; retain original source RGB and fixed Russian lettering'})
 try:res=trial.api('/generate/custom/'+trial.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'})
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover job, do not resubmit'
 trial.save('Job.json',{'id':jid,'status':job.get('status')});print(name,'accepted',flush=True)
elif action=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError(action)
