from pathlib import Path
import json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
levels=json.loads((active/'levels.json').read_text())['levels'];story=json.loads((active/'story.json').read_text())
copy={
71:('Самоцветов хватило. Заглянем в соседний двор.','We have enough gems. Let us look in on the next courtyard.',
    'Буду следить за обоими цветами: одного для цели недостаточно.','I will watch both target colors. Gathering just one is not enough.',
    'Two color goals of sixteen each; no obstacles or delivery items.'),
72:('Завалы по обе стороны убраны. Соседи смогут пройти.','The piles on both sides are cleared. The neighbors can get through.',
    'Проверю обе группы ящиков и счётчик нужных самоцветов.','I will check both groups of crates and the target gem counter.',
    'Ten single-layer crates in two side groups and a twenty-gem color goal.'),
73:('От толстой наледи ничего не осталось. Двор стал светлее.','The thick ice is gone. The courtyard looks brighter.',
    'Трещина во льду ещё не значит, что клетка очищена.','A crack in the ice does not mean the cell is clear yet.',
    'All six ice cells have two layers; obstacle completion requires full removal. One color goal also applies.'),
74:('Замки на тропинках сняты. Ещё один путь к соседям открыт.','The locks along the paths are gone. Another way to the neighbors is open.',
    'Цепи разбросаны по полю. Осмотрю и края, и середину.','The chains are spread across the board. I will check the edges and center.',
    'Eight single-layer chains distributed from column zero to seven; one color goal; six tile colors.'),
75:('Оба пера у нас, калитки свободны. Можно нести помощь дальше.','We have both feathers, and the gates are clear. We can carry help onward.',
    'Под перьями есть крепкие цепи. Сохраню ходы для повторных ударов.','There are sturdy chains beneath the feathers. I will save moves for more hits.',
    'Six crates, four chains and two feathers. Two lowest chains at row five have two layers.'),
76:('По краям больше нет льда. Солнечный уголок снова наш.','No ice remains at the edges. This sunny corner is ours again.',
    'Не упущу лёд у краёв поля, пока собираю нужные самоцветы.','I will watch the ice at the board edges while gathering the target gems.',
    'Four single-layer ice cells only at columns zero and seven; one color goal.'),
77:('Обе горсти самоцветов собраны. Искры осветят наш путь.','Both handfuls of gems are gathered. Their sparks will light our way.',
    'Преград нет, но самоцветов нужно много. Сверюсь с обеими целями.','There are no obstacles, but we need many gems. I will check both goals.',
    'Two color goals of twenty-four each, larger than round seventy-one; no obstacles or board items.'),
78:('Ни ящиков, ни цепей, ни льда. Вот теперь здесь можно пройти.','No crates, chains or ice remain. Now we can pass through.',
    'Здесь три вида преград. Проверю, какой ещё остался.','There are three kinds of obstacles here. I will check which remain.',
    'Four crates, four chains and four ice cells in separate groups, all single-layer. No collection or delivery goal.'),
79:('Третье перо тоже добралось вниз. Все три тропы расчищены.','The third feather reached the bottom too. All three paths are clear.',
    'Прослежу путь каждого из трёх перьев и проверю оставшийся лёд.','I will follow each of the three feathers and check the remaining ice.',
    'Three feathers in columns one, three and six; six single-layer ice cells. Avoid claiming ice is an immovable crate.'),
80:('Дворы соединены свободным проходом. Вместе пережить ночь легче.','A clear passage links the courtyards. The night is easier to face together.',
    'Проверю цепи в центре, ящики по бокам и оба пера.','I will check the central chains, the side crates and both feathers.',
    'Eight side crates, six central chains and two feathers. Preserve the existing courtyard_paths milestone and its next destination.')
}
rows=[]
for n,values in copy.items():
 level=next(x for x in levels if x['levelId']==n)
 for outcome,ru,en in [('victory',values[0],values[1]),('failure',values[2],values[3])]:
  r=next(x for x in story['reactions'] if x['levelId']==n and x['outcome']==outcome)
  rows.append(dict(round=n,outcome=outcome,key=r['textKey'],speakerKey=r['speakerKey'],ru=ru,en=en,review=values[4],reviewedAgainst={k:level[k] for k in ['goals','obstacles','boardItems']}))
(base/'Reaction71To80Draft.json').write_text(json.dumps(dict(status='Reviewed against installed goals, durability, delivery positions and adjacent story scenes before installation.',entries=rows),ensure_ascii=False,indent=2)+'\n')
print('Prepared 20 bilingual reaction pairs.')
