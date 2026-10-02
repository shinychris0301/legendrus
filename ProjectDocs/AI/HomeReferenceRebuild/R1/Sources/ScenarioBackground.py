"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Reconstruct ONLY the clean background environment from the supplied Russian fairy-tale HOME reference. Retain visible dusk town architecture, deep navy starry sky, peach-orange sunset near mid-height, birches at both side edges, dark old wooden house and stairs at left, golden onion-domed churches at right, lanterns and stone path at bottom. Preserve source color palette and perspective. Remove ALL UI, top resource header and portrait, the massive logo, Ivan man and red cloak, golden Firebird and entire tail, green Play button, four navigation cards, lower wood quest panel, mail and chest circles and frames, all lettering and numbers. Reconstruct scenery hidden behind them. NO figures, faces, animals, birds, text, logos, panels, buttons, badges, borders, interface or frames anywhere. Upper area is navy sky between edge birches; lower area is quiet dark stone path and low foliage. Full bleed portrait 941:1672 composition, source painterly Russian-fairy-tale style. Independent original foreground sprites will be layered on this environment; do not create a finished game screen."""

try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Home_R1_hidden_village_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
