from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'repair';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Repair the THREE exact UI surfaces in this reference atlas. Keep exact positions and sizes. Large upper panel at x20,y10,width795,height982: retain the gold carved perimeter and sapphire top/bottom crests exactly. Remove ALL inner text, chest, coins, gems, light rays, reward boxes, and the green lower button. Reconstruct a continuous empty dark navy floral lacquer face matching the visible original material; no rectangle seams, no bright patches. Remove any unrelated outside scenery; outer area black. Separate emerald button below at x50,y1060,width451,height138: remove only its letters, preserve original gold ornaments and sapphire bottom crest. Separate small reward card at x600,y1060,width122,height130: remove coins and number, preserve its fine gold frame and repair empty dark navy interior. No new text, objects or ornaments. Do not rearrange or resize the three parts. Output is used only beneath bounded masks; original visible perimeter pixels will be restored separately."""
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/repair.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'RewardReceived_empty_surfaces_R1.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
