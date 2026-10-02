"""Scenario source-alpha isolation plus one clean empty card. Submit once per named asset."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ShopStyleAlignment/R1';name,action=sys.argv[1:];assert name in ('title','bag','chest','cart','magic','card')
trial.OUT=base/'Sources/Scenario'/name;trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_openai-gpt-image-2' if name=='card' else 'model_pixa-background-removal'
if action=='submit':
 p=base/'Sources/Crops'/(name+'.png')
 if name=='card':
  if not (trial.OUT/'Reference.json').exists():
   res=trial.api('/assets',{'name':'Shop_Original_Card_Reference.png','image':base64.b64encode(p.read_bytes()).decode()});trial.save('Reference.json',{'assetId':res['asset']['id'],'source':str(p.relative_to(r)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
  prompt='''Use case: precise-object-edit. Reconstruct this ONE original Russian fairy-tale shop product card as a completely EMPTY frame. Keep the precise shape, front-facing aspect ratio, restrained antique-gold ornate narrow border, corner scrollwork and original dark warm brown parchment interior. Remove the coin bag, loose coins, title lettering, amount plaque, green purchase button, price, all words and numbers. Their areas become continuous dark brown parchment matching the original empty corners, natural irregular non-repeating texture. Do not leave any lettering shadow, coin residue, badge or button. Keep the gold border thin and exactly at the original outer perimeter, never enlarge the ornament or bow/stretch the frame. Single rounded rectangular card, not a screen. The entire inner face is opaque. Exterior plain black for local silhouette masking; no checkerboard. No other object, no person, no scenery, no text, no glow, no exaggerated yellow highlights. Original source border will be retained separately where possible, so accurate alignment is important.'''
  body={'prompt':prompt,'referenceImages':[json.loads((trial.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1152,'quality':'medium','background':'opaque','numOutputs':1};trial.save('Request.json',{'modelId':trial.MODEL,'body':body})
 else:
  body={'image':'data:image/png;base64,'+base64.b64encode(p.read_bytes()).decode(),'outputFormat':'rgba'};trial.save('Request.json',{'modelId':trial.MODEL,'source':str(p.relative_to(r)),'outputFormat':'rgba','use':'alpha only with unchanged original RGB'})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,body)
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover request, do not resubmit';trial.save('Job.json',{'id':jid,'status':job.get('status')});print(name,'accepted',flush=True)
elif action=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError('submit or collect')
