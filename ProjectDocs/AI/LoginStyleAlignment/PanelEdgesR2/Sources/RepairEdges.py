"""Reapply bounded Login extraction-edge repairs from immutable R1 inputs. No API calls."""
from pathlib import Path
from PIL import Image
import numpy as np, json, hashlib
root=Path(__file__).resolve().parents[5]
b=root/'ProjectDocs/AI/LoginStyleAlignment/PanelEdgesR2'; r1=b.parent/'R1'; dest=root/'Assets/LegendsOfRus/Resources/ProductionUI/LoginAligned'
original=Image.open(r1/'Sources/OriginalReference.png').convert('RGB')
raw=Image.open(r1/'Sources/Scenario/panel/booster_medallion_trial01.png').convert('RGBA')
alpha=np.array(raw.getchannel('A').resize((1360,1756),Image.Resampling.LANCZOS))
records=[]
for name in ['panel','panel_blank']:
 old=np.array(Image.open(b/'Sources/Before'/f'{name}.png').convert('RGBA')); out=old.copy()
 # Restore the uncut crest and fine top trim from the original, using the existing Scenario silhouette.
 source=np.array(original.crop((133,794,813,1672)).resize((1360,1756),Image.Resampling.LANCZOS))
 out[:160,:,:3]=source[:160]
 out[:160,:,3]=alpha[:160]
 Image.fromarray(out).save(dest/f'{name}.png')
 records.append({'asset':name,'rgbOutsideTop80NativeRowsUnchanged':bool(np.array_equal(out[160:,:,:3],old[160:,:,:3]))})
# Reconstruct only visible cloak/feather strips previously removed by the rectangular knockout.
old=np.array(Image.open(b/'Sources/Before/subject.png').convert('RGBA'));out=old.copy()
pa=np.array(Image.open(dest/'panel.png'))[:,:,3]
source=np.array(original.crop((0,393,941,1270)).resize((1882,1754),Image.Resampling.LANCZOS))
# Original subject behind the panel: top band plus thin side strips. No source scenery below hands/tail.
repair=np.zeros(old.shape[:2],np.uint8)
repair[900:920,266:1628]=255
repair[900:1610,266:322]=255
repair[900:1610,1574:1628]=255
# Cut follows the panel's actual opacity, excluding its opaque wood/paper face.
panel_at=np.zeros(old.shape[:2],np.uint8);hh=min(pa.shape[0],old.shape[0]-802);panel_at[802:802+hh,266:1626]=pa[:hh]
mask=repair>0
out[mask,:3]=source[mask]
out[mask,3]=(255-panel_at[mask])
Image.fromarray(repair).save(b/'Sources/SubjectRepairRegion.png')
Image.fromarray(out).save(dest/'subject.png')
records.append({'asset':'subject','rgbOutsideRepairRegionUnchanged':bool(np.array_equal(out[~mask,:3],old[~mask,:3])),'repairPixels':int(mask.sum())})
for r in records:
 p=dest/(r['asset']+'.png');r['runtime']=str(p.relative_to(root));r['sha256']=hashlib.sha256(p.read_bytes()).hexdigest()
(b/'AssetProvenance.json').write_text(json.dumps({'method':'Existing Scenario alpha and bounded original-pixel restoration; no new API request or generation','scenarioCostCU':0,'records':records},indent=2)+'\n')
for label,color in [('Black','black'),('White','white'),('Magenta','magenta')]:
 for name in ['panel','panel_blank','subject']:
  im=Image.open(dest/f'{name}.png');bg=Image.new('RGBA',im.size,color);bg.alpha_composite(im);bg.thumbnail((941,878));bg.convert('RGB').save(b/'QA'/f'{name}_On{label}.png')
print(records)
