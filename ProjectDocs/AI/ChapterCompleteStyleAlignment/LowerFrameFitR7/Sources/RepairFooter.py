"""Restore original lower corner scrolls obscured by the rectangular wood repair. No generation."""
from pathlib import Path
import json,hashlib
import numpy as np
from PIL import Image,ImageDraw,ImageFilter
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/LowerFrameFitR7';p=json.loads((o/'Sources/Provenance.json').read_text());source=Image.open(r/p['source']).convert('RGB').crop(p['footerBox']);old=Image.open('/tmp/lor-chaptercomplete-r7/before/Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned/footer.png').convert('RGBA');out=old.copy();mask=Image.new('L',out.size)
large=Image.new('L',(out.width*4,out.height*4))
for polygon in p['repairPolygons']:
 ImageDraw.Draw(large).polygon([(x*4,y*4) for x,y in polygon],fill=255)
mask=large.resize(out.size,Image.Resampling.LANCZOS).filter(ImageFilter.GaussianBlur(.6))
out.paste(source,(0,0),mask);out.putalpha(old.getchannel('A'));out.save(o/'Staging/footer.png');mask.save(o/'Sources/RepairMask.png')
for name,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 canvas=Image.new('RGBA',out.size,col+(255,));canvas.alpha_composite(out);canvas.convert('RGB').save(o/'QA'/('FooterOn'+name+'.png'))
# Demonstrate that the formerly obscured inner curls are exactly restored from original source RGB.
arr=np.asarray(out);src=np.asarray(source);orig=np.asarray(old);m=np.asarray(mask)
assert np.array_equal(arr[:,:,3],orig[:,:,3]);assert np.array_equal(arr[m==0],orig[m==0]);assert np.array_equal(arr[m==255,:3],src[m==255])
(o/'QA/PixelRepairAudit.json').write_text(json.dumps({'alphaUnchanged':True,'pixelsOutsideRepairMaskUnchanged':True,'opaqueRestorationPixelsMatchSource':True,'restoredOpaquePixelCount':int((m==255).sum()),'changedPixelCount':int(np.any(arr!=orig,axis=2).sum())},indent=2)+'\n')
(o/'QA/ExtractionAudit.json').write_text(json.dumps([{'name':'footer','size':out.size,'sha256':hashlib.sha256((o/'Staging/footer.png').read_bytes()).hexdigest()}],indent=2)+'\n');print('Repaired original scrolls')
