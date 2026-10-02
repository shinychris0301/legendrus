from pathlib import Path
import json
b=Path('/tmp/lor-splash-reference-r1/Candidate');b.mkdir(exist_ok=True)
def stage(path,s):
 p=b/path;p.parent.mkdir(parents=True,exist_ok=True);p.write_text(s)
p='Assets/LegendsOfRus/Resources/ProductionUI/SplashTitleConfig/SplashTitleLayout.json';d=json.loads(Path(p).read_text());art='ProductionUI/SplashReferenceAligned/'
for a in d['art']:
 if a['name']=='BackgroundEnvironment':a.update(file=art+'clean_environment.png',rect=[0,0,941,1672],color=[1,1,1,1])
 elif a['name']=='Logo':a.update(file=art+'source_logo.png',rect=[66,88,806,601],color=[1,1,1,1])
 elif a['name']=='Ivan':a.update(file=art+'source_ivan_firebird.png',rect=[16,644,910,1010],fadeBottom=0,color=[1,1,1,1])
 else:a['hidden']=True
# A separate transparent ornament; never a page screenshot.
d['art'].append({'name':'SourceOuterFrame','file':art+'source_outer_frame.png','rect':[0,0,941,1672],'color':[1,1,1,1],'fadeBottom':0})
for a in d['buttons']:
 if a['name']=='StartButton':a.update(file=art+'source_start_blank.png',rect=[159,1356,629,198],artRect=[159,1356,629,198])
 else:a['hidden']=True
for a in d['texts']:
 if a['name']=='StartText':a.update(rect=[229,1435,493,49],size=36,text='НАЖМИТЕ, ЧТОБЫ НАЧАТЬ',fontStyle=0)
 elif a['name']=='VersionText':a.update(rect=[414,1570,113,38],size=26,alignment=4,centerHorizontally=True)
 else:a['hidden']=True
