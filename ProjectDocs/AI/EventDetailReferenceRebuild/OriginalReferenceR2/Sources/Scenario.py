from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;job,action=sys.argv[1:3];assert job in ['isolate','firebird','surfaces']
t.OUT=b/'Scenario'/job;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2' if job=='surfaces' else 'model_pixa-background-removal'
try:
 if action=='submit':
  p=b/({'isolate':'IsolateGuidance','firebird':'FirebirdGuidance','surfaces':'SurfaceGuidance'}[job]+'.png');raw=p.read_bytes()
  if job=='surfaces':
   if not(t.OUT/'Reference.json').exists():
    r=t.api('/assets',{'name':'EventDetail_R2_empty_surfaces.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
   prompt='''Repair this UI atlas in place, preserving every asset's exact bounding box, position, size, original colours and visible outer gold frames. The canvas is grey. At the top is a navy rectangular milestone panel: REMOVE every chest, icon, inner reward-card frame, number, gold progress bar and all text, leaving ONLY its original thin ornate outer gold rectangle with continuous empty dark navy lacquer inside. Below it the very wide short objective panel: remove all text, feather, count and small inset frame, leaving its outer gold frame and empty dark navy inside. Middle left leaderboard panel: remove all rows, portraits, lines, rank badges, numbers, letters and text; leave the outer gold frame and continuous navy interior. Middle right premium panel: remove all text, chests, gems, feather, shield, inner panels and buttons; preserve its outer frame and purple-black floral interior. Lower left large ornate title: remove ALL lettering, keep exact original ornate gold frame and sapphires and its navy backing, and remove any bird feather/scenery fragments attached outside. At bottom large green button and smaller gold button: remove their lettering only, preserve exact shapes and engravings and complete original green/gold faces. No new decorations, no objects, no lettering, no numbers or icons remain in any face. Do not move or resize anything. No full-screen UI, no redesigned kit. Output this same sheet with each original asset now empty and independent.'''
   body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1}
  else:body={'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'}
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'input':str(p),'sha256':hashlib.sha256(raw).hexdigest(),'body':body if job=='surfaces' else {'outputFormat':'rgba','image':'[local image bytes omitted]'}})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print(job,'accepted')
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(job,'HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
