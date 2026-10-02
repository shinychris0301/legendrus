"""Private SQLite online snapshot / restore-to-new-path; never replace a live DB."""
import argparse
from contextlib import closing
import os
from pathlib import Path
import sqlite3
import tempfile


def validate(db):
    if db.execute("PRAGMA user_version").fetchone()[0] != 2:
        raise ValueError("Unsupported snapshot schema")
    if db.execute("PRAGMA integrity_check").fetchone()[0] != "ok":
        raise ValueError("Snapshot integrity failed")
    if db.execute("PRAGMA foreign_key_check").fetchone() is not None:
        raise ValueError("Snapshot references failed")
    for table in ("players", "sessions", "orders", "grants", "auth_families", "refresh_credentials"):
        db.execute('SELECT 1 FROM "' + table + '" LIMIT 1').fetchone()


def snapshot(source, destination):
    source, destination = Path(source), Path(destination)
    if not source.is_absolute() or not source.is_file() or source.is_symlink():
        raise ValueError("Existing absolute source required")
    if not destination.is_absolute() or not destination.parent.is_dir():
        raise ValueError("Existing private absolute destination directory required")
    if destination.parent.stat().st_mode & 0o077:
        raise ValueError("Destination directory must be private (0700)")
    if any(Path(str(destination) + suffix).exists() or Path(str(destination) + suffix).is_symlink()
           for suffix in ("", "-wal", "-shm", "-journal")):
        raise ValueError("Destination and journals must not exist")
    fd, temporary = tempfile.mkstemp(prefix=".lor-snapshot-", dir=destination.parent)
    os.close(fd)
    try:
        # backup() includes committed WAL data; copying the main DB file does not.
        with closing(sqlite3.connect(source.as_uri() + "?mode=ro", uri=True, timeout=5)) as original:
            validate(original)
            with closing(sqlite3.connect(temporary)) as output:
                original.backup(output)
                output.execute("PRAGMA journal_mode=DELETE")
                validate(output)
        with open(temporary, "rb") as stream:
            os.fsync(stream.fileno())
        # Atomic no-clobber publication, same filesystem. Fails if target appeared meanwhile.
        os.link(temporary, destination)
        directory = os.open(destination.parent, os.O_RDONLY | os.O_DIRECTORY)
        try:
            os.fsync(directory)
        finally:
            os.close(directory)
    finally:
        os.unlink(temporary)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("source")
    parser.add_argument("destination")
    args = parser.parse_args()
    try:
        snapshot(args.source, args.destination)
    except (OSError, sqlite3.Error, ValueError):
        parser.exit(1, "Snapshot failed; no source modified. Inspect permissions/schema offline.\n")
    print("Snapshot verified and published; source unchanged.")
