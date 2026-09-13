"""QA only: render declared sprite rectangles over contrasting backgrounds; original atlas bytes unchanged."""
from pathlib import Path
from PIL import Image,ImageDraw
import re,json,hashlib
root=Path(__file__).resolve().parents[5];doc=root/'ProjectDocs/AI/GameplayStyleAlignment/R1';art=root/'Assets/LegendsOfRus/Resources/ProductionUI/GameplayAligned'
s=(root/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionGameplayArt.cs').read_text();records=[];tiles=[]
for sheet,name,x,y,w,h in re.findall(r'new Region\("(.*?)","(.*?)",(\d+),(\d+),(\d+),(\d+)\)',s):
 x,y,w,h=map(int,(x,y,w,h));im=Image.open(art/(sheet+'.png')).convert('RGBA');piece=im.crop((x,y,x+w,y+h));records.append(dict(sheet=sheet,name=name,topLeftRect=[x,y,w,h],alphaRange=piece.getchannel('A').getextrema()));piece.thumbnail((125,125));tile=Image.new('RGB',(405,153),'#15212a');d=ImageDraw.Draw(tile);d.text((8,132),name,fill='white')
 for k,c in enumerate(['black','white','#ff00d8']):
  bg=Image.new('RGBA',(133,130),c);bg.alpha_composite(piece,((133-piece.width)//2,(130-piece.height)//2));tile.paste(bg.convert('RGB'),(k*135,0))
 tiles.append(tile)
for start in range(0,len(tiles),6):
 page=Image.new('RGB',(810,459),'#15212a')
 for j,t in enumerate(tiles[start:start+6]):page.paste(t,((j%2)*405,(j//2)*153))
 page.save(doc/'QA'/f'atlas_alpha_{start//6+1}.png')
(doc/'AtlasRegions.json').write_text(json.dumps(records,indent=2)+'\n')
print('Inspected',len(records),'declared regions; source copies',[(n,hashlib.sha256((art/(n+'.png')).read_bytes()).hexdigest()==hashlib.sha256((doc/'Sources'/(n+'.png')).read_bytes()).hexdigest()) for n in ['gems','boosters','obstacles']])
