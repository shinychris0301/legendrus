"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;name,action=sys.argv[1:3];t.OUT=b/'Scenario'/name;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompts={
 'town': "Reconstruct ONLY the empty background behind the portrait in the reference. A restrained dark blue medieval Russian fairy tale town at twilight, tiny amber windows, slender timber towers and distant domes at the sides, muted cool indigo blue sky above, subtle warm specks of light and shadowy buildings below. Match the visible original scenery palette and small architectural scale. This is a clean independent background plate for a separate character card sprite. REMOVE the man, all face, hair, red cloak, clothes and jewelry entirely. REMOVE every UI border, gold ornament, star, blue icon and all text. No people, silhouettes, creatures, birds, UI, symbols, letters or frame. No bright foreground building or large central palace. Edge-to-edge painterly background with open center and subdued detail; preserve atmospheric soft focus rather than sharp photographic detail. Portrait 192:202 composition, no panels or layout sheets.",
 'forest': "Reconstruct ONLY the empty dark woodland background behind the portrait in the reference. Ancient Russian fairy tale forest at dusk, muted charcoal blue and dark brown tangled tree trunks, dim olive undergrowth, subtle tiny warm amber light specks deep in the wood, restrained texture. Match the dark original scene and small environmental scale. REMOVE the old woman, all face, hair, clothing and necklace entirely. REMOVE all card borders, gold filigree, stars, crossed swords, icons and words. No people, silhouettes, creatures, birds, skulls, faces, UI, symbols, lettering or frame. Full edge-to-edge clean independent forest background plate for a separate portrait sprite, open center, atmospheric painterly realism, no bright moon or vivid green foliage. Portrait 192:202 composition, no panels or layout sheets."
}
prompt=prompts[name]
try:
 if action=='submit':
  p=b/'Inputs'/(name+'.png');raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Characters_R12_'+name+'_background_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1088,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print(name,'background accepted',flush=True)
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
