"""One bounded portrait-cap repair request, no automatic paid retry."""
from pathlib import Path
import sys,json,hashlib,base64,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'ScenarioPortraitCaps';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
 raw=(b/'PortraitRepairInput.png').read_bytes()
 if not(t.OUT/'Reference.json').exists():
  a=t.api('/assets',{'name':'IndividualRanking_R3_three_portrait_cap_repairs.png','image':base64.b64encode(raw).decode()})
  t.save('Reference.json',{'assetId':a['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
 body={'prompt':'''Restore this exact horizontal strip of THREE small game portrait medallions, in exactly the same order, positions, scale and geometry as the supplied image. Canvas 1536 by512. The first medallion is the brown-bearded fierce Slavic forest robber with gold cap; the second is the long green-bearded water spirit; the third is the red multiheaded dragon. These are EXISTING portraits, not new designs. Each medallion occupies its same512x512 cell. Keep all original faces, eyes, expressions, hats, heads, colors and the dark blue interior unchanged. ONLY complete the missing lowest section below the straight cut: continue a SHORT natural section of the existing brown beard / green beard / dragon neck and dark clothing, and close the thin antique-gold circular rim at the bottom. Remove any neighboring portrait intrusions. No long necks, no extra faces, no added jewelry or bodies. Each portrait must become a complete clean round or slightly oval medallion with a continuous thin ornate antique-gold rim, matching its existing rim. Preserve original lighting and source painterly texture; no blur or vertical smears. Keep the exact cell layout; no rearrangement, zoom, enlargement, square frames, text, lettering, numbers or additional items. Use the same solid charcoal background outside the medallions. This is a conservative restoration of only the missing bottom caps. Everything above the cut must remain visually identical.''','referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1536,'height':512,'quality':'medium','background':'opaque','numOutputs':1}
 with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False},f)
 t.save('Request.json',{'endpoint':'/generate/custom/'+t.MODEL,'model':t.MODEL,'body':body,'reason':'Local cap extension produced visible streaking and is rejected. Only bottom repair regions will be composited; original faces and remaining RGB retained.'})
 r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Portrait-cap repair accepted',flush=True)
else:
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
