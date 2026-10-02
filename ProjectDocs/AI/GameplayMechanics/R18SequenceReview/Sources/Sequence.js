'use strict';
// Offline art composition, not the Unity board resolver. All times are authored review beats.
const c=document.querySelector('#board'),g=c.getContext('2d'),W=940,H=1672,base='../../../../Assets/LegendsOfRus/Resources/ProductionUI/';
const keys=['ruby','sapphire','emerald','gold','amethyst','orange'],images={},sprites={};
const urls={env:base+'GameplayHeaderSceneryR10/environment.png',frame:base+'GameplayAligned/board_frame_r7.png',grid:base+'GameplaySourceGridR11/grid.png',fx:'Art/EffectsAtlas.png',mat:'Art/MaterialsAndFormation.png'};
keys.forEach(k=>urls[k]=base+'GameplayReferenceAligned/gem_'+k+'.png');
['line','bomb','crate','crate_damaged'].forEach(k=>urls[k]=base+'MechanicsBoardArtR10/'+k+'.png');
const clamp=(v,a=0,b=1)=>Math.min(b,Math.max(a,v)),ease=v=>1-(1-clamp(v))**3,smooth=v=>{v=clamp(v);return v*v*(3-2*v)},rnd=k=>{const v=Math.sin(k*91.17+9.2)*15371.71;return v-Math.floor(v)};
const cell=99,bx=74,by=461,cx=col=>bx+(col+.5)*cell,cy=row=>by+(row+.5)*cell;
let t=0,last=0,playing=!matchMedia('(prefers-reduced-motion: reduce)').matches,ready=false,sound=false,audioCtx;
const lineCells=[[3,1],[3,2],[3,3],[3,4]],bombCells=[[4,1],[4,2],[4,3],[5,3],[6,3]];
const same=(a,x,y)=>a.some(p=>p[0]===x&&p[1]===y);
const gem=(x,y)=>same(lineCells,x,y)?1:same(bombCells,x,y)?3:(x+2*y)%6;
function cropSheet(key,bounds){const im=images[key],off=document.createElement('canvas');off.width=im.width;off.height=im.height;const ctx=off.getContext('2d',{willReadFrequently:true});ctx.drawImage(im,0,0);const data=ctx.getImageData(0,0,im.width,im.height).data;
 sprites[key]=[];for(let row=0;row<4;row++)for(let col=0;col<4;col++){const l=Math.floor(col*im.width/4),r=Math.floor((col+1)*im.width/4),top=bounds[row],bot=bounds[row+1];let x0=r,y0=bot,x1=l,y1=top;for(let y=top;y<bot;y++)for(let x=l;x<r;x++)if(data[(y*im.width+x)*4+3]>8){x0=Math.min(x0,x);y0=Math.min(y0,y);x1=Math.max(x1,x);y1=Math.max(y1,y)}sprites[key].push([Math.max(l,x0-3),Math.max(top,y0-3),Math.min(r-1,x1+3)-Math.max(l,x0-3)+1,Math.min(bot-1,y1+3)-Math.max(top,y0-3)+1]);}}
