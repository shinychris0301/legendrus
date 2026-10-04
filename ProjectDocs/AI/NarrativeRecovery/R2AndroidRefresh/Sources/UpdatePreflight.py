"""Pin the actual inspected APK and require compiled narrative acknowledgement evidence."""
from pathlib import Path
import json,ast
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
a=json.loads((QA/'Artifact.json').read_text());assert json.loads((QA/'BuildValidation.json').read_text())['result']=='Passed'
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/Preflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace('ProjectDocs/AI/EnergyRecovery/R2AndroidRefresh/QA','ProjectDocs/AI/NarrativeRecovery/R2AndroidRefresh/QA').replace('766024682',str(a['bytes'])).replace('8db3d3e4fb4f8ba24a69ccff1b981df9fdb7b99bd207be19e252f16eb338b569',a['sha256'])
s=s.replace("'ScreenRouter_CommitGameplayVictory')", "'ScreenRouter_CommitGameplayVictory','GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion','StoryService_AdvanceDialogue','ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete')",1)
s=s.replace("PACKAGE = ", "ATOMIC_NARRATIVE_APIS = ('GameState_CompleteStoryScene','GameState_AcknowledgeChapterCompletion')\nNARRATIVE_ROUTES = ('ScreenRouter_ContinueAfterDialogue','ScreenRouter_ContinueAfterChapterComplete')\nPACKAGE = ",1)
s=s.replace("final.get('nativeMethodCount') == 30", "final.get('nativeMethodCount') == 35\n            and final.get('atomicNarrativeAcknowledgementRecoveryPackaged') is True\n            and native.get('atomicNarrativeAcknowledgementRecoveryPackaged') is True")
needle="            and native.get('packagedLibrarySha256') == final.get('nativeRecoveryLibrarySha256')"
s=s.replace(needle,"""            and all(native.get('methods', {}).get(owner, {}).get('callsAtomicBalanceCommit') is True
                    and native.get('methods', {}).get(owner, {}).get('callsPersist') is False
                    for owner in ATOMIC_NARRATIVE_APIS)
            and native.get('methods', {}).get('StoryService_AdvanceDialogue', {}).get('callsStoryCommit') is True
            and native.get('methods', {}).get('StoryService_AdvanceDialogue', {}).get('rejectsBeforeClearingFinalSequence') is True
            and all(native.get('methods', {}).get(owner, {}).get('callsErrorDialog') is True for owner in NARRATIVE_ROUTES)
            and native.get('methods', {}).get('ScreenRouter_ContinueAfterChapterComplete', {}).get('callsChapterAcknowledgement') is True
"""+needle)
s=s.replace('                      cappedAtomicEnergyAndClockRecoveryPackaged=bool(matches),','                      atomicNarrativeAcknowledgementRecoveryPackaged=bool(matches),\n                      cappedAtomicEnergyAndClockRecoveryPackaged=bool(matches),')
ast.parse(s);p.write_text(s)
p=ROOT/'ProjectDocs/AI/AndroidGameplayJourney/R2OfflineHandoff/Sources/CheckPreflight.py';s=p.read_text();assert s==(BASE/'Sources/Before'/p.relative_to(ROOT)).read_text()
s=s.replace("'nativeMethodCount': 30", "'nativeMethodCount': 35,'atomicNarrativeAcknowledgementRecoveryPackaged': True")
s=s.replace("'stage': 'After', 'cappedAtomicEnergyAndClockRecoveryPackaged': True", "'stage': 'After', 'atomicNarrativeAcknowledgementRecoveryPackaged': True, 'cappedAtomicEnergyAndClockRecoveryPackaged': True")
s=s.replace("tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS", "tool.ATOMIC_SPENDS + tool.ATOMIC_ENERGY_APIS + tool.ATOMIC_NARRATIVE_APIS")
s=s.replace("'callsTryEnergyApi': owner in tool.ENERGY_WRAPPERS", "'callsStoryCommit':owner=='StoryService_AdvanceDialogue',\n                    'rejectsBeforeClearingFinalSequence':owner=='StoryService_AdvanceDialogue',\n                    'callsErrorDialog':owner in tool.NARRATIVE_ROUTES,\n                    'callsChapterAcknowledgement':owner=='ScreenRouter_ContinueAfterChapterComplete',\n                    'callsTryEnergyApi': owner in tool.ENERGY_WRAPPERS")
s=s.replace('    mutations = [',"""    mutations = [
        ('unverified narrative build proof','BuildValidation.json','atomicNarrativeAcknowledgementRecoveryPackaged',False),
        ('unverified narrative native proof','AfterNative.json','atomicNarrativeAcknowledgementRecoveryPackaged',False),""")
s=s.replace("            ('GameState_SpendGold', 'packagedMethodBytesMatchCompiledOutput', False),", """            ('GameState_CompleteStoryScene','callsAtomicBalanceCommit',False),
            ('GameState_CompleteStoryScene','callsPersist',True),
            ('GameState_AcknowledgeChapterCompletion','callsAtomicBalanceCommit',False),
            ('StoryService_AdvanceDialogue','callsStoryCommit',False),
            ('StoryService_AdvanceDialogue','rejectsBeforeClearingFinalSequence',False),
            ('StoryService_AdvanceDialogue','packagedMethodBytesMatchCompiledOutput',False),
            ('ScreenRouter_ContinueAfterDialogue','callsErrorDialog',False),
            ('ScreenRouter_ContinueAfterChapterComplete','callsErrorDialog',False),
            ('ScreenRouter_ContinueAfterChapterComplete','callsChapterAcknowledgement',False),
            ('GameState_SpendGold', 'packagedMethodBytesMatchCompiledOutput', False),""")
ast.parse(s);p.write_text(s)
print('Current preflight now requires all35 owners and atomic narrative final-line/error evidence.')
