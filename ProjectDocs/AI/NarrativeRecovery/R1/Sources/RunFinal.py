from pathlib import Path
import subprocess,sys,json
BASE=Path(__file__).resolve().parent.parent
runs=[
 ('Core','PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 ('UI','PlayMode','GameplayAlignmentPlayModeTests;PreLevelAlignmentPlayModeTests;ErrorDialogAlignmentPlayModeTests;HomeAlignmentPlayModeTests.LiveHud_RefreshesFromServiceChangesAndReentry;InventoryAlignmentPlayModeTests;OutOfEnergyAlignmentPlayModeTests'),
 ('Runtime','PlayMode','Match3RuntimePlayModeTests;CoreLoopVfxBoosterFeedbackPlayModeTests.SeaPearl_GrantsFiveMovesWithoutBoardOrUnrelatedSaveMutation;CoreLoopVfxBoosterFeedbackPlayModeTests.Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce'),
 ('Retained','PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 ('Journey','PlayMode','FunctionalDemoNavigationPlayModeTests.EnergyClockCatchupIsCappedAndNotRepeated;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 ('NarrativeFinalUI','PlayMode','DialogueAlignmentPlayModeTests;StoryMilestoneReactionPlayModeTests;LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.DepartureRetiresReaderWithoutStaleCallbacks;LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.EmptyHistoryIsTruthfulAndCannotOpenReader;LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.FinaleFinishAndRepeatedReentryNeverDispatchChapterComplete;LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.IntroRepeatedNativeTapAdvancesOneLineAndBackCloses;LegendsOfRus.Tests.PlayMode.StoryRecapPlayModeTests.NativeReaderBrowsesAndReturnsWithoutChangingStoryOrJournal'),
 ('RecapQA','PlayMode','StoryRecapPlayModeTests.DisposableRecapPreservesSyntheticNormalJournal'),
 ('Legacy','PlayMode','StoryDataRuntimePlayModeTests;ChapterCompleteAlignmentPlayModeTests'),
 ('Edit','EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;CampaignBoosterTests;OutOfEnergyAlignmentEditModeTests;StoryRecapEditModeTests;DialogueAlignmentEditModeTests;ChapterCompleteAlignmentEditModeTests')]
for name,mode,filter in runs:
 if len(sys.argv)>1 and name not in sys.argv[1:]:continue
 print('Running '+name,flush=True)
 command=[sys.executable,str(BASE/'Sources/Run.py'),name,mode,filter]
 if name=='RecapQA':command.append(str(BASE/'Sources/RecapQA.json'))
 result=subprocess.run(command,capture_output=True)
 (BASE/'QA'/name/'launcher.txt').write_bytes(result.stdout+result.stderr)
 if result.returncode:
  print(result.stdout.decode(errors='replace')[-5000:],flush=True);raise RuntimeError(name+' failed; evidence retained')
 s=json.loads((BASE/'QA'/name/'summary.json').read_text());print(json.dumps({'group':name,'result':s['result']['result'],'passed':s['result']['passed'],'failed':s['result']['failed'],'skipped':s['result']['skipped']}),flush=True)
