"""Read compiled DEX class definitions and JNI members without extracting application data."""
from pathlib import Path
import struct, zipfile, json
root=Path(__file__).resolve().parents[5];base=Path(__file__).resolve().parent.parent
apk=root/'Builds/Verification/v1_17_step01/LegendsOfRus-v1.17-step01-development.apk'
prefix='Lcom/legendsofrus/credentials/'
def inspect(data):
    assert data[:4]==b'dex\n'
    def uint(offset):return struct.unpack_from('<I',data,offset)[0]
    def uleb(offset):
        value=0;shift=0
        while True:
            byte=data[offset];offset+=1;value|=(byte&127)<<shift
            if byte<128:return value,offset
            shift+=7
    def string(offset):
        _,offset=uleb(offset);end=data.index(0,offset)
        return data[offset:end].decode('utf-8',errors='replace')
    strings=[string(uint(uint(60)+i*4)) for i in range(uint(56))]
    types=[strings[uint(uint(68)+i*4)] for i in range(uint(64))]
    fields=[struct.unpack_from('<HHI',data,uint(84)+i*8) for i in range(uint(80))]
    methods=[struct.unpack_from('<HHI',data,uint(92)+i*8) for i in range(uint(88))]
    def signature(index):
        _,proto,name=methods[index];offset=uint(76)+proto*12
        ret=types[uint(offset+4)];parameters=uint(offset+8)
        args=[] if parameters==0 else [types[struct.unpack_from('<H',data,parameters+4+2*i)[0]] for i in range(uint(parameters))]
        return strings[name]+'('+''.join(args)+')'+ret
    classes={}
    for i in range(uint(96)):
        offset=uint(100)+i*32;name=types[uint(offset)]
        if not name.startswith(prefix):continue
        c=uint(offset+24);counts=[]
        if not c:classes[name]={'fields':[],'methods':[]};continue
        for _ in range(4):count,c=uleb(c);counts.append(count)
        field_defs=[];method_defs=[]
        for group,count in enumerate(counts):
            index=0
            for _ in range(count):
                delta,c=uleb(c);index+=delta;access,c=uleb(c)
                if group<2:
                    _,type_index,name_index=fields[index]
                    field_defs.append({'name':strings[name_index], 'type':types[type_index], 'accessFlags':access})
                else:
                    code,c=uleb(c);method_defs.append({'signature':signature(index),'accessFlags':access,'hasCode':code!=0})
        classes[name]={'fields':field_defs,'methods':method_defs}
    return classes
classes={}
with zipfile.ZipFile(apk) as archive:
    for name in archive.namelist():
        if name.startswith('classes') and name.endswith('.dex'):classes.update(inspect(archive.read(name)))
bridge=classes[prefix+'AndroidCredentialBridge;'];operation=classes[prefix+'AndroidCredentialBridge$Operation;']
expected=['beginRead(Landroid/content/Context;)Lcom/legendsofrus/credentials/AndroidCredentialBridge$Operation;', 'beginWrite(Landroid/content/Context;Ljava/lang/String;)Lcom/legendsofrus/credentials/AndroidCredentialBridge$Operation;']
for signature in expected:
    assert any(m['signature']==signature and m['hasCode'] and m['accessFlags']&9==9 for m in bridge['methods']),signature
for name,t in [('done','Z'),('success','Z'),('value','Ljava/lang/String;')]:
    assert any(f['name']==name and f['type']==t and f['accessFlags']&0x41==0x41 for f in operation['fields']),name
for name in ['CredentialVault;','CredentialVault$Keys;','CredentialVault$Journal;']:assert prefix+name in classes
result={'compiledCredentialClasses':classes,'jniEntryDefinitionsVerified':True,'operationFieldDefinitionsVerified':True,'deviceExecutionVerified':False}
(base/'QA/CredentialDex.json').write_text(json.dumps(result,indent=2)+'\n')
print(f'{len(classes)} credential DEX classes present; JNI methods and operation fields verified.')
