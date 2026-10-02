"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="Create ONE clean environment plate reconstructing only the scenery hidden behind the man and fiery bird in the supplied reference. Remove Ivan and the firebird COMPLETELY, including all feathers, cloak, clothing, face, hair, hands, jewelry and body. No characters, people, birds, creatures, silhouettes, UI, text, panel, border or decorative frame. Preserve the reference setting: a medieval Russian fairy-tale town at warm sunset, steep carved dark wooden roofs and slender towers at left and right, a distant onion-domed citadel, amber window lights, warm orange-peach cloud bands in the top third against muted indigo-purple sky, dim blue-brown rooftops and stone street lower down. Source restrained dark painterly realism, antique amber illumination, deep blue shadows. Keep the architectural perspective and visible building placement along the left side as close to the source as possible; seamlessly reconstruct the hidden central/right background. The foreground is just dim empty stone and timber, without any figure-shaped object. Full edge-to-edge scenery for a separate original character sprite, not a game screenshot. Avoid oversaturated orange, bright broad daytime sky, gigantic central palace or foreground vegetation."
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/hero_firebird_r7.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Characters_R7_hidden_sunset_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1280,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
