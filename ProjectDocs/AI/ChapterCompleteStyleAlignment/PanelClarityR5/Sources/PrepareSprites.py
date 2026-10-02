"""Blend one Scenario navy material inside existing panels, preserving all alpha and frame pixels."""
from pathlib import Path
import hashlib,json
import numpy as np
from PIL import Image,ImageOps,ImageFilter
r=Path.cwd();o=r/'ProjectDocs/AI/ChapterCompleteStyleAlignment/PanelClarityR5';p=json.loads((o/'Sources/Provenance.json').read_text());generated=Image.open(o/'Sources/ScenarioNavy/booster_medallion_trial01.png').convert('RGB');sample=Image.open(r/p['source']).convert('RGB').crop(p['navyReferenceBox']);mean=np.asarray(sample,dtype=float).mean(axis=(0,1));audit=[]
for name,box in p['repairBoxes'].items():
 source=Path('/tmp/lor-chaptercomplete-r5/before/Assets/LegendsOfRus/Resources/ProductionUI/ChapterCompleteAligned')/(name+'.png');im=Image.open(source).convert('RGBA');x,y,x1,y1=box;w,h=x1-x,y1-y
 patch=ImageOps.fit(generated,(w,h),method=Image.Resampling.LANCZOS);a=np.asarray(patch,dtype=float);a=np.clip(a*mean/np.maximum(1,a.mean(axis=(0,1))),0,255).astype('uint8');patch=Image.fromarray(a)
 border=4;mask=ImageOps.expand(Image.new('L',(w,h),255),border=border,fill=0).filter(ImageFilter.GaussianBlur(2)).crop((border,border,w+border,h+border));im.paste(patch,(x,y),mask)
 out=o/'Staging'/(name+'.png');im.save(out);audit.append({'name':name,'size':im.size,'repairBox':box,'sha256':hashlib.sha256(out.read_bytes()).hexdigest(),'role':'Scenario navy material, source mean color matched, edge-blended within existing surface; original alpha/frame preserved'})
for name,col in [('Black',(0,0,0)),('White',(255,255,255)),('Magenta',(200,20,170))]:
 sheet=Image.new('RGB',(960,560),col)
 for asset,xy in [('header_blank',(8,8)),('reward_card',(8,410))]:
  im=Image.open(o/'Staging'/(asset+'.png')).convert('RGBA');sheet.paste(im,xy,im)
 sheet.save(o/'QA'/('AssetsOn'+name+'.png'))
(o/'QA/ExtractionAudit.json').write_text(json.dumps(audit,indent=2)+'\n');print('Prepared',len(audit),'assets')
