"""Mail joined tabs and preview: source alpha isolation only; exclusive markers prevent duplicate paid requests."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;name,action=sys.argv[1:3];t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_pixa-background-removal'
try:
 if action=='submit':
  p=b/'Sheet.png';raw=p.read_bytes();body={'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'}
  with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False},f)
  recorded={'image':{'localFile':str(p.relative_to(root)),'sha256':hashlib.sha256(raw).hexdigest()},'outputFormat':'rgba'}
  t.save('Request.json',{'model':t.MODEL,'endpoint':'/generate/custom/'+t.MODEL,'body':recorded,'operation':'source alpha isolation only; retain original input RGB'})
  result=t.api('/generate/custom/'+t.MODEL,body);job=result.get('job',result);jid=job.get('jobId')or job.get('id');assert jid,'Recover accepted job; never resubmit';t.save('Job.json',{'id':jid,'status':job.get('status')});print(name,'accepted',flush=True)
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 else:raise ValueError('Use submit or collect')
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(name,'HTTP',e.code,'details suppressed; no automatic retry');e.close();raise SystemExit(1)
