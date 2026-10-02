const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');
const fs=require('fs'),path=require('path'),{pathToFileURL}=require('url');
(async()=>{
 const base=path.resolve('ProjectDocs/AI/GameplayMechanics/R10'),manifest=JSON.parse(fs.readFileSync(path.join(base,'QA/CaptureManifest.json')));
 const browser=await chromium.launch({executablePath:'/usr/bin/google-chrome',headless:true,args:['--no-sandbox']}),page=await browser.newPage(),errors=[];let captureChecks=0,assetChecks=0;
 page.on('pageerror',e=>errors.push(String(e)));
 for(const width of [430,1440]){
  await page.setViewportSize({width,height:1000});await page.goto(pathToFileURL(path.join(base,'Gameplay_Mechanics_R10_Review.html')).href);
  for(let g=0;g<manifest.length;g++){
   await page.selectOption('#group',String(g));
   for(let i=0;i<manifest[g].files.length;i++){
    if(i)await page.click('#next');await page.locator('#frame').evaluate(e=>e.decode());
    if(!await page.evaluate(()=>document.querySelector('#frame').naturalWidth===576&&document.querySelector('#frame').naturalHeight===1280&&document.querySelectorAll('img').length===1&&document.documentElement.scrollWidth<=innerWidth))throw Error('Capture layout failure');captureChecks++;
   }
  }
  await page.selectOption('#group','1');await page.click('#next');await page.click('#previous');if(!await page.locator('#previous').isDisabled())throw Error('Previous');
  await page.click('#native');if(!await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth&&Math.abs(document.querySelector('#frame').getBoundingClientRect().width-576)<1))throw Error('Native containment');await page.click('#native');
  for(const href of await page.locator('a').evaluateAll(a=>a.map(x=>x.getAttribute('href'))))if(!fs.existsSync(path.resolve(base,href)))throw Error('Missing '+href);
  await page.goto(pathToFileURL(path.join(base,'AssetReview.html')).href);
  for(const asset of ['bomb','line','spark','color','crate','ice','chain','seal']){
   await page.selectOption('#asset',asset);const states=['crate','ice','chain','seal'].includes(asset)?2:1;
   for(let state=0;state<states;state++){
    if(state)await page.click('#damage');
    await page.locator('#piece').evaluate(e=>e.decode());await page.locator('#large').evaluate(e=>e.decode());
    for(const bg of ['#000000','#ffffff','#ff00bb']){
     await page.selectOption('#backdrop',bg);if(!await page.evaluate(()=>document.querySelector('#piece').naturalWidth>0&&document.documentElement.scrollWidth<=innerWidth))throw Error('Asset layout failure');assetChecks++;
    }
   }
  }
 }
 if(errors.length)throw Error(errors.join('\n'));
 fs.writeFileSync(path.join(base,'QA/BrowserReview.json'),JSON.stringify({passed:true,captureChecks,assetChecks,viewports:[430,1440],localLinks:true,nativePixelToggle:true,stateNavigation:true,jsErrors:errors},null,2));await browser.close();console.log({captureChecks,assetChecks});
})().catch(e=>{console.error(e);process.exit(1)});
