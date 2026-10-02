from pathlib import Path
import json
base=Path(__file__).resolve().parent.parent
captures={}
for run in ['MarginPlayMode','Baseline']:
    captures[run]=[str(p.relative_to(base)) for p in sorted((base/'QA'/run/'Captures').rglob('*.png'))]
    assert captures[run],run
html='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>PreLevel and Pause — readability R1</title>
<style>body{margin:0;background:#111720;color:#f0e3cd;font:16px/1.5 system-ui,sans-serif}main{max-width:920px;margin:auto;padding:24px}h1{font-size:24px}a{color:#e9c986}label{display:inline-block;margin:0 16px 12px 0}select{font:inherit;color:inherit;background:#202b3b;padding:5px;max-width:100%}figure{margin:14px 0}img{display:block;width:360px;max-width:100%;height:auto;margin:auto}figcaption{text-align:center;margin:12px}.native{overflow:auto}.native img{max-width:none}</style>
<main><h1>PreLevel and Pause — readability R1</h1>
<p>Actual Unity Play Mode captures from the installed prefabs. Test saves and display sizes/safe areas are simulated; these are not physical-phone captures. Select one portrait at a time.</p>
<label>Version <select id="version"><option value="MarginPlayMode">Implemented</option><option value="Baseline">Before</option></select></label>
<label>Capture <select id="choice"></select></label>
<label><input id="native" type="checkbox"> Native capture pixels</label>
<figure id="frame"><img id="capture" alt="PreLevel or Pause actual Unity capture"><figcaption id="caption"></figcaption></figure>
<p>PreLevel: larger selected-round and claimed-reward captions; larger three-line booster explanations; consistently wrapped catalog action with checked decorative margins. Existing art, wording, fonts and actions are preserved.</p>
<p>Pause: a larger panel constrained to the safe area, larger remaining-goal values and action targets above 44 rendered pixels in the tested small portrait. Original Russian lettering is retained.</p>
<p><a href="TestResults.md">Test results and limits</a>. Small embedded artwork captions and PreLevel secondary controls remain below a universal text/touch minimum; this change is not whole-app readability acceptance. Physical-phone verification remains deferred.</p></main>
<script>
const captures=CAPTURES;
const version=document.getElementById('version'),choice=document.getElementById('choice'),native=document.getElementById('native'),capture=document.getElementById('capture'),frame=document.getElementById('frame'),caption=document.getElementById('caption');
function show(){capture.src=choice.value;capture.alt=choice.selectedOptions[0].text;caption.textContent=version.selectedOptions[0].text+' · '+choice.selectedOptions[0].text;frame.classList.toggle('native',native.checked);capture.style.width=native.checked?'auto':'360px';}
function populate(){const prior=choice.value.split('/').pop()||'PreLevel_360_RU.png';choice.replaceChildren();captures[version.value].forEach(path=>{const option=document.createElement('option');option.value=path;option.text=path.split('/').pop().replace('.png','').replaceAll('_',' ');choice.append(option);if(path.endsWith('/'+prior))choice.value=path;});show();}
version.addEventListener('change',populate);choice.addEventListener('change',show);native.addEventListener('change',show);populate();
</script></html>'''.replace('CAPTURES',json.dumps(captures))
(base/'PreLevel_Pause_Readability_R1_Review.html').write_text(html)
print('Review capture links validated:',sum(map(len,captures.values())))
