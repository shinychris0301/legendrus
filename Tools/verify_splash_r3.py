#!/usr/bin/env python3
"""Portable source/asset checks. This is not a Unity compiler or a runtime test."""
from __future__ import annotations
import argparse, hashlib, json, math, re, sys, zipfile
from pathlib import Path

def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--project", type=Path, default=Path(__file__).resolve().parents[1])
    ap.add_argument("--baseline", type=Path, help="Optional original project ZIP for preservation checks")
    ap.add_argument("--report", type=Path, help="Optional JSON result output")
    args = ap.parse_args(); root = args.project.resolve(); results = []
    def check(name: str, ok: bool) -> None:
        results.append({"name": name, "passed": bool(ok)})
        print(("PASS " if ok else "FAIL ") + name)
    doc = root / "ProjectDocs/SplashTitle_R3_Integration"
    try:
        current = root / "ProjectDocs/SplashTitle_R3_FidelityFix02/asset_inventory.json"
        inventory_path = current if current.exists() else doc / "asset_inventory.json"
        inventory = json.loads(inventory_path.read_text(encoding="utf-8"))
        check("Ten active PNG resources recorded", len(inventory) == 10)
        for item in inventory:
            path = root / item["file"]
            check(item["source"] + " active resource hash", path.is_file() and hashlib.sha256(path.read_bytes()).hexdigest() == item["sha256"])
        path = root / "Assets/LegendsOfRus/Resources/ProductionUI/Pages/SplashTitle.prefab"
        prefab = path.read_text(encoding="utf-8")
        ids = re.findall(r"(?m)^--- !u!\d+ &(\d+)", prefab)
        check("Unique serialized fileIDs", len(ids) == len(set(ids)))
        refs = re.findall(r"\{fileID: (\d+)\}", prefab)
        check("Prefab local references resolve", all(x == "0" or x in ids for x in refs))
        for name, guid, count in [("native Buttons", "4e29b1a8efbd4b44bb3f3716e73f07ff", 5),
                                  ("live Text objects", "5f7201a12d95ffc409449d95f23cf332", 8),
                                  ("sprite Images", "fe87c0e1cc204ed48ad3b37840f39efc", 10)]:
            check(f"{count} {name}", prefab.count("guid: " + guid) == count)
        check("Ten aspect-preserved Images", prefab.count("m_PreserveAspect: 1") == 10)
        check("Five input graphics only", prefab.count("m_RaycastTarget: 1") == 5)
        ui = root / "Assets/LegendsOfRus/Scripts/Runtime/UI"
        loader = (ui / "ProductionSplashTitleRuntime.cs").read_text()
        check("Canonical prefab instantiated at runtime", "Object.Instantiate(prefab, parent, false)" in loader and "ProductionUI/Pages/SplashTitle" in loader)
        router = (ui / "ScreenRouter.cs").read_text()
        block = router.split("void BuildInteractionLayer", 1)[1].split("case ScreenId.SplashTitle:", 1)[1].split("case ScreenId.", 1)[0]
        check("No legacy Splash hotspots", "Hotspot(" not in block)
        check("Five existing routes bound", all(x in block for x in ["ScreenId.Loading", "ScreenId.LanguageSelect", "ScreenId.Settings", "ScreenId.Login", "ScreenId.TutorialOverlay"]))
        config = root / "Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig"
        layout = json.loads((config / "SplashTitleLayout.json").read_text())
        for width, height, top, bottom in [(941,1672,0,0),(1080,1920,0,0),(1080,2340,0,0),(1080,2400,0,0),(1080,2400,105,72)]:
            scale = min(width/941, (height-top-bottom)/1672)
            ox = (width-941*scale)/2; oy = top + (height-top-bottom-1672*scale)/2
            valid = True
            for button in layout["buttons"]:
                x,y,w,h = button["rect"]
                valid &= ox+x*scale >= -1e-5 and oy+y*scale >= top-1e-5 and ox+(x+w)*scale <= width+1e-5 and oy+(y+h)*scale <= height-bottom+1e-5
            check(f"Offline safe-area geometry {width}x{height} / {top},{bottom}", valid)
        if args.baseline:
            with zipfile.ZipFile(args.baseline) as archive:
                originals = [n for n in archive.namelist() if not n.endswith("/")]
                protected = [n for n in originals if n.endswith(".meta") or n.startswith(("Packages/", "ProjectSettings/"))]
                check("Baseline protected files unchanged", all((root/n).is_file() and hashlib.sha256((root/n).read_bytes()).digest()==hashlib.sha256(archive.read(n)).digest() for n in protected))
        report = {"checks": results, "passed": all(x["passed"] for x in results),
                  "unity_compilation": "not performed by this tool", "runtime_execution": "not performed by this tool",
                  "exact_font_identity": "requires original font evidence; not inferred from a screenshot",
                  "visual_match": "not verified by source checks"}
        if args.report:
            args.report.parent.mkdir(parents=True,exist_ok=True)
            args.report.write_text(json.dumps(report, indent=2),encoding="utf-8")
        print("Unity compilation, runtime navigation, glyph rendering and reference fidelity still require Editor/device testing.")
        return 0 if report["passed"] else 1
    except (OSError,ValueError,KeyError,IndexError,zipfile.BadZipFile) as exc:
        print("Verification could not complete: " + str(exc),file=sys.stderr);return 2
if __name__ == "__main__":
    raise SystemExit(main())
