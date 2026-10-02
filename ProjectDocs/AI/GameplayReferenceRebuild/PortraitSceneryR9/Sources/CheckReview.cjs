const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const path=require('path'),fs=require('fs');
(async()=>{const browser=await chromium.launch({headless:true,executablePath:'/usr/bin/google-chrome',args:['--no-sandbox']});
const page=await browser.newPage({viewport:{width:390,height:844}});const errors=[];page.on('pageerror',e=>errors.push(String(e)));
const base=path.resolve(__dirname,'..');await page.goto('file://'+base+'/Gameplay_PortraitScenery_R9_Review.html');
const values=await page.locator('#view option').evaluateAll(o=>o.map(x=>x.value));
for(const value of values){await page.selectOption('#view',value);await page.locator('#capture').evaluate(e=>e.decode());}
const overflow=await page.evaluate(()=>document.documentElement.scrollWidth>innerWidth);
const result={views:values.length,errors,horizontalOverflow:overflow};fs.writeFileSync(base+'/QA/BrowserChecks.json',JSON.stringify(result,null,2));console.log(result);await browser.close();if(errors.length||overflow)process.exit(1);})();
