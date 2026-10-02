from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'scenery_seam';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Repair this existing game background crop, preserving exactly the same Russian fairy-tale painting, camera, birch trunks, gold leaves and onion-dome buildings. There is an unwanted rectangular dark patch covering the upper-left part of the image: its horizontal bottom edge is at about 45 percent image height and its right edge about 65 percent width. It looks like a semi-transparent black UI strip over the trees. Remove this artificial rectangular lighting boundary completely. Repaint only the seam so the trees, branches, leaves and blue dusk sky transition naturally and continuously across it, with no visible horizontal or vertical cutoff. Match the soft blue dusk and warm leaf lighting immediately below the seam. Preserve the two birch trunks and the buildings in their existing positions. The upper part should be a natural continuation of these same trees against the same dusk sky, not a uniformly dark rectangle. No panel, shadow strip, gradient bar, text, character, portrait, frame, badge, symbol or interface. Do not add or change the layout or introduce additional objects. This is a local scenery repair, not a new landscape.'''
try:
 if sys.argv[1]=='submit':
  p=b/'EnvironmentSeamInput.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Gameplay_portrait_underlay_seam.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False,'allowedRequests':1},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Repair the visible rectangular join between earlier generated portrait-underlay scenery and original trees/sky.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('One scenery repair accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
