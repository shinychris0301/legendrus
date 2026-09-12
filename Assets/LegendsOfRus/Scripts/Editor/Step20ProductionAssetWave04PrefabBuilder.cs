#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>v1.20 Step 10 screenshot-free Wave 4 candidate builder. Parallel/inactive; no account/network authority.</summary>
    public static class Step20ProductionAssetWave04PrefabBuilder
    {
        public const string Version = "v1.20-step10-wave04-account-language-system-ui";
        public const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave04Pages";
        public const string AssetRoot = "Assets/LegendsOfRus/Art/Production/Wave04";
        public const string Wave0Root = "Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation";
        public const string ProductionFontRoot = "Assets/LegendsOfRus/Art/Fonts/Production";
        const float W = 941f, H = 1672f;
        static Font cachedFont;
        static readonly ScreenId[] Pages = { ScreenId.Settings, ScreenId.LanguageSelect, ScreenId.Login, ScreenId.TutorialOverlay, ScreenId.OfflineState, ScreenId.MaintenanceState, ScreenId.EmptyErrorState };

        [InitializeOnLoadMethod] static void QueueBuild() { EditorApplication.delayCall += delegate { if (EditorApplication.isPlayingOrWillChangePlaymode) return; try { if (NeedsBuild()) BuildAll(false); } catch (Exception e) { Debug.LogError("v1.20 Step 10 Wave 4 candidate build failed: " + e.Message); } }; }
        [MenuItem("Legends of Rus/Production UI/Build/Repair v1.20 Step 10 Wave 4 Candidates")] public static void BuildMenu() { BuildAll(true); }
        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 10 Wave 4 Candidates")] public static void ValidateMenu() { var f=ValidateAll(); if(f.Count==0) Debug.Log("v1.20 Step 10 Wave 4 candidates validate: 7/7. Active runtime unchanged."); else throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray())); }
        public static bool NeedsBuild() { string l=CandidateRoot+"/WAVE04_BUILD.lock"; if(!File.Exists(l)||!File.ReadAllText(l).StartsWith(Version,StringComparison.Ordinal)) return true; foreach(ScreenId id in Pages) if(AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot+"/"+id+".prefab")==null) return true; return false; }
        public static void BuildAll(bool showDialog)
        {
            EnsureFolder(CandidateRoot); ConfigureAllPngs(Wave0Root); ConfigureAllPngs(AssetRoot); cachedFont=null;
            foreach(ScreenId id in Pages) BuildPage(id);
            File.WriteAllText(CandidateRoot+"/WAVE04_BUILD.lock",Version+"\nprefabs=7\nactive-runtime-cutover=false\nreferenceonly-dependencies=forbidden\ndesignscreens-dependencies=forbidden\nprojectdocs-dependencies=forbidden\nlive-localized-text=true\naccount-provider-simulation=forbidden\nnetwork-authority-simulation=forbidden\n");
            AssetDatabase.ImportAsset(CandidateRoot+"/WAVE04_BUILD.lock",ImportAssetOptions.ForceUpdate); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            var failures=ValidateAll(); if(failures.Count>0) throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 10 built 7 screenshot-free Wave 4 candidates. Settings uses demo/design guidance; six pages are locked-style derived. No account/network backend activation and no runtime cutover.");
            if(showDialog&&!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus","Step 10 Wave 4 candidates rebuilt: 7/7. Settings is demo-guided; six pages are style-system derived. Runtime cutover remains blocked pending runtime QA/reference authority.","OK");
        }
        public static List<string> ValidateAll()
        {
            var failures=new List<string>(); if(Pages.Length!=7) failures.Add("Wave 4 must contain exactly 7 pages.");
            foreach(ScreenId id in Pages)
            {
                string path=CandidateRoot+"/"+id+".prefab"; GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path); if(prefab==null){failures.Add("Missing Wave 4 candidate: "+path);continue;}
                var marker=prefab.GetComponent<ProductionCoreLoopPageMarker>(); if(marker==null||marker.ScreenId!=id||!marker.ScreenshotFree) failures.Add(path+": screenshot-free marker missing/mismatch.");
                if(prefab.GetComponentsInChildren<Text>(true).Length==0) failures.Add(path+": no live Text components.");
                if(prefab.GetComponentsInChildren<Button>(true).Length!=0) failures.Add(path+": visual candidate must not contain Button behavior.");
                foreach(Text t in prefab.GetComponentsInChildren<Text>(true)) if(t.raycastTarget) failures.Add(path+": text intercepts input: "+t.name);
                foreach(string dep in AssetDatabase.GetDependencies(path,true)){string n=dep.Replace('\\','/'); if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/")) failures.Add(path+": forbidden reference/QA dependency "+dep);}
            }
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Accepted core-loop production registry changed; expected 10 routes.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP10_WAVE04_ASSET_MANIFEST.json")) failures.Add("Missing Step 10 Wave 4 asset manifest.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP10_WAVE04_REQUIREMENT_COVERAGE.json")) failures.Add("Missing Step 10 requirement coverage.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP10_REFERENCE_AUTHORITY.json")) failures.Add("Missing Step 10 reference authority manifest.");
            return failures;
        }
        static void BuildPage(ScreenId id)
        {
            GameObject root=new GameObject("Wave04_"+id,typeof(RectTransform)); root.layer=5; RectTransform rt=root.GetComponent<RectTransform>(); Stretch(rt); var marker=root.AddComponent<ProductionCoreLoopPageMarker>(); marker.Configure(id);
            switch(id){case ScreenId.Settings: BuildSettings(rt); break; case ScreenId.LanguageSelect: BuildLanguage(rt); break; case ScreenId.Login: BuildLogin(rt); break; case ScreenId.TutorialOverlay: BuildTutorial(rt); break; case ScreenId.OfflineState: BuildOffline(rt); break; case ScreenId.MaintenanceState: BuildMaintenance(rt); break; case ScreenId.EmptyErrorState: BuildError(rt); break;}
            PrefabUtility.SaveAsPrefabAsset(root,CandidateRoot+"/"+id+".prefab"); UnityEngine.Object.DestroyImmediate(root);
        }
        static void Background(RectTransform r,string folder){Page(r,folder,"background_plate.png",0,0,941,1672);}
        static void BuildSettings(RectTransform r)
        {
            Background(r,"Settings"); Shared(r,"Panels/top_bar.png",0,0,941,145); Shared(r,"Icons/icon_back.png",24,42,58,58); Shared(r,"Icons/icon_coin.png",385,42,48,48); Shared(r,"Icons/icon_gem.png",610,42,48,48); Shared(r,"Icons/icon_energy.png",805,42,48,48); HudT(r,"12 540",435,42,120,48,18); HudT(r,"350",660,42,85,48,18); HudT(r,"25/30",850,42,78,48,16);
            Page(r,"Settings","header_shell.png",90,145,760,160); DisplayT(r,"НАСТРОЙКИ",175,180,590,90,34); Page(r,"Settings","settings_panel.png",65,330,810,480);
            string[] labels={"Музыка","Звуки","Вибрация","Уведомления"}; string[] icons={"music","sound","vibrate","notify"};
            for(int i=0;i<4;i++){float y=365+i*105; Page(r,"Settings","icon_"+icons[i]+".png",120,y,72,72); BodyDarkT(r,labels[i],220,y+8,300,55,20); Page(r,"Settings","toggle_on.png",655,y+6,145,60); Page(r,"Settings","toggle_knob.png",742,y+7,58,58);}
            string[] nav={"Язык","Облако","Аккаунт","Поддержка","Конфиденциальность"}; string[] navI={"language","cloud","account","support","privacy"}; float ny=835;
            for(int i=0;i<nav.Length;i++){Page(r,"Settings","nav_row.png",80,ny,780,92); Page(r,"Settings","icon_"+navI[i]+".png",100,ny+8,72,72); HeadingT(r,nav[i],205,ny+17,430,55,19); Shared(r,"Icons/icon_play.png",790,ny+18,50,50); ny+=105;}
            Page(r,"Settings","logout_row.png",80,1360,780,100); HeadingT(r,"ВЫЙТИ ИЗ АККАУНТА",210,1377,530,55,20); Shared(r,"Icons/icon_back.png",790,1383,50,50); Shared(r,"Controls/back_button.png",385,1510,170,130);
        }
        static void BuildLanguage(RectTransform r)
        {
            Background(r,"LanguageSelect"); Page(r,"LanguageSelect","header_shell.png",110,180,720,150); DisplayT(r,"ЯЗЫК",220,210,500,75,34); string[] labels={"Русский","English"}; string[] flags={"ru","en"};
            for(int i=0;i<2;i++){float y=560+i*170; Page(r,"LanguageSelect",i==0?"language_row_selected.png":"language_row_idle.png",80,y,780,135); Page(r,"LanguageSelect","flag_"+flags[i]+".png",120,y+28,120,76); HeadingT(r,labels[i],290,y+30,320,65,23); if(i==0) Page(r,"LanguageSelect","selected_check.png",760,y+35,64,64); else Page(r,"LanguageSelect","download_indicator.png",756,y+31,72,72);}
            BodyLightT(r,"Дополнительные языки будут доступны позже",150,1080,640,80,16); Shared(r,"Controls/back_button.png",385,1470,170,130);
        }
        static void BuildLogin(RectTransform r)
        {
            Background(r,"Login"); Page(r,"Login","logo_plate.png",130,170,680,150); DisplayT(r,"ЛЕГЕНДЫ РУСИ",185,205,570,72,30); DisplayT(r,"Вход в игру",260,405,420,70,28);
            Page(r,"Login","provider_button_shell.png",120,600,700,100); HeadingT(r,"ВОЙТИ ЧЕРЕЗ ПРОВАЙДЕРА",170,620,600,55,17); Page(r,"Login","provider_button_shell.png",120,720,700,100); HeadingT(r,"ОБЛАЧНЫЙ АККАУНТ",200,740,540,55,17); Page(r,"Login","recovery_link_frame.png",230,860,480,70); HudT(r,"Восстановить аккаунт",270,873,400,42,14);
            Page(r,"Login","guest_button_shell.png",120,1080,700,115); DisplayT(r,"ПРОДОЛЖИТЬ КАК ГОСТЬ",175,1105,590,62,20); Page(r,"Login","legal_strip.png",90,1235,760,120); BodyLightT(r,"Продолжая, вы принимаете условия использования\nи политику конфиденциальности",130,1253,680,75,13); Shared(r,"Controls/back_button.png",385,1470,170,130);
        }
        static void BuildTutorial(RectTransform r)
        {
            Background(r,"TutorialOverlay"); Page(r,"TutorialOverlay","spotlight_mask.png",0,0,941,1672); Page(r,"TutorialOverlay","highlight_frame.png",210,570,520,320); Page(r,"TutorialOverlay","hand_pointer.png",640,845,150,190); Page(r,"TutorialOverlay","arrow.png",520,980,160,100); Page(r,"TutorialOverlay","callout_panel.png",130,1110,680,260); BodyDarkT(r,"Сделайте свайп, чтобы поменять камни местами.\nСоберите три одинаковых символа в ряд.",190,1155,560,120,17); Page(r,"TutorialOverlay","step_dots.png",365,1385,210,50); Page(r,"TutorialOverlay","skip_button_shell.png",100,1480,260,80); Page(r,"TutorialOverlay","next_button_shell.png",580,1480,260,80); HeadingT(r,"ПРОПУСТИТЬ",120,1497,220,45,14); HeadingT(r,"ДАЛЕЕ",600,1497,220,45,14);
        }
        static void BuildOffline(RectTransform r)
        {
            Background(r,"OfflineState"); Page(r,"OfflineState","header_shell.png",110,190,720,150); DisplayT(r,"НЕТ СОЕДИНЕНИЯ",175,220,590,70,28); Page(r,"OfflineState","status_panel.png",90,520,760,480); Page(r,"OfflineState","illustration.png",375,575,190,190); BodyDarkT(r,"Проверьте подключение к сети.\nНекоторые онлайн-функции временно недоступны.",180,815,580,100,16); Page(r,"OfflineState","retry_button_shell.png",260,1090,420,90); DisplayT(r,"ПОВТОРИТЬ",310,1110,320,50,18); Page(r,"OfflineState","continue_offline_shell.png",260,1200,420,90); HeadingT(r,"ИГРАТЬ ОФЛАЙН",300,1220,340,50,17); Shared(r,"Controls/back_button.png",385,1470,170,130);
        }
        static void BuildMaintenance(RectTransform r)
        {
            Background(r,"MaintenanceState"); Page(r,"MaintenanceState","header_shell.png",110,190,720,150); DisplayT(r,"ТЕХНИЧЕСКИЕ РАБОТЫ",150,220,640,70,26); Page(r,"MaintenanceState","status_panel.png",90,520,760,480); Page(r,"MaintenanceState","illustration.png",375,575,190,190); BodyDarkT(r,"Сервер временно недоступен.\nМы уже работаем над восстановлением.",180,815,580,100,16); Page(r,"MaintenanceState","estimated_time_badge.png",285,1010,370,80); HeadingT(r,"~ 30 МИНУТ",325,1028,290,42,16); Page(r,"MaintenanceState","retry_button_shell.png",100,1160,360,90); Page(r,"MaintenanceState","home_button_shell.png",480,1160,360,90); DisplayT(r,"ПОВТОРИТЬ",135,1180,290,48,17); HeadingT(r,"НА ГЛАВНУЮ",515,1180,290,48,17); Shared(r,"Controls/back_button.png",385,1470,170,130);
        }
        static void BuildError(RectTransform r)
        {
            Background(r,"EmptyErrorState"); Page(r,"EmptyErrorState","header_shell.png",110,190,720,150); DisplayT(r,"ЧТО-ТО ПОШЛО НЕ ТАК",155,220,630,70,25); Page(r,"EmptyErrorState","status_panel.png",90,520,760,480); Page(r,"EmptyErrorState","illustration.png",375,575,190,190); BodyDarkT(r,"Не удалось загрузить данные.\nПопробуйте ещё раз или вернитесь назад.",180,815,580,100,16); Page(r,"EmptyErrorState","variant_badge.png",360,1005,220,60); HeadingT(r,"ERROR",385,1015,170,36,14); Page(r,"EmptyErrorState","retry_button_shell.png",130,1110,320,90); Page(r,"EmptyErrorState","back_button_shell.png",490,1110,320,90); DisplayT(r,"ПОВТОРИТЬ",160,1130,260,48,17); HeadingT(r,"НАЗАД",530,1130,240,48,17); Shared(r,"Controls/back_button.png",385,1470,170,130);
        }
        static void Page(RectTransform r,string folder,string file,float x,float y,float w,float h){Img(r,AssetRoot+"/"+folder+"/"+file,x,y,w,h);} static void Shared(RectTransform r,string rel,float x,float y,float w,float h){Img(r,Wave0Root+"/"+rel,x,y,w,h);}
        static void Img(RectTransform r,string path,float x,float y,float w,float h){Sprite s=AssetDatabase.LoadAssetAtPath<Sprite>(path); if(s==null) throw new InvalidOperationException("Missing Sprite: "+path); GameObject go=new GameObject(Path.GetFileNameWithoutExtension(path),typeof(RectTransform),typeof(Image)); go.layer=5; go.transform.SetParent(r,false); Set(go.GetComponent<RectTransform>(),x,y,w,h); Image im=go.GetComponent<Image>(); im.sprite=s; im.type=Image.Type.Simple; im.preserveAspect=false; im.raycastTarget=false;}
        static void DisplayT(RectTransform r,string v,float x,float y,float w,float h,int s){TextImpl(r,v,x,y,w,h,s,new Color32(244,205,119,255),new Color32(64,28,9,245),true);} static void HeadingT(RectTransform r,string v,float x,float y,float w,float h,int s){TextImpl(r,v,x,y,w,h,s,new Color32(237,203,132,255),new Color32(50,25,10,235),true);} static void HudT(RectTransform r,string v,float x,float y,float w,float h,int s){TextImpl(r,v,x,y,w,h,s,new Color32(244,224,180,255),new Color32(15,18,23,230),false);} static void BodyLightT(RectTransform r,string v,float x,float y,float w,float h,int s){TextImpl(r,v,x,y,w,h,s,new Color32(239,218,176,255),new Color32(20,20,20,210),false);} static void BodyDarkT(RectTransform r,string v,float x,float y,float w,float h,int s){TextImpl(r,v,x,y,w,h,s,new Color32(73,45,27,255),new Color32(225,194,133,0),false);}
        static void TextImpl(RectTransform r,string value,float x,float y,float w,float h,int size,Color color,Color outline,bool strong){GameObject go=new GameObject("LiveText",typeof(RectTransform),typeof(Text)); go.layer=5; go.transform.SetParent(r,false); Set(go.GetComponent<RectTransform>(),x,y,w,h); Text t=go.GetComponent<Text>(); t.text=value; t.font=ResolveFont(); t.fontSize=size; t.fontStyle=strong?FontStyle.Bold:FontStyle.Normal; t.alignment=TextAnchor.MiddleCenter; t.color=color; t.horizontalOverflow=HorizontalWrapMode.Wrap; t.verticalOverflow=VerticalWrapMode.Overflow; t.raycastTarget=false; if(outline.a>.01f){Outline o=go.AddComponent<Outline>(); o.effectColor=outline; o.effectDistance=strong?new Vector2(2,-2):new Vector2(1,-1); o.useGraphicAlpha=true; Shadow sh=go.AddComponent<Shadow>(); sh.effectColor=new Color(0f,0f,0f,strong ? 0.55f : 0.35f); sh.effectDistance=new Vector2(1,-2); sh.useGraphicAlpha=true;}}
        static Font ResolveFont(){if(cachedFont!=null)return cachedFont; if(AssetDatabase.IsValidFolder(ProductionFontRoot)){string[] gs=AssetDatabase.FindAssets("t:Font",new[]{ProductionFontRoot}); if(gs!=null&&gs.Length>0){Array.Sort(gs,StringComparer.Ordinal); cachedFont=AssetDatabase.LoadAssetAtPath<Font>(AssetDatabase.GUIDToAssetPath(gs[0]));}} if(cachedFont==null) cachedFont=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); if(cachedFont==null) cachedFont=Resources.GetBuiltinResource<Font>("Arial.ttf"); return cachedFont;}
        static void Set(RectTransform rt,float x,float y,float w,float h){rt.anchorMin=new Vector2(x/W,1f-(y+h)/H); rt.anchorMax=new Vector2((x+w)/W,1f-y/H); rt.offsetMin=Vector2.zero; rt.offsetMax=Vector2.zero;} static void Stretch(RectTransform rt){rt.anchorMin=Vector2.zero;rt.anchorMax=Vector2.one;rt.offsetMin=Vector2.zero;rt.offsetMax=Vector2.zero;}
        static void ConfigureAllPngs(string root){string abs=Path.Combine(Directory.GetCurrentDirectory(),root.Replace('/',Path.DirectorySeparatorChar)); if(!Directory.Exists(abs))return; foreach(string f in Directory.GetFiles(abs,"*.png",SearchOption.AllDirectories)){string asset=f.Replace('\\','/'); string prefix=Directory.GetCurrentDirectory().Replace('\\','/').TrimEnd('/')+"/"; if(asset.StartsWith(prefix,StringComparison.OrdinalIgnoreCase))asset=asset.Substring(prefix.Length); TextureImporter ti=AssetImporter.GetAtPath(asset) as TextureImporter; if(ti==null)continue; bool dirty=false; if(ti.textureType!=TextureImporterType.Sprite){ti.textureType=TextureImporterType.Sprite;dirty=true;} if(ti.spriteImportMode!=SpriteImportMode.Single){ti.spriteImportMode=SpriteImportMode.Single;dirty=true;} if(!ti.alphaIsTransparency){ti.alphaIsTransparency=true;dirty=true;} if(ti.isReadable){ti.isReadable=false;dirty=true;} if(ti.mipmapEnabled){ti.mipmapEnabled=false;dirty=true;} if(ti.wrapMode!=TextureWrapMode.Clamp){ti.wrapMode=TextureWrapMode.Clamp;dirty=true;} if(dirty)ti.SaveAndReimport();}}
        static void EnsureFolder(string path){string[] parts=path.Split('/'); string cur=parts[0]; for(int i=1;i<parts.Length;i++){string next=cur+"/"+parts[i]; if(!AssetDatabase.IsValidFolder(next))AssetDatabase.CreateFolder(cur,parts[i]); cur=next;}}
    }
}
#endif
