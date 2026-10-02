from pathlib import Path
import json, hashlib, zipfile, xml.etree.ElementTree as ET, html
root=Path.cwd();base=root/'ProjectDocs/AI/CampaignFoundation/R7'
def write(p,obj):p.write_text(json.dumps(obj,ensure_ascii=False,indent=2)+'\n')
def digest(p):return hashlib.sha256(p.read_bytes()).hexdigest()
assets={str(p.relative_to(root)):digest(p) for d in ['Assets','Packages','ProjectSettings'] for p in (root/d).rglob('*') if p.is_file()}
if not (base/'Sources/BeforeHashes.json').exists():write(base/'Sources/BeforeHashes.json',assets)
paths=['Assets/LegendsOfRus/Resources/Config/chapter01_levels.json','Assets/LegendsOfRus/Resources/Config/chapter01_story.json','Assets/LegendsOfRus/Resources/Localization/story_ru.json','Assets/LegendsOfRus/Resources/Localization/story_en.json','ProjectDocs/ContentPipeline/CampaignV2/campaign_definition.json','ProjectDocs/SourceDocuments/Match3_Full_Story_and_Scenario.docx','Assets/LegendsOfRus/Scripts/GameplayCore/BoardMechanicPrimitives.cs','Assets/LegendsOfRus/Scripts/GameplayCore/BoardResolutionPlanner.cs']
write(base/'Sources/Inputs.json',[{'path':p,'sha256':digest(root/p)} for p in paths])
with zipfile.ZipFile(root/paths[5]) as z:
 doc=ET.fromstring(z.read('word/document.xml'));ns={'w':'http://schemas.openxmlformats.org/wordprocessingml/2006/main'}
 paragraphs=[''.join(p.itertext()) for p in []] # Extract text nodes only, not document metadata.
 paragraphs=[''.join(t.text or '' for t in p.findall('.//w:t',ns)) for p in doc.findall('.//w:p',ns)]
 start=paragraphs.index('Chapter 1 — Родная Деревня (The Native Village)');end=paragraphs.index('Chapter 2 — Густой Лес (The Dense Forest)',start)
 (base/'Sources/Chapter1Scenario.txt').write_text('\n'.join(paragraphs[start:end])+'\n')
