from pathlib import Path
import json, copy, uuid, shutil, hashlib

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
SRC = BASE / 'Sources'
read = lambda name: json.loads((SRC / (name + '.before.json')).read_text())
levels, story = read('levels'), read('story')
locales = {lang: read('story_' + lang) for lang in ('ru', 'en')}
assert len(levels['levels']) == 200 and story['completion'] is None

# Ten-round locations; individual lines state narrative progress, not new game systems.
blocks = [
 ('river_traces','Следы у реки','Tracks by the River','Тень тянется вдоль берега. Старые обереги стоят выше по тропе.','The shadow follows the bank. The old wards stand farther up the path.'),
 ('reed_path','Тропа в камышах','The Reed Path','Между камышами нашлась тропа к первому оберегу. Пойдём, пока видны следы.','A path through the reeds leads to the first ward. Let us follow the tracks while we can.'),
 ('first_ward','Первый оберег','The First Ward','Камень оберега ещё тёплый. Значит, свет не погас — ему нужно помочь.','The ward stone is still warm. Its light has not gone out; it needs our help.'),
 ('ford_locks','Затворы у брода','Locks by the Ford','Брод свободен. Теперь можно проверить обереги на другом берегу.','The ford is clear. We can now check the wards on the opposite bank.'),
 ('river_wards','Речные обереги','River Wards','Над рекой стало светлее. Но между оберегами и деревней остаются тёмные проходы.','The river looks brighter. Dark passages still separate the wards from the village.'),
 ('ward_lane','Обереговая тропа','The Ward Trail','Тропа вывела нас к старой дороге. По ней жители ходили к колодцу.','The trail meets an old road. The villagers used it to reach the well.'),
 ('well_crossing','Развилка у колодца','The Well Crossing','У колодца снова слышны голоса. Осмотрим развилку: дальний путь ещё закрыт.','Voices return to the well. Let us check the crossing; the far path is still blocked.'),
 ('hedge_gaps','Проходы в изгороди','Gaps in the Hedge','Через изгородь можно пройти к домам. Там нас ждёт ещё одна цепь оберегов.','We can pass through the hedge toward the homes. Another line of wards awaits us there.'),
 ('home_wards','Обереги у домов','Wards by the Homes','Маленькие огни у домов держатся. До следующего оберега доберёмся через двор.','The small lights beside the homes hold steady. The next ward lies beyond the yard.'),
 ('linked_paths','Соединённые тропы','Joined Paths','Колодец и дома больше не отрезаны друг от друга. След тени ведёт к склону.','The well and homes are no longer cut off from each other. The shadow leads toward the hillside.'),
 ('slope_steps','Ступени на склоне','Hillside Steps','Со склона видно, где прерывается свет. Поднимемся к старому камню.','From the slope we can see where the light breaks. Let us climb to the old stone.'),
 ('stone_approach','Подступ к камню','The Stone Approach','Под камнем сохранился узор. Он указывает на тропу вдоль гребня.','A pattern survives beneath the stone. It points to the ridge path.'),
 ('ridge_path','Тропа по гребню','The Ridge Path','Ветер разогнал туман, но не тень. Проверим оба спуска с гребня.','The wind scattered the mist, but not the shadow. We must check both descents from the ridge.'),
 ('paired_descents','Два спуска','Two Descents','Оба спуска открыты. Жар-Птица заметила слабый огонь у нижнего оберега.','Both descents are open. The Firebird spotted a faint light at the lower ward.'),
 ('hillside_ward','Оберег на склоне','The Hillside Ward','Свет на склоне окреп. До заката нужно убедиться, что деревенские дороги свободны.','The hillside light grows stronger. Before sunset we must check the village roads.'),
 ('evening_road','Вечерняя дорога','The Evening Road','Сумерки сгущаются. Начнём обход с дороги, по которой возвращаются жители.','Dusk is gathering. We will begin with the road the villagers use to return home.'),
 ('yard_watch','Дозор у двора','The Yard Watch','У двора тихо. Не оставим без внимания боковую тропу к амбару.','The yard is quiet. We must not overlook the side path to the granary.'),
 ('granary_return','Возвращение к амбару','Return to the Granary','Путь к амбару свободен. У крайних домов ещё виднеется тёмная полоса.','The granary path is clear. A dark strip still hangs beside the outer homes.'),
 ('outer_lights','Крайние огни','The Outer Lights','Огни у крайних домов не погасли. Осталось проверить дорогу к общему очагу.','The lights by the outer homes have held. We still need to check the road to the shared hearth.'),
 ('dusk_checkpoint','Обход в сумерках','The Dusk Patrol','Деревня встретила вечер с огнями. Завтра проверим последние подступы к лесной дороге.','The village meets the evening with lights. Tomorrow we will check the last approaches to the forest road.'),
 ('dawn_lane','Тропа на рассвете','The Dawn Trail','Рассвет застал нас у поворота. Отсюда начинается дорога за деревенскую ограду.','Dawn finds us at the bend. Beyond it, the road passes the village fence.'),
 ('fence_passage','Проход у ограды','The Fence Passage','У ограды стало просторно. Но на соседней тропе ещё остались преграды.','The fence passage is open. Obstacles remain on the neighboring trail.'),
 ('neighboring_trail','Соседняя тропа','The Neighboring Trail','Соседняя тропа свободна. Проверим место, где сходятся дороги от домов.','The neighboring trail is clear. We should check where the roads from the homes meet.'),
 ('road_meeting','Сход дорог','Where Roads Meet','Дороги сошлись у старого указателя. Лес совсем близко, но наш обход ещё не закончен.','The roads meet at the old signpost. The forest is close, but our patrol is not finished.'),
 ('last_approaches','Последние подступы','The Last Approaches','Подступы к дороге открыты. Прежде чем уйти, убедимся, что оставляем деревню в безопасности.','The approaches are open. Before leaving, we must make sure the village is safe.'),
 ('farewell_well','Прощание у колодца','Farewell at the Well','У колодца благодарят за помощь. Я обещал вернуться — и сдержу слово.','The people at the well thank us. I promised to return, and I will keep my word.'),
 ('orchard_morning','Утро в саду','Morning in the Orchard','В саду распустился первый цветок. Это только начало: за деревней свет всё ещё угасает.','The first flower opens in the orchard. It is only a beginning; beyond the village, the light still fades.'),
 ('gate_checks','Проверка ворот','Checking the Gate','Ворота открыты, а дороги за ними ещё незнакомы. Проверим последние обереги перед выходом.','The gate is open, but the roads beyond are unfamiliar. Let us check the last wards before we leave.'),
 ('last_ward','Последний оберег','The Last Ward','Последний оберег держит свет. Осталось освободить выход и проститься с деревней.','The last ward holds its light. We must clear the way out and say farewell to the village.'),
 ('village_departure','Дорога из деревни','The Road from the Village','Я вернусь, когда огни засияют ярче. А пока наш путь лежит в лес.','I will return when the lights shine brighter. For now, our road leads into the forest.'),
]

