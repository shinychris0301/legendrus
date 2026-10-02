from pathlib import Path
import json, collections
import xml.etree.ElementTree as ET

base = Path(__file__).resolve().parent.parent
runs = []
for path in sorted((base/'QA').glob('*/results.xml')):
    root = ET.parse(path).getroot()
    fixtures = collections.defaultdict(collections.Counter)
    issues = []
    cases = root.findall('.//test-case')
    for test in cases:
        name = test.get('fullname', '')
        fixture = test.get('classname') or name.rsplit('.', 1)[0]
        fixtures[fixture][test.get('result')] += 1
        if test.get('result') != 'Passed':
            message = test.findtext('.//message') or ''
            # Source-string assertions may include an entire script; keep XML as evidence.
            issues.append({'name': name, 'result': test.get('result'),
                           'messageExcerpt': message[:700],
                           'stack': (test.findtext('.//stack-trace') or '')[:1300]})
    runs.append({'run': path.parent.name, 'counts': {k: root.get(k) for k in
                 ['total','passed','failed','skipped','inconclusive','duration']},
                 'fixtures': dict(fixtures), 'issues': issues})
(base/'QA/RunInventory.json').write_text(json.dumps(runs, indent=2))
for run in runs:
    print(run['run'], run['counts'])
    for issue in run['issues']:
        print(issue['result'], issue['name'], issue['messageExcerpt'][:170].replace('\n',' '))
