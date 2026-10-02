from pathlib import Path
import json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
levels=json.loads((active/'levels.json').read_text())['levels'];story=json.loads((active/'story.json').read_text())
copy={
81:('Самоцветы собраны. У домашнего огня можно перевести дух.','The gems are gathered. We can catch our breath by the hearth.',
    'Сначала сверюсь с целями: здесь нужны самоцветы двух цветов.','I will check the goals first: we need gems of two colors here.',
    'Two color goals of sixteen each; no obstacles, delivery items or survival timer.'),
82:('У крыльца больше нет льда. Можно подойти к тёплому дому.','The ice by the porch is gone. We can reach the warmth of the house.',
    'Лёд есть и внизу поля. Не забуду о нём, собирая самоцветы.','There is ice near the bottom too. I will watch for it while gathering gems.',
    'Eight single-layer ice cells, including two at row five; one twenty-gem color goal.'),
83:('Перо спустилось, тропа очищена. Пройдём между домами.','The feather is down and the path is clear. We can pass between the houses.',
    'Проверю толстый лёд в середине и ящики по сторонам.','I will check the thick ice in the middle and the crates on either side.',
    'One feather in column three; four two-layer ice cells there and six side crates. Do not imply ice behaves like a fixed crate.'),
84:('Засовы сняты, самоцветы у нас. Дома больше не отрезаны от улицы.','The bolts are gone and we have the gems. The houses are no longer cut off.',
    'Не стану гнаться лишь за самоцветами: цепи тоже нужно снять.','I will not focus only on gems: the chains must come off too.',
    'Eight single-layer chains and a twenty-gem color goal, with six tile colors.'),
85:('Три пера у нас. К каждому из очагов ведёт свободная дорога.','We have all three feathers. There is a clear road to each hearth.',
    'У нижнего края лёд толще. Учту это на всех трёх дорожках.','The ice is thicker near the bottom. I will allow for that on all three paths.',
    'Three delivery feathers at columns zero, three and seven. Nine ice cells; the three at row five have two layers.'),
86:('В переулке сняты все цепи. Здесь можно пройти спокойно.','Every chain in the lane is gone. We can walk through safely.',
    'Осмотрю обе стороны переулка: цепи есть и слева, и справа.','I will check both sides of the lane; there are chains on the left and right.',
    'Four single-layer chains split between columns one and six; a sixteen-gem color goal.'),
87:('У порога больше нет завала. Можно добраться до дров.','The pile by the doorstep is cleared. We can reach the firewood.',
    'Проверю ящики на разных рядах и сколько самоцветов ещё нужно.','I will check crates on each row and how many gems we still need.',
    'Ten single-layer crates in staggered rows and a twenty-two-gem color goal. Reaching firewood is narrative, not an added inventory reward.'),
88:('Слева сняты цепи, справа разбит лёд. Оба прохода свободны.','The chains on the left and the ice on the right are gone. Both ways are clear.',
    'Справа внизу лёд в два слоя. Одной трещины недостаточно.','The ice at the lower right has two layers. One crack is not enough.',
    'Six single-layer chains on the left and six ice cells on the right. The two lowest ice cells have two layers; no delivery or color goal.'),
89:('Обходной путь расчищен, оба пера собраны. Продолжим дорогу.','The detour is clear and both feathers are gathered. We can carry on.',
    'Нижние ящики под перьями крепче остальных. Оставлю ходы на них.','The lowest crates beneath the feathers are sturdier. I will save moves for them.',
    'Eight crates and two feathers. Two lowest crates under the delivery columns have two layers; all other crates have one.'),
90:('К очагам снова можно подойти. Их свет ещё послужит деревне.','We can reach the hearths again. Their light will help the village.',
    'Проверю оба пера, лёд по бокам и цепи между ними.','I will check both feathers, the ice at the sides and the chains between them.',
    'Eight side ice cells, six central chains and two feathers, all obstacle cells single-layer. Preserve the hearth_routes milestone and next well connection.')
}
rows=[]
for n,values in copy.items():
 level=next(x for x in levels if x['levelId']==n)
 for outcome,ru,en in [('victory',values[0],values[1]),('failure',values[2],values[3])]:
  r=next(x for x in story['reactions'] if x['levelId']==n and x['outcome']==outcome)
  rows.append(dict(round=n,outcome=outcome,key=r['textKey'],speakerKey=r['speakerKey'],ru=ru,en=en,review=values[4],reviewedAgainst={k:level[k] for k in ['goals','obstacles','boardItems']}))
(base/'Reaction81To90Draft.json').write_text(json.dumps(dict(status='Reviewed against installed goals, durability, delivery positions and adjacent story scenes before installation.',entries=rows),ensure_ascii=False,indent=2)+'\n')
print('Prepared 20 bilingual reaction pairs.')
