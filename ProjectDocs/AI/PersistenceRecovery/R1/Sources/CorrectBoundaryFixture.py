"""Keep the envelope fixture within the existing test assembly references using reflection."""
from pathlib import Path
import json
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent
paths=[ROOT/'Assets/LegendsOfRus/Tests/PlayMode/LevelSystemRuntimePlayModeTests.cs',BASE/'Sources/AddBoundaries.py']
for p in paths:
 s=p.read_text()
 s=s.replace('var encoded=new LegendsOfRus.SaveCore.SaveEnvelopeCodec().TryEncode(JsonUtility.ToJson(GameState.Instance.Save),6,long.MaxValue);Assert.That(encoded.Success,Is.True);raw=encoded.RawEnvelope;', 'raw=PersistenceEncodeEnvelope(GameState.Instance.Save,long.MaxValue);')
 s=s.replace('var encoded=new LegendsOfRus.SaveCore.SaveEnvelopeCodec().TryEncode(JsonUtility.ToJson(seed),5,long.MaxValue);Assert.That(encoded.Success,Is.True);PlayerPrefs.SetString(PersistedSaveKeys[0],encoded.RawEnvelope);', 'PlayerPrefs.SetString(PersistedSaveKeys[0],PersistenceEncodeEnvelope(seed,long.MaxValue));')
 needle='        void PersistenceSeedBlockedJournal(int reason)';assert needle in s
 helper='''        string PersistenceEncodeEnvelope(PlayerSaveData save,long revision)
        {
            System.Type codec=null;foreach(var assembly in System.AppDomain.CurrentDomain.GetAssemblies()){codec=assembly.GetType("LegendsOfRus.SaveCore.SaveEnvelopeCodec");if(codec!=null)break;}
            Assert.That(codec,Is.Not.Null);var encode=codec.GetMethod("TryEncode",new[]{typeof(string),typeof(int),typeof(long)});
            var result=encode.Invoke(System.Activator.CreateInstance(codec),new object[]{JsonUtility.ToJson(save),save.saveVersion,revision});
            Assert.That((bool)result.GetType().GetProperty("Success").GetValue(result),Is.True);return (string)result.GetType().GetProperty("Envelope").GetValue(result);
        }
'''
 s=s.replace(needle,helper+needle,1);p.write_text(s)
(BASE/'QA/BoundaryFixtureCorrection.json').write_text(json.dumps({'initialLauncherExitCode':1,'initialUnityCasesExecuted':0,'initialErrors':'Two direct SaveCore codec references are outside the existing PlayMode assembly references.','correction':'Reflect the already loaded public codec and use its actual Envelope property; no asmdef/package/production change.','initialEvidence':'Boundaries/summary.json','nextRun':'BoundariesCorrected'},indent=2)+'\n')
print('Envelope test fixture corrected without changing assembly references or production code.')
