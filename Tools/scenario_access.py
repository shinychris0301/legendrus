#!/usr/bin/env python3
"""Local Scenario credential setup and read-only access check; standard library only."""

import argparse
import base64
import getpass
import json
import os
from pathlib import Path
import stat
import sys
import tempfile
import urllib.error
import urllib.request
import warnings


CREDENTIAL_FILE = Path.home() / ".config/legends-of-rus/scenario/credentials.json"
MODELS_URL = "https://api.cloud.scenario.com/v1/models"


class SetupError(Exception):
    """Messages are fixed strings and must never contain credential values."""


class NoRedirects(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        # Do not forward Basic credentials to any redirect destination.
        return None


def validate_pair(key, secret):
    if not all(isinstance(v, str) and v and not any(c.isspace() for c in v)
               for v in (key, secret)) or ":" in key:
        raise SetupError("Both values must be nonempty, without whitespace; the key cannot contain a colon.")
    return key, secret


def private_directory():
    directory = CREDENTIAL_FILE.parent
    directory.mkdir(mode=0o700, parents=True, exist_ok=True)
    info = directory.lstat()
    if (not stat.S_ISDIR(info.st_mode) or info.st_uid != os.getuid()
            or stat.S_IMODE(info.st_mode) & 0o077):
        raise SetupError("Credential directory must be owned by you, not a symlink, and have mode 700.")
    return directory


def setup(replace=False):
    if not sys.stdin.isatty():
        raise SetupError("Run setup yourself in an interactive terminal; do not pipe credentials into it.")
    directory = private_directory()
    if CREDENTIAL_FILE.is_symlink():
        raise SetupError("Refusing to replace a credential-file symlink.")
    if CREDENTIAL_FILE.exists() and not replace:
        raise SetupError("Credentials already exist. Use check, or setup --replace to enter a new pair.")
    print("Paste each value at its prompt; input is hidden and is not added to shell history.")
    with warnings.catch_warnings():
        warnings.simplefilter("error", getpass.GetPassWarning)
        try:
            key = getpass.getpass("Scenario API key: ")
            secret = getpass.getpass("Scenario API secret: ")
        except getpass.GetPassWarning:
            raise SetupError("This terminal cannot hide input. Use a normal interactive terminal.") from None
    key, secret = validate_pair(key.strip(), secret.strip())
    fd, temporary = tempfile.mkstemp(prefix=".credentials-", dir=directory)
    try:
        with os.fdopen(fd, "w", encoding="utf-8") as output:
            json.dump({"api_key": key, "api_secret": secret}, output)
            output.write("\n")
        os.replace(temporary, CREDENTIAL_FILE)
    finally:
        if os.path.exists(temporary):
            os.unlink(temporary)
    print("Saved outside the project: ~/.config/legends-of-rus/scenario/credentials.json")
    print("File permissions: 600 (owner only). Storage is local plaintext, not encrypted.")
    print("Next: python3 Tools/scenario_access.py check")
    return 0


def load_credentials():
    # Read only inside this process, never echo values or include them in exceptions.
    try:
        fd = os.open(CREDENTIAL_FILE, os.O_RDONLY | os.O_NOFOLLOW | os.O_NONBLOCK)
    except FileNotFoundError:
        raise SetupError("Credentials are not configured. Run: python3 Tools/scenario_access.py setup") from None
    try:
        info = os.fstat(fd)
        if (not stat.S_ISREG(info.st_mode) or info.st_uid != os.getuid()
                or stat.S_IMODE(info.st_mode) != 0o600):
            raise SetupError("Credential file must be a regular file owned by you with mode 600.")
        with os.fdopen(fd, "r", encoding="utf-8") as source:
            fd = None
            data = json.load(source)
        if not isinstance(data, dict):
            raise SetupError("Credential file format is invalid; run setup --replace.")
        return validate_pair(data.get("api_key"), data.get("api_secret"))
    except (ValueError, UnicodeError):
        raise SetupError("Credential file format is invalid; run setup --replace.") from None
    finally:
        if fd is not None:
            os.close(fd)


def check():
    key, secret = load_credentials()
    token = base64.b64encode((key + ":" + secret).encode("utf-8")).decode("ascii")
    request = urllib.request.Request(MODELS_URL, headers={
        "Authorization": "Basic " + token,
        "Accept": "application/json",
        "User-Agent": "LegendsOfRus-ScenarioAccess/1",
    }, method="GET")
    opener = urllib.request.build_opener(NoRedirects())
    try:
        with opener.open(request, timeout=25) as response:
            if response.status != 200:
                print("Unexpected response; access has not been verified.")
                return 1
            # Suppress response bodies, which may contain private project/account data.
        print("PASS: Scenario returned HTTP 200 for GET /v1/models.")
        print("Model-list access verified. No upload or generation was requested.")
        print("Generation permissions, credits and individual model access are not yet verified.")
        return 0
    except urllib.error.HTTPError as error:
        messages = {
            401: "Authentication rejected. Check the key/secret pair or whether the key was revoked.",
            403: "Access forbidden. Check project selection, Models Read/Run scope and account restrictions.",
            429: "Rate limit reached. Wait and retry the check.",
        }
        print("FAIL: HTTP " + str(error.code) + ". " + messages.get(
            error.code, "Unexpected API response; no response body was printed."))
        error.close()
        return 1
    except (urllib.error.URLError, TimeoutError, OSError):
        print("FAIL: Network/TLS connection could not be completed. Check connectivity or sandbox access.")
        return 1


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    setup_command = commands.add_parser("setup", help="Enter credentials privately in your terminal")
    setup_command.add_argument("--replace", action="store_true", help="Replace a previously saved pair")
    commands.add_parser("check", help="Read-only GET /v1/models; no asset generation")
    args = parser.parse_args()
    try:
        return setup(args.replace) if args.command == "setup" else check()
    except SetupError as error:
        print("ERROR: " + str(error), file=sys.stderr)
        return 1
    except (OSError, EOFError, KeyboardInterrupt):
        print("ERROR: Local setup/read was unavailable or cancelled; credential details suppressed.", file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
