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
    await send('Page.navigate',{url:pathToFileURL(join(base,'GameplayStock_R5_Review.html')).href});
    for(let i=0;i<100;i++){
        if(await evaluate("document.getElementById('portrait')?.complete && document.getElementById('portrait').naturalWidth===360"))break;
        await pause(100);
    }
    const checks=[];
    async function check(name,expression,expected){const observed=await evaluate(expression);assert.equal(observed,expected,name);checks.push({name,observed,expected});}
    await check('Default final portrait exists',"document.querySelector('#portrait').naturalWidth",360);
    await check('Default metric uses actual mesh ink',"document.querySelector('#measurement').textContent.includes('11.00')",true);
    await evaluate("document.querySelector('#fit').value='previous';document.querySelector('#fit').dispatchEvent(new Event('change'))");
    await check('Circular control selection',"document.querySelector('#portrait').src.endsWith('Stock_PreviousCircular_9999.png')",true);
    await evaluate("document.querySelector('#language').value='en';document.querySelector('#language').dispatchEvent(new Event('change'))");
    await check('English returns to valid current image',"document.querySelector('#portrait').src.endsWith('Stock_9999_en_360.png')",true);
    await evaluate("document.querySelector('#quantity').value='2147483647';document.querySelector('#quantity').dispatchEvent(new Event('change'))");
    await check('Exact maximum quantity image',"document.querySelector('#portrait').src.endsWith('Stock_2147483647_en_360.png')",true);
    await evaluate("document.querySelector('#width').value='1080';document.querySelector('#width').dispatchEvent(new Event('change'))");
    await check('Tall native portrait selection',"document.querySelector('#portrait').src.endsWith('Stock_2147483647_en_1080.png')",true);
    await check('Evidence links',"document.querySelectorAll('a[href]').length",3);
    await evaluate("document.querySelector('#quantity').value='9999';document.querySelector('#language').value='ru';document.querySelector('#width').value='360';document.querySelector('#width').dispatchEvent(new Event('change'))");
    await pause(250);
    for(const [name,width,height] of [['Desktop',1280,900],['Portrait',390,844]]){
        await send('Emulation.setDeviceMetricsOverride',{width,height,deviceScaleFactor:1,mobile:false});
        await pause(150);
        const capture=await send('Page.captureScreenshot',{format:'png'});
        await writeFile(join(out,`${name}.png`),Buffer.from(capture.data,'base64'));
    }
    assert.deepEqual(errors,[],'JavaScript errors');
    await writeFile(join(out,'Checks.json'),JSON.stringify({passed:checks.length,failed:0,checks,errors,
        scope:'Review-page browser checks in isolated Chrome profile; embedded portraits are actual Unity captures, browser screenshots are documentation'},null,2)+'\n');
    console.log(JSON.stringify({passed:checks.length,failed:0,errors:errors.length}));
} finally {
    if(ws)ws.close();chrome.kill('SIGTERM');
    await pause(300);await rm(profile,{recursive:true,force:true,maxRetries:3,retryDelay:300});
}
