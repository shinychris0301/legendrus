from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=root/'ProjectDocs/AI/RewardReceivedStyleAlignment/R1/Sources';kind,action=sys.argv[1:3]
t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2' if kind in ('panel','panel_repair') else 'model_pixa-background-removal'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
source=b/(kind+'_source.png')
if kind.endswith('_alpha'):source=b/'Scenario'/kind[:-6]/'booster_medallion_trial01.png'
if kind in ('panel','panel_repair'):
 if not (t.OUT/'Reference.json').exists():
  r=t.api('/assets',{'name':'RewardReceived_Panel_Reference.png','image':base64.b64encode(source.read_bytes()).decode()});t.save('Reference.json',{'assetId':r['asset']['id']})
 prompt='Precisely edit this Russian fairy tale reward panel. Retain ONLY the original complete outer antique gold filigree frame, prominent central sapphire crown ornament at the top, lower curled corners and opaque dark midnight navy textured center. Remove ALL contents inside the frame: lettering, treasure chest, glow, coins, gems, nine small reward tiles, green button, numbers and icons. Reconstruct a continuous empty dark navy lacquer face. Preserve the original shape, proportions, colors, slender engraved gold rails, curved shoulders and corner scrolls as closely as possible. Entire panel fits within the canvas with a small black exterior margin. Exactly one empty upright panel, front view. No new motifs, no UI contents, no text, no scenery, no checkerboard. Transparent isolation will be applied separately.'
 if kind=='panel_repair':prompt='Repair this existing isolated empty gold-and-navy panel ONLY: the central sapphire crest at the top has a clipped upper tip. Reconstruct its small complete pointed gold finial and keep it entirely inside the padded canvas. Preserve all existing frame artwork, its long side rails, corners, sapphire, navy opaque face, colors and proportions. Keep a CLEAR 6 percent black margin on ALL four sides. The entire ornament must be fully visible, never touching the image boundary. Exactly one empty panel, no text, icons, scenery or other changes. Output the complete panel centered on pure black exterior.'
 body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1280,'quality':'medium','background':'opaque','numOutputs':1}
else:body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),**({'prompt':body['prompt']} if kind in ('panel','panel_repair') else {'outputFormat':'rgba'})})
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job, do not resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
