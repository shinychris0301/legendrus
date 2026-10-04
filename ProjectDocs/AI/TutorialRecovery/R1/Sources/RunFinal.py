from pathlib import Path
import subprocess,sys,json
BASE=Path(__file__).resolve().parent.parent
runs=[
 ('Core','PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 ('UI','PlayMode','TutorialAlignmentPlayModeTests;GameplayAlignmentPlayModeTests;PreLevelAlignmentPlayModeTests;ErrorDialogAlignmentPlayModeTests;HomeAlignmentPlayModeTests.LiveHud_RefreshesFromServiceChangesAndReentry;InventoryAlignmentPlayModeTests;OutOfEnergyAlignmentPlayModeTests'),
 ('Runtime','PlayMode','Match3RuntimePlayModeTests;CoreLoopVfxBoosterFeedbackPlayModeTests.SeaPearl_GrantsFiveMovesWithoutBoardOrUnrelatedSaveMutation;CoreLoopVfxBoosterFeedbackPlayModeTests.Recovery_ClearsBoosterVfxSuppressesCompletionAndRefundsOnce'),
 ('Retained','PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 ('Journey','PlayMode','FunctionalDemoNavigationPlayModeTests.EnergyClockCatchupIsCappedAndNotRepeated;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 ('TutorialFunnel','PlayMode','BalanceTelemetryTutorialFunnelPlayModeTests;Chapter01MechanicCurvePlayModeTests'),
 ('Edit','EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;CampaignBoosterTests;OutOfEnergyAlignmentEditModeTests;TutorialAlignmentEditModeTests;BalanceTelemetryTutorialFunnelEditModeTests')]
for name,mode,filter in runs:
 print('Running '+name,flush=True)
 result=subprocess.run([sys.executable,str(BASE/'Sources/Run.py'),name,mode,filter],capture_output=True)
 (BASE/'QA'/name/'launcher.txt').write_bytes(result.stdout+result.stderr)
 if result.returncode:
  print(result.stdout.decode(errors='replace')[-5000:],flush=True);raise RuntimeError(name+' failed; evidence retained')
 s=json.loads((BASE/'QA'/name/'summary.json').read_text());print(json.dumps({'group':name,'result':s['result']['result'],'passed':s['result']['passed'],'failed':s['result']['failed'],'skipped':s['result']['skipped']}),flush=True)
