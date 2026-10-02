"""One user-authorized Home shortcut sheet; record once and poll without resubmission."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/icons';t.OUT.mkdir(parents=True,exist_ok=True)
prompt='''Use case: style-transfer. Production game UI assets for Legends of Rus, Russian fairy-tale match-three game. Create a coordinated set of exactly THREE separate ornate navigation buttons in one horizontal row. The two supplied images show approved Home navigation button STYLE. Match their painterly dimensional antique gold metal, dark bronze bevels, fine gold edge filigree, midnight navy lacquer interiors and small sapphire crown. All three buttons have the SAME frontal square shield-shaped silhouette with chamfered corners, top central sapphire finial, bottom central tiny gold finial, thin inner rails and a completely BLANK navy caption plaque across the lower 22 percent. Preserve their material richness and warm rim lighting. Fit subject illustrations entirely ABOVE the caption plaque, with margins from the trim. No visible background or scenery beyond the button. No lettering or numerals anywhere.
LEFT button: a substantial polished antique-gold trophy cup with two elegant handles, laurel leaves at its base and a small sapphire in its stem, recognizably Ranking. CENTER button: a rich blue enamel guild shield with an embossed gold two-headed eagle and subtle gold scroll engraving, recognizably Team. RIGHT button: two friendly painterly Slavic adventurers shown as overlapping shoulder-up portraits, one brown-haired young man in a blue tunic and one auburn-haired young woman in a dark red cloak, recognizably Friends. No helmets hiding faces. These are distinct simple icons readable at 100-pixel height, not large detailed scenes.
Three separate matching square-framed tiles, equal size and aligned, occupying left middle right thirds of canvas, with ample transparent empty gaps between them. Transparent background outside each tile; their navy interior surfaces are fully opaque. Show every decorative tip completely without clipping. No drop shadow extending far outside the frames, no ambient scenery, no labels, no letters, no words, no badges or plus signs. This is an asset sheet, not a game screenshot. Leave minimum 40 pixels empty around all canvas edges and minimum 80 pixels empty between buttons. Output 1536 by 1024; center the row vertically, each button approximately 420 by 470 pixels.''' 
try:
 if sys.argv[1]=='submit':
  refs=[]
  for n in ['source_shop_blank','source_characters_blank']:
   p=b/(n+'.png');raw=p.read_bytes();dest=t.OUT/(n+'.json')
   if not dest.exists():
    a=t.api('/assets',{'name':'Home_Style_'+n+'.png','image':base64.b64encode(raw).decode()})['asset'];dest.write_text(json.dumps({'assetId':a['id'],'sha256':hashlib.sha256(raw).hexdigest(),'path':str(p)},indent=2))
   refs.append(json.loads(dest.read_text())['assetId'])
  body={'prompt':prompt,'referenceImages':refs,'width':1536,'height':1024,'numOutputs':1,'quality':'high','background':'transparent'}
  t.save('Request.json',{'modelId':t.MODEL,'body':body})
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  r=t.api('/generate/custom/'+t.MODEL,body);job=r.get('job',r);jid=job.get('jobId')or job.get('id');assert jid,'Recover existing job; do not resubmit'
  t.save('Job.json',{'id':jid,'status':job.get('status')});print('Home shortcut sheet submitted once.')
 elif sys.argv[1]=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
 else:raise SystemExit('Use submit or collect')
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('Scenario HTTP',e.code,'details suppressed; no automatic retry');e.close();raise SystemExit(1)
