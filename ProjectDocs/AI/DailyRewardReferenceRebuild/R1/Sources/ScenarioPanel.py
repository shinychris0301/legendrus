from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'Scenario'/'panel';t.OUT.mkdir(parents=True,exist_ok=True);t.MODEL='model_openai-gpt-image-2'
prompt="""Edit this exact atlas of SEVEN Russian fairy tale UI parts. Preserve every part's exact position, size, shape and ornate gold perimeter. Remove ALL text/numbers, chest and other reward icons, check marks, the embedded small green claim button in the TOP RIGHT card, and progress bars inside panels. Reconstruct the empty interior material coherently. Top left tall card: dark navy floral lacquer. Top middle wide card: luminous warm amber/gold embossed interior with subtle golden rays, no chest or objects. Top right tall card: deep emerald floral lacquer, clean empty face. Middle wide horizontal panel: navy floral lacquer, no icons/bar/badge/text. Next horizontal panel: navy floral lacquer with its original gold frame. Next emerald button: empty green face. Bottom title: blank navy face inside the original gold frame and sapphire crest. Keep opaque panel faces and original dividers where possible; exterior black. No new words, symbols, icons, rewards, borders or layout changes. Do not rearrange or resize parts. Result will be used ONLY under bounded repair masks; original perimeters restored separately."""
try:
 if sys.argv[1]=='submit':
  p=b/'Inputs/panel.png';raw=p.read_bytes()
  if not (t.OUT/'Reference.json').exists():
   r=t.api('/assets',{'name':'DailyReward_empty_surfaces_R1.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':r['asset']['id'],'sourceSHA256':hashlib.sha256(raw).hexdigest()})
  body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1024,'height':1536,'quality':'medium','background':'opaque','numOutputs':1}
  with (t.OUT/'SubmissionAttempt.json').open('x') as f:json.dump({'automaticResubmission':False},f)
  t.save('Request.json',{'model':t.MODEL,'body':body,'reason':'Original faces are occluded by sample lettering and icons; replace only bounded interior masks, retaining original gold and alpha.'})
  r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);jid=j.get('jobId')or j.get('id');assert jid;t.save('Job.json',{'id':jid,'status':j.get('status')});print('Background accepted')
 else:
  t.poll()
  if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
except Exception as e:print(type(e).__name__,'Scenario operation failed; no resubmission');sys.exit(1)
