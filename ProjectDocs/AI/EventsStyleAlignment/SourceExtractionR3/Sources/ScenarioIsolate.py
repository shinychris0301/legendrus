"""Source alpha requests with immutable non-secret parameters and no auto-resubmission."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;name,action=sys.argv[1:3];assets=json.loads((b/'AssetAssessment.json').read_text())['assets'];asset=next(x for x in assets if x['name']==name)
t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_pixa-background-removal'
try:
 if action=='submit':
  p=b/'Inputs'/(name+'.png');raw=p.read_bytes()
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'endpoint':'/generate/custom/'+t.MODEL,'imageFile':str(p.relative_to(root)),'imageSHA256':hashlib.sha256(raw).hexdigest(),'outputFormat':'rgba','sourceBox':asset['sourceBox'],'operation':'Alpha isolation; original RGB retained outside explicitly recorded repairs'})
  r=t.api('/generate/custom/'+t.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'});j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover accepted job; never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'accepted')
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(name,'HTTP',e.code,'details suppressed; no retry');e.close();raise SystemExit(1)
