from pathlib import Path
import subprocess,sys
base=Path(__file__).resolve().parent
for name in ['ProfileWrite','ProfileResume','ProfileReset','EmptyEnergy','NormalRead']:
 subprocess.run([sys.executable,str(base/'Verify.py'),name,'PlayMode','LegendsOfRus.Tests.PlayMode.DevelopmentGameplayProfilePlayModeTests',str(base/(name+'.json'))],check=True)
