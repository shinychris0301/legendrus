from pathlib import Path
import subprocess,sys,json
b=Path(__file__).resolve().parent.parent
for kind in ['Language','Persistence']:
 for phase in ['write','read']:
  name=kind+phase.capitalize()+'01'
  f='LegendsOfRus.Tests.PlayMode.LanguagePreferenceRestartPlayModeTests.DevicePreferenceSurvivesFreshUnityProcess' if kind=='Language' else 'LegendsOfRus.Tests.PlayMode.FunctionalDemoNavigationPlayModeTests.'+('PersistenceWriterCompletesAndSaves' if phase=='write' else 'PersistenceReaderRestoresWithoutDuplicateReward')
  subprocess.run([sys.executable,str(b/'Sources/Verify.py'),name,'PlayMode',f,str(b/('Sources/'+kind+phase+'.json'))],check=True)
  result=json.loads((b/('QA/'+name+'/summary.json')).read_text())
  if result.get('result',{}).get('result')!='Passed':
   print(kind,phase,'failed; dependent reader not run',flush=True);break
