from pathlib import Path
import sys,json,base64,hashlib,urllib.error
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt='Precise original-source UI atlas cleanup. Keep every asset at the exact input pixel location and exact size on this 1024x1024 grey canvas. Do not rearrange or rescale. Top wide strip at x20 y20 width941 height174 is a dark carved footer: remove BOTH circular controls including their full rings, icons and red notification badge, and remove the large central green Play button including text and gold frame. Repair those removed areas with continuous dark antique bronze carved scrollwork matching the intact original footer. Preserve every exposed existing carving pixel, original restrained dark tones and the thin outer boundary; do not add light bars or unrelated objects. At x20 y230 width400 height65 is a bronze leaderboard row: remove all portraits, badge, text, feather and numbers; retain its original thin gold perimeter and original dark bronze-black surface. At x20 y330 width400 height70 is a blue leaderboard row: remove all portrait, text, feather and numbers; retain original blue sapphire surface and thin gold perimeter. At x450 y230 width183 height164 is a circular blue/gold control: remove envelope, retain complete original circular rim and empty opaque sapphire face; remove surrounding dark carved background so ONLY the complete circle remains on flat grey. At x680 y230 width192 height166 is another circular blue/gold control: remove chest and red quantity badge, restoring the hidden circular rim and continuous blue face; remove carved background outside the circle. Bottom three small gold/silver/bronze rank medallions at x20,110,200 y480, each53x63: remove only the numerals and restore matching empty centre metal; keep exact outer ornate shield contours, remove their surrounding rectangular row backgrounds. All exterior outside separate sprites must be flat grey RGB48,48,48. Navy/sapphire and metal faces stay opaque. No text, numbers, badges, feathers, chests, portraits or envelopes remain. Preserve exact original dimensions and source style. Output the same atlas.'
try:
 if sys.argv[1]=='submit':
  raw=(b/'Guidance.png').read_bytes()
  if not(t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'EventDetail_R4_footer_leaders.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
  with(t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body});r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('R4 footer/leader repair accepted')
 elif sys.argv[1]=='collect':
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except urllib.error.HTTPError as e:
 t.save('AccessResult.json',{'httpStatus':e.code});print('HTTP',e.code,'no automatic retry');e.close();raise SystemExit(1)
