"""Bounded Scenario extraction; exclusive submission markers prevent duplicate paid calls."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;name,action=sys.argv[1:3];t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True)
t.MODEL='model_openai-gpt-image-2' if name=='background' else 'model_pixa-background-removal'
try:
 if action=='submit':
  p=b/'OriginalReference.png' if name=='background' else b/'Inputs'/(name+'.png');raw=p.read_bytes()
  if name=='background':
   if not (t.OUT/'Reference.json').exists():
    r=t.api('/assets',{'name':'IndividualRanking_original_scenery_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
   prompt='''Create only a clean full-bleed background environment plate for this Russian fairy-tale ranking screen. Remove the entire user interface and reconstruct only its hidden scenery. Preserve the original visible edge scenery and its color mood as closely as possible: midnight navy and black enchanted birch forest, pale textured white birch trunks arching up both side edges, sparse subdued bronze-gold leaves, tiny dark foliage at the bottom, central area receding into a very dark textured blue-black forest. Portrait orientation. NO sunset, city, castle, people, portraits, firebird, light rays, rewards, chests, medals, frames, plaques, borders, icons, text, letters, numbers or interface whatsoever. The center must stay very dark and low contrast so opaque separate native UI panels can be placed there later. This image is only the environment, not a complete game screen. No checkerboard. Do not repaint or include any of the interface assets.'''
   body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  else:body={'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  recorded=body.copy()
  if 'image' in recorded:recorded['image']={'localFile':str(p.relative_to(root)),'sha256':hashlib.sha256(raw).hexdigest()}
  t.save('Request.json',{'model':t.MODEL,'endpoint':'/generate/custom/'+t.MODEL,'body':recorded,'operation':'clean hidden scenery reconstruction' if name=='background' else 'source alpha isolation only; retain original input RGB'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover accepted job; never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'accepted',flush=True)
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(name,'HTTP',e.code,'details suppressed; no retry');e.close();raise SystemExit(1)
