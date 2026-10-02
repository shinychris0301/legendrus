from pathlib import Path
import re,html
base=Path(__file__).resolve().parent.parent

def inline(s):
 s=html.escape(s)
 s=re.sub(r'\[([^\]]+)\]\(([^)]+)\)',r'<a href="\2">\1</a>',s)
 s=re.sub(r'\*\*(.+?)\*\*',r'<strong>\1</strong>',s)
 return re.sub(r'`([^`]+)`',r'<code>\1</code>',s)

def render(text):
 lines=text.splitlines(); out=[];i=0
 while i<len(lines):
  line=lines[i]
  if not line.strip():i+=1;continue
  if line.startswith('|'):
   rows=[]
   while i<len(lines) and lines[i].startswith('|'):
    parts=[x.strip() for x in lines[i].strip('|').split('|')];i+=1
    if all(re.fullmatch(r'[-: ]+',x) for x in parts):continue
    tag='th' if not rows else 'td'
    rows.append('<tr>'+''.join(f'<{tag}>{inline(x)}</{tag}>' for x in parts)+'</tr>')
   out.append('<div class="table-wrap"><table><thead>'+rows[0]+'</thead><tbody>'+''.join(rows[1:])+'</tbody></table></div>');continue
  h=re.match(r'^(#{1,6}) (.*)',line)
  if h:out.append(f'<h{len(h[1])}>{inline(h[2])}</h{len(h[1])}>');i+=1;continue
  ordered=bool(re.match(r'^\d+\. ',line));unordered=line.startswith('- ')
  if ordered or unordered:
   tag='ol' if ordered else 'ul';items=[];pattern=r'^\d+\. ' if ordered else r'^- '
   while i<len(lines) and re.match(pattern,lines[i]):
    items.append('<li>'+inline(re.sub(pattern,'',lines[i]))+'</li>');i+=1
   out.append(f'<{tag}>'+''.join(items)+f'</{tag}>');continue
  para=[line];i+=1
  while i<len(lines) and lines[i].strip() and not re.match(r'^(#|\||- |\d+\. )',lines[i]):para.append(lines[i]);i+=1
  out.append('<p>'+inline(' '.join(para))+'</p>')
 return '\n'.join(out)
body=render((base/'DevelopmentStageReview.md').read_text())
scenario=render((base/'ScenarioCoverage.md').read_text())
page='''<!doctype html><html lang="en"><meta charset="utf-8"><meta name="viewport" content="width=device-width,initial-scale=1"><title>Development Stage Review R1</title><style>
:root{color-scheme:dark}*{box-sizing:border-box}body{margin:0;background:#0b1420;color:#e6eaf0;font:17px/1.65 system-ui,sans-serif}header,main{max-width:1250px;margin:auto;padding:24px}header{border-bottom:1px solid #65542f}h1{font-size:30px;line-height:1.25;color:#e6c581}h2{font-size:24px;color:#e6c581;margin-top:36px}h3{font-size:20px;color:#d9c399}a{color:#93caff}p,li{max-width:1050px}li{margin:12px 0}code{overflow-wrap:anywhere;color:#d0dbed}nav{display:flex;flex-wrap:wrap;gap:12px}button{font:inherit;background:#182b42;color:#fff;border:1px solid #7e704f;border-radius:6px;padding:10px 16px;cursor:pointer}button[aria-selected=true]{background:#375174}.table-wrap{overflow:auto}table{border-collapse:collapse;width:100%;font-size:15px}th,td{padding:13px;text-align:left;border:1px solid #354251;vertical-align:top}th{background:#223145}tr:nth-child(even){background:#111f2d}small{color:#b8c3ce}.metrics{display:flex;flex-wrap:wrap;gap:14px;margin:22px 0}.metric{background:#172535;border:1px solid #4e4b3b;padding:12px 20px;border-radius:6px}.metric strong{font-size:27px;color:#e6c581;display:block}.notice{border-left:4px solid #d7b56e;padding:8px 16px;background:#182333}[hidden]{display:none!important}@media(max-width:650px){header,main{padding:16px}h1{font-size:25px}th,td{min-width:140px}}@media print{body{background:white;color:black}nav{display:none}section[hidden]{display:block!important}th,td{color:black}.table-wrap{overflow:visible}}
</style><header><small>LEGENDS OF RUS · DEVELOPMENT AUDIT · 26 SEPTEMBER 2026</small><h1>From the playable slice to a complete campaign</h1><p class="notice">Development review document — not a game screen or a new Unity capture. Based on current local source and recorded verification evidence.</p><div class="metrics"><div class="metric"><strong>36</strong>native UI pages</div><div class="metric"><strong>10</strong>authored playable rounds</div><div class="metric"><strong>20 × 500</strong>approved campaign target</div><div class="metric"><strong>8</strong>Chapter 1 scenes</div></div><nav aria-label="Report sections"><button type="button" data-panel="stage" aria-selected="true">Status and next steps</button><button type="button" data-panel="scenario" aria-selected="false">Scenario coverage</button><a href="QA/Inventory.json">Evidence inventory</a></nav></header><main><section id="stage">'''+body+'''</section><section id="scenario" hidden>'''+scenario+'''</section></main><script>document.querySelectorAll('button[data-panel]').forEach(b=>b.addEventListener('click',()=>{document.querySelectorAll('main>section').forEach(s=>s.hidden=s.id!==b.dataset.panel);document.querySelectorAll('button[data-panel]').forEach(x=>x.setAttribute('aria-selected',String(x===b)));}));</script></html>'''
(base/'Development_Stage_Review_R1.html').write_text(page)
print('Wrote review:',len(page),'characters')
