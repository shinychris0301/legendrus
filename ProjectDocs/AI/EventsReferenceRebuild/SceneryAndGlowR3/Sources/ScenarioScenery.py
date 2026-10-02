"""Bounded source-guided repairs; existing foreground sprites are never regenerated."""
from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;job,action=sys.argv[1:3];assert job in ['cards','heading']
t.OUT=b/'Scenario'/job;t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompts={
'cards':'''Edit this exact 2x2 sheet by removing ONLY the four foreground subjects and inpainting their occupied pixels. Preserve visible background architecture, mountains, tree trunks, skyline, camera framing, color palette and light direction as closely as possible. Do not replace these scenes with different compositions. Return exactly four rectangular scenery-only plates in the same 2x2 arrangement, no gaps, frames or margins. Top-left remove the entire Firebird, ALL feathers, flame trails, golden sparks and fiery glow. Reconstruct the dark Russian onion-domed city and violet-orange dusk behind it. This plate should have twilight highlights only, not a firestorm or washed-out orange day. Top-right remove Baba Yaga and her entire foreground body, staff, cauldron, pot and smoke; retain the original eerie blue-green woods, original hut on chicken legs at the RIGHT and moonlight. Bottom-left remove the merchant and all foreground clothing/props, reconstruct the Russian harbor around the original large ship on the RIGHT and distant golden-domed skyline. Bottom-right remove the knight, helmet, body, sword and shield, retaining the exact pale icy castle composition. No humans, creatures, birds, tools, floating objects, text, UI frames or medallions in any plate. Do not add new characters. Painterly Russian fairy-tale style from the supplied image. These are clean environment layers behind separately preserved original characters, not finished UI cards.''',
'heading':'''Remove all interface elements from this wide Russian fairy-tale skyline fragment and reconstruct the scenery they hide. Preserve the exact visible city silhouette, violet and cobalt starry night sky, dark birch branches at both sides, onion domes, warm windows, and positions/scale of the visible towers. Do not create a different city or change the camera. Remove the large gold title plaque, all lettering, the player portrait and coat of arms on the left, top resource bar remnants, bottom tab strips and every UI ornament. Fill those occupied areas coherently with the same uninterrupted night sky and distant city. The skyline remains low in the image, sky above. No foreground characters, logos, text, borders, scrollwork, badges, buttons, panels or user interface. Output one clean wide landscape plate filling the entire canvas. Keep the original dark navy/violet palette and local sunset on the horizon, not bright orange sunset covering the full image.'''}
try:
 if action=='submit':
  p=b/('SceneryGuidance.png' if job=='cards' else 'HeadingGuidance.png');raw=p.read_bytes()
  if not(t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Events_R3_'+job+'_scenery_guidance.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompts[job],'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':768 if job=='cards' else 384,'quality':'medium','background':'opaque','numOutputs':1}
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Reconstruct occluded scenery only; original subject art and UI remain separate.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid,'Recover job, never resubmit';t.save('Job.json',{'id':jid,'status':j.get('status')});print(job,'accepted')
 elif action=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print(job,'HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
