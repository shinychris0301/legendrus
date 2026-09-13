"""Install whole generated images. User-authorized alpha cleanup only: near-opaque alpha restored to 255, original RGB untouched."""
from pathlib import Path
from PIL import Image
import numpy as np,json,shutil,uuid,hashlib
r=Path.cwd();d=r/'ProjectDocs/AI/ChapterMapStyleAlignment/R1';q=d/'QA';q.mkdir(exist_ok=True);out=r/'Assets/LegendsOfRus/Resources/ProductionUI/ChapterMapAligned';out.mkdir(exist_ok=True)
reports=[]
for name,rec in json.loads((d/'Generation.json').read_text()).items():
 src=Path(rec['source']); im=Image.open(src).convert('RGBA');arr=np.array(im);a=arr[:,:,3];dst=out/(name+'.png')
 changed=int(((a>=245)&(a<255)).sum()) if name!='environment' else 0
 if name!='environment':
  a[a>=245]=255;im=Image.fromarray(arr);im.save(dst)
 else:shutil.copyfile(src,dst)
 meta=dst.with_suffix('.png.meta')
 if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n')
 row={'name':name,'size':im.size,'sourceMode':Image.open(src).mode,'alphaMin':int(a.min()),'transparentPixels':int((a==0).sum()),'opaquePixels':int((a==255).sum()),'partialAlphaPixels':int(((a>0)&(a<255)).sum()),'centerAlpha':int(a[a.shape[0]//2,a.shape[1]//2]),'nearOpaqueAlphaRestored':changed,'rgbUnchanged':bool(np.array_equal(arr[:,:,:3],np.array(Image.open(src).convert('RGBA'))[:,:,:3])),'sourceSha256':hashlib.sha256(src.read_bytes()).hexdigest(),'runtimeSha256':hashlib.sha256(dst.read_bytes()).hexdigest()};reports.append(row)
 if name!='environment':
  assert a.min()==0 and row['centerAlpha']==255
  thumb=im.copy();thumb.thumbnail((650,350));board=Image.new('RGB',(thumb.width*3,thumb.height))
  for i,c in enumerate(['black','white','#ff00c0']):
   layer=Image.new('RGBA',thumb.size,c);layer.alpha_composite(thumb);board.paste(layer,(i*thumb.width,0))
  board.save(q/(name+'_alpha_review.png'))
(d/'AssetAlphaChecks.json').write_text(json.dumps(reports,indent=2)+'\n');print(json.dumps(reports))
b=d/'UnityVerification/Before';b.mkdir(parents=True,exist_ok=True)
for p in Path('/tmp/lor-chaptermap-r1/baseline/ChapterMap').iterdir():shutil.copyfile(p,b/p.name)
shutil.copyfile('/tmp/lor-chaptermap-r1/baseline/results.xml',b/'BaselinePlayMode.xml')
