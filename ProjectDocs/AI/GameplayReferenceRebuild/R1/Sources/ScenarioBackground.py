from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'environment';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Edit this exact reference into a clean game environment plate. Remove every interface element: character portrait, all upper header panels and words, the large 8 by 7 gem board and border, four lower booster cards and settings button. Reconstruct the scenery hidden behind these only. Preserve the exact camera, framing, scale, lighting and surviving architecture. Dark Russian fairy tale dusk village with birch trunks along both edges; navy starry sky and crescent moon; glowing gold onion domes and timber houses across the upper middle at y18-29 percent; river and reflection below the board, flowers and dark cobbled foreground at the bottom. Keep restrained dark navy/bronze palette and warm windows. Fill the central former board with quiet dark village scenery, no dramatic new character or giant object. No UI, borders, boards, panels, text, symbols, people, controls, gems, gold rails or watermarks. This is hidden scenery reconstruction, not a different landscape.'''
try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Gameplay_calmer_original_scenery.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Scenery hidden under panels cannot be recovered from original pixels; keep visible original environment outside repair mask.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
