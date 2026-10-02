"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Reconstruct a clean environment plate from this exact Russian fairy-tale Loading screen. Preserve the original dusk village composition and visible architecture. Remove all UI: gigantic logo/plaque at top, outer gold/sapphire border, parchment tip panel with text, loading label and progress bar, all numbers and decoration. Remove the man walking away on the LEFT, his full body and red cloak, and the standing golden Firebird on the RIGHT with every tail feather. Do not replace with other people or animals. Reconstruct hidden scenery only. Edge-to-edge portrait941:1672: birch trunks on far left and dark gold autumn foliage on both top sides; deep indigo sky with peach-gold clouds, a glowing distant village church near x60percent y29percent and a windmill at x74percent y27percent; small dark timber houses with amber windows on middle/right hillsides; winding stone path from bottom center towards the distant village, low dark grasses and foliage, scattered golden motes. Keep the painterly source colors and perspective. NO logos, letters, words, numbers, panels, frames, borders, UI, symbols, people, faces, figures, birds or silhouettes. Upper sky continues naturally through the removed logo; the foreground path and plants continue through removed subjects and tip panel. This will sit underneath independent exact original source sprites; never create a full page design."""

try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Loading_R1_hidden_village_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
