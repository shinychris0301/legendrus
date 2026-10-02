from pathlib import Path
import subprocess,sys
base=Path(__file__).resolve().parent
for name,selection,options in [
 ('NativeJourneys',(base/'JourneyFilter.txt').read_text(),base/'Journeys.json'),
 ('NormalAfterJourneys','LegendsOfRus.Tests.PlayMode.DevelopmentGameplayProfilePlayModeTests',base/'FinalNormalRead.json'),
 ('NormalRegression',(base/'RegressionFilter.txt').read_text(),None)]:
 args=[sys.executable,str(base/'Verify.py'),name,'PlayMode',selection]
 if options:args.append(str(options))
 subprocess.run(args,check=True)
