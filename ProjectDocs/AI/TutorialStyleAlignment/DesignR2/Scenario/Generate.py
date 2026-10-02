"""One authorized Scenario design preview. Never imported into Unity."""
import base64
import hashlib
import json
from pathlib import Path
import sys
import urllib.error

ROOT = Path(__file__).resolve().parents[5]
sys.path.insert(0, str(ROOT / 'Tools'))
import scenario_trial as api

api.OUT = Path(__file__).resolve().parent
SOURCE = Path('/home/k2/Downloads/match3final/tuturial.png')
PROMPT = '''Use case: ui-mockup. Create ONE polished portrait Tutorial / Help screen design for the Russian fairy-tale match-three game Legends of Rus.
Input image is a STYLE REFERENCE for the ornate panel only, not commercial content. Adapt its fine antique-gold filigree border, sapphire crest centered at top, engraved midnight navy lacquer, dimensional red Close button upper right, emerald/gold primary button, and elegant readable Cyrillic serif typography. Match the reference's rich dark tones and warm gold highlights. Maintain the graceful undistorted proportions of the ornamentation. This is a new educational composition, not a shop.
Composition: a single standalone portrait screen, no device mockup or collage. Dim fairy-tale forest scenery behind a large framed help panel. No player/resource header required. Panel has comfortable internal margins and contains everything without overlap. Large gold title at top: "КАК ИГРАТЬ". Below it four evenly spaced compact section tabs: "Основы", "Комбинации", "Бустеры", "Преграды". First tab selected sapphire blue, others subdued navy.
Main content: centered heading "Соберите три в ряд". Beneath, a clear educational BEFORE / AFTER diagram: two small three-by-three dark carved-wood grids, thin gold connections. Calm polished emblem gems, red teardrop rubies with fleur-de-lis, blue elongated sapphire hexagons, green square emeralds with fleur-de-lis, gold round tokens. No dazzling bloom. Left grid colors by row: red blue green / blue red gold / red blue green. Show a small gold horizontal two-way arrow between the middle-left blue and middle-center red to indicate their swap. Right grid after swap: red blue green / red blue gold / red blue green. Highlight the left vertical column of three red gems with a subtle fine gold outline. A simple right-pointing gold arrow between grids. Text labels over grids: "Обмен" and "Совпадение". Instruction below in large light warm-ivory serif: "Меняйте соседние самоцветы местами." then "Собирайте линии из трёх и более." Ensure all nine cells per grid remain discrete and visible.
Lower panel: two spacious horizontal instruction rows, each on a subtly inset navy face with fine gold corners, generous icon/text separation. First row has a small gold-rimmed green gem and checkmark at left; heading "Выполняйте цели" and body "Соберите нужные предметы." Second row has an antique-gold hourglass at left; heading "Берегите ходы" and body "Достигните целей за отведённые ходы." Keep text legible at phone scale, body cream not dim bronze. No tiny disclaimers.
Bottom: centered page indicator "1 / 4", then wide emerald primary button "ДАЛЕЕ", preserving natural reference button aspect ratio; no stretched ornament. Strong visual hierarchy, restrained depth, crisply rendered glyphs, uncluttered spacing. Frame fully visible on all sides. No prices, sale ribbons, chest, reward amounts, countdown, resource quantities, advertising, logo variant, neon glow, invented characters, watermarks, English labels or extra body paragraphs.'''

def submit():
    raw = SOURCE.read_bytes()
    ref_path = api.OUT / 'Reference.json'
    if not ref_path.exists():
        result = api.api('/assets', {'name':'LegendsOfRus_Tutorial_PanelStyle_DesignR2.png', 'image':base64.b64encode(raw).decode()})
        api.save('Reference.json', {'assetId':result['asset']['id'], 'path':str(SOURCE), 'sha256':hashlib.sha256(raw).hexdigest(), 'role':'Panel style reference only; preview is not runtime artwork'})
    reference = json.loads(ref_path.read_text())
    body = {'prompt':PROMPT, 'referenceImages':[reference['assetId']], 'width':1024, 'height':1792, 'numOutputs':1, 'quality':'high', 'background':'opaque'}
    api.save('Request.json', {'modelId':api.MODEL,'body':body})
    with (api.OUT / 'SubmissionAttempt.json').open('x') as f:
        json.dump({'generationRequestsAllowed':1,'automaticResubmission':False}, f)
    result = api.api('/generate/custom/' + api.MODEL, body)
    job = result.get('job',result)
    job_id = job.get('jobId') or job.get('id')
    if not job_id:
        raise api.SetupError('Missing job ID; do not resubmit.')
    api.save('Job.json', {'id':job_id,'status':job.get('status')})
    print('One Tutorial design preview submitted; job recorded.')

if __name__ == '__main__':
    try:
        {'submit':submit,'poll':api.poll,'download':api.download}[sys.argv[1]]()
    except urllib.error.HTTPError as error:
        print('Scenario HTTP', error.code, '(response suppressed); no automatic retry.')
        error.close()
        sys.exit(1)
    except (OSError, ValueError, KeyError, api.SetupError) as error:
        print('Request or local state unavailable:', type(error).__name__, '; no automatic retry.')
        sys.exit(1)
