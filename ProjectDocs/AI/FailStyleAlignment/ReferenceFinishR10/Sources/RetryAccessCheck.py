import sys,json,time,re,urllib.error
from pathlib import Path
sys.path.insert(0,str(Path.cwd()/'Tools'))
import scenario_trial as trial
base=Path('ProjectDocs/AI/FailStyleAlignment/ReferenceFinishR10')
config=json.loads((base/'Sources/ScenarioRequests.json').read_text())
asset=next(a for a in config['assets'] if not (base/'Sources/Scenario'/a['name']/'Job.json').exists())
folder=base/'Sources/Scenario'/asset['name'];folder.mkdir(parents=True,exist_ok=True)
trial.OUT=folder;trial.MODEL=config['modelId']
marker=folder/'SubmissionAttempt.json'
with marker.open('x') as f:json.dump({'submittedUnix':time.time(),'automaticResubmission':False,'authorization':'User: retry check; one pending asset only'},f)
trial.save('Request.json',{'modelId':trial.MODEL,'body':asset['body']})
record={'asset':asset['name'],'time':time.time(),'requests':1}
try:
 response=trial.api('/generate/custom/'+trial.MODEL,asset['body'])
except urllib.error.HTTPError as e:
 record.update({'httpStatus':e.code,'retryAfter':e.headers.get('Retry-After'),'reset':e.headers.get('X-RateLimit-Reset')})
 raw=e.read(16384)
 try:payload=json.loads(raw)
 except ValueError:payload={}
 messages=[]
 def walk(value):
  if isinstance(value,dict):
   for key,item in value.items():
    if key.lower() in ('message','msg','detail','error','code','reason','title','description') and isinstance(item,str):
     messages.append(re.sub(r'(?:https?://\S+|[A-Za-z0-9_+/=-]{24,})','[redacted]',item)[:500])
    elif isinstance(item,(dict,list)):walk(item)
  elif isinstance(value,list):
   for item in value:walk(item)
 walk(payload)
 record['messages']=messages
 if e.code==429:
  marker.rename(folder/('Rejected429_'+str(time.time_ns())+'.json'))
  record['acceptedJob']=False
 else:record['acceptedJob']='unverified; submission marker retained'
else:
 job=response.get('job',response);job_id=job.get('jobId') or job.get('id')
 if not job_id:raise RuntimeError('Missing job ID; do not retry this request.')
 trial.save('Job.json',{'id':job_id,'status':job.get('status')})
 record.update({'acceptedJob':True,'jobStatus':job.get('status')})
path=base/'QA'/('AccessRetry_'+str(time.time_ns())+'.json');path.write_text(json.dumps(record,indent=2)+'\n')
print(json.dumps(record,indent=2))
