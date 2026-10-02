"""One bounded title-alpha request; preserve original RGB and never auto-resubmit."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'ScenarioTitle';t.OUT.mkdir(exist_ok=True);t.MODEL='model_pixa-background-removal'
a=sys.argv[1]
try:
 if a=='submit':
  p=b/'TitleInput.png';raw=p.read_bytes()
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'imageFile':str(p),'sha256':hashlib.sha256(raw).hexdigest(),'sourceBox':[205,95,754,302],'operation':'Isolate full original Events title and lettering; no RGB regeneration'})
  r=t.api('/generate/custom/'+t.MODEL,{'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'});j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover job; do not resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print('Title alpha accepted')
 elif a=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('HTTP',e.code,'no retry');e.close();raise SystemExit(1)
