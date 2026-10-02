"""Inventory installed story resources and source anchors; no player data or Unity mutation."""
from pathlib import Path
import hashlib
import json

ROOT = Path(__file__).resolve().parents[5]
BASE = Path(__file__).resolve().parent.parent
QA = BASE / 'QA'


def sha(path):
    with path.open('rb') as stream:
        return hashlib.file_digest(stream, 'sha256').hexdigest()


resources = ROOT / 'Assets/LegendsOfRus/Resources/CampaignActive'
read = lambda name: json.loads((resources / (name + '.json')).read_text())
story, levels, ru, en = [read(name) for name in ('story', 'levels', 'story_ru', 'story_en')]
localized = {}
for language, data in (('ru', ru), ('en', en)):
    entries = data['entries']
    assert len(entries) == len({entry['key'] for entry in entries})
    localized[language] = {entry['key']: entry['value'] for entry in entries}
scenes = {scene['id']: scene for scene in story['scenes']}
rounds = {level['levelId']: level for level in levels['levels']}
milestones = sorted(story['milestones'], key=lambda row: row['afterLevelId'])
assert len(scenes) == len(story['scenes']) == 58 and len(milestones) == 57
assert len(rounds) == 500 and set(rounds) == set(range(1, 501))
assert len({row['sceneId'] for row in milestones}) == 57
ordered = [{'sceneId': story['introSceneId'], 'afterLevel': 0, 'kind': 'intro'}]
ordered += [{'sceneId': row['sceneId'], 'afterLevel': row['afterLevelId'],
             'kind': 'finale' if row['sceneId'] == story['completion']['finaleSceneId'] else 'milestone'}
            for row in milestones]
assert len({row['sceneId'] for row in ordered}) == 58
observations = []
line_count = 0
for row in ordered:
    scene = scenes[row['sceneId']]
    assert scene['lines']
    if row['afterLevel']:
        assert scene['trigger'] in rounds[row['afterLevel']]['storyTriggers']
    for line in scene['lines']:
        for locale in localized.values():
            assert locale.get(line['speakerKey'], '').strip() and locale.get(line['textKey'], '').strip()
    line_count += len(scene['lines'])
    observations.append({**row, 'trigger': scene['trigger'], 'returnRoute': scene['returnRoute'],
                         'lines': len(scene['lines']), 'russianAndEnglishKeysComplete': True})
assert observations[-1]['afterLevel'] == story['completion']['finalLevelId'] == 500
assert observations[-1]['kind'] == 'finale' and observations[-1]['returnRoute'] == 'ChapterComplete'
assert sum(row['returnRoute'] == 'ChapterComplete' for row in observations) == 1
content = {'sceneCount': 58, 'milestoneCount': 57, 'roundCount': 500, 'localizedSceneLines': line_count,
           'localizationEntryCounts': {language: len(data) for language, data in localized.items()},
           'introScene': story['introSceneId'], 'finaleScene': story['completion']['finaleSceneId'],
           'scenesInNarrativeOrder': observations,
           'sceneLineLengthCounts': {str(count): sum(len(scene['lines']) == count for scene in scenes.values())
                                   for count in sorted({len(scene['lines']) for scene in scenes.values()})},
           'existingSceneSummaryFields': any(any('summary' in key.lower() for key in scene) for scene in scenes.values()),
           'inspectionIsNotHumanAcceptanceOrRecapImplementation': True,
           'fiveResourceSha256': {str(p.relative_to(ROOT)): sha(p) for p in resources.glob('*.json')}}
assert len(content['fiveResourceSha256']) == 5
(QA / 'InstalledStoryInventory.json').write_text(json.dumps(content, indent=2) + '\n')

anchors = {
    'Assets/LegendsOfRus/Scripts/Runtime/Gameplay/ActiveCampaignContent.cs': ['ResourceRoot = "CampaignActive/"', 'FinaleReady = true'],
    'Assets/LegendsOfRus/Scripts/Runtime/Services/StoryData.cs': ['ActiveCampaignContent.Load(forceReload).LoadStory(1)', 'public sealed class StorySequenceRuntime', 'public LocalizedStoryLine LocalizeCurrentLine'],
    'Assets/LegendsOfRus/Scripts/Runtime/Services/StoryService.cs': ['if (state.HasCompletedStoryScene(scene.id)) return false;', 'bool completedNow = state != null && state.CompleteStoryScene', 'public static void ResetRuntime()'],
    'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs': ['public int selectedLevel = 1;', 'public List<string> completedStoryScenes', 'public bool CompleteStoryScene', 'public void EnsureSelectedLevelIsUnlocked'],
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs': ['home.BindNavigation(target => Show(target), OpenChapterMap)', 'void OpenSelectedLevel()', 'bool TryShowPendingChapterFlow()', 'void ContinueAfterDialogue()'],
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionChapterMapScroller.cs': ['public void JumpToCurrent()=>JumpToLevel', 'CurrentLabel.text=en?"CURRENT":"К УРОВНЮ"'],
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionProfileRuntime.cs': ['static void BuildHistory', 'SAVED LEVEL PROGRESS', 'No recorded attempts yet.'],
    'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionDialogueView.cs': ['public Func<string, LocalizedStoryLine> LineProvider;', 'public void Bind(Action onContinue)', 'LineProvider != null ? LineProvider(language)'],
    'Assets/LegendsOfRus/Scripts/Runtime/Services/CampaignStoryFlow.cs': ['catalog.StoryLedgerKey(chapter, scene.id)', 'public CampaignSaveOperation AcknowledgeStory', 'if (!catalog.IsLegacyStoryBridge(chapter)) return false;']}
source = []
for name, expected in anchors.items():
    path = ROOT / name
    lines = path.read_text().splitlines()
    source.append({'path': name, 'sha256': sha(path), 'anchors': [
        {'text': anchor, 'line': next(index + 1 for index, line in enumerate(lines) if anchor in line)}
        for anchor in expected]})
(QA / 'SourceAnchors.json').write_text(json.dumps(source, indent=2) + '\n')
print(json.dumps({key: value for key, value in content.items() if key not in ('scenesInNarrativeOrder', 'fiveResourceSha256')}, indent=2))
