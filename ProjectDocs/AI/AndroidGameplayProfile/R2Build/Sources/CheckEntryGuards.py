from pathlib import Path
import subprocess,json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
mono=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/MonoBleedingEdge/bin/mono')
csc=mono.parent.parent/'lib/mono/4.5/csc.exe'
profile=root/'Assets/LegendsOfRus/Scripts/Runtime/Services/DevelopmentGameplayProfile.cs'
badge=profile.with_name('DevelopmentGameplayProfileBadge.cs')
qa=base/'QA/EntryGuards';qa.mkdir(exist_ok=False)
records=[]
for mode,defines in [('Development','UNITY_ANDROID,DEVELOPMENT_BUILD'),('Release','UNITY_ANDROID')]:
 exe=qa/(mode+'.exe')
 cmd=[str(mono),str(csc),'-nologo','-out:'+str(exe),'-define:'+defines,str(base/'Sources/EntryGuardHarness.cs'),str(profile)]
 if mode=='Release':cmd.append(str(badge))
 compiled=subprocess.run(cmd,stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True)
 (qa/(mode+'Compile.log')).write_text(compiled.stdout);assert compiled.returncode==0,compiled.stdout
 for scenario in ['normal','fresh','resume','empty-energy','unknown','missing-owner','wrong-owner','wrong-package','not-debuggable','jni-failure','write-failure']:
  p=subprocess.run([str(mono),str(exe),scenario],stdout=subprocess.PIPE,stderr=subprocess.STDOUT,text=True)
  (qa/(mode+'-'+scenario+'.log')).write_text(p.stdout)
  assert p.returncode==0,mode+' '+scenario+' failed'
  records.append({'mode':mode,'scenario':scenario,'passed':True})
(qa/'summary.json').write_text(json.dumps({'checks':records,'passed':len(records),'failed':0,'actualProfileSourceCompiled':True,'fakeJniAndPreferences':True,'realAndroidExecution':False,'actualUnityReleaseBuild':False},indent=2))
print(str(len(records))+' compiled entry-guard checks passed (fake JNI/preferences).')
