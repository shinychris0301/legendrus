from pathlib import Path
import subprocess,resource,json,hashlib,os
base=Path(__file__).resolve().parent.parent;root=base.parents[3];unity=Path('/home/k2/Unity/Hub/Editor/6000.5.9f1/Editor/Data');mono=unity/'MonoBleedingEdge/bin/mono';sources=sorted((root/'Assets/LegendsOfRus/Scripts/GameplayCore').glob('*.cs'));out=Path('/tmp/lor-r12-golden.exe')
def limits():resource.setrlimit(resource.RLIMIT_NOFILE,(4096,resource.getrlimit(resource.RLIMIT_NOFILE)[1]))
cmd=[str(mono),str(unity/'MonoBleedingEdge/lib/mono/4.5/csc.exe'),'-nologo','-out:'+str(out),'-r:'+str(unity/'Managed/UnityEngine/UnityEngine.CoreModule.dll'),'-r:'+str(unity/'MonoBleedingEdge/lib/mono/unityjit-linux/Facades/netstandard.dll')]+[str(p) for p in sources]+[str(base/'Sources/LegacyGolden.cs')]
r=subprocess.run(cmd,capture_output=True,text=True,preexec_fn=limits);print(r.stdout,r.stderr);r.check_returncode()
env=os.environ.copy();env['MONO_PATH']=str(unity/'Managed/UnityEngine')
r=subprocess.run([str(mono),str(out)],capture_output=True,text=True,preexec_fn=limits,env=env);(base/'QA/LegacyGolden.tsv').write_text(r.stdout+r.stderr);r.check_returncode();print("Recorded legacy plans:",len(r.stdout.splitlines()))
(base/'QA/SourceHashes.json').write_text(json.dumps({str(p.relative_to(root)):hashlib.sha256(p.read_bytes()).hexdigest() for p in sources},indent=2))
