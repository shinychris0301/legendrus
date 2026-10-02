const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const fs=require('fs'),path=require('path'),{pathToFileURL}=require('url');
(async()=>{
const base=path.resolve('ProjectDocs/AI/CampaignFoundation/R6');
const browser=await chromium.launch({executablePath:'/usr/bin/google-chrome',headless:true,args:['--no-sandbox']});
const page=await browser.newPage();const errors=[];page.on('pageerror',e=>errors.push(String(e)));
const checks=[];
for(const width of [430,1440]){
await page.setViewportSize({width,height:1000});await page.goto(pathToFileURL(path.join(base,'Campaign_Foundation_R6_Review.html')).href);
for(const key of ['Gameplay','PreLevel','Victory','Recovery_RU','Fail_RU','SaveError']){
await page.selectOption('#capture',key);await page.locator('#screen').evaluate(img=>img.decode());
const state=await page.evaluate(()=>({contained:document.documentElement.scrollWidth<=innerWidth,loaded:document.querySelector('#screen').naturalWidth>0,count:document.querySelectorAll('figure img').length}));
if(!state.contained||!state.loaded||state.count!==1)throw Error(JSON.stringify(state));checks.push({width,key,...state});}
}
const hrefs=await page.locator('nav a').evaluateAll(xs=>xs.map(x=>x.getAttribute('href')));for(const href of hrefs)if(!fs.existsSync(path.resolve(base,href)))throw Error('Missing '+href);
if(errors.length)throw Error(errors.join('\n'));
fs.writeFileSync(path.join(base,'QA/BrowserReview.json'),JSON.stringify({passed:true,checks,localLinks:hrefs.length,jsErrors:errors},null,2));await browser.close();console.log('Passed: 12 capture/viewport checks, 4 local links, zero JS errors');
})().catch(e=>{console.error(e);process.exit(1)});
