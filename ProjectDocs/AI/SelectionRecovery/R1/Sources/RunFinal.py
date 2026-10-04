from pathlib import Path
import json,subprocess,sys
BASE=Path(__file__).resolve().parent.parent
runs=[
 ('Core','PlayMode','LevelSystemRuntimePlayModeTests;VictoryAlignmentPlayModeTests;DailyRewardAlignmentPlayModeTests'),
 ('Navigation','PlayMode','ChapterMapAlignmentPlayModeTests;WorldMapAlignmentPlayModeTests;PreLevelAlignmentPlayModeTests;HomeAlignmentPlayModeTests.LiveHud_RefreshesFromServiceChangesAndReentry;ErrorDialogAlignmentPlayModeTests;FunctionalDemoNavigationPlayModeTests.NativeJourneyWinsAndReturnsHome;FunctionalDemoNavigationPlayModeTests.NativeJourneyFailsRetriesAndExitsHome'),
 ('Retained','PlayMode','ContinuationTransactionPlayModeTests;RetainedRoundPlayModeTests'),
 ('Edit','EditMode','SaveJournalWriteEditModeTests;SaveJournalLoadEditModeTests;PlayerSaveMigrationRegistryEditModeTests;CampaignBoosterTests;OutOfEnergyAlignmentEditModeTests;ChapterMapAlignmentEditModeTests;PreLevelAlignmentEditModeTests')]
for name,mode,filter in runs:
 print('Running '+name,flush=True)
 r=subprocess.run([sys.executable,str(BASE/'Sources/Run.py'),name,mode,filter],capture_output=True)
 (BASE/'QA'/name/'launcher.txt').write_bytes(r.stdout+r.stderr)
 if r.returncode:
  print(r.stdout.decode(errors='replace')[-5500:],flush=True);raise RuntimeError(name+' failed; evidence retained')
 s=json.loads((BASE/'QA'/name/'summary.json').read_text());print(json.dumps({'group':name,'result':s['result']['result'],'passed':s['result']['passed'],'failed':s['result']['failed'],'skipped':s['result']['skipped']}),flush=True)
