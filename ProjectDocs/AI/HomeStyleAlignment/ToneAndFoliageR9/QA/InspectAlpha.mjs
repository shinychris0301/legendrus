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
const file=process.argv[2],p=png(file);let zero=0,opaque=0,partial=0,lo=[p.w,p.h],hi=[-1,-1];for(let i=0;i<p.w*p.h;i++){const a=p.pixels[i*p.bpp+3];if(a===0)zero++;else{if(a===255)opaque++;else partial++;lo=[Math.min(lo[0],i%p.w),Math.min(lo[1],Math.floor(i/p.w))];hi=[Math.max(hi[0],i%p.w),Math.max(hi[1],Math.floor(i/p.w))];}}const samples=[[0,0],[512,0],[100,400],[200,400],[300,100],[500,100],[900,700],[512,1535]].map(([x,y])=>({x,y,rgba:[...p.pixels.subarray((y*p.w+x)*4,(y*p.w+x)*4+4)]}));const result={file,width:p.w,height:p.h,bpp:p.bpp,zero,opaque,partial,nonzeroBounds:[lo,hi],samples};console.log(JSON.stringify(result,null,2));if(process.argv[3])fs.writeFileSync(process.argv[3],JSON.stringify(result,null,2)+'\n');