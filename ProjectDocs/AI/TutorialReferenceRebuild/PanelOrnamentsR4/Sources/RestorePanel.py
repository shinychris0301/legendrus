"""Restore source ornament pixels, restricting replacement surfaces to frame interiors.
No generation. Existing R3 PNGs/GUIDs remain unchanged; output two new sprites.
"""
from pathlib import Path
from PIL import Image,ImageDraw,ImageOps
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parents[1];dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/TutorialOriginal')
source=Image.open(b.parent/'NativeOriginalR3/Sources/Reference.png').convert('RGB').crop((76,304,864,1425))
base=Image.open(dst/'panel_ru.png').convert('RGBA')
original=np.array(source);rgb=np.array(base)[:,:,:3].copy();protection=Image.new('L',base.size)
def polygon(points):
 im=Image.new('L',base.size);ImageDraw.Draw(im).polygon([(x-76,y-304) for x,y in points],fill=255);return np.array(im)>0
def box(rect):
 im=Image.new('L',base.size);ImageDraw.Draw(im).rectangle((rect[0]-76,rect[1]-304,rect[2]-76,rect[3]-304),fill=255);return np.array(im)>0
protected=np.zeros((1121,788),bool)
# Entire reward surround and heading with its native decorative arrows.
# Only the broad central illustration face may carry repaired lacquer.
reward=box((137,1247,810,1407))
inside=polygon([(195,1284),(306,1284),(326,1302),(625,1302),(646,1284),(738,1284),(751,1300),(754,1315),(750,1328),(758,1335),(749,1344),(752,1366),(741,1394),(201,1394),(190,1377),(185,1355),(194,1335),(185,1317)])
protected|=reward&~inside
# Preserve the complete moves medallion: the example '28' is source artwork.
protected|=box((482,1082,615,1223))
# Keep all six source card borders; repaired faces stop before the bevels.
for x0,y0,x1,y1,inset in [(115,761,266,906,14),(273,761,423,906,14),(430,761,583,906,14),(115,1067,232,1189,14),(233,1067,350,1189,14),(351,1067,478,1189,14)]:
 face=polygon([(x0+inset+7,y0+inset),(x1-inset-7,y0+inset),(x1-inset,y0+inset+7),(x1-inset,y1-inset-7),(x1-inset-7,y1-inset),(x0+inset+7,y1-inset),(x0+inset,y1-inset-7),(x0+inset,y0+inset+7)])
 protected|=box((x0,y0,x1,y1))&~face
rgb[protected]=original[protected]
# The source Firebird itself crosses two parts of its frame. Repair those small
# hidden spans with the corresponding complete source frame, not a flat patch.
donor=np.zeros_like(protected)
for x0,y0,x1,y1 in [(299,769,409,779),(294,891,410,902)]:
 ys=slice(y0-304,y1-304);xs=slice(x0-76,x1-76)
 rgb[ys,xs]=original[ys,slice(x0-76-157,x1-76-157)]
 donor[ys,xs]=True
protected[donor]=False
Image.fromarray(donor.astype('uint8')*255).save(b/'Sources/HiddenFrameDonorMask.png')
Image.fromarray(protected.astype('uint8')*255).save(b/'Sources/ProtectedOrnaments.png')
# Russian remains exact over every protected ornament/label region.
ru=Image.fromarray(rgb).convert('RGBA');ru.putalpha(base.getchannel('A'));ru.save(dst/'panel_ru_r4.png')
# Apply the same original-frame restoration to the existing localized panel.
en=np.array(Image.open(dst/'panel.png').convert('RGBA'));en[:,:,:3][protected]=original[protected];en[:,:,:3][donor]=rgb[donor]
# Remove source letters only inside the English reward/moves captions.
# No rectangle surface repair is allowed to reach the frame or decorative arrows.
ink=np.zeros((1121,788),np.uint8)
for rect in [(397,1264,542,1291),(519,1116,576,1140)]:
 x0,y0,x1,y1=rect;x0-=76;x1-=76;y0-=304;y1-=304
 ink[y0:y1,x0:x1]=255
en[:,:,:3]=cv2.inpaint(en[:,:,:3],ink,4,cv2.INPAINT_TELEA);Image.fromarray(en).save(dst/'panel_r4.png');Image.fromarray(ink).save(b/'Sources/EnglishLetterRepair.png')
assert np.array_equal(np.array(ru)[:,:,:3][protected],original[protected])
keep=protected&(ink==0);assert np.array_equal(en[:,:,:3][keep],original[keep])
# Original-rim crop comparisons are inspection evidence, not native screenshots.
for name,im in [('Original',source),('Before',base),('After',ru)]:
 im.crop((50,934,735,1106)).resize((1370,344)).save(b/'QA'/('Reward'+name+'.png'))
for name,color in [('Black','black'),('White','white'),('Magenta','magenta')]:
 sheet=Image.new('RGB',(1576,1121),color)
 for i,im in enumerate([ru,Image.fromarray(en)]):sheet.paste(im,(788*i,0),im)
 sheet.save(b/'QA'/('Alpha'+name+'.png'))
report={'source':'../NativeOriginalR3/Sources/Reference.png','protectedSourcePixelsRU':int(protected.sum()),'protectedSourcePixelsEN':int(keep.sum()),'hiddenFrameDonorPixels':int(donor.sum()),'sourceRGBEqualRU':True,'sourceRGBEqualENOutsideLetterRepair':True,'alphaUnchanged':True,'scenarioJobs':0,'generatedArt':False,'outputs':{name:hashlib.sha256((dst/name).read_bytes()).hexdigest() for name in ['panel_ru_r4.png','panel_r4.png']}}
(b/'QA/OrnamentChecks.json').write_text(json.dumps(report,indent=2)+'\n');print(json.dumps(report))
