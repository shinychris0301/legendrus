const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const fs=require('fs'),path=require('path');
(async()=>{
 const base=path.resolve('ProjectDocs/AI/DevelopmentStageReview/R1');
 const browser=await chromium.launch({executablePath:'/usr/bin/google-chrome',headless:true,args:['--no-sandbox']});
 const page=await browser.newPage();const errors=[];page.on('pageerror',e=>errors.push(e.message));const checks=[];
 for(const width of [430,1440]){
  await page.setViewportSize({width,height:1000});await page.goto('file://'+base+'/Development_Stage_Review_R1.html');
  for(const panel of ['scenario','stage']){
   await page.click('[data-panel="'+panel+'"]');
   const visible=await page.locator('#'+panel).isVisible();
   const overflow=await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth);
   checks.push({width,panel,visible,overflow});if(!visible||overflow)throw new Error('Layout/tab failure');
  }
 }
 const links=await page.locator('a').evaluateAll(a=>a.map(x=>x.getAttribute('href')));
 const missing=links.filter(x=>!x.startsWith('http')&&!x.startsWith('#')&&!fs.existsSync(path.resolve(base,x)));
 if(errors.length||missing.length)throw new Error(JSON.stringify({errors,missing}));
 fs.writeFileSync(base+'/QA/BrowserChecks.json',JSON.stringify({checks,errors,missing,localLinksChecked:links.length},null,2));
 await browser.close();console.log('PASS: report tabs, desktop/mobile containment, '+links.length+' local links');
})().catch(e=>{console.error(e);process.exit(1)});
