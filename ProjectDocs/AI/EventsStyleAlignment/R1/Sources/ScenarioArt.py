from pathlib import Path
import sys,json,base64
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;kind,action=sys.argv[1:3];t.OUT=b/'Scenario'/kind;t.OUT.mkdir(parents=True,exist_ok=True)
alpha=kind.endswith('_alpha');t.MODEL='model_pixa-background-removal' if alpha else 'model_openai-gpt-image-2'
if action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 raise SystemExit
source=b/'Scenario'/kind[:-6]/'booster_medallion_trial01.png' if alpha else b/(kind+'_source.png')
if alpha:body={'image':'data:image/png;base64,'+base64.b64encode(source.read_bytes()).decode(),'outputFormat':'rgba'}
else:
 if not (t.OUT/'Reference.json').exists():
  r=t.api('/assets',{'name':'Events_'+kind+'.png','image':base64.b64encode(source.read_bytes()).decode()});t.save('Reference.json',{'assetId':r['asset']['id']})
 prompts={
 'title':'Reconstruct this exact Russian fairy tale heading plaque with its sapphire crest, antique gold filigree, black navy lacquer and curled ends. Remove all lettering and restore the continuous empty navy face. Entire plaque isolated centered with generous black exterior margin on all sides. Preserve source silhouette, colors and ornament details. No text, scenery, icons, checkerboard, new symbols or clipped edges. Opaque navy face. Wide horizontal plaque.',
 'frame':'Reconstruct ONLY the empty broad rectangular event-card frame. Thin antique-gold double rails, small elaborate gold scroll corners, opaque continuous midnight navy textured lacquer face. Remove ALL text, illustration, firebird, icons, premium ribbon, timer, reward icons and green button. Empty navy interior from edge to edge. Entire thin frame visible with clear black margin around all four edges. No text or extra ornaments, no checkerboard, no vertical divisions. Keep source material and raised thin gold corners. Broad horizontal card, front view.',
 'firebird':'Paint the reference firebird event illustration as a clean standalone landscape illustration: spectacular golden orange Russian Firebird, turned to left, wings spreading gracefully with streaming ember tail, above a Russian fairy tale city at violet dusk. Closely preserve reference creature, intricate gold feathers, lighting and palette. Fill the complete rectangular canvas with coherent scenery. No UI, no text, no buttons, borders or icons. This is an individual illustrated event-card painting.',
 'yaga':'Paint the reference Baba Yaga event illustration as a clean standalone landscape illustration: elderly Slavic witch in mossy hood, hawk nose and silver hair, leaning over green glowing cauldron in moonlit enchanted forest, wooden hut on chicken legs behind to right. Closely preserve reference pose, face, materials and restrained dark teal emerald palette. Fill complete rectangular canvas with coherent scenery. No UI, text, borders, icons or buttons. Individual illustrated event-card painting.',
 'merchant':'Paint the reference merchant event illustration as a clean standalone landscape illustration: imposing bearded medieval Russian merchant with dark fur-trimmed embroidered coat and gold chains, looking to right, wooden treasure chest by his side, luminous golden Russian city and harbor behind. Closely preserve reference face, costume and dark antique gold colors. Fill complete rectangular canvas. No UI, text, borders, icons or buttons. Individual illustrated event-card painting.',
 'knight':'Paint the reference ice tournament illustration as a clean standalone landscape illustration: armored Slavic knight in dark sapphire and engraved gold armor and full-face helmet, holding large round blue gold snowflake shield, snowy pale blue Russian towers behind. Closely preserve source pose, armor, shield ornament and controlled cold blue gold palette. Fill complete rectangular canvas. No UI, text, borders, icons or buttons. Individual illustrated event-card painting.'}
 body={'prompt':prompts[kind],'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024 if kind in ['title','frame'] else 768,'height':384 if kind in ['title','frame'] else 512,'quality':'medium','background':'opaque','numOutputs':1}
t.save('Request.json',{'model':t.MODEL,'source':str(source.relative_to(root)),**({'prompt':body['prompt']} if not alpha else {'outputFormat':'rgba'})})
with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover existing job, never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(kind,'submitted',flush=True)
