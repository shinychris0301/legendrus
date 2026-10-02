from pathlib import Path
import json, subprocess, sys
base = Path(__file__).resolve().parent
plan = json.loads((base / "PhasePlan.json").read_text())
for step in plan:
    code = subprocess.call([sys.executable, str(base / "Verify.py"), step["name"], "PlayMode", step["filter"], step["options"]])
    summary = json.loads((base.parent / "QA" / step["name"] / "summary.json").read_text())
    if code or summary.get("exitCode") or summary.get("result", {}).get("result") != "Passed":
        raise SystemExit("Phase failed: " + step["name"])
print("All ten dedicated process phases passed.", flush=True)
