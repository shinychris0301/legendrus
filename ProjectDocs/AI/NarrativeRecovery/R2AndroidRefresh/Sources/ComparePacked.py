"""Require unchanged packed membership and native page object sizes for the service-only fix."""
from collections import Counter
from pathlib import Path
import json

ROOT = Path(__file__).resolve().parents[5]
QA = Path(__file__).resolve().parent.parent / 'QA'
before = json.loads((ROOT / 'ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh/QA/PackedAssets.json').read_text())['assets']
record = json.loads((QA / 'PackedAssets.json').read_text()); after = record['assets']
assert record['result'] == 'Succeeded'
assert [record[k] for k in ('profileRevision', 'dialogueRevision', 'gameplayRevision', 'victoryRevision',
    'stockLayouts', 'storyScenes', 'storyMilestones', 'dailyRewardRevision', 'receiptRevision')] == [13,2,24,12,4,58,57,5,2]
key = lambda r: (r['path'], r['guid'], r['type'])
assert len(before) == len(after) == 29387
assert Counter(map(key, before)) == Counter(map(key, after))
pages = {}
for name in ('Profile', 'Dialogue', 'Gameplay', 'Victory', 'ChapterComplete', 'DailyReward', 'RewardClaim'):
    path = 'Assets/LegendsOfRus/Resources/ProductionUI/Pages/' + name + '.prefab'
    old = [r for r in before if r['path'] == path]; new = [r for r in after if r['path'] == path]
    assert old and Counter(map(key, old)) == Counter(map(key, new))
    assert sum(r['bytes'] for r in old) == sum(r['bytes'] for r in new)
    pages[name] = {'beforeObjects': len(old), 'afterObjects': len(new), 'packedObjectByteDelta': 0}
assert [r['selector'] for r in record['fixedPresets']] == ['stock-999','stock-1000','stock-9999','stock-max','story-early','story-finale']
result = {'beforeRows': len(before), 'afterRows': len(after), 'addedRows': 0,
    'allExistingPathGuidTypeMembershipUnchanged': True, 'pageDeltas': pages,
    'sixPureEditorCandidatesChecked': record['fixedPresets'], 'candidateCheckIsNotAndroidActivation': True}
(QA / 'PackedComparison.json').write_text(json.dumps(result, indent=2) + '\n')
print('All 29,387 packed rows and seven page object sizes retained.')
