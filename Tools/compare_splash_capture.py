#!/usr/bin/env python3
"""Compare an actual, reference-sized capture without silently stretching or aligning it.
Dependency: Pillow. Outputs are diagnostic comparisons, never an automatic visual approval.
"""
from __future__ import annotations
import argparse, json, sys
from pathlib import Path
from PIL import Image, ImageChops, ImageDraw, ImageStat

def main() -> int:
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument("--capture",type=Path,required=True)
    ap.add_argument("--reference",type=Path,default=Path(__file__).resolve().parents[1]/"Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/SplashTitle.png")
    ap.add_argument("--output",type=Path,required=True)
    args=ap.parse_args()
    try:
        ref=Image.open(args.reference).convert("RGB");cap=Image.open(args.capture).convert("RGB")
        if ref.size!=cap.size:
            raise ValueError(f"Dimensions differ: reference {ref.size}, capture {cap.size}. Capture 941x1672 for reference comparison; do not distort taller device screenshots.")
        files=["comparison.png","overlay.png","absolute_difference.png","metrics.json"]
        if any((args.output/f).exists() for f in files):raise FileExistsError("Output files already exist. Choose a new output folder.")
        args.output.mkdir(parents=True,exist_ok=True)
        w,h=ref.size;sheet=Image.new("RGB",(2*w,h+45),(20,25,35));d=ImageDraw.Draw(sheet)
        d.text((15,15),"REFERENCE",fill="white");d.text((w+15,15),"SUPPLIED CAPTURE — inspect visually",fill="white")
        sheet.paste(ref,(0,45));sheet.paste(cap,(w,45));sheet.save(args.output/"comparison.png")
        Image.blend(ref,cap,.5).save(args.output/"overlay.png")
        diff=ImageChops.difference(ref,cap);diff.save(args.output/"absolute_difference.png")
        stat=ImageStat.Stat(diff)
        result={"capture":str(args.capture),"reference":str(args.reference),"size":ref.size,
                "mean_absolute_difference_rgb_0_255":stat.mean,
                "rms_difference_rgb_0_255":stat.rms,
                "visual_match_verified":False,"note":"Metrics are not a production signoff. Inspect typography, silhouettes, placement, and contamination separately."}
        (args.output/"metrics.json").write_text(json.dumps(result,indent=2),encoding="utf-8")
        print(str(args.output.resolve()));return 0
    except (OSError,ValueError) as exc:
        print(str(exc),file=sys.stderr);return 2
if __name__=="__main__":raise SystemExit(main())
