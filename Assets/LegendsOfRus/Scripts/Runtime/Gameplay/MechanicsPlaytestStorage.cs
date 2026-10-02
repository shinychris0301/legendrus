using System;
using System.IO;
using System.Text;
namespace LegendsOfRus
{
    // One checkpoint and one previous copy per session. Never opens campaign saves.
    // An unclean shutdown leaves an honest ongoing snapshot, not an inferred result.
    public sealed class MechanicsPlaytestStorage
    {
        readonly MechanicsPlaytestLog log;
        string savedJson;
        public string CheckpointPath {get;}
        public MechanicsPlaytestStorage(MechanicsPlaytestLog log,string directory)
        {
            this.log=log??throw new ArgumentNullException(nameof(log));
            CheckpointPath=Path.Combine(directory,"Sessions",log.Source,"playtest-"+log.SessionId+".json");
        }
        public void Save()
        {
            string json=log.ToJson();
            if(json==savedJson&&File.Exists(CheckpointPath))return;
            Directory.CreateDirectory(Path.GetDirectoryName(CheckpointPath));
            string temp=CheckpointPath+".tmp";
            bool created=false;
            try
            {
                using(var stream=new FileStream(temp,FileMode.CreateNew,FileAccess.Write,FileShare.None))
                {
                    created=true;
                    byte[] bytes=new UTF8Encoding(false).GetBytes(json);
                    stream.Write(bytes,0,bytes.Length);
                    stream.Flush(true);
                }
                // Keep the previous committed bytes if the replacement cannot complete.
                // No delete-then-move fallback: unsupported filesystems report failure.
                if(File.Exists(CheckpointPath))File.Replace(temp,CheckpointPath,CheckpointPath+".bak");
                else File.Move(temp,CheckpointPath);
                savedJson=json;
            }
            finally{if(created&&File.Exists(temp))File.Delete(temp);}
        }
    }
}
