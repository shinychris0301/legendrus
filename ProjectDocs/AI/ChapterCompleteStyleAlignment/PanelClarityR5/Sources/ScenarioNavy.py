"""One authorized Scenario material reconstruction; atomic submission, resumable collection."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/PanelClarityR5';trial.OUT=base/'Sources/ScenarioNavy';trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
 p=base/'Sources/NavyReference.png'
 if not (trial.OUT/'Reference.json').exists():
  res=trial.api('/assets',{'name':'ChapterComplete_NavyLacquer_Reference.png','image':base64.b64encode(p.read_bytes()).decode()});trial.save('Reference.json',{'assetId':res['asset']['id'],'source':str(p.relative_to(r)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
 prompt='''Use case: precise-object-edit. Reconstruct a clean dark navy lacquer material from the small supplied original material sample for a Russian fairy-tale UI. Expand the same very dark blue-black navy surface into one full square texture, flat front-facing consistent illumination. Subdued natural mottling and fine painterly lacquer texture only. Preserve the original muted blue-black palette; extremely low contrast so gold text and reward icons remain clear. No ornamental pattern, no checkerboard, no repeated mirrored patches, no tiles, no grain grid, no stripes, no seam, no geometry, no gold, no border, no frame, no icon, no letters, no text, no scenery. This is only an opaque material fill used inside existing original gold frames, not a new panel or screen. Fine irregular unobtrusive texture, restrained broad tonal variation; no dramatic glow or vignette.'''
 body={'prompt':prompt,'referenceImages':[json.loads((trial.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
 trial.save('Request.json',{'modelId':trial.MODEL,'body':body})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,body)
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover accepted request; do not resubmit';trial.save('Job.json',{'id':jid,'status':job.get('status')});print('Navy job accepted')
elif sys.argv[1]=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError('submit or collect')
