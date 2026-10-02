"""R12 collection uses its own exact content fingerprint, excluding R9 outcomes."""
import argparse
import hashlib
import importlib.util
import json
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
BASE=Path(__file__).resolve().parent.parent
spec=importlib.util.spec_from_file_location('r10_analysis', BASE.parent/'R10/Sources/AnalyzeSessions.py')
module=importlib.util.module_from_spec(spec);spec.loader.exec_module(module)
def candidate_id():
    folder=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates'
    texts=[(folder/'Chapter01Sector01R12/levels.json').read_text()]
    texts.extend((folder/'Chapter01Sector01'/name).read_text() for name in ['story.json','story_ru.json','story_en.json'])
    return 'CampaignSector01R12-'+hashlib.sha256('\n'.join(texts).encode()).hexdigest()
if __name__=='__main__':
    parser=argparse.ArgumentParser();parser.add_argument('folder',type=Path);parser.add_argument('--output',required=True,type=Path);args=parser.parse_args()
    result=module.analyze(args.folder,candidate_id());args.output.parent.mkdir(parents=True,exist_ok=True);args.output.write_text(json.dumps(result,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'manualSessions':result['manualSessions'],'excludedFiles':result['excludedFiles'],'invalidFiles':result['invalidFiles']}))
