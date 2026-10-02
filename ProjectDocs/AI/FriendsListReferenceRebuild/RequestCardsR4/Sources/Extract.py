from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json,hashlib,cv2
b=Path(__file__).resolve().parent;dst=Path('Assets/LegendsOfRus/Resources/ProductionUI/FriendsListAligned')
atlas=Image.open(b/'RefinedInputs/frames.png').convert('RGBA');alpha=Image.open(b/'Scenario/frames/booster_medallion_trial01.png').convert('RGBA').resize(atlas.size,Image.Resampling.LANCZOS).getchannel('A')
bounds=json.loads((b/'Bounds.json').read_text());records={}
for name,entry in bounds.items():
 box=entry['atlasBounds'];im=atlas.crop(box);a=np.array(alpha.crop(box));w,h=im.size
 # Retain intentionally opaque panel faces; exclude neighboring scenery at the corners.
 core=Image.new('L',(w,h));d=ImageDraw.Draw(core)
 inset=28*entry['scale'];d.rectangle((inset,inset,w-inset,h-inset),fill=255);a=np.maximum(a,np.asarray(core))
 boundary=Image.new('L',(w,h));d=ImageDraw.Draw(boundary)
 if name=='card':
  points=[(25,0),(w-25,0),(w-8,20),(w-8,h//2-24),(w,h//2),(w-8,h//2+24),(w-8,h-20),(w-25,h),(25,h),(8,h-20),(8,h//2+24),(0,h//2),(8,h//2-24),(8,20)]
  d.polygon(points,fill=255)
 else:
  d.rounded_rectangle((0,0,w-1,h-14),radius=22,fill=255)
  # Preserve full original bottom blue jewel restored after submission.
  d.polygon([(w//2-85,h-54),(w//2+85,h-54),(w//2+35,h-18),(w//2,h-2),(w//2-35,h-18)],fill=255)
  a[h-55:h-14,24:w-24]=255
 a=np.minimum(a,np.asarray(boundary));im.putalpha(Image.fromarray(a))
 key='request_card_r4' if name=='card' else 'requests_surround_r4';im.save(dst/(key+'.png'));records[key]={'sha256':hashlib.sha256((dst/(key+'.png')).read_bytes()).hexdigest(),'bounds':entry}
# Show opaque navy faces and thin isolated borders over diagnostic backgrounds.
proof=Image.new('RGB',(1200,1120));d=ImageDraw.Draw(proof)
for i,key in enumerate(records):
 im=Image.open(dst/(key+'.png'));im.thumbnail((380,850),Image.Resampling.LANCZOS)
 y=0 if i==0 else 180
 for j,col in enumerate([(0,0,0),(255,255,255),(255,0,150)]):
  x=j*400;d.rectangle((x,y,x+399,y+im.height+45),fill=col);proof.paste(im,(x+10,y+30),im);d.text((x+10,y+5),key,fill=(150,150,150))
proof.save(b.parent/'QA/AssetAlpha.png')
(b/'AssetProvenance.json').write_text(json.dumps({'source':'RequestsReference.png','model':'model_pixa-background-removal','assets':records,'repairs':'Empty navy source donor fill, mirrored occluded left trim, reconstructed interrupted top/bottom rail portions, source surround sapphire restored after submission. Source-derived RGB; hidden surfaces and contours approximate.'},indent=2))
