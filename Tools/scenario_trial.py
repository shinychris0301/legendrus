#!/usr/bin/env python3
"""One authorized Scenario medallion trial; submit once, poll/download separately."""
import argparse
import base64
import hashlib
import json
from pathlib import Path
import sys
import urllib.error
import urllib.parse
import urllib.request

from scenario_access import load_credentials, NoRedirects, SetupError

ROOT = Path(__file__).resolve().parent.parent
OUT = ROOT / "ProjectDocs/AI/ScenarioAssetWorkflow/Trial01"
REFERENCE = ROOT / "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/Fail.png"
MODEL = "model_openai-gpt-image-2"
BASE = "https://api.cloud.scenario.com/v1"


class HttpsDownloadRedirects(urllib.request.HTTPRedirectHandler):
    """CDN downloads have no auth headers; allow HTTPS-only CDN redirects."""
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        parsed = urllib.parse.urlsplit(newurl)
        if parsed.scheme != "https" or parsed.username or parsed.password:
            raise SetupError("Refusing an insecure CDN redirect.")
        return super().redirect_request(req, fp, code, msg, headers, newurl)


PROMPT = """Create ONE independent empty circular booster medallion UI asset for the Russian fairy-tale game Legends of Rus.
The supplied full-screen image is STYLE REFERENCE ONLY: study the four circular booster medallions along its bottom. Do not reproduce the screen, crop from it, or include any existing booster illustration. Create a new isolated blank medallion matching their restrained antique gold, midnight-navy lacquer and delicate carved rim.
Front view, precisely circular, centered, balanced bilateral symmetry, no perspective. The full medallion including its small top and bottom ornaments fits within a square canvas with 6 percent transparent margin on every side. A relatively slender dimensional antique-gold rim, fine legible engraved leaf scrollwork, tiny restrained sapphire accents at the top and bottom. The rim must not dominate: leave a large round empty central face, at least 74 percent of the medallion diameter, for a separate booster illustration in Unity.
The central face is OPAQUE near-black midnight-navy lacquer, subtle fine texture and a gentle inner shadow; it is NOT a transparent hole. Match the reference's subdued warm gold, dark bronze recesses and navy. Warm upper-left highlights and modest lower-right shadows. Crisp outer silhouette and clear details at 170 pixels displayed size; avoid microscopic decoration, excessive gloss, glowing sparks and oversaturated yellow metal.
Only the EXTERIOR outside the medallion is genuinely transparent alpha. No scenery, checkerboard, solid exterior background, loose shadow outside the rim, lettering, numbers, hero, hammer, firebird, shield, potion, quantity badge, nameplate, extra object, watermark or cropped ornament. Exactly one blank medallion, not a sheet or layout."""


def save(name, data):
    OUT.mkdir(parents=True, exist_ok=True)
    destination = OUT / name
    temporary = destination.with_suffix(destination.suffix + ".tmp")
    temporary.write_text(json.dumps(data, ensure_ascii=False, indent=2) + "\n")
    temporary.replace(destination)


def api(path, body=None):
    if not path.startswith("/") or path.startswith("//"):
        raise SetupError("Invalid API path.")
    key, secret = load_credentials()
    token = base64.b64encode((key + ":" + secret).encode()).decode()
    headers = {"Authorization": "Basic " + token, "Accept": "application/json"}
    if body is not None:
        headers["Content-Type"] = "application/json"
    req = urllib.request.Request(BASE + path, headers=headers,
                                 data=None if body is None else json.dumps(body).encode(),
                                 method="GET" if body is None else "POST")
    with urllib.request.build_opener(NoRedirects()).open(req, timeout=55) as response:
        return json.load(response)


def upload():
    if (OUT / "Reference.json").exists():
        print("Reference upload already recorded; reused.")
        return
    raw = REFERENCE.read_bytes()
    if len(raw) >= 4 * 1024 * 1024:
        raise SetupError("Reference exceeds the simple-upload limit.")
    result = api("/assets", {"name": "LegendsOfRus_Fail_StyleReference_Trial01.png",
                             "image": base64.b64encode(raw).decode()})
    asset = result["asset"]
    save("Reference.json", {"assetId": asset["id"], "path": str(REFERENCE.relative_to(ROOT)),
                            "sha256": hashlib.sha256(raw).hexdigest(),
                            "role": "Full original style reference only; never runtime art"})
    print("Original Failure style reference uploaded; asset ID recorded.")


