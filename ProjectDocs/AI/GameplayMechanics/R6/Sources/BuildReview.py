from pathlib import Path
import json,shutil,sys
base=Path(__file__).resolve().parent.parent;src=Path(sys.argv[1]);captures=base/'Captures';captures.mkdir(exist_ok=True)
samples=base/'QA/Samples';samples.mkdir(exist_ok=True)
for source in map(Path,sys.argv[1:]):
 for p in source.glob('Mechanics_Storage_*.png'):shutil.copy2(p,captures/p.name)
 for p in source.glob('Storage_*.json'):shutil.copy2(p,samples/p.name)
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Gameplay Mechanics R6 · Reliable local sessions</title>
<style>body{margin:0;background:#0b121c;color:#ecdcb9;font:17px/1.5 system-ui}main{max-width:1050px;margin:auto;padding:24px}h1{font-size:28px;color:#e1bd74}a{color:#9dd1f0}button,select,input{font:inherit;max-width:100%}button,select{background:#172638;color:#f6e7c3;border:1px solid #9a7741;border-radius:6px;padding:9px;margin:4px 0}#capture{display:block;width:min(100%,576px);height:auto;margin:20px auto;border:1px solid #8b703e}.note{color:#b0bfd0;font-size:14px}details,section{border-top:1px solid #415068;margin-top:24px;padding-top:16px}code{overflow-wrap:anywhere}.scroll{overflow-x:auto}table{border-collapse:collapse;width:100%;font-size:14px}th,td{padding:8px;text-align:left;border-bottom:1px solid #3a495a}#source{font-weight:bold;color:#efc56e}#error{color:#ffbeac}li{margin-bottom:8px}</style>
<main><h1>Gameplay Mechanics R6 · Reliable local sessions</h1><p>Playtest sessions and feedback now save locally across sandbox exits and normal Play Mode shutdown. The campaign and its saves remain unchanged.</p>
<label>Unity capture <select id="view"><option value="Win">Win with example feedback</option><option value="Error">Write failure with a visible retry action</option></select></label>
<img id="capture" src="Captures/Mechanics_Storage_Win.png" alt="Actual Unity capture of the playtest feedback panel">
<p class="note">Actual 576×1280 Unity captures. The ratings shown were entered by an automated test; they are not user feedback or a difficulty finding.</p>
<details open><summary>Record a manual session</summary><ol><li>Enter Play Mode and reach Home. Choose <b>Legends of Rus → Development → Gameplay Mechanics Sandbox</b>.</li><li>Play a round without reading the R4 solution. Tap Difficulty, Goals and Effects to cycle optional ratings. Leave them unrated when you have no opinion.</li><li>Use Retry or Next to continue. Wins, losses and interrupted attempts are kept separately; an animation interrupted by navigation is marked unsettled.</li><li>Watch for <b>Saved locally</b>. Settled moves, feedback changes, backgrounding and normal exit checkpoint the session; a 15-second checkpoint also runs during play. Use <b>Save now</b> to retry a failed save, or <b>Export JSON</b> for a separate snapshot. Copy location gives the latest explicit export path, or otherwise the checkpoint path. Nothing is uploaded.</li></ol><p class="note">Manual checkpoints are under <code>Application.persistentDataPath/MechanicsPlaytests/Sessions/manual_sandbox</code>; automated runs use <code>Sessions/automated_test</code>. Each session has a unique JSON file and a previous-copy .bak file. Only the last 200 closed attempts plus one active attempt are kept per session, with a dropped count. Older session files are not automatically deleted. Explicit exports remain unchanged snapshots. After a crash, an unfinished checkpoint remains ongoing; no outcome is inferred and the board is not resumed. Open the JSON file below to inspect it after leaving Play Mode. If a primary file is damaged, copy its .bak file to a separate .json file for inspection. Never count primary, backup and export snapshots of the same session as separate sessions.</p></details>
<section><h2>Inspect a checkpoint or exported session locally</h2><p>The file is read in this browser only. Automated examples are clearly identified; no win-rate or balance claim is inferred.</p><input id="file" type="file" accept=".json,application/json" aria-label="Choose playtest JSON"><button id="example">Load automated example</button><p id="error" role="alert"></p><p id="source"></p><p id="summary"></p><div class="scroll"><table><thead><tr><th>Attempt / Round</th><th>Outcome</th><th>Moves left</th><th>Seconds*</th><th>Difficulty</th><th>Goals</th><th>Effects</th><th>Board state</th></tr></thead><tbody id="records"></tbody></table></div><p class="note">*Foreground session time, including animations and decision time. It is not an FPS or response-time measurement. Background time and time after a terminal result are excluded.</p></section>
<p><a href="ImplementationReport.md">Implementation report</a> · <a href="QA/Verification.json">Test evidence</a> · <a href="QA/Scope.json">Changed files</a> · <a href="QA/Samples/Storage_Win.json">Automated example JSON</a></p></main>
<script>
const sample=SAMPLE;
document.querySelector('#view').onchange=e=>document.querySelector('#capture').src='Captures/Mechanics_Storage_'+e.target.value+'.png';
const validInt=(x,min,max)=>Number.isInteger(x)&&x>=min&&x<=max;
function inspect(data){
 document.querySelector('#records').replaceChildren();document.querySelector('#source').textContent='';document.querySelector('#summary').textContent='';document.querySelector('#error').textContent='';
 try{
 if(!data||data.schemaVersion!==1||!['manual_sandbox','automated_test'].includes(data.source)||!Array.isArray(data.attempts)||data.attempts.length>200||!Array.isArray(data.activeAttempts)||data.activeAttempts.length>1||!validInt(data.droppedAttempts,0,100000000))throw Error('Unsupported session format.');
 const rows=[...data.attempts,...data.activeAttempts],ids=new Set();
 for(const [i,a] of rows.entries()){
 if(!a||!validInt(a.number,1,100000000)||ids.has(a.number)||!validInt(a.round,1,5)||!validInt(a.moveBudget,1,1000)||!validInt(a.movesRemaining,0,a.moveBudget)||!Number.isFinite(a.activeSeconds)||a.activeSeconds<0||typeof a.snapshotSettled!=='boolean'||!['unrated','easy','fair','hard'].includes(a.difficulty)||!['unrated','clear','unclear'].includes(a.goalClarity)||!['unrated','readable','busy'].includes(a.effectClarity)||!Array.isArray(a.goalsRemaining)||a.goalsRemaining.length>3||!a.goalsRemaining.every(g=>validInt(g,0,100000)))throw Error('Invalid attempt record.');
 if(i<data.attempts.length?!['won','lost','interrupted'].includes(a.outcome):a.outcome!=='ongoing')throw Error('Invalid attempt outcome.');ids.add(a.number);
 }
 document.querySelector('#source').textContent=data.source==='automated_test'?'AUTOMATED TEST DATA — not human feedback':'MANUAL SANDBOX SESSION — self-recorded feedback';
 const count=s=>rows.filter(a=>a.outcome===s).length;
 document.querySelector('#summary').textContent=`${count('won')} wins · ${count('lost')} losses · ${count('interrupted')} interrupted · ${count('ongoing')} ongoing · ${data.droppedAttempts} older records dropped`;
 for(const a of rows){const tr=document.createElement('tr');for(const value of [`${a.number} / ${a.round}`,a.outcome,`${a.movesRemaining}/${a.moveBudget}`,a.activeSeconds.toFixed(1),a.difficulty,a.goalClarity,a.effectClarity,a.snapshotSettled?'settled':'interrupted/animating']){const td=document.createElement('td');td.textContent=value;tr.append(td)}document.querySelector('#records').append(tr)}
 }catch(e){document.querySelector('#error').textContent='Cannot load session: '+e.message;}
}
document.querySelector('#example').onclick=()=>inspect(sample);
document.querySelector('#file').onchange=async e=>{const f=e.target.files[0];if(!f)return;try{if(f.size>5000000)throw Error('File exceeds5MB.');inspect(JSON.parse(await f.text()));}catch(err){inspect(null);document.querySelector('#error').textContent='Cannot load session: '+err.message;}};
inspect(sample);
</script></html>'''
(base/'Gameplay_Mechanics_R6_Review.html').write_text(html.replace('SAMPLE',json.dumps(json.loads((src/'Storage_Win.json').read_text()),ensure_ascii=False)))
