from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Precise source-preserving sprite extraction and empty-surface repair. This 1024x1024 atlas is the edit target. Keep every object at its EXACT input pixel position and size. Do not redesign, rotate, rearrange, upscale, or relight. The top rectangle (x100 y20 width710 height343) contains an ORIGINAL firebird: remove ALL scenery, skyline, sky, title frame above, countdown plaque below, hourglass, text and horizontal bottom rail from that rectangle. Retain the ENTIRE original bird including the long LEFT curling tail down to y350, delicate separate feather strands, ALL three blue/turquoise eye patterns, red shaded feathers, feet and isolated gold starburst sparks. Preserve original feather contours; do not cut off the tail or punch holes in blue/red feathers. Do not invent new feathers. Behind and between feather strands must be flat canvas grey, RGB48,48,48. At x30 y420 width490 height63 is a separate ornate dark navy countdown plaque: retain its EXACT gold scrollwork and intentionally OPAQUE navy face; remove hourglass, text, numbers, claws, feather fragment above left and scenery outside the gold silhouette. Fill the text/hourglass holes with continuous original dark navy material. At x570 y420 width244 height84 is a feather balance panel: remove the feather and number ONLY, retain its thin complete gold border and opaque navy face. At x30 y570 width455 height131 is the premium reward inset: remove the chest, gem, feather, shield and numbers ONLY, preserve the complete original ornate gold perimeter and fill interior with continuous original dark navy lacquer. All exterior outside these sprites must be the same flat grey RGB48,48,48. No checkerboard. No new ornaments. Original artist's exact style, colors and shapes; keep all dimensions and placement. Output the same atlas with only the fully isolated bird and three complete empty original UI panels.'''
try:
 if sys.argv[1]=='submit':
  raw=(b/'Guidance.png').read_bytes()
  if not(t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'EventDetail_R3_source_extraction.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body});r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('R3 source extraction accepted')
 elif sys.argv[1]=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
