from pathlib import Path
import re,difflib
base=Path(__file__).resolve().parent.parent;sources=base/'Sources';root=Path(__file__).resolve().parents[5]
p=sources/'AuthorAndInstall.py';s=p.read_text().replace('==150','==160').replace('orchard_entrance','orchard_paths').replace('afterLevelId=160','afterLevelId=170').replace('Chapter01Sector04R29','Chapter01Sector04R30').replace('Installed orchard entrance block','Installed orchard-row block').replace('Installed160 rounds; first150 preserved.','Installed170 rounds; first160 preserved.')
s=s.replace('Вход в сад открыт. Теперь расчистим дорожки между деревьями, а затем проверим путь к полям.','Дорожки между деревьями свободны. Теперь проверим путь из сада к полям — там ещё остались преграды.').replace('The orchard entrance is open. Now we must clear the paths between the trees, then check the way to the fields.','The paths between the trees are clear. Now we must check the way from the orchard to the fields; obstacles still remain there.')
reactions={
161:('Лёд между деревьями больше не мешает.','The ice between the trees is out of the way.','Поищу красные самоцветы возле оставшегося льда.','I will look for red gems beside the remaining ice.'),
162:('Оба пера собраны, дорожки свободны.','Both feathers are collected and the paths are clear.','Проверю ящики на обоих путях и цепи в середине.','I will check the crates on both routes and the central chains.'),
163:('В тени больше нет ледяных преград.','No icy barriers remain in the shade.','На второй полосе лёд крепче — оставлю на него ходы.','The ice in the second group is stronger; I will save moves for it.'),
164:('Перо из крайнего ряда собрано.','The feather from the outer row is collected.','Сохраню ходы для нижней цепи справа.','I will save moves for the lower chain on the right.'),
165:('Все три прохода освобождены ото льда.','All three passages are clear of ice.','Проверю нижние ледяные клетки на каждом пути.','I will check the lower ice cells on each route.'),
166:('Под яблонями стало спокойнее.','It is quieter beneath the apple trees.','Поищу рядом синие и зелёные самоцветы.','I will look for nearby blue and green gems.'),
167:('Красные и жёлтые самоцветы собраны.','The red and yellow gems are collected.','На открытом поле поищу длинную комбинацию.','I will look for a long match on the open board.'),
168:('Обход свободен от ящиков и цепей.','The way around is clear of crates and chains.','Крепкие ящики требуют ещё одного удара.','The sturdy crates need another hit.'),
169:('Проходы больше не скованы цепями.','The paths are no longer bound by chains.','Проверю нижние цепи, пока ещё есть ходы.','I will check the lower chains while moves remain.'),
170:('Дорожки сада свободны, можно идти к полям.','The orchard paths are clear; we can head toward the fields.','Освобожу три выхода и проверю отдельные ледяные клетки.','I will clear the three exits and check the separate ice cells.')}
a=s.index('reactions=');b=s.index('\n\nfor lang',a);s=s[:a]+'reactions='+repr(reactions)+s[b:];p.write_text(s)
p=sources/'CheckContent.py';s=p.read_text().replace('Chapter01Sector04R29','Chapter01Sector04R30');s=re.sub(r'\b(150|155|160|161)\b',lambda m:str(int(m[0])+10),s).replace('first150','first160').replace('after150/165','after160/165').replace('boosterMace=0,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=1','boosterMace=1,boosterFirebird=0,boosterLivingLight=0,boosterSeaPearl=0');p.write_text(s)
p=sources/'AuditContent.py';s=p.read_text();s=re.sub(r'\b(160|161|320)\b',lambda m:{'160':'170','161':'171','320':'340'}[m[0]],s).replace('==24','==25').replace('==23','==24').replace('scenes=24,milestones=23','scenes=25,milestones=24').replace('filesCovering31To160','filesCovering31To170');p.write_text(s)
p=sources/'Verify.py';p.write_text(p.read_text().replace('lor-campaign-r29','lor-campaign-r30'))
p=sources/'SummarizeWitnesses.py';s=p.read_text().replace('R27MillCheckpoint','R29OrchardEntrance');s=re.sub(r'\b(150|161|450|480)\b',lambda m:{'150':'160','161':'171','450':'480','480':'510'}[m[0]],s);p.write_text(s)
p=sources/'Summarize.py';p.write_text(p.read_text().replace('31–160','31–170'))
testroot=root/'Assets/LegendsOfRus/Tests';p=testroot/'CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text().replace('Chapter01Sector04R29','Chapter01Sector04R30').replace('Chapter01Sector03R27','Chapter01Sector04R29');lines=[]
for line in s.splitlines(True):
 if '[TestCase(1)]' in line:line=line.rstrip()+''.join(f'[TestCase({n})]' for n in range(161,171))+'\n'
 elif 'ExistingHundredFiftyRoundSave' in line:line+='        [UnityTest] public IEnumerator ExistingHundredSixtyRoundSaveKeepsBalancesAndUnlocksHundredSixtyOne() { yield return SaveMigration(160); }\n'
 elif 'state.Save.completedStoryScenes=StoryCatalog' in line:line=line.replace('(n==160?', '(n==170?"campaign.chapter01.orchard_paths":n==160?')
 elif 'if(n==' in line:
  if 'if(n==160)' in line:line+='                    if(n==170)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_paths"),Is.True);\n'
 else:
  line=re.sub(r'\b(150|151|155|158|160|161)\b',lambda m:{'158':'169'}.get(m[0],str(int(m[0])+10)),line)
  line=line.replace('orchard_entrance','orchard_paths').replace('PlaysOrchardRecovery','PlaysOrchardRowsRecovery')
 lines.append(line)
p.write_text(''.join(lines))
p=testroot/'CampaignPlayMode/CampaignBoardIntegrationTests.cs';p.write_text(p.read_text().replace('levels.Length,Is.EqualTo(160)','levels.Length,Is.EqualTo(170)'))
p=testroot/'PlayMode/LevelSystemRuntimePlayModeTests.cs';p.write_text(p.read_text().replace('OneHundredSixtyValidated','OneHundredSeventyValidated').replace('Has.Length.EqualTo(160)','Has.Length.EqualTo(170)'))
p=testroot/'CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(160,','Math.Min(170,').replace('[TestCase(160)]','[TestCase(160)][TestCase(170)]').replace('AllTwentyThree','AllTwentyFour').replace('HundredSixtySave','HundredSeventySave').replace('PrepareCompletedFixture(160,','PrepareCompletedFixture(170,').replace('milestones.Length, Is.EqualTo(23)','milestones.Length, Is.EqualTo(24)').replace('completedStoryScenes.Count, Is.EqualTo(24)','completedStoryScenes.Count, Is.EqualTo(25)').replace('Find(1, 161)','Find(1, 171)').replace('Find(1, 160)','Find(1, 170)').replace('SelectLevel(1, 161)','SelectLevel(1, 171)').replace('highestUnlockedLevel, Is.EqualTo(160)','highestUnlockedLevel, Is.EqualTo(170)');p.write_text(s)
patch=''
for old in sources.glob('*.before.cs'):
 p=next(testroot.rglob(old.name.replace('.before.cs','.cs')));patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),p.read_text().splitlines(True),fromfile=str(old),tofile=str(p)))
(sources/'TestChanges.patch').write_text(patch)
print('R30 installer/checks and four existing tests prepared.')
