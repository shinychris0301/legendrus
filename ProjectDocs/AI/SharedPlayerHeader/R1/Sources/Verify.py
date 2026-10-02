from pathlib import Path
import json,os,subprocess,sys
root=Path.cwd();out=Path('/tmp/lor-shared-header-r1')/sys.argv[1]
config=str(out/'test-config');over={'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config,'LOR_WORLDMAP_CAPTURE_DIR':str(out/'WorldMap'),'LOR_HOME_CAPTURE_DIR':str(out/'Home')};env=os.environ.copy();env.update(over)
for phase,fil in [('EditMode','LegendsOfRus.Tests.ProductionCoreLoopUi.WorldMapAlignmentEditModeTests;LegendsOfRus.Tests.ProductionCoreLoopUi.HomeAlignmentDeliveryEditModeTests'),('PlayMode','LegendsOfRus.Tests.PlayMode.WorldMapAlignmentPlayModeTests;LegendsOfRus.Tests.PlayMode.HomeAlignmentPlayModeTests')]:
    d=out/phase;d.mkdir(parents=True,exist_ok=True)
    cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests','-testPlatform',phase,'-testFilter',fil,'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
    with(d/'launcher.log').open('w')as f:code=subprocess.call(cmd,env=env,stdout=f,stderr=subprocess.STDOUT)
    (d/'Command.json').write_text(json.dumps({'command':cmd,'environmentOverrides':over,'exitCode':code},indent=2));print(phase,code,flush=True)
