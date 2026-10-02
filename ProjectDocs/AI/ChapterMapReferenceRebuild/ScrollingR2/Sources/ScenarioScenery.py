from pathlib import Path
import sys, json, hashlib, base64, urllib.error

root = Path.cwd()
sys.path.insert(0, str(root / 'Tools'))
import scenario_trial as t

t.OUT = Path(__file__).resolve().parent / 'Scenario' / 'chapter01'
t.OUT.mkdir(parents=True, exist_ok=True)
t.MODEL = 'model_openai-gpt-image-2'
try:
    if sys.argv[1] == 'submit':
        source = root / 'ProjectDocs/AI/WorldMapReferenceRebuild/DesignR1/Assets/region_01.png'
        reference = t.OUT / 'Reference.json'
        if not reference.exists():
            raw = source.read_bytes()
            asset = t.api('/assets', {'name': 'Approved_Chapter01_WorldMap.png', 'image': base64.b64encode(raw).decode()})['asset']
            t.save('Reference.json', {'assetId': asset['id'], 'source': str(source.relative_to(root)), 'sha256': hashlib.sha256(raw).hexdigest()})
        prompt = '''Create one clean PORTRAIT CHAPTER MAP ENVIRONMENT based on the supplied approved World Map Chapter 1 painting. Retain its specific Russian fairy-tale village identity: dark hand-carved timber cottages with steep wooden roofs, birch trees, pine forest, small gardens, warm amber window lamps, golden autumn foliage, muted moss and deep navy shadows. Match this painting's brushwork and restrained rich palette. This is a closer, taller exploration of that same village, not a different city. No blue onion-domed cathedral, no grand castle, no windmill, no urban boulevard. Remove visible people; leave scenery only.
Composition: high three-quarter aerial game-map view looking onto the ground, NO SKY and NO HORIZON. A broad pale earth/cobblestone path meanders continuously from bottom center to top center through the village, with seven gentle alternating left/right bends kept in the central 35–65 percent width. Leave generous clear ground on the bends for separately rendered level tokens, without drawing any tokens. Place interesting cottages, fenced gardens, a stone well, firewood stacks, orchard and birch clusters mainly in the left and right thirds; do not obscure the route. Path is the clearest feature. Avoid busy high contrast patches immediately along its central corridor. Lower foreground and upper edge both end in similar subdued forest ground and a centered continuing path, suitable for overlapping vertical scrolling. No black vignette or hard borders. Consistent map scale from top to bottom. Native art will be shown behind interactive UI; output ONLY environment.
STRICTLY NO text, letters, numbers, titles, logos, UI, buttons, stars, node circles, portraits, resource bars, banners, gold frames, interface panels or watermarks. One detailed continuous portrait painting, not a collage or a complete screen.'''
        body = {'prompt': prompt, 'referenceImages': [json.loads(reference.read_text())['assetId']], 'width': 1024, 'height': 1792, 'quality': 'medium', 'background': 'opaque', 'numOutputs': 1}
        with (t.OUT / 'SubmissionAttempt.json').open('x') as f:
            json.dump({'maximumPaidSubmissions': 1, 'automaticResubmission': False}, f)
        t.save('Request.json', {'model': t.MODEL, 'body': body})
        result = t.api('/generate/custom/' + t.MODEL, body)
        job = result.get('job', result)
        job_id = job.get('jobId') or job.get('id')
        assert job_id
        t.save('Job.json', {'id': job_id, 'status': job.get('status')})
        print('Chapter 1 background accepted; poll existing job.')
    else:
        t.poll()
        if json.loads((t.OUT / 'Status.json').read_text())['status'] == 'success':
            t.download()
except urllib.error.HTTPError as error:
    t.save('AccessResult.json', {'httpStatus': error.code})
    print('Scenario HTTP', error.code, '; no resubmission.')
    error.close()
    raise SystemExit(1)
