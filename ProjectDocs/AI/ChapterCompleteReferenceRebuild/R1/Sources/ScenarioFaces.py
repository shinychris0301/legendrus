from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'faces';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Exact localized edit of this asset atlas. Maintain all five panels in precisely the SAME locations, dimensions and colors. Remove ALL lettering from the red chapter banner at top, green button and blue button below it, small red victory ribbon in the middle, and parchment teaser at bottom. Reconstruct smooth continuous intricate painterly materials underneath: crimson embossed silk for the banners, emerald lacquer and sapphire lacquer for the buttons, warm old parchment for the teaser. Keep original gold frames, sapphire jewels and right-side forest illustration. The faces must have subtle irregular natural texture, NO repeating tiles or stripes, NO letters, NO numbers, NO new decorations. Do not move or resize anything. Keep dark original colors. Original exterior pixels will be restored by bounded compositing, only the text-removal regions are needed.' 
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/FaceAtlas.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'ChapterComplete_faces_R8.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
