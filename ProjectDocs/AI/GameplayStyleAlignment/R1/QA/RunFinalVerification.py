"""Unity checks use isolated preferences and fresh per-run output directories."""
from pathlib import Path
import os,json,subprocess,sys
root=Path(__file__).resolve().parents[5];base=Path(sys.argv[1]);base.mkdir(parents=True,exist_ok=True)
prior=json.loads((root/'ProjectDocs/AI/PreLevelStyleAlignment/R1/UnityVerification/Reproduction.json').read_text())
edits=prior['EditModeFixtures']+['LegendsOfRus.Tests.ProductionCoreLoopUi.GameplayAlignmentEditModeTests']
plays=prior['PlayModeFixtures']+['LegendsOfRus.Tests.PlayMode.'+n for n in ['GameplayAlignmentPlayModeTests','Match3RuntimePlayModeTests','LevelSystemRuntimePlayModeTests','LevelGoalRuntimeTests','CoreLoopMotionFeedbackPlayModeTests','CoreLoopVfxBoosterFeedbackPlayModeTests']]
env=os.environ.copy();env.update(XDG_CONFIG_HOME='/tmp/lor-home-r3/test-config',LOR_LOADING_TEST_CONFIG_ROOT='/tmp/lor-home-r3/test-config')
for key,directory in [('HOME','Home'),('SPLASH','Splash'),('LOADING','Loading'),('WORLDMAP','WorldMap'),('CHAPTERMAP','ChapterMap'),('PRELEVEL','PreLevel'),('GAMEPLAY','Gameplay')]:env['LOR_'+key+'_CAPTURE_DIR']=str(base/directory)
records=[]
for mode,fixtures in [('EditMode',edits),('PlayMode',plays)]:
 d=base/mode;d.mkdir(exist_ok=True)
 args=[prior['unityExecutable'],'-batchmode','-projectPath',str(root),'-runTests','-testPlatform',mode,'-testFilter',';'.join(fixtures),'-testResults',str(d/'results.xml'),'-logFile',str(d/'unity.log')]
 (d/'command.json').write_text(json.dumps(args,indent=2)+'\n');print('Starting',mode,len(fixtures),'fixture filters',flush=True)
 code=subprocess.call(args,env=env);records.append({'platform':mode,'exitCode':code,'output':str(d)});print(records[-1],flush=True)
(base/'Runs.json').write_text(json.dumps(records,indent=2)+'\n')
