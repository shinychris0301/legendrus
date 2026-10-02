from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'faces';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Precise object edit of this UI crop, keep the entire composition at EXACTLY the same coordinates. Remove ALL text, numbers, coin/gem/leaf/lightning icons, booster illustrations, video icon and x2, inner reward card frames, horizontal internal rules and small heading flourishes. Retain only the outer large navy panel, outer green button and outer blue button and background. Reconstruct completely empty continuous navy-black lacquer faces with subtle fine irregular painted stone/leaf texture, continuous emerald green button face, continuous deep sapphire blue button face. Preserve original light levels, navy near black, subdued texture with NO symmetric tiling, NO repeated patterns, NO letters, NO symbols, NO new ornaments, NO gradients into other colors. Do not move or resize panels or buttons. This repair supplies blank interior material only; all original exterior gold carving will be restored by compositing. Keep texture varied but quiet and sharp, never blurred or mottled. No extra objects. No text anywhere.'
try:
 if sys.argv[1]=='submit':
  p=b/'FaceRepairInput.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Victory_bounded_faces_R2.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
