"""Private backups and isolated native history reconstruction; no credentials copied."""
from pathlib import Path
import hashlib,json,os,queue,shutil,sqlite3,subprocess,threading,time,datetime
from zoneinfo import ZoneInfo
PROJECT=Path('/home/k2/LegendsOfRus_SplashTitle_R3_FidelityFix02_FULL')
STORE=Path('/home/k2/.codex')
THREAD='01a0d00e-5c8c-7e31-9fcb-a9d58d9cf1c8'
ALIAS='01a0f294-8d96-7501-8df9-b16f2f7fe00a'
BINARY=Path('/home/k2/.vscode/extensions/openai.chatgpt-26.930.21537-linux-x64/bin/linux-x86_64/codex')
RUN=Path(json.loads((Path(__file__).parent/'RunLocation.json').read_text())['privateRunDirectory'])
TABLES=['thread_turns','thread_items','thread_realtime_items','thread_history_projection_state']
def now():return datetime.datetime.now(ZoneInfo('America/New_York')).isoformat()
def write_json(path,data):
 path=Path(path);path.parent.mkdir(parents=True,exist_ok=True,mode=0o700)
 temp=path.with_name(path.name+'.tmp');temp.write_text(json.dumps(data,indent=2)+'\n');temp.chmod(0o600);os.replace(temp,path)
def sha(path):
 with Path(path).open('rb') as stream:return hashlib.file_digest(stream,'sha256').hexdigest()
def connect(path,readonly=False):
 c=sqlite3.connect(Path(path).as_uri()+('?mode=ro' if readonly else '?mode=rw'),uri=True,timeout=30);c.execute('PRAGMA busy_timeout=30000');return c
def checkpoint(c):c.execute('PRAGMA wal_checkpoint(TRUNCATE)');c.commit()
def sqlite_backup(source,target):
 with connect(source,True) as a,sqlite3.connect(target) as b:a.backup(b,pages=4096,sleep=.02)
 Path(target).chmod(0o600)
def complete_copy(source,target):
 limit=source.stat().st_size;target.parent.mkdir(parents=True,exist_ok=True,mode=0o700)
 with source.open('rb') as a,target.open('wb') as b:
  remaining=limit
  while remaining:
   block=a.read(min(1024*1024,remaining))
   if not block:raise RuntimeError('Source shrank during backup')
   b.write(block);remaining-=len(block)
  b.flush();os.fsync(b.fileno())
 with target.open('rb+') as f:
  length=f.seek(0,2)
  if length:
   tail_size=min(length,1024*1024);f.seek(length-tail_size);tail=f.read()
   if not tail.endswith(b'\n'):
    cut=tail.rfind(b'\n')
    if cut<0:raise RuntimeError('No complete final record found')
    f.truncate(length-tail_size+cut+1)
 target.chmod(0o600)
 return {'sourceBytesAtStart':limit,'completeBytesBackedUp':target.stat().st_size}
def backup(name,quiescent=False):
 folder=RUN/name;folder.mkdir(mode=0o700,exist_ok=False)
 for n in ['state_5.sqlite','thread_history_1.sqlite']:sqlite_backup(STORE/n,folder/n)
 with connect(folder/'state_5.sqlite',True) as c:
  rows=c.execute('SELECT id,rollout_path FROM threads WHERE id IN (?,?)',(THREAD,'01a09624-160e-7f10-a32e-7cc91172628e')).fetchall()
 files=[]
 for tid,path in rows:
  source=Path(path);target=folder/'rollouts'/source.name
  details=complete_copy(source,target)
  files.append({'threadId':tid,'source':str(source),'backup':str(target),'sha256':sha(target),**details})
 index=STORE/'session_index.jsonl'
 if index.exists():shutil.copyfile(index,folder/'session_index.jsonl');(folder/'session_index.jsonl').chmod(0o600)
 checks={}
 for n in ['state_5.sqlite','thread_history_1.sqlite']:
  with connect(folder/n,True) as c:checks[n]=c.execute('PRAGMA quick_check').fetchone()[0]
  assert checks[n]=='ok'
 manifest={'createdAt':now(),'quiescent':quiescent,'sqliteBackupsConsistentIndividually':True,'sqliteQuickChecks':checks,'files':files,
 'databaseHashes':{n:sha(folder/n) for n in checks},'credentialsConfigAndOtherPrivateStoresExcluded':True}
 write_json(folder/'manifest.json',manifest);return folder

