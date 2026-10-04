"""Pin the verified tutorial APK and require exact compiled recovery owners/callbacks."""
from pathlib import Path
import ast,json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
new_owners=('GameState_CompleteTutorial','LevelTutorialPresenter_Advance','LevelTutorialPresenter_BuildOverlay','ScreenRouter_ShowGameplayTutorialError','ScreenRouter_BuildGameplayOverlay','ScreenRouter_NavigateBack','ScreenRouter_RetireModalRootImmediately')
flags={'LevelTutorialPresenter_Advance':('rejectsBeforeClearingTutorialStep','guardsInactiveTutorialPresenter'),'LevelTutorialPresenter_BuildOverlay':('capturesPresentedOverlayAndStep','ownedContinueCallbackCompiledAndPackaged'),'ScreenRouter_ShowGameplayTutorialError':('scopedTutorialErrorOwner',),'ScreenRouter_BuildGameplayOverlay':('registersAcknowledgementRejected','ownedErrorCallbackCompiledAndPackaged'),'ScreenRouter_NavigateBack':('tutorialBackOwnerPriority',),'ScreenRouter_RetireModalRootImmediately':('clearsTutorialErrorOwner',)}
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/TutorialRecovery/R2AndroidRefresh/QA').replace('766019098',str(a['bytes'])).replace('932535353501df1e04e65814ffd2383edc44b33bc60973aa692ae5e84b3ae928',a['sha256'])
s=s.replace("'ScreenRouter_ContinueAfterChapterComplete')", "'ScreenRouter_ContinueAfterChapterComplete',"+','.join(repr(o) for o in new_owners)+')',1)
s=s.replace('PACKAGE = ', 'ATOMIC_TUTORIAL_APIS = (\'GameState_CompleteTutorial\',)\nTUTORIAL_FLAGS = '+repr(flags)+'\nPACKAGE = ',1)
s=s.replace("final.get('nativeMethodCount') == 35", "final.get('nativeMethodCount') == 42\n            and final.get('atomicTutorialAcknowledgementRecoveryPackaged') is True\n            and native.get('atomicTutorialAcknowledgementRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')"
s=s.replace(needle,"""            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False for owner in ATOMIC_TUTORIAL_APIS)
            and all(native.get('methods', {}).get(owner, {}).get(flag) is True for owner,flags in TUTORIAL_FLAGS.items() for flag in flags)
            and set(native.get('tutorialCallbacks', {})) == {'OwnedTutorialContinue','OwnedTutorialError'}
            and all(row.get('packagedMethodBytesMatchCompiledOutput') is True for row in native.get('tutorialCallbacks', {}).values())
"""+needle)
s=s.replace('                      atomicNarrativeAcknowledgementRecoveryPackaged=bool(matches),','                      atomicTutorialAcknowledgementRecoveryPackaged=bool(matches),\n                      atomicNarrativeAcknowledgementRecoveryPackaged=bool(matches),')
ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 35", "'nativeMethodCount': 42,'atomicTutorialAcknowledgementRecoveryPackaged': True")
s=s.replace("'stage': 'After', 'atomicNarrative", "'stage': 'After','atomicTutorialAcknowledgementRecoveryPackaged': True,'tutorialCallbacks':{k:{'packagedMethodBytesMatchCompiledOutput':True} for k in ('OwnedTutorialContinue','OwnedTutorialError')}, 'atomicNarrative")
s=s.replace('tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS + tool.ATOMIC_NARRATIVE_APIS','tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS + tool.ATOMIC_NARRATIVE_APIS + tool.ATOMIC_TUTORIAL_APIS')
s=s.replace("'callsStoryCommit':owner", "**{flag:True for flag in tool.TUTORIAL_FLAGS.get(owner,())},\n                    'callsStoryCommit':owner")
s=s.replace('    mutations = [',"""    mutations = [
        ('unverified tutorial build proof','BuildValidation.json','atomicTutorialAcknowledgementRecoveryPackaged',False),
        ('unverified tutorial native proof','AfterNative.json','atomicTutorialAcknowledgementRecoveryPackaged',False),
        ('missing tutorial callback proof','AfterNative.json','tutorialCallbacks',{}),
        ('rejected callback native-byte proof','AfterNative.json','tutorialCallbacks',{'OwnedTutorialContinue':{'packagedMethodBytesMatchCompiledOutput':False},'OwnedTutorialError':{'packagedMethodBytesMatchCompiledOutput':True}}),""")
needle="            ('GameState_CompleteStoryScene','callsAtomicBalanceCommit',False),"
mutations=[('GameState_CompleteTutorial','callsAtomicBalanceCommit',False),('GameState_CompleteTutorial','callsPersist',True),('GameState_CompleteTutorial','packagedMethodBytesMatchCompiledOutput',False)]+[(o,k,False) for o,keys in flags.items() for k in keys]
s=s.replace(needle, ''.join('            '+repr(r)+',\n' for r in mutations)+needle)
ast.parse(s);p.write_text(s)
print('Current preflight requires 42 owners, two packaged callbacks and tutorial rejection/owner guards.')
