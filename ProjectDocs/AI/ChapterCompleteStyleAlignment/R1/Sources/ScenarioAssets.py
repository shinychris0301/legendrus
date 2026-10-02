"""Scenario only: source-alpha extraction and one independent clean scenery plate."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/R1';name,action=sys.argv[1:];assert name in ('header','banner','foreground','next','map','environment')
trial.OUT=base/'Sources/Scenario'/name;trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_openai-gpt-image-2' if name=='environment' else 'model_pixa-background-removal'
if action=='submit':
 if name=='environment':
  p=r/'Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/ChapterComplete.png'
  reference=trial.OUT/'Reference.json'
  if not reference.exists():
   res=trial.api('/assets',{'name':'ChapterComplete_R1_Reference.png','image':base64.b64encode(p.read_bytes()).decode()});trial.save('Reference.json',{'assetId':res['asset']['id'],'source':str(p.relative_to(r)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
  prompt='''Use case: precise-object-edit. Asset: one clean opaque environment plate for a Unity Russian fairy-tale chapter completion page. Input image is the approved Chapter Complete reference. Reconstruct ONLY its sunset Russian village scenery behind the foreground man, firebird and UI. Remove every person, bird, title, decorative border, ribbon, stars, reward panel, button, teaser and text. Fill the occluded areas with coherent village scenery. Preserve its painterly realism, warm golden-orange sunset through birch trees, dark wooden cottage with lit windows on the left, Russian onion-domed church on the distant right, low houses and rustic fence to the right, meadow and narrow village path in the lower center. Broad open amber sky in upper middle, softly layered amber clouds, leafy trees framing sides. Main horizon around midheight, cottages occupy lower half. Controlled rich gold light and dark blue-green shadows; no modern buildings, no fantasy city skyline. Full-bleed landscape composition of the scenery section, not a poster. No interface, no frame, no text, no actor, no bird. New independent clean background, not a screenshot with covered UI.'''
  body={'prompt':prompt,'referenceImages':[json.loads(reference.read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  trial.save('Request.json',{'modelId':trial.MODEL,'body':body})
 else:
  p=base/'Sources/Crops'/f'{name}.png';body={'image':'data:image/png;base64,'+base64.b64encode(p.read_bytes()).decode(),'outputFormat':'rgba'}
  trial.save('Request.json',{'modelId':trial.MODEL,'source':str(p.relative_to(r)),'outputFormat':'rgba','use':'alpha only with original source RGB'})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,body)
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover accepted job; do not resubmit'
 trial.save('Job.json',{'id':jid,'status':job.get('status')});print(name,'accepted',flush=True)
elif action=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError(action)
