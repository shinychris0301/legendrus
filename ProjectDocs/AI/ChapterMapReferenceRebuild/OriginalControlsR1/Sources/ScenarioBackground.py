"""One generation for scenery hidden behind the original subjects; never redraw subjects."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
root=Path.cwd();sys.path.insert(0,str(root/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario/backgrounds';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Edit the supplied original Chapter Map image into a clean background plate, with maximum fidelity to its ORIGINAL visible scenery. This is interface removal, NOT a new painting or a redesign. Preserve camera, viewpoint, architecture, light, colors, forest edge, clouds, cobblestone road and river exactly where shown. Remove ONLY all interface overlays: top resource header and portrait, the red chapter title banner with all its gold ornaments, all seven navy round level badges and numbers 10-16, every gold filled/empty star, the bottom green Play button, round Back arrow and bottom carved UI border. Reconstruct the scenery hidden under those overlays naturally. No text, icons, numbers, medallions, frames, UI, circles, stars, borders, buttons or portraits anywhere in the output.
Composition is critical, normalized to source 941x1672: onion-domed blue-and-gold church on LEFT at x25percent y34percent, its large dome at y29percent and church entrance y41percent; tall wooden windmill on upper-middle RIGHT at x64percent y28percent, sails at y24percent; dark birch trunks at far left and foliage along both edges; grey-blue cloudy dusk sky with pale warm opening on upper right, NOT a broad orange sunset horizon; small wooden house x11percent y43percent, large house x11percent y62percent, carved timber house x86percent y48percent, large ornate house x84percent y69percent; stream descends from x71percent y40percent to x75percent y59percent; stone winding road descends from windmill through x48percent y43percent, x43percent y50percent, x34percent y60percent, x53percent y69percent, x33percent y75percent, x45percent y83percent to bottom center. Retain natural photographic-painterly dark forest tones and warm window lamps. Buildings are NOT blue recolored substitutes. Absolutely no layout rearrangement, extra church, large central sun, new fortress or new camera. Output one edge-to-edge portrait environment, not a designed game screen. Keep all surviving source scenery as close as possible; fill only places hidden by removed interface."""

try:
 if sys.argv[1]=='submit':
  p=b/'OriginalReference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'ChapterMap_R1_original_scenery_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original scenery hidden behind extracted subjects cannot be recovered from source pixels; generate only clean plates, retain original foreground.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Clean backgrounds accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 print('Scenario HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)

except Exception as e:
 print(type(e).__name__,"Scenario operation failed; no automatic resubmission");raise SystemExit(1)
