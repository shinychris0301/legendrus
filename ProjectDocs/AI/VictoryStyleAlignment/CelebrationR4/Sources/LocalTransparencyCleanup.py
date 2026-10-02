"""Remove the generated exterior checkerboard; never sample the screen reference.

Uses the user's existing 'Allow local transparency cleanup' authorization.
Run with the system Python (Pillow and numpy). Opaque artwork RGB is unchanged.
"""
from pathlib import Path
from collections import deque
import hashlib
import json
import numpy as np
from PIL import Image

root = Path(__file__).resolve().parents[5]
review = Path(__file__).resolve().parent.parent
source = Path(__file__).with_name('Banner_Generated_Raw.png')
destination = root / 'Assets/LegendsOfRus/Resources/ProductionUI/VictoryAligned/banner_r4.png'
rgb = np.asarray(Image.open(source).convert('RGB')).copy()
height, width = rgb.shape[:2]
neutral = (np.ptp(rgb, axis=2) < 38) & (rgb.min(axis=2) > 66)
exterior = np.zeros((height, width), dtype=bool)
queue = deque()
for x in range(width):
    for y in (0, height - 1):
        if neutral[y, x] and not exterior[y, x]:
            exterior[y, x] = True
            queue.append((y, x))
for y in range(height):
    for x in (0, width - 1):
        if neutral[y, x] and not exterior[y, x]:
            exterior[y, x] = True
            queue.append((y, x))
while queue:
    y, x = queue.popleft()
    for yy, xx in ((y - 1, x), (y + 1, x), (y, x - 1), (y, x + 1)):
        if 0 <= yy < height and 0 <= xx < width and neutral[yy, xx] and not exterior[yy, xx]:
            exterior[yy, xx] = True
            queue.append((yy, xx))
alpha = np.where(exterior, 0, 255).astype(np.uint8)
rgba = np.dstack((rgb, alpha))
sprite = Image.fromarray(rgba, 'RGBA')
box = sprite.getbbox()
box = (max(0, box[0] - 6), max(0, box[1] - 6), min(width, box[2] + 6), min(height, box[3] + 6))
sprite = sprite.crop(box)
sprite.save(destination)
for name, color in (('Black', (0, 0, 0, 255)), ('White', (255, 255, 255, 255)), ('Magenta', (230, 0, 190, 255))):
    bg = Image.new('RGBA', sprite.size, color)
    bg.alpha_composite(sprite)
    bg.convert('RGB').save(review / 'QA' / ('BannerAlpha_' + name + '.png'))
report = {
    'source': str(source), 'source_size': [width, height], 'source_mode': Image.open(source).mode,
    'output': str(destination.relative_to(root)), 'output_size': list(sprite.size),
    'trim_box': box, 'exterior_pixels_cleared': int(exterior.sum()),
    'method': 'Border-connected neutral exclusion (RGB max-min < 38 and min > 66); 6px transparent-padding trim only',
    'opaque_rgb_unchanged': bool(np.array_equal(np.asarray(sprite)[:, :, :3], rgb[box[1]:box[3], box[0]:box[2]])),
    'authorization': "Earlier user reply: Allow local transparency cleanup",
    'source_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
    'output_sha256': hashlib.sha256(destination.read_bytes()).hexdigest(),
}
(review / 'QA' / 'BannerTransparencyCleanup.json').write_text(json.dumps(report, indent=2) + '\n')
print(json.dumps(report, indent=2))
