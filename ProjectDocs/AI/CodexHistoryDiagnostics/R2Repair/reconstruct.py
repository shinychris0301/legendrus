"""Rehearse a canonical, monotonic rollout and native additive materialization."""
from pathlib import Path
import json,shutil,sqlite3,collections
import repair_lib as r

def column_names(c,table):return [row[1] for row in c.execute('PRAGMA table_info('+table+')')]
def prepare(snapshot,folder):
 folder.mkdir(parents=True,mode=0o700,exist_ok=False)
 for name in ['state_5.sqlite','thread_history_1.sqlite']:
  shutil.copyfile(snapshot/name,folder/name);(folder/name).chmod(0o600)
 manifest=json.loads((snapshot/'manifest.json').read_text());source=Path(next(x['backup'] for x in manifest['files'] if x['threadId']==r.THREAD))
 canonical=folder/'sessions/2026/09/30'/('rollout-2026-09-30T09-50-16-'+r.THREAD+'.jsonl');canonical.parent.mkdir(parents=True,mode=0o700)
 with r.connect(folder/'thread_history_1.sqlite') as c:
  maxima=[]
  for table in ['thread_turns','thread_items','thread_realtime_items']:
   value=c.execute('SELECT MAX(rollout_ordinal) FROM '+table+' WHERE thread_id IN (?,?)',(r.THREAD,r.ALIAS)).fetchone()[0]
   maxima.append(value or 0)
  cursor=c.execute('SELECT MAX(next_rollout_ordinal) FROM thread_history_projection_state WHERE thread_id IN (?,?)',(r.THREAD,r.ALIAS)).fetchone()[0] or 0
  offset=max(max(maxima)+1,cursor)
  transferred={}
  for table in ['thread_turns','thread_items','thread_realtime_items']:
   columns=column_names(c,table);select=['?' if name=='thread_id' else 'a.'+name for name in columns]
   key={'thread_turns':['turn_id'],'thread_items':['turn_id','item_id'],'thread_realtime_items':['item_id']}[table]
   match=' AND '.join('b.'+x+'=a.'+x for x in key)
   sql='INSERT INTO '+table+' ('+','.join(columns)+') SELECT '+','.join(select)+' FROM '+table+' a WHERE a.thread_id=? AND NOT EXISTS (SELECT 1 FROM '+table+' b WHERE b.thread_id=? AND '+match+')'
   transferred[table]=c.execute(sql,(r.THREAD,r.ALIAS,r.THREAD)).rowcount
  c.execute('INSERT INTO thread_history_projection_state(thread_id,next_rollout_byte_offset,next_rollout_ordinal) VALUES (?,0,0) ON CONFLICT(thread_id) DO UPDATE SET next_rollout_byte_offset=0,next_rollout_ordinal=0',(r.THREAD,));c.commit();r.checkpoint(c)
 counts=collections.Counter();records=0;prev=-1;original_text_fingerprint=__import__('hashlib').sha256();rebased_text_fingerprint=__import__('hashlib').sha256()
 with source.open() as a,canonical.open('w') as b:
  for line in a:
   d=json.loads(line);ordinal=d.get('ordinal')
   if not isinstance(ordinal,int) or ordinal<=prev:raise RuntimeError('Raw rollout ordinal discontinuity')
   prev=ordinal
   if records==0 and (d.get('type')!='session_meta' or d['payload'].get('id')!=r.THREAD or d['payload'].get('session_id')!=r.THREAD):raise RuntimeError('Unexpected target session identity')
   original_text_fingerprint.update(json.dumps({k:v for k,v in d.items() if k!='ordinal'},sort_keys=True).encode())
   d['ordinal']=ordinal+offset
   rebased_text_fingerprint.update(json.dumps({k:v for k,v in d.items() if k!='ordinal'},sort_keys=True).encode())
   b.write(json.dumps(d,ensure_ascii=False,separators=(',',':'))+'\n');records+=1;counts[str(d.get('timestamp',''))[:10]]+=1
 canonical.chmod(0o600)
 assert original_text_fingerprint.digest()==rebased_text_fingerprint.digest()
 with r.connect(folder/'state_5.sqlite') as c:
  # Every metadata path is relocated: native startup cannot access live rollouts.
  for ident,path in c.execute('SELECT id,rollout_path FROM threads').fetchall():
   c.execute('UPDATE threads SET rollout_path=? WHERE id=?',(str(canonical) if ident==r.THREAD else str(folder/'missing-other-rollouts'/Path(path).name),ident))
  c.commit();r.checkpoint(c)
 config='[analytics]\nenabled = false\n[features]\nplugins = false\n'
 (folder/'config.toml').write_text(config);(folder/'config.toml').chmod(0o600)
 result={'home':str(folder),'snapshot':str(snapshot),'canonicalRollout':str(canonical),'ordinalOffset':offset,'lastOrdinal':prev+offset,'records':records,'recordsByUtcDate':counts,'messageContentUnchanged':True,'aliasRowsAddedToPrimary':transferred,'noOriginalHistoryRowsDeleted':True,'rawOriginalBackupHash':r.sha(source),'canonicalRolloutHash':r.sha(canonical)}
 r.write_json(folder/'Preparation.json',result);return result

