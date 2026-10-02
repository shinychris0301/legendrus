from pathlib import Path
from PIL import Image,ImageDraw,ImageFilter
import numpy as np,json
b=Path(__file__).resolve().parent; src=Image.open(b/'Inbox.png').convert('RGB');res=Path('Assets/LegendsOfRus/Resources/ProductionUI')
navy=Image.open(res/'TeamRankingAligned/table_panel_r4.png').convert('RGB');navy=navy.crop((navy.width//3,navy.height//3,navy.width*2//3,navy.height*2//3))
blue=Image.open(res/'FriendsListAligned/tab_0_active_r2.png').convert('RGB');blue=blue.crop((blue.width//3,blue.height//3,blue.width*2//3,blue.height*2//3))
# Joined rails, curls and separators stay source pixels; lettering footprints get clean approved lacquer.
tabs=src.crop((30,262,911,351)); mask=Image.new('L',tabs.size);d=ImageDraw.Draw(mask)
for box in [(88,31,267,64),(340,31,554,64),(628,31,787,64)]: d.rectangle(box,fill=255)
# Source blue face is replaced entirely so selection can move between all three tabs.
poly=[(69,22),(285,22),(295,32),(295,64),(283,73),(70,73),(61,63),(61,34)]
d.polygon(poly,fill=255)
tabs.paste(navy.resize(tabs.size), (0,0),mask.filter(ImageFilter.GaussianBlur(1)))
# Remove the two baked unread badges; reconstruct occluded top rails from visible same-tab left corners.
for dst,left in [((276,0,320,46),(48,0,92,46)),((805,0,849,46),(574,0,618,46))]:
 piece=tabs.crop(left).transpose(Image.Transpose.FLIP_LEFT_RIGHT);tabs.paste(piece,dst[:2])
# Mark background above rail as staging grey to prevent remnants being selected.
a=np.array(tabs);a[:7]=[230,230,230];tabs=Image.fromarray(a)
portrait=src.crop((74,1049,276,1288));pm=Image.new('L',portrait.size);ImageDraw.Draw(pm).rectangle((21,173,181,224),fill=255)
portrait.paste(navy.resize(portrait.size),(0,0),pm.filter(ImageFilter.GaussianBlur(1)))
sheet=Image.new('RGB',(1820,760),(230,230,230));sheet.paste(tabs.resize((1762,178),Image.Resampling.LANCZOS),(29,20));sheet.paste(portrait.resize((404,478),Image.Resampling.LANCZOS),(700,240));sheet.save(b/'Sheet.png')
(b/'Bounds.json').write_text(json.dumps({'tabs_joined_r2':{'source':[30,262,911,351],'sheet':[29,20,1791,198]},'preview_portrait_r2':{'source':[74,1049,276,1288],'sheet':[700,240,1104,718]},'repairs':'Tab captions and inactive/selected faces use approved navy. Badge occlusion repaired with mirrored same-tab left rails. Portrait plaque caption footprint uses approved navy. RGB never generated.','navy':'TeamRankingAligned/table_panel_r4.png','selected':'FriendsListAligned/tab_0_active_r2.png'},indent=2))
