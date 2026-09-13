"""Record this turn's diff against preserved local changes, without Git mutation."""
from pathlib import Path
import difflib
import hashlib
import json

root = Path.cwd()
delivery = root / 'ProjectDocs/AI/HomeStyleAlignment/ReferenceReplicaR6'
audit_path = delivery / 'QA/IndependentSourceAudit.json'
audit = json.loads(audit_path.read_text())
assert audit['passed'], 'A complete passing final preservation audit is required.'
sha = lambda p: hashlib.sha256(p.read_bytes()).hexdigest()
rows = [dict(r, status='modified') for r in audit['modified']]
rows += [dict(r, status='added') for r in audit['addedAuditedFiles']]
for row in rows:
    assert sha(root / row['path']) == row['sha256'], row['path']
manifest = {'homeRevision': 9, 'basis': 'Recorded pre-R6 local working tree; earlier local changes preserved.',
    'modified': len(audit['modified']), 'added': len(audit['addedAuditedFiles']), 'files': rows}
(delivery / 'SourceChangeManifest.json').write_text(json.dumps(manifest, indent=2) + '\n')
(delivery / 'FinalSourceAudit.json').write_bytes(audit_path.read_bytes())
before = json.loads((delivery / 'BeforeHashes.json').read_text())
paths = sorted({r['path'] for r in before} | {r['path'] for r in rows})
final = [{'path': p, 'sha256': sha(root / p)} for p in paths]
(delivery / 'FinalProductionHashes.json').write_text(json.dumps(final, indent=2) + '\n')
(delivery / 'GuidPreservation.json').write_text(json.dumps({'metadata': audit['metadata'],
    'prefabIds': audit['prefabIds'], 'additions': audit['additionPolicy']}, indent=2) + '\n')

diff = []
whitespace = []
for row in rows:
    rel = row['path']
    p = root / rel
    if p.suffix not in {'.cs', '.prefab', '.meta'}:
        continue
    previous = Path('/tmp/lor-home-r6/before') / rel
    old = previous.read_text().splitlines(keepends=True) if row['status'] == 'modified' else []
    current = p.read_text().splitlines(keepends=True)
    diff.extend(difflib.unified_diff(old, current, fromfile='before/' + rel, tofile='after/' + rel))
    if p.suffix == '.cs':
        issues = [i for i, line in enumerate(current, 1) if line.rstrip('\r\n').rstrip() != line.rstrip('\r\n')]
        whitespace.append({'path': rel, 'trailingWhitespaceLines': issues, 'endsInNewline': p.read_bytes().endswith(b'\n')})
(delivery / 'AppliedSourceChanges.diff').write_text(''.join(diff))
assert all(not r['trailingWhitespaceLines'] and r['endsInNewline'] for r in whitespace)
(delivery / 'QA/SourceWhitespaceChecks.json').write_text(json.dumps(whitespace, indent=2) + '\n')
print(json.dumps({'modified': manifest['modified'], 'added': manifest['added'], 'finalHashes': len(final), 'csharpWhitespaceChecks': len(whitespace)}))
