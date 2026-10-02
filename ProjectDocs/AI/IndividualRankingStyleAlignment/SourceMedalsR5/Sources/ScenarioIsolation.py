"""Three bounded source-alpha jobs; no regeneration or automatic resubmission."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
base=Path(__file__).resolve().parent
name,action=sys.argv[1:3];assert name in ('medal_gold','medal_silver','medal_bronze')
t.OUT=base/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_pixa-background-removal'
try:
 if action=='submit':
  p=base/(name+'_input.png');raw=p.read_bytes()
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'operation':'isolate original medal with original numeral; original RGB retained locally','input':str(p.relative_to(root)),'sha256':hashlib.sha256(raw).hexdigest(),'outputFormat':'rgba'})
  r=t.api('/generate/custom/'+t.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'});j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job; do not resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'accepted',flush=True)
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(name,'HTTP',e.code,'no retry',flush=True);e.close();raise SystemExit(1)
