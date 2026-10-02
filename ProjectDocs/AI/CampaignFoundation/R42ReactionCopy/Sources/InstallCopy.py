from pathlib import Path
import json,uuid,shutil
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
active=root/'Assets/LegendsOfRus/Resources/CampaignActive'
draft=json.loads((base/'Reaction91To100Draft.json').read_text())
rows=draft['entries'];assert len(rows)==20
levels=json.loads((active/'levels.json').read_text())['levels'];story=json.loads((active/'story.json').read_text())
for row in rows:
 level=levels[row['round']-1]
 assert row['reviewedAgainst']=={k:level[k] for k in ['goals','obstacles','boardItems']}
 reaction=next(r for r in story['reactions'] if r['levelId']==row['round'] and r['outcome']==row['outcome'])
 assert reaction['textKey']==row['key'] and reaction['speakerKey']==row['speakerKey']
folder=root/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01EditorialR42';folder.mkdir(exist_ok=False)
folder.with_suffix('.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
for name in ['levels','story']:shutil.copy2(active/f'{name}.json',folder/f'{name}.json')
for lang in ['ru','en']:
 path=active/f'story_{lang}.json';data=json.loads(path.read_text());values={e['key']:e for e in data['entries']}
 assert len({r[lang] for r in rows})==20
 for row in rows:values[row['key']]['value']=row[lang]
 text=json.dumps(data,ensure_ascii=False,indent=2)+'\n';path.write_text(text);(folder/path.name).write_text(text)
for path in folder.glob('*.json'):Path(str(path)+'.meta').write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nTextScriptImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n')
path=root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/ActiveCampaignIntegrationTests.cs'
text=path.read_text();assert text.count('Chapter01EditorialR41')==1;path.write_text(text.replace('Chapter01EditorialR41','Chapter01EditorialR42'))
(base/'AppliedCopy.json').write_text(json.dumps(dict(status='Installed in CampaignActive; R42 reviewed draft and prior candidates preserved.',entries=rows),ensure_ascii=False,indent=2)+'\n')
print('Installed 20 reaction replacements per locale; R42 candidate created; R41 preserved.')
