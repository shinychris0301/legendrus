"""Restore original podium stat surfaces; remove only baked numeric glyphs.

No generation: preserves existing isolated alpha, ribbons, crests and rewards.
The tiny surfaces hidden by original digits are interpolated, not recovered pixels.
"""
from pathlib import Path
from PIL import Image, ImageDraw
import cv2, numpy as np, json, hashlib, uuid, re

b = Path(__file__).resolve().parents[1]
art = Path('Assets/LegendsOfRus/Resources/ProductionUI/TeamRankingAligned')
ref = Image.open(b/'Sources/OriginalReference.png').convert('RGB')
records = []
assets = []
for name, box, restore, letters in [
    ('gold', (309,305,633,795), (400,606,606,684), [(450,612,548,641),(436,649,578,684)]),
    ('silver', (23,352,290,790), (125,606,250,684), [(141,612,239,641),(132,649,245,684)]),
    ('bronze', (638,357,884,790), (710,606,843,684), [(729,612,831,641),(721,649,837,684)]),
]:
    old = Image.open(art/f'podium_{name}_r3.png').convert('RGBA')
    original = np.array(ref.crop(restore))
    mask = np.zeros(original.shape[:2], np.uint8)
    # Pale gold glyphs are distinct from the dark blue/brown face. Restrict to
    # numeral footprints so source member/shield pictograms and divider survive.
    rgb = original.astype(np.int16)
    gold = (rgb[:,:,0]>70) & (rgb[:,:,1]>49) & (rgb[:,:,0]>rgb[:,:,2]*1.24)
    for x1,y1,x2,y2 in letters:
        xa,ya,xb,yb=x1-restore[0],y1-restore[1],x2-restore[0],y2-restore[1]
        mask[ya:yb,xa:xb] = gold[ya:yb,xa:xb]*255
    # Include the embossed letters' dark outlines, not just their gold faces.
    mask = cv2.dilate(mask,np.ones((9,9),np.uint8),iterations=1)
    # Protect the original divider and reward rail from the expanded glyph mask.
    mask[35:43,:]=0
    mask[-3:,:]=0
    # Protect pictogram tips beside the numerals from the dilated shadow mask.
    member_start,score_start={'gold':(451,436),'silver':(139,130),'bronze':(732,727)}[name]
    mask[:35,:max(0,member_start-restore[0])]=0
    mask[43:,:max(0,score_start-restore[0])]=0
    repaired=cv2.inpaint(original,mask,5,cv2.INPAINT_NS)
    scale=old.width/(box[2]-box[0])
    region=(round((restore[0]-box[0])*scale),round((restore[1]-box[1])*scale),round((restore[2]-box[0])*scale),round((restore[3]-box[1])*scale))
    new=old.copy()
    new.paste(Image.fromarray(repaired).resize((region[2]-region[0],region[3]-region[1]),Image.Resampling.LANCZOS),region[:2])
    new.putalpha(old.getchannel('A'))
    out=art/f'podium_{name}_r5.png';new.save(out)
    meta=Path(str(out)+'.meta')
    if not meta.exists():meta.write_text(re.sub(r'guid: [a-f0-9]{32}','guid: '+uuid.uuid4().hex,(art/f'podium_{name}_r3.png.meta').read_text(),count=1))
    before,after=np.array(old),np.array(new)
    outside=np.ones(before.shape[:2],bool);outside[region[1]:region[3],region[0]:region[2]]=False
    assert np.array_equal(before[:,:,3],after[:,:,3])
    assert np.array_equal(before[outside],after[outside])
    Image.fromarray(mask).save(b/'Sources'/f'{name}_numeral_mask.png')
    records.append(dict(asset=out.name,sha256=hashlib.sha256(out.read_bytes()).hexdigest(),sourceBox=box,restoredSourceBox=restore,numeralBoxes=letters,interpolatedPixels=int(np.count_nonzero(mask)),visibleSourcePixels=int(np.count_nonzero(mask==0)),sourceRGBMismatches=int(np.count_nonzero(np.any(repaired[mask==0]!=original[mask==0],axis=1))),alphaPreserved=True,outsideRestorationPreserved=True))
    assets.append((name,new))
sheet=Image.new('RGB',(1050,810));d=ImageDraw.Draw(sheet)
for i,(name,im) in enumerate(assets):
    for j,c in enumerate(['black','white','#ff00dd']):
        d.rectangle((j*350,i*270,(j+1)*350,(i+1)*270),fill=c)
        thumb=im.copy();thumb.thumbnail((330,240));sheet.paste(thumb,(j*350+(350-thumb.width)//2,i*270),thumb)
    d.text((8,i*270+247),'podium_'+name+'_r5',fill='#bbbbbb')
sheet.save(b/'QA/AssetAlpha.png')
(b/'Sources/AssetProvenance.json').write_text(json.dumps(records,indent=2)+'\n')
print(json.dumps(records,indent=2))
