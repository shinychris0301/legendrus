"""User-authorized local alpha cleanup of complete newly generated World Map controls.

No reference screenshots, existing production sprites or artwork RGB interiors are edited.
Dependencies/model live only in the isolated /tmp tooling environment.
"""
from pathlib import Path
import hashlib
import json
import numpy as np
from PIL import Image
from rembg import remove, new_session
from scipy import ndimage

delivery = Path('ProjectDocs/AI/WorldMapStyleAlignment/R1')
target = delivery / 'Sources/Final'
target.mkdir(parents=True, exist_ok=True)
session = new_session('isnet-general-use', providers=['CPUExecutionProvider'])
records = []
for name in ('title', 'map', 'tasks', 'guild'):
    source = delivery / 'Sources' / (name + '.png')
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
    rgb = np.asarray(original)
    # The detector mistook the guild's blue lacquer for background. Restore only
    # the known solid interior of that medallion, never the surrounding silhouette.
    restored_panel = np.zeros(alpha.shape, dtype=bool)
    if name == 'guild':
        yy, xx = np.indices(alpha.shape)
        restored_panel = ((xx - 627) / 455) ** 2 + ((yy - 545) / 415) ** 2 < 1
        result[restored_panel, :3] = rgb[restored_panel]
        result[restored_panel, 3] = 255
    # Remove neutral checker remnants connected to the external background.
    # Colored gold/sapphire detail and enclosed opaque lacquer are retained.
    neutral = (rgb.max(axis=2).astype(int) - rgb.min(axis=2).astype(int) < 14) & (rgb.min(axis=2) > 55)
    seed = np.zeros(alpha.shape, dtype=bool)
    seed[0] = neutral[0]; seed[-1] = neutral[-1]; seed[:, 0] = neutral[:, 0]; seed[:, -1] = neutral[:, -1]
    exterior = ndimage.binary_propagation(seed, mask=neutral)
    result[exterior & ~restored_panel] = 0
    final = target / (name + '.png')
    Image.fromarray(result, 'RGBA').save(final)
    records.append({'asset': name, 'source': str(source), 'final': str(final),
                    'sourceSha256': hashlib.sha256(source.read_bytes()).hexdigest(),
                    'finalSha256': hashlib.sha256(final.read_bytes()).hexdigest(),
                    'dimensions': list(original.size),
                    'solidPixelsPreservingExactSourceRgb': int(solid.sum()),
                    'alphaZero': int((result[:, :, 3] == 0).sum()),
                    'alphaOpaque': int((result[:, :, 3] == 255).sum())})
    records[-1]['opaqueGuildInteriorRestored'] = int(restored_panel.sum())
    records[-1]['externalNeutralCheckerMaskPixels'] = int(exterior.sum())
    print(json.dumps(records[-1]), flush=True)
(delivery / 'Sources/LocalAlphaCleanup.json').write_text(json.dumps({
    'authorization': 'User explicitly answered Allow local transparency cleanup after two RGB checkerboard exports.',
    'method': 'Local rembg isnet-general-use CPU mask with alpha matting; original RGB restored in solid interiors.',
    'modelAndDependencies': '/tmp only; no Unity package changes',
    'completeGeneratedSourceUsed': True, 'referenceScreenshotCrops': False,
    'assets': records, 'visualAcceptance': 'Pending black/white/magenta inspection',
    'scope': 'Reuse of the user-authorized local transparency-cleanup method for new generated controls. Opaque navy panels must be retained.'
}, indent=2) + '\n')
