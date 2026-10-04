from pathlib import Path
import subprocess,sys,json
BASE=Path(__file__).resolve().parent.parent
runs=[
 ('Core','PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 ('Runtime','PlayMode','Match3RuntimePlayModeTests;CoreLoopVfxBoosterFeedbackPlayModeTests.SeaPearl_GrantsFiveMovesWithoutBoardOrUnrelatedSaveMutation;CoreLoopVfxBoosterFeedbackPlayModeTests.Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce'),
 ('UI','PlayMode','GameplayAlignmentPlayModeTests;PreLevelAlignmentPlayModeTests;ErrorDialogAlignmentPlayModeTests;HomeAlignmentPlayModeTests.LiveHud_RefreshesFromServiceChangesAndReentry;InventoryAlignmentPlayModeTests'),
 ('Retained','PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 ('Journeys','PlayMode','FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 ('Edit','EditMode','SaveJournalWriteEditModeTests;PlayerSaveMigrationRegistryEditModeTests;CampaignBoosterTests'),
 ('Policy','EditMode','LegendsOfRus.Tests.AndroidBuild;LegendsOfRus.Tests.ContentDelivery;LegendsOfRus.Tests.ProductionCoreLoopUi.DevelopmentGameplayFixtureEditModeTests;LegendsOfRus.Tests.ProductionCoreLoopUi.DailyRewardAlignmentEditModeTests;LegendsOfRus.Tests.ProductionCoreLoopUi.RewardReceivedAlignmentEditModeTests')]
for name,mode,filter in runs:
 print('Running final '+name,flush=True)
 result=subprocess.run([sys.executable,str(BASE/'Sources/Run.py'),name,mode,filter],capture_output=True)
 (BASE/'QA'/name/'launcher.txt').write_bytes(result.stdout+result.stderr)
 if result.returncode:
  print(result.stdout.decode(errors='replace')[-5000:],flush=True);raise RuntimeError(name+' failed; evidence retained')
 summary=json.loads((BASE/'QA'/name/'summary.json').read_text())
 print(json.dumps({'group':name,'result':summary['result']['result'],'passed':summary['result']['passed'],'failed':summary['result']['failed'],'skipped':summary['result']['skipped']}),flush=True)
