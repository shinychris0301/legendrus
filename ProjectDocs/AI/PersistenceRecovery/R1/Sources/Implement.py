"""Make general persistence acknowledge saving without bypassing the session lock."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5];p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text()
needle='        bool canPersist = true;';assert needle in s;s=s.replace(needle,needle+'\n        bool persistenceInProgress;',1)
a=s.index('        public void Persist()');end=s.rfind('    }');assert end>a
s=s[:a]+'''        public void Persist()
        {
            // A Changed observer may request saving again while this write is publishing.
            if (persistenceInProgress) return;
            if (!TryPersist())
                Debug.LogError("Player save write blocked: " + LastWriteResult);
        }

        public bool TryPersist()
        {
            if (!canPersist || persistenceInProgress) return false;
            persistenceInProgress = true;
            try
            {
                // This legacy API persists the already prepared live Save. Preserve its
                // identity and the existing durable-primary acknowledgement policy.
                return TryCommitBalance(Save);
            }
            finally { persistenceInProgress = false; }
        }
'''+s[end:];p.write_text(s)
print('General persistence now acknowledges the shared commit policy, guards session lock/reentry and preserves live Save identity.')
