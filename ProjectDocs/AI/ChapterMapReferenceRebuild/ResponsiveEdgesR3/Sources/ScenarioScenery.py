from pathlib import Path
import sys, json, hashlib, base64, urllib.error

root = Path.cwd()
sys.path.insert(0, str(root / 'Tools'))
import scenario_trial as t

t.OUT = Path(__file__).resolve().parent / 'Scenario' / 'chapter01_landmarks'
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
        prompt = '''Create ONE spectacular but readable portrait chapter-map environment based on the approved Chapter 1 Russian village painting. Preserve its Russian fairy-tale identity: carved timber izbas, steep wooden roofs, birches, autumn foliage, dark navy forest shadows, warm amber lamps. Elevate composition and richness, not generic castles or a different biome. Paint a high three-quarter aerial adventure-map view with NO SKY OR HORIZON, consistent miniature map scale from top to bottom.
The village occupies terraced ground around a beautiful narrow river, with a graceful arched stone bridge in the middle-right, little cascades and luminous reflections. A distinctive carved timber chapel with a tall spire and glowing windows sits in the upper-left; an ancient giant oak and a village square with lanterns and a decorated well sit lower-left. A modest wooden palisade gate and distant cottages occupy upper-right. Rich ferns, birch leaves, wildflowers, soft mist layers and diagonal shafts of warm light create depth. Keep it recognizably the same native village as the input, not a royal city. No grand blue-domed cathedral, no giant fortress, no characters or crowds.
A broad winding natural earth-and-stone route zigzags from bottom to top, traversing alternating clear terraces at approximately 30 percent and 66 percent of image width. Keep central 25–72 percent usable for separately overlaid native round medallions: no large roofs or landmark focal points there. Strong silhouettes and layered foreground/midground/background, carefully controlled jewel-like highlights, dark edges, readable calm landing areas. Do not make the entire image equally busy. Similar forest-ground tones and continuing routes at top and bottom support vertical overlap. The goal is a more impressive chapter environment than repetitive identical houses.
STRICTLY scenery only: NO typography, UI, numbers, circles, medallions, badges, panels, portraits, gold frames, icons, resource bars, dotted progress lines or watermarks. No collage, no screenshot design.'''
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
