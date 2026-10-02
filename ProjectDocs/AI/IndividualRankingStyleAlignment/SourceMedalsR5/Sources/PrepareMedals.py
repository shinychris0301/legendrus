"""Apply Scenario alpha to unchanged original RGB; remove disconnected adjacent crowns."""
from pathlib import Path
from PIL import Image,ImageDraw
import numpy as np,cv2,json,hashlib
b=Path(__file__).resolve().parent;out=b.parent/'Extracted';out.mkdir(exist_ok=True);qa=b.parent/'QA';qa.mkdir(exist_ok=True)
records=[];sheet=Image.new('RGB',(3*320,3*270))
for i,name in enumerate(['medal_gold','medal_silver','medal_bronze']):
 original=Image.open(b/(name+'_original.png')).convert('RGB');inputImage=Image.open(b/(name+'_input.png')).convert('RGB')
 api=Image.open(b/'Scenario'/name/'booster_medallion_trial01.png').convert('RGBA');a=np.array(api.getchannel('A').resize(inputImage.size,Image.Resampling.LANCZOS))
 n,labels,stats,_=cv2.connectedComponentsWithStats((a>2).astype(np.uint8),8);keep=1+int(np.argmax(stats[1:,cv2.CC_STAT_AREA]));selected=(labels==keep).astype(np.uint8);selected=cv2.dilate(selected,np.ones((3,3),np.uint8));removed=int(((a>0)&(selected==0)).sum());a[selected==0]=0
 # Preserve source artwork and face; no numeral removal, face replacement, recolor or sharpening.
 size=(original.width*3,original.height*3);rgb=original.resize(size,Image.Resampling.LANCZOS);alpha=Image.fromarray(a).resize(size,Image.Resampling.LANCZOS)
 edge=np.array(alpha);edge[0]=0;edge[-1]=0;edge[:,0]=0;edge[:,-1]=0;alpha=Image.fromarray(edge)
 final=rgb.convert('RGBA');final.putalpha(alpha);final.save(out/(name+'.png'))
 native=original.convert('RGBA');native.putalpha(Image.fromarray(a).resize(original.size,Image.Resampling.LANCZOS));native.save(out/(name+'_native.png'))
 assert np.array_equal(np.array(final)[:,:,:3],np.array(rgb))
 assert np.array_equal(np.array(native)[:,:,:3],np.array(original))
 npAlpha=np.array(alpha);assert npAlpha[0].max()==0 and npAlpha[-1].max()==0 and npAlpha[:,0].max()==0 and npAlpha[:,-1].max()==0
 for j,bg in enumerate([(0,0,0),(255,255,255),(210,0,145)]):
  tile=Image.new('RGBA',(320,270),bg+(255,));tile.alpha_composite(final,((320-final.width)//2,8));ImageDraw.Draw(tile).text((12,245),name,fill=(255,255,255) if j!=1 else (0,0,0));sheet.paste(tile.convert('RGB'),(i*320,j*270))
 records.append({'asset':name,'originalSourceBox':json.loads((b/'Source.json').read_text())['boxes'][name],'originalRGBPreserved':True,'nativeRGBAOriginalRGBIdentical':True,'runtimeRGBEqualsLanczos3xSource':True,'numeral':'original fixed source pixels','cleanup':'retain largest connected medal component; reject adjacent next-row crown; clear one-pixel resampling fringe at canvas edge','removedAlphaPixelsAt8x':removed,'size':size,'transparentCanvasEdges':True,'sha256':hashlib.sha256((out/(name+'.png')).read_bytes()).hexdigest(),'billing':json.loads((b/'Scenario'/name/'Billing.json').read_text())})
sheet.save(qa/'TransparencyInspection.png');(b.parent/'AssetProvenance.json').write_text(json.dumps(records,indent=2));print(json.dumps(records,indent=2))
