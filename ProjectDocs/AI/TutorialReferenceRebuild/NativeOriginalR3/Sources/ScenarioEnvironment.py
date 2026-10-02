from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'environment';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Reconstruct the clean environment painting behind this Russian fairy tale match-3 tutorial screen. Preserve the original camera and architectural composition: a dark carved wooden study with a tall arched leaded window on the left, candles and parchment scrolls at lower left, wooden desk and books on the right, and an open view to luminous blue and gold Russian palace domes at twilight on the right. Antique gold accents, deep navy night sky, warm candle lighting, detailed painterly game background. Remove ALL user interface: player portrait, resource header, text, title plaque, gems, board, parchment tutorial panel, section labels, reward tray, buttons, arrows, lower decorative UI frame and icons. Reconstruct plausible continuous room/environment in every hidden region, with a dark subdued central wall/desk so independent UI panels can be placed over it. No lettering, symbols, borders, coins, gems, rewards, user interface, figures or floating artifacts anywhere. This must be one clean full portrait environment plate, not another screenshot or mockup. Keep visible scenery faithful to the reference."""
try:
 if sys.argv[1]=='submit':
  p=b/'Reference.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'Tutorial_original_clean_environment.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False,'allowedRequests':1},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Reconstruct hidden scenery for the approved original Tutorial layout.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('One scenery repair accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
