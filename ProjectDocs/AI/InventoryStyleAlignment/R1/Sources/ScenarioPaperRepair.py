from pathlib import Path
import sys,json,base64,hashlib
sys.path.insert(0,str(Path.cwd()/'Tools'));import scenario_trial as t
s=Path(__file__).resolve().parent;t.OUT=s/'ScenarioPaperRepair';t.OUT.mkdir(exist_ok=True);t.MODEL='model_openai-gpt-image-2';action=sys.argv[1]
if action=='submit':
 p=s/'Crops/paper.png';raw=p.read_bytes()
 if not(t.OUT/'Reference.json').exists():
  result=t.api('/assets',{'name':'Inventory_original_parchment_repair.png','image':base64.b64encode(raw).decode()});t.save('Reference.json',{'assetId':result['asset']['id'],'localFile':str(p),'sha256':hashlib.sha256(raw).hexdigest()})
 prompt='''Repair this single existing parchment UI panel. Preserve its original rectangular outline, original antique-gold thin corner frame, front view, original warm muted tan paper color, natural fine parchment fibers and gently aged margins. Remove ALL text, letters, numbers, underline, two buttons, button frames and any other content INSIDE the panel, replacing only those areas with continuous clean blank parchment. Keep the outer gold perimeter frame intact. Entire interior is empty opaque paper, no pale erased lettering, no ghost characters, no broad paint blobs, no artificial repeated patterns, no light patches, no new ornaments. Match the input paper shade and subtle grain precisely. Output one panel filling the canvas in the same orientation and relative proportions; no margin, no scene, no new design. This is a bounded repair of covered paper, not redesigning or recoloring the frame.'''
 body={'prompt':prompt,'referenceImages':[json.loads((t.OUT/'Reference.json').read_text())['assetId']],'width':1536,'height':1024,'quality':'medium','background':'opaque','numOutputs':1}
 with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'automaticResubmission':False,'authorizedPurpose':'Repair unrecoverable paper behind lettering and buttons; only bounded interior enters runtime'},f)
 t.save('Request.json',{'modelId':t.MODEL,'body':body});result=t.api('/generate/custom/'+t.MODEL,body);job=result.get('job',result);jid=job.get('jobId')or job.get('id');assert jid;t.save('Job.json',{'id':jid,'status':job.get('status')});print('One bounded paper repair accepted')
elif action=='collect':
 t.poll()
 if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
