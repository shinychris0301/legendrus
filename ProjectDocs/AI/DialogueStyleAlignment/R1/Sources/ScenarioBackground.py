"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Create one clean background environment plate based on the supplied story dialogue reference. Remove BOTH men entirely, all human figures, dialogue panel, nameplate, button, lettering, frame and all UI. Reconstruct the concealed carved wooden Russian medieval hall. Preserve dark walnut carved pillars and beams, amber brass hanging lanterns in upper left and upper right, long dark blue banner with antique gold heraldic lion in upper center, cool blue latticed windows along left wall, warm candlelit depth and detailed carved wood. No humans, no faces, no silhouettes, no characters, no interface or text. Full portrait composition extending to the bottom with a dark carved table and wooden floor. Restrained painterly realism, source warm amber and deep brown colors. A clean scene for separate original character sprites; NOT a screenshot reproduction. Do not replace the source architecture with an outdoor landscape.'
try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Dialogue_R1_hidden_hall_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
