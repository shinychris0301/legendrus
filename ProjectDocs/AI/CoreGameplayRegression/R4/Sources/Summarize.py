from pathlib import Path
import json
import xml.etree.ElementTree as ET
base = Path(__file__).resolve().parent.parent
runs = []
latest = {}
for path in sorted((base / "QA").glob("*/summary.json"), key=lambda p: p.stat().st_mtime):
    result_path = path.parent / "results.xml"
    if not result_path.exists():
        continue
    tree = ET.parse(result_path)
    info = json.loads(path.read_text())
    run = {"name": path.parent.name, "exitCode": info["exitCode"], "result": tree.getroot().attrib,
           "nonPassing": []}
    for case in tree.findall(".//test-case"):
        item = {"name": case.get("fullname"), "result": case.get("result"), "run": run["name"]}
        message = case.findtext("failure/message") or case.findtext("reason/message")
        if message: item["message"] = message
        if case.get("result") != "Passed": run["nonPassing"].append(item)
        latest[item["name"]] = item
    runs.append(run)
counts = {state: sum(x["result"] == state for x in latest.values()) for state in sorted({x["result"] for x in latest.values()})}
report = {"countsAreLatestDistinctCasesNotOneCombinedRun": True, "latestCounts": counts,
          "runs": runs, "latestNonPassing": [x for x in latest.values() if x["result"] != "Passed"]}
(base / "QA/RunInventory.json").write_text(json.dumps(report, indent=2) + "\n")
(base / "QA/LatestCases.json").write_text(json.dumps(list(latest.values()), indent=2) + "\n")
print(json.dumps({"latestCounts": counts, "latestNonPassing": report["latestNonPassing"]}, indent=2))
