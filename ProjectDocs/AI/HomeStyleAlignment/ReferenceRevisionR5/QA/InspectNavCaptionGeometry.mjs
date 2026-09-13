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

const rows=[];
for(const name of ['map','shop','events','characters']){
 const file='ProjectDocs/AI/HomeStyleAlignment/ReferenceRevisionR5/Sources/nav_'+name+'_generated.png';const {w,h,bpp,pixels}=png(file);const runs=[];
 const navy=(x,y)=>{const i=(y*w+x)*bpp;return pixels[i]<65&&pixels[i+1]<80&&pixels[i+2]<100&&pixels[i+2]>pixels[i]&&pixels[i+3]>220;};
 for(let y=Math.floor(h*.65);y<Math.floor(h*.95);y++){
  const cx=Math.floor(w/2);if(!navy(cx,y)){runs.push({y,left:cx,right:cx,width:0});continue;}let l=cx,r=cx;while(l>0&&navy(l-1,y))l--;while(r<w-1&&navy(r+1,y))r++;runs.push({y,left:l,right:r,width:r-l+1});
 }
 const bands=[];let band=[];for(const r of runs){if((()=>{let n=0,total=0;for(let x=Math.ceil(w*.2);x<Math.floor(w*.8);x++){total++;if(navy(x,r.y))n++;}return n/total>.95;})())band.push(r);else if(band.length){bands.push(band);band=[];}}if(band.length)bands.push(band);bands.sort((a,b)=>b.length-a.length);
 const selected=bands[0];const safeRows=selected.slice(8,-8);const centerY=Math.round((safeRows[0].y+safeRows.at(-1).y)/2),glyphRows=[];for(let y=safeRows[0].y;y<=safeRows.at(-1).y;y++){let l=Math.floor(w/2),r=l,gap=0;for(let x=l;x>=0;x--){gap=navy(x,y)?0:gap+1;if(gap>=10){l=x+10;break;}}gap=0;for(let x=r;x<w;x++){gap=navy(x,y)?0:gap+1;if(gap>=10){r=x-10;break;}}glyphRows.push({left:l,right:r});}const rect={left:Math.max(...glyphRows.map(r=>r.left))+8,right:Math.min(...glyphRows.map(r=>r.right))-8,top:safeRows[0].y,bottom:safeRows.at(-1).y};
 rows.push({asset:file,sourceSize:[w,h],method:'Rows from65%-95%height with over95%dark-navy pixels within central60%width, R<65 G<80 B<100 B>R alpha>220. Largest contiguous band;8pxvertical margin. Read-only source pixels, occasional painted texture permitted. Horizontal bounds span every safe blank-band row with 8pixel horizontal margin; 10 consecutive nonnavy pixels define the edge. No fixed glyph-height assumption: use this measured rectangle for runtime glyph containment.',sourcePixelSafeCaptionRect:rect,normalizedSafeCaptionRect:Object.fromEntries(Object.entries(rect).map(([k,v])=>[k,v/(k==='left'||k==='right'?w:h)])),normalizedCaptionCenter:[(rect.left+rect.right)/2/w,(rect.top+rect.bottom)/2/h]});
}
fs.writeFileSync('ProjectDocs/AI/HomeStyleAlignment/ReferenceRevisionR5/QA/NavCaptionGeometry.json',JSON.stringify(rows,null,2)+'\n');console.log(JSON.stringify(rows,null,2));
