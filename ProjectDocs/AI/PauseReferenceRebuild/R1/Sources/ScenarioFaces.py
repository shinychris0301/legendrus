from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'faces';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Repair this fantasy game modal panel into a completely empty continuous dark navy-black lacquer panel. Preserve only the exterior antique gold frame and the dark navy face. Remove ALL internal content: all text, numbers, button frames, green/blue/red buttons, gem icons, booster icons, small horizontal rules and decorations inside the large outer frame. Fill the entire cleared interior with subtle, sharp, irregular painterly navy floral lacquer texture matching the original unoccupied background. No repeated stripes, no tiled checker pattern, no letters or icons, no light patches. Preserve original panel coordinates and proportions. This is a blank material repair; original outer frame pixels will be restored by compositing. No extra objects.' 
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/panel.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Pause_bounded_panel_R2.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