def native_writers():
 found=[]
 for path in Path('/proc').glob('[0-9]*/cmdline'):
  try:
   args=[b.decode(errors='replace') for b in path.read_bytes().split(b'\0') if b]
   exe=Path(os.readlink(path.parent/'exe')).name
   # An IDE agent server can reopen threads even without a held thread lock.
   if exe in ['codex','codex-app-server'] and int(path.parent.name)!=os.getpid():
    found.append({'pid':int(path.parent.name),'executable':os.readlink(path.parent/'exe')})
  except (FileNotFoundError,PermissionError,ProcessLookupError):pass
 locks=Path('/proc/locks').read_text().splitlines()
 for path in (STORE/'thread-writer-locks').glob('*.lock'):
  for line in locks:
   if ':'+str(path.stat().st_ino)+' ' in line:
    if int(line.split()[4])!=os.getpid():found.append({'threadLock':path.name,'pid':int(line.split()[4])})
 return found

class NativeReader:
 def __init__(self,home,label):
  self.home=Path(home);self.events=queue.Queue();self.next_id=0
  env=os.environ.copy();env.update(CODEX_HOME=str(home),XDG_CONFIG_HOME=str(home/'isolated-config'),HTTP_PROXY='http://127.0.0.1:9',HTTPS_PROXY='http://127.0.0.1:9',ALL_PROXY='http://127.0.0.1:9',NO_PROXY='localhost,127.0.0.1')
  env.pop('OPENAI_API_KEY',None);env.pop('CHATGPT_AUTH_TOKEN',None)
  self.log=home/(label+'.stderr.log');self.err=self.log.open('wb');self.log.chmod(0o600)
  self.proc=subprocess.Popen([str(BINARY),'app-server'],cwd=home,env=env,stdin=subprocess.PIPE,stdout=subprocess.PIPE,stderr=self.err,text=True,bufsize=1)
  def reader():
   try:
    for line in self.proc.stdout:
     try:self.events.put(json.loads(line))
     except json.JSONDecodeError:self.events.put({'parseError':True})
   finally:self.events.put({'eof':True})
  self.thread=threading.Thread(target=reader,daemon=True);self.thread.start()
  self.request('initialize',{'clientInfo':{'name':'codex_history_repair','title':'Local History Repair','version':'1'},'capabilities':{'experimentalApi':True}})
  self.send({'method':'initialized','params':{}})
 def send(self,msg):self.proc.stdin.write(json.dumps(msg)+'\n');self.proc.stdin.flush()
 def request(self,method,params,timeout=90):
  assert method in ['initialize','thread/list','thread/read','thread/turns/list','thread/items/list','thread/resume']
  self.next_id+=1;ident=self.next_id;self.send({'id':ident,'method':method,'params':params});deadline=time.monotonic()+timeout
  while time.monotonic()<deadline:
   try:message=self.events.get(timeout=min(1,max(.01,deadline-time.monotonic())))
   except queue.Empty:continue
   if message.get('eof'):raise RuntimeError('Isolated native reader exited')
   if message.get('id')!=ident:continue
   if 'error' in message:raise RuntimeError('Native '+method+' failed: '+str(message['error'].get('message',''))[:300])
   return message['result']
  raise TimeoutError('Native '+method+' timed out')
 def close(self):
  if self.proc.poll() is None:
   self.proc.stdin.close()
   try:self.proc.wait(timeout=20)
   except subprocess.TimeoutExpired:self.proc.terminate();self.proc.wait(timeout=10)
  self.err.close()
 def __enter__(self):return self
 def __exit__(self,*args):self.close()