def native_validate(folder,label):
 page_ids=[]
 with r.NativeReader(folder,label) as server:
  server.request('thread/resume',{'threadId':r.THREAD,'cwd':str(folder),'approvalPolicy':'never'})
  meta=server.request('thread/read',{'threadId':r.THREAD,'includeTurns':False})
  result=server.request('thread/turns/list',{'threadId':r.THREAD,'limit':100,'sortDirection':'asc','itemsView':'notLoaded'})
  while True:
   page_ids.extend(row['id'] for row in result['data'])
   if result.get('nextCursor') is None:break
   result=server.request('thread/turns/list',{'threadId':r.THREAD,'limit':100,'sortDirection':'asc','itemsView':'notLoaded','cursor':result['nextCursor']})
  # Read all persisted items, without starting/resuming a writer or any model turn.
  item_count=0;types=collections.Counter();cursor=None;native_chat={}
  while True:
   params={'threadId':r.THREAD,'limit':100,'sortDirection':'asc'}
   if cursor is not None:params['cursor']=cursor
   items=server.request('thread/items/list',params)
   for entry in items['data']:
    item=entry['item'];types[item.get('type','unknown')]+=1
    if item.get('type') in ['userMessage','agentMessage']:native_chat[(entry['turnId'],item['id'])]=item
   item_count+=len(items['data']);cursor=items.get('nextCursor')
   if cursor is None:break
  listing=server.request('thread/list',{'sourceKinds':['vscode'],'cwd':str(folder),'useStateDbOnly':True,'limit':50})
  listed=r.THREAD in [row['id'] for row in listing['data']]
 with r.connect(folder/'thread_history_1.sqlite',True) as c:
  projection=c.execute('SELECT next_rollout_byte_offset,next_rollout_ordinal FROM thread_history_projection_state WHERE thread_id=?',(r.THREAD,)).fetchone()
  user_dates=c.execute("SELECT date(created_at_ms/1000,'unixepoch'),count(*) FROM thread_items WHERE thread_id=? AND item_type='userMessage' GROUP BY 1",(r.THREAD,)).fetchall()
  indexed_counts={table:c.execute('SELECT count(*) FROM '+table+' WHERE thread_id=?',(r.THREAD,)).fetchone()[0] for table in ['thread_turns','thread_items','thread_realtime_items']}
  checked=c.execute('PRAGMA quick_check').fetchone()[0]
 prep=json.loads((folder/'Preparation.json').read_text());assert meta['thread']['id']==r.THREAD
 actual_size=Path(prep['canonicalRollout']).stat().st_size
 with Path(prep['canonicalRollout']).open('rb') as stream:
  stream.seek(max(0,actual_size-1048576));last=json.loads(stream.read().splitlines()[-1])
 assert projection[0]==actual_size and projection[1]==last['ordinal']+1,(projection,last['ordinal'])
 assert listed and any(day=='2026-10-02' and count>0 for day,count in user_dates) and checked=='ok'
 assert len(page_ids)>=6 and item_count>100
 log=(folder/(label+'.stderr.log')).read_text(errors='replace')
 assert 'skipping duplicate or regressed rollout ordinal' not in log
 retention=validate_retention(folder,native_chat)
 result={'retention':retention,'legacyResumeAdapterDropWarnings':log.count('dropping turn-scoped item for unknown turn id'),'label':label,'turnsVisibleViaNativeApi':len(page_ids),'itemsVisibleViaNativeApi':item_count,'nativeItemTypes':types,'primaryListed':listed,'projectionNextByteOffset':projection[0],'projectionNextOrdinal':projection[1],'indexedRows':indexed_counts,'userMessagesByUtcDate':user_dates,'sqliteQuickCheck':checked,'ordinalSkipWarning':False,'isolatedNativeResumeWithoutModelTurn':True}
 r.write_json(folder/(label+'.json'),result);return result

