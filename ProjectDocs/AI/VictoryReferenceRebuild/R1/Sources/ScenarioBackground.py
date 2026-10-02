from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'environment';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Edit this exact Victory reference into a CLEAN ENVIRONMENT PLATE. Remove ALL UI, the entire Firebird, all three stars, every title/frame/panel/button, rewards, icons, gold rails, lettering, numbers, scattered gold sparks and celebration rays. Reconstruct only scenery hidden behind them. Keep the original camera and architecture placement in visible portions: dark Russian fairy tale village at dusk, birch trunks at left/right, warm windows, distant golden onion domes right-middle, timber houses on left, navy purple starry sky above, dark cobbled street and foliage below. Match the original dark navy and warm antique gold lighting; no new focal subject. Preserve the ornate dark carved corner/side environment surround but no UI frames. Avoid a flat rectangle where the old panel was. This is hidden scenery reconstruction from the same view, not a redesigned landscape. No people, birds, gems, stars as icons, panels, borders, buttons, text or watermark.'
try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Victory_original_scenery.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Scenery hidden under panels cannot be recovered from original pixels; keep visible original environment outside repair mask.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
