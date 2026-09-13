import {spawn} from 'node:child_process';
import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
const root=process.cwd(), dir=path.join(root,'ProjectDocs/AI/HomeStyleAlignment/PlayLetteringR8');
const qa=path.join(dir,'QA'); await fs.mkdir(qa,{recursive:true});
const profile=await fs.mkdtemp('/tmp/lor-home-chrome-');
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


 await load(pathToFileURL(path.join(dir,'Home_PlayLettering_R8_Review.html')).href,600,900);
 const checks=[];
 for(const key of ['ru','before','reference','phone','tall','en']){
  const row=await evaluate(`(async()=>{const select=document.querySelector('#capture');select.value=${JSON.stringify(key)};select.dispatchEvent(new Event('change'));await window.viewReady;const img=document.querySelector('#portrait');return {key:${JSON.stringify(key)},loaded:img.complete&&img.naturalWidth>0,dimensions:[img.naturalWidth,img.naturalHeight],horizontalOverflow:document.documentElement.scrollWidth>innerWidth,detailDisabled:document.querySelector('#detail').disabled};})()`,true);checks.push(row);
 }
 const details=[];
 for(const key of ['ru','before','reference'])for(const detail of ['play']){
  const row=await evaluate(`(async()=>{const select=document.querySelector('#capture');select.value=${JSON.stringify(key)};select.dispatchEvent(new Event('change'));await window.viewReady;const mode=document.querySelector('#detail');mode.value=${JSON.stringify(detail)};mode.dispatchEvent(new Event('change'));const img=document.querySelector('#portrait');const rect=document.querySelector('#viewport').getBoundingClientRect();return {key:${JSON.stringify(key)},detail:${JSON.stringify(detail)},zoom:document.querySelector('#figure').classList.contains('zoom'),square:Math.abs(rect.width / rect.height - 3.8)<.01,imageWidth:parseFloat(img.style.width),horizontalOverflow:document.documentElement.scrollWidth>innerWidth};})()`,true);details.push(row);
  if(key==='ru'||key==='reference')await shot(path.join(qa,`ViewerZoom_${key}_${detail}.png`));
 }
 const success=checks.every(c=>c.loaded&&!c.horizontalOverflow)&&details.every(d=>d.zoom&&d.square&&Math.abs(d.imageWidth - 941 * Math.min(2, 600 / 360)) < .1&&!d.horizontalOverflow);
 await fs.writeFile(path.join(qa,'StandaloneReviewBrowserChecks.json'),JSON.stringify({tool:'Local Chrome CDP',viewerOnly:true,notFreshUnityRendering:true,success,checks,details},null,2)+'\n');
 console.log(JSON.stringify({success,views:checks.length,zoomChecks:details.length}));if(!success)throw Error('Review viewer failed');
}finally{if(ws)ws.close();chrome.kill('SIGTERM');}
