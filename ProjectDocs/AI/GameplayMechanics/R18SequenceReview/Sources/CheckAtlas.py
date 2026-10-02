from PIL import Image
from pathlib import Path
import json,hashlib
r=Path(__file__).resolve().parents[1];res=[]
for name,rows in [('EffectsAtlas.png',[0,330,660,890,1254]),('MaterialsAndFormation.png',[0,314,618,960,1254])]:
 im=Image.open(r/'Art'/name);w,h=im.size;a=im.getchannel('A');cells=[]
 for y in range(4):
  for x in range(4):
   l=x*w//4;rt=(x+1)*w//4;top=rows[y];bot=rows[y+1];cr=a.crop((l,top,rt,bot));edge=max(cr.crop((0,0,cr.width,1)).getextrema()[1],cr.crop((0,cr.height-1,cr.width,cr.height)).getextrema()[1],cr.crop((0,0,1,cr.height)).getextrema()[1],cr.crop((cr.width-1,0,cr.width,cr.height)).getextrema()[1]);cells.append({'index':y*4+x,'sourceRegion':[l,top,rt-l,bot-top],'boundaryAlphaMax':edge})
 res.append({'file':name,'size':[w,h],'mode':im.mode,'alphaRange':a.getextrema(),'sha256':hashlib.sha256((r/'Art'/name).read_bytes()).hexdigest(),'regions':cells})
(r/'QA/AtlasRegions.json').write_text(json.dumps(res,indent=2));bad=[c for s in res for c in s['regions'] if c['boundaryAlphaMax']>16];print(json.dumps({'sprites':32,'boundaryFailures':bad}));assert not bad
