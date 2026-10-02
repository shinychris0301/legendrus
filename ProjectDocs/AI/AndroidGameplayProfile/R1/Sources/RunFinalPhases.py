from pathlib import Path
import subprocess,sys,json
base=Path(__file__).resolve().parent
for name in json.loads((base/'FinalPhasePlan.json').read_text()):
 subprocess.run([sys.executable,str(base/'Verify.py'),name,'PlayMode','LegendsOfRus.Tests.PlayMode.DevelopmentGameplayProfilePlayModeTests',str(base/(name+'.json'))],check=True)
