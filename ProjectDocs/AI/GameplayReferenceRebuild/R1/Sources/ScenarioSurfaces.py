from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'surfaces';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='''Generate a material repair sheet for the supplied Russian fairy-tale game reference. Output a perfectly square 2 by 2 contact sheet of FOUR PLAIN SURFACE TEXTURES, each filling its entire square quadrant with no margins or separators. Upper left: the dark burgundy-red lacquer/velvet surface from the moves panel, blank, subtle mottled red texture, dark and restrained. Upper right: the warm light ochre parchment from the goals panel, blank, fine natural paper grain, evenly lit. Lower left: dark antique brown wood with horizontal fine irregular grain matching the bottom booster cards. Lower right: very dark brown wood with fine irregular horizontal grain matching the score panel. These are empty material textures ONLY, to repair text/icon removal behind live controls. NO text, numbers, shadows of letters, symbols, gems, metallic lines, frames, borders, ornaments, jewels, objects, icons, perspective or vignettes. Keep the same subdued original material colours. Four distinct plain square material patches, arranged precisely as specified.'''
try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Gameplay_original_material_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Occupied surface recovery: eradicate baked numeral shadows and repeated extraction texture; original exterior frames remain untouched.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
