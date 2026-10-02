from pathlib import Path
import subprocess,tempfile,json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
player=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/PlaybackEngines/AndroidPlayer')
java=player/'OpenJDK/bin';sources=root/'Assets/LegendsOfRus/Plugins/AndroidCredentials'
with tempfile.TemporaryDirectory(prefix='lor-credential-compile-') as out:
    commands=[
        [str(java/'javac'),'-source','8','-target','8','-classpath',str(player/'SDK/platforms/android-36/android.jar'),'-d',out,str(sources/'CredentialVault.java'),str(sources/'AndroidCredentialBridge.java')],
        [str(java/'javac'),'-source','8','-target','8','-classpath',out,'-d',out,str(base/'Sources/CredentialVaultHostTests.java')],
        [str(java/'java'),'-cp',out,'com.legendsofrus.credentials.CredentialVaultHostTests']]
    logs=[];results=[]
    for command in commands:
        run=subprocess.run(command,text=True,stdout=subprocess.PIPE,stderr=subprocess.STDOUT)
        logs.append(run.stdout);results.append({'command':command,'exitCode':run.returncode})
        if run.returncode:break
    (base/'QA/JavaVerification.log').write_text('\n'.join(logs))
    record={'stages':results,'androidApi':36,'hostTests':17 if len(results)==3 and results[-1]['exitCode']==0 else 0,'deviceKeystoreVerified':False,'androidAtomicFileExecuted':False}
    (base/'QA/JavaVerification.json').write_text(json.dumps(record,indent=2))
    print(json.dumps(record,indent=2))
    if any(r['exitCode']!=0 for r in results):print('\n'.join(logs));raise SystemExit(1)
