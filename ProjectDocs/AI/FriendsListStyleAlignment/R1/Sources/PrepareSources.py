from pathlib import Path
from PIL import Image
import numpy as np,cv2,json,hashlib,shutil,subprocess
b=Path('ProjectDocs/AI/FriendsListStyleAlignment/R1');s=b/'Sources'
for n in ['Crops','Inputs','RepairMasks','Scenario','Final']:(s/n).mkdir(parents=True,exist_ok=True)
(b/'QA').mkdir(exist_ok=True)
refs={}
for n,f in [('roster','friends.png'),('requests','friend.png')]:
 p=Path('/home/k2/Downloads/match3final')/f;shutil.copy2(p,s/(n+'.png'));refs[n]={'source':str(p),'sha256':hashlib.sha256(p.read_bytes()).hexdigest()}
assets=[('title_ru','roster',(248,115,699,324),[]),('energy_frame','roster',(552,465,801,535),[(628,480,779,522)]),('visit_frame','roster',(552,536,801,597),[(609,550,755,580)]),('accept','requests',(669,487,758,579),[]),('decline','requests',(779,488,867,579),[])]
for i,(y1,y2) in enumerate([(447,605),(611,769),(777,938),(944,1101),(1105,1261),(1271,1420)]):assets.append(('portrait_'+str(i),'roster',(92,y1,249,y2),[]))
for i,(y1,y2) in enumerate([(441,635),(632,812),(810,992),(990,1178),(1176,1361)]):assets.append(('request_'+str(i),'requests',(58,y1,253,y2),[]))
for n,box in [('gift_red',(569,493,654,581)),('gift_blue',(568,669,654,762)),('gift_green',(566,846,654,941))]:assets.append((n,'requests',box,[]))
records=[]
for n,ref,box,rep in assets:
 im=Image.open(s/(ref+'.png')).convert('RGB').crop(box);im.save(s/'Crops'/(n+'.png'));rgb=np.array(im);m=np.zeros(rgb.shape[:2],'uint8')
 for x1,y1,x2,y2 in rep:m[y1-box[1]:y2-box[1],x1-box[0]:x2-box[0]]=255
 if rep:rgb=cv2.inpaint(rgb,m,5,cv2.INPAINT_TELEA)
 im=Image.fromarray(rgb);im.save(s/'Inputs'/(n+'_native.png'));im.resize((im.width*3,im.height*3),Image.Resampling.LANCZOS).save(s/'Inputs'/(n+'.png'));Image.fromarray(m).save(s/'RepairMasks'/(n+'.png'));records.append({'name':n,'reference':ref,'box':box,'repairs':rep})
(s/'Assessment.json').write_text(json.dumps({'references':refs,'assets':records,'reuse':'Approved clean scenery, account header, row/tab materials and footer navigation art. No screenshot page.'},indent=2))
p=Path('ProjectDocs/AI/TeamRankingStyleAlignment/R1/Sources/ScenarioRequests.py').read_text().replace('Team Ranking','Friends List');(s/'ScenarioRequests.py').write_text(p)
