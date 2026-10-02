from pathlib import Path
import sys,json
sys.path.insert(0,str(Path.cwd()/"Tools"))
import scenario_fail_r2 as batch
batch.BASE=Path(__file__).resolve().parent
# Select an explicit pair by using a temporary config in its own source folder.
config=json.loads((batch.BASE/"ScenarioRequests.json").read_text())
if sys.argv[1]=="submit":
 for asset in config["assets"][int(sys.argv[2]):int(sys.argv[2])+2]:
  folder=batch.BASE/asset["name"];folder.mkdir(exist_ok=True)
  (folder/"ScenarioRequests.json").write_text(json.dumps({**config,"assets":[asset]},indent=2)+"\n")
  batch.BASE=folder;batch.run("submit");batch.BASE=folder.parent
else:
 for asset in config["assets"]:
  folder=batch.BASE/asset["name"]
  if folder.exists():batch.BASE=folder;batch.run("collect");batch.BASE=folder.parent
