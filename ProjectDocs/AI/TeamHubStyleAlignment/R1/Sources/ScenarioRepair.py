"""One bounded portrait-cap repair request, no automatic paid retry."""
from pathlib import Path
import sys,json,hashlib,base64,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'ScenarioPortraitCaps';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
 raw=(b/'PortraitRepairInput.png').read_bytes()
 if not(t.OUT/'Reference.json').exists():
  a=t.api('/assets',{'name':'TeamHub_R1_five_portrait_cap_repairs.png','image':base64.b64encode(raw).decode()})
  t.save('Reference.json',{'assetId':a['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
 body={'prompt':'''Repair only the missing lower caps and gold rims of these FIVE existing Russian fairy-tale game portrait medallions. Retain the EXACT input layout on a1536x1024 canvas with six512x512 cells; leave the bottom-right cell empty. Top row Ivan (brown hair, young man), Olga (woman in blue gold crown), Svyatoslav (bearded man in metal helmet). Bottom row Velimir (older hooded bearded man), Lada (dark-haired woman). Keep the existing portrait positions, sizes, faces, hair, colors and antique-gold upper rims unchanged. Their lower edges have cutoffs and fragments of neighboring portraits. ONLY restore the small lower sections of collar/beard and complete each continuous thin gold oval medallion rim. Remove intruding neighbor portrait pieces and horizontal row bars. No extra objects, text, badges, embellishments or new faces. Keep each portrait within its current source bounding box and exact original scale, aligned with its original upper rim. Use flat charcoal outside the five medallions. This is a conservative missing-edge repair, not new character design.''','referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1536,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
 with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False},f)
 t.save('Request.json',{'endpoint':'/generate/custom/'+t.MODEL,'model':t.MODEL,'body':body,'reason':'Original source portrait bottoms are occluded by neighboring roster portraits; full rims cannot be extracted. Only bottom repair regions will be composited; original faces and remaining RGB retained.'})
 r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Portrait-cap repair accepted',flush=True)
else:
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
