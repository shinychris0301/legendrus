from pathlib import Path
import json
import subprocess

ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent
mono=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data/MonoBleedingEdge/bin/mono');csc=mono.parent.parent/'lib/mono/4.5/csc.exe'
profile=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Services/DevelopmentGameplayProfile.cs'
factory=profile.with_name('DevelopmentGameplayFixtures.cs');badge=profile.with_name('DevelopmentGameplayProfileBadge.cs')
out=BASE/'QA/EntryGuards';out.mkdir(exist_ok=False)
presets=['stock-999','stock-1000','stock-9999','stock-max','story-early','story-finale']
cases=['normal','fresh','resume','empty-energy','unknown','missing-owner','wrong-owner','wrong-package','not-debuggable','jni-failure','write-failure','fresh-foreign']
cases+=presets+[prefix+name for prefix in ['owned-','foreign-','emptyowner-','preparefail-','contentfail-','commitfail-','markerfail-'] for name in presets]
records=[]
for mode,defines in [('Development','UNITY_ANDROID,DEVELOPMENT_BUILD'),('Release','UNITY_ANDROID')]:
    exe=out/(mode+'.exe')
    command=[str(mono),str(csc),'-nologo','-out:'+str(exe),'-define:'+defines,str(BASE/'Sources/EntryGuardHarness.cs'),str(profile),str(factory)]
    if mode=='Release':command.append(str(badge))
    compiled=subprocess.run(command,capture_output=True,text=True);(out/(mode+'Compile.log')).write_text(compiled.stdout+compiled.stderr)
    assert compiled.returncode==0,compiled.stdout+compiled.stderr
    for case in cases:
        run=subprocess.run([str(mono),str(exe),case],capture_output=True,text=True);(out/(mode+'-'+case+'.log')).write_text(run.stdout+run.stderr)
        assert run.returncode==0,(mode,case,run.stdout,run.stderr)
        records.append({'mode':mode,'scenario':case,'passed':True})
result={'passed':len(records),'failed':0,'checks':records,'actualProfileAndFactorySourceCompiled':True,
        'fakeJniContentAndPreferences':True,'realAndroidExecution':False,'actualUnityReleaseBuild':False}
(out/'summary.json').write_text(json.dumps(result,indent=2)+'\n');print(str(len(records))+' actual-source compiled entry/factory guard checks passed (fake JNI/content/preferences).')
