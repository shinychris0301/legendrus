"""Diagnostic shape comparison only. Font binaries remain outside the project.
Whole-word normalization permits size/width fitting; scores are NOT identity probabilities.
"""
from pathlib import Path
from PIL import Image,ImageFont,ImageDraw
from fontTools.ttLib import TTFont
import numpy as np,cv2,json,hashlib
B=Path('ProjectDocs/AI/TypographyIdentification/R1');D=Path('/tmp/lor-font-identification')
texts={'title':'ЛИЧНЫЙ РЕЙТИНГ','name_masha':'Маша','name_ilya':'Илья Муромец','name_vasilisa':'Василиса','name_koschey':'Кощей','numbers_score':'1 985 300','rank_1':'1','rank_2':'2','rank_3':'3','rank_4':'4','rank_10':'10','rank_12':'12','tab_season':'СЕЗОН'}
def tight(a):
 ys,xs=np.where(a>0);return a[ys.min():ys.max()+1,xs.min():xs.max()+1]
def source(key,threshold=110):
 a=np.array(Image.open(B/'Samples'/f'{key}.png').convert('RGB')).astype(float)
 threshold=175 if key.startswith('rank_') else threshold
 m=((a[:,:,0]>threshold)&(((a[:,:,0]>a[:,:,2]*1.3)&(a[:,:,1]>a[:,:,2]*1.08))|((a.min(2)>175)&((a.max(2)-a.min(2))<85)))).astype('uint8')
 # Eliminate tiny detached background specks, retaining accent marks.
 n,l,s,_=cv2.connectedComponentsWithStats(m,8)
 for i in range(1,n):
  if s[i,cv2.CC_STAT_AREA]<2:m[l==i]=0
 return tight(m)*255
paths=list((D/'fonts').rglob('*.ttf'))
for folder in ['/usr/share/fonts/truetype/liberation','/usr/share/fonts/truetype/liberation2','/usr/share/fonts/truetype/freefont','/usr/share/fonts/truetype/dejavu','/usr/share/fonts/truetype/noto']:
 for p in Path(folder).glob('*'):
  if p.suffix=='.ttf' and 'Italic' not in p.name and any(n in p.name for n in ['LiberationSerif','FreeSerif','DejaVuSerif','NotoSerif-','NotoSerifDisplay-']):
   if any(n in p.name for n in ['Regular','Bold','Medium','SemiBold']):paths.append(p)
records=[];fonts={};excluded=[]
for p in paths:
 t=TTFont(p); cmap=t.getBestCmap()
 if not all(ord(c) in cmap for c in ''.join(texts.values()) if not c.isspace()):excluded.append(str(p));continue
 fam=t['name'].getDebugName(1);style=t['name'].getDebugName(2)
 weights=[None]
 if 'fvar' in t and any(x.axisTag=='wght' for x in t['fvar'].axes):weights=[400,500,600,700]
 for wt in weights:
  f=ImageFont.truetype(str(p),100)
  if wt:
   f.set_variation_by_axes([max(x.minValue,min(x.maxValue,wt)) if x.axisTag=='wght' else x.defaultValue for x in t['fvar'].axes])
  name=f'{fam} {style}'+(f' w{wt}' if wt else '')
  if name in fonts:continue
  fonts[name]=f
  record={'name':name,'path':str(p),'weight':wt,'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'samples':{}}
  for k,txt in texts.items():
   bb=f.getbbox(txt);r=Image.new('L',(bb[2]-bb[0]+8,bb[3]-bb[1]+8));ImageDraw.Draw(r).text((4-bb[0],4-bb[1]),txt,font=f,fill=255)
   a=tight(np.array(r));s=source(k);h,w=s.shape;scaled=cv2.resize(a,(w,h),interpolation=cv2.INTER_AREA)>110;m=s>0
   # Distance measured on source grid, plus overlap loss. Exact family not inferable from this score.
   d1=cv2.distanceTransform((~m).astype('uint8'),cv2.DIST_L2,3);d2=cv2.distanceTransform((~scaled).astype('uint8'),cv2.DIST_L2,3)
   chamfer=(d1[scaled].mean()+d2[m].mean())/2
   iou=np.sum(scaled&m)/np.sum(scaled|m)
   score=float(chamfer+.7*(1-iou))
   record['samples'][k]={'error':score,'iou':float(iou),'widthFactorAfterHeightFit':float((w/h)/(a.shape[1]/a.shape[0]))}
  record['bodyError']=float(np.mean([record['samples'][k]['error'] for k in texts if k.startswith('name_')]))
  record['rankError']=float(np.mean([record['samples'][k]['error'] for k in texts if k.startswith('rank_')]))
  records.append(record)
(B/'ComparisonMetrics.json').write_text(json.dumps({'method':'Normalized silhouette screening; lower error better within this candidate set; not confidence or proof of identity. Lettering effects and low-resolution masks confound scores.','candidates':records,'excludedMissingCyrillic':excluded},indent=2,ensure_ascii=False))
for group in ['body','title','rank']:
 key=(lambda r:r['bodyError']) if group=='body' else (lambda r:r['rankError']) if group=='rank' else (lambda r:r['samples']['title']['error'])
 ranked=sorted(records,key=key);print(group,[(r['name'],round(key(r),3)) for r in ranked[:8]])
 selected=ranked[:8];samplekeys=['name_ilya','name_vasilisa','name_koschey'] if group=='body' else ['title'] if group=='title' else ['rank_1','rank_2','rank_3','rank_4','rank_10','rank_12']
 canvas=Image.new('RGB',(1500,100+(len(selected)+1)*115),'#18202a');draw=ImageDraw.Draw(canvas);draw.text((12,10),'ANALYSIS SPECIMENS — not a Unity preview. Each word fitted to reference bounds.',fill='white')
 for j,rec in enumerate([None]+selected):
  y=45+j*115;draw.text((12,y),'ORIGINAL' if rec is None else rec['name'],fill='white');x=330
  for k in samplekeys:
   s=source(k);targeth=60;targetw=round(s.shape[1]*targeth/s.shape[0])
   if rec is None:
    im=Image.fromarray(s).resize((targetw,targeth));tint=Image.new('RGB',im.size,'#e7c47c');bg=Image.new('RGB',im.size,'#18202a');bg.paste(tint,mask=im);im=bg
   else:
    f=fonts[rec['name']];txt=texts[k];bb=f.getbbox(txt);im=Image.new('L',(bb[2]-bb[0]+8,bb[3]-bb[1]+8));ImageDraw.Draw(im).text((4-bb[0],4-bb[1]),txt,font=f,fill=255);im=Image.fromarray(tight(np.array(im))).resize((targetw,targeth));tint=Image.new('RGB',im.size,'#e7c47c');bg=Image.new('RGB',im.size,'#18202a');bg.paste(tint,mask=im);im=bg
   if x+im.width>1495:im=im.resize((1495-x,im.height))
   canvas.paste(im,(x,y+20));x+=im.width+22
 canvas.save(B/f'{group.title()}Comparison.png')
print('Compared',len(records),'font instances.')
