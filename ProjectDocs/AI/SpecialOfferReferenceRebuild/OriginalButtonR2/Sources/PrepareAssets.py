from pathlib import Path
from PIL import Image
import hashlib,json,uuid
import numpy as np
b=Path(__file__).resolve().parent;root=Path('Assets/LegendsOfRus/Resources/ProductionUI');out=root/'SpecialOfferAligned'
source=Image.open('ProjectDocs/AI/SpecialOfferReferenceRebuild/R1/Sources/OriginalReference.png').convert('RGBA');records={}
def save(key,im,method):
 p=out/(key+'.png');im.save(p)
 meta=p.with_suffix('.png.meta')
 if not meta.exists():
  template=(out/'hero_source_r2.png.meta').read_text();old=next(x for x in template.splitlines() if x.startswith('guid:'));meta.write_text(template.replace(old,'guid: '+uuid.uuid4().hex))
 records[key]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'size':im.size,'method':method}
buy=source.crop((173,1263,770,1388));buy.putalpha(Image.open(root/'TutorialAligned/next.png').getchannel('A'))
save('buy_lettered_r3',buy,'Original button RGB including full КУПИТЬ СЕЙЧАС lettering, reused Scenario next-button alpha. No regeneration or text removal.')
for lang in ['ru','blank']:
 panel=Image.open(out/('panel_'+lang+'_r2.png')).convert('RGBA');save('header_'+lang+'_r3',panel.crop((0,0,885,278)),'Dedicated title header from approved '+lang+' panel extraction, including original crest and gold divider. Same scale and position.')
# Clear the header portion of the body so translucent edges are drawn only once.
body=Image.open(out/'panel_blank_r2.png').convert('RGBA');alpha=np.array(body.getchannel('A'));alpha[:278]=0;body.putalpha(Image.fromarray(alpha));save('panel_body_r3',body,'Existing repaired panel body with transparent header region; dedicated header renders crest/lettering once.')
arr=np.array(buy);orig=np.array(source.crop((173,1263,770,1388)));records['sourceButtonRgbMismatchPixels']=int(np.any(arr[:,:,:3]!=orig[:,:,:3],axis=2).sum())
(b/'AssetProvenance.json').write_text(json.dumps(records,indent=2))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(925,470),color)
 for key,xy in [('header_ru_r3',(20,20)),('buy_lettered_r3',(164,324))]:
  im=Image.open(out/(key+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
print(records)