def submit():
    reference = json.loads((OUT / "Reference.json").read_text())
    body = {"prompt": PROMPT, "referenceImages": [reference["assetId"]],
            "width": 1024, "height": 1024, "numOutputs": 1,
            "quality": "medium", "background": "transparent"}
    save("Request.json", {"modelId": MODEL, "body": body})
    # Atomic marker BEFORE transmission: uncertain responses must never cause a second paid POST.
    with (OUT / "SubmissionAttempt.json").open("x") as f:
        json.dump({"generationRequestsAllowed": 1, "automaticResubmission": False}, f)
    result = api("/generate/custom/" + MODEL, body)
    job = result.get("job", result)
    job_id = job.get("jobId") or job.get("id")
    if not job_id:
        raise SetupError("No job ID in response. Recover the existing request through job history; do not resubmit.")
    save("Job.json", {"id": job_id, "status": job.get("status")})
    print("One generation submitted. Job ID recorded; use poll to resume without generating again.")


def poll():
    saved = json.loads((OUT / "Job.json").read_text())
    result = api("/jobs/" + urllib.parse.quote(saved["id"], safe=""))
    job = result.get("job", result)
    status = job.get("status")
    metadata = job.get("metadata") or {}
    record = {"id": saved["id"], "status": status,
              "assetIds": metadata.get("assetIds", []),
              "metadataKeys": list(metadata), "responseKeys": list(job)}
    # Save only IDs/status/cost fields, not signed URLs, account data or credential headers.
    for k in ("cost", "computeUnits", "credits", "duration", "errorCode"):
        if isinstance(job.get(k), (int, float, str)):
            record[k] = job[k]
    save("Status.json", record)
    if isinstance(job.get("billing"), dict):
        # Only numeric cost fields; never account/owner identifiers.
        save("Billing.json", {k: v for k, v in job["billing"].items()
                              if isinstance(v, (int, float)) and not isinstance(v, bool)})
    print("Generation status:", status)
    if status in ("failure", "failed", "canceled", "cancelled"):
        print("Generation ended without success. No automatic replacement generation.")


def download():
    status = json.loads((OUT / "Status.json").read_text())
    if status["status"] != "success" or len(status["assetIds"]) != 1:
        raise SetupError("Expected a successful job with exactly one output asset; inspect recorded status.")
    asset_id = status["assetIds"][0]
    result = api("/assets/" + urllib.parse.quote(asset_id, safe=""))
    asset = result.get("asset", result)
    url = asset.get("url")
    parsed = urllib.parse.urlsplit(url or "")
    if parsed.scheme != "https" or not parsed.hostname or parsed.username or parsed.password:
        raise SetupError("Expected a HTTPS output asset URL.")
    # Download via a fresh unauthenticated opener. Never send Scenario credentials to a CDN.
    with urllib.request.build_opener(HttpsDownloadRedirects()).open(url, timeout=55) as response:
        raw = response.read(32 * 1024 * 1024 + 1)
    if len(raw) > 32 * 1024 * 1024 or not raw.startswith(b"\x89PNG\r\n\x1a\n"):
        raise SetupError("Output is not a PNG within the download size limit.")
    destination = OUT / "booster_medallion_trial01.png"
    if destination.exists() and destination.read_bytes() != raw:
        raise SetupError("Refusing to overwrite a different local output.")
    destination.write_bytes(raw)
    save("Output.json", {"assetId": asset_id, "file": destination.name,
                         "sha256": hashlib.sha256(raw).hexdigest(), "bytes": len(raw),
                         "downloadHost": parsed.hostname, "modelId": MODEL})
    print("Downloaded one unmodified PNG:", str(destination.relative_to(ROOT)))


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("action", choices=["upload", "submit", "poll", "download"])
    args = parser.parse_args()
    try:
        {"upload": upload, "submit": submit, "poll": poll, "download": download}[args.action]()
        return 0
    except FileExistsError:
        print("STOP: Trial submission was already attempted. Poll the existing job; never auto-resubmit.")
    except urllib.error.HTTPError as error:
        print("Scenario HTTP error:", error.code, "(body and headers suppressed).")
        error.close()
    except SetupError as error:
        print("STOP:", str(error))
    except (OSError, ValueError, KeyError, TypeError, urllib.error.URLError):
        print("STOP: Request/local data unavailable or unexpected response. Details suppressed; no automatic retry.")
    return 1


if __name__ == "__main__":
    sys.exit(main())
