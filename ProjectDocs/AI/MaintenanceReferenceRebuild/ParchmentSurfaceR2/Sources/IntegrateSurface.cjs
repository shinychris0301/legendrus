const sharp=require('/tmp/lor-parchment-tools/node_modules/sharp'),fs=require('fs'),path=require('path'),assert=require('assert');
const b=path.resolve(__dirname,'..'),assets=path.resolve('Assets/LegendsOfRus/Resources/ProductionUI/MaintenanceAligned');
(async()=>{
 const w=820,h=1212;
 const gen=await sharp(b+'/Sources/GeneratedBlankSurface.png').resize(w,h).ensureAlpha().raw().toBuffer();
 const ref=await sharp(b+'/Sources/Reference.png').extract({left:60,top:311,width:w,height:h}).ensureAlpha().raw().toBuffer();
 const samples=[[120,489,320,499],[490,489,670,499],[110,737,430,750],[105,939,370,974]];
 const delta=[0,0,0];let count=0;for(const [x0,y0,x1,y1] of samples)for(let y=y0;y<y1;y++)for(let x=x0;x<x1;x++){const i=4*(y*w+x);for(let c=0;c<3;c++)delta[c]+=ref[i+c]-gen[i+c];count++;}for(let c=0;c<3;c++)delta[c]/=count;
 const mask=Buffer.alloc(w*h),smooth=x=>{x=Math.max(0,Math.min(1,x));return x*x*(3-2*x)};
 for(let y=0;y<h;y++)for(let x=0;x<w;x++){
  let a=smooth(Math.min((x-52)/32,(768-x)/32,(y-326)/40,(1141-y)/32));
  // Retain only the original separator ink and a narrow feather around its contour.
  if(y>=491&&y<=536&&x>=137&&x<=674){
   let d=20;for(let yy=Math.max(491,y-4);yy<=Math.min(536,y+4);yy++)for(let xx=Math.max(137,x-4);xx<=Math.min(674,x+4);xx++){const i=4*(yy*w+xx);if(ref[i]<155&&ref[i+1]<115&&ref[i+2]<78)d=Math.min(d,Math.hypot(x-xx,y-yy));}
   a*=smooth(d/4);
  }
  mask[y*w+x]=Math.round(a*255);
 }
 const reports=[];
 for(const [input,output] of [['panel_ru_r2','panel_ru_surface_r3'],['panel','panel_en_surface_r3']]){
  const old=await sharp(assets+'/'+input+'.png').ensureAlpha().raw().toBuffer();assert.equal(old.length,w*h*4);const out=Buffer.from(old);let changed=0;
  for(let i=0;i<w*h;i++){let a=mask[i]/255;if(!a)continue;for(let c=0;c<3;c++)out[4*i+c]=Math.round(old[4*i+c]*(1-a)+Math.max(0,Math.min(255,gen[4*i+c]+delta[c]))*a);changed++;}
  for(let i=0;i<w*h;i++){assert.equal(out[4*i+3],old[4*i+3]);if(mask[i]===0)assert(out.subarray(i*4,i*4+4).equals(old.subarray(i*4,i*4+4)));}
  await sharp(out,{raw:{width:w,height:h,channels:4}}).png().toFile(assets+'/'+output+'.png');
  reports.push({input,output,interiorPixels:changed,alphaUnchanged:true,outsideRepairUnchanged:true});
 }
 await sharp(mask,{raw:{width:w,height:h,channels:1}}).png().toFile(b+'/Sources/InteriorMask.png');
 for(const [name,color] of [['Black','#000000'],['White','#ffffff'],['Magenta','#ff00ff']])await sharp(assets+'/panel_ru_surface_r3.png').flatten({background:color}).png().toFile(b+'/QA/Alpha'+name+'.png');
 fs.writeFileSync(b+'/QA/SurfaceIntegrity.json',JSON.stringify({tool:'Built-in imagegen; bounded integration with original art via sharp',generated:'Sources/GeneratedBlankSurface.png',colorDelta:delta,colorSampleRects:samples,variants:reports},null,2));console.log(JSON.stringify({colorDelta:delta,variants:reports}));
})().catch(e=>{console.error(e);process.exit(1)});
