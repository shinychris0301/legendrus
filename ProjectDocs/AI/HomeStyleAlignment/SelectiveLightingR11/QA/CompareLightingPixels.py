"""Read-only decoded pixel comparisons. Does not create or edit raster images."""
from pathlib import Path
import json
import numpy as np
from PIL import Image

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/SelectiveLightingR11'
out = delivery / 'UnityVerification'
accepted = root / 'ProjectDocs/AI/FrontEndHomeAlignment/R1/UnityVerification'
def pixels(path): return np.array(Image.open(path).convert('RGBA'), dtype=np.int16)
before_path = root / 'ProjectDocs/AI/HomeStyleAlignment/SceneToneR10/UnityVerification/Home/Home_941x1672_RU.png'
after_path = out / 'Home/Home_941x1672_RU.png'
before, after = pixels(before_path), pixels(after_path)
delta = after - before
changed = np.any(delta[:, :, :3] != 0, axis=2)
alpha = int(np.count_nonzero(delta[:, :, 3]))
yy, xx = np.indices(changed.shape)
# Existing reference rectangles plus the mesh's 3% effect padding and 1px rounding allowance.
hero = (xx >= -50) & (xx <= 977) & (yy >= 444) & (yy <= 1471)
foliage = (xx >= -22) & (xx <= 105) & (yy >= 1206) & (yy <= 1457)
outside = int(np.count_nonzero(changed & ~(hero | foliage)))
regions = {
    'Ivan face': (400,560,35,40), 'HUD balance': (350,48,160,40), 'Avatar': (70,70,30,40),
    'Logo': (350,210,210,180), 'Play and lettering': (280,1080,380,90),
    'Map': (90,1290,120,90), 'Event': (520,1290,120,90), 'Event badge': (636,1240,14,14),
    'Gift badge': (905,1475,12,12), 'Mail': (55,1535,65,45), 'Quest gem': (645,1520,35,35)
}
protected = []
for name, (x,y,w,h) in regions.items():
    d = delta[y:y+h,x:x+w]
    row = {'name':name, 'rectTopLeft':[x,y,w,h], 'changedPixels':int(np.count_nonzero(np.any(d != 0,axis=2))), 'maxChannelDifference':int(np.abs(d).max())}
    protected.append(row)
    assert row['maxChannelDifference'] <= 1, row
samples = []
for name, (x,y,w,h) in {'Blue clothing': (360,840,110,120), 'Red cloak': (255,740,55,140),
                      'Wing and feather boundary': (600,650,310,360), 'Visible Map foliage': (0,1245,50,140)}.items():
    d = delta[y:y+h,x:x+w,:3]
    sums = d.sum(axis=2)
    samples.append({'name':name, 'rectTopLeft':[x,y,w,h], 'meanRgbDelta':d.mean(axis=(0,1)).tolist(),
        'darkerPixelsRgbSumBelowMinus3':int(np.count_nonzero(sums < -3)), 'brighterPixelsRgbSumAbove3':int(np.count_nonzero(sums > 3))})
regression = []
for category, prior in [('SplashRegression','Splash'), ('LoadingRegression','Loading')]:
    for path in sorted((out / category).glob('*.png')):
        if path.name.startswith('ActualLoading'): continue  # Captured progress moment can vary.
        a,b = pixels(accepted / prior / path.name), pixels(path)
        count = int(np.count_nonzero(np.any(a != b,axis=2)))
        regression.append({'capture':str(path.relative_to(out)), 'accepted':str((accepted/prior/path.name).relative_to(root)), 'differentPixels':count})
        assert count == 0, regression[-1]
roundtrip = int(np.count_nonzero(np.any(after != pixels(out / 'Home/Home_941x1672_RU_roundtrip.png'),axis=2)))
assert roundtrip == 0 and alpha == 0 and outside == 0
backgrounds = []
for name, expected in [('Black',[0,0,0]),('White',[255,255,255]),('Magenta',[255,0,217])]:
    sample = pixels(out / f'Home/LightingBackground_{name}.png')[71,900,:3]
    assert np.max(np.abs(sample-np.array(expected))) <= 1
    backgrounds.append({'name':name,'emptyCornerRgb':sample.tolist(),'expectedRgbWithinOne':expected})
sums = delta[:,:,:3].sum(axis=2)
result = {'passed':True, 'kind':'Read-only R10/R11 decoded pixels; no raster modification',
    'before':str(before_path.relative_to(root)), 'after':str(after_path.relative_to(root)),
    'changedPixels':int(changed.sum()), 'darkerPixelsRgbSumBelowMinus3':int(np.count_nonzero(sums < -3)),
    'brighterPixelsRgbSumAbove3':int(np.count_nonzero(sums > 3)), 'alphaChanges':alpha,
    'maxChannelReduction':int(-delta[:,:,:3].min()), 'maxChannelIncrease':int(delta[:,:,:3].max()),
    'changedPixelsOutsidePaddedSprites':outside, 'protectedRegions':protected, 'lightingSamples':samples,
    'homeRussianRoundtripDifferentPixels':roundtrip, 'acceptedFrontEndComparisons':regression,
    'diagnosticBackgroundColors':backgrounds, 'note':'Original design reference is visually reviewed, not an identical-pixel target. Geometry/art remains the accepted R10 version.'}
(out / 'LightingPixelComparison.json').write_text(json.dumps(result,indent=2)+'\n')
print(json.dumps({k:result[k] for k in ['passed','changedPixels','darkerPixelsRgbSumBelowMinus3','brighterPixelsRgbSumAbove3','alphaChanges','changedPixelsOutsidePaddedSprites']}))
print('Identical accepted Splash/Loading captures:',len(regression))
