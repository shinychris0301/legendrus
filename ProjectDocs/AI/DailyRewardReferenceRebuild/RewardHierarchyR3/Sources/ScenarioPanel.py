from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'panel';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Repair THREE original game UI surfaces in this image. Preserve exact positions and dimensions on the black 1024x1024 canvas. Top LEFT gold panel (x30,y30,width430,height355): remove the large chest and the red Day 7 title plaque and all lettering, reconstruct warm dark bronze/gold material with luminous gold sparkles and soft golden rays around the center. Preserve original outer gold frame. Top RIGHT red plaque (x520,y30,width280,height81): remove only lettering, preserve opaque dark burgundy face and gold scrollwork exactly. Below it blue shield badge (x560,y200,width101,height122): remove only the number and word, preserve opaque sapphire blue enamel and the original gold perimeter. Do not invent text or objects, do not move or resize parts, do not crop ornaments. Keep all areas outside these three parts black. Only hidden face pixels will be used; original visible borders are retained separately."""
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/panel.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'DailyReward_hierarchy_surfaces_R3.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
