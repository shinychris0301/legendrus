"""Pin the inspected selection APK and require compiled acknowledged navigation evidence."""
from pathlib import Path
import ast,json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
owners=('GameState_SelectLevel','GameState_EnsureSelectedLevelIsUnlocked','GameState_TryEnsureSelectedLevelIsUnlocked','GameState_NormalizeLevelProgress','ScreenRouter_OpenSelectedLevel','ProductionChapterMapView_BindNavigation','ScreenRouter_BuildInteractionLayer')
flags={'GameState_SelectLevel':('respectsSelectionWriteLock',),'GameState_TryEnsureSelectedLevelIsUnlocked':('respectsSelectionWriteLock','normalizesCandidateBeforeCommit'),'GameState_EnsureSelectedLevelIsUnlocked':('delegatesToAcknowledgedNormalization',),'GameState_NormalizeLevelProgress':('normalizesExplicitCandidate',),'ScreenRouter_StartLevel':('normalizationCheckedBeforeEnergySpend',),'ScreenRouter_OpenSelectedLevel':('normalizationCheckedBeforeStoryRouting',),'ProductionChapterMapView_BindNavigation':('acknowledgedMapCallbacksCompiledAndPackaged',),'ScreenRouter_BuildInteractionLayer':('bindsAcknowledgedMapOpenAndError',)}
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/TutorialRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/SelectionRecovery/R2AndroidRefresh/QA').replace('766026550',str(a['bytes'])).replace('b699646db5f7ac9effc403c768eb19200b5eb3b58f14858b331f5edb80aa5fb4',a['sha256'])
s=s.replace("'ScreenRouter_RetireModalRootImmediately')", "'ScreenRouter_RetireModalRootImmediately',"+','.join(repr(o) for o in owners)+')',1)
s=s.replace('PACKAGE = ', "ATOMIC_SELECTION_APIS = ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked')\nSELECTION_FLAGS = "+repr(flags)+'\nPACKAGE = ',1)
s=s.replace("final.get('nativeMethodCount') == 42", "final.get('nativeMethodCount') == 49\n            and final.get('atomicSelectionNormalizationRecoveryPackaged') is True\n            and native.get('atomicSelectionNormalizationRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')"
s=s.replace(needle,"""            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False for owner in ATOMIC_SELECTION_APIS)
            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in SELECTION_FLAGS.items() for flag in flags)
            and set(native.get('selectionCallbacks', {})) == {'AcknowledgedMapPlay','AcknowledgedMapNode','SelectionError'}
            and all(row.get('packagedMethodBytesMatchCompiledOutput') is True for row in native.get('selectionCallbacks', {}).values())
"""+needle)
s=s.replace('                      atomicTutorialAcknowledgementRecoveryPackaged=bool(matches),','                      atomicSelectionNormalizationRecoveryPackaged=bool(matches),\n                      atomicTutorialAcknowledgementRecoveryPackaged=bool(matches),')
ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 42", "'nativeMethodCount': 49,'atomicSelectionNormalizationRecoveryPackaged': True")
s=s.replace("'stage': 'After','atomicTutorial", "'stage': 'After','atomicSelectionNormalizationRecoveryPackaged': True,'selectionCallbacks':{k:{'packagedMethodBytesMatchCompiledOutput':True} for k in ('AcknowledgedMapPlay','AcknowledgedMapNode','SelectionError')},'atomicTutorial")
s=s.replace('tool.ATOMIC_NARRATIVE_APIS + tool.ATOMIC_TUTORIAL_APIS', 'tool.ATOMIC_NARRATIVE_APIS + tool.ATOMIC_TUTORIAL_APIS + tool.ATOMIC_SELECTION_APIS')
s=s.replace('**{flag:True for flag in tool.TUTORIAL_FLAGS.get(owner,())},','**{flag:True for flag in tool.TUTORIAL_FLAGS.get(owner,())},\n                    **{flag:True for flag in tool.SELECTION_FLAGS.get(owner,())},')
s=s.replace('    mutations = [',"""    mutations = [
        ('unverified selection build proof','BuildValidation.json','atomicSelectionNormalizationRecoveryPackaged',False),
        ('unverified selection native proof','AfterNative.json','atomicSelectionNormalizationRecoveryPackaged',False),
        ('missing acknowledged map callback proof','AfterNative.json','selectionCallbacks',{}),
        ('rejected map callback native-byte proof','AfterNative.json','selectionCallbacks',{'AcknowledgedMapNode':{'packagedMethodBytesMatchCompiledOutput':False},'AcknowledgedMapPlay':{'packagedMethodBytesMatchCompiledOutput':True},'SelectionError':{'packagedMethodBytesMatchCompiledOutput':True}}),""")
mutations=[(o,'callsAtomicBalanceCommit',False) for o in ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked')]+[(o,'callsPersist',True) for o in ('GameState_SelectLevel','GameState_TryEnsureSelectedLevelIsUnlocked')]+[(o,k,False) for o,keys in flags.items() for k in keys]+[('GameState_SelectLevel','packagedMethodBytesMatchCompiledOutput',False)]
needle="            ('GameState_CompleteTutorial', 'callsAtomicBalanceCommit', False),"
assert needle in s
s=s.replace(needle,''.join('            '+repr(r)+',\n' for r in mutations)+needle)
ast.parse(s);p.write_text(s)
print('Current preflight requires49owners/five callbacks and selection/normalization acknowledgements.')
