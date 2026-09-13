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
const [beforePath,afterPath,outPath]=process.argv.slice(2);const a=png(beforePath),b=png(afterPath);if(a.w!==941||a.h!==1672||a.w!==b.w||a.h!==b.h)throw Error('Reference-size941x1672 captures required');
const rectangles=[[311,1093,321,66]];
let total=0,outside=0,outsideAboveOne=0,max=0,maxOutside=0;
for(let i=0;i<a.w*a.h;i++){let difference=0;for(let c=0;c<4;c++){const x=c<a.bpp?a.pixels[i*a.bpp+c]:255,y=c<b.bpp?b.pixels[i*b.bpp+c]:255;difference=Math.max(difference,Math.abs(x-y));}if(!difference)continue;total++;max=Math.max(max,difference);const x=i%a.w,y=Math.floor(i/a.w);if(!rectangles.some(([rx,ry,rw,rh])=>x>=rx&&x<rx+rw&&y>=ry&&y<ry+rh)){outside++;maxOutside=Math.max(maxOutside,difference);if(difference>1)outsideAboveOne++;}}
const result={kind:'Read-only decoded pixel comparison; no PNG edits',beforePath,afterPath,width:a.w,height:a.h,expectedPlayInkAndShadowRects:rectangles,changedPixels:total,maxChannelDifference:max,outsidePlayInkChangedPixels:outside,outsidePlayInkMaxChannelDifference:maxOutside,outsidePlayInkPixelsAboveOne:outsideAboveOne,allChangesAboveOneConfinedToPlayInk:outsideAboveOne===0};fs.writeFileSync(outPath,JSON.stringify(result,null,2)+'\n');console.log(JSON.stringify(result));if(outsideAboveOne)process.exitCode=1;
