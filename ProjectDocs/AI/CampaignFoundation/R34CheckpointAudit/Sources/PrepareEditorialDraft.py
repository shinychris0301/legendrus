from pathlib import Path
import json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent;active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
story=json.loads((active/'story.json').read_text());levels=json.loads((active/'levels.json').read_text())['levels']
# Bounded editorial proposal; never writes Resources or historical candidates.
copy={
51:('Оба цвета собраны. У колодца стало светлее.','Both colors are gathered. The well is a little brighter.','Поищу комбинацию, которая соберёт сразу оба нужных цвета.','I will look for a match that helps collect both target colors.'),
52:('Ледяные уступы разбиты, самоцветы собраны. Можно идти дальше.','The icy ledges are broken and the gems gathered. We can move on.','Проверю оставшийся лёд и счётчик самоцветов: нужны обе цели.','I will check the remaining ice and gem counter. Both goals matter.'),
53:('Цепи сняты. Оба пера добрались до нижнего края.','The chains are gone. Both feathers have reached the bottom.','Освобожу клетки под каждым пером, чтобы ни одно не застряло.','I will free the cells beneath each feather so neither gets stuck.'),
54:('Ящики больше не загораживают ступени к воде.','The crates no longer block the steps to the water.','Разберу ступени из ящиков и не забуду про нужные самоцветы.','I will clear the stepped crates and keep collecting the target gems.'),
55:('Нижние цепи разорваны. Лёд уже не удержит эти перья.','The lower chains are broken. The ice cannot hold these feathers now.','Нижним цепям нужны два удара. Оставлю ходы и на лёд.','The lower chains need two hits. I will leave moves for the ice too.'),
56:('Нужные самоцветы собраны. Во дворе снова есть свет.','The target gems are gathered. There is light in the courtyard again.','На свободном поле поищу сочетание из четырёх самоцветов.','On this open board, I will look for a four-gem match.'),
57:('Мы продержались, и ледяная преграда исчезла.','We held on, and the icy barrier is gone.','Даже без льда нужно завершить нужное число ходов.','Even after the ice is gone, I must complete the required moves.'),
58:('Ящики убраны, цепи сняты. Между дворами можно пройти.','The crates and chains are gone. The passage between the yards is open.','Слева ящики, справа цепи. Проверю, какая цель ещё осталась.','Crates on the left, chains on the right. I will check which goal remains.'),
59:('Оба пера прошли через лёд. Их свет снова у нас.','Both feathers passed through the ice. Their light is with us again.','Поищу совпадения под перьями у краёв поля.','I will look for matches beneath the feathers near the board edges.'),
60:('Путь к колодцу открыт. Оба пера удалось сохранить.','The path to the well is open. Both feathers are safe.','Освобожу оба средних прохода и проверю ящики по сторонам.','I will free both middle routes and check the crates along the sides.')}
rows=[]
for n,text in copy.items():
 for outcome,offset in [('victory',0),('failure',2)]:
  reaction=next(r for r in story['reactions'] if r['levelId']==n and r['outcome']==outcome)
  rows.append(dict(round=n,outcome=outcome,key=reaction['textKey'],speakerKey=reaction['speakerKey'],ru=text[offset],en=text[offset+1],reviewedAgainst=dict(goals=levels[n-1]['goals'],obstacles=levels[n-1]['obstacles'],boardItems=levels[n-1]['boardItems'])))
assert len(rows)==20
for lang in ['ru','en']:assert len({r[lang] for r in rows})==20
(base/'Reaction51To60Draft.json').write_text(json.dumps(dict(status='Editorial proposal only; not installed or visually validated.',preserved='Existing keys and Ivan speaker; no difficulty, rewards or scene changes.',entries=rows),ensure_ascii=False,indent=2)+'\n')
lines=['# Reaction copy proposal: rounds 51–60','', 'Draft only. Original runtime copy is unchanged. Each entry was reviewed against the installed goals and obstacle layout; static checking cannot establish literary quality or on-screen fit. Keep accepted early scenes unchanged.','', '| Round | Outcome | Russian | English |','|---|---|---|---|']
for r in rows:lines.append(f"| {r['round']} | {r['outcome']} | {r['ru']} | {r['en']} |")
lines+=['','Address 61–100 and the repeated 11–50 failure lines in separate bounded passes. Occasional shared tactical wording in 101–200 is lower priority than 50 consecutive identical outcomes.']
(base/'EditorialDraft.md').write_text('\n'.join(lines)+'\n')
print('20 bilingual draft reactions, 20 distinct texts per language; no runtime installation.')
