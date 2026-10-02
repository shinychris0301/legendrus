from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'environment';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Edit this reference into a clean environment plate for the same game screen. Remove ALL user interface: top player/resources header, portrait, red chapter banner, all navy goal/move/booster/reward panels and all their lettering/icons, bottom buttons and carved footer. Reconstruct only the scenery hidden behind them. Retain the exact original camera, lighting, scenery and positions: dusk Russian fairy-tale village, church with gold onion domes on the LEFT at x12 percent y31 percent, wooden windmill on the RIGHT at x85 percent y29 percent, dark timber houses down both edges, birch branches on both edges, navy dusk sky with warm sunset clouds. The center is an unobtrusive stone village lane. Keep the left and right surviving scenery faithful with warm lit windows and restrained antique-gold autumn leaves. Painterly dark navy/bronze art, no fluorescent colors. Do not move the church to the center, do not add a central castle, lake or large hero. Output only a full edge-to-edge portrait landscape with NO UI, panels, frame, icons, text or people. This is background repair, not a redesign.'''
try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'PreLevel_original_scenery_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Scenery hidden under panels cannot be recovered from original pixels; keep visible original environment outside repair mask.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
