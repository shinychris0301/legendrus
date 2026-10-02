from pathlib import Path
import sys,json,base64,time
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=root/'ProjectDocs/AI/DailyRewardStyleAlignment/BackIconR2';t.OUT=b/'Sources/Scenario';t.MODEL='model_pixa-background-removal';t.OUT.mkdir(exist_ok=True)
if sys.argv[1]=='submit':
 p=b/'Sources/BackSource.png';body={'image':'data:image/png;base64,'+base64.b64encode(p.read_bytes()).decode(),'outputFormat':'rgba'}
 t.save('Request.json',{'modelId':t.MODEL,'source':str(p.relative_to(root)),'outputFormat':'rgba','use':'fresh alpha; retain original RGB'})
 with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 res=t.api('/generate/custom/'+t.MODEL,body);j=res.get('job',res);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing request, do not resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print('Fresh extraction accepted')
elif sys.argv[1]=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
