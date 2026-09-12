#!/usr/bin/env python3
"""Verify the shipped serialized Splash artifact, without Unity or third-party Python modules.

Usage: python Tools/verify_splash_prefabfix07.py --project . --output result.json
This checks files, IDs and coordinates. It is NOT a Unity compiler, renderer or Play Mode test.
"""
import argparse
import hashlib
import json
import re
import sys
from pathlib import Path

PREFIX = 'Assets/LegendsOfRus/'
PREFAB = PREFIX + 'Resources/ProductionUI/Pages/SplashTitle.prefab'
LAYOUT = PREFIX + 'Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json'
VIEW = PREFIX + 'Scripts/Runtime/UI/SplashTitleView.cs'
BUILDER = PREFIX + 'Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs'
LOADER = PREFIX + 'Scripts/Runtime/UI/ProductionSplashTitleRuntime.cs'


def verify(root):
    checks = []
    def check(name, ok, detail=''):
        checks.append(dict(name=name, passed=bool(ok), detail=detail))
    def read(rel):
        return (root / rel).read_text(encoding='utf-8-sig')
    def scalar(body, key):
        match = re.search(r'^  ' + re.escape(key) + r': (.*)$', body, re.M)
        if not match: raise ValueError('Missing field ' + key)
        return match.group(1)
    def vec(body, key):
        return {k: float(v) for k,v in re.findall(r'([xyz]): ([-+\deE.]+)', scalar(body, key))}
    def ref(body, key):
        return int(re.search(r'fileID: (\d+)', scalar(body,key)).group(1))
    def guid(meta):
        return re.search(r'^guid: ([a-f0-9]+)$', meta, re.M).group(1)
    def script_guid(body):
        m=re.search(r'^  m_Script: .*guid: ([a-f0-9]+)',body,re.M)
        return m.group(1) if m else None

    prefab=read(PREFAB)
    blocks={}
    for part in re.split(r'(?m)(?=^--- !u!)',prefab)[1:]:
        match=re.match(r'--- !u!(\d+) &(\d+)\n',part)
        if not match: raise ValueError('Malformed Unity object header')
        kind,ident=map(int,match.groups())
        check('unique_object_id_'+str(ident),ident not in blocks)
        blocks[ident]=(kind,part)
    check('prefab_yaml_header',prefab.startswith('%YAML 1.1'))
    local_refs=[int(m.group(1)) for m in re.finditer(r'\{fileID: (\d+)\}',prefab)]
    invalid_refs=sorted({v for v in local_refs if v!=0 and v not in blocks})
    check('all_local_file_ids_resolve',not invalid_refs,str(invalid_refs))

    source=read(VIEW)
    current=int(re.search(r'CurrentRevision\s*=\s*(\d+)',source).group(1))
    view_guid=guid(read(VIEW+'.meta'))
    view_parts=[body for kind,body in blocks.values() if script_guid(body)==view_guid]
    check('one_splash_view',len(view_parts)==1)
    view=view_parts[0]
    check('saved_revision_matches_code',int(scalar(view,'Revision'))==current,
          'serialized='+scalar(view,'Revision')+' required='+str(current))
    check('colorfix06_revision_preserved',current==5)
    check('strict_readiness_gate_retained',
          'HasRequiredBindings => HasCurrentRevision && HasCompleteBindings' in source)
    check('separate_revision_diagnostic','Serialized revision ' in source and 'Missing binding: ' in source)

    fields=['StartButton','LanguageButton','SettingsButton','AccountButton','HelpButton',
            'VersionText','LanguageText','StartText','SettingsText','AccountText','HelpText','TaglineText','LegalText']
    binding_ids=[ref(view,f) for f in fields]
    for field,ident in zip(fields,binding_ids):
        check('bound_'+field,ident in blocks and ident!=0)
    check('distinct_bound_controls_and_labels',len(set(binding_ids))==13)
    type_guid=re.search(r'guid: ([a-f0-9]+)',scalar(view,'Typography')).group(1)
    typ=PREFIX+'Resources/ProductionUI/SplashTitleConfig/SplashTitleTypography.asset'
    check('typography_config_guid_resolves',guid(read(typ+'.meta'))==type_guid)

    ui_guids={'Button':'4e29b1a8efbd4b44bb3f3716e73f07ff',
              'Text':'5f7201a12d95ffc409449d95f23cf332',
              'Image':'fe87c0e1cc204ed48ad3b37840f39efc'}
    counts={name:sum(script_guid(body)==sg for _,body in blocks.values()) for name,sg in ui_guids.items()}
    for name,want in [('Button',5),('Text',8),('Image',10)]:
        check('component_count_'+name,counts[name]==want,str(counts[name]))
    for field,ident in zip(fields,binding_ids):
        check('binding_type_'+field,script_guid(blocks.get(ident,(-1,''))[1])==ui_guids['Button' if field.endswith('Button') else 'Text'])
    check('no_nested_canvas',not any(kind==223 for kind,_ in blocks.values()))
    sprite_paths={guid(p.read_text()):p for p in (root/(PREFIX+'Resources/ProductionUI/SplashTitleReconstructed')).glob('*.png.meta')}
    sprites=[]
    for ident,(kind,body) in blocks.items():
        if script_guid(body)==ui_guids['Image']:
            sg=re.search(r'guid: ([a-f0-9]+)',scalar(body,'m_Sprite')).group(1)
            sprites.append(sg)
            check('sprite_reference_'+str(ident),sg in sprite_paths and Path(str(sprite_paths.get(sg,''))[:-5]).is_file())
            color=scalar(body,'m_Color')
            check('no_additional_tint_'+str(ident),color=='{r: 1, g: 1, b: 1, a: 1}')
    check('ten_distinct_reconstructed_sprites',len(sprites)==len(set(sprites))==10)

    names={ident:json.loads(scalar(body,'m_Name')) if scalar(body,'m_Name').startswith('"') else scalar(body,'m_Name')
           for ident,(kind,body) in blocks.items() if kind==1}
    transforms={ident:(ref(body,'m_GameObject'),ref(body,'m_Father'),body)
                for ident,(kind,body) in blocks.items() if kind==224}
    def hpath(ident):
        game,parent,_=transforms[ident]
        if parent==0:return ''
        prefix=hpath(parent)
        return (prefix+'/' if prefix else '')+names[game]
    paths={hpath(i):body for i,(_,_,body) in transforms.items()}
    layout=json.loads(read(LAYOUT))
    check('layout_schema_unchanged',layout['schemaVersion']==4)
    def rectcheck(path,expected,parent=(0,0,941,1672),density=1):
        body=paths[path]
        position=vec(body,'m_AnchoredPosition');size=vec(body,'m_SizeDelta');scale=vec(body,'m_LocalScale')
        ok=(abs(position['x']-(expected[0]-parent[0]))<.001 and
            abs(position['y']+(expected[1]-parent[1]))<.001 and
            abs(size['x']-expected[2]*density)<.001 and abs(size['y']-expected[3]*density)<.001 and
            abs(scale['x']-1/density)<.00001 and abs(scale['y']-1/density)<.00001)
        for field in ['m_AnchorMin','m_AnchorMax','m_Pivot']:
            v=vec(body,field);ok=ok and v['x']==0 and v['y']==1
        check('layout_'+path,ok)
    for spec in layout['art']:rectcheck(spec['name'],spec['rect'])
    for spec in layout['buttons']:
        rectcheck(spec['name'],spec['rect'])
        rectcheck(spec['name']+'/Artwork',spec['artRect'],spec['rect'])
    buttons={b['name']:b for b in layout['buttons']}
    for spec in layout['texts']:
        parent=buttons[spec['parent']]['rect'] if spec['parent'] else (0,0,941,1672)
        rectcheck((spec['parent']+'/' if spec['parent'] else '')+spec['name'],spec['rect'],parent,spec['rasterScale'])
    for name in ['SettingsButton','AccountButton','HelpButton']:
        size=vec(paths[name+'/Artwork'],'m_SizeDelta')
        check('retained_15_percent_icon_increase_'+name,abs(size['x']-110*1.15)<.001 and abs(size['y']-110*1.15)<.001)

    builder=read(BUILDER)
    check('rebuilder_stamps_current_revision','view.Revision = SplashTitleView.CurrentRevision' in builder)
    check('save_result_is_checked','if (!savedSuccessfully)' in builder)
    check('saved_prefab_readback_checked','savedView == null || !savedView.HasRequiredBindings' in builder)
    check('no_playmode_prefab_writes','if (EditorApplication.isPlayingOrWillChangePlaymode)' in builder)
    check('recheck_after_asset_imports','static void OnPostprocessAllAssets' in builder)
    check('recheck_after_return_to_editmode','state == PlayModeStateChange.EnteredEditMode' in builder)
    check('valid_prefab_is_noop','if (!force && existing != null && existing.HasRequiredBindings) return;' in builder)
    check('no_open_scene_calls','OpenScene(' not in builder)
    check('runtime_resource_path_unchanged','"ProductionUI/Pages/SplashTitle"' in read(LOADER))
    check('no_legacy_screenshot_loader','SplashTitleFinal/' not in read(LOADER))
    return dict(revision='PrefabFix07',passed=all(c['passed'] for c in checks),checks=checks,
                scope='Static serialized files and source checks only. Unity compilation, import, Play Mode, visual comparison and Android builds not executed.',
                component_counts=counts,object_id_count=len(blocks))


def main():
    ap=argparse.ArgumentParser(description=__doc__)
    ap.add_argument('--project',type=Path,default=Path('.'))
    ap.add_argument('--output',type=Path)
    args=ap.parse_args()
    try: result=verify(args.project.resolve())
    except (OSError,ValueError,KeyError,AttributeError,IndexError) as ex:
        result={'revision':'PrefabFix07','passed':False,'error':str(ex)}
    if args.output:
        args.output.parent.mkdir(parents=True,exist_ok=True)
        args.output.write_text(json.dumps(result,indent=2,ensure_ascii=False),encoding='utf-8')
    print(('PASS' if result['passed'] else 'FAIL')+': Splash PrefabFix07 portable verification')
    for item in result.get('checks',[]):
        if not item['passed']: print('FAILED: '+item['name']+' '+item['detail'])
    if 'error' in result:print(result['error'])
    return 0 if result['passed'] else 1

if __name__=='__main__':sys.exit(main())
