"""Read-only pixel comparisons; run in the isolated alpha-QA Python environment."""
from pathlib import Path
import json
import numpy as np
from PIL import Image

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/FrontEndHomeAlignment/R1/UnityVerification'
prior = root / 'ProjectDocs/AI/HomeStyleAlignment/SceneToneR10/UnityVerification/Home'
pairs = [(prior / p.name, p) for p in sorted((delivery / 'HomeRegression').glob('*.png')) if (prior / p.name).exists()]
assert len(pairs) == 7
for page in ('Splash', 'Loading'):
    pairs.append((delivery / page / (page+'_941x1672.png'), delivery / page / (page+'_941x1672_roundtrip.png')))
rows = []
for before, after in pairs:
    a = np.asarray(Image.open(before).convert('RGBA')).astype(int)
    b = np.asarray(Image.open(after).convert('RGBA')).astype(int)
    delta = np.abs(a-b)
    rows.append({'before': str(before.relative_to(root)), 'after': str(after.relative_to(root)),
        'changedPixels': int(np.any(delta, axis=2).sum()), 'maxChannelDelta': int(delta.max()),
        'pixelIdentical': bool(np.array_equal(a,b))})
result = {'comparisons': rows, 'allPixelIdentical': all(r['pixelIdentical'] for r in rows),
          'scope': 'Seven accepted Home R10 captures unchanged; final Splash and Loading RU/EN/RU roundtrip restoration.'}
(delivery / 'ProtectedPixelComparison.json').write_text(json.dumps(result, indent=2)+'\n')
print(json.dumps({'passed': result['allPixelIdentical'], 'comparisons': len(rows)}))
assert result['allPixelIdentical'], rows
