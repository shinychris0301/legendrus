from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;kind,action=sys.argv[1:3]
t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True)
generated=kind in ('panel','play');t.MODEL='model_openai-gpt-image-2' if generated else 'model_pixa-background-removal'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
source=b/(kind+'_source.png')
if kind.endswith('_alpha'):source=b/'Scenario'/kind[:-6]/'booster_medallion_trial01.png'
if generated:
 if not (t.OUT/'Reference.json').exists():
  r=t.api('/assets',{'name':'BoosterSelection_'+kind+'.png','image':base64.b64encode(source.read_bytes()).decode()});t.save('Reference.json',{'assetId':r['asset']['id']})
 prompts={
 'panel':'Precisely reconstruct ONLY the empty outer panel from this Russian fairy tale UI reference. Preserve the complete antique gold frame, top central sapphire crest, curved upper corners, thin horizontal gold divider with central sapphire just below the heading area, lower corner scrolls and bottom central sapphire ornament. Preserve original geometry, navy lacquer material and gold colors. Remove ALL lettering, all eight circular booster medallions, every number, plus button, toggle, info symbol, close button and large Play button. Reconstruct a seamless empty opaque navy textured face beneath removed elements. No inner cards or new decorations. Entire panel and every gold/sapphire tip fully visible with clear black margin on ALL four sides. No scenery, text, icons, checkerboard or clipped edges.',
 'play':'Reproduce this exact emerald green and antique gold Russian fairy tale Play button, front view. Remove ONLY the Cyrillic lettering from its center and reconstruct the continuous empty emerald lacquer surface. Remove exterior background scenery. Preserve the broad original proportions, curled gold endcaps, lower engraved trim, small sapphire top jewel and material colors. Exactly one isolated empty horizontal button centered within the canvas, clear black margin on all sides. No text, symbols or new ornamentation; full button including both pointed gold ends must be visible. Opaque emerald center, black exterior.'}
 body={'prompt':prompts[kind],'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536 if kind=='panel' else 384,'quality':'medium','background':'opaque','numOutputs':1}
else:body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),**({'prompt':body['prompt']} if generated else {'outputFormat':'rgba'})})
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job, never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
