"""Run separate Unity writer/reader processes, then isolated lifecycle/save regression."""
from pathlib import Path
import os,sys,json,subprocess,time,xml.etree.ElementTree as ET
root=Path.cwd()
assert len(sys.argv)==2 and Path(sys.argv[1]).name==sys.argv[1] and sys.argv[1] not in ('.','..')
out=Path('/tmp/lor-text-clarity-r1')/sys.argv[1]
out.mkdir(parents=True,exist_ok=False)
fixture='LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.'
# Discover the fixture's regression tests, excluding the process-specific phases.
import re
source=(root/'Assets/LegendsOfRus/Tests/PlayMode/FunctionalDemoNavigationPlayModeTests.cs').read_text()
regression=re.findall(r'public (?:IEnumerator|void) (\w+)\(', source)
regression=[x for x in regression if x not in ('SetUp','TearDown','PersistenceWriterCompletesAndSaves','PersistenceReaderRestoresWithoutDuplicateReward')]
steps=[('Write',fixture+'PersistenceWriterCompletesAndSaves','write'),('Read',fixture+'PersistenceReaderRestoresWithoutDuplicateReward','read'),('Regression',';'.join([fixture+x for x in regression]+['LegendsOfRus.Tests.PlayMode.SaveMigrationRecoveryPlayModeTests','LegendsOfRus.Tests.PlayMode.AudioHapticsFidelityVerificationPlayModeTests.AdaptiveCanvasScaler_ApplyUsesSafeAreaFitScaleAndCrossover','LegendsOfRus.Tests.PlayMode.AudioHapticsFidelityVerificationPlayModeTests.AdaptiveCanvasScaler_RefreshesFixedTextOnlyWhenActualScaleChanges']),'regression')]
for name,filter_,phase in steps:
    folder=out/name;folder.mkdir()
    config=str(out/('regression-config' if phase=='regression' else 'persistent-config'))
    overrides={'XDG_CONFIG_HOME':config,'LOR_LOADING_TEST_CONFIG_ROOT':config,'LOR_PERSISTENCE_PHASE':phase,'LOR_PERSISTENCE_CHECKPOINT':str(out/'checkpoint.json'),'LOR_JOURNEY_CAPTURE_DIR':str(folder/'Captures')}
    env=os.environ.copy();env.update(overrides)
    cmd=['/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Unity','-batchmode','-projectPath',str(root),'-runTests','-testPlatform','PlayMode','-testFilter',filter_,'-testResults',str(folder/'results.xml'),'-logFile',str(folder/'unity.log')]
    record={'command':cmd,'environmentOverrides':overrides,'startUnix':time.time()}
    with (folder/'launcher.log').open('w') as log:
        record['exitCode']=subprocess.call(cmd,env=env,stdout=log,stderr=subprocess.STDOUT,start_new_session=True)
    record['endUnix']=time.time();(folder/'command.json').write_text(json.dumps(record,indent=2))
    print(name,record['exitCode'],flush=True)
    if record['exitCode']:raise SystemExit(record['exitCode'])
    result=ET.parse(folder/'results.xml').getroot().attrib
    print({k:result[k] for k in ('result','total','passed','failed','skipped')},flush=True)
    if result['result']!='Passed' or result['skipped']!='0':raise SystemExit(1)
