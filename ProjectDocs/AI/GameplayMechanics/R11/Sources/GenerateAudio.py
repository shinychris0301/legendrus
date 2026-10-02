"""Original deterministic synthesized sound design. No recordings or third-party samples."""
from pathlib import Path
import math, random, struct, wave, json, hashlib
root=Path(__file__).resolve().parents[5]
out=root/'Assets/LegendsOfRus/Resources/MechanicsAudioR11';out.mkdir(exist_ok=True)
rate=44100
specs={'swap':(.10,650),'reject':(.16,190),'match':(.24,1500),'create':(.38,740),'bomb':(.52,95),'line':(.30,400),'flight':(.52,680),'arrival':(.24,920),'color':(.52,1050),'mace':(.36,130),'firebird':(.44,290),'renewal':(.58,640),'wood':(.22,210),'ice':(.28,2200),'metal':(.30,830),'seal':(.34,440)}
records=[]
for index,(name,(duration,freq)) in enumerate(specs.items()):
 rng=random.Random(1100+index);values=[];low=0
 for n in range(int(duration*rate)):
  t=n/rate;u=t/duration;noise=rng.uniform(-1,1);low+=.065*(noise-low)
  attack=min(1,t/.003);tail=(1-u)**2
  if name in ['match','ice','metal','arrival']:
   value=sum(math.sin(2*math.pi*freq*k*t)*math.exp(-t*(12+j*8)) / (j+1) for j,k in enumerate([1,1.51,2.73,4.12]))*.42+noise*math.exp(-t*35)*.2
   if name=='match':value+=.2*noise*max(0,1-abs(t-.045)/.018)
  elif name in ['bomb','mace','wood','firebird']:
   value=.7*math.sin(2*math.pi*(freq*t-freq*.32*t*t/duration))*math.exp(-t*10)+low*2.5*math.exp(-t*5)+noise*.25*math.exp(-t*45)
   if name=='wood':value+=.4*noise*max(0,1-abs(t-.065)/.012)
  elif name in ['line','flight']:
   value=noise*.3*math.sin(math.pi*u)**1.3+math.sin(2*math.pi*(freq*t+750*t*t))*.2*math.sin(math.pi*u)
  elif name in ['create','color','renewal','seal']:
   value=0
   for j,k in enumerate([1,1.25,1.5,2]):
    v=t-j*.035
    if v>=0:value+=.26*math.sin(2*math.pi*freq*k*v)*math.exp(-v*7)*min(1,v/.006)
   value+=noise*.08*math.exp(-t*24)
  else:value=math.sin(2*math.pi*(freq*t-freq*.25*t*t/duration))*math.exp(-t*25)*.65
  values.append(value*attack*tail)
 peak=max(abs(v) for v in values);gain=.72/peak
 path=out/(name+'.wav')
 with wave.open(str(path),'wb') as w:
  w.setparams((1,2,rate,len(values),'NONE','not compressed'));w.writeframes(b''.join(struct.pack('<h',round(v*gain*32767)) for v in values))
 records.append({'name':name,'seconds':duration,'sampleRate':rate,'peak':.72,'rms':(sum((v*gain)**2 for v in values)/len(values))**.5,'sha256':hashlib.sha256(path.read_bytes()).hexdigest()})
(root/'ProjectDocs/AI/GameplayMechanics/R11/QA/AudioAssets.json').write_text(json.dumps(records,indent=2))
print('Generated',len(records),'original mono cues')
