from pathlib import Path
import sys, json, base64, hashlib
root=Path.cwd();sys.path.insert(0,str(root/'Tools'))
import scenario_trial as t
b=Path(__file__).resolve().parent
name,action=sys.argv[1:3]
t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True)
t.MODEL='model_pixa-background-removal'
try:
    if action=='submit':
        source=b/'Inputs'/(name+'.png');raw=source.read_bytes()
        body={'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'}
        with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'maximumPaidSubmissions':1,'automaticResubmission':False},f)
        t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),'sha256':hashlib.sha256(raw).hexdigest(),'outputFormat':'rgba'})
        r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r)
        t.save('Job.json',{'id':j.get('jobId')or j.get('id'),'status':j.get('status')})
        print(name,'accepted')
    else:
        t.poll()
        if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as error:
    print(type(error).__name__, "Scenario operation failed; details suppressed, no resubmission.");sys.exit(1)
