from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'card';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Precise material repair of this ONE Russian fairy-tale shop product card. Keep its EXACT original framing, dimensions, colors and thin antique-gold corner scrollwork. Remove the coin bag and ALL loose coins, ALL lettering, coin amount plaque, green price button and all price figures. Fill their occupied areas with continuous dark warm brown parchment matching the original upper card corners. Subtle irregular natural texture, no tiling, no stripes, no rectangular patches or shadows. Preserve original gold perimeter at its exact position and thickness. Do not add any decoration, subject, text, numbers, coins or buttons. No glow, no enlargement. Entire inner face opaque. Exterior black. The original border pixels will be restored separately; only the repaired parchment face will be used."""
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/Card.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Shop_empty_card_R2.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
