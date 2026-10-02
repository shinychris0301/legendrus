from pathlib import Path
import re,difflib
base=Path(__file__).resolve().parent.parent;sources=base/'Sources';root=Path(__file__).resolve().parents[5]
p=sources/'AuthorAndInstall.py';s=p.read_text().replace('==160','==170').replace('orchard_paths','field_approach').replace('afterLevelId=170','afterLevelId=180').replace('Chapter01Sector04R30','Chapter01Sector04R31').replace('Installed orchard-row block','Installed field-approach block').replace('Installed170 rounds; first160 preserved.','Installed180 rounds; first170 preserved.')
s=s.replace('Дорожки между деревьями свободны. Теперь проверим путь из сада к полям — там ещё остались преграды.','Путь из сада к полям открыт. Теперь соединим полевые тропы с дорогами к домам, чтобы жители могли проходить без преград.').replace('The paths between the trees are clear. Now we must check the way from the orchard to the fields; obstacles still remain there.','The way from the orchard to the fields is open. Now we must connect the field paths with the roads to the homes so the villagers can pass freely.')
reactions={
171:('Цепи за садовой оградой сняты.','The chains beyond the orchard fence are cleared.','Поищу фиолетовые самоцветы рядом с цепями.','I will look for purple gems beside the chains.'),
172:('Три группы ящиков больше не мешают.','The three crate groups are out of the way.','Крепкие ящики требуют повторного удара.','The sturdy crates need another hit.'),
173:('Перья собраны, оба пути свободны.','The feathers are collected and both ways are clear.','Проверю нижний лёд на двух путях.','I will check the lower ice on both routes.'),
174:('Зелёные и жёлтые самоцветы собраны.','The green and yellow gems are collected.','Открытое поле оставляет место для длинных комбинаций.','The open board leaves room for long matches.'),
175:('Поворот свободен от цепей и ящиков.','The turn is clear of chains and crates.','Не забуду дальний путь справа.','I will remember the distant route on the right.'),
176:('Можно перевести дух перед дорогой к полю.','We can catch our breath before the field road.','Поищу красные и фиолетовые самоцветы.','I will look for red and purple gems.'),
177:('Холодная межа больше не преграждает путь.','The frozen boundary no longer blocks the way.','Внутренние ледяные клетки крепче внешних.','The inner ice cells are stronger than the outer ones.'),
178:('Дорога освобождена от ящиков и льда.','The road is clear of crates and ice.','Проверю обе преграды, пока есть ходы.','I will check both obstacle types while moves remain.'),
179:('Перо у межи собрано.','The feather by the boundary is collected.','Оставлю ходы на нижнюю цепь и боковой лёд.','I will save moves for the lower chain and side ice.'),
180:('Выход к полям свободен.','The way to the fields is clear.','Освобожу нижние ящики и проверю цепи в стороне.','I will clear the lower crates and check the chains off to the side.')}
a=s.index('reactions=');b=s.index('\n\nfor lang',a);p.write_text(s[:a]+'reactions='+repr(reactions)+s[b:])
p=sources/'CheckContent.py';s=p.read_text().replace('Chapter01Sector04R30','Chapter01Sector04R31');s=re.sub(r'\b(160|165|170|171)\b',lambda m:str(int(m[0])+10),s).replace('first160','first170').replace('after160/175','after170/175').replace('boosterMace=1,boosterFirebird=0','boosterMace=0,boosterFirebird=1');p.write_text(s)
p=sources/'AuditContent.py';s=p.read_text();s=re.sub(r'\b(170|171|340)\b',lambda m:{'170':'180','171':'181','340':'360'}[m[0]],s).replace('==25','==26').replace('==24','==25').replace('scenes=25,milestones=24','scenes=26,milestones=25').replace('filesCovering31To170','filesCovering31To180');p.write_text(s)
p=sources/'Verify.py';p.write_text(p.read_text().replace('lor-campaign-r30','lor-campaign-r31'))
p=sources/'SummarizeWitnesses.py';s=p.read_text().replace('R29OrchardEntrance','R30OrchardRows');s=re.sub(r'\b(160|171|480|510)\b',lambda m:{'160':'170','171':'181','480':'510','510':'540'}[m[0]],s);p.write_text(s)
p=sources/'Summarize.py';p.write_text(p.read_text().replace('31–170','31–180'))
testroot=root/'Assets/LegendsOfRus/Tests';p=testroot/'CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text().replace('Chapter01Sector04R30','Chapter01Sector04R31').replace('Chapter01Sector04R29','Chapter01Sector04R30');lines=[]
for line in s.splitlines(True):
 if '[TestCase(1)]' in line:line=line.rstrip()+''.join(f'[TestCase({n})]' for n in range(171,181))+'\n'
 elif 'ExistingHundredSixtyRoundSave' in line:line+='        [UnityTest] public IEnumerator ExistingHundredSeventyRoundSaveKeepsBalancesAndUnlocksHundredSeventyOne() { yield return SaveMigration(170); }\n'
 elif 'state.Save.completedStoryScenes=StoryCatalog' in line:line=line.replace('(n==170?', '(n==180?"campaign.chapter01.field_approach":n==170?')
 elif 'if(n==' in line:
  if 'if(n==170)' in line:line+='                    if(n==180)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.field_approach"),Is.True);\n'
 else:
  line=re.sub(r'\b(160|161|165|169|170|171)\b',lambda m:{'169':'178'}.get(m[0],str(int(m[0])+10)),line)
  line=line.replace('orchard_paths','field_approach').replace('PlaysOrchardRowsRecovery','PlaysFieldApproachRecovery')
 lines.append(line)
p.write_text(''.join(lines))
p=testroot/'CampaignPlayMode/CampaignBoardIntegrationTests.cs';p.write_text(p.read_text().replace('levels.Length,Is.EqualTo(170)','levels.Length,Is.EqualTo(180)'))
p=testroot/'PlayMode/LevelSystemRuntimePlayModeTests.cs';p.write_text(p.read_text().replace('OneHundredSeventyValidated','OneHundredEightyValidated').replace('Has.Length.EqualTo(170)','Has.Length.EqualTo(180)'))
p=testroot/'CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(170,','Math.Min(180,').replace('[TestCase(170)]','[TestCase(170)][TestCase(180)]').replace('AllTwentyFour','AllTwentyFive').replace('HundredSeventySave','HundredEightySave').replace('PrepareCompletedFixture(170,','PrepareCompletedFixture(180,').replace('milestones.Length, Is.EqualTo(24)','milestones.Length, Is.EqualTo(25)').replace('completedStoryScenes.Count, Is.EqualTo(25)','completedStoryScenes.Count, Is.EqualTo(26)').replace('Find(1, 171)','Find(1, 181)').replace('Find(1, 170)','Find(1, 180)').replace('SelectLevel(1, 171)','SelectLevel(1, 181)').replace('highestUnlockedLevel, Is.EqualTo(170)','highestUnlockedLevel, Is.EqualTo(180)');p.write_text(s)
patch=''
for old in sources.glob('*.before.cs'):
 p=next(testroot.rglob(old.name.replace('.before.cs','.cs')));patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),p.read_text().splitlines(True),fromfile=str(old),tofile=str(p)))
(sources/'TestChanges.patch').write_text(patch)
print('R31 authoring and test expectations prepared.')
