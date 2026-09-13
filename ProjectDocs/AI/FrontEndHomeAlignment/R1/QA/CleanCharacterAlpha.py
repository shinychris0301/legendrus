"""User-authorized local alpha cleanup of complete newly generated character images.

No reference screenshots, existing production sprites or artwork RGB interiors are edited.
Dependencies/model live only in the isolated /tmp tooling environment.
"""
from pathlib import Path
import hashlib
import json
import numpy as np
from PIL import Image
from rembg import remove, new_session

delivery = Path('ProjectDocs/AI/FrontEndHomeAlignment/R1')
target = delivery / 'Sources/Final'
target.mkdir(parents=True, exist_ok=True)
session = new_session('isnet-general-use', providers=['CPUExecutionProvider'])
records = []
for name in ('masha_dusk', 'ivan_splash_dusk', 'ivan_loading_dusk'):
    source = delivery / 'Sources/Initial' / (name + '.png')
    original = Image.open(source).convert('RGB')
    matte = remove(original, session=session, alpha_matting=True,
                   alpha_matting_foreground_threshold=240,
                   alpha_matting_background_threshold=10,
                   alpha_matting_erode_size=6)
    result = np.asarray(matte).copy()
    alpha = result[:, :, 3]
    # Reinstate unchanged source RGB in solid interior pixels; retain edge matting.
    solid = alpha >= 245
    result[solid, :3] = np.asarray(original)[solid]
    result[solid, 3] = 255
    result[alpha <= 2] = 0
    final = target / (name + '.png')
    Image.fromarray(result, 'RGBA').save(final)
    records.append({'asset': name, 'source': str(source), 'final': str(final),
                    'sourceSha256': hashlib.sha256(source.read_bytes()).hexdigest(),
                    'finalSha256': hashlib.sha256(final.read_bytes()).hexdigest(),
                    'dimensions': list(original.size),
                    'solidPixelsPreservingExactSourceRgb': int(solid.sum()),
                    'alphaZero': int((result[:, :, 3] == 0).sum()),
                    'alphaOpaque': int((result[:, :, 3] == 255).sum())})
    print(json.dumps(records[-1]), flush=True)
(delivery / 'Sources/LocalAlphaCleanup.json').write_text(json.dumps({
    'authorization': 'User explicitly answered Allow local transparency cleanup after two RGB checkerboard exports.',
    'method': 'Local rembg isnet-general-use CPU mask with alpha matting; original RGB restored in solid interiors.',
    'modelAndDependencies': '/tmp only; no Unity package changes',
    'completeGeneratedSourceUsed': True, 'referenceScreenshotCrops': False,
    'assets': records, 'visualAcceptance': 'Pending black/white/magenta inspection'
}, indent=2) + '\n')
