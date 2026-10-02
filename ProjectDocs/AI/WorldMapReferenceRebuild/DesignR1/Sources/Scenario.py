from pathlib import Path
import sys,json,hashlib,base64,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;name,action=sys.argv[1:3];t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True)
extract=name in ['title','title_full','navigation'];t.MODEL='model_pixa-background-removal' if extract else 'model_openai-gpt-image-2'
try:
 if action=='submit':
  if extract:
   p=b/'Inputs'/(name+'.png');raw=p.read_bytes();body={'image':'data:image/png;base64,'+base64.b64encode(raw).decode(),'outputFormat':'rgba'};record={'imageSource':str(p.relative_to(root)),'sha256':hashlib.sha256(raw).hexdigest(),'outputFormat':'rgba'}
  else:
   reference=b/'ReferenceUpload.json'
   if not reference.exists():
    p=b/'OriginalReference.png';r=t.api('/assets',{'name':'WorldMap_original_style.png','image':base64.b64encode(p.read_bytes()).decode()});reference.write_text(json.dumps({'assetId':r['asset']['id'],'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}))
   chapters=json.loads((b/'Chapters.json').read_text());batch=int(name[-1]);group=chapters[(batch-1)*4:batch*4]
   descriptions='\n'.join(f'ZONE {i+1}, vertical band {i*25}-{(i+1)*25}%: {x["en"]}. {x["theme"]}. Story setting: {x["narrative"]}' for i,x in enumerate(group))
   prompt='''Use case: stylized-concept. Asset: ONE vertical painted adventure world-map terrain strip, 1024x1536, made of FOUR vertically stacked contiguous REGIONS, not a finished interface. Source image is STYLE REFERENCE ONLY for painterly Russian fairy-tale landscapes, realistic miniature wooden villages, birches, deep pine forests, antique gold sunset light, dark navy shadows and winding river geography. Render from consistent high three-quarter aerial map view, close enough that each region is a distinct dramatic landmark, NOT distant panoramic scenery. Entire image filled with terrain; no sky or horizon. EXACTLY FOUR equal-height regions, one per vertical quarter, each with a large readable centerpiece alternating left/right. Soft organic wooded boundaries between regions, NO hard card frames or horizontal rules. Continuous narrow winding earth road connects all four regions from TOP to BOTTOM. Four landmarks must remain inside their own quarter with foliage at top/bottom edges. Keep the lowest 20% of each quarter subdued and uncluttered for separately overlaid native chapter name. Rich but controlled detail, warm lanterns, restrained highlights, legible silhouettes at phone size. NO UI, text, lettering, numbers, labels, emblems, nodes, gold dotted lines, circles, lock icons, panels, portraits, hero characters, frames, borders, watermarks. Treat narrative only as location guidance, do not render story scenes or characters. Regions top to bottom:\n'''+descriptions
   body={'prompt':prompt,'referenceImages':[json.loads(reference.read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1};record=body
  with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':record});r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'accepted',flush=True)
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(name,'HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
