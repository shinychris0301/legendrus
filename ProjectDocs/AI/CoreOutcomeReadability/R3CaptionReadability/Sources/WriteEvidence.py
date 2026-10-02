"""Summarize completed caption captures/checks and supply one-portrait review."""
from pathlib import Path
from collections import Counter
import hashlib
import json
import struct
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'
RUN = QA / 'PlayAccepted'
observations = []
for path in sorted(RUN.glob('CaptionMeasurements_*.json')):
    suffix = path.stem.removeprefix('CaptionMeasurements_')
    tokens = suffix.split('_')
    page, language, width = tokens[0], tokens[-2], int(tokens[-1])
    state = tokens[1] if page == 'Victory' else 'Normal'
    png = RUN / ('GAMEPLAY' if page == 'Gameplay' else 'VICTORY') / (
        f'CaptionNames_{language}_{width}.png' if page == 'Gameplay' else
        f'LiveCaptions_{state}_{language}_{width}.png')
    assert png.is_file()
    data = png.read_bytes()
    assert data[:8] == b'\x89PNG\r\n\x1a\n'
    actual_width, actual_height = struct.unpack_from('>II', data, 16)
    assert actual_width == width
    rows = json.loads(path.read_text())['captions']
    assert rows
    for row in rows:
        row['effectiveFontPixels'] = row['fontPixels'] * row['meshScale']
        assert row['meshScale'] >= .98
    observations.append({'page': page, 'state': state, 'language': language,
        'width': width, 'height': actual_height,
        'capture': str(png.relative_to(BASE)), 'captions': rows})
assert len(observations) == 48
assert len({(row['page'], row['state'], row['language'], row['width']) for row in observations}) == 48
assert Counter(row['page'] for row in observations) == {'Gameplay': 8, 'Victory': 40}
(QA / 'CaptionMeasurements.json').write_text(json.dumps(observations, indent=2, ensure_ascii=False) + '\n')
coverage = {}
for name, expected in [('EditAccepted', 19), ('PlayAccepted', 25)]:
    summary = json.loads((QA / name / 'summary.json').read_text())
    assert summary['exitCode'] == 0 and summary['result']['result'] == 'Passed'
    cases = ET.parse(QA / name / 'results.xml').findall('.//test-case')
    assert len(cases) == expected and all(case.get('result') == 'Passed' for case in cases)
    coverage[name] = dict(Counter(case.get('classname') for case in cases))
(QA / 'Coverage.json').write_text(json.dumps(coverage, indent=2) + '\n')
game = [text for row in observations if row['page'] == 'Gameplay' and row['width'] == 360 for text in row['captions']]
metrics = {'configurationCount': 48, 'gameplayConfigurations': 8, 'victoryConfigurations': 40,
    'finalPngFiles': len(list(RUN.rglob('*.png'))),
    'gameplay360NominalFontPixels': [min(row['fontPixels'] for row in game), max(row['fontPixels'] for row in game)],
    'gameplay360EffectiveFontPixels': [min(row['effectiveFontPixels'] for row in game), max(row['effectiveFontPixels'] for row in game)],
    'allGameMeshScales': [min(text['meshScale'] for row in observations if row['page'] == 'Gameplay' for text in row['captions']),
                         max(text['meshScale'] for row in observations if row['page'] == 'Gameplay' for text in row['captions'])],
    'physicalDeviceMeasurements': False}
(QA / 'MeasurementSummary.json').write_text(json.dumps(metrics, indent=2) + '\n')
apk = ROOT / 'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
with apk.open('rb') as stream:
    digest = hashlib.file_digest(stream, 'sha256').hexdigest()
assert apk.stat().st_size == 765773174 and digest == 'c065e5ab23f9404cb960660e4fd460ae57f15c4310a181022a65da094890fdb5'
assert apk.with_suffix('.apk.sha256').read_text().split()[0] == digest
(QA / 'ExistingApk.json').write_text(json.dumps({'path': str(apk.relative_to(ROOT)), 'bytes': apk.stat().st_size,
    'sha256': digest, 'unchangedSinceR2': True, 'includesR3CaptionChanges': False,
    'newAndroidBuildPerformed': False, 'deviceTested': False}, indent=2) + '\n')
review = '''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Core captions R3 — actual Unity review</title><style>body{margin:24px;background:#17191d;color:#eee;font:16px system-ui}p{max-width:850px;color:#bbc0c9}label{display:inline-block;margin:8px}select{padding:8px}figure{margin:12px 0}img{display:block;max-height:78vh;max-width:100%;height:auto}a{color:#aacfff}</style><h1>Core captions R3</h1><p>One actual Unity Editor portrait at a time. Safe areas and display sizes were simulated. Gameplay names use native lacquer faces; Victory headings are live Russian/English text over the existing blank panel. These are not phone captures.</p><label>Page <select id="page"><option>Gameplay</option><option>Victory</option></select></label><label>State <select id="state"></select></label><label>Language <select id="language"><option value="ru">Russian</option><option value="en">English</option></select></label><label>Viewport <select id="size"><option>360x640</option><option>576x1280</option><option>941x1672</option><option>1080x2400</option></select></label><figure><img id="portrait" alt="Standalone actual Unity portrait"><figcaption id="caption"></figcaption></figure><p>19 Edit Mode and 25 Play Mode checks passed. Small-caption improvement is measured in rendered pixels; phone readability and large stock badges remain open. <a href="TestResults.md">Results, baseline and limitations</a>.</p><script>const captures=CAPTURE_DATA;const states={Gameplay:['Normal'],Victory:['FirstClear','NoBoosters','Replay','Unavailable','NoResult']};const el=Object.fromEntries(['page','state','language','size'].map(id=>[id,document.getElementById(id)]));function render(){const width=Number(el.size.value.split('x')[0]);const row=captures.find(row=>row.page===el.page.value&&row.state===el.state.value&&row.language===el.language.value&&row.width===width);document.getElementById('portrait').src=row.capture;document.getElementById('caption').textContent=row.page+' · '+row.state+' · '+row.language.toUpperCase()+' · '+row.width+'×'+row.height+' · actual Unity Editor'}function updateStates(){el.state.replaceChildren(...states[el.page.value].map(state=>{const option=document.createElement('option');option.textContent=state;return option}));render()}el.page.addEventListener('change',updateStates);for(const id of ['state','language','size'])el[id].addEventListener('change',render);updateStates();</script></html>
'''
data = [{key: value for key, value in row.items() if key != 'captions'} for row in observations]
(BASE / 'CoreCaptions_R3_Review.html').write_text(review.replace('CAPTURE_DATA', json.dumps(data)))
print(json.dumps(metrics, indent=2))
