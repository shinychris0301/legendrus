"""User-authorized continuous rim recreation; submit once, resume by polling."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/rim';t.OUT.mkdir(parents=True,exist_ok=True)
prompt='''Use case: precise-object-edit. Asset: ONE empty circular booster frame for Legends of Rus, matching the supplied original medallion.
The reference shows a slender antique-gold circular metal rim around a blue hammer. Recreate the COMPLETE continuous metal rim as one coherent new object: uninterrupted concentric gold rails, finely engraved dark bronze channel, small gold diamond ornaments on left and right, and small sapphire-blue diamonds in delicate gold settings at top and bottom. Preserve this reference's restrained proportions, gold tone, narrow band width and painterly game-art rendering. A perfect frontal circle, no perspective. Consistent warm gold metal over the whole circumference, natural upper-left lighting, smoothly continuous lower arcs with no cut seams, patches, missing sections, different-colored segments or joins. Small regular engravings, not chunky scrollwork. The rim must be narrow like the reference: central clear diameter approximately 82 percent of outer diameter. Leave a six-percent exterior margin and show all four ornaments completely.
Remove the hammer AND the blue inner face: this is a hollow ring with a completely empty transparent circular opening. Remove both green number/plus badges entirely and recreate the circular metal they obscure. No numbers, plus signs, text, icons, badges, glowing halos, loose shadows, background scenery or additional objects. Transparent exterior and transparent central opening. Exactly one ring, centered, square canvas. This full recreation deliberately replaces all the old ring metal rather than patching its lower corners.'''
try:
 if sys.argv[1]=='submit':
  raw=(b/'RimReference.png').read_bytes()
  if not(t.OUT/'Reference.json').exists():
   a=t.api('/assets',{'name':'SharedBooster_OriginalRim_Reference.png','image':base64.b64encode(raw).decode()})['asset']
   t.save('Reference.json',{'assetId':a['id'],'sha256':hashlib.sha256(raw).hexdigest(),'path':str(b/'RimReference.png')})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'numOutputs':1,'quality':'medium','background':'transparent'}
  t.save('Request.json',{'modelId':t.MODEL,'body':body})
  with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False},f)
  result=t.api('/generate/custom/'+t.MODEL,body);job=result.get('job',result);jid=job.get('jobId')or job.get('id');assert jid,'Recover existing job; never resubmit'
  t.save('Job.json',{'id':jid,'status':job.get('status')});print('Continuous rim submitted once.')
 elif sys.argv[1]=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 else:raise SystemExit('Use submit or collect')
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('Scenario HTTP',e.code,'response suppressed; no automatic retry');e.close();raise SystemExit(1)
