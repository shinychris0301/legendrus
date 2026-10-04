"""Run applicable final Unity groups sequentially with a separate healthy language restart."""
from pathlib import Path
import subprocess,sys
BASE=Path(__file__).resolve().parent
groups=[
 ('Core','PlayMode','LevelSystemRuntimePlayModeTests;SaveMigrationRecoveryPlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 ('Settings','PlayMode','SettingsAlignmentPlayModeTests;AudioHapticsFidelityVerificationPlayModeTests;PauseAlignmentPlayModeTests;LanguageSelectAlignmentPlayModeTests;ChapterFinaleCompletionPlayModeTests.LegacyV5Save_MigratesAcknowledgementStateToVersion6'),
 ('Navigation','PlayMode','PreLevelAlignmentPlayModeTests;ErrorDialogAlignmentPlayModeTests;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 ('Retained','PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 ('Edit','EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;SettingsAlignmentEditModeTests;TutorialAlignmentEditModeTests;ErrorDialogAlignmentEditModeTests;PauseAlignmentEditModeTests'),
 ('LanguageWrite','PlayMode','LanguagePreferenceRestartPlayModeTests'),
 ('LanguageRead','PlayMode','LanguagePreferenceRestartPlayModeTests')]
for name,mode,filter in groups:
    args=[sys.executable,str(BASE/'Run.py'),name,mode,filter]
    if name.startswith('Language'):args.append(str(BASE/(name+'.json')))
    print('Final Unity group: '+name,flush=True)
    subprocess.run(args,check=True)
