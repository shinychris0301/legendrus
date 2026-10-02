"""Authorized exterior transparency cleanup and transparent-padding trim only.

Run with system Python after retaining each selected *_Generated_Raw.png here.
No screenshot is read. RGB of every retained source pixel is preserved.
"""
from pathlib import Path
from collections import deque
import hashlib
import json
import numpy as np
from PIL import Image

root = Path(__file__).resolve().parents[5]
review = Path(__file__).resolve().parent.parent
reports = []
for key in ('ivan','panel'):
    source = Path(__file__).with_name(key + '_Generated_Raw.png')
    if not source.exists():
        continue
    image = Image.open(source)
    rgba = np.asarray(image.convert('RGBA')).copy()
    rgb = rgba[:, :, :3]
    height, width = rgb.shape[:2]
    cleared = 0
    if image.mode != 'RGBA':
        threshold = 38
        neutral = (np.ptp(rgb, axis=2) < threshold) & (rgb.min(axis=2) > 66)
        exterior = np.zeros((height, width), dtype=bool)
        queue = deque()
        # Neutral background enclosed by hair curls. Coordinates are in the
        # original image; eye whites and the shirt collar are deliberately excluded.
        if key=='ivan':
            for left,top,right,bottom in ((622,47,648,58),(568,49,578,57),(742,138,746,149),(726,149,732,161),(730,172,735,185),(373,186,379,196),(366,196,373,205),(716,202,722,211),(324,259,335,267),(335,285,350,302),(327,302,338,308)):
                ys,xs=np.where(neutral[top:bottom,left:right])
                if len(xs):
                    y,x=top+int(ys[0]),left+int(xs[0]);exterior[y,x]=True;queue.append((y,x))
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
        rgba[:, :, 3] = np.where(exterior, 0, 255).astype(np.uint8)
        cleared = int(exterior.sum())
    if key=='panel':
        # Generator left alpha=1 noise far outside the gold outline. Remove only
        # this imperceptible exterior residue, retaining every alpha > 1 sample.
        rgba[:,:,3]=np.where(rgba[:,:,3]<=1,0,rgba[:,:,3])
    sprite = Image.fromarray(rgba, 'RGBA')
    box = sprite.getbbox()
    box = (max(0, box[0] - 6), max(0, box[1] - 6), min(width, box[2] + 6), min(height, box[3] + 6))
    sprite = sprite.crop(box)
    destination = root / ('Assets/LegendsOfRus/Resources/ProductionUI/FailAligned/' + key + '_r1.png')
    sprite.save(destination)
    for name, color in (('Black', (0, 0, 0, 255)), ('White', (255, 255, 255, 255)), ('Magenta', (230, 0, 190, 255))):
        bg = Image.new('RGBA', sprite.size, color)
        bg.alpha_composite(sprite)
        bg.convert('RGB').save(review / 'QA' / (key + '_Alpha_' + name + '.png'))
    reports.append({
        'asset': str(destination.relative_to(root)), 'source': str(source.relative_to(root)),
        'source_mode': image.mode, 'source_size': [width, height], 'output_size': list(sprite.size),
        'trim_box': box, 'exterior_pixels_cleared': cleared,
        'method': 'Generated alpha retained above 1/255; remove exterior alpha=1 residue and trim padding' if image.mode == 'RGBA' else 'Connected neutral exclusion: min > 66, RGB range < 38; inspected enclosed hair-background gaps; transparent-padding trim',
        'retained_rgb_unchanged': bool(np.array_equal(np.asarray(sprite)[:, :, :3], rgb[box[1]:box[3], box[0]:box[2]])),
        'authorization': 'Existing user authorization: Allow local transparency cleanup',
        'source_sha256': hashlib.sha256(source.read_bytes()).hexdigest(),
        'output_sha256': hashlib.sha256(destination.read_bytes()).hexdigest(),
    })
(review / 'QA' / 'AssetPreparation.json').write_text(json.dumps(reports, indent=2) + '\n')
print(json.dumps(reports, indent=2))
