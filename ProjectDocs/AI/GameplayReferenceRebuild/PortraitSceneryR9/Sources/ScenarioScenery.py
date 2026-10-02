from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'portrait_scenery';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Edit this existing Russian fairy-tale game scenery. REMOVE the broad dark navy/black rectangular-looking patch across the UPPER LEFT, behind and between the birch trees. The previous repair only softened its lower edge but left the dark patch itself, which is unacceptable. Replace the ENTIRE dark patch with luminous blue-violet dusk sky and soft pink sunset clouds continuing naturally from the unobstructed sky in the upper RIGHT and middle of this image. Carry that same sky brightness and cloud lighting all the way behind BOTH birch trunks at upper left. The upper-left sky must not remain uniformly dark navy or black. It must visually connect with the brighter violet twilight around the church spires. Keep the gold leaves and birch trunks, but light them consistently with this twilight; no rectangular or horizontal dark overlay, no shadow strip, no vignette or dark halo. Preserve precisely the camera, building silhouettes, their positions, domes and warm windows in the middle/lower part. Keep painterly fine details and the existing blue-violet/antique gold palette. This is one continuous background painting, no interface, text, frame, portrait, character, panel, border, badge, or new object. Only replace the unwanted dark sky/lighting patch behind the upper-left trees; preserve the rest.'''
try:
 if sys.argv[1]=='submit':
  p=b/'EnvironmentInput.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Gameplay_remove_dark_portrait_patch.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False,'allowedRequests':1},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Replace the remaining dark patch itself; R8 softened its edge without removing its dark body.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('One scenery repair accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
