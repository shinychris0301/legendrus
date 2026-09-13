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


const base='ProjectDocs/AI/HomeStyleAlignment/SplashReuseR4/',rows=[];
for(const name of ['avatar_ring.png','home_firebird_generated.png']){const {w,h,bpp,pixels}=png(base+'Sources/'+name),alpha=(x,y)=>pixels[(y*w+x)*bpp+3],bounds={};for(const t of [1,8,32,64,128,245]){let a=[w,h,-1,-1],count=0;for(let y=0;y<h;y++)for(let x=0;x<w;x++)if(alpha(x,y)>=t){count++;a=[Math.min(a[0],x),Math.min(a[1],y),Math.max(a[2],x),Math.max(a[3],y)];}bounds[t]={bounds:a,pixels:count};}const r={asset:name,width:w,height:h,alphaThresholdBounds:bounds};
if(name==='avatar_ring.png'){const sx=Math.floor(w/2),sy=Math.floor(h*.43),vis=new Uint8Array(w*h),q=[sy*w+sx];vis[q[0]]=1;let out=[w,h,-1,-1];for(let k=0;k<q.length;k++){const i=q[k],x=i%w,y=Math.floor(i/w);out=[Math.min(out[0],x),Math.min(out[1],y),Math.max(out[2],x),Math.max(out[3],y)];for(const j of [i-1,i+1,i-w,i+w]){if(j<0||j>=w*h||vis[j]||pixels[j*bpp+3]>20)continue;const nx=j%w,ny=Math.floor(j/w);if(Math.abs(nx-x)+Math.abs(ny-y)!==1)continue;vis[j]=1;q.push(j);}}r.transparentOpening={method:'4-connected flood fill alpha<=20 from image center near portrait eye line; source pixels read only',seed:[sx,sy],bounds:out,pixelCount:q.length};const runs=[];for(const f of [.2,.25,.3,.35,.4,.43,.45,.5,.55,.6,.65,.7]){const y=Math.round(h*f);let l=sx,rt=sx;while(l>0&&alpha(l-1,y)<=20)l--;while(rt<w-1&&alpha(rt+1,y)<=20)rt++;runs.push({y,left:l,right:rt,width:rt-l+1});}r.openingHorizontalRuns=runs;}
rows.push(r);}
fs.writeFileSync(base+'QA/AvatarFirebirdGeometry.json',JSON.stringify(rows,null,2)+'\n');console.log(JSON.stringify(rows,null,2));
