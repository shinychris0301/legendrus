"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Restore ONLY the backgrounds behind these four foreground characters as four clean environment plates. Characters will be original extracted sprites added separately: do NOT draw any characters or creatures. Exactly 2 by 2 equal rectangular panels, no gutters, no frames, no text. Top left: dense gilded Russian fairy-tale city with violet twilight sky and warm lamps, source firebird is completely absent, no feathers or wings. Top right: mysterious midnight teal enchanted forest, mossy hut on chicken legs, faint green magical light low down; no witch, cauldron or person. Bottom left: golden-lit medieval Russian merchant harbor and distant domed city; no merchant, chest or person. Bottom right: cold pale blue snowy Russian towers and ice landscape; no knight, shield or person. Faithfully retain the source architecture, local palette, soft painterly lighting and position of visible buildings where possible. Restore coherent empty scenery where foreground subjects obscure it. Each panel fills its own rectangle right to its boundary. No interface, lettering, icons, badges, banners or borders. These are separate background plates, not a game screen.'''
try:
 if sys.argv[1]=='submit':
  p=b/'background_guidance.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Events_R3_hidden_scenery_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':768,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
