from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=root/'ProjectDocs/AI/NavigationStyleAlignment/R1/Sources';kind,action=sys.argv[1:3]
t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2' if kind=='header' else 'model_pixa-background-removal'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
source=b/('header_source.png' if kind=='header' else 'icons_source.png')
if kind=='header_alpha':source=b/'Scenario/header/booster_medallion_trial01.png'
if kind=='header':
 if not (t.OUT/'Reference.json').exists():
  r=t.api('/assets',{'name':'NavigationHeaderSource.png','image':base64.b64encode(source.read_bytes()).decode()});t.save('Reference.json',{'assetId':r['asset']['id']})
 prompt='Edit this exact narrow horizontal game resource header. Remove the large portrait and its frame on the left, all lettering, all numbers, progress bar, coins, gems, lightning, plus buttons, gear button and their small capsules. Preserve and reconstruct ONLY the long outer antique gold frame and opaque dark midnight navy lacquer face, including the original delicate curled end ornaments. Match the source dark colors and slim frame; no new motifs. Continuous blank surface for separately layered live text and icons. Entire isolated very wide thin horizontal header, fully visible, front view. Small pure black exterior margin, opaque navy inside. No text, icons, portrait, scenery or checkerboard. Width to height approximately 7.2 to 1.'
 body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1536,'height':256,'quality':'medium','background':'opaque','numOutputs':1}
else:body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),**({'prompt':body['prompt']} if kind=='header' else {'outputFormat':'rgba'})})
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job; never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
