from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2'
try:
 if sys.argv[1]=='submit':
  raw=(b/'SurfaceGuidance.png').read_bytes()
  if not(t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Events_R4_tab_and_footer_surfaces.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  prompt='''Edit this exact 1024x512 UI asset sheet in place. Keep both assets at precisely their existing coordinates and sizes, with the same grey surrounding canvas. On the upper blue tab, erase the gold Cyrillic caption and its black shadow, restoring uninterrupted rich mottled sapphire-blue lacquer. Remove the red exclamation badge completely and reconstruct the original thin antique gold frame and blue inner rail it covers at the upper right. Preserve all visible original frame pixels, colours, corners and shapes. On the very thin lower footer strip, erase ONLY the sentence, restoring the same dark navy-black carved floral lacquer under the text. Preserve the small flourishes beside the sentence, ALL gold lower rail, blue centre jewel, both curled gold corners, and dark outer carved edge. Do not reposition, resize, crop or redesign either asset. No lettering, symbols, notifications or text remaining. No extra ornaments, no brighter metal, no wood or scenery. This is a precise hidden-surface repair, not a new UI design.'''
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':512,'quality':'medium','background':'opaque','numOutputs':1}
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Only hidden blue tab and navy footer under original lettering/badge require repair; original exposed pixels retained.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Repair job accepted')
 elif sys.argv[1]=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
