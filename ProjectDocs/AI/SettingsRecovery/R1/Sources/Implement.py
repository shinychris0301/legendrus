"""Apply the reproduced settings fix to existing state and native UI owners."""
from pathlib import Path
ROOT=Path(__file__).resolve().parents[5]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs';s=p.read_text()
a=s.index('        public void ToggleMusic()');b=s.index('        public void ResetProgress()',a)
s=s[:a]+'''        public void ToggleMusic() => TryTogglePreference(0);
        public void ToggleSfx() => TryTogglePreference(1);
        public void ToggleVibration() => TryTogglePreference(2);
        public void ToggleNotifications() => TryTogglePreference(3);

        public bool TryTogglePreference(int index)
        {
            if (index < 0 || index > 3) throw new ArgumentOutOfRangeException(nameof(index));
            if (!canPersist) return false;
            var candidate = JsonUtility.FromJson<PlayerSaveData>(JsonUtility.ToJson(Save));
            switch (index)
            {
                case 0: candidate.music = !candidate.music; break;
                case 1: candidate.sfx = !candidate.sfx; break;
                case 2: candidate.vibration = !candidate.vibration; break;
                case 3: candidate.notifications = !candidate.notifications; break;
            }
            return TryCommitBalance(candidate);
        }

'''+s[b:];p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionSettingsView.cs';s=p.read_text()
s=s.replace('        int infoKind;','        int infoKind;\n        object bindingOwner=new object();\n        bool preferenceChangeInProgress;',1)
s=s.replace('void Unbind(){for(', 'void Unbind(){bindingOwner=new object();for(',1)
s=s.replace('if(!ActionsBound||committed||!isActiveAndEnabled)', 'if(!ActionsBound||committed||preferenceChangeInProgress||!isActiveAndEnabled)',1)
s=s.replace('bool overlay=false){UnityAction call=()=>{if(!InputReady()', 'bool overlay=false){object owner=bindingOwner;UnityAction call=()=>{if(!ReferenceEquals(owner,bindingOwner)||!InputReady()',1)
a=s.index('            switch(index){case 0:GameState.Instance.ToggleMusic();');b=s.index('        public bool Preference(',a)
s=s[:a]+'''            bool accepted;
            preferenceChangeInProgress=true;
            try { accepted=GameState.Instance.TryTogglePreference(index); }
            finally { preferenceChangeInProgress=false; }
            RefreshState();
            if(!accepted)ShowInfo(7);
        }
'''+s[b:]
s=s.replace('if(kind<0||kind>6)', 'if(kind<0||kind>7)',1)
s=s.replace('            InfoHeading.text=infoKind==6?', '            bool error=infoKind==7;\n            InfoHeading.text=error?(en?"AN ERROR OCCURRED":"ПРОИЗОШЛА ОШИБКА"):infoKind==6?',1)
s=s.replace('InfoDescription.text=(en?eng:ru)[infoKind];','InfoDescription.text=error?(en?"Action could not be completed.\\nClose this message and try again.":"Действие не выполнено.\\nЗакройте окно и попробуйте снова."):(en?eng:ru)[infoKind];',1)
p.write_text(s)
print('Atomic preference acknowledgement, retained native error panel, callback ownership and reentry guard applied.')
