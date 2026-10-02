from pathlib import Path
from PIL import Image
import numpy as np,json
b=Path(__file__).resolve().parents[1];root=Path('Assets/LegendsOfRus/Resources/ProductionUI');dst=root/'TutorialOriginal';ref=Image.open(b/'Sources/Reference.png').convert('RGB');records={}
for key,box,rails in [('done',(202,1415,738,1557),[(90,10,440,17),(90,117,440,127)]),('first',(248,1551,700,1634),[(62,8,389,13),(62,67,389,74)])]:
 source=np.array(ref.crop(box));ru=np.array(Image.open(dst/(key+'_ru_r5.png')));en=np.array(Image.open(dst/(key+'_r5.png')));mask=np.array(Image.open(b/'Sources'/(key+'_LetterMask.png')))>0
 assert np.array_equal(source,ru[:,:,:3]);assert np.array_equal(en[:,:,:3][~mask],source[~mask]);assert np.array_equal(ru[:,:,3],en[:,:,3])
 for x0,y0,x1,y1 in rails:assert (ru[y0:y1,x0:x1,3]==255).all()
 records[key]={'originalRussianRGBUnchanged':True,'englishOutsideLetteringMaskUnchanged':True,'topAndBottomRailsFullyOpaque':True}
old=np.array(Image.open(root/'HomeReferenceAligned/source_gift.png').convert('RGBA'));gift=np.array(Image.open(dst/'gift_medallion_r5.png'));assert np.array_equal(old[:,:,:3],gift[:,:,:3]);assert gift[:12,:12,3].max()==0;assert gift[-12:,-12:,3].max()==0
records['gift']={'originalRGBUnchanged':True,'removedOpaqueBackgroundPixels':int(((old[:,:,3]>0)&(gift[:,:,3]==0)).sum())}
(b/'QA/ExtractionChecks.json').write_text(json.dumps(records,indent=2)+'\n');print(records)
