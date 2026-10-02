from pathlib import Path
import json,os,sys,time,subprocess
root=Path.cwd();base=Path('/tmp/lor-rewardreceived-r1');out=base/sys.argv[1];out.mkdir(exist_ok=False)
config=str(out/'test-config');over={'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config,'LOR_REWARD_RECEIVED_CAPTURE_DIR':str(out/'RewardReceived'),'LOR_DAILY_REWARD_CAPTURE_DIR':str(out/'DailyReward'),'LOR_SHOP_CAPTURE_DIR':str(out/'Shop'),'LOR_HOME_CAPTURE_DIR':str(out/'Home')};env=os.environ.copy();env.update(over)
steps=[('Import',['-executeMethod','LegendsOfRus.Editor.ProductionRewardReceivedPrefabBuilder.Rebuild','-quit']),('EditMode',['-runTests','-testPlatform','EditMode','-testFilter',';'.join('LegendsOfRus.Tests.ProductionCoreLoopUi.'+x+'AlignmentEditModeTests' for x in ['RewardReceived','DailyReward','Shop'])]),('PlayMode',['-runTests','-testPlatform','PlayMode','-testFilter',';'.join('LegendsOfRus.Tests.PlayMode.'+x+'AlignmentPlayModeTests' for x in ['RewardReceived','DailyReward','Shop'])+';LegendsOfRus.Tests.PlayMode.HomeAlignmentPlayModeTests.QuestUnavailableAndDailyRewardBadgeReflectRealPersistedState'])]
for name,args in steps:
 d=out/name;d.mkdir();cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),*args,'-logFile',str(d/'unity.log')]
 if name!='Import':cmd+=['-testResults',str(d/'results.xml')]
 record={'command':cmd,'environmentOverrides':over,'start':time.time()}
 with (d/'launcher.log').open('w') as f:code=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
 record.update(exitCode=code,end=time.time());(d/'Command.json').write_text(json.dumps(record,indent=2));print(name,code,flush=True)
 if name=='Import' and code:raise SystemExit(code)
