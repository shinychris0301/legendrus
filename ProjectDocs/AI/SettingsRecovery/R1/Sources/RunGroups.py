from pathlib import Path
import subprocess,sys,json
base=Path(__file__).resolve().parent;qa=base.parent/'QA'
assert json.loads((qa/'Boundaries/summary.json').read_text())['result']['result']=='Passed'
groups={
 'Core':('PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 'Settings':('PlayMode','SettingsAlignmentPlayModeTests;AudioHapticsFidelityVerificationPlayModeTests;PauseAlignmentPlayModeTests'),
 'Navigation':('PlayMode','PreLevelAlignmentPlayModeTests;ErrorDialogAlignmentPlayModeTests;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 'Retained':('PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 'Edit':('EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;SettingsAlignmentEditModeTests;TutorialAlignmentEditModeTests;ErrorDialogAlignmentEditModeTests;PauseAlignmentEditModeTests')
}
(qa/'FinalGroups.json').write_text(json.dumps(groups,indent=2)+'\n')
for name,(mode,filters) in groups.items():
 print('Starting '+name,flush=True);subprocess.run([sys.executable,str(base/'Run.py'),name,mode,filters],check=True)
