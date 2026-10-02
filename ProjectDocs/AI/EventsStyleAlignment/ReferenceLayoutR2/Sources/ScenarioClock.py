"""One authorized clock isolation; never duplicate a paid submission."""
from pathlib import Path
import sys, json, base64, urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'ScenarioClock';t.OUT.mkdir(exist_ok=True);t.MODEL='model_pixa-background-removal'
try:
 if sys.argv[1]=='submit':
  body={'image':'data:image/png;base64,'+base64.b64encode((b/'clock_api_input.png').read_bytes()).decode(),'outputFormat':'rgba'}
  t.save('Request.json',{'model':t.MODEL,'source':'clock_api_input.png','outputFormat':'rgba','purpose':'Isolate original Events clock, retain original RGB'})
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing request; never resubmit'
  t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clock isolation submitted.')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'details suppressed');e.close();raise SystemExit(1)
