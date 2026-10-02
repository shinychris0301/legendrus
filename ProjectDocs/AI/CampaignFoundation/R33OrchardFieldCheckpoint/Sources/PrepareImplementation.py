from pathlib import Path
import re,difflib
base=Path(__file__).resolve().parent.parent;sources=base/'Sources';root=Path(__file__).resolve().parents[5]
p=sources/'AuthorAndInstall.py';s=p.read_text().replace('==180','==190').replace('field_home_paths','orchard_field_paths').replace('afterLevelId=190','afterLevelId=200').replace('Chapter01Sector04R32','Chapter01Sector04R33').replace('Installed field-to-home block','Installed orchard/field checkpoint block').replace('Installed190 rounds; first180 preserved.','Installed200 rounds; first190 preserved.')
s=s.replace('Полевые тропы соединены с дорогами к домам. Осталось проверить обходные пути у сада и полей, чтобы ни один проход не остался закрытым.','Дороги у сада и полей свободны, но у реки всё ещё держится тень. Проверим тропу к старым оберегам деревни.').replace('The field paths now connect with the roads to the homes. We still need to check the bypasses by the orchard and fields so no passage remains blocked.','The roads by the orchard and fields are clear, but a shadow still lingers by the river. We must check the path to the village\'s old wards.')
# Use JSON-compatible double-quoted source for the scene string to preserve the English apostrophe.
s=s.replace("else 'The roads by the orchard and fields are clear, but a shadow still lingers by the river. We must check the path to the village's old wards.'", 'else "The roads by the orchard and fields are clear, but a shadow still lingers by the river. We must check the path to the village\'s old wards."')
reactions={
191:('Цепи на обходной тропе сняты.','The chains on the bypass path are cleared.','Поищу синие самоцветы рядом с оставшимися цепями.','I will look for blue gems beside the remaining chains.'),
192:('Оба обхода свободны от ящиков и льда.','Both bypasses are clear of crates and ice.','Нижние группы ящиков крепче верхних.','The lower crate groups are stronger than the upper ones.'),
193:('Перо у поворота собрано.','The feather by the bend is collected.','Сохраню ходы для нижней цепи и боковых ящиков.','I will save moves for the lower chain and side crates.'),
194:('Фиолетовые и жёлтые самоцветы собраны.','The purple and yellow gems are collected.','На открытом поле поищу длинную комбинацию.','I will look for a long match on the open board.'),
195:('Три обходных пути открыты.','The three bypass routes are open.','Проверю крепкие ящики внизу каждого пути.','I will check the sturdy crates at the bottom of each route.'),
196:('У сада можно ненадолго отдохнуть.','We can rest briefly by the orchard.','Поищу красные и зелёные самоцветы поблизости.','I will look for red and green gems nearby.'),
197:('Крайние проходы больше не скованы цепями.','The outer passages are no longer bound by chains.','На краях цепи крепче — оставлю на них ходы.','The outer chains are stronger; I will save moves for them.'),
198:('Обходы освобождены от ящиков.','The bypasses are clear of crates.','Проверю ящики, которым нужен ещё один удар.','I will check the crates that need another hit.'),
199:('Перья с обеих троп собраны.','The feathers from both paths are collected.','Нижний лёд на двух путях ещё может задерживать перья.','The lower ice on both routes may still hold up the feathers.'),
200:('Сад и поля связаны свободными дорогами.','Clear roads connect the orchard and fields.','Освобожу нижние цепи и проверю оставшийся лёд.','I will clear the lower chains and check the remaining ice.')}
a=s.index('reactions=');b=s.index('\n\nfor lang',a);p.write_text(s[:a]+'reactions='+repr(reactions)+s[b:])
p=sources/'CheckContent.py';s=p.read_text().replace('Chapter01Sector04R32','Chapter01Sector04R33');s=re.sub(r'\b(180|185|190|191)\b',lambda m:str(int(m[0])+10),s).replace('first180','first190').replace('after180/195','after190/195').replace('boosterLivingLight=1,boosterSeaPearl=0','boosterLivingLight=0,boosterSeaPearl=1');p.write_text(s)
p=sources/'AuditContent.py';s=p.read_text();s=re.sub(r'\b(190|191|380)\b',lambda m:{'190':'200','191':'201','380':'400'}[m[0]],s).replace('==27','==28').replace('==26','==27').replace('scenes=27,milestones=26','scenes=28,milestones=27').replace('filesCovering31To190','filesCovering31To200');p.write_text(s)
p=sources/'Verify.py';p.write_text(p.read_text().replace('lor-campaign-r32','lor-campaign-r33'))
p=sources/'SummarizeWitnesses.py';s=p.read_text().replace('R31FieldApproach','R32FieldHomePaths');s=re.sub(r'\b(180|191|540|570)\b',lambda m:{'180':'190','191':'201','540':'570','570':'600'}[m[0]],s);p.write_text(s)
p=sources/'Summarize.py';p.write_text(p.read_text().replace('31–190','31–200'))
testroot=root/'Assets/LegendsOfRus/Tests';p=testroot/'CampaignPlayMode/ActiveCampaignIntegrationTests.cs';s=p.read_text().replace('Chapter01Sector04R32','Chapter01Sector04R33').replace('Chapter01Sector04R31','Chapter01Sector04R32');lines=[]
for line in s.splitlines(True):
 if '[TestCase(1)]' in line:line=line.rstrip()+''.join(f'[TestCase({n})]' for n in range(191,201))+'\n'
 elif 'ExistingHundredEightyRoundSave' in line:line+='        [UnityTest] public IEnumerator ExistingHundredNinetyRoundSaveKeepsBalancesAndUnlocksHundredNinetyOne() { yield return SaveMigration(190); }\n'
 elif 'state.Save.completedStoryScenes=StoryCatalog' in line:line=line.replace('(n==190?', '(n==200?"campaign.chapter01.orchard_field_paths":n==190?')
 elif 'if(n==' in line:
  if 'if(n==190)' in line:line+='                    if(n==200)Assert.That(state.HasCompletedStoryScene("campaign.chapter01.orchard_field_paths"),Is.True);\n'
 else:
  line=re.sub(r'\b(180|181|185|188|190|191)\b',lambda m:str(int(m[0])+10),line)
  line=line.replace('field_home_paths','orchard_field_paths').replace('PlaysFieldHomeRecovery','PlaysOrchardFieldRecovery')
 lines.append(line)
