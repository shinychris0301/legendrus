"""Correlate generated owner methods and native ELF bytes with the actual packaged library."""
from pathlib import Path
import hashlib,json,re,subprocess,struct,sys,zipfile,io
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA';QA.mkdir(exist_ok=True)
stage=sys.argv[1];assert stage in ('Before','After')
source='\n'.join(p.read_text() for p in sorted((ROOT/'Library/Bee/artifacts/Android/il2cppOutput/cpp').glob('LegendsOfRus.Runtime*.cpp')))
targets=['GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_CompleteLevel','GameState_TryCompleteLevel','GameState_TryDoubleLastLevelGoldReward','GameState_TryAddGold','GameState_TryAddGems','GameState_TryAddBooster','GameState_TryCredit','GameState_TryCommitBalance','GameState_TryBeginBoosterCommand','GameState_TrySettleBoosterCommand','GameState_TryRecoverPendingBoosterCommand','GameState_ClaimDailyReward','Match3Board_ConsumeCommandBooster','Match3Board_RefundCommand','Match3Board_RecoverActiveCommand','Match3Board_TryResume','ScreenRouter_StartLevel','ScreenRouter_CommitGameplayVictory']
pattern=re.compile(r'^IL2CPP_EXTERN_C IL2CPP_METHOD_ATTR \S+ ((?:GameState|Match3Board|ScreenRouter)_[A-Za-z0-9_]+_m[0-9A-F]+) \([^\n;]*\)\s*\n\{',re.M)
selected={}
for m in pattern.finditer(source):
 owner=m.group(1).rsplit('_m',1)[0]
 if owner not in targets:continue
 if owner=='GameState_ClaimDailyReward' and 'DailyRewardGrant_' not in m.group(0):continue
 assert owner not in selected,owner
 opening=source.index('{',m.start());depth=1;end=opening+1
 while depth:
  if source[end]=='{':depth+=1
  elif source[end]=='}':depth-=1
  end+=1
 selected[owner]=(m.group(1),source[m.start():end])
if stage=='After':assert set(selected)==set(targets),(set(targets)-set(selected))
else:assert {'GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_TryDoubleLastLevelGoldReward','GameState_ClaimDailyReward','Match3Board_RecoverActiveCommand','Match3Board_TryResume','ScreenRouter_StartLevel'}<=set(selected)
local=ROOT/'Library/Bee/artifacts/Android/d8kzr/libil2cpp.so'
listing=subprocess.check_output(['readelf','--wide','--syms',str(local)],text=True);symbols={}
for line in listing.splitlines():
 cols=line.split()
 if len(cols)>=8 and cols[3]=='FUNC':symbols[cols[-1]]=(int(cols[1],16),int(cols[2]))
def load_segments(stream):
 stream.seek(0);h=stream.read(64);assert h[:6]==b'\x7fELF\x02\x01'
 offset=struct.unpack_from('<Q',h,32)[0];stride,count=struct.unpack_from('<HH',h,54);rows=[]
 for i in range(count):
  stream.seek(offset+i*stride);p=stream.read(stride)
  if struct.unpack_from('<I',p)[0]==1:
   fileoff,virtual,_,size=struct.unpack_from('<QQQQ',p,8);rows.append((fileoff,virtual,size))
 return rows
def method(stream,segments,address,size):
 for offset,virtual,filesize in segments:
  if virtual<=address and address+size<=virtual+filesize:
   stream.seek(offset+address-virtual);b=stream.read(size);assert len(b)==size;return b
 raise AssertionError('Method is not in a file-backed load segment.')
apk=ROOT/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
with zipfile.ZipFile(apk) as z:packed=z.read('lib/arm64-v8a/libil2cpp.so');metadata=z.read('assets/bin/Data/Managed/Metadata/global-metadata.dat')
gradle=ROOT/'Library/Bee/Android/Prj/IL2CPP/Gradle/unityLibrary/src/main/jniLibs/arm64-v8a/libil2cpp.so'
with gradle.open('rb') as f:assert hashlib.file_digest(f,'sha256').hexdigest()==hashlib.sha256(packed).hexdigest()
record={'stage':stage,'methods':{},'packagedLibrarySha256':hashlib.sha256(packed).hexdigest(),'packagedLibraryMatchesGradleInput':True,'physicalExecutionVerified':False}
pstream=io.BytesIO(packed);psegments=load_segments(pstream)
with local.open('rb') as f:
 segments=load_segments(f)
 for owner,(symbol,body) in selected.items():
  assert symbol in symbols,symbol
  address,size=symbols[symbol];assert size>0
  native=method(f,segments,address,size);assert native==method(pstream,psegments,address,size)
  out=BASE/'Sources'/f'{stage}{owner}.cpp';assert not out.exists();out.write_text(body+'\n')
  record['methods'][owner]={'symbol':symbol,'generatedMethodSha256':hashlib.sha256(body.encode()).hexdigest(),'nativeMethodBytes':size,'nativeMethodSha256':hashlib.sha256(native).hexdigest(),'packagedMethodBytesMatchCompiledOutput':True,'generatedOverflowCatch':'catch(Il2CppExceptionWrapper&' in body and 'OverflowException_' in body,'callsPersist':'GameState_Persist_' in body,'callsAtomicBalanceCommit':'GameState_TryCommitBalance_' in body}
if stage=='After':
 for owner in ('GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster'):
  assert record['methods'][owner]['callsAtomicBalanceCommit'] and not record['methods'][owner]['callsPersist']
 for owner in ('GameState_ClaimDailyReward','GameState_TryDoubleLastLevelGoldReward','GameState_TryCompleteLevel','GameState_TryCredit','GameState_TrySettleBoosterCommand'):
  assert record['methods'][owner]['generatedOverflowCatch'],owner
 for name in ('BoosterCommandReceipt','boosterCommand','TryBeginBoosterCommand','TrySettleBoosterCommand','ShowGameplayInventoryError'):
  assert (name+'\0').encode() in metadata,name
 record['validatedReceiptAndRecoveryMetadataPresent']=True
 before=json.loads((QA/'BeforeNative.json').read_text())
 changed=[owner for owner in before['methods'] if before['methods'][owner]['nativeMethodSha256']!=record['methods'][owner]['nativeMethodSha256']]
 assert {'GameState_SpendGold','GameState_SpendGems','GameState_SpendEnergy','GameState_TryUseBooster','GameState_TryDoubleLastLevelGoldReward','Match3Board_RecoverActiveCommand'}<=set(changed)
 record['changedNativeMethodsFromBaseline']=changed
(QA/(stage+'Native.json')).write_text(json.dumps(record,indent=2)+'\n')
print(stage+': '+str(len(selected))+' generated/native methods match the packaged library.')
