"""Whole generated resources; local alpha cleanup under the user's existing authorization."""
from pathlib import Path
from PIL import Image
import numpy as np,json,shutil,uuid,hashlib
r=Path.cwd();d=r/'ProjectDocs/AI/PreLevelStyleAlignment/R1';sources=d/'Sources';sources.mkdir(exist_ok=True);out=r/'Assets/LegendsOfRus/Resources/ProductionUI/PreLevelAligned';out.mkdir(exist_ok=True)
sha=lambda p:hashlib.sha256(p.read_bytes()).hexdigest()
records=json.loads((d/'Generation.json').read_text());reports=[]
for name,rec in records.items():
 src=sources/(name+'.png')
 if not src.exists():shutil.copyfile(rec['cacheSource'],src)
 rec['source']=str(src.relative_to(r));original=Image.open(src);rgb=np.array(original.convert('RGB'));restored=0
 assert original.mode=='RGBA', 'Reviewed generated sources must retain their original alpha.'
 arr=np.array(original);method='Generated RGBA silhouette preserved; near-opaque alpha restored.'
 a=arr[:,:,3];a[a>=245]=255;im=Image.fromarray(arr);dst=out/(name+'.png');im.save(dst)
 meta=dst.with_suffix('.png.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n')
 assert a.min()==0 and a[a.shape[0]//2,a.shape[1]//2]==255
 opaque=a==255;assert np.array_equal(arr[opaque,:3],rgb[opaque])
 reports.append({'name':name,'size':im.size,'sourceMode':original.mode,'sourceSha256':sha(src),'runtimeSha256':sha(dst),'method':method,'opaqueSourceRgbPreserved':True,'restoredNavyInteriorPixels':restored,'transparentPixels':int((a==0).sum()),'opaquePixels':int(opaque.sum()),'partialAlphaPixels':int(((a>0)&(a<255)).sum())})
 thumb=im.copy();thumb.thumbnail((500,280));board=Image.new('RGB',(thumb.width*3,thumb.height))
 for i,c in enumerate(['black','white','#ff00c0']):
  layer=Image.new('RGBA',thumb.size,c);layer.alpha_composite(thumb);board.paste(layer,(i*thumb.width,0))
 board.save(d/'QA'/(name+'_alpha_review.png'))
 print(name,method,flush=True)
(d/'Generation.json').write_text(json.dumps(records,indent=2)+'\n');(d/'AssetAlphaChecks.json').write_text(json.dumps(reports,indent=2)+'\n')
b=d/'UnityVerification/Before';b.mkdir(parents=True,exist_ok=True)
for p in Path('/tmp/lor-prelevel-r1/baseline/PreLevel').iterdir():shutil.copyfile(p,b/p.name)
shutil.copyfile('/tmp/lor-prelevel-r1/baseline/results.xml',b/'BaselinePlayMode.xml')