roles = [('Передышка','A Breather'),('Боковые преграды','Side Obstacles'),('Перо в пути','A Feather on the Way'),('Самоцветы','Gem Gathering'),('Три прохода','Three Passages'),('Тихий уголок','A Quiet Corner'),('Разделённые пути','Divided Paths'),('Сохранить проход','Keep the Passage'),('Два пера','Two Feathers'),('Проверка пути','The Path Check')]
outcomes = [
 ('Цепи сняты, самоцветы собраны.','The chains are cleared and the gems collected.'),
 ('Ящики и лёд больше не мешают.','Crates and ice no longer block the way.'),
 ('Перо найдено, преграды убраны.','The feather is found and the obstacles cleared.'),
 ('Нужные цвета собраны.','The required colors are collected.'),
 ('Все три пера прошли через преграды.','All three feathers made it past the obstacles.'),
 ('Самоцветы собраны — можно передохнуть.','The gems are collected; time for a breather.'),
 ('Цепи и лёд сняты с проходов.','The passages are free of chains and ice.'),
 ('Ящики убраны; испытание ходами пройдено.','Crates cleared; the move challenge is complete.'),
 ('Оба пера найдены, лёд и цепи сняты.','Both feathers are found; ice and chains are gone.'),
 ('Три пера собраны, путь свободен.','Three feathers collected; the way is clear.'),
]
collect = lambda t,n: dict(type='collect_tile',tileType=t,target=n)
clear = lambda t,n: dict(type='clear_obstacle',obstacleType=t,target=n)
feather = lambda n: dict(type='bring_down',itemType='firebird_feather',target=n)
briefs=[]
for block,(slug,ru,en,line_ru,line_en) in enumerate(blocks):
    sector,variation=divmod(block,5)
    for slot in range(10):
        n=201+block*10+slot
        # Six sector layouts: diagonal banks, split banks, ridge zigzags, outer patrol,
        # offset crossings and the final gate. Rotations vary their entry lanes.
        lanes=[(variation+sector)%8,(variation+sector+3)%8,(variation+sector+6)%8]
        def point(i):
            x=(i*3+variation+slot)%8
            y=1+(i+sector+slot//2)%5
            if sector==1:x=(x+4*(i%2))%8
            elif sector==2:y=1+(2*i+variation)%5
            elif sector==3:x=(i%2*6+variation+i//2)%8
            elif sector==4:y=1+(3*i+slot)%5
            elif sector==5:x=(x+2*(i//3))%8
            return x,y
        used=set();obstacles=[]
        def add(t,count,double=0,lane_gates=False):
            for i in range(count):
                xy=(lanes[i%len(lanes)],2+(i//len(lanes))*3) if lane_gates else point(i+len(obstacles))
                tries=0
                while xy in used:
                    tries+=1;xy=((xy[0]+1)%8,1+(xy[1]+(tries%2))%5)
                    assert tries<40
                used.add(xy);obstacles.append(dict(x=xy[0],y=xy[1],type=t,layers=2 if i>=count-double else 1))
        difficulty='recovery' if slot in (0,5) else 'hard' if slot in (4,9) else 'normal'
        moves=34 if difficulty=='recovery' else (30 if slot==9 else 28) if difficulty=='hard' else 29
        colors=6 if slot in (3,4,9) else 5
        t=(block+slot)%colors;other=(t+2)%colors
        items=[]
        if slot==0:
            add('chain',4);goals=[clear('chain',4),collect(t,16+variation)]
            tip=('Собирайте самоцветы рядом с цепями.','Match gems beside the chains.')
        elif slot==1:
            add('crate',6+variation%3,3);add('ice',4+sector%2)
            goals=[clear('crate',6+variation%3),clear('ice',4+sector%2)]
            tip=('Крепким ящикам нужны два удара.','Sturdy crates need two hits.')
        elif slot==2:
            add('chain',3,1);add('crate',5+sector%3)
            items=lanes[:1];goals=[clear('chain',3),clear('crate',5+sector%3),feather(1)]
            tip=('Освободите путь под пером и снимите цепи.','Open the path below the feather and clear the chains.')
        elif slot==3:
            goals=[collect(t,24+variation),collect(other,24+sector%3)]
            if sector in (2,5):goals.append(collect((t+4)%colors,18))
            tip=('Длинные совпадения помогают собрать нужные цвета.','Long matches help collect the required colors.')
        elif slot==4:
            add('crate',6,3,True);add('ice',4+variation%3)
            items=lanes;goals=[clear('crate',6),clear('ice',4+variation%3),feather(3)]
            tip=('У каждого пера свой проход; проверьте все три.','Each feather has its own passage; check all three.')
        elif slot==5:
            goals=[collect(t,16+sector),collect(other,16+variation%3)]
            tip=('Начните с цвета, которого на поле больше.','Start with the color that is more plentiful on the board.')
        elif slot==6:
            add('chain',6,3+variation%3);add('ice',5+sector%2)
            goals=[clear('chain',6),clear('ice',5+sector%2)]
            tip=('Снимите оба слоя крепких цепей.','Remove both layers of the stronger chains.')
        elif slot==7:
            add('crate',6+sector%3,3)
            goals=[dict(type='survival',target=18+variation%3),clear('crate',6+sector%3)]
            tip=('Нужны потраченные ходы и все ящики; ожидание не считается.','Use the required moves and clear every crate; waiting does not count.')
        elif slot==8:
            add('ice',4,2);add('chain',4+variation%2)
            items=[lanes[0],lanes[2]];goals=[clear('ice',4),clear('chain',4+variation%2),feather(2)]
            tip=('Проверьте лёд и выходы под обоими перьями.','Check the ice and the exits beneath both feathers.')
        else:
            add('chain',6,3,True);add('ice',6+variation%3)
            items=lanes;goals=[clear('chain',6),clear('ice',6+variation%3),feather(3)]
            tip=('Разделите ходы между тремя перьями и оставшимся льдом.','Share your moves between the three feathers and the remaining ice.')
        reward=dict(gold=160 if difficulty=='recovery' else 220 if difficulty=='hard' else 180,gems=3 if difficulty=='hard' else 0,boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0)
        if slot==9:reward[['boosterMace','boosterFirebird','boosterLivingLight','boosterSeaPearl'][block%4]]=1
        level=dict(id=f'chapter01.round{n:03}',chapterId=1,levelId=n,displayName=ru+' — '+roles[slot][0].lower(),columns=8,rows=7,tileTypes=colors,moveLimit=moves,startingScore=0,boardSeed=20000+n,difficulty=difficulty,difficultyRating={'recovery':2,'normal':4,'hard':6}[difficulty],goals=goals,fixedCells=[],obstacles=obstacles,boardItems=[dict(x=x,y=0,type='firebird_feather') for x in items],rewards=reward,starScoreThresholds=[800,1800,3200],storyTriggers=[],tutorialSteps=[])
        levels['levels'].append(level)
        briefs.append(dict(round=n,sector=5+sector,nameRu=level['displayName'],nameEn=en+' — '+roles[slot][1],intent=roles[slot][1]+' / '+en,level=copy.deepcopy(level),hintRu=tip[0],hintEn=tip[1]))
        for lang,data in locales.items():
            value=level['displayName'] if lang=='ru' else en+' — '+roles[slot][1]
            data['entries'].append(dict(key=f'campaign.round{n:03}.name',value=value))
            for outcome in ('victory','failure'):
                key=f'reaction.campaign.chapter01.round{n:03}.{outcome}'
                text=(ru if lang=='ru' else en)+': '+(outcomes[slot][lang=='en'] if outcome=='victory' else tip[lang=='en'])
                data['entries'].append(dict(key=key,value=text))
                if lang=='ru':story['reactions'].append(dict(id=key,levelId=n,outcome=outcome,speakerKey='speaker.ivan',textKey=key))
    scene='campaign.chapter01.'+slug
    last=levels['levels'][-1]
    last['storyTriggers']=['dialogue.'+scene]
    lines=[('speaker.ivan',line_ru,line_en)]
    if block==29:
        lines=[('speaker.ivan','Дороги свободны, и у домов снова горят огни. Деревня может перевести дух.','The roads are clear, and lights burn beside the homes again. The village can breathe.'),
               ('speaker.firebird','Ты помог этому месту. Но дальше свет всё ещё угасает — источник беды не здесь.','You have helped this place. Farther away, the light still fades; its source is not here.'),
               ('speaker.dobrynya','Не забудь дорогу домой, Иван. Мы присмотрим за оберегами.','Remember the road home, Ivan. We will watch over the wards.'),
               ('speaker.ivan',line_ru,line_en),
               ('speaker.firebird','Тогда идём. Вместе мы найдём, как вернуть свет Руси.','Then let us go. Together we will find how to restore the light of Rus.')]
        last['storyTriggers'].append('chapter.complete')
        story['completion']=dict(finalLevelId=500,finaleSceneId=scene,completionTrigger='chapter.complete',summaryTitleKey='campaign.chapter01.complete.title',summaryFormatKey='campaign.chapter01.complete.summary',rewardNoteKey='campaign.chapter01.complete.rewards')
    sl=[]
    for i,(speaker,ru_text,en_text) in enumerate(lines,1):
        key=f'{scene}.{i:02}';sl.append(dict(speakerKey=speaker,textKey=key))
        for lang,data in locales.items():data['entries'].append(dict(key=key,value=ru_text if lang=='ru' else en_text))
    story['scenes'].append(dict(id=scene,trigger='dialogue.'+scene,returnRoute='ChapterComplete' if block==29 else 'ChapterMap',lines=sl))
    story['milestones'].append(dict(id=scene+'.milestone',afterLevelId=last['levelId'],sceneId=scene,major=True))
    briefs[-1]['level']=copy.deepcopy(last)

for lang,data in locales.items():
    for suffix,text in {
        'title':'Родная деревня — глава завершена' if lang=='ru' else 'Native Village — Chapter Complete',
        'summary':'Пройдено {0}/{1} уровней. Собрано {2}/{3} звёзд.' if lang=='ru' else 'Completed {0}/{1} levels. Earned {2}/{3} stars.',
        'rewards':'Награды уже получены за уровни. Дополнительного бонуса нет.' if lang=='ru' else 'Level rewards have already been received. No additional bonus.'
    }.items():data['entries'].append(dict(key='campaign.chapter01.complete.'+suffix,value=text))
    keys=[e['key'] for e in data['entries']];assert len(keys)==len(set(keys))
    assert 'speaker.firebird' in keys

assert levels['levels'][:200]==read('levels')['levels']
shapes=[]
for b in briefs:
    l=b['level'];pos=[(o['x'],o['y']) for o in l['obstacles']]
    assert len(pos)==len(set(pos)) and all(0<=x<8 and 1<=y<=5 for x,y in pos)
    assert len(l['goals'])<=3
    for goal in l['goals']:
        if goal['type']=='clear_obstacle':assert goal['target']==sum(o['type']==goal['obstacleType'] for o in l['obstacles'])
        if goal['type']=='bring_down':assert goal['target']==len(l['boardItems'])
    shapes.append(json.dumps({k:l[k] for k in ['goals','obstacles','boardItems','tileTypes']},sort_keys=True))
(BASE/'RoundBriefs.json').write_text(json.dumps({'status':'Authored full Chapter 1 candidate; difficulty is provisional pending human playtests','rows':briefs},ensure_ascii=False,indent=2)+'\n')
out=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01CompleteR44';out.mkdir(exist_ok=True)
for name,data in [('levels',levels),('story',story),*[(f'story_{lang}',data) for lang,data in locales.items()]]:
    p=out/(name+'.json');p.write_text(json.dumps(data,ensure_ascii=False,indent=2)+'\n');shutil.copyfile(p,ROOT/'Assets/LegendsOfRus/Resources/CampaignActive'/p.name)
for p in [out,*out.glob('*.json')]:
    meta=Path(str(p)+'.meta')
    if not meta.exists():meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\n'+('folderAsset: yes\nDefaultImporter:\n' if p.is_dir() else 'TextScriptImporter:\n')+'  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
print(json.dumps({'installed':len(levels['levels']),'newBriefs':len(briefs),'distinctNewPuzzleConfigurations':len(set(shapes)),'scenes':len(story['scenes']),'milestones':len(story['milestones']),'reactions':len(story['reactions']),'localeEntries':len(locales['ru']['entries'])}))
