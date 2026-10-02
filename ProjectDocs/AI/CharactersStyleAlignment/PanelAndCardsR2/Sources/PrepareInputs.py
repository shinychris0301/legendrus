from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;source=b.parent.parent/'R1/Sources/OriginalReference.png';src=Image.open(source).convert('RGB')
regions={'rarity_r2':(506,398,829,445),'star_r2':(570,367,604,400),'badge_sword_r2':(253,848,304,899),'badge_gem_r2':(414,850,465,900),'badge_shield_r2':(854,852,894,900),'badge_skull_r2':(696,1096,742,1140),'ability_0_r2':(473,611,568,703),'ability_1_r2':(575,611,669,703),'ability_2_r2':(678,611,769,703),'ability_3_r2':(779,611,871,703)}
records=[]
for n,box in regions.items():
 im=src.crop(box);mask=Image.new('L',im.size);d=ImageDraw.Draw(mask)
 if n=='rarity_r2':
  d.rectangle((54,12,277,35),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(1));a=np.array(src.crop((546,405,561,432)));a=np.tile(a,(3,25,1))[:im.height,:im.width];im=Image.composite(Image.fromarray(a),im,mask)
 elif n.startswith('ability_'):
  # Retain the source circular level frame; clear its painted number for a live unavailable dash.
  d.ellipse((68,65,87,84),fill=255);mask=mask.filter(ImageFilter.GaussianBlur(.6));im=Image.composite(Image.new('RGB',im.size,(18,22,21)),im,mask)
 im.save(b/'Inputs'/f'{n}.png');mask.save(b/'RepairMasks'/f'{n}.png');records.append({'name':n,'bounds':box,'inputSha256':hashlib.sha256((b/'Inputs'/f'{n}.png').read_bytes()).hexdigest()})
(b/'Assessment.json').write_text(json.dumps({'source':str(source),'sourceSha256':hashlib.sha256(source.read_bytes()).hexdigest(),'assets':records,'decision':'Scenario alpha extraction only. Preserve recoverable source RGB. Bounded rarity lettering and ability level cleanup, native labels. Reuse approved stat symbols, clean Gameplay environment solely inside new card layers, original R1 portraits.'},indent=2))
