from pathlib import Path
import json

BASE = Path(__file__).resolve().parent.parent
data = json.loads((BASE / 'CoverageMatrix.json').read_text())
embedded = json.dumps(data, ensure_ascii=False).replace('<', '\\u003c')
html = '''<!doctype html>
<html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1">
<title>Chapter 1 human acceptance — collection coverage</title>
<style>
:root{color-scheme:dark}body{margin:0;background:#101724;color:#e9e5dd;font:16px/1.5 system-ui,sans-serif}
main{max-width:1200px;margin:auto;padding:28px}h1{font-size:28px;color:#e6c880}h2{font-size:21px}
a{color:#e6c880}p{max-width:90ch}.metrics{display:flex;gap:16px;flex-wrap:wrap}.metric{padding:14px 22px;background:#1b2839;border:1px solid #516176;border-radius:8px}
.metric strong{font-size:28px;display:block}code{overflow-wrap:anywhere}label{display:inline-block;margin:0 14px 12px 0}
select,input{padding:9px;background:#1b2839;color:#fff;border:1px solid #78889e;border-radius:4px;font:inherit}
.scroll{overflow:auto;max-height:65vh;border:1px solid #516176}table{border-collapse:collapse;width:100%;font-size:14px}
th,td{padding:10px;text-align:left;border-bottom:1px solid #39485c;vertical-align:top}th{background:#243248;position:sticky;top:0;z-index:1}
tr:nth-child(even){background:#182234}.pending{color:#efd18a}.small{font-size:14px;color:#b8c5d8}button{font:inherit;padding:8px 12px;border:1px solid #78889e;border-radius:4px;background:#243248;color:#fff;cursor:pointer}
</style><main>
<p class="small">Planning report · offline evidence audit · not a game page or Unity gameplay capture</p>
<h1>Chapter 1 human acceptance</h1>
<p id="finding"></p><div class="metrics" id="metrics"></div>
<p>Historical manual results for 11–30 remain preserved. Solver results and automated test ratings do not establish human difficulty or pacing. Authored labels below describe the installed content.</p>
<p><a href="CollectionInstructions.md">How to record the next block</a> · <a href="CoverageMatrix.csv">470-round CSV</a> · <a href="CollectionQueue.csv">47-block queue</a> · <a href="TestResults.md">Audit evidence and limits</a></p>
<h2>Collection order</h2><p>Begin 201–210 and finish 201–250; then record 491–500, including hard 495, recovery 496 and finale 500. Review sector checkpoints and finish the remaining gaps.</p>
<details><summary>Show the complete ordered block queue</summary><div class="scroll"><table><thead><tr><th>Order</th><th>Priority</th><th>Block</th><th>Hard rounds</th><th>Recovery</th><th>Rounds missing</th></tr></thead><tbody id="queue"></tbody></table></div></details>
<h2>Round coverage</h2>
<label>Priority <select id="priority"><option value="">All</option><option value="1" selected>1 · 201–250</option><option value="2">2 · Finale block</option><option value="3">3 · Late checkpoints</option><option value="4">4 · Earlier gaps</option><option value="5">5 · Remaining late blocks</option></select></label>
<label>Authored difficulty <select id="difficulty"><option value="">All</option><option>hard</option><option>recovery</option><option>normal</option><option>easy</option></select></label>
<label>Round or title <input id="search" placeholder="e.g. 495 or title" type="search"></label><button id="reset">Show all rounds</button>
<p id="count" aria-live="polite"></p><div class="scroll"><table><thead><tr><th>Round / block</th><th>Title</th><th>Authored difficulty / moves</th><th>Goals / obstacles</th><th>Human recording</th><th>Story after win</th></tr></thead><tbody id="rounds"></tbody></table></div>
<p class="small">Installed content fingerprint: <code id="fingerprint"></code>. This report is a snapshot of the two documented search scopes. External folders and devices were not searched. A manual recorder source tag does not independently identify the player.</p>
</main><script id="data" type="application/json">__DATA__</script><script>
'use strict';
const data=JSON.parse(document.getElementById('data').textContent), s=data.summary;
const el=id=>document.getElementById(id);
const esc=value=>String(value).replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
el('finding').textContent=`${s.currentManualRounds31To500} of 470 rounds have current manual outcomes in the searched evidence. ${s.roundsNeedingRecording} rounds across ${s.blocksNeedingRecording} blocks still need recording.`;
el('metrics').innerHTML=[[s.currentRatedRounds31To500,'Current rated rounds'],[s.roundsNeedingRecording,'Rounds to collect'],[s.blocksNeedingRecording,'Blocks to collect']].map(([n,t])=>`<div class="metric"><strong>${esc(n)}</strong>${esc(t)}</div>`).join('');
el('fingerprint').textContent=s.contentFingerprint;
el('queue').innerHTML=data.blocks.map((b,i)=>`<tr><td>${i+1}</td><td>${b.priority}</td><td>${b.start}–${b.end}</td><td>${esc(b.hardRounds.join(', '))}</td><td>${esc(b.recoveryRounds.join(', '))}</td><td>${b.roundsNeedingRecording.length}</td></tr>`).join('');
function render(){
 const p=el('priority').value,d=el('difficulty').value,q=el('search').value.trim().toLocaleLowerCase();
 const rows=data.rounds.filter(r=>(!p||String(r.priority)===p)&&(!d||r.authoredDifficulty===d)&&(!q||String(r.round).includes(q)||r.title.toLocaleLowerCase().includes(q)));
 el('count').textContent=`Showing ${rows.length} of ${data.rounds.length} rounds`;
 el('rounds').innerHTML=rows.map(r=>`<tr><td>${r.round}<br><span class="small">${r.blockStart}–${r.blockEnd}</span></td><td>${esc(r.title)}</td><td>${esc(r.authoredDifficulty)} (${r.authoredDifficultyRating})<br>${r.moveLimit} moves</td><td>${esc(r.goals)}<br><span class="small">${esc(r.obstacleTypes)}</span></td><td class="pending">${esc(r.collectionStatus.replaceAll('_',' '))}<br><span class="small">${r.currentManualOutcomes} outcomes / ${r.currentRatedOutcomes} rated</span></td><td>${esc(r.storyScenesAfterWin||'—')}</td></tr>`).join('');
}
['priority','difficulty','search'].forEach(id=>el(id).addEventListener('input',render));
el('reset').addEventListener('click',()=>{['priority','difficulty','search'].forEach(id=>el(id).value='');render();});render();
</script></html>
'''.replace('__DATA__', embedded)
(BASE / 'CoverageReview.html').write_text(html)
print('Standalone review generated:', len(data['rounds']), 'rounds;', len(data['blocks']), 'ordered blocks')
