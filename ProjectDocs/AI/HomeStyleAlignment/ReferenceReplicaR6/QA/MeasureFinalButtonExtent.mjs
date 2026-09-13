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

for(const file of ['Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png','ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/UnityVerification/Home/Home_941x1672_RU.png']){
 const {w,h,bpp,pixels}=png(file);let rows=[];
 for(let y=990;y<1240;y++){let n=0,left=w,right=-1;for(let x=175;x<780;x++){let i=(y*w+x)*bpp,r=pixels[i],g=pixels[i+1],b=pixels[i+2];if(g>r*1.25&&g>b*1.1&&g>18){n++;left=Math.min(left,x);right=Math.max(right,x);}}if(n>=100)rows.push({y,n,left,right});}
 console.log(file,JSON.stringify({first:rows[0],last:rows.at(-1),height:rows.at(-1).y-rows[0].y+1,peak:rows.reduce((a,b)=>a.n>b.n?a:b)}));
}