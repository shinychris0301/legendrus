from pathlib import Path
from PIL import Image,ImageDraw
import cv2,numpy as np,json,hashlib
b=Path('ProjectDocs/AI/IndividualRankingStyleAlignment/ReferenceLayoutR2/Sources')
for d in ['Crops','Inputs','RepairMasks']:(b/d).mkdir(exist_ok=True)
ref=Image.open(b/'OriginalReference.png').convert('RGB')
portraits={
'masha':(155,610,247,692),'ilya':(154,684,247,765),'firebird':(154,754,246,835),'vasilisa':(154,818,247,895),'cat':(154,880,248,959),'koschey':(154,941,247,1020),'dobrynya':(154,1002,247,1080),'yaga':(154,1064,247,1139),'alyosha':(154,1120,247,1200),'zmei':(153,1179,248,1250),'ivan':(150,1253,250,1343),'solovey':(153,1329,248,1410),'vodyanoi':(154,1388,248,1463),'leshy':(154,1466,248,1542)}
icons={'medal_gold':(40,615,134,692),'medal_silver':(40,686,134,763),'medal_bronze':(40,758,134,832),'rank_plain':(58,833,111,890),'rank_player':(54,1272,113,1328),'chest_gold':(72,429,217,548),'chest_silver':(365,441,497,548),'chest_bronze':(660,447,791,548),'chest_purple':(751,824,881,895),'power':(526,635,560,669),'gem':(236,475,258,504),'card':(236,506,270,541),'clock':(525,263,563,307),'back':(20,1533,130,1652)}
frames={'title':(113,107,856,276),'tab':(120,310,343,379),'tab_active':(606,310,827,379),'rules':(247,1548,697,1629),'reward_frame':(65,419,345,551),'row':(30,894,910,954),'row_player':(28,1264,914,1339),'reward_panel':(16,379,925,578),'table_frame':(17,575,924,1546),'footer':(6,1534,935,1663)}
records=[]
for group,boxes in [('portraits',portraits),('icons',icons),('frames',frames)]:
 for key,box in boxes.items():
  name='portrait_'+key if group=='portraits' else key;im=ref.crop(box);im.save(b/'Crops'/f'{name}.png');rgb=np.array(im);h,w=rgb.shape[:2];mask=np.zeros((h,w),np.uint8);fixed=rgb.copy()
  if name in ['medal_gold','medal_silver','medal_bronze','rank_plain','rank_player']:
   center={'medal_gold':(29,16,w-25,h-14),'medal_silver':(29,16,w-25,h-14),'medal_bronze':(29,16,w-25,h-14),'rank_plain':(17,12,w-13,h-11),'rank_player':(13,12,w-12,h-9)}[name]
   x0,y0,x1,y1=center;mask[y0:y1,x0:x1]=255;fixed=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
  if group=='frames':
   if name in ['title','tab','tab_active','rules']:
    bounds={'title':(52, sixty:=65,w-54,131),'tab':(63,19,w-55,49),'tab_active':( sixty:=62,19,w-38,49),'rules':(65,25,w-54,57)}[name]
    x0,y0,x1,y1=bounds;part=rgb[y0:y1,x0:x1].astype(float);gold=(part[:,:,0]>part[:,:,2]*1.25)&(part[:,:,0]>65)&(part[:,:,1]>35)
    mask[y0:y1,x0:x1]=gold.astype(np.uint8)*255;mask=cv2.dilate(mask,np.ones((5,5),np.uint8));fixed=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
   else:
    inset=12 if name in ['reward_frame','reward_panel','table_frame'] else 7
    mask[inset:h-inset,inset:w-inset]=255
    # Every composited icon/label inside the panel is removed before extraction.
    donor=ref.crop((406,631,490,669)) if name in ['row','reward_frame'] else ref.crop((774,276,815,308))
    if name=='row_player':donor=ref.crop((676,1280,733,1325))
    tile=np.array(donor);tile=np.concatenate([tile,tile[:,::-1]],1);tile=np.concatenate([tile,tile[::-1]],0);tile=np.tile(tile,(h//tile.shape[0]+1,w//tile.shape[1]+1,1))[:h,:w]
    fixed[mask>0]=tile[mask>0]
    if name=='footer':
     # Back and Rules elements obscure this surface: this is reconstruction, not recovered pixels.
     mask[:]=255;fixed=tile.copy()
  Image.fromarray(mask).save(b/'RepairMasks'/f'{name}.png');Image.fromarray(fixed).save(b/'Inputs'/f'{name}.png')
  records.append({'name':name,'group':group,'sourceBox':list(box),'repairedPixels':int((mask>0).sum()),'classification':'original sprite isolation' if group!='frames' else 'source ornament isolation with bounded blank-face reconstruction','retainOpaqueInterior':group in ['frames','portraits'],'inputSHA256':hashlib.sha256((b/'Inputs'/f'{name}.png').read_bytes()).hexdigest()})
# Two sparse atlases keep exact per-item mappings and avoid a separate request for every tiny icon.
for group in ['portraits','icons']:
 items=[r for r in records if r['group']==group];sheet=Image.new('RGB',(1024,1024),(85,89,98))
 for i,r in enumerate(items):
  im=Image.open(b/'Inputs'/(r['name']+'.png'));scale=min(224/im.width,224/im.height);size=(round(im.width*scale),round(im.height*scale));im=im.resize(size,Image.Resampling.LANCZOS)
  x=(i%4)*256+(256-size[0])//2;y=(i//4)*256+(256-size[1])//2;sheet.paste(im,(x,y));r['atlasBox']=[x,y,x+size[0],y+size[1]];r['atlas']=group
 sheet.save(b/'Inputs'/(group+'_atlas.png'))
# Panels require their opaque faces retained; isolate each distinctive outline without a packed atlas.
(b/'AssetAssessment.json').write_text(json.dumps({'source':'OriginalReference.png','sha256':hashlib.sha256((b/'OriginalReference.png').read_bytes()).hexdigest(),'assets':records,'background':'Generate only clean dark navy birch-forest scenery, which is hidden by the source interface. No characters or UI regeneration.'},indent=2)+'\n')
print('Prepared',len(records),'components, two small-item atlases; production untouched.')
