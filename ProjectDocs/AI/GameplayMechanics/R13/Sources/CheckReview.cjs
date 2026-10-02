const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const fs=require('fs'),path=require('path');
(async()=>{
 const base=path.resolve(__dirname,'..');const browser=await chromium.launch({executablePath:'/usr/bin/google-chrome',headless:true,args:['--no-sandbox']});const page=await browser.newPage();const errors=[];page.on('pageerror',e=>errors.push(e.message));let stills=0,animations=0;
 for(const width of [430,1440]){
  await page.setViewportSize({width,height:1100});await page.goto('file://'+base+'/Gameplay_Mechanics_R13_Review.html');
  const count=await page.locator('#sequence option').count();
  for(let i=0;i<count;i++){
   await page.selectOption('#sequence',String(i));
   const max=await page.locator('#frame').getAttribute('max');
   for(let frame=0;frame<=Number(max);frame++){
    await page.locator('#frame').evaluate((s,f)=>{s.value=f;s.dispatchEvent(new Event('input'));},String(frame));
    await page.locator('#screen').evaluate(i=>i.decode());if(!await page.locator('#screen').evaluate(i=>i.naturalWidth===576&&i.naturalHeight===1280))throw Error('Bad frame dimensions');stills++;
   }
   await page.click('#play');await page.locator('#screen').evaluate(i=>i.decode());if(!await page.locator('#screen').evaluate(i=>i.src.endsWith('.webp')))throw Error('Animation did not load');animations++;
   await page.click('#stop');await page.locator('#screen').evaluate(i=>i.decode());
  }
  if(await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth))throw Error('Horizontal overflow');
  await page.selectOption('#sequence','1');await page.locator('#frame').evaluate(s=>{s.value='4';s.dispatchEvent(new Event('input'));});await page.locator('#screen').evaluate(i=>i.decode());await page.screenshot({path:'/tmp/lor-r13-review-'+width+'.png',fullPage:true});
 }
 await browser.close();if(errors.length)throw Error(errors.join('\n'));fs.writeFileSync(base+'/QA/BrowserVerification.json',JSON.stringify({widths:[430,1440],stillChecks:stills,animationChecks:animations,pageErrors:errors,horizontalOverflow:false},null,2));console.log({stills,animations,errors});
})();
