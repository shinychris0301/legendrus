from pathlib import Path
from PIL import Image,ImageDraw,ImageChops
import json,hashlib,uuid
b=Path(__file__).resolve().parent
old=b.parents[1]/'R1/Sources'
out=Path('Assets/LegendsOfRus/Resources/ProductionUI/DailyRewardAligned')
src=Image.open('Assets/LegendsOfRus/Resources/DesignScreens/DailyReward.png').convert('RGBA')
records={}
def save(name,im,method):
 p=out/(name+'.png');im.save(p)
 meta=p.with_suffix('.png.meta')
 if not meta.exists():
  template=(out/'back_r2.png.meta').read_text();guid=next(x for x in template.splitlines() if x.startswith('guid:'))
  meta.write_text(template.replace(guid,'guid: '+uuid.uuid4().hex))
 records[name]={'sha256':hashlib.sha256(p.read_bytes()).hexdigest(),'size':im.size,'method':method}
claim=src.crop((39,659,246,723))
a=Image.open(b/'Scenario/claim/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(claim.size,Image.Resampling.LANCZOS)
# Remove adjacent card sides retained by segmentation. Keep opaque emerald button face.
outline=Image.new('L',claim.size);ImageDraw.Draw(outline).polygon([(0,34),(12,25),(20,10),(29,2),(178,2),(187,11),(196,25),(206,34),(193,42),(184,58),(176,62),(30,62),(20,56),(12,43)],fill=255)
a=ImageChops.multiply(a,outline);ImageDraw.Draw(a).polygon([(28,13),(178,13),(190,31),(179,52),(28,52),(17,33)],fill=255)
claim.putalpha(a);save('claim_ru_r4',claim,'Original crop [39,659,246,723] RGB, Scenario Pixa alpha with adjacent card rails excluded; original ПОЛУЧИТЬ lettering retained.')
blank=claim.copy();surface=Image.open(out/'claim_blank_r3.png').convert('RGBA').crop((80,36,405,92)).resize((154,38),Image.Resampling.LANCZOS)
blank.paste(surface,(27,14));blank.putalpha(a);save('claim_blank_r4',blank,'Source claim perimeter plus reused R1 Scenario emerald material in lettering area; live English label.')
confirm=src.crop((225,1502,715,1631));alpha=Image.open(old/'Scenario/claim/booster_medallion_trial01.png').convert('RGBA').getchannel('A').resize(confirm.size,Image.Resampling.LANCZOS)
ImageDraw.Draw(alpha).rectangle((60,28,426,96),fill=255);confirm.putalpha(alpha)
save('confirm_ru_r4',confirm,'Exact original crop [225,1502,715,1631] RGB including ОТЛИЧНО; reused R1 Scenario alpha.')
(b/'AssetProvenance.json').write_text(json.dumps({'newScenarioCU':3,'assets':records},indent=2,ensure_ascii=False))
for label,color in [('Black','black'),('White','white'),('Magenta','#ff00aa')]:
 sheet=Image.new('RGB',(560,350),color)
 for name,xy in [('claim_ru_r4',(20,20)),('claim_blank_r4',(300,20)),('confirm_ru_r4',(35,150))]:
  im=Image.open(out/(name+'.png'));sheet.paste(im,xy,im)
 sheet.save(b.parent/'QA'/('Alpha'+label+'.png'))
