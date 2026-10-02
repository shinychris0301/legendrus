"""One authorized Scenario scenery reconstruction; atomic submission, resumable collection."""
from pathlib import Path
import sys,json,time,base64,urllib.error,hashlib
r=Path.cwd();sys.path.insert(0,str(r/'Tools'));import scenario_trial as trial
base=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/SceneryR4';trial.OUT=base/'Sources/ScenarioScene';trial.OUT.mkdir(parents=True,exist_ok=True);trial.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
 p=base/'Sources/SceneReference.png'
 if not (trial.OUT/'Reference.json').exists():
  res=trial.api('/assets',{'name':'ChapterComplete_SceneComposition_Reference.png','image':base64.b64encode(p.read_bytes()).decode()});trial.save('Reference.json',{'assetId':res['asset']['id'],'source':str(p.relative_to(r)),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()})
 prompt='''Use case: precise-object-edit. Edit target is a square crop of the approved Chapter Complete scene. Produce a CLEAN INDEPENDENT BACKGROUND PLATE at exactly the same camera framing, object scale and spatial composition. Remove the man, firebird, all gold stars, shield, weapons, red chapter ribbon, all lettering, title remnants, decorative side rails and the entire lower victory ribbon/footer remnants. Reconstruct the scenery hidden by those foreground items. No people, birds, animals, text, frame, metallic ornament, plaque, panel or interface anywhere in the result. Do NOT redesign or recompose the visible village. Keep the dark thatched cottage at the far LEFT, its roof apex near x=17% y=43%, lit windows around y=63%, and the blue/gold onion-domed church at the far RIGHT around x=87% y=42%. Preserve the small dark right-side house around x=90% y=69%, birch tree and warm foliage at the far left, the branching dark canopy across the upper RIGHT corner. Preserve the broad horizontal sunset sky and its cloud layers in the upper half, with a warm pale amber glow behind the removed man's head/shoulder region. Do not move the horizon upwards, enlarge the cottage, add a giant church, brighten the whole scene, or change the viewpoint. Fill the formerly occupied center with quiet coherent distant village/trees and a subtle narrow path through shadowed grass lower down. Painterly realism, same deep blue-green shadows and warm antique amber color balance as reference, not neon orange or overexposed yellow. Extend scenery naturally to all four edges after removing UI. Output only an opaque full-bleed square clean environment, no remnants of the UI or actors. Existing exact source foreground and interface will be independently overlaid later in Unity.'''
 body={'prompt':prompt,'referenceImages':[json.loads((trial.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
 trial.save('Request.json',{'modelId':trial.MODEL,'body':body})
 with (trial.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'time':time.time(),'automaticResubmission':False},f)
 try:res=trial.api('/generate/custom/'+trial.MODEL,body)
 except urllib.error.HTTPError as e:
  trial.save('AccessResult.json',{'httpStatus':e.code,'noAutomaticRetry':True});raise SystemExit('HTTP '+str(e.code))
 job=res.get('job',res);jid=job.get('jobId') or job.get('id');assert jid,'Recover accepted request; do not resubmit';trial.save('Job.json',{'id':jid,'status':job.get('status')});print('Scene job accepted')
elif sys.argv[1]=='collect':
 trial.poll()
 if json.loads((trial.OUT/'Status.json').read_text())['status']=='success':trial.download()
else:raise ValueError('submit or collect')
