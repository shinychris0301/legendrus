const sharp=require('/tmp/lor-parchment-tools/node_modules/sharp'),fs=require('fs'),path=require('path'),assert=require('assert');
(async()=>{const b=path.resolve(__dirname,'..'),old=path.resolve('Assets/LegendsOfRus/Resources/ProductionUI/DialogueAligned'),out=b+'/ProposedAssets';fs.mkdirSync(out,{recursive:true});const records=[];
for(const [name,x,y,w,h,rect] of [['panel',45,1022,849,618,[240,62,380,75]],['continue',195,1454,552,138,[0,0,552,138]]]){
 const source=await sharp(b+'/Sources/Reference.png').extract({left:x,top:y,width:w,height:h}).ensureAlpha().raw().toBuffer(),prior=await sharp(old+'/'+name+'.png').ensureAlpha().raw().toBuffer(),result=Buffer.from(prior);assert.equal(source.length,result.length);
 const [rx,ry,rw,rh]=rect;for(let yy=ry;yy<ry+rh;yy++)for(let xx=rx;xx<rx+rw;xx++)for(let c=0;c<3;c++)result[(yy*w+xx)*4+c]=source[(yy*w+xx)*4+c];
 await sharp(result,{raw:{width:w,height:h,channels:4}}).png().toFile(out+'/'+name+'_ru_r2.png');records.push({asset:name+'_ru_r2.png',sourceBounds:[x,y,w,h],restoredRectangle:rect,alphaUnchanged:true});
}
for(const [n,c] of [['Black','#000000'],['White','#ffffff'],['Magenta','#ff00ff']])await sharp({create:{width:849,height:770,channels:4,background:c}}).composite([{input:out+'/panel_ru_r2.png',left:0,top:0},{input:out+'/continue_ru_r2.png',left:145,top:625}]).png().toFile(b+'/QA/Alpha'+n+'.png');
fs.writeFileSync(b+'/Sources/AssetProvenance.json',JSON.stringify({method:'Original source lettering RGB within bounded original face; existing accepted extraction alpha. No generation.',scenarioJobs:0,assets:records},null,2));
})().catch(e=>{console.error(e);process.exit(1)});
