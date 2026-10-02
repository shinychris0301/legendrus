from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'environment';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Repair the lower scenery of this exact Russian fairy-tale game screenshot. Remove the entire gem board, frame, booster cards and gear. KEEP the narrow existing skyline strip at the top, at its exact original size and position. Below it paint ONE continuous dark riverbank and reflective river with small warm reflections, flowers and birch trunks at the left and right edges, cobbled footpath at the bottom. The central area formerly hidden by the board should be quiet dark river, shore and low vegetation, NOT another city. Do NOT add or duplicate towers, onion domes, moon, sky or a second horizon anywhere below the existing topmost strip. Keep the surviving lower water and foreground composition and antique navy/gold painterly style. No UI, lettering, grids, frames, gems, panels, people or watermarks. Single coherent scenery, no collage, abrupt horizontal joins or repeated skyline. This is a bounded hidden-background repair.'
try:
 if sys.argv[1]=='submit':
  p=b/'LowerReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Gameplay_calmer_original_scenery.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1408,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Scenery hidden under panels cannot be recovered from original pixels; continue a quiet river below the original skyline without duplicating architecture.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
