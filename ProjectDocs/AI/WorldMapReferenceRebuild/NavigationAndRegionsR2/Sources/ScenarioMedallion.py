from pathlib import Path
import sys, json
root = Path.cwd()
sys.path.insert(0, str(root / 'Tools'))
import scenario_trial as t
b = Path(__file__).resolve().parent
t.OUT = b / 'ScenarioMedallion'
t.OUT.mkdir(exist_ok=True)
t.MODEL = 'model_openai-gpt-image-2'
if sys.argv[1] == 'submit':
    reference = json.loads((b.parent.parent / 'DesignR1/Sources/ReferenceUpload.json').read_text())
    prompt = '''Create one isolated blank chapter-number medallion for this Russian fairy-tale world map. Original map supplied as style reference only. A small dignified antique-gold circular cartouche with slender engraved leaf filigree, four balanced small ornamental points and one restrained sapphire jewel at top. Front view, symmetry, no perspective. Large EMPTY OPAQUE midnight-navy lacquer center occupying 65 percent of diameter, for separately rendered live chapter numerals. Subdued warm gold, bronze shadows, readable silhouette at 100 pixels, not shiny yellow plastic. Full ornament fits with 8 percent transparent margin. Exterior genuinely transparent, center opaque. No number, lettering, icon, scenery, checkerboard, extra objects, glow, shadow rectangle or interface. Match existing game's antique gold and navy materials.'''
    body = dict(prompt=prompt, referenceImages=[reference['assetId']], width=1024, height=1024,
                quality='medium', background='transparent', numOutputs=1)
    with (t.OUT / 'SubmissionAttempt.json').open('x') as f:
        json.dump({'maximumPaidSubmissions': 1, 'automaticResubmission': False}, f)
    t.save('Request.json', {'model': t.MODEL, 'body': body})
    r = t.api('/generate/custom/' + t.MODEL, body)
    j = r.get('job', r)
    t.save('Job.json', {'id': j.get('jobId') or j.get('id'), 'status': j.get('status')})
    print('One chapter medallion submitted to Scenario; request recorded.')
else:
    t.poll()
    if json.loads((t.OUT / 'Status.json').read_text())['status'] == 'success':
        t.download()