def validate_retention(folder,native_chat=None):
 prep=json.loads((folder/'Preparation.json').read_text());expected={}
 with Path(prep['canonicalRollout']).open() as stream:
  for line in stream:
   d=json.loads(line);a=d.get('payload',{})
   if d.get('type')=='event_msg' and a.get('type')=='item_completed' and a.get('item',{}).get('type') in ['UserMessage','AgentMessage']:
    item=a['item'];expected[(a['turn_id'],item['id'])]=item
 with r.connect(folder/'thread_history_1.sqlite',True) as c:
  actual={(turn,item):json.loads(body) for turn,item,body in c.execute('SELECT turn_id,item_id,item_json FROM thread_items WHERE thread_id=?',(r.THREAD,))}
  for key,item in expected.items():
   assert key in actual,('Missing persisted chat item',key)
   stored=actual[key]
   assert stored['id']==item['id']
   if native_chat is not None:
    assert key in native_chat,('Missing native API chat item',key)
    native=native_chat[key]
    if item['type']=='UserMessage':assert native['content']==stored['content']
    else:assert native['text']==stored['text']
   if item['type']=='UserMessage':assert stored['content']==item['content'] and stored.get('clientId')==item.get('client_id')
   else:
    text=''.join(part.get('text','') for part in item['content'])
    assert stored['text']==text and stored.get('phase')==item.get('phase'),('Changed agent text',key)
 with r.connect(Path(prep['snapshot'])/'thread_history_1.sqlite',True) as before:
  historical=before.execute('SELECT turn_id,item_id,item_json FROM thread_items WHERE thread_id IN (?,?)',(r.THREAD,r.ALIAS)).fetchall()
  for turn,item,body in historical:assert actual[(turn,item)]==json.loads(body),('Changed historical indexed item',turn,item)
 return {'rawChatItemsRecovered':len(expected),'rawUserMessagesRecovered':sum(i['type']=='UserMessage' for i in expected.values()),'rawAgentMessagesRecovered':sum(i['type']=='AgentMessage' for i in expected.values()),'rawUserAndAgentTextExact':True,'existingIndexedItemsPreserved':len(historical),'rawChatTextAvailableViaNativeApi':native_chat is not None}

if __name__=='__main__':
 folder=r.RUN/'Rehearsal4';prep=prepare(r.RUN/'InitialBackup',folder)
 print(json.dumps({k:v for k,v in prep.items() if k not in ['recordsByUtcDate']},indent=2),flush=True)
 first=native_validate(folder,'FirstRead');print(json.dumps(first,indent=2),flush=True)
 second=native_validate(folder,'RestartRead');assert first['turnsVisibleViaNativeApi']==second['turnsVisibleViaNativeApi'] and first['itemsVisibleViaNativeApi']==second['itemsVisibleViaNativeApi']
 print(json.dumps(second,indent=2),flush=True)
