from pathlib import Path
import re,difflib
base=Path(__file__).resolve().parent.parent;sources=base/'Sources';root=Path(__file__).resolve().parents[5]
p=sources/'AuthorAndInstall.py';s=p.read_text().replace('==170','==180').replace('field_approach','field_home_paths').replace('afterLevelId=180','afterLevelId=190').replace('Chapter01Sector04R31','Chapter01Sector04R32').replace('Installed field-approach block','Installed field-to-home block').replace('Installed180 rounds; first170 preserved.','Installed190 rounds; first180 preserved.')
s=s.replace('Путь из сада к полям открыт. Теперь соединим полевые тропы с дорогами к домам, чтобы жители могли проходить без преград.','Полевые тропы соединены с дорогами к домам. Осталось проверить обходные пути у сада и полей, чтобы ни один проход не остался закрытым.').replace('The way from the orchard to the fields is open. Now we must connect the field paths with the roads to the homes so the villagers can pass freely.','The field paths now connect with the roads to the homes. We still need to check the bypasses by the orchard and fields so no passage remains blocked.')
reactions={
181:('Лёд на тропе к домам больше не мешает.','The ice on the path to the homes is out of the way.','Поищу жёлтые самоцветы рядом со льдом.','I will look for yellow gems beside the ice.'),
182:('Обе дороги освобождены от преград.','Both roads are clear of obstacles.','Слева ящики крепче; справа ещё остался лёд.','The crates on the left are stronger; ice still remains on the right.'),
183:('Перья между дорогами собраны.','The feathers between the roads are collected.','Проверю нижние цепи и лёд по краям.','I will check the lower chains and the ice at the edges.'),
184:('Красные и синие самоцветы собраны.','The red and blue gems are collected.','На открытой поляне есть место для длинной комбинации.','The open clearing has room for a long match.'),
185:('Три дороги к домам свободны.','The three roads to the homes are clear.','Оставлю ходы на крепкие нижние ящики.','I will save moves for the sturdy lower crates.'),
186:('На тихой дороге можно перевести дух.','We can catch our breath on the quiet road.','Поищу синие и фиолетовые самоцветы поблизости.','I will look for blue and purple gems nearby.'),
187:('Цепи у развилки сняты.','The chains at the fork are cleared.','Проверю крепкие цепи по диагонали.','I will check the stronger diagonal chains.'),
188:('Лёд больше не разделяет дороги.','The ice no longer separates the roads.','Проверю клетки с оставшимся вторым слоем льда.','I will check the cells with a second layer of ice still remaining.'),
189:('Оба боковых выхода свободны, перья собраны.','Both side exits are clear and the feathers collected.','Проверю нижний лёд на каждой стороне.','I will check the lower ice on each side.'),
190:('Полевые тропы снова ведут к домам.','The field paths lead to the homes again.','Освобожу нижние цепи на трёх путях и проверю ящики.','I will clear the lower chains on the three routes and check the crates.')}
a=s.index('reactions=');b=s.index('\n\nfor lang',a);p.write_text(s[:a]+'reactions='+repr(reactions)+s[b:])
p=sources/'CheckContent.py';s=p.read_text().replace('Chapter01Sector04R31','Chapter01Sector04R32');s=re.sub(r'\b(170|175|180|181)\b',lambda m:str(int(m[0])+10),s).replace('first170','first180').replace('after170/185','after180/185').replace('boosterFirebird=1,boosterLivingLight=0','boosterFirebird=0,boosterLivingLight=1');p.write_text(s)
p=sources/'AuditContent.py';s=p.read_text();s=re.sub(r'\b(180|181|360)\b',lambda m:{'180':'190','181':'191','360':'380'}[m[0]],s).replace('==26','==27').replace('==25','==26').replace('scenes=26,milestones=25','scenes=27,milestones=26').replace('filesCovering31To180','filesCovering31To190');p.write_text(s)
p=sources/'Verify.py';p.write_text(p.read_text().replace('lor-campaign-r31','lor-campaign-r32'))
p=sources/'SummarizeWitnesses.py';s=p.read_text().replace('R30OrchardRows','R31FieldApproach');s=re.sub(r'\b(170|181|510|540)\b',lambda m:{'170':'180','181':'191','510':'540','540':'570'}[m[0]],s);p.write_text(s)
p=sources/'Summarize.py';p.write_text(p.read_text().replace('31–180','31–190'))
testroot=root/'Assets/LegendsOfRus/Tests';p=testroot/'CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text().replace('Chapter01Sector04R31','Chapter01Sector04R32').replace('Chapter01Sector04R30','Chapter01Sector04R31');lines=[]
for line in s.splitlines(True):
 if '[TestCase(1)]' in line:line=line.rstrip()+''.join(f'[TestCase({n})]' for n in range(181,191))+'\n'
 elif 'ExistingHundredSeventyRoundSave' in line:line+='        [UnityTest] public IEnumerator ExistingHundredEightyRoundSaveKeepsBalancesAndUnlocksHundredEightyOne() { yield return SaveMigration(180); }\n'
 elif 'state.Save.completedStoryScenes=StoryCatalog' in line:line=line.replace('(n==180?', '(n==190?"campaign.chapter01.field_home_paths":n==180?')
 elif 'if(n==' in line:
  if 'if(n==180)' in line:line+='                    if(n==190)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.field_home_paths"),Is.True);\n'
 else:
  line=re.sub(r'\b(170|171|175|178|180|181)\b',lambda m:str(int(m[0])+10),line)
  line=line.replace('field_approach','field_home_paths').replace('PlaysFieldApproachRecovery','PlaysFieldHomeRecovery')
 lines.append(line)
p.write_text(''.join(lines))
p=testroot/'CampaignPlayMode/CampaignBoardIntegrationTests.cs';p.write_text(p.read_text().replace('levels.Length,Is.EqualTo(180)','levels.Length,Is.EqualTo(190)'))
p=testroot/'PlayMode/LevelSystemRuntimePlayModeTests.cs';p.write_text(p.read_text().replace('OneHundredEightyValidated','OneHundredNinetyValidated').replace('Has.Length.EqualTo(180)','Has.Length.EqualTo(190)'))
p=testroot/'CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(180,','Math.Min(190,').replace('[TestCase(180)]','[TestCase(180)][TestCase(190)]').replace('AllTwentyFive','AllTwentySix').replace('HundredEightySave','HundredNinetySave').replace('PrepareCompletedFixture(180,','PrepareCompletedFixture(190,').replace('milestones.Length, Is.EqualTo(25)','milestones.Length, Is.EqualTo(26)').replace('completedStoryScenes.Count, Is.EqualTo(26)','completedStoryScenes.Count, Is.EqualTo(27)').replace('Find(1, 181)','Find(1, 191)').replace('Find(1, 180)','Find(1, 190)').replace('SelectLevel(1, 181)','SelectLevel(1, 191)').replace('highestUnlockedLevel, Is.EqualTo(180)','highestUnlockedLevel, Is.EqualTo(190)');p.write_text(s)
patch=''
for old in sources.glob('*.before.cs'):
 p=next(testroot.rglob(old.name.replace('.before.cs','.cs')));patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),p.read_text().splitlines(True),fromfile=str(old),tofile=str(p)))
(sources/'TestChanges.patch').write_text(patch)
print('R32 authoring and test expectations prepared.')
