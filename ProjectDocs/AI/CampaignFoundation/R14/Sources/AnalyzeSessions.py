"""Aggregate current manual candidate sessions; never mix solver/test evidence into player rates."""
import argparse,json,hashlib
from pathlib import Path
from collections import Counter
ROOT=Path(__file__).resolve().parents[5]
def candidate_id():
    d=ROOT/'Assets/LegendsOfRus/EditorContent/CampaignCandidates/Chapter01Sector01R14'
    contents='\n'.join((d/n).read_text() for n in ['levels.json','story.json','story_ru.json','story_en.json'])
    return 'CampaignSector01R14-'+hashlib.sha256(contents.encode()).hexdigest()
def analyze(folder,expected):
    latest={};excluded=Counter();issues=[]
    for p in sorted(Path(folder).rglob('*.json')):
        try:
            if p.stat().st_size>8*1024*1024:raise ValueError('oversized')
            x=json.loads(p.read_text())
            if x.get('source')!='manual_campaign':excluded['not_manual_campaign']+=1;continue
            if x.get('contentVersion')!=expected:excluded['different_content']+=1;continue
            if x.get('schemaVersion')!=1 or not isinstance(x.get('sessionId'),str) or not x['sessionId']:raise ValueError('session identity/schema')
            items=x.get('attempts',[])+x.get('activeAttempts',[]);numbers=set()
            for a in items:
                if a.get('number') in numbers:raise ValueError('duplicate attempt number')
                numbers.add(a.get('number'))
                if a.get('chapter')!=1 or not isinstance(a.get('round'),int) or a.get('ruleVersion')!=5:raise ValueError('round/rules identity')
                if a.get('outcome') not in ('won','lost','interrupted','ongoing'):raise ValueError('outcome')
                if not isinstance(a.get('inventoryBoostersUsed'),int):raise ValueError('missing inventory provenance')
                if not isinstance(a.get('movesRemaining'),int) or not isinstance(a.get('moveBudget'),int) or a['movesRemaining']<0 or a['moveBudget']<1:raise ValueError('moves')
                if a.get('outcome')=='won' and (not a.get('goalsRemaining') or any(v!=0 for v in a['goalsRemaining'])):raise ValueError('won with incomplete goals')
            old=latest.get(x['sessionId']);record=(p.stat().st_mtime_ns,str(p),x)
            if old is not None:excluded['older_duplicate_session']+=1
            if old is None or record[:2]>old[:2]:latest[x['sessionId']]=record
        except (ValueError,TypeError,KeyError,OSError,AttributeError) as e:issues.append({'file':str(p),'reason':str(e)})
    rows={n:{'round':n,'won':0,'lost':0,'interrupted':0,'ongoing':0,'boostedOrUnknown':0,'winMovesRemaining':[],'difficulty':Counter(),'goals':Counter(),'specials':Counter()} for n in range(31,41)}
    dropped=0
    for _,_,x in latest.values():
        dropped+=x.get('droppedAttempts',0)
        for a in x.get('attempts',[])+x.get('activeAttempts',[]):
            if a['round'] not in rows:continue
            r=rows[a['round']]
            if a['inventoryBoostersUsed']!=0:r['boostedOrUnknown']+=1;continue
            r[a['outcome']]+=1
            if a['outcome']=='won':r['winMovesRemaining'].append(a['movesRemaining'])
            for field,key in [('difficulty','difficulty'),('goals','goalClarity'),('specials','specialClarity')]:r[field][a.get(key,'unrated')]+=1
    return {'contentVersion':expected,'manualSessions':len(latest),'excludedFiles':dict(excluded),'invalidFiles':issues,'droppedAttempts':dropped,'rows':list(rows.values()),'limits':'Session counts are not unique people. Boosted/unknown inventory runs excluded from unassisted outcomes. Interrupted/ongoing are not losses. No population win-rate or difficulty acceptance inferred. Missing rounds are not filled with solver results.'}
if __name__=='__main__':
    p=argparse.ArgumentParser();p.add_argument('folder',type=Path);p.add_argument('--output',type=Path,required=True);args=p.parse_args()
    report=analyze(args.folder,candidate_id());args.output.parent.mkdir(parents=True,exist_ok=True);args.output.write_text(json.dumps(report,ensure_ascii=False,indent=2)+'\n')
    print(json.dumps({'manualSessions':report['manualSessions'],'invalidFiles':len(report['invalidFiles']),'output':str(args.output)}))
