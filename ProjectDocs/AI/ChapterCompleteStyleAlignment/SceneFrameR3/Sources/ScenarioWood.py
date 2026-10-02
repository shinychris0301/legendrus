"""One authorized Scenario material reconstruction; atomic submission, resumable collection."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/SceneFrameR3';trial.OUT=base/'Sources/ScenarioWood';trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
 p=base/'Sources/WoodMaterialReference.png'
 if not (trial.OUT/'Reference.json').exists():
  res=trial.api('/assets',{'name':'ChapterComplete_DarkWood_MaterialReference.png','image':base64.b64encode(p.read_bytes()).decode()});trial.save('Reference.json',{'assetId':res['asset']['id'],'source':str(p.relative_to(r)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
 prompt='''Use case: precise-object-edit. Reconstruct ONE clean continuous dark antique wooden UI panel surface from the supplied small material sample. The sample is only the empty wood face of an approved Russian fairy-tale game panel. Match its very dark brown and charcoal walnut palette, restrained warm highlights, broad horizontal grain and painterly realism. Expand this exact material into a clean full square surface with natural non-repeating grain. Flat front-facing texture, consistent scale and subdued illumination, no perspective or bevel. Keep the wood dark enough for pale gold text to remain readable. No golden ornament, no border, no text, no symbols, no icons, no buttons, no seam grid, no mirrored repeated patches, no scenery, no character, no vignette. Full bleed opaque wood material only. This is a material layer placed behind independent live controls, not a page mockup.'''
 body={'prompt':prompt,'referenceImages':[json.loads((trial.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
 trial.save('Request.json',{'modelId':trial.MODEL,'body':body})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,body)
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover accepted request; do not resubmit';trial.save('Job.json',{'id':jid,'status':job.get('status')});print('Wood job accepted')
elif sys.argv[1]=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError('submit or collect')
