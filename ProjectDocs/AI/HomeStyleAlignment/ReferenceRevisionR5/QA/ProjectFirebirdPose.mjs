import fs from 'node:fs';
import zlib from 'node:zlib';
function png(file) {
 const b=fs.readFileSync(file);let w,h,bpp,parts=[];
 for(let off=8;off<b.length;){const n=b.readUInt32BE(off),type=b.toString('ascii',off+4,off+8),data=b.subarray(off+8,off+8+n);if(type==='IHDR'){w=data.readUInt32BE(0);h=data.readUInt32BE(4);if(data[8]!==8||![2,6].includes(data[9])||data[12]!==0)throw Error('Unsupported inspection PNG '+file);bpp=data[9]===6?4:3;}if(type==='IDAT')parts.push(data);off+=n+12;}
 const raw=zlib.inflateSync(Buffer.concat(parts)),stride=w*bpp,pixels=Buffer.alloc(w*h*bpp);let src=0;
 function paeth(a,b,c){const p=a+b-c,pa=Math.abs(p-a),pb=Math.abs(p-b),pc=Math.abs(p-c);return pa<=pb&&pa<=pc?a:pb<=pc?b:c;}
 for(let y=0;y<h;y++){const f=raw[src++];for(let x=0;x<stride;x++){const i=y*stride+x,a=x>=bpp?pixels[i-bpp]:0,up=y>0?pixels[i-stride]:0,c=y>0&&x>=bpp?pixels[i-stride-bpp]:0;let v=raw[src++];if(f===1)v+=a;else if(f===2)v+=up;else if(f===3)v+=Math.floor((a+up)/2);else if(f===4)v+=paeth(a,up,c);else if(f!==0)throw Error('Unknown PNG filter');pixels[i]=v&255;}}
 return{w,h,bpp,pixels};
}

const dir='ProjectDocs/AI/HomeStyleAlignment/ReferenceRevisionR5';
const bird=png(dir+'/Sources/home_firebird_generated.png'),ivan=png('Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleReconstructed/slavic_fantasy_prince_warrior_portrait.png');
function alpha(p,x,y){if(x<0||y<0||x>=p.w||y>=p.h)return 0;return p.bpp===4?p.pixels[(y*p.w+x)*4+3]:255;}
const cases=[{x:467,y:442,width:550,angle:10},{x:495,y:442,width:550,angle:15},{x:495,y:442,width:540,angle:15},{x:490,y:442,width:540,angle:15}];
for(const q of cases){
 const s=q.width/bird.w,rad=q.angle*Math.PI/180,c=Math.cos(rad),t=Math.sin(rad),tf=(x,y)=>[q.x+s*(c*x-t*y),q.y+s*(t*x+c*y)];
 const out={...q,height:q.width*1.5,beakTip:tf(62,219),frontClaw:tf(135,590),rearClaw:tf(210,655),solidWingBounds:{minX:1e9,minY:1e9,maxX:-1,maxY:-1},solidWingClipped:0,headBeakSolid:0,headBeakOverIvan:0};
 for(let y=0;y<bird.h;y++)for(let x=0;x<bird.w;x++)if(alpha(bird,x,y)>=245){const [wx,wy]=tf(x,y);if(x>=270&&y>=150&&y<=750){const b=out.solidWingBounds;b.minX=Math.min(b.minX,wx);b.maxX=Math.max(b.maxX,wx);b.minY=Math.min(b.minY,wy);b.maxY=Math.max(b.maxY,wy);if(wx>941)out.solidWingClipped++;}if(x<=90&&y>=160&&y<=250){out.headBeakSolid++;if(alpha(ivan,Math.floor((wx-50)/.625),Math.floor((wy-490)/.625))>=128)out.headBeakOverIvan++;}}
 q.analysis=out;
}
const result={kind:'Read-only source alpha projection: bird top-left clockwise screen-space pose; Ivan at (50,490), 640×960. This is geometric evidence, not a Unity capture or visual acceptance.',cases:cases.map(x=>x.analysis)};
fs.writeFileSync(dir+'/QA/FirebirdPoseProjection.json',JSON.stringify(result,null,2)+'\n');console.log(JSON.stringify(result,null,2));
