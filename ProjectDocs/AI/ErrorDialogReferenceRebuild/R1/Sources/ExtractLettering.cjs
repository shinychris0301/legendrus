const sharp=require('/tmp/lor-parchment-tools/node_modules/sharp'),fs=require('fs'),path=require('path'),assert=require('assert');
(async()=>{const b=path.resolve(__dirname,'..'),old=path.resolve('Assets/LegendsOfRus/Resources/ProductionUI/ErrorDialogAligned'),out=b+'/ProposedAssets';fs.mkdirSync(out,{recursive:true});const records=[];
for(const [name,x,y,w,h,rows] of [['panel',124,407,696,859,174],['retry',225,922,493,106,106],['close',259,1042,423,91,91]]){
 const source=await sharp(b+'/Sources/Reference.png').extract({left:x,top:y,width:w,height:h}).ensureAlpha().raw().toBuffer(),prior=await sharp(old+'/'+name+'.png').ensureAlpha().raw().toBuffer(),result=Buffer.from(prior);assert.equal(source.length,result.length);
 for(let i=0;i<w*rows;i++)for(let c=0;c<3;c++)result[i*4+c]=source[i*4+c];
 for(let i=0;i<w*h;i++){assert.equal(prior[i*4+3],result[i*4+3]);if(i>=w*rows)assert(prior.subarray(i*4,i*4+4).equals(result.subarray(i*4,i*4+4)));}
 await sharp(result,{raw:{width:w,height:h,channels:4}}).png().toFile(out+'/'+name+'_ru_r2.png');records.push({asset:name+'_ru_r2.png',sourceBounds:[x,y,x+w,y+h],restoredRows:rows,alphaUnchanged:true,remainingRowsUnchanged:true});
}
for(const [n,c] of [['Black','#000000'],['White','#ffffff'],['Magenta','#ff00ff']])await sharp({create:{width:1220,height:859,channels:4,background:c}}).composite([{input:out+'/panel_ru_r2.png',left:0,top:0},{input:out+'/retry_ru_r2.png',left:710,top:140},{input:out+'/close_ru_r2.png',left:745,top:330}]).png().toFile(b+'/QA/Alpha'+n+'.png');
fs.writeFileSync(b+'/Sources/AssetProvenance.json',JSON.stringify({method:'Original source lettering RGB, existing accepted Scenario silhouette. No new generation.',scenarioJobs:0,assets:records},null,2));console.log('Three lettering variants staged; parchment below row174 and all alpha preserved.');
})().catch(e=>{console.error(e);process.exit(1)});
