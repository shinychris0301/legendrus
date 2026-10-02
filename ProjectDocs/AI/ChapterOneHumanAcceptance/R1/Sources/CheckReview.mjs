import {spawn} from 'node:child_process';
import {mkdtemp, readFile, writeFile, mkdir, rm} from 'node:fs/promises';
import {tmpdir} from 'node:os';
import {join} from 'node:path';
import {fileURLToPath, pathToFileURL} from 'node:url';
import assert from 'node:assert/strict';

const base=fileURLToPath(new URL('../',import.meta.url)), out=join(base,'QA/Browser');
await mkdir(out,{recursive:true});
const profile=await mkdtemp(join(tmpdir(),'lor-acceptance-review-'));
const chrome=spawn('/usr/bin/google-chrome',['--headless','--no-sandbox','--disable-gpu',
    `--user-data-dir=${profile}`,'--remote-debugging-port=0','about:blank'],{stdio:'ignore'});
let ws;
const pending=new Map(), errors=[];
let sequence=0;
const pause=ms=>new Promise(resolve=>setTimeout(resolve,ms));
try {
    let port;
    for(let i=0;i<100;i++){
        try {port=(await readFile(join(profile,'DevToolsActivePort'),'utf8')).split('\n')[0];break;}catch{}
        await pause(100);
    }
    assert.ok(port,'Chrome did not start');
    const page=await (await fetch(`http://127.0.0.1:${port}/json/new?about:blank`,{method:'PUT'})).json();
    ws=new WebSocket(page.webSocketDebuggerUrl);
    await new Promise((resolve,reject)=>{ws.addEventListener('open',resolve,{once:true});ws.addEventListener('error',reject,{once:true});});
    ws.addEventListener('message',event=>{
        const message=JSON.parse(event.data);
        if(message.method==='Runtime.exceptionThrown') errors.push(message.params.exceptionDetails);
        if(message.id){const callbacks=pending.get(message.id);pending.delete(message.id);
            if(message.error) callbacks.reject(new Error(JSON.stringify(message.error)));else callbacks.resolve(message.result);}
    });
    function send(method,params={}){
        return new Promise((resolve,reject)=>{const id=++sequence;pending.set(id,{resolve,reject});ws.send(JSON.stringify({id,method,params}));});
    }
    async function evaluate(expression){
        const result=await send('Runtime.evaluate',{expression,returnByValue:true,awaitPromise:true});
        assert.ok(!result.exceptionDetails,JSON.stringify(result.exceptionDetails));return result.result.value;
    }
    await send('Page.enable');await send('Runtime.enable');
    await send('Page.navigate',{url:pathToFileURL(join(base,'CoverageReview.html')).href});
    for(let i=0;i<100;i++){
        if(await evaluate("document.getElementById('rounds')?.children.length===50"))break;
        await pause(100);
    }
    const checks=[];
    async function check(name,expression,expected){const observed=await evaluate(expression);assert.equal(observed,expected,name);checks.push({name,observed,expected});}
    await check('Default priority1 shows all 201–250',"document.querySelector('#rounds').children.length",50);
    await check('Ordered queue has 47 blocks',"document.querySelector('#queue').children.length",47);
    await evaluate("document.querySelector('#priority').value='2';document.querySelector('#priority').dispatchEvent(new Event('input'))");
    await check('Finale block filter includes 491–500',"document.querySelector('#rounds').children.length",10);
    await evaluate("document.querySelector('#difficulty').value='hard';document.querySelector('#difficulty').dispatchEvent(new Event('input'))");
    await check('Finale hard rounds only 495 and500',"document.querySelector('#rounds').children.length",2);
    await evaluate("document.querySelector('#search').value='495';document.querySelector('#search').dispatchEvent(new Event('input'))");
    await check('Round search identifies495',"document.querySelector('#rounds').children.length",1);
    await evaluate("document.querySelector('#reset').click()");
    await check('Reset shows every missing round',"document.querySelector('#rounds').children.length",470);
    await check('All four report links exist',"document.querySelectorAll('a[href]').length",4);
    for(const [name,width,height] of [['Desktop',1280,900],['Portrait',390,844]]){
        await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});
        await pause(150);
        const capture=await send('Page.captureScreenshot',{format:'png'});
        await writeFile(join(out,`${name}.png`),Buffer.from(capture.data,'base64'));
    }
    assert.deepEqual(errors,[],'JavaScript errors');
    await writeFile(join(out,'Checks.json'),JSON.stringify({passed:checks.length,failed:0,checks,errors,
        scope:'Offline planning report, isolated Chrome profile; not Unity or gameplay screenshots'},null,2)+'\n');
    console.log(JSON.stringify({passed:checks.length,failed:0,errors:errors.length}));
} finally {
    if(ws)ws.close();chrome.kill('SIGTERM');
    await pause(300);await rm(profile,{recursive:true,force:true,maxRetries:3,retryDelay:300});
}
