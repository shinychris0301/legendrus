"""Bounded Failure R10 art requests; each asset submits at most once and resumes by job ID."""
import argparse
import json
from pathlib import Path
import time
import re
import urllib.error
import scenario_trial as trial

ROOT = Path.cwd()
BASE = ROOT / "ProjectDocs/AI/FailStyleAlignment/ReferenceFinishR10/Sources"


def classify_limit(error):
    raw=json.loads(error.read(16384))
    safe={}
    for key in ("message","error","code"):
        value=raw.get(key)
        if isinstance(value,str):
            safe[key]=re.sub(r"(?:https?://\S+|[A-Za-z0-9_+/=-]{24,})", "[redacted]", value)[:500]
    trial.save("RateLimitReason.json",safe)
    message=json.dumps(raw).lower()
    # Record only recognized classifications, never response content or account identifiers.
    words=["concurrent", "credit", "compute", "quota", "insufficient", "rate", "minute", "hour", "day", "billing", "plan", "cooldown", "subscription", "balance", "simultaneous", "maximum", "parallel", "fast", "slow", "limit", "generation", "inference", "active", "queue", "allowed", "upgrade", "subscription", "exceed", "process"]
    return [word for word in words if word in message]


def run(action):
    config = json.loads((BASE / "ScenarioRequests.json").read_text())
    for asset in config["assets"]:
        folder = BASE / "Scenario" / asset["name"]
        folder.mkdir(parents=True, exist_ok=True)
        trial.OUT = folder
        trial.MODEL = config["modelId"]
        if action == "submit":
            if (folder / "Job.json").exists():
                print(asset["name"], "job already recorded", flush=True)
                continue
            trial.save("Request.json", {"modelId": trial.MODEL, "body": asset["body"]})
            with (folder / "SubmissionAttempt.json").open("x") as f:
                json.dump({"submittedUnix": time.time(), "automaticResubmission": False}, f)
            try:
                response = trial.api("/generate/custom/" + trial.MODEL, asset["body"])
            except urllib.error.HTTPError as error:
                if error.code == 429:
                    trial.save("RateLimit.json", {"time": time.time(), "status":429, "retryAfter": error.headers.get("Retry-After"), "limit":error.headers.get("X-RateLimit-Limit"), "reset":error.headers.get("X-RateLimit-Reset"), "categories": classify_limit(error)})
                    # Explicit rejection: no job accepted. Retain evidence and retry later.
                    (folder / "SubmissionAttempt.json").rename(folder / ("Rejected429_" + str(time.time_ns()) + ".json"))
                raise
            job = response.get("job", response)
            job_id = job.get("jobId") or job.get("id")
            if not job_id:
                raise trial.SetupError("No job identifier: recover existing request, never resubmit.")
            trial.save("Job.json", {"id": job_id, "status": job.get("status")})
            print(asset["name"], "submitted once", flush=True)
        else:
            if not (folder / "Job.json").exists():
                continue
            destination = folder / (asset["name"] + ".png")
            if destination.exists():
                print(asset["name"], "already downloaded", flush=True)
                continue
            trial.poll()
            state = json.loads((folder / "Status.json").read_text())
            if state["status"] == "success":
                trial.download()
                (folder / "booster_medallion_trial01.png").rename(destination)
                output = json.loads((folder / "Output.json").read_text())
                output["file"] = destination.name
                trial.save("Output.json", output)
                print(asset["name"], "ready", flush=True)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("action", choices=["submit", "collect"])
    try:
        run(parser.parse_args().action)
    except urllib.error.HTTPError as e:
        print("HTTP", e.code, "details suppressed; no automatic retry")
        raise SystemExit(1)
    except (OSError, ValueError, KeyError, trial.SetupError):
        print("Request or data error; details suppressed. Never clear a submission marker to retry blindly.")
        raise SystemExit(1)
