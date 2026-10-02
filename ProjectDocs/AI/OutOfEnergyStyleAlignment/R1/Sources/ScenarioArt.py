from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;kind,action=sys.argv[1:3]
t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True)
generated=kind in ('panel','controls');t.MODEL='model_openai-gpt-image-2' if generated else 'model_pixa-background-removal'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
source=b/(kind+'_source.png')
if kind.endswith('_alpha'):source=b/'Scenario'/kind[:-6]/'booster_medallion_trial01.png'
if generated:
 if not (t.OUT/'Reference.json').exists():
  r=t.api('/assets',{'name':'OutOfEnergy_'+kind+'.png','image':base64.b64encode(source.read_bytes()).decode()});t.save('Reference.json',{'assetId':r['asset']['id']})
 prompts={
 'panel':'Edit the provided Russian fairy tale UI panel into a clean EMPTY reusable panel. Preserve precisely the entire upright antique gold outer frame, complete sapphire crest at TOP and small sapphire at BOTTOM, navy textured opaque surface, all corner curls, ONE smaller countdown rectangular gold frame in the middle-lower area and THREE wide stacked gold-outlined option-row frames below it. Remove EVERY text, number, energy emblem, close cross button, hourglass, friends icon, clapperboard icon, and ALL green/blue action buttons. Reconstruct dark navy texture under removed content. The inner frames must remain empty navy. Absolutely no lettering, symbols, icons or scenery. Preserve original frame geometry and positions. Entire panel including both sapphire ornaments fully inside canvas, with clear black margin on ALL sides, no clipped tips. Do not add frames or ornaments. Black exterior, opaque navy interior.',
 'controls':'Precisely edit this sheet of THREE Russian fairy tale UI assets, keep each in its ORIGINAL position and proportions on the square sheet. TOP CENTER: complete large round ornate gold energy medallion with luminous amber lightning bolt and red jewels. Remove ONLY the small overlapping lower-right number 0 circular badge entirely, repairing the uninterrupted medallion edge behind it. Keep the large original energy medallion unchanged otherwise. BOTTOM LEFT: existing emerald green faceted rectangular gold-bordered button; remove 99 and currency symbol, retain empty green face. BOTTOM RIGHT: original sapphire blue rectangular gold-bordered button; remove all Cyrillic lettering, retain empty blue face. Preserve exact materials, colors and shapes. No text or numbers anywhere, no scenery, no extra objects. Three separated assets, pure black exterior with ample margins, each fully visible.'}
 body={'prompt':prompts[kind],'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1280 if kind=='panel' else 1024,'quality':'medium','background':'opaque','numOutputs':1}
else:body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),**({'prompt':body['prompt']} if generated else {'outputFormat':'rgba'})})
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job, never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
