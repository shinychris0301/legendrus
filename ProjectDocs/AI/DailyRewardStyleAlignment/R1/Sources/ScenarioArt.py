from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=root/'ProjectDocs/AI/DailyRewardStyleAlignment/R1/Sources'
kind,action=sys.argv[1:3];t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True)
t.MODEL='model_pixa-background-removal' if kind.endswith('_alpha') or kind=='chest' else 'model_openai-gpt-image-2'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
if action!='submit':raise SystemExit('submit or collect')
if kind.endswith('_alpha'):source=b/'Scenario'/kind[:-6]/'booster_medallion_trial01.png'
elif kind=='environment':source=root/'Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png'
else:source=b/(kind+'_source.png')
if t.MODEL=='model_pixa-background-removal':
 body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
 record={'model':t.MODEL,'source':str(source.relative_to(root)),'outputFormat':'rgba'}
else:
 if not (t.OUT/'Reference.json').exists():
  result=t.api('/assets',{'name':'DailyReward_'+kind+'_source.png','image':base64.b64encode(source.read_bytes()).decode()})
  t.save('Reference.json',{'assetId':result['asset']['id'],'source':str(source.relative_to(root))})
 prompts={
 'header':('Precisely edit this single horizontal Russian fairy tale title plaque. Remove ALL lettering from the navy center, leaving continuous empty midnight navy lacquer. Preserve the gold frame, sapphire crest, silhouette, ornamental shapes and colors exactly. Remove all scenery and any small remnants outside the plaque. Isolated plaque only on pure black exterior. Opaque navy center. No letters, numbers, checkerboard, shadows outside frame, people or other UI. Front view, centered, entire ornament fits with 4 percent margin.',1536,512),
 'card':('Precisely edit this single narrow tall daily reward card: remove ALL lettering, day number, gemstone, quantity, green checkmark and all other content. Preserve the rectangular antique gold frame, delicate geometric corner decorations, dark navy black lacquer face and two slender horizontal gold separators near 19 percent and 87 percent of height. Restore continuous empty navy texture in removed areas. Exactly one blank card, no other objects. Isolated on pure black exterior with small margin. Keep opaque center and straight original proportions; no text, no glyphs, no scenery, no checkerboard.',768,1280),
 'environment':('Create a clean empty background plate matching ONLY the scenery behind the interface of this reference: dark Russian fairy tale carved timber hall, antique wooden pillars at both sides, dim warm amber lantern illumination, shadowed dark blue stone floor toward bottom, painterly realistic game illustration. Preserve its subdued navy/black and warm brown palette. Remove EVERY UI element, portrait, person, frame, panel, gem, coins, chest, text, number, symbol, button and progress bar. Reconstruct continuous believable architecture where UI was. Dark unobtrusive central area for separately layered UI. Full bleed portrait environment only, no transparent areas, no bright focal object.',1024,1792)}
 prompt,w,h=prompts[kind];body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':w,'height':h,'quality':'medium','background':'opaque','numOutputs':1};record={'model':t.MODEL,'body':body}
t.save('Request.json',record)
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job, no resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
