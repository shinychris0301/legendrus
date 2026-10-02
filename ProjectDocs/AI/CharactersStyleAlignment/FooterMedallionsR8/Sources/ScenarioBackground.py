"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Create a single clean horizontal carved navigation footer backing based on the supplied original game footer. Remove ALL circular icons, medallions, back arrow, map, chest, scroll, shield, captions, text and notification badges. Reconstruct only the ornamental panel hidden underneath. Dark nearly black carved walnut and antique bronze-gold Slavic vine scrollwork, restrained warm edge highlights, continuous narrow antique gold top and bottom borders, dark inset inner band, symmetrical curving scrolls concentrated on left and right outer corners. Preserve the reference subtle dark palette, intricate thin engraving and understated relief, no bright gold masses. Full edge-to-edge flat front-facing panel, no perspective. Aspect ratio 4 to 1. This is a UI backing texture only, no buttons, no lettering, no figures, no jewels, no scenic background. Keep the center dark so separately added round icons will read clearly.'
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/footer_guidance.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Characters_R8_hidden_footer_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':256,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Backing hidden underneath source footer controls; reconstruct only clean backing, preserve separately isolated original icon RGB.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
