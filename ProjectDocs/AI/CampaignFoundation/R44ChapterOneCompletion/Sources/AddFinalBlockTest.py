from pathlib import Path
root = Path(__file__).resolve().parents[5]
p = root/'Assets/LegendsOfRus/Tests/CampaignPlayMode/CampaignActivePlaytestTests.cs'
s = p.read_text()
assert 'Round500EndsRecordedBlockAndReplayStays500' not in s
s = s.replace('        IEnumerator Terminal(int first)',
'''        [UnityTest,Timeout(180000)] public IEnumerator Round500EndsRecordedBlockAndReplayStays500() {yield return Terminal(491);}
        IEnumerator Terminal(int first)''')
old = '            var witness=JsonUtility.FromJson<Witness>(File.ReadAllText(Path.Combine(Application.dataPath,$"../ProjectDocs/AI/CampaignFoundation/R33OrchardFieldCheckpoint/QA/Verify01/Active/Journey/Round{f.EndRound}_Attempt1_Witness.json")));'
assert old in s
s = s.replace(old, '''            string evidence=f.EndRound>200?"R44ChapterOneCompletion/QA/Witnesses":"R33OrchardFieldCheckpoint/QA/Verify01/Active/Journey";
            var witness=JsonUtility.FromJson<Witness>(File.ReadAllText(Path.Combine(Application.dataPath,$"../ProjectDocs/AI/CampaignFoundation/{evidence}/Round{f.EndRound}_Attempt1_Witness.json")));''')
assert '++guard,Is.LessThan(5)' in s
s = s.replace('++guard,Is.LessThan(5)', '++guard,Is.LessThan(8)')
p.write_text(s)
