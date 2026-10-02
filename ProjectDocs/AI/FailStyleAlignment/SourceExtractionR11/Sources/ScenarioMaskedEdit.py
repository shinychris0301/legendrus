"""One masked caption repair using the existing Scenario model; never a whole-asset redesign."""
import json,base64,time,sys,urllib.error
from pathlib import Path
from ScenarioExtract import safe_messages,trial,ROOT,BASE
trial.MODEL='model_openai-gpt-image-2';trial.OUT=BASE/'Sources/Scenario/MaskedNameplate';trial.OUT.mkdir(parents=True,exist_ok=True)
if (trial.OUT/'Job.json').exists():raise SystemExit('Existing job recorded; collect it instead of submitting again.')
spec=json.loads((BASE/'Sources/MaskedEditSpec.json').read_text())
encode=lambda path:'data:image/png;base64,'+base64.b64encode((BASE/path).read_bytes()).decode()
body={'prompt':spec['prompt'],'referenceImages':[encode(spec['referenceImages'][0])],'mask':encode(spec['mask']),'width':944,'height':1680,'quality':'medium','numOutputs':1}
marker=trial.OUT/'SubmissionAttempt.json'
with marker.open('x') as f:json.dump({'time':time.time(),'automaticRetry':False},f)
trial.save('Request.json',{'endpoint':'/generate/custom/'+trial.MODEL,'specFile':'Sources/MaskedEditSpec.json','scope':'One caption repair, original border pixels retained outside edit mask'})
try:response=trial.api('/generate/custom/'+trial.MODEL,body)
except urllib.error.HTTPError as e:
    try:payload=json.loads(e.read(16384))
    except ValueError:payload={}
    record={'time':time.time(),'httpStatus':e.code,'retryAfter':e.headers.get('Retry-After'),'reset':e.headers.get('X-RateLimit-Reset'),'messages':safe_messages(payload),'acceptedJob':False if e.code in (400,402,403,422,429) else 'unknown; do not retry'}
    trial.save('AccessResult.json',record);print(json.dumps(record,indent=2))
    if e.code==429:marker.rename(trial.OUT/('Rejected429_'+str(time.time_ns())+'.json'))
else:
    job=response.get('job',response);job_id=job.get('jobId') or job.get('id')
    if not job_id:raise RuntimeError('No job ID; retain submission marker and recover from history.')
    trial.save('Job.json',{'id':job_id,'status':job.get('status')});print('One masked caption edit accepted; job ID saved.')
