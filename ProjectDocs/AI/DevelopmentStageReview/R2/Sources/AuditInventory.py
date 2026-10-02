"""Read-only development inventory. Counts are not feature acceptance scores."""
from pathlib import Path
import hashlib
import json
import xml.etree.ElementTree as ET

ROOT = Path(__file__).resolve().parents[5]
OUT = Path(__file__).resolve().parent.parent / 'QA'
RESOURCE = ROOT / 'Assets/LegendsOfRus/Resources'

def read(path):
    return json.loads(path.read_text())

def reference(path):
    return {'path': str(path.relative_to(ROOT)),
            'sha256': hashlib.sha256(path.read_bytes()).hexdigest()}

definition = read(RESOURCE / 'CampaignActive/definition.json')
levels = read(RESOURCE / 'CampaignActive/levels.json')
story = read(RESOURCE / 'CampaignActive/story.json')
locales = {lang: read(RESOURCE / f'CampaignActive/story_{lang}.json')['entries']
           for lang in ('ru', 'en')}
required = set()
def keys(value):
    if isinstance(value, dict):
        for key, child in value.items():
            if key.endswith('Key') and isinstance(child, str) and child:
                required.add(child)
            else:
                keys(child)
    elif isinstance(value, list):
        for child in value:
            keys(child)
keys(story)
keys(levels)
locale_report = {}
for lang, entries in locales.items():
    populated = {e['key'] for e in entries if e.get('value', '').strip()}
    locale_report[lang] = {'entries': len(entries), 'populatedKeys': len(populated),
        'duplicateKeys': len(entries) - len({e['key'] for e in entries}),
        'missingReferencedKeys': sorted(required - populated)}

configs = ['commercial_runtime_activation', 'economy_runtime_activation',
           'functionality_first_demo_v120_step04']
activation = {name: read(RESOURCE / f'Config/{name}.json') for name in configs}
pages = sorted(p.stem for p in (RESOURCE / 'ProductionUI/Pages').glob('*.prefab'))
r43 = ROOT / 'ProjectDocs/AI/CampaignFoundation/R43RegressionAudit/QA'
edit = ET.parse(r43 / 'EditMode03/results.xml').getroot().attrib
runtime = read(r43 / 'LatestRuntimeStatus.json')
inventory = {
    'scope': 'Fresh local source inventory; historical runtime evidence explicitly labelled.',
    'editor': (ROOT / 'ProjectSettings/ProjectVersion.txt').read_text().splitlines()[0],
    'pagePrefabs': pages,
    'campaign': {'definedChapters': len(definition['Chapters']),
        'roundsPerChapterTarget': definition['RoundsPerChapter'],
        'installedChapter': levels['chapterId'], 'installedRounds': len(levels['levels']),
        'roundIds': sorted(l['levelId'] for l in levels['levels']),
        'boardSizes': sorted({f"{l['columns']}x{l['rows']}" for l in levels['levels']}),
        'goalTypes': sorted({g['type'] for l in levels['levels'] for g in l['goals']}),
        'storyScenes': len(story['scenes']), 'storyMilestones': len(story['milestones']),
        'reactions': len(story['reactions']), 'completion': story['completion'],
        'referencedLocalizationKeys': len(required), 'locales': locale_report},
    'activation': activation,
    'historicalR43': {'editMode': edit, 'latestRuntimeCounts': runtime['counts'],
        'runtimeCaveat': runtime['note']},
    'sources': [reference(p) for p in sorted((RESOURCE / 'CampaignActive').glob('*.json'))]
        + [reference(RESOURCE / f'Config/{name}.json') for name in configs]
}
checks = {
    'native_page_inventory_36': len(pages) == 36,
    'defined_chapters_20_target_500': len(definition['Chapters']) == 20 and definition['RoundsPerChapter'] == 500,
    'installed_chapter1_contiguous_1_200': levels['chapterId'] == 1 and inventory['campaign']['roundIds'] == list(range(1, 201)),
    'no_premature_chapter_finale': story['completion'] is None,
    'story_locales_complete_and_unique': all(not v['missingReferencedKeys'] and not v['duplicateKeys'] and v['populatedKeys'] == v['entries'] for v in locale_report.values()),
    'commercial_disabled': activation['commercial_runtime_activation']['environment'] == 'disabled' and not activation['commercial_runtime_activation']['finalRuntimeActivationApproved'],
    'social_identity_liveops_disabled': all(not v for k, v in activation['functionality_first_demo_v120_step04'].items() if k.startswith('production') or k.startswith('automatic')),
    'r43_failures_remain_visible': runtime['counts']['Failed'] == 40,
}
OUT.mkdir(exist_ok=True)
(OUT / 'CurrentInventory.json').write_text(json.dumps(inventory, indent=2, ensure_ascii=False))
(OUT / 'InventoryChecks.json').write_text(json.dumps(checks, indent=2))
print(json.dumps({'pages': len(pages), 'campaign': inventory['campaign'], 'checks': checks}, ensure_ascii=False, indent=2))
raise SystemExit(0 if all(checks.values()) else 1)
