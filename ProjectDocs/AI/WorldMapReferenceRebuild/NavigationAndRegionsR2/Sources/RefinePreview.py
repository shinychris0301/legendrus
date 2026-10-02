"""Apply the R2 design contract to the retained R1 preview builder."""
from pathlib import Path
b=Path(__file__).resolve().parent.parent
s=(b.parent/'DesignR1/Sources/BuildPreview.py').read_text()
s=s.replace('Design R1','Design R2').replace('WorldMap_Regions_Design_R1_Review','WorldMap_NavigationAndRegions_R2_Review')
s=s.replace('Swipe upward inside the map to reveal later chapters.', 'Later chapters lie above: drag downward inside the map to reveal them. Back is at the bottom.')
s=s.replace('20 глав · листайте вверх ↑','Тяните вниз — впереди новые земли')
s=s.replace('Assets/source_header.png','Assets/header_joined.png')
s=s.replace('chapters.forEach(c=>','[...chapters].reverse().forEach(c=>')
s=s.replace('Assets/region_${String(c.id)', '../DesignR1/Assets/region_${String(c.id)')
s=s.replace("'Доступно · продолжить'", "'Не пройдено · доступно'")
s=s.replace("'Закрыто'", "'Не пройдено · закрыто'")
s=s.replace('<span class="chapter">ГЛАВА ${c.id}</span>', '<span class="chapter">ГЛАВА</span>')
s=s.replace('map.scrollTop=old}', 'map.scrollTop=map.scrollHeight}')
s=s.replace('top:0,behavior:', 'top:map.scrollHeight,behavior:')
s=s.replace("top:Math.max(0,el.offsetTop-80)","top:Math.max(0,el.offsetTop-map.clientHeight+el.offsetHeight)")
s=s.replace("stage.classList.toggle('compact',e.target.value==='4')", "{stage.classList.toggle('compact',e.target.value==='4');map.scrollTop=map.scrollHeight}")
start=s.index('<nav class="footer"')
end=s.index('</nav>',start)+len('</nav>')
s=s[:start]+'''<nav class="footer" aria-label="Shared Home navigation"><img class="footer-carving art" src="Assets/source_footer_carving.png" alt=""><button class="nav" data-nav="map" aria-label="Map"><img src="Assets/source_map_ru.png" alt="Карта мира"></button><button class="nav" data-nav="shop" aria-label="Shop"><img src="Assets/source_shop_ru.png" alt="Магазин"></button><button class="nav" data-nav="events" aria-label="Events"><img src="Assets/source_events_ru.png" alt="События"></button><button class="nav" data-nav="characters" aria-label="Characters"><img src="Assets/source_characters_ru.png" alt="Герои"></button><span class="footer-direction">20 земель · одно путешествие</span></nav>'''+s[end:]
s=s.replace("b.dataset.nav==='tasks'?'Задания главы — демонстрация навигации.':'Гильдия — демонстрация навигации.'", "({shop:'Магазин',events:'События',characters:'Герои'})[b.dataset.nav]+' — демонстрация навигации.'")
# Native touch scrolling is retained. Mouse dragging mirrors touch at the scaled stage size.
gesture='''
let drag=null,suppressClick=false;
map.addEventListener('pointerdown',e=>{if(e.pointerType!=='mouse'||e.button!==0)return;drag={x:e.clientX,y:e.clientY,top:map.scrollTop,id:e.pointerId,moved:false}});
map.addEventListener('pointermove',e=>{if(!drag)return;let dy=e.clientY-drag.y;if(Math.abs(dy)>5&&!drag.moved){drag.moved=true;map.setPointerCapture(e.pointerId)}if(drag.moved){let scale=document.getElementById('device').clientWidth/941;map.scrollTop=drag.top-dy/scale;e.preventDefault()}});
function endDrag(){if(drag&&drag.moved){suppressClick=true;setTimeout(()=>suppressClick=false,0)}drag=null}
map.addEventListener('pointerup',endDrag);map.addEventListener('pointercancel',endDrag);
map.addEventListener('click',e=>{if(suppressClick){e.preventDefault();e.stopImmediatePropagation()}},true);
'''
s=s.replace('render();\n</script>', 'render();'+gesture+'\n</script>')
css=(b/'Sources/Refinement.css').read_text()
s=s.replace('</style>',css+'\n</style>')
s=s.replace('href="Sources/OriginalReference.png"','href="../DesignR1/Sources/OriginalReference.png"')
(b/'Sources/BuildPreview.py').write_text(s)
