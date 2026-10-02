from pathlib import Path
b=Path(__file__).resolve().parent.parent
# The latest user instruction supersedes the borderless prototype.
s=(b/'Sources/BuildPreview.py').read_text()
s=s.replace('WorldMap_NavigationAndRegions_R2_Review','WorldMap_ReferenceCards_R2_Review')
s=s.replace('World Map — Twenty Regions','World Map — Reference Chapter Cards')
s=s.replace('Assets/title.png','Assets/chapter_title.png').replace('Assets/back.png','Assets/back_source.png')
s=s.replace('<option value="3">3 + next-region preview</option><option value="4">4 + next-region preview</option>', '<option value="3">3 larger chapter cards</option><option value="4" selected>4 chapter cards · reference scale</option>')
s=s.replace('<option value="3" selected>Chapters 1–2 complete · 3 active</option>', '<option value="2" selected>Chapter 1 complete · 2 active</option><option value="3">Chapters 1–2 complete · 3 active</option>')
s=s.replace('<div id="stage">','<div id="stage" class="compact">')
s=s.replace('<button class="review-button" id="current">', '<label>Reading order<select id="order"><option value="ascending">Later chapters above · drag down</option><option value="reference">Reference comparison · first chapter on top</option></select></label><button class="review-button" id="current">')
start=s.index('<nav class="footer"');end=s.index('</nav>',start)+len('</nav>')
nav='<nav class="footer" aria-label="Reference navigation"><img class="footer-carving art" src="Assets/source_footer_carving.png" alt="">'
for name,label in [('home','Главная'),('map','Карта'),('tasks','Задания'),('guild','Гильдия'),('shop','Магазин')]:
    nav+=f'<button class="nav" data-nav="{name}" aria-label="{label}"><img src="Assets/nav_{name}.png" alt="{label}"></button>'
nav+='</nav><button class="to-map" aria-label="Open selected chapter"><img src="Assets/map_button_source.png" alt="К карте"></button>'
s=s[:start]+nav+s[end:]
start=s.index('function render(){');end=s.index('\nprogress.onchange=',start)
render='''function render(){active=+progress.value;map.innerHTML='';const list=document.getElementById('order').value==='ascending'?[...chapters].reverse():chapters;list.forEach(c=>{let state=c.id<active?'completed':c.id===active?'active':'locked';let status=state==='completed'?'Пройдено':state==='active'?'Доступно · продолжить':'Не пройдено · закрыто';let el=document.createElement('button');el.className='region '+state;el.dataset.chapter=c.id;el.setAttribute('aria-label',`Глава ${c.id}. ${c.ru}. ${status}`);if(state==='locked')el.setAttribute('aria-disabled','true');const count=state==='completed'?'15/15':state==='active'?'9/15':'0/15';el.innerHTML=`<span class="card-face"></span><img class="terrain" src="../DesignR1/Assets/region_${String(c.id).padStart(2,'0')}.png" alt=""><span class="shade"></span><img class="frame" src="Assets/card_frame.png" alt=""><span class="copy"><span class="chapter">ГЛАВА ${c.id}</span><strong class="name">${c.ru}</strong><span class="status">${status}</span><span class="progress-plate"><span>${count}</span></span>${state==='active'?'<img class="continue-art" src="Assets/continue_source.png" alt="Продолжить">':''}</span>${state==='locked'?'<img class="lock" src="Assets/lock_source.png" alt="Закрыто">':''}`;el.onclick=()=>notice(state==='locked'?`Глава ${c.id}: завершите предыдущую главу. Поздние главы ещё не реализованы.`:`${c.ru} — переход к карте главы. Демонстрация дизайна.`);map.append(el)});map.querySelectorAll('.name').forEach(el=>{let size=30;el.style.fontSize=size+'px';while(el.offsetHeight>62&&size>22){size--;el.style.fontSize=size+'px'}});map.scrollTop=document.getElementById('order').value==='ascending'?map.scrollHeight:0}
'''
s=s[:start]+render+s[end:]
s=s.replace("progress.onchange=render;", "progress.onchange=render;document.getElementById('order').onchange=()=>{render();document.querySelector('.scrollHint').textContent=document.getElementById('order').value==='ascending'?'Тяните вниз — впереди новые земли':'Порядок оригинала · листайте вверх'};document.querySelector('.to-map').onclick=()=>notice('Переход к текущей главе — демонстрация дизайна.');")
s=s.replace("({shop:'Магазин',events:'События',characters:'Герои'})", "({home:'Главная',shop:'Магазин',tasks:'Задания',guild:'Гильдия'})")
s=s.replace('The progress selector shows design examples only.', 'The progress selector and 15/15, 9/15 examples illustrate the supplied reference; they are not saved player data or authored level counts.')
s=s.replace('render();\nlet drag=', 'render();document.fonts.ready.then(render);\nlet drag=')
s=s.replace('</style>',(b/'Sources/ReferenceLayout.css').read_text()+'\n</style>')
s=s.replace("(b/'WorldMap_ReferenceCards_R2_Review.html').write_text(html)","(b/'WorldMap_ReferenceCards_R2_Review.html').write_text(html)")
(b/'Sources/BuildReferencePreview.py').write_text(s)
