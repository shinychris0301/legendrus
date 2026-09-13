import {spawn} from 'node:child_process';
import fs from 'node:fs/promises';
import path from 'node:path';
import {pathToFileURL} from 'node:url';
const root=process.cwd(), dir=path.join(root,'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6');
const qa=path.join(dir,'QA/RootAssets'); await fs.mkdir(qa,{recursive:true});
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

 const html=`<!doctype html><meta charset="utf-8"><title>R6 source pose diagnostic</title><style>html,body{margin:0;background:#18232e;color:#f4e0b6;font:14px Arial,sans-serif;width:941px}header{height:66px;padding:10px;box-sizing:border-box;line-height:22px}main{width:941px;height:1672px;position:relative;overflow:hidden}img{position:absolute;display:block}.bg,.reference{left:0;top:0;width:941px;height:1672px}.hero{left:-20px;top:474px;width:967px;height:967px}body.ref .bg,body.ref .hero,body:not(.ref) .reference{display:none}</style><header>Home R6 — full-source background and hero placement diagnostic<br>Offline incomplete art composition only; not Unity, not implemented UI. Full PNGs displayed unchanged.</header><main><img class="bg" src="../../Sources/home_environment_generated.png"><img class="hero" src="../../Sources/home_hero_firebird.png"><img class="reference" src="../../../../../../Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Home.png"></main><script>if(location.search.includes('reference'))document.body.className='ref';window.previewReady=Promise.all([...document.images].map(i=>i.decode()));</script>`;
 await fs.writeFile(path.join(qa,'HeroPoseReview.html'),html);
 for(const mode of ['pose','reference']){
  await load(pathToFileURL(path.join(qa,'HeroPoseReview.html')).href+(mode==='reference'?'?reference':''),941,1738);
  await shot(path.join(qa,mode==='reference'?'OriginalReferenceWithQALabel.png':'ProvisionalHeroPose.png'));
 }
 const metrics=await evaluate(`(()=>{const im=document.querySelector('.reference'),c=document.createElement('canvas');c.width=im.naturalWidth;c.height=im.naturalHeight;const ctx=c.getContext('2d',{willReadFrequently:true});ctx.drawImage(im,0,0);const d=ctx.getImageData(0,0,c.width,c.height).data;const results=[];for(const roi of [{name:'PlayLettering',x0:305,x1:641,y0:1080,y1:1177},{name:'QuestCaption',x0:247,x1:603,y0:1490,y1:1540}]){const rows=[],cols=[],pts=[];for(let y=roi.y0;y<roi.y1;y++){let n=0;for(let x=roi.x0;x<roi.x1;x++){const i=(y*c.width+x)*4;const [r,g,b]=d.slice(i,i+3);const yes=roi.name==='PlayLettering'?r>180&&g>100&&g<235&&b<190&&r>b*1.2:r>150&&g>145&&b>100&&Math.max(r,g,b)-Math.min(r,g,b)<80;if(yes){n++;pts.push([x,y]);}}rows.push({y,count:n});}for(let x=roi.x0;x<roi.x1;x++)cols.push({x,count:pts.filter(p=>p[0]===x).length});results.push({roi,rows,cols,count:pts.length,bounds:{minX:Math.min(...pts.map(p=>p[0])),maxX:Math.max(...pts.map(p=>p[0])),minY:Math.min(...pts.map(p=>p[1])),maxY:Math.max(...pts.map(p=>p[1]))}});}return results;})()`);
 await fs.writeFile(path.join(qa,'ReferenceGlyphColorMetrics.json'),JSON.stringify({method:'Read-only whole-reference canvas color predicates within declared diagnostic measurement regions; no cropped image is produced or used as artwork.',results:metrics},null,2)+'\n');
 console.log(JSON.stringify(metrics.map(m=>({name:m.roi.name,bounds:m.bounds,substantialRows:m.rows.filter(r=>r.count>20)})),null,2));
}finally{if(ws)ws.close();chrome.kill('SIGTERM');}