function draw(im,x,y,w,h=w,a=1,rot=0){g.save();g.globalAlpha=clamp(a);g.translate(x,y);g.rotate(rot);g.drawImage(im,-w/2,-h/2,w,h);g.restore()}
function sprite(key,i,x,y,w,a=1,rot=0,flip=false){const r=sprites[key][i];g.save();g.globalAlpha=clamp(a);g.translate(x,y);g.rotate(rot);if(flip)g.scale(-1,1);const h=w*r[3]/r[2];g.drawImage(images[key],...r,-w/2,-h/2,w,h);g.restore()}
function blend(key,start,count,u,x,y,w,a=1){const f=clamp(u)*(count-1),i=Math.floor(f),s=f-i;sprite(key,start+i,x,y,w,a*(1-s));if(s>.001)sprite(key,start+Math.min(i+1,count-1),x,y,w,a*s)}
function label(s,x,y,size,color='#eed499'){g.fillStyle=color;g.font=`${size}px Georgia,serif`;g.textAlign='center';g.fillText(s,x,y)}
function soft(x,y,r,col,a){g.save();g.globalAlpha=clamp(a);const v=g.createRadialGradient(x,y,0,x,y,r);v.addColorStop(0,col);v.addColorStop(1,'transparent');g.fillStyle=v;g.fillRect(x-r,y-r,r*2,r*2);g.restore()}
function plaque(x,y,w,h){g.fillStyle='#07121feb';g.fillRect(x,y,w,h);g.strokeStyle='#ac813e';g.lineWidth=3;g.strokeRect(x,y,w,h);g.strokeStyle='#594627';g.lineWidth=1;g.strokeRect(x+8,y+8,w-16,h-16)}
function material(i,x,y,age,seed,n=4,force=1){if(age<0||age>760)return;const u=age/1000,fade=1-smooth((age-380)/380);for(let k=0;k<n;k++){const a=k*2.399+rnd(seed)*3,v=(110+rnd(seed+k*4)*190)*force;const px=x+Math.cos(a)*v*u,py=y+(Math.sin(a)*v-130)*u+390*u*u;const size=(12+rnd(seed+k*9)*18)*(1+.3*Math.sin(u*Math.PI));sprite('mat',i,px,py,size,fade,(rnd(seed+k)-.5)*u*12,k%2===0);}}
function hit(x,y){if(x>=2&&x<=4||y>=2&&y<=4)return 5250+Math.min(x>=2&&x<=4?Math.abs(y-3):99,y>=2&&y<=4?Math.abs(x-3):99)*57;return Infinity}
function chainHit(x,y){return Math.abs(x-6)<=2&&Math.abs(y-4)<=2?6500+Math.hypot(x-6,y-4)*35:Infinity}
function blast(x,y,age,scale=1){if(age<0||age>820)return;const u=clamp(age/650),size=(110+ease(age/230)*250)*scale;blend('fx',4,4,u,x,y,size,1-smooth((age-440)/380));material(14,x,y,age,41,7,1.4*scale);material(15,x,y,age,97,4,1.3*scale)}
function projectiles(time){const age=time-5250;if(age<0||age>600)return;for(let off=-1;off<=1;off++)for(const vertical of [false,true])for(const sign of [-1,1]){const dis=age*1.74;if(dis>470)continue;const x=cx(3)+(vertical?off*cell:sign*(dis-55)),y=cy(3)+(vertical?sign*(dis-55):off*cell);sprite('fx',8+Math.floor(age/48)%4,x,y,181,1-smooth((age-220)/180),vertical?sign*Math.PI/2:0,!vertical&&sign===-1);}}
function phase(time){if(time<2200)return [0,'РОЖДЕНИЕ СИЛЫ','Formation · directional item','Matched jewels gather into a crystal body. The final pose hands off to the approved idle item.'];if(time<4200)return [1,'СОЗДАНИЕ БОМБЫ','Formation · bomb','Golden pieces assemble into the shell, then a charged bomb settles beside the line item.'];if(time<5100)return [2,'СОЕДИНЕНИЕ СИЛ','Combination · anticipation','Both powers converge. A short compression precedes the combined release.'];if(time<6000)return [3,'МОЩНЫЙ УДАР','Combined release','Three rows and three columns are struck. Gems fracture at the travelling attack fronts.'];if(time<6480)return [4,'ЦЕПНАЯ РЕАКЦИЯ','Damage → chain reaction','The damaged crate remains visible while the secondary bomb charges.'];if(time<7400)return [4,'ЦЕПНАЯ РЕАКЦИЯ','Chain reaction · final break','The secondary bomb destroys the remaining crate layer with wooden debris.'];return [5,'НОВЫЕ ВОЗМОЖНОСТИ','Recovery','Smoke and debris recede; the scripted refill settles into readable cells.'];}
function render(time){g.clearRect(0,0,W,H);g.drawImage(images.env,0,0,W,H);g.fillStyle='#03102035';g.fillRect(0,0,W,H);plaque(110,75,720,130);label('МАГИЯ САМОЦВЕТОВ',470,133,38);label('LEGENDS OF RUS · R18',470,177,20,'#9dbbd8');const p=phase(time);label(p[1],470,329,35);label('ART & MOTION PREVIEW',470,370,18,'#bed0df');
 let recoil=time>=5250&&time<5450?Math.exp(-(time-5250)/70)*5:time>=6500&&time<6640?Math.exp(-(time-6500)/60)*4:0;
 g.save();g.translate(Math.sin(time*.09)*recoil,Math.cos(time*.07)*recoil);g.fillStyle='#160f08f2';g.fillRect(bx-13,by-13,cell*8+26,cell*7+26);g.strokeStyle='#cb9b4f';g.lineWidth=4;g.strokeRect(bx-13,by-13,cell*8+26,cell*7+26);
 for(let y=0;y<7;y++)for(let x=0;x<8;x++){g.fillStyle=(x+y)%2?'#211b12':'#1d180f';g.fillRect(bx+x*cell,by+y*cell,cell,cell);}
 for(let y=0;y<=7;y++)for(let x=0;x<8;x++)g.drawImage(images.grid,4,4,80,10,bx+x*cell,by+y*cell-5,cell,10);
 for(let x=0;x<=8;x++)for(let y=0;y<7;y++)g.drawImage(images.grid,92,4,10,80,bx+x*cell-5,by+y*cell,10,cell);
 for(let y=0;y<=7;y++)for(let x=0;x<=8;x++)g.drawImage(images.grid,4,24,20,24,bx+x*cell-10,by+y*cell-12,20,24);
 g.drawImage(images.frame,bx-28,by-28,cell*8+56,cell*7+56);{}
 const parts=[];for(let y=0;y<7;y++)for(let x=0;x<8;x++){const X=cx(x),Y=cy(y),n=gem(x,y),h=hit(x,y),ch=chainHit(x,y);let skip=false;
 if(x===6&&y===2){const first=h,final=ch;if(time<final){draw(images[time<first?'crate':'crate_damaged'],X,Y,86);if(time>=first)material(6,X,Y,time-first,44,2,.55)}else material(6,X,Y,time-final,81,7,1.45);skip=true;}
 if(x===6&&y===4){if(time<6500){const q=clamp((time-5900)/600);draw(images.bomb,X,Y,76+q*9,76-q*8,1,Math.sin(time*.08)*q*.09);if(q>0)soft(X,Y,80,'#ffb137',q*.4)}skip=true;}
 if(same(lineCells,x,y)&&time>=800&&time<1830){const u=smooth((time-800)/430);draw(images.sapphire,X+(cx(3)-X)*u,Y+(cy(3)-Y)*u,79*(1-.68*u),79*(1-.68*u),1-u);skip=true;}
 if(same(bombCells,x,y)&&time>=2820&&time<4010){const u=smooth((time-2820)/450);draw(images.gold,X+(cx(4)-X)*u,Y+(cy(3)-Y)*u,79*(1-.7*u),79*(1-.7*u),1-u);skip=true;}
 if(x===3&&y===3&&time>=1230&&time<5250)skip=true;
 if(x===4&&y===3&&time>=3270&&time<5250)skip=true;
 const first=Math.min(h,ch);if(time>=first){skip=true;if(!(x===6&&(y===2||y===4))&&!(y===3&&(x===3||x===4)))parts.push([ [1,0,2,3,4,5][n],X,Y,time-first,x+y*8,4,1.3]);}
 if(time>=7500&&Number.isFinite(first)){const u=clamp((time-7500-y*37-x*9)/460),d=1-ease(u);if(u>0)draw(images[keys[(n+2)%6]],X,Y-110*d,79,79,clamp(u*4));skip=true;}
 if(!skip){let ix=X,iy=Y,show=n;if(time<800&&x===3&&y===2){show=0;ix-=cell*smooth((time-430)/300)}if(time<800&&x===2&&y===2){show=1;ix+=cell*smooth((time-430)/300)}if(time>=800&&x===2&&y===2)show=0;
 if(time<2820&&x===4&&y===2){show=0;if(time>=2450)ix+=cell*smooth((time-2450)/300)}if(time<2820&&x===5&&y===2){show=3;if(time>=2450)ix-=cell*smooth((time-2450)/300)}if(time>=2820&&x===5&&y===2)show=0;
 if(same(lineCells,x,y)&&!(x===3&&y===3)&&time>=1830){show=(n+3)%6;iy-=70*(1-ease((time-1830)/350));}if(same(bombCells,x,y)&&!(x===4&&y===3)&&time>=4010){show=(n+2)%6;iy-=70*(1-ease((time-4010)/350));}draw(images[keys[show]],ix,iy,79);}}
 if(time>=1180&&time<1850){const u=clamp((time-1180)/550);blend('fx',0,4,u,cx(3),cy(3),135,1-smooth((time-1740)/110));}
 if(time>=1730&&time<5100){const merge=ease((time-4690)/410);draw(images.line,cx(3)+merge*cell*.45,cy(3)-Math.sin(merge*Math.PI)*28,85*(1+.06*Math.sin(time*.013)),72,1,merge*.22);}
 if(time>=3240&&time<4050)blend('mat',8,4,clamp((time-3240)/600),cx(4),cy(3),91,1-smooth((time-3950)/100));
 if(time>=3940&&time<5100){const merge=ease((time-4690)/410);draw(images.bomb,cx(4)-merge*cell*.55,cy(3)+Math.sin(merge*Math.PI)*17,84,88,1,-merge*.15);}
 if(time>=5100&&time<5250){draw(images.bomb,cx(3),cy(3),102,83);soft(cx(3),cy(3),125,'#ffdb65',.65);}
 blast(cx(3),cy(3),time-5250,1.16);projectiles(time);parts.forEach(p=>material(...p));blast(cx(6),cy(4),time-6500,.96);
 g.restore();plaque(130,1265,680,164);label(p[1],470,1324,30);const ru=['Четыре самоцвета — новая сила','Соберите L или T — создайте бомбу','Две силы соединяются','Удар по трём рядам и столбцам','Преграда меняется после удара','Поле готово к следующему ходу'][p[0]];label(ru,470,1377,24,'#d2dee8');
 for(let i=0;i<6;i++){g.fillStyle=i<=p[0]?'#e4b75d':'#263445';g.fillRect(178+i*100,1467,83,5)}label('ПРЕДПРОСМОТР · НЕ ЗАПИСЬ UNITY',470,1563,18,'#c0ccd5');document.querySelector('#phase').textContent=p[2];document.querySelector('#explanation').textContent=p[3];}