p.write_text(''.join(lines))
p=testroot/'CampaignPlayMode/CampaignBoardIntegrationTests.cs';p.write_text(p.read_text().replace('levels.Length,Is.EqualTo(190)','levels.Length,Is.EqualTo(200)'))
p=testroot/'PlayMode/LevelSystemRuntimePlayModeTests.cs';p.write_text(p.read_text().replace('OneHundredNinetyValidated','TwoHundredValidated').replace('Has.Length.EqualTo(190)','Has.Length.EqualTo(200)'))
p=testroot/'CampaignPlayMode/CampaignCheckpointLifecycleTests.cs';s=p.read_text().replace('Math.Min(190,','Math.Min(200,').replace('[TestCase(190)]','[TestCase(190)][TestCase(200)]').replace('AllTwentySix','AllTwentySeven').replace('HundredNinetySave','TwoHundredSave').replace('PrepareCompletedFixture(190,','PrepareCompletedFixture(200,').replace('milestones.Length, Is.EqualTo(26)','milestones.Length, Is.EqualTo(27)').replace('completedStoryScenes.Count, Is.EqualTo(27)','completedStoryScenes.Count, Is.EqualTo(28)').replace('Find(1, 191)','Find(1, 201)').replace('Find(1, 190)','Find(1, 200)').replace('SelectLevel(1, 191)','SelectLevel(1, 201)').replace('highestUnlockedLevel, Is.EqualTo(190)','highestUnlockedLevel, Is.EqualTo(200)');p.write_text(s)
patch=''
for old in sources.glob('*.before.cs'):
 p=next(testroot.rglob(old.name.replace('.before.cs','.cs')));patch+=''.join(difflib.unified_diff(old.read_text().splitlines(True),p.read_text().splitlines(True),fromfile=str(old),tofile=str(p)))
(sources/'TestChanges.patch').write_text(patch)
print('R33 authoring and test expectations prepared.')
