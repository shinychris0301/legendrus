from pathlib import Path
import subprocess,sys,json
base=Path(__file__).resolve().parent
qa=base.parent/'QA'
assert json.loads((qa/'BoundariesFinal/summary.json').read_text())['result']['result']=='Passed'
groups={
 'Core':('PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 'Navigation':('PlayMode','ChapterMapAlignmentPlayModeTests;WorldMapAlignmentPlayModeTests;PreLevelAlignmentPlayModeTests;HomeAlignmentPlayModeTests.LiveHud_RefreshesFromServiceChangesAndReentry;ErrorDialogAlignmentPlayModeTests;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 'Runtime':('PlayMode','Match3RuntimePlayModeTests;CoreLoopVfxBoosterFeedbackPlayModeTests;GameplayAlignmentPlayModeTests;FailAlignmentPlayModeTests;TutorialAlignmentPlayModeTests;OutOfEnergyAlignmentPlayModeTests;FunctionalDemoNavigationPlayModeTests.LifecycleCallbacksPreserveBoardAndReleaseInterruptedTouch'),
 'Retained':('PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 'Edit':('EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;CampaignBoosterTests;OutOfEnergyAlignmentEditModeTests;ChapterMapAlignmentEditModeTests;PreLevelAlignmentEditModeTests;GameplayAlignmentEditModeTests;FailAlignmentEditModeTests;TutorialAlignmentEditModeTests')
}
(qa/'FinalGroups.json').write_text(json.dumps(groups,indent=2)+'\n')
for name,(mode,filters) in groups.items():
 print('Starting '+name,flush=True)
 subprocess.run([sys.executable,str(base/'Run.py'),name,mode,filters],check=True)