function tick(now){const old=t;if(last&&playing)t=(t+Math.min(now-last,60)*Number(document.querySelector('#speed').value))%10200;last=now;if(sound&&playing&&t>old)audioBeats(old,t);if(ready)render(t);document.querySelector('#time').value=t;document.querySelector('#clock').textContent=(t/1000).toFixed(2)+' s';requestAnimationFrame(tick)}
function button(){document.querySelector('#play').textContent=playing?'Pause':'Play'}
function seek(ms){t=ms;playing=false;button();if(ready)render(t)}
window.reviewSeek=seek;window.reviewInfo=()=>({time:t,playing,ready,sprites});
document.querySelector('#play').onclick=()=>{playing=!playing;button()};document.querySelector('#replay').onclick=()=>{t=0;playing=true;button()};document.querySelector('#time').oninput=e=>seek(+e.target.value);document.querySelectorAll('[data-time]').forEach(b=>b.onclick=()=>seek(+b.dataset.time));
document.querySelector('#backing').onchange=e=>document.querySelector('#atlas').style.background=e.target.value;document.querySelector('#sheet').onchange=e=>document.querySelector('#atlas').src=e.target.value;
function tone(freq,duration,gain,drop=1){const a=audioCtx,o=a.createOscillator(),v=a.createGain();o.type='sine';o.frequency.setValueAtTime(freq,a.currentTime);o.frequency.exponentialRampToValueAtTime(Math.max(25,freq*drop),a.currentTime+duration);v.gain.setValueAtTime(0,a.currentTime);v.gain.linearRampToValueAtTime(gain,a.currentTime+.008);v.gain.exponentialRampToValueAtTime(.0001,a.currentTime+duration);o.connect(v).connect(a.destination);o.start();o.stop(a.currentTime+duration+.01)}
function boom(){tone(130,.34,.18,.22);const a=audioCtx,b=a.createBuffer(1,a.sampleRate*.25,a.sampleRate),d=b.getChannelData(0);for(let i=0;i<d.length;i++)d[i]=(rnd(i)-.5)*Math.pow(1-i/d.length,2);const s=a.createBufferSource(),v=a.createGain(),f=a.createBiquadFilter();s.buffer=b;v.gain.value=.2;f.type='lowpass';f.frequency.value=1900;s.connect(f).connect(v).connect(a.destination);s.start()}
function audioBeats(a,b){for(const ms of [1230,1430,1630,3270,3470,3670])if(a<ms&&b>=ms)tone(ms<2000?660+(ms-1230)*1.1:440+(ms-3270)*.9,.18,.06,1.25);for(const ms of [5250,6500])if(a<ms&&b>=ms)boom();}
document.querySelector('#audio').onclick=async()=>{audioCtx??=new (window.AudioContext||window.webkitAudioContext)();await audioCtx.resume();sound=!sound;document.querySelector('#audio').textContent=sound?'Sound on':'Sound off';document.querySelector('#audio').setAttribute('aria-pressed',sound);};
Promise.all(Object.entries(urls).map(([key,path])=>new Promise((res,rej)=>{const im=new Image();im.onload=()=>{images[key]=im;res()};im.onerror=()=>rej(Error(path));im.src=path}))).then(()=>{cropSheet('fx',[0,330,660,890,images.fx.height]);cropSheet('mat',[0,314,618,960,images.mat.height]);ready=true;window.sequenceReady=true;button();requestAnimationFrame(tick)}).catch(e=>{document.querySelector('#phase').textContent='Artwork failed to load: '+e.message;console.error(e)});
