// Checks of an offline financial planning artifact; no Unity or user browser access.
import {spawn} from 'node:child_process';
import {mkdtemp,readFile,writeFile,mkdir,rm} from 'node:fs/promises';
import {tmpdir} from 'node:os';
import {join} from 'node:path';
import {fileURLToPath,pathToFileURL} from 'node:url';
import assert from 'node:assert/strict';

const base=fileURLToPath(new URL('./',import.meta.url)),out=join(base,'QA');
await mkdir(out,{recursive:true});
const profile=await mkdtemp(join(tmpdir(),'lor-revenue-review-'));
const chrome=spawn('/usr/bin/google-chrome',['--headless','--no-sandbox','--disable-gpu',
 `--user-data-dir=${profile}`,'--remote-debugging-port=0','about:blank'],{stdio:'ignore'});
const pending=new Map(),errors=[],checks=[];let sequence=0,ws;
const pause=ms=>new Promise(resolve=>setTimeout(resolve,ms));
try{
 let port;
 for(let i=0;i<100;i++){try{port=(await readFile(join(profile,'DevToolsActivePort'),'utf8')).split('\n')[0];break;}catch{}await pause(100);}
 assert.ok(port,'Isolated Chrome did not start');
 const page=await(await fetch(`http://127.0.0.1:${port}/json/new?about:blank`,{method:'PUT'})).json();
 ws=new WebSocket(page.webSocketDebuggerUrl);
 await new Promise((resolve,reject)=>{ws.addEventListener('open',resolve,{once:true});ws.addEventListener('error',reject,{once:true});});
 ws.addEventListener('message',event=>{const m=JSON.parse(event.data);if(m.method==='Runtime.exceptionThrown')errors.push(m.params.exceptionDetails);if(m.id){const c=pending.get(m.id);pending.delete(m.id);m.error?c.reject(new Error(JSON.stringify(m.error))):c.resolve(m.result);}});
 const send=(method,params={})=>new Promise((resolve,reject)=>{const id=++sequence;pending.set(id,{resolve,reject});ws.send(JSON.stringify({id,method,params}));});
 async function evaluate(expression){const r=await send('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});assert.ok(!r.exceptionDetails,JSON.stringify(r.exceptionDetails));return r.result.value;}
 async function check(name,expression,expected){const observed=await evaluate(expression);assert.equal(observed,expected,name);checks.push({name,observed,expected});}
 async function near(name,expression,expected,tolerance=.005){const observed=await evaluate(expression);assert.ok(Math.abs(observed-expected)<tolerance,`${name}: ${observed} vs ${expected}`);checks.push({name,observed,expected,tolerance});}
 async function select(value){await evaluate(`document.querySelector('#scenario').value=${JSON.stringify(value)};document.querySelector('#scenario').dispatchEvent(new Event('change'))`);}
 async function input(id,value){await evaluate(`document.getElementById(${JSON.stringify(id)}).value=${JSON.stringify(String(value))};document.getElementById(${JSON.stringify(id)}).dispatchEvent(new Event('input',{bubbles:true}))`);}
 await send('Page.enable');await send('Runtime.enable');
 await send('Page.navigate',{url:pathToFileURL(join(base,'ProfitabilityReview.html')).href});
 for(let i=0;i<100;i++){if(await evaluate('Boolean(window.currentResults)'))break;await pause(100);}
 await near('Working example net receipts: 200 payers ×500 less refunds/fee','currentResults.netIap',83300);
 await near('Working example includes UA, variable and allocated fixed costs','currentResults.preTaxCohortSurplus',-151700);
 await near('Break-even paid CPI includes fixed/variable allocation','currentResults.breakEvenPaidCpi',4.83);
 await near('Required payer conversion uses same 90-day cost horizon','currentResults.requiredPayerFraction',23.5/416.5,.000001);
 await select('low');await near('Low conversion independent expected surplus','currentResults.preTaxCohortSurplus',-210093.3);
 await select('high');await near('High conversion independent expected surplus','currentResults.preTaxCohortSurplus',181500);
 await select('working');await input('paidFraction',0);
 await near('Organic cohort has no paid acquisition cost','currentResults.marketingCost',0);
 await near('Organic installs retain operating costs','currentResults.preTaxCohortSurplus',48300);
 await check('No paid installs has no CPI boundary','currentResults.breakEvenPaidCpi',null);
 await input('allocatedDevelopmentCost',48301);await near('Owner/development allocation changes payback','currentResults.preTaxCohortSurplus',-1);
 await select('working');await input('netAdsPerInstall',1);await near('Net ad proceeds do not receive another store fee','currentResults.preTaxCohortSurplus',-141700);
 await select('working');await input('refundFraction',100);await near('Fully refunded IAP has no net receipts','currentResults.netIap',0);
 await check('Zero realized proceeds cannot meet positive costs','currentResults.requiredPayerFraction',null);
 await select('working');await input('paidCpi',1000);await check('Conversion above100% is labeled unreachable',"document.querySelector('#required').textContent.startsWith('Unreachable')",true);
 await select('working');await input('installs',0);await check('Zero installs clears stale result','currentResults',null);
 await select('working');await input('paidCpi',-1);await check('Negative costs rejected','currentResults',null);
 await select('working');await input('payerFraction',101);await check('Invalid percentage rejected','currentResults',null);
 await select('working');await input('grossSpendPerPayer','');await check('Blank input rejected','currentResults',null);
 await select('working');await input('installs',1.5);await check('Fractional installs rejected','currentResults',null);
 const scenarios=JSON.parse(await readFile(join(base,'ProfitabilityScenarios.json'),'utf8'));
 for(const c of scenarios.cases){for(const [key,expected] of Object.entries(c.outputs)){if(key==='scope')continue;const expression=`calculate(${JSON.stringify(c.inputs)})[${JSON.stringify(key)}]`;expected===null?await check(`${c.name}: ${key} parity`,expression,null):await near(`${c.name}: ${key} parity`,expression,expected,key==='requiredPayerFraction'?.0000001:.0051);}}
 await select('working');
 for(const [name,width,height] of [['Desktop',1280,1000],['Portrait',390,844]]){
  await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await pause(150);
  await check(`${name} has no horizontal overflow`,'document.documentElement.scrollWidth<=innerWidth',true);
  const capture=await send('Page.captureScreenshot',{format:'png',captureBeyondViewport:true});await writeFile(join(out,`${name}.png`),Buffer.from(capture.data,'base64'));
 }
 assert.deepEqual(errors,[],'JavaScript exceptions');
 await writeFile(join(out,'BrowserChecks.json'),JSON.stringify({passed:checks.length,failed:0,checks,errors,scope:'Offline revenue planning tool; isolated Chrome profile, not Unity or game screenshots'},null,2)+'\n');
 console.log(JSON.stringify({passed:checks.length,failed:0,errors:errors.length}));
}finally{if(ws)ws.close();chrome.kill('SIGTERM');await pause(300);await rm(profile,{recursive:true,force:true,maxRetries:3,retryDelay:300});}
