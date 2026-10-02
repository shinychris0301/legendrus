import unittest,tempfile,json,os
from pathlib import Path
from AnalyzeSessions import analyze
class AnalysisTests(unittest.TestCase):
    def test_sources_content_duplicates_and_unfinished_are_separated(self):
        with tempfile.TemporaryDirectory() as d:
            def save(name,source='manual_campaign',content='current',outcome='won',boosters=0,session='s'):
                x={'schemaVersion':1,'sessionId':session,'source':source,'contentVersion':content,'attempts':[{'number':1,'chapter':1,'round':21,'ruleVersion':5,'outcome':outcome,'inventoryBoostersUsed':boosters,'moveBudget':32,'movesRemaining':5,'goalsRemaining':[0,0]}]};p=Path(d)/name;p.write_text(json.dumps(x));return p
            old=save('old.json',outcome='ongoing');os.utime(old,ns=(1,1));save('new.json')
            save('auto.json',source='automated_campaign');save('other.json',content='old');save('open.json',outcome='ongoing',session='open');save('boosted.json',boosters=1,session='boosted');save('quit.json',outcome='interrupted',session='quit')
            r=analyze(d,'current');self.assertEqual(r['manualSessions'],4);a=r['rows'][0];self.assertEqual((a['won'],a['lost'],a['ongoing'],a['interrupted'],a['boostedOrUnknown']),(1,0,1,1,1));self.assertEqual(r['excludedFiles']['older_duplicate_session'],1);self.assertEqual(r['rows'][1]['won'],0)
    def test_missing_folder_is_no_evidence(self):
        r=analyze('/nonexistent-campaign-fixture','current');self.assertEqual(r['manualSessions'],0);self.assertEqual(r['invalidFiles'],[])
    def test_incomplete_goals_cannot_be_counted_as_win(self):
        with tempfile.TemporaryDirectory() as d:
            x={'schemaVersion':1,'sessionId':'s','source':'manual_campaign','contentVersion':'current','attempts':[{'number':1,'chapter':1,'round':21,'ruleVersion':5,'outcome':'won','inventoryBoostersUsed':0,'moveBudget':32,'movesRemaining':5,'goalsRemaining':[1]}]};(Path(d)/'invalid.json').write_text(json.dumps(x));r=analyze(d,'current');self.assertEqual(r['manualSessions'],0);self.assertEqual(len(r['invalidFiles']),1)
if __name__=='__main__':unittest.main()
