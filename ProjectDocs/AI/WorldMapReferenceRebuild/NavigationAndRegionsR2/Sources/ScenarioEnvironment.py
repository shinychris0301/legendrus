from pathlib import Path
import sys,json,base64,hashlib
root=Path.cwd();sys.path.insert(0,str(root/'Tools'))
import scenario_trial as t
b=Path(__file__).resolve().parent;t.OUT=b/'ScenarioEnvironment';t.OUT.mkdir(exist_ok=True)
t.MODEL='model_openai-gpt-image-2'
if sys.argv[1]=='submit':
    src=b/'ChapterSelectReference.png'
    ref=t.OUT/'Reference.json'
    if not ref.exists():
        r=t.api('/assets',{'name':'ChapterSelect_OriginalEnvironmentReference.png','image':base64.b64encode(src.read_bytes()).decode()})
        t.save('Reference.json',{'assetId':r['asset']['id'],'sha256':hashlib.sha256(src.read_bytes()).hexdigest()})
    prompt='''Edit the supplied portrait Russian fairy-tale chapter-selection reference into its CLEAN BACKGROUND PLATE only. Preserve the visible surrounding scenery and exact overall placement: black birch trunks on left, twilight purple-orange sky near top, distant golden-domed city on right at upper and lower heights, dark wooden buildings on left, winding stone lane toward bottom center, dim foliage. Reconstruct only the scenery hidden behind UI. REMOVE ALL UI: player portrait, top resource strip, title, every rectangular chapter card including the landscapes WITHIN cards, buttons, bottom medallions, lettering, numbers, icons, all gold panel frames and carved black footer. Those UI elements will be original extracted separate sprites. No figures, interface, words, symbols, overlays, floating ornaments or poster composition. Full bleed painterly environment matching reference color, architecture scale, twilight lighting and perspective. Keep center subdued so separate UI stays legible; no large new foreground landmark. This is a clean environmental reconstruction, not a redesigned screen.'''
    body={'prompt':prompt,'referenceImages':[json.loads(ref.read_text())['assetId']],'width':1024,'height':1792,'quality':'medium','background':'opaque','numOutputs':1}
    with(t.OUT/'SubmissionAttempt.json').open('x')as f:json.dump({'maximumPaidSubmissions':1,'automaticResubmission':False},f)
    t.save('Request.json',{'model':t.MODEL,'body':body});r=t.api('/generate/custom/'+t.MODEL,body);j=r.get('job',r);t.save('Job.json',{'id':j.get('jobId')or j.get('id'),'status':j.get('status')});print('Clean environment accepted')
else:
    t.poll()
    if json.loads((t.OUT/'Status.json').read_text())['status']=='success':t.download()
