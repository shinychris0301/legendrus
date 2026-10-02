"""One scoped Scenario text-removal trial; no redraw prompt, no automatic resubmission."""
from pathlib import Path
import sys,json,time,base64,re,urllib.error
ROOT=Path.cwd();sys.path.insert(0,str(ROOT/'Tools'))
import scenario_trial as trial
BASE=ROOT/'ProjectDocs/AI/FailStyleAlignment/SourceExtractionR11'
trial.OUT=BASE/'Sources/Scenario/Nameplate';trial.OUT.mkdir(parents=True,exist_ok=True)
trial.MODEL='model_photoroom-text-remover'

def safe_messages(payload):
    result=[]
    def visit(v):
        if isinstance(v,dict):
            for key,value in v.items():
                if key.lower() in ('message','msg','detail','error','reason','code') and isinstance(value,str):
                    result.append(re.sub(r'(?:https?://\S+|[A-Za-z0-9_+/=-]{24,})','[redacted]',value)[:500])
                elif isinstance(value,(dict,list)):visit(value)
        elif isinstance(v,list):
            for value in v:visit(value)
    visit(payload);return result

def main(action):
    if action=='submit':
        if (trial.OUT/'Job.json').exists():print('Job already accepted; use collect.');return
        marker=trial.OUT/'SubmissionAttempt.json'
        with marker.open('x') as f:json.dump({'time':time.time(),'automaticRetry':False},f)
        source=BASE/'Sources/nameplate_input.png'
        trial.save('Request.json',{'endpoint':'/generate/custom/'+trial.MODEL,'modelId':trial.MODEL,'imageFile':str(source.relative_to(ROOT)),'mode':'artificial','purpose':'Remove caption; preserve original plaque pixels. Intermediate output only pending comparison and clean-alpha isolation.'})
        body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'mode':'artificial'}
        try:response=trial.api('/generate/custom/'+trial.MODEL,body)
        except urllib.error.HTTPError as e:
            try:payload=json.loads(e.read(16384))
            except ValueError:payload={}
            record={'time':time.time(),'httpStatus':e.code,'retryAfter':e.headers.get('Retry-After'),'reset':e.headers.get('X-RateLimit-Reset'),'messages':safe_messages(payload),'acceptedJob':False if e.code==429 else 'unverified; do not resubmit blindly'}
            trial.save('AccessResult.json',record)
            if e.code==429:marker.rename(trial.OUT/('Rejected429_'+str(time.time_ns())+'.json'))
            print(json.dumps(record,indent=2));return
        job=response.get('job',response);job_id=job.get('jobId') or job.get('id')
        if not job_id:raise RuntimeError('Missing job ID; retain marker and recover from history.')
        trial.save('Job.json',{'id':job_id,'status':job.get('status')});print('One text-removal job accepted; ID recorded.')
    elif action=='collect':
        trial.poll();state=json.loads((trial.OUT/'Status.json').read_text())
        if state['status']=='success':
            trial.download()
            (trial.OUT/'booster_medallion_trial01.png').rename(trial.OUT/'nameplate_text_removed.png')
            p=trial.OUT/'Output.json';o=json.loads(p.read_text());o['file']='nameplate_text_removed.png';p.write_text(json.dumps(o,indent=2)+'\n')
    else:raise ValueError('Use submit or collect')

if __name__=='__main__':main(sys.argv[1])