# No extra tagline or contrast band is shown in the selected original.
d['footerShade']['color']=[0,0,0,0];d['implementationRevision']='SplashReferenceRebuild-R1';stage(p,json.dumps(d,ensure_ascii=False,indent=2)+'\n')
p='Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleView.cs';s=Path(p).read_text().replace('CurrentRevision = 7','CurrentRevision = 8').replace('HasHomeStyleBindings','HasReferenceStyleBindings');s=s.replace('HasArt("Logo", "home_logo")','HasArt("Logo", "source_logo")').replace('HasArt("BackgroundEnvironment", "home_environment_generated")','HasArt("BackgroundEnvironment", "clean_environment")').replace('HasArt("Ivan", "ivan_splash_dusk") && HasArt("Masha", "masha_dusk")','HasArt("Ivan", "source_ivan_firebird") && HasArt("SourceOuterFrame", "source_outer_frame")').replace('start.sprite.name == "home_play_button"','start.sprite.name == "source_start_blank"').replace('Missing shared Home-style artwork or Start lettering','Missing original-reference artwork or live Start lettering').replace('en ? "Start" : "Начать"','en ? "TAP TO START" : "НАЖМИТЕ, ЧТОБЫ НАЧАТЬ"');s=s.replace('        public SplashTitleTypography Typography;','        public SplashTitleTypography Typography;\n        public Font ReferenceTitleFont, ReferenceVersionFont;').replace('public bool HasReferenceStyleBindings =>','public bool HasReferenceStyleBindings => ReferenceTitleFont != null && ReferenceVersionFont != null &&').replace('Font regular = Typography.Resolve(false, out source);','Font regular = ReferenceVersionFont; source = regular.name + " (reference identity unverified)";').replace('StartText.font = Typography.Resolve(true, out source);','StartText.font = ReferenceTitleFont;');stage(p,s)
p='Assets/LegendsOfRus/Scripts/Runtime/UI/SplashTitleForegroundLayout.cs';s=Path(p).read_text().replace('        public RectTransform Logo;','        public bool ReferenceComposition;\n        public RectTransform Logo;');s=s.replace('Vector2 shift = Vector2.up * gap;','Vector2 shift = ReferenceComposition ? Vector2.zero : Vector2.up * gap;');stage(p,s)
p='Assets/LegendsOfRus/Scripts/Editor/Step20SplashTitleR3PrefabRebuilder.cs';s=Path(p).read_text().replace('ApplyHomeStyle','ApplyReferenceStyle');a=s.index('        static void ApplyReferenceStyle(');z=s.index('\n        static void BindResponsiveLayout',a)
s=s[:a]+'''        static void ApplyReferenceStyle(GameObject root)
        {
            const string referenceArt="Assets/LegendsOfRus/Resources/ProductionUI/SplashReferenceAligned/";
            foreach(string file in Directory.GetFiles(referenceArt,"*.png")) {
                var importer=(TextureImporter)AssetImporter.GetAtPath(file);
                importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;
                importer.alphaIsTransparency=true;importer.mipmapEnabled=false;importer.textureCompression=TextureImporterCompression.Uncompressed;
                importer.npotScale=TextureImporterNPOTScale.None;importer.maxTextureSize=2048;importer.filterMode=FilterMode.Bilinear;
                importer.wrapMode=TextureWrapMode.Clamp;importer.spritePixelsPerUnit=100;importer.spriteBorder=Vector4.zero;importer.SaveAndReimport();
            }
            LayoutDoc doc=JsonUtility.FromJson<LayoutDoc>(File.ReadAllText(LayoutPath));
            if(doc==null||doc.schemaVersion!=4)throw new InvalidOperationException("Unsupported Splash layout schema.");
            Rect whole=new Rect(0,0,doc.referenceWidth,doc.referenceHeight);
            var rootRect=(RectTransform)root.transform;
            foreach(ArtSpec item in doc.art) {
                var child=root.transform.Find(item.name);
                Image image=child?child.GetComponent<Image>():MakeImage(rootRect,item.name,RequireSprite(item.file),RectOf(item.rect),whole,false);
                image.sprite=RequireSprite(item.file);ApplyRect(image.rectTransform,RectOf(item.rect),whole,1);image.preserveAspect=true;
                image.color=item.color!=null&&item.color.Length==4?new Color(item.color[0],item.color[1],item.color[2],item.color[3]):Color.white;
                image.raycastTarget=false;image.gameObject.SetActive(!item.hidden);
                var fade=image.GetComponent<SplashTitleCharacterFade>();if(fade){fade.BottomFraction=item.fadeBottom;fade.enabled=item.fadeBottom>0;}
            }
            var view=root.GetComponent<SplashTitleView>();
            foreach(ButtonSpec item in doc.buttons) {
                var tr=(RectTransform)root.transform.Find(item.name);Rect bounds=RectOf(item.rect);ApplyRect(tr,bounds,whole,1);
                var image=tr.Find("Artwork").GetComponent<Image>();image.sprite=RequireSprite(item.file);
                ApplyRect(image.rectTransform,RectOf(item.artRect),bounds,1);image.preserveAspect=true;tr.gameObject.SetActive(!item.hidden);
            }
            foreach(TextSpec item in doc.texts) {
                var parent=string.IsNullOrEmpty(item.parent)?root.transform:root.transform.Find(item.parent);
                var text=parent.Find(item.name).GetComponent<Text>();Rect parentBounds=whole;
                foreach(ButtonSpec button in doc.buttons)if(button.name==item.parent)parentBounds=RectOf(button.rect);
                int density=Mathf.Clamp(item.rasterScale,1,3);ApplyRect(text.rectTransform,RectOf(item.rect),parentBounds,density);
                text.fontSize=item.size*density;text.fontStyle=(FontStyle)item.fontStyle;text.alignment=(TextAnchor)item.alignment;
                text.text=item.text;text.raycastTarget=false;text.gameObject.SetActive(!item.hidden);
                var geometry=text.GetComponent<SplashTitleTextGeometry>();geometry.CenterHorizontally=item.centerHorizontally;
                geometry.CenterVertically=item.centerVertically;geometry.OpticalOffset=new Vector2(item.opticalOffset[0],item.opticalOffset[1])*density;
            }
            view.ReferenceVersionFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/RankNumbers");
            view.ReferenceTitleFont=Resources.Load<Font>("ProductionUI/IndividualRankingFonts/Title");
            // These are the existing selected fonts, not an assertion of exact source-font identity.
            if(!view.ReferenceVersionFont||!view.ReferenceTitleFont)throw new InvalidOperationException("Missing embedded Cyrillic fonts.");

            view.StartText.color=new Color32(244,199,100,255);
            view.StartText.material=AssetDatabase.LoadAssetAtPath<Material>("Assets/LegendsOfRus/Resources/"+ProductionHomeRuntime.PlayGoldMaterialResource+".mat");
            var gold=view.StartText.GetComponent<ProductionHomeGoldText>()??view.StartText.gameObject.AddComponent<ProductionHomeGoldText>();gold.enabled=true;gold.BeveledFace=true;
            var shadow=view.StartText.GetComponent<Shadow>()??view.StartText.gameObject.AddComponent<Shadow>();shadow.effectColor=new Color32(34,15,2,230);shadow.effectDistance=new Vector2(0,-2);
            root.transform.Find("FooterContrast").GetComponent<SplashTitleFooterShade>().color=Color.clear;
            root.transform.Find("TaglineDivider").gameObject.SetActive(false);
            root.transform.Find("SourceOuterFrame").SetAsLastSibling();view.StartButton.transform.SetAsLastSibling();view.VersionText.transform.SetAsLastSibling();
        }
        static void ApplyRect(RectTransform tr,Rect r,Rect parent,int density)
        {
            tr.anchorMin=tr.anchorMax=tr.pivot=new Vector2(0,1);tr.anchoredPosition=new Vector2(r.x-parent.x,-(r.y-parent.y));
            tr.sizeDelta=r.size*density;tr.localScale=new Vector3(1f/density,1f/density,1);
        }
''' +s[z:]
s=s.replace('            layout.Logo =','            layout.ReferenceComposition = true;\n            layout.Logo =').replace('public float fadeBottom;}','public float fadeBottom;public bool hidden;}').replace('public float[] artRect;}','public float[] artRect;public bool hidden;}').replace('public bool startFont,','public bool hidden;public bool startFont,').replace('Splash Home-style alignment saved','Splash original-reference alignment saved').replace('; existing object identities and control rectangles preserved.','; existing object identities preserved; source composition applied.')
# Ensure first-build branch imports the new resources too before requiring sprites.
s=s.replace('            foreach (ArtSpec item in doc.art) RequireSprite(item.file);','            AssetDatabase.Refresh();\n            foreach (ArtSpec item in doc.art) RequireSprite(item.file);')
stage(p,s)
print('Staged owning builder, revision8 view, reference composition layout and configuration outside Assets.')
