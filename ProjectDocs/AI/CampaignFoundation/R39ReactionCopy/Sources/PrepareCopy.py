from pathlib import Path
import json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
levels=json.loads((active/'levels.json').read_text())['levels'];story=json.loads((active/'story.json').read_text())
copy={
61:('Лёд у края улицы разбит. Мы ближе к дальним домам.','The ice at the street edge is broken. The far houses are closer.',
    'Проверю, где остался лёд и сколько самоцветов ещё нужно.','I will check the remaining ice and how many gems we still need.',
    'Four single-layer ice cells and one color goal; no delivery items.'),
62:('Оба завала разобраны. К заборам снова можно подойти.','Both piles are cleared. We can reach the fences again.',
    'Крепкие ящики требуют повторного удара. Не оставлю их напоследок.','Sturdy crates need another hit. I will not leave them until last.',
    'Two separated groups of crates, four with two layers; one color goal.'),
63:('Цепи сняты, оба пера собраны. Калитки больше не преграда.','The chains are gone and both feathers are gathered. The gates are clear.',
    'Освобожу клетки под перьями от цепей, чтобы перья могли опуститься.','I will clear the chains beneath the feathers so they can drop.',
    'Six single-layer chains, including cells below both delivery feathers.'),
64:('Лёд разбит и самоцветы собраны. Проход снова свободен.','The ice is broken and the gems are gathered. The passage is clear again.',
    'Проверю лёд по краям поля и оставшиеся самоцветы цели.','I will check the ice along the edges and the gems still needed.',
    'Ten single-layer ice cells concentrated near sides; one color goal; six tile colors.'),
65:('Все три пера добрались вниз. Пороги расчищены.','All three feathers reached the bottom. The thresholds are clear.',
    'Проверю нижние ящики: они могут задерживать перья.','I will check the lower crates; they may be holding up the feathers.',
    'Three delivery columns, nine crates; three two-layer crates at row five.'),
66:('Оба цвета собраны. Свет в дальних окнах уже виден.','Both colors are gathered. I can see light in the distant windows.',
    'Здесь нет преград. Сосредоточусь на двух нужных цветах.','There are no obstacles here. I will focus on the two target colors.',
    'Two color goals only; no obstacles or board items.'),
67:('Выстояли, и цепей больше нет. Можно идти дальше.','We held out, and the chains are gone. We can move on.',
    'Сниму цепи и продолжу делать ходы до выполнения цели.','I will remove the chains and keep making moves until the goal is met.',
    'Survival counts sixteen consumed moves, not elapsed seconds; six single-layer chains.'),
68:('Ящики разобраны, лёд между ними разбит. Дорога открыта.','The crates are cleared and the ice between them is broken. The way is open.',
    'Разберу крепкие ящики, не забывая о льде между ними.','I will break the sturdy crates and watch for ice between them.',
    'Six two-layer crates in separated groups and six central ice cells; no color or delivery goal.'),
69:('Цепи сняты, перья и самоцветы собраны. Ворота свободны.','The chains are gone, and feathers and gems are gathered. The gates are clear.',
    'Одного удара по цепям мало. Проверю также перья и самоцветы.','These chains need more than one hit. I will check feathers and gems too.',
    'Four two-layer chains, two feathers and a color goal; all three goals matter.'),
70:('Перья собраны, завалы убраны. Дальняя улица свободна.','The feathers are gathered and the debris is cleared. The far street is open.',
    'Расчищу путь под обоими перьями и проверю оставшийся лёд.','I will clear the way beneath both feathers and check the remaining ice.',
    'Ten crates, six ice cells and two delivery feathers; retain the existing far-street milestone scene.')
}
rows=[]
for n,values in copy.items():
 level=next(x for x in levels if x['levelId']==n)
 for outcome,ru,en in [('victory',values[0],values[1]),('failure',values[2],values[3])]:
  r=next(x for x in story['reactions'] if x['levelId']==n and x['outcome']==outcome)
  rows.append(dict(round=n,outcome=outcome,key=r['textKey'],speakerKey=r['speakerKey'],ru=ru,en=en,review=values[4],reviewedAgainst={k:level[k] for k in ['goals','obstacles','boardItems']}))
(base/'Reaction61To70Draft.json').write_text(json.dumps(dict(status='Reviewed against installed goals, obstacle layers, delivery columns and adjacent story scenes before installation.',entries=rows),ensure_ascii=False,indent=2)+'\n')
print('Prepared 20 bilingual reaction pairs.')
