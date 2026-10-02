from pathlib import Path
import zipfile,json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
folder=root/'Builds/Verification/v1_17_step01'
with zipfile.ZipFile(folder/'LegendsOfRus-v1.17-step01-development.apk') as z:
 current=json.loads(z.read('assets/bin/Data/RuntimeInitializeOnLoads.json'))['root']
with zipfile.ZipFile(folder/'BeforeGameplayProfileR2/LegendsOfRus-v1.17-step01-development.apk') as z:
 prior=json.loads(z.read('assets/bin/Data/RuntimeInitializeOnLoads.json'))['root']
added=[r for r in current if r not in prior];removed=[r for r in prior if r not in current]
assert not removed and len(added)==1 and added[0]['className']=='DevelopmentGameplayProfile' and added[0]['methodName']=='ResetSession' and added[0]['loadTypes']==4
assert len([r for r in current if r['className']=='RuntimeBootstrap' and r['methodName']=='Boot'])==1
(base/'QA/BootstrapInspection.json').write_text(json.dumps({'added':added,'removed':removed,'existingRuntimeBootCount':1,'runtimeExecutionTested':False},indent=2))
print('One existing RuntimeBootstrap.Boot; only new registration is profile session reset.')
