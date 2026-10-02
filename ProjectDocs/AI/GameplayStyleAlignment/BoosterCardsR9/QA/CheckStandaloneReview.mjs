import {spawn} from 'node:child_process';
import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
const root=process.cwd(), dir=path.join(root,'ProjectDocs/AI/GameplayStyleAlignment/BoosterCardsR9');
const qa=path.join(dir,'QA'); await fs.mkdir(qa,{recursive:true});
const profile=await fs.mkdtemp('/tmp/lor-gameplay-chrome-');
const chrome=spawn('/usr/bin/google-chrome',['--headless','--no-sandbox','--disable-gpu','--disable-dev-shm-usage','--disable-background-networking','--disable-component-update','--disable-sync','--disable-extensions','--no-first-run','--no-default-browser-check','--hide-scrollbars','--allow-file-access-from-files','--force-device-scale-factor=1','--remote-debugging-port=0','--remote-debugging-address=127.0.0.1','--user-data-dir='+profile,'about:blank'],{stdio:'ignore'});
let ws;const pause=ms=>new Promise(r=>setTimeout(r,ms));
try {
 let port;for(let i=0;i<150;i++){try{port=(await fs.readFile(path.join(profile,'DevToolsActivePort'),'utf8')).split('\n')[0];break;}catch{await pause(100);}}
 if(!port)throw Error('Chrome DevTools startup timed out');
 const targets=await(await fetch('http://127.0.0.1:'+port+'/json/list')).json();
 ws=new WebSocket(targets.find(t=>t.type==='page').webSocketDebuggerUrl);await new Promise((r,j)=>{ws.onopen=r;ws.onerror=j;});
 let id=0;const pending=new Map();ws.onmessage=e=>{const m=JSON.parse(e.data);if(pending.has(m.id)){const {resolve,reject}=pending.get(m.id);pending.delete(m.id);m.error?reject(Error(JSON.stringify(m.error))):resolve(m.result);}};
 function call(method,params={}){return new Promise((resolve,reject)=>{const n=++id;pending.set(n,{resolve,reject});ws.send(JSON.stringify({id:n,method,params}));});}
 async function evaluate(expression,awaitPromise=false){const r=await call('Runtime.evaluate',{expression,awaitPromise,returnByValue:true});if(r.exceptionDetails)throw Error(JSON.stringify(r.exceptionDetails));return r.result.value;}
 await call('Page.enable');await call('Runtime.enable');
 async function load(url,width,height){await call('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});await call('Page.navigate',{url});for(let i=0;i<150;i++){if(await evaluate('document.readyState === "complete" && Boolean(window.previewReady)'))break;await pause(100);}await evaluate('window.previewReady',true);await pause(80);}
 async function shot(file){const r=await call('Page.captureScreenshot',{format:'png',fromSurface:true,captureBeyondViewport:false});await fs.writeFile(file,Buffer.from(r.data,'base64'));}


 await load(pathToFileURL(path.join(dir,'Gameplay_BoosterCards_R9_Review.html')).href,600,1000);
 const checks=[];
 for(const key of await evaluate("Array.from(document.querySelector('#capture').options).map(o=>o.value)")){
  const row=await evaluate(`(async()=>{const select=document.querySelector('#capture');select.value=${JSON.stringify(key)};select.dispatchEvent(new Event('change'));await window.viewReady;const img=document.querySelector('#portrait');return {key:${JSON.stringify(key)},loaded:img.complete&&img.naturalWidth>0,dimensions:[img.naturalWidth,img.naturalHeight],horizontalOverflow:document.documentElement.scrollWidth>innerWidth};})()`,true);checks.push(row);
 }
 await call('Emulation.setDeviceMetricsOverride',{width:360,height:800,deviceScaleFactor:1,mobile:false});
 await evaluate(`(async()=>{const select=document.querySelector('#capture');select.value='Gameplay_941x1672_RU';select.dispatchEvent(new Event('change'));await window.viewReady;})()`,true);
 await pause(100);
 const narrow=await evaluate(`({width:innerWidth,overflow:document.documentElement.scrollWidth>innerWidth,loaded:document.querySelector('#portrait').naturalWidth===941})`);
 await shot(path.join(qa,'ReviewBrowser_360px.png'));
 const links=await evaluate(`Array.from(document.querySelectorAll('a')).map(a=>a.href)`);
 const localLinks=[];
 for(const url of links){let valid=false;try{await fs.access(new URL(url));valid=true;}catch{}localLinks.push({url,exists:valid});}
 const success=checks.every(c=>c.loaded&&!c.horizontalOverflow)&&narrow.loaded&&!narrow.overflow&&localLinks.every(l=>l.exists);
 await fs.writeFile(path.join(qa,'StandaloneReviewBrowserChecks.json'),JSON.stringify({tool:'Local Chrome CDP',viewerOnly:true,notFreshUnityRendering:true,success,checks,narrow,localLinks},null,2)+'\n');
 console.log(JSON.stringify({success,views:checks.length,links:localLinks.length,narrow}));if(!success)throw Error('Review viewer failed');
}finally{if(ws)ws.close();chrome.kill('SIGTERM');}
