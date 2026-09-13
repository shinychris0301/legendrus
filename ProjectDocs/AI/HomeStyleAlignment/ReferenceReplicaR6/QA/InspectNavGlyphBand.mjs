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

const boxes=[];
for(const [name,y0,y1] of [['map',940,1025],['shop',940,1025],['events',940,1025],['characters',940,1025]]){
 const file='ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/Sources/nav_'+name+'_generated.png';const {w,h,bpp,pixels}=png(file);const valid=[];
 for(let x=0;x<w;x++){let n=0;for(let y=y0;y<=y1;y++){let i=(y*w+x)*bpp;if(pixels[i]<65&&pixels[i+1]<85&&pixels[i+2]<110&&pixels[i+2]>pixels[i]&&pixels[i+3]>220)n++;}valid[x]=n/(y1-y0+1)>.94;}
 const cx=Math.floor(w/2);let l=cx,r=cx,gap=0;for(let x=cx;x>=0;x--){gap=valid[x]?0:gap+1;if(gap>=10){l=x+10;break;}}gap=0;for(let x=cx;x<w;x++){gap=valid[x]?0:gap+1;if(gap>=10){r=x-10;break;}}boxes.push({name,w,h,rect:{left:l+6,right:r-6,top:y0,bottom:y1}});
}fs.writeFileSync('ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6/QA/NavGlyphBandGeometry.json',JSON.stringify({method:'Read-only columns containing >94% navy pixels across actual glyph-height candidate band y940..1025; edge after10 consecutive non-navy columns,6pxmargin. Full-source1254x1254 unchanged. Applies only to this narrow vertical band, not whole caption plaque.',sourcePixelEdits:false,boxes},null,2)+'\n');console.log(JSON.stringify(boxes,null,2));