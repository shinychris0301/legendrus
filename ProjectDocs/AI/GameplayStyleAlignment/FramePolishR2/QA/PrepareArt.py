"""Previously authorized local transparency cleanup; no RGB edits or screenshot crops."""
from pathlib import Path
from PIL import Image
import numpy as np
from scipy import ndimage
import hashlib, json, shutil

root = Path(__file__).resolve().parents[5]
delivery = root / 'ProjectDocs/AI/GameplayStyleAlignment/FramePolishR2'
art = root / 'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned'
generated = Path('/home/k2/.codex/generated_images/01a09624-160e-7f10-a32e-7cc91172628e')
names = {'moves_frame': 'exec-5b0a9bad-299a-4a87-bb14-d7936006ee50.png',
         'cell_lacquer': 'exec-2af9239b-2621-4921-9935-822713bc3fef.png'}
records = []
for name, source in names.items():
    original = generated / source
    saved = delivery / 'Sources' / (name + '.png')
    shutil.copyfile(original, saved)
    im = Image.open(saved).convert('RGBA')
    a = np.array(im)
    if name == 'moves_frame':
        rgb = a[:, :, :3].astype(int)
        # Neutral light checkerboard, including its enclosed holes. Gold/blue/black art is excluded.
        background = (rgb.max(axis=2) - rgb.min(axis=2) < 25) & (rgb.min(axis=2) > 105)
        foreground = ~background
        labels, _ = ndimage.label(foreground)
        counts = np.bincount(labels.ravel()); counts[0] = 0
        foreground = labels == counts.argmax()
        # Keep the artwork at full opacity. Only exterior pixels are discarded.
        a[:, :, 3] = np.where(foreground, 255, 0).astype('uint8')
        Image.fromarray(a).save(art / (name + '.png'))
    else:
        shutil.copyfile(saved, art / (name + '.png'))
    out = Image.open(art / (name + '.png')).convert('RGBA')
    rgba = np.array(out)
    assert np.array_equal(a[:, :, :3], rgba[:, :, :3])
    if name == 'moves_frame':
        assert rgba[591,665,3] == 255
        assert not np.any(rgba[[0,-1],:,3]) and not np.any(rgba[:,[0,-1],3])
        piece=out.copy();piece.thumbnail((390,390))
        proof=Image.new('RGB',(1170,390))
        for i,c in enumerate(('black','white','#ec00aa')):
            bg=Image.new('RGBA',(390,390),c)
            bg.alpha_composite(piece,((390-piece.width)//2,(390-piece.height)//2))
            proof.paste(bg.convert('RGB'),(390*i,0))
        proof.save(delivery/'QA'/'moves_frame_alpha.png')
    records.append({'name':name,'source':str(original),'sourceSha256':hashlib.sha256(saved.read_bytes()).hexdigest(),
                    'runtime':str((art/(name+'.png')).relative_to(root)),
                    'runtimeSha256':hashlib.sha256((art/(name+'.png')).read_bytes()).hexdigest(),
                    'dimensions':out.size,'rgbUnchanged':True,'alphaRange':out.getchannel('A').getextrema(),
                    'method':'Neutral-checker mask; largest connected artwork retained; alpha only' if name=='moves_frame' else 'Byte-identical original opaque texture'})
(delivery/'GeneratedAssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
print(json.dumps(records,indent=2))
