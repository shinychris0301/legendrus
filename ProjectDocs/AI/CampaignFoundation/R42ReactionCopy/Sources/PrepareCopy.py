from pathlib import Path
import json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
levels=json.loads((active/'levels.json').read_text())['levels'];story=json.loads((active/'story.json').read_text())
copy={
91:('Последние льдинки у краёв разбиты. Утро встречает нас тишиной.','The last ice at the edges is broken. The morning greets us quietly.',
    'Проверю четыре крайние клетки и оставшуюся цель по самоцветам.','I will check the four edge cells and the remaining gem goal.',
    'Four single-layer ice cells at the outer columns and a sixteen-gem color goal; no delivery items.'),
92:('Цепи и наледь убраны. К воде снова можно подойти без обхода.','The chains and ice are cleared. We can reach the water without a detour.',
    'Очищу не только середину: лёд остался целью и по краям.','I must clear more than the center; the ice near the edges is a goal too.',
    'Eight single-layer ice cells near the upper/lower sides and four central chains. No color or delivery goal.'),
93:('Оба пера добрались до нас. Две дороги к колодцу готовы.','Both feathers have reached us. The two roads to the well are ready.',
    'Нижние ящики крепче верхних. Учту это на обеих дорогах.','The lower crates are sturdier than the upper ones. I will plan for both paths.',
    'Eight crates, two feathers in columns two and six; only the two crates at row five have two layers.'),
94:('Самоцветы двух цветов собраны. Над улицей снова пляшут искры.','Both gem colors are gathered. Sparks dance above the street again.',
    'Цветов на поле шесть, а нужны два. Сверюсь с их счётчиками.','There are six colors on the board, but we need two. I will check their counters.',
    'Six available tile colors; two goals of twenty-four gems each. No obstacles or items.'),
95:('Три пера спасены, преграды сняты. Свет доберётся до каждого дома.','Three feathers are safe and the obstacles are gone. Light can reach each home.',
    'Проверю крепкие цепи внизу всех трёх путей и отдельные клетки льда.','I will check the sturdy chains below all three paths and the separate ice cells.',
    'Three feathers; six chains, the three lower ones with two layers, and six separate single-layer ice cells.'),
96:('Нужные самоцветы у нас. У воды можно ненадолго перевести дух.','We have the gems we need. We can catch our breath by the water.',
    'Здесь нужны только два цвета. Сосредоточусь на том, которого не хватает.','Only two colors are needed here. I will focus on the one still missing.',
    'Two sixteen-gem goals, no obstacles, delivery items or survival counter. Rest is narrative, not a timer.'),
97:('Лёд между поленницами разбит. К запасам снова ведёт чистый проход.','The ice between the woodpiles is broken. The way to the supplies is clear.',
    'Ящики по краям хрупкие, но лёд в середине нужно разбить в два приёма.','The outer crates take one hit, but the central ice needs two.',
    'Eight outer single-layer crates and four central two-layer ice cells. No delivery or collection goal.'),
98:('Выдержали и сняли цепи. Дороги к домам удалось сберечь.','We held out and removed the chains. The roads to the houses are safe.',
    'Нужно сделать не меньше восемнадцати ходов и снять цепи.','We must make at least eighteen moves and remove the chains.',
    'Survival target eighteen counts consumed moves, not seconds; six single-layer chains. Completion can occur after move eighteen.'),
99:('Оба пера и самоцветы собраны. Искры не погасли во льду.','Both feathers and the gems are gathered. The ice did not quench the sparks.',
    'Одних перьев мало: проверю лёд и счётчик нужных самоцветов.','Feathers alone are not enough; I will check the ice and target gems.',
    'Eight single-layer ice cells, two feathers and twenty-two gems of one color. All three goals required.'),
100:('Дороги связали колодец с очагами. Деревне будет легче.','The well and hearths are linked again. That will help the village.',
     'Под перьями есть цепи, а по бокам — крепкие ящики. Проверю все цели.','Chains lie beneath the feathers, with sturdy crates at the sides. I will check every goal.',
     'Eight crates including four with two layers, six single-layer chains and three feathers. Preserve the existing well_and_hearths milestone toward the mill; this is not a campaign finale.')
}
rows=[]
for n,values in copy.items():
 level=next(x for x in levels if x['levelId']==n)
 for outcome,ru,en in [('victory',values[0],values[1]),('failure',values[2],values[3])]:
  r=next(x for x in story['reactions'] if x['levelId']==n and x['outcome']==outcome)
  rows.append(dict(round=n,outcome=outcome,key=r['textKey'],speakerKey=r['speakerKey'],ru=ru,en=en,review=values[4],reviewedAgainst={k:level[k] for k in ['goals','obstacles','boardItems']}))
(base/'Reaction91To100Draft.json').write_text(json.dumps(dict(status='Reviewed against installed goals, durability, delivery positions, consumed-move survival and adjacent story scenes before installation.',entries=rows),ensure_ascii=False,indent=2)+'\n')
print('Prepared 20 bilingual reaction pairs.')