levels=json.loads((root/paths[0]).read_text())['levels'];story=json.loads((root/paths[1]).read_text())
ru={e['key']:e['value'] for e in json.loads((root/paths[2]).read_text())['entries']};en={e['key']:e['value'] for e in json.loads((root/paths[3]).read_text())['entries']}
write(base/'Sources/LegacyLevels.json',levels);write(base/'Sources/LegacyStory.json',story)
# The following are hand-authored briefs, not runtime level JSON or generated boards.
# round|RU|EN|band|moves|colors|goals shorthand|layout / authoring intent
briefs='''11|Тёплый пепел|Warm Ash|recovery|32|5|red:14,blue:14|Open board; cascades without blockers after the legacy climax.
12|Искры над садом|Sparks over the Orchard|teach|30|5|red:18,score:1400|Author one optional match-four opportunity; do not require inventory use.
13|Ряды света|Rows of Light|normal|28|5|ice:6,blue:18|Two separated ice clusters reward a horizontal special.
14|Столбы тепла|Columns of Warmth|normal|29|5|chain:6,red:18|Two vertical chain strips; leave a free center lane.
15|Замёрзший колодец|The Frozen Well|hard|27|6|ice:10,blue:22|Two-layer ice in a central basin; preserve routes on both sides.
16|Вода для очага|Water for the Hearth|recovery|32|5|ice:4,blue:14|Single-layer corner ice with open cascade space.
17|Освободить ворот|Free the Windlass|normal|29|5|chain:8,red:18|Split chain pockets; no completely sealed start.
18|Отблеск в воде|Reflection in the Water|teach|30|5|crate:6,score:1800|Optional L/T special opportunity near crates; teach Burst rather than invent a new booster.
19|Вёдра у колодца|Buckets by the Well|normal|29|5|crate:8,blue:18|Two crate islands with exposed adjacent match lanes.
20|Первый чистый ручей|The First Clear Stream|hard|27|6|ice:8,chain:6,blue:22|Separate ice/chain cells; complete the well episode using existing rules.
21|Передышка у ворот|Rest by the Gate|recovery|33|5|red:16,green:16|Open field and generous moves; no new tutorial.
22|Проход к амбару|Passage to the Granary|normal|29|5|crate:8,green:18|Staggered crate steps with accessible sides.
23|Две корзины|Two Baskets|normal|30|5|feather:2,chain:4|Two feather columns with free bottom exits.
24|Свет пяти искр|Light of Five Sparks|teach|31|5|red:20,score:2000|Optional straight-five opportunity for Color special; all goals possible through ordinary matches.
25|Засов амбара|The Granary Bar|hard|26|6|crate:12,green:22|Central crate block; include at least three accessible faces.
26|Запах тёплого хлеба|The Scent of Warm Bread|recovery|33|5|crate:4,red:16|Four one-layer crates with broad empty match space.
27|Перья под крышей|Feathers under the Roof|normal|29|5|feather:2,crate:6|Offset feather lanes; crates never sit on final exit cells.
28|Окна амбара|Granary Windows|normal|28|5|ice:8,green:20|Two-layer side patches; use center to prepare specials.
29|Оковы старых дверей|Chains on Old Doors|normal|28|6|chain:10,red:20|Alternating chain strips and open lanes.
30|Запасы для деревни|Supplies for the Village|hard|27|6|crate:10,feather:2,green:22|Combine crate access and feather descent; close granary episode.
31|Тропа после дождя|The Path after Rain|recovery|33|5|blue:16,green:16|Open board; lower demand after mixed objectives.
32|Камыши у воды|Reeds by the Water|normal|29|5|chain:6,blue:20|Chain clusters evoke reeds without new reed mechanics or art.
33|Письмо течения|A Message in the Current|normal|30|5|feather:2,ice:6|Clear ice beneath movable feathers; no current or reverse gravity feature.
34|Три мостка|Three Walkways|normal|29|5|crate:9,red:20|Three small crate groups with open separator columns.
35|Тень под мостом|Shadow beneath the Bridge|hard|26|6|chain:10,feather:3|Three visible, reachable exits; plan descent before using lines.
36|Тихая заводь|The Quiet Backwater|recovery|33|5|feather:1,blue:16|One central feather; no blocker at the exit.
37|Круг света|A Circle of Light|teach|30|5|ice:8,score:2400|Optional row/column combination; avoid scripted guaranteed victory.
38|Скрип переправы|The Creaking Crossing|normal|29|5|crate:8,chain:6|Different obstacle cells; alternate which pocket is opened first.
39|След за рекой|A Trail beyond the River|normal|28|6|feather:2,green:24|Offset starting feather heights with open central lane.
40|Берег удержан|The Bank Holds|hard|27|6|ice:10,crate:8,blue:24|No boss combat promise; a mixed-objective river checkpoint.
41|Огонь у дома|Fire by the House|recovery|33|5|red:16,blue:16|Unobstructed recovery board; keep village setting.
42|Угольки в ладони|Embers in the Palm|normal|28|5|crate:10,red:22|Break outer crate ring in segments; keep several openings.
43|Последний замок|The Last Lock|normal|29|6|chain:10,green:22|Offset chain patches; space for special construction.
44|Не дать огню угаснуть|Keep the Fire Alive|normal|26|5|survival:16,ice:6|Move-based survival already taught at round9; not a timed defense battle.
45|Три огня|Three Fires|hard|26|6|feather:3,crate:10|Three descent routes; one-layer crates before deeper two-layer pockets.
46|Утро в саду|Morning in the Orchard|recovery|34|5|ice:4,green:16|Small corner patches and open center.
47|Собрать искры|Gather the Sparks|normal|29|5|red:24,blue:24|Color-collection focus; contrast with preceding obstacle rounds.
48|Путь к сигнальному огню|Path to the Beacon|normal|29|6|crate:8,chain:8|Two obstacle zones; no overlapping ice/chain/crate cells.
49|Держать свет|Hold the Light|normal|28|5|survival:18,blue:22|Stable move budget and no new mechanic before checkpoint.
50|Сигнальный огонь|The Beacon|hard|28|6|crate:12,feather:3,red:26|Sector checkpoint only; light a village beacon, keep Firebird weak and stay in Chapter1.'''
rows=[]
for l in levels:
 rows.append({'round':l['levelId'],'key':f"chapter01.round{l['levelId']:03d}",'status':'existing-board-preserved','nameRu':l['displayName'],'nameEn':None,'difficulty':l['difficulty'],'moves':l['moveLimit'],'colors':l['tileTypes'],'goals':l['goals'],'reward':l['rewards'],'legacyId':l['id'],'legacyBoardSeed':l['boardSeed'],'layoutBrief':'Preserve exact source board, goals, moves, seed, thresholds and first-clear reward. Round10 narrative/label adaptation is proposed separately.' if l['levelId']==10 else 'Preserve exact authored source board and reward.','boardAuthored':True,'campaignPublished':False})
