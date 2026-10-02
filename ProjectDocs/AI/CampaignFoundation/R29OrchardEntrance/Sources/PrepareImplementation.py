from pathlib import Path
import re
base=Path('ProjectDocs/AI/CampaignFoundation/R29OrchardEntrance');sources=base/'Sources'
p=sources/'AuthorAndInstall.py';s=p.read_text().replace('==140','==150').replace('mill_routes_secured','orchard_entrance').replace('afterLevelId=150','afterLevelId=160').replace('Chapter01Sector03R27','Chapter01Sector04R29').replace('Installed mill checkpoint block','Installed orchard entrance block').replace('Installed150 rounds; first140 preserved.','Installed160 rounds; first150 preserved.')
s=s.replace('Дороги между мельницей, складом и домами свободны. Теперь проверим тропы к саду и полям: деревне ещё нужна наша помощь.','Вход в сад открыт. Теперь расчистим дорожки между деревьями, а затем проверим путь к полям.').replace('The roads between the mill, storehouse and homes are clear. Now we must check the paths to the orchard and fields: the village still needs our help.','The orchard entrance is open. Now we must clear the paths between the trees, then check the way to the fields.')
reactions={
151:('Лёд на садовой тропе растаял.','The ice on the orchard path has melted.','Поищу зелёные самоцветы рядом с оставшимся льдом.','I will look for green gems beside the remaining ice.'),
152:('Оба входа свободны от ящиков.','Both entrances are clear of crates.','В середине есть место для длинной комбинации.','There is room in the centre for a long match.'),
153:('Перья у калитки собраны.','The feathers by the gate are collected.','Нижние цепи крепче — проверю оба прохода.','The lower chains are stronger; I will check both passages.'),
154:('Самоцветы сияют сквозь ветви.','The gems shine through the branches.','Поищу комбинацию из синих или фиолетовых самоцветов.','I will look for a match of blue or purple gems.'),
155:('Все три пути для перьев расчищены.','All three feather routes are clear.','Оставлю ходы на крепкие ящики внизу.','I will save moves for the sturdy lower crates.'),
156:('В тихом уголке можно перевести дух.','We can catch our breath in the quiet corner.','Красные и зелёные самоцветы помогут закончить сбор.','Red and green gems will help finish the collection.'),
157:('Цепи больше не преграждают путь.','The chains no longer block the path.','Начну с крепких цепей ближе к середине.','I will start with the stronger chains near the centre.'),
158:('Проход свободен от ледяных преград.','The passage is clear of icy barriers.','Проверю клетки, где ещё остался второй слой льда.','I will check the cells that still have a second layer of ice.'),
159:('Боковая дорожка свободна, перья собраны.','The side path is clear and the feathers are collected.','Проверю цепи в стороне от путей перьев.','I will check the chains away from the feather routes.'),
160:('Вход в сад открыт для жителей.','The orchard entrance is open to the villagers.','Сначала освобожу нижние цепи на трёх путях.','I will clear the lower chains on the three routes first.')}
a=s.index('reactions={');b=s.index('\n\nfor lang',a);s=s[:a]+'reactions='+repr(reactions)+s[b:];p.write_text(s)
p=sources/'CheckContent.py';s=p.read_text();s=s.replace('Chapter01Sector03R27','Chapter01Sector04R29');s=re.sub(r'\b(140|145|150|151)\b',lambda m:str(int(m[0])+10),s).replace('first140','first150').replace('after140/145','after150/155').replace('boosterLivingLight=1,boosterSeaPearl=0','boosterLivingLight=0,boosterSeaPearl=1');p.write_text(s)
p=sources/'AuditContent.py';s=p.read_text();s=re.sub(r'\b(150|151|300|337)\b',lambda m:{'150':'160','151':'161','300':'320','337':'358'}[m[0]],s);s=s.replace('==23','==24').replace('==22','==23').replace('scenes=23,milestones=22','scenes=24,milestones=23').replace('for start in [1,51,101]:','for start in [1,51,101,151]:').replace('last=start+49','last=min(start+49,len(levels))').replace('filesCovering31To150','filesCovering31To160');p.write_text(s)
p=sources/'Verify.py';p.write_text(p.read_text().replace('lor-campaign-r27','lor-campaign-r29'))
p=sources/'SummarizeWitnesses.py';s=p.read_text().replace('R26ProvisionPaths','R27MillCheckpoint');s=re.sub(r'\b(140|151|420|450)\b',lambda m:{'140':'150','151':'161','420':'450','450':'480'}[m[0]],s);p.write_text(s)
p=sources/'Summarize.py';p.write_text(p.read_text().replace('31–150','31–160'))
# Four existing tests: update catalog boundary, preserve earlier cases, add the new milestone/migration.
testroot=Path('Assets/LegendsOfRus/Tests');p=testroot/'CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text()
s=s.replace('Chapter01Sector03R27','Chapter01Sector04R29').replace('Chapter01Sector03R26','Chapter01Sector03R27')
# Update expectations outside the historical migration and per-round test case declarations.
lines=[]
for line in s.splitlines(True):
 if '[TestCase(1)]' in line:line=line.rstrip()+''.join(f'[TestCase({n})]' for n in range(151,161))+'\n'
 elif 'ExistingHundredFortyRoundSave' in line:line+='        [UnityTest] public IEnumerator ExistingHundredFiftyRoundSaveKeepsBalancesAndUnlocksHundredFiftyOne() { yield return SaveMigration(150); }\n'
 elif 'state.Save.completedStoryScenes=StoryCatalog' in line:line=line.replace('(n==150?', '(n==160?"campaign.chapter01.orchard_entrance":n==150?')
 elif 'if(n==' in line: # keep existing checkpoint acknowledgements and add160
  if 'if(n==150)' in line:line+='                    if(n==160)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_entrance"),Is.True);\n'
 else:
  line=re.sub(r'\b(140|141|145|148|150|151)\b',lambda m:str(int(m[0])+10),line)
  line=line.replace('mill_routes_secured','orchard_entrance').replace('PlaysMillRecovery','PlaysOrchardRecovery')
 lines.append(line)
p.write_text(''.join(lines))
p=testroot/'CampaignPlayMode/CampaignBoardIntegrationTests.cs';p.write_text(p.read_text().replace('levels.Length,Is.EqualTo(150)','levels.Length,Is.EqualTo(160)'))
p=testroot/'PlayMode/LevelSystemRuntimePlayModeTests.cs';p.write_text(p.read_text().replace('OneHundredFiftyValidated','OneHundredSixtyValidated').replace('Has.Length.EqualTo(150)','Has.Length.EqualTo(160)'))
p=testroot/'CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(150,','Math.Min(160,').replace('[TestCase(150)]','[TestCase(150)][TestCase(160)]').replace('AllTwentyTwo','AllTwentyThree').replace('HundredFiftySave','HundredSixtySave').replace('PrepareCompletedFixture(150,','PrepareCompletedFixture(160,').replace('milestones.Length, Is.EqualTo(22)','milestones.Length, Is.EqualTo(23)').replace('completedStoryScenes.Count, Is.EqualTo(23)','completedStoryScenes.Count, Is.EqualTo(24)').replace('Find(1, 151)','Find(1, 161)').replace('Find(1, 150)','Find(1, 160)').replace('SelectLevel(1, 151)','SelectLevel(1, 161)').replace('highestUnlockedLevel, Is.EqualTo(150)','highestUnlockedLevel, Is.EqualTo(160)');p.write_text(s)
print('R29 installer, checks and four test expectations prepared.')
