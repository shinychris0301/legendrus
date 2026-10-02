from pathlib import Path
from PIL import Image
import json,hashlib,shutil,cv2,numpy as np
b=Path('ProjectDocs/AI/TeamChatStyleAlignment/R1');s=b/'Sources'
for n in ['Crops','Inputs','RepairMasks','Scenario','Final']:(s/n).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True)
source=Path('/home/k2/Downloads/match3final/ChatGPT Image Aug 25, 2026, 09_00_51 PM (4).png');shutil.copy2(source,s/'OriginalReference.png');im=Image.open(source).convert('RGB')
assets={
 'title_ru':((147,124,793,291),[]),
 'portrait_yaroslav':((56,410,184,552),[]),
 'portrait_alena':((59,551,184,685),[]),
 'portrait_mikhail':((60,683,182,810),[]),
 'portrait_svetlana':((59,808,182,928),[]),
 'portrait_viktor':((55,1018,185,1138),[]),
 'portrait_nikolai':((58,1138,185,1263),[]),
 'chest':((638,269,829,416),[]),
 'message_frame':((184,446,864,548),[(215,462,841,530)]),
 'composer_frame':((123,1404,675,1487),[(147,1429,476,1468)]),
 'send_frame':((679,1406,898,1486),[(707,1427,875,1465)]),
 'gift':((139,1494,249,1589),[]),
 'help':((405,1518,531,1588),[]),
 'sticker':((687,1494,790,1587),[])
}
records=[]
for name,(box,repairs) in assets.items():
 crop=im.crop(box);crop.save(s/'Crops'/(name+'.png'));rgb=np.array(crop);mask=np.zeros(rgb.shape[:2],'uint8')
 for x1,y1,x2,y2 in repairs:mask[y1-box[1]:y2-box[1],x1-box[0]:x2-box[0]]=255
 if repairs:rgb=cv2.inpaint(rgb,mask,5,cv2.INPAINT_TELEA)
 Image.fromarray(rgb).save(s/'Inputs'/(name+'_native.png'));Image.fromarray(rgb).resize((crop.width*3,crop.height*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(name+'.png'));Image.fromarray(mask).save(s/'RepairMasks'/(name+'.png'));records.append({'name':name,'box':box,'repairRegions':repairs,'operation':'Scenario alpha only; original RGB except bounded local text removal'})
(s/'Assessment.json').write_text(json.dumps({'source':str(source),'sha256':hashlib.sha256(source.read_bytes()).hexdigest(),'assets':records,'reuse':'Existing clean Ranking background, shared header, selected fonts, TeamHub guild crest, existing frame materials. No full-page screenshot used at runtime.'},indent=2))
p=Path('ProjectDocs/AI/TeamRankingStyleAlignment/R1/Sources/ScenarioRequests.py').read_text().replace('Team Ranking','Team Chat');(s/'ScenarioRequests.py').write_text(p)