for line in briefs.splitlines():
 n,ruName,enName,band,moves,colors,goals,layout=line.split('|');n=int(n);gs=[]
 for part in goals.split(','):
  k,v=part.split(':');v=int(v)
  if k in ['red','blue','green']:g={'type':'collect_tile','tileType':{'red':0,'blue':1,'green':2}[k],'target':v}
  elif k in ['ice','chain','crate']:g={'type':'clear_obstacle','obstacleType':k,'target':v}
  elif k=='feather':g={'type':'bring_down','itemType':'firebird_feather','target':v}
  else:g={'type':k,'target':v}
  gs.append(g)
 reward={'gold':{'recovery':160,'teach':170,'normal':180,'hard':220}[band],'gems':3 if band=='hard' else 0,'boosterMace':int(n in [20,50]),'boosterFirebird':int(n==30),'boosterLivingLight':int(n==40),'boosterSeaPearl':int(n==50)}
 rows.append({'round':n,'key':f'chapter01.round{n:03d}','status':'proposed-brief-unbalanced','nameRu':ruName,'nameEn':enName,'difficulty':band,'moves':int(moves),'colors':int(colors),'goals':gs,'reward':reward,'layoutBrief':layout,'boardAuthored':False,'campaignPublished':False})
plan={'status':'held-for-gameplay-mechanics-review-not-runtime-content','userClarification':'rotation means rounds, not gem swap or group rotation','dependency':'Review GameplayReview.md and validate candidate rules/effects before authoring or balancing these baseline briefs.','chapter':1,'roundsPerChapter':500,'sector':1,'boardSize':{'columns':8,'rows':7},'rewards':'First-clear only; zero replay/scene/sector extra grants. All proposed values provisional.','rows':rows}
write(base/'Plan/First50Rounds.json',plan)
# Existing scene strings stay verbatim; only new IDs get draft text.
mapping=[{'sceneId':s['id'],'legacyAfterRound':next((m['afterLevelId'] for m in story['milestones'] if m['sceneId']==s['id']),0),'proposedAfterRound':500 if s['id']=='chapter01.finale' else next((m['afterLevelId'] for m in story['milestones'] if m['sceneId']==s['id']),0),'textPolicy':'preserve existing RU/EN verbatim','legacySeenPolicy':'legacy finale never acknowledges campaign finale' if s['id']=='chapter01.finale' else 'proposed explicit scene-ID credit; no rewards; not applied'} for s in story['scenes']]
write(base/'Plan/StoryMapping.json',{'status':'proposed-not-applied','existingScenes':mapping,'chapterUnlock':'all500 wins + acknowledged published finale + validated Chapter2 package','round10':'Replace campaign trigger only with campaign.chapter01.hearth_secured; leave legacy Resources unchanged.','round50':'Sector checkpoint only; no chapter complete, no Chapter2 unlock.'})
newScenes=[
(2,'apple_blossom','speaker.narrator','На старой яблоне раскрылся первый цветок. Деревня ещё бледна, но свет уже возвращается.','The first blossom opened on the old apple tree. The village was still pale, but light was returning.'),
(10,'hearth_secured','speaker.firebird','Этот очаг снова горит, Иван. Но за рекой мои искры всё ещё гаснут. Нам нужно сберечь свет для всей деревни.','This hearth burns again, Ivan. But my sparks are still fading beyond the river. We must keep the light alive for the whole village.'),
(20,'well_cleared','speaker.elder','В колодце снова чистая вода. Помоги открыть амбар — людям нужны силы, чтобы пережить эту ночь.','The well has clear water again. Help us open the granary; the villagers need strength to get through this night.'),
(30,'granary_open','speaker.ivan','Припасы в безопасности. Теперь проверим переправу: тень всё ещё тянется к нашим огням.','The supplies are safe. Now we should check the crossing: the shadow is still reaching for our fires.'),
(40,'river_bank','speaker.firebird','У берега стало светлее, но это лишь передышка. Разожги сигнальный огонь, чтобы деревня не потеряла путь во тьме.','The riverbank is brighter, but this is only a respite. Light the beacon so the village can find its way through the darkness.'),
(50,'beacon_lit','speaker.ivan','Сигнальный огонь зажжён. Я останусь, пока мы не защитим деревню, а потом найду источник угасания.','The beacon is lit. I will stay until we have protected the village, then find the source of the fading.')]
write(base/'Plan/NewSceneDrafts.json',{'status':'draft-needs-narrative-review','scenes':[{'id':f'campaign.chapter01.{id}','afterRound':n,'returnRoute':'ChapterMap','lines':[{'speakerKey':speaker,'textKey':f'campaign.chapter01.{id}.01','ru':r,'en':e}]} for n,id,speaker,r,e in newScenes]})
translations=[
'The first spark has awakened. Follow its light; it will show the way.',
'I understand the board now. Next time, the spark will be ours.',
'With every careful move, the fire grows brighter.',
'Do not rush, Ivan. See the whole pattern before you move.',
'The ice has cracked. The darkness no longer seems invincible.',
'The ice is strong, but every crack teaches us something.',
'The village feels warmth again. Keep following the trail.',
'I will not give up. I only need to choose the right order of moves.',
'The chains have fallen. The raven can no longer hold this light.',
'Break the bonds around your goal before chasing the sparks.',
'The old road is open. Beyond it lies the source of the fading.',
'The crates hide the sparks we need. Clear the space around them.',
'Your determination is restoring my strength, Ivan.',
'I was one good decision short. Now I can see it.',
'The golden feathers are free. They will guide us across the river.',
'Clear the downward path first, then free the feathers.',
'I held out. The Living Flame is very close now.',
'Keep the fire alive until the last move. This trial takes patience.',
'You have brought dawn back to the village. Today, Rus has grown stronger.',
'The final seal still holds, but now I know its weak point.',
'CHAPTER SUMMARY','{0}/{1} LEVELS • {2}/{3} STARS','The final level reward has already been received on the Victory screen.']
missing=[k for k in ru if k not in en];assert len(missing)==len(translations)==23
write(base/'Plan/LocalizationDraft.json',{'status':'draft-not-installed','entries':[{'key':k,'ru':ru[k],'en':v,'campaignPolicy':'legacy-only at round10; campaign round10 needs replacement reaction copy' if '.level10.' in k else 'candidate translation; editorial review required'} for k,v in zip(missing,translations)]})
budget={key:sum(r['reward'][key] for r in rows) for key in levels[0]['rewards']}
write(base/'QA/RewardBudget.json',{'status':'proposed-not-granted','first10':{k:sum(r['reward'][k] for r in rows[:10]) for k in budget},'new11to50':{k:sum(r['reward'][k] for r in rows[10:]) for k in budget},'first50':budget,'replayGrant':0,'extraSceneOrSectorGrant':0})
print('Created 50 round briefs, 8 mappings, 6 scene drafts and 23 EN draft repairs.')
