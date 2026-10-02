from pathlib import Path
import os,sys,json,subprocess,time
out=Path('/tmp/lor-navigation-r1')/sys.argv[1];out.mkdir(exist_ok=False);env=os.environ.copy();over={'XDG_CONFIG_HOME':str(out/'test-config'),'LOR_LOADING_TEST_CONFIG_ROOT':str(out/'test-config')}
for key,name in [('HOME','Home'),('WORLDMAP','WorldMap'),('CHAPTERMAP','ChapterMap'),('PRELEVEL','PreLevel'),('FAIL','Fail'),('SHOP','Shop'),('SPECIAL_OFFER','SpecialOffer'),('DAILY_REWARD','DailyReward')]:over['LOR_'+key+'_CAPTURE_DIR']=str(out/name)
env.update(over)
pages=['WorldMap','ChapterMap','PreLevel','Fail','Shop','SpecialOffer','DailyReward']
edit=['LegendsOfRus.Tests.ProductionCoreLoopUi.'+p+'AlignmentEditModeTests' for p in pages]
play=['LegendsOfRus.Tests.PlayMode.'+p+'AlignmentPlayModeTests' for p in pages]
play += ['LegendsOfRus.Tests.PlayMode.HomeAlignmentPlayModeTests.'+m for m in ['PreparedHome_IsSerializedAndUsesCurrentResourceContract','LiveHud_RefreshesFromServiceChangesAndReentry','VisibleControls_RouteNineActionsAndThreeResourceButtonsOnceFromTallSafeArea','HomePortraits_RenderLivePageAndKeepControlsInsideSafeArea']]
for mode,filters in [('EditMode',edit),('PlayMode',play)]:
 d=out/mode;d.mkdir();cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(Path.cwd()),'-runTests','-testPlatform',mode,'-testFilter',';'.join(filters),'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
 record={'command':cmd,'environmentOverrides':over,'start':time.time()}
 with (d/'launcher.log').open('w') as log:code=subprocess.call(cmd,env=env,stdout=log,stderr=subprocess.STDOUT)
 record.update(exitCode=code,end=time.time());(d/'Command.json').write_text(json.dumps(record,indent=2));print(mode,code,flush=True)
