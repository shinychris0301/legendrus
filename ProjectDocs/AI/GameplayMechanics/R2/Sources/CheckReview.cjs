const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const fs=require('fs'),path=require('path'),{pathToFileURL}=require('url');
(async()=>{
const base=path.resolve('ProjectDocs/AI/GameplayMechanics/R2');
const browser=await chromium.launch({executablePath:'/usr/bin/google-chrome',headless:true,args:['--no-sandbox']});
const page=await browser.newPage();const errors=[];page.on('pageerror',e=>errors.push(String(e)));const checks=[];
for(const width of [430,1440]){
await page.setViewportSize({width,height:1000});await page.goto(pathToFileURL(path.join(base,'Gameplay_Mechanics_R2_Review.html')).href);
for(const key of ['Mechanics_Spark_Before.png','Mechanics_Spark_After.png','Mechanics_Spark_Created.png','sequence']){
await page.selectOption('#view',key);await page.locator('#capture').evaluate(i=>i.decode());
const state=await page.evaluate(()=>({contained:document.documentElement.scrollWidth<=innerWidth,loaded:document.querySelector('#capture').naturalWidth===576,images:document.querySelectorAll('img').length}));if(!state.contained||!state.loaded||state.images!==1)throw Error(JSON.stringify(state));checks.push({width,key,...state});}
for(let i=0;i<16;i++){await page.locator('#scrub').evaluate((e,i)=>{e.value=i;e.dispatchEvent(new Event('input'))},i);await page.locator('#capture').evaluate(i=>i.decode());checks.push({width,frame:i,loaded:true});}
await page.click('#play');await page.waitForTimeout(400);if(await page.locator('#scrub').inputValue()==='0')throw Error('Playback stuck');await page.click('#play');
if(width===430)await page.screenshot({path:path.join(base,'QA/ReviewMobile.png'),fullPage:true});
}
const hrefs=await page.locator('a').evaluateAll(xs=>xs.map(x=>x.getAttribute('href')));for(const href of hrefs)if(!fs.existsSync(path.resolve(base,href)))throw Error('Missing '+href);
if(errors.length)throw Error(errors.join('\n'));
fs.writeFileSync(path.join(base,'QA/BrowserReview.json'),JSON.stringify({passed:true,checks,localLinks:hrefs.length,jsErrors:errors},null,2));await browser.close();console.log('Passed: 40 capture/viewport checks, playback at 2 widths, 4 local links, zero JS errors');
})().catch(e=>{console.error(e);process.exit(1)});
