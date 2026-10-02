const {chromium}=require('/tmp/lor-map-preview/node_modules/playwright');const fs=require('fs'),path=require('path');
(async()=>{
const dir=path.resolve('ProjectDocs/AI/WorldMapReferenceRebuild/NavigationAndRegionsR2'),errors=[],checks=[];
const b=await chromium.launch({headless:true,executablePath:'/usr/bin/google-chrome',args:['--no-sandbox']});
const p=await b.newPage({viewport:{width:1000,height:2200}});p.on('pageerror',e=>errors.push(String(e)));
await p.goto('file://'+dir+'/WorldMap_ReferenceCards_R2_Review.html');await p.evaluate(async()=>{await document.fonts.ready;await Promise.all([...document.images].map(i=>i.decode()))});
await p.addStyleTag({content:'main{max-width:none;padding:0}#device{width:941px;border:0}'});
const assert=(x,s)=>{if(!x)throw Error(s);checks.push(s)};
const shot=async n=>p.locator('#device').screenshot({path:dir+'/QA/'+n+'.png'});
const visible=()=>p.locator('#map').evaluate(m=>{let r=m.getBoundingClientRect();return [...m.children].filter(e=>{let b=e.getBoundingClientRect();return b.top>=r.top-1&&b.bottom<=r.bottom+1}).map(e=>+e.dataset.chapter)});
assert(await p.locator('.region').count()===20,'20 confirmed chapter cards');
assert(JSON.stringify(await visible())==='[4,3,2,1]','Four complete cards; chapter 1 at bottom');await shot('Design_DownwardDrag_Start');
const before=await p.locator('#map').evaluate(e=>e.scrollTop),r=await p.locator('#map').boundingBox();
await p.mouse.move(r.x+100,r.y+260);await p.mouse.down();await p.mouse.move(r.x+100,r.y+590,{steps:15});await p.mouse.up();await p.waitForTimeout(100);
assert(await p.locator('#map').evaluate(e=>e.scrollTop)<before-250,'Mouse drag down reveals later chapters above');
assert(await p.locator('.toast.visible').count()===0,'Dragging does not accidentally open a chapter');await shot('Design_After_DownwardDrag');
await p.locator('#map').evaluate(e=>e.scrollTop=0);assert((await visible()).includes(20),'Chapter 20 visible at upper scroll end');await shot('Design_FinalRegions');
await p.locator('[data-chapter="20"]').click({force:true});assert(await p.locator('#message').innerText().then(s=>s.includes('предыдущую')),'Locked chapter explains gating');await p.locator('#close').click();
await p.selectOption('#order','reference');await shot('Design_ReferenceReadingOrder');
await p.locator('[data-chapter="1"]').click();assert(await p.locator('#message').innerText().then(s=>s.includes('Демонстрация')),'Completed chapter action');await p.locator('#close').click();
for(const n of ['home','map','tasks','guild','shop']){await p.locator(`[data-nav="${n}"]`).click();if(n!=='map'){assert(await p.locator('.toast.visible').count()===1,n+' navigation preview');await p.locator('#close').click()}}
await p.locator('.back').click();assert(await p.locator('#message').innerText().then(s=>s.includes('Возврат')),'Bottom Back action');await p.locator('#close').click();
const geom=await p.evaluate(()=>{const rect=s=>{const r=document.querySelector(s).getBoundingClientRect();return {top:r.top,bottom:r.bottom,left:r.left,right:r.right}};return {device:rect('#device'),title:rect('.title'),back:rect('.back'),footer:rect('.footer')}});
assert(geom.title.top>=geom.device.top&&geom.title.bottom<geom.device.bottom,'Title fully inside portrait');assert(geom.back.top>geom.device.top+1300,'Back in lower action row');
for(const v of ['1','8','20','21']){await p.selectOption('#progress',v);assert(await p.locator('.locked').count()===Math.max(0,20-Number(v)),'Progress fixture '+v);await shot('Design_Progress_'+v)}
await p.selectOption('#progress','3');await p.selectOption('#density','3');await shot('Design_ThreeCards');await p.selectOption('#density','4');
const fit=await p.locator('.copy').evaluateAll(es=>es.map(e=>({chapter:e.closest('.region').dataset.chapter,inside:[...e.children].filter(c=>getComputedStyle(c).display!=="none").every(c=>{let a=c.getBoundingClientRect(),b=e.closest('.region').getBoundingClientRect();return a.top>=b.top+5&&a.bottom<=b.bottom-5&&a.left>=b.left&&a.right<=b.right})})));
assert(fit.every(e=>e.inside),'All chapter captions and progress fit within card frames');
await p.setViewportSize({width:390,height:1100});await p.addStyleTag({content:'main{padding:12px}#device{width:100%}'});await p.selectOption('#order','ascending');await p.waitForTimeout(100);await shot('Design_Phone390');assert(await p.evaluate(()=>document.documentElement.scrollWidth<=innerWidth),'No phone horizontal overflow');
// Real touch events, not a synthetic scrollTop assignment.
const cdp=await p.context().newCDPSession(p);await cdp.send('Emulation.setTouchEmulationEnabled',{enabled:true,maxTouchPoints:1});const touch=await p.locator('#map').boundingBox(),initial=await p.locator('#map').evaluate(e=>e.scrollTop);
await cdp.send('Input.dispatchTouchEvent',{type:'touchStart',touchPoints:[{x:touch.x+80,y:touch.y+55}]});
for(let i=1;i<=12;i++){await cdp.send('Input.dispatchTouchEvent',{type:'touchMove',touchPoints:[{x:touch.x+80,y:touch.y+55+i*10}]});await p.waitForTimeout(15)}
await cdp.send('Input.dispatchTouchEvent',{type:'touchEnd',touchPoints:[]});await p.waitForTimeout(250);assert(await p.locator('#map').evaluate(e=>e.scrollTop)<initial-100,'Touch drag downward reveals later chapters');
assert(errors.length===0,'No browser JavaScript errors');
fs.writeFileSync(dir+'/QA/BrowserChecks.json',JSON.stringify({result:'Passed',checks,fit,geometry:geom,errors,rendering:'Offline interactive design preview; not a Unity capture'},null,2));await b.close();console.log(checks.length+' browser checks passed');
})().catch(e=>{console.error(e);process.exit(1)});
