"""Apply the precise approved owner/prefab cleanup and byte-preserving archive moves."""
from pathlib import Path
import re,json,hashlib
ROOT=Path(__file__).resolve().parents[5];BASE=Path(__file__).resolve().parent.parent;QA=BASE/'QA'
before=json.loads((QA/'BeforeHashes.json').read_text())
for rel,digest in before.items():
 p=ROOT/rel
 assert p.exists() and hashlib.file_digest(p.open('rb'),'sha256').hexdigest()==digest,rel
rows=json.loads((QA/'CandidatesBefore.json').read_text());names=[r['name'] for r in rows]
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeRuntime.cs';s=p.read_text()
start=s.index('        static readonly string[] TextureNames = {');end=s.index('        static readonly string[] TextureKeys',start)
s=s[:start]+'''        static readonly string[] TextureNames = {
            "home_logo", "avatar_portrait", "avatar_ring",
            "resource_capsule", "coin_icon", "gems_icon", "energy_icon", "settings_medallion",
            "resource_plus"
        };
'''+s[end:]
s=s.replace('ClearDormantPlayArtwork(existing.PlayButton); return;', 'ClearDormantPlayArtwork(existing.PlayButton); ClearDormantArtwork(existing); return;')
for n in names:s=s.replace('sprites["'+n+'"]','null')
s=s.replace('            string[] navNames = { "nav_map_generated", "nav_shop_generated", "nav_events_generated", "nav_characters_generated" };\n','').replace('sprites[navNames[i]]','null')
s=s.replace('bool gift = sprite.name == "reward_medallion_generated";', 'bool gift = button.name == "DailyRewardButton";')
marker='        internal static void ClearDormantProgress(ProductionHomeView view)'
helper='''        // Run after current source targets are assigned. Keep every native object/component.
        internal static void ClearDormantArtwork(ProductionHomeView view)
        {
            foreach (var button in new[] { view.WorldMapButton, view.ShopButton, view.EventsButton, view.CharactersButton })
                ClearDormantPlayArtwork(button);
            foreach (var button in new[] { view.MailButton, view.DailyRewardButton })
            {
                var old = button.transform.Find("Medallion");
                var image = old != null ? old.GetComponent<Image>() : null;
                if (image != null && image != button.targetGraphic) image.sprite = null;
            }
            foreach (var image in new[] { view.QuestRewardFrame, view.QuestRewardIcon, view.MapFoliage })
                if (image != null && !image.enabled) image.sprite = null;
        }

'''
assert marker in s;s=s.replace(marker,helper+marker);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeSourceComposition.cs';s=p.read_text().replace('            layout.SourceComposition=true;','            ProductionHomeRuntime.ClearDormantArtwork(v);\n            layout.SourceComposition=true;');p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Runtime/UI/ProductionHomeView.cs';s=p.read_text();start=s.index('        public bool HasSceneLightingBindings');end=s.index('        // The historical control diagnostic',start)
s=s[:start]+'''        // Current source art does not use the historical lighting effects or foliage overlay.
        public bool HasSceneLightingBindings => HasRequiredBindings &&
            HasDisabledLighting(Ivan) && HasDisabledLighting(MapFoliage);

        static bool HasDisabledLighting(Image image)
        {
            var lighting = image != null ? image.GetComponent<ProductionHomeSceneLighting>() : null;
            return lighting != null && !lighting.enabled;
        }

        public bool HasReferenceBackdropBindings => HasRequiredBindings &&
            BackgroundShade != null && !BackgroundShade.enabled &&
            MapFoliage != null && !MapFoliage.enabled && MapFoliage.sprite == null;

'''+s[end:]
start=s.index('        static bool HasArtwork');end=s.index('        readonly Button[] boundButtons',start);s=s[:start]+s[end:];p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs';s=p.read_text();old='                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/footer_base_generated.png")';new=old[:-1]+' ||\n'+' ||\n'.join('                assetPath == "Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome/'+n+'.png"' for n in names)+')';assert old in s;s=s.replace(old,new);p.write_text(s)
p=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/Pages/Home.prefab';s=p.read_text();changes=[]
for r in rows:
 pattern=r'  m_Sprite: \{fileID: 21300000, guid: '+r['guid']+r', type: 3\}'
 s,count=re.subn(pattern,'  m_Sprite: {fileID: 0}',s);assert count==1,(r,count);changes.append({'name':r['name'],'guid':r['guid'],'clearedCount':count})
p.write_text(s);(QA/'PrefabClearing.json').write_text(json.dumps(changes,indent=2)+'\n')
for n in names:
 for ext in ('.png','.png.meta'):
  old=ROOT/'Assets/LegendsOfRus/Resources/ProductionUI/HomeAligned'/(n+ext);new=ROOT/'Assets/LegendsOfRus/Art/ReferenceOnly/RetiredHome'/(n+ext);assert not new.exists();old.rename(new)
for n in ('RetiredHomeHeaderQuestEditModeTests','RetiredHomeWorldMapFooterEditModeTests'):
 p=ROOT/'Assets/LegendsOfRus/Tests/ProductionCoreLoopUiEditMode'/(n+'.cs');s=p.read_text().replace('?0:18','?0:9').replace(',"quest_reward_frame"','').replace('MissingUnrelatedQuestRewardStillBlocksFreshAndLegacyConstruction','MissingUnrelatedCapsuleStillBlocksFreshAndLegacyConstruction').replace('ArtRoot+"quest_reward_frame"?null','ArtRoot+"resource_capsule"?null');p.write_text(s)
print('Owners aligned; exactly nine prefab references cleared and nine PNG/meta pairs moved.')
