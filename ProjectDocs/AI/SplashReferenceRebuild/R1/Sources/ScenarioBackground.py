"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="Reconstruct only a clean environment plate from the attached original Splash reference. Remove the entire gigantic gold text logo and its black carved plaque, the man Ivan, the Firebird and all feathers, cloak, hands, clothes, the Start button and its words, the version lettering, all UI and the entire outer gold border. No people, birds, silhouettes, creatures, text, signs, symbols, frames or UI. Preserve the original Russian fairy tale sunset composition: vast dark indigo starry sky in upper half with warm ember-peach cloud streaks near the horizon at y45 percent; distant golden onion domes and slender wooden towers along the left and right margins, central river/city valley narrowing into sunset. Dark birch tree trunk and restrained gold leaves at the far left edge. Painterly ornate medieval wooden town in the lower half, gold window lights, deep blue-black shadowed roofs, subtle snow/frost on far rooftops, dim foreground. Match original restrained dark colors and architectural placements where visible. Fill the formerly hidden central and lower areas with plausible uninterrupted town and river scenery, not a figure-shaped shape or a giant new landmark. Edge-to-edge 941:1672 portrait scenery for independent source sprites to be placed over it. Do not reproduce the screenshot, logo, foreground hero or UI."

try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Splash_R1_hidden_sunset_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
