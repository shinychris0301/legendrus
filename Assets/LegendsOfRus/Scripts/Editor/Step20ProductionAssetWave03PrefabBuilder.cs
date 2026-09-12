#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>v1.20 Step 9 screenshot-free Wave 3 candidate builder. Parallel/inactive; no social/LiveOps authority.</summary>
    public static class Step20ProductionAssetWave03PrefabBuilder
    {
        public const string Version = "v1.20-step09.3-reference-accurate-wave03-refinement";
        public const string CandidateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Wave03Pages";
        public const string AssetRoot = "Assets/LegendsOfRus/Art/Production/Wave03";
        public const string RefinedRoot = "Assets/LegendsOfRus/Art/Production/Wave03Refined";
        public const string Wave0Root = "Assets/LegendsOfRus/Art/Production/UI/Wave0Foundation";
        public const string ProductionFontRoot = "Assets/LegendsOfRus/Art/Fonts/Production";
        const float W = 941f, H = 1672f;
        static Font cachedFont;
        static readonly ScreenId[] Pages = { ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub, ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail };

        [InitializeOnLoadMethod] static void QueueBuild() { EditorApplication.delayCall += delegate { if (EditorApplication.isPlayingOrWillChangePlaymode) return; try { if (NeedsBuild()) BuildAll(false); } catch (Exception e) { Debug.LogError("v1.20 Step 9 Wave 3 candidate build failed: " + e.Message); } }; }
        [MenuItem("Legends of Rus/Production UI/Build/Repair v1.20 Step 9.3 Refined Wave 3 Candidates")] public static void BuildMenu() { BuildAll(true); }
        [MenuItem("Legends of Rus/Production UI/Validate v1.20 Step 9.3 Refined Wave 3 Candidates")] public static void ValidateMenu() { var f=ValidateAll(); if(f.Count==0) Debug.Log("v1.20 Step 9 Wave 3 candidates validate: 8/8. Active runtime unchanged."); else throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray())); }
        public static bool NeedsBuild() { string l=CandidateRoot+"/WAVE03_BUILD.lock"; if(!File.Exists(l)||!File.ReadAllText(l).StartsWith(Version,StringComparison.Ordinal)) return true; foreach(ScreenId id in Pages) if(AssetDatabase.LoadAssetAtPath<GameObject>(CandidateRoot+"/"+id+".prefab")==null) return true; return false; }
        public static void BuildAll(bool showDialog)
        {
            EnsureFolder(CandidateRoot); ConfigureAllPngs(Wave0Root); ConfigureAllPngs(AssetRoot); ConfigureAllPngs(RefinedRoot); cachedFont=null;
            foreach(ScreenId id in Pages) BuildPage(id);
            File.WriteAllText(CandidateRoot+"/WAVE03_BUILD.lock",Version+"\nprefabs=8\nactive-runtime-cutover=false\nreferenceonly-dependencies=forbidden\ndesignscreens-dependencies=forbidden\nprojectdocs-dependencies=forbidden\nlive-localized-text=true\nsectional-guidance-reconstruction=true\nserver-authority-simulation=forbidden\n");
            AssetDatabase.ImportAsset(CandidateRoot+"/WAVE03_BUILD.lock",ImportAssetOptions.ForceUpdate); AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            var failures=ValidateAll(); if(failures.Count>0) throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 9.3 built 8 refined screenshot-free Wave 3 candidates. Demo-guided Events/Mail use semantic sectional reconstruction; no active runtime cutover and no social/LiveOps backend simulation.");
            if(showDialog&&!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus","Step 9.3 refined Wave 3 candidates rebuilt: 8/8. Events/Mail are demo-guided sectional reconstructions; six pages remain locked-style derived. Runtime cutover remains blocked because 0/8 pages have direct approved references.","OK");
        }
        public static List<string> ValidateAll()
        {
            var failures=new List<string>(); if(Pages.Length!=8) failures.Add("Wave 3 must contain exactly 8 pages.");
            foreach(ScreenId id in Pages)
            {
                string path=CandidateRoot+"/"+id+".prefab"; GameObject prefab=AssetDatabase.LoadAssetAtPath<GameObject>(path); if(prefab==null){failures.Add("Missing Wave 3 candidate: "+path);continue;}
                var marker=prefab.GetComponent<ProductionCoreLoopPageMarker>(); if(marker==null||marker.ScreenId!=id||!marker.ScreenshotFree) failures.Add(path+": screenshot-free marker missing/mismatch.");
                if(prefab.GetComponentsInChildren<Text>(true).Length==0) failures.Add(path+": no live Text components.");
                if(prefab.GetComponentsInChildren<Button>(true).Length!=0) failures.Add(path+": visual candidate must not contain Button behavior.");
                foreach(Text t in prefab.GetComponentsInChildren<Text>(true)) if(t.raycastTarget) failures.Add(path+": text intercepts input: "+t.name);
                foreach(string dep in AssetDatabase.GetDependencies(path,true)){string n=dep.Replace('\\','/'); if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/")) failures.Add(path+": forbidden reference/QA dependency "+dep);}
            }
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Accepted core-loop production registry changed; expected 10 routes.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_WAVE03_ASSET_MANIFEST.json")) failures.Add("Missing Step 9 Wave 3 asset manifest.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_REFERENCE_AUTHORITY.json")) failures.Add("Missing Step 9 reference authority manifest.");
            if(!File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP09_3_REFINEMENT_MANIFEST.json")) failures.Add("Missing Step 9.3 refinement manifest.");
            if(!File.Exists(RefinedRoot+"/wave03_refined_manifest.json")) failures.Add("Missing Step 9.3 refined asset manifest.");
            return failures;
        }
        static void BuildPage(ScreenId id)
        {
            GameObject root=new GameObject("Wave03_"+id,typeof(RectTransform)); root.layer=5; RectTransform rt=root.GetComponent<RectTransform>(); Stretch(rt); var marker=root.AddComponent<ProductionCoreLoopPageMarker>(); marker.Configure(id);
            switch(id){case ScreenId.Events: BuildEvents(rt); break; case ScreenId.EventDetail: BuildEventDetail(rt); break; case ScreenId.IndividualRanking: BuildIndividualRanking(rt); break; case ScreenId.TeamHub: BuildTeamHub(rt); break; case ScreenId.TeamRanking: BuildTeamRanking(rt); break; case ScreenId.TeamChat: BuildTeamChat(rt); break; case ScreenId.FriendsList: BuildFriends(rt); break; case ScreenId.Mail: BuildMail(rt); break;}
            PrefabUtility.SaveAsPrefabAsset(root,CandidateRoot+"/"+id+".prefab"); UnityEngine.Object.DestroyImmediate(root);
        }
        static void Base(RectTransform r,string folder,string title)
        {
            string refinedBg=RefinedRoot+"/"+folder+"/background_refined.png";
            if(AssetDatabase.LoadAssetAtPath<Sprite>(refinedBg)!=null) Refined(r,folder,"background_refined.png",0,0,941,1672); else Page(r,folder,"background_plate.png",0,0,941,1672);
            Shared(r,"Panels/top_bar.png",0,0,941,150); Shared(r,"Icons/icon_back.png",24,45,55,55); Shared(r,"Icons/icon_coin.png",385,45,50,50); Shared(r,"Icons/icon_gem.png",610,45,50,50); Shared(r,"Icons/icon_energy.png",800,45,50,50);
            HudT(r,"12 540",430,45,120,50,18); HudT(r,"350",660,45,90,50,18); HudT(r,"25/30",850,45,80,50,16);
            string refinedHeader=RefinedRoot+"/"+folder+"/header_refined.png";
            if(AssetDatabase.LoadAssetAtPath<Sprite>(refinedHeader)!=null) Refined(r,folder,"header_refined.png",90,155,760,150); else Shared(r,"Panels/header_frame.png",115,155,711,155);
            DisplayT(r,title,170,185,601,90,32);
        }
        static void BuildEvents(RectTransform r)
        {
            Refined(r,"EventsHub","top_hud_shell.png",0,0,941,132);
            Refined(r,"EventsHub","header_ornament_shell.png",180,120,580,182);
            Refined(r,"EventsHub","tabs_shell.png",28,300,885,87);
            Refined(r,"EventsHub","event_card_1_shell.png",32,390,878,328);
            Refined(r,"EventsHub","event_card_2_shell.png",32,720,878,312);
            Refined(r,"EventsHub","event_card_3_shell.png",32,1038,878,282);
            Refined(r,"EventsHub","event_card_4_shell.png",32,1323,878,269);
            Refined(r,"EventsHub","footer_shell.png",115,1595,711,67);
            HudT(r,"Иван",175,24,150,34,18); HudT(r,"Уровень 28",175,58,160,30,14); HudT(r,"12 540",390,35,110,50,18); HudT(r,"350",590,35,95,50,18); HudT(r,"25/30",755,35,120,50,16);
            DisplayT(r,"СОБЫТИЯ",250,175,441,78,34); TabText(r,"АКТИВНЫЕ",68,320,225,52); TabText(r,"СКОРО",360,320,205,52); TabText(r,"ЗАВЕРШЕННЫЕ",650,320,240,52);
            string[] names={"Фестиваль Жар-Птицы","Испытания Бабы Яги","Сокровища Купцов","Ледяной Турнир"}; string[] times={"2 д. 14 ч.","1 д. 21 ч.","5 д. 14 ч.","3 д. 9 ч."}; string[] descriptions={"Собирайте перья Жар-Птицы\nв матчах и получайте ценные награды!","Проходите испытания каждый день\nи докажите свою силу духа!","Открывайте сундуки и собирайте\nкупеческие печати!","Сражайтесь с другими игроками\nи поднимайтесь в рейтинге!"}; string[] ctas={"УЧАСТВОВАТЬ","НАЧАТЬ","ОТКРЫТЬ","В БОЙ"}; float[] ys={390f,720f,1038f,1323f};
            for(int i=0;i<4;i++){float y=ys[i]; HeadingT(r,names[i],430,y+22,430,64,21); HudT(r,times[i],445,y+87,220,42,15); BodyLightT(r,descriptions[i],430,y+128,410,82,14); HeadingT(r,"Награды:",430,y+205,150,35,14); DisplayT(r,ctas[i],700,y+236,175,44,14);}
        }
        static void TabText(RectTransform r,string label,float x,float y,float w,float h){HeadingT(r,label,x,y,w,h,15);}
        static void BuildEventDetail(RectTransform r)
        {
            Base(r,"EventDetail","СОБЫТИЕ"); Page(r,"EventDetail","event_hero_firebird.png",90,325,761,440); Page(r,"EventDetail","event_title_frame.png",90,785,761,105); DisplayT(r,"ФЕСТИВАЛЬ ЖАР-ПТИЦЫ",140,805,661,62,25);
            Page(r,"EventDetail","objective_row.png",90,910,761,120); HeadingT(r,"Соберите 50 перьев Жар-Птицы",130,925,681,48,18); Page(r,"EventDetail","progress_bar_frame.png",145,985,651,38); Page(r,"EventDetail","milestone_rewards.png",110,1050,721,160); Page(r,"EventDetail","event_pass_banner.png",90,1235,761,170); HeadingT(r,"НАГРАДЫ СОБЫТИЯ",150,1260,641,44,19); Page(r,"EventDetail","claim_button.png",300,1430,341,82); DisplayT(r,"ЗАБРАТЬ",320,1443,301,52,22);
        }
        static void BuildIndividualRanking(RectTransform r)
        {
            Base(r,"IndividualRanking","РЕЙТИНГ"); Page(r,"IndividualRanking","season_header.png",70,325,801,150); DisplayT(r,"СЕЗОН БОГАТЫРЕЙ",150,350,641,55,24); Page(r,"IndividualRanking","rank_tab.png",115,490,231,64); HeadingT(r,"МИР",125,500,211,44,16); Page(r,"IndividualRanking","rank_tab.png",355,490,231,64); HeadingT(r,"ДРУЗЬЯ",365,500,211,44,16);
            for(int i=0;i<7;i++){float y=580+i*125; Page(r,"IndividualRanking","ranking_row.png",70,y,801,112); Page(r,"IndividualRanking","avatar_frame.png",105,y+8,90,90); HeadingT(r,(i+1)+"",75,y+28,40,42,18); BodyLightT(r,"Игрок "+(i+1),215,y+20,260,42,17); HudT(r,(1500-i*97)+"",665,y+25,130,40,17);}
            Page(r,"IndividualRanking","player_sticky_row.png",70,1470,801,125); HeadingT(r,"ВАШЕ МЕСТО: 28",120,1490,350,50,18); HudT(r,"1 248",660,1490,130,50,18);
        }
        static void BuildTeamHub(RectTransform r)
        {
            Base(r,"TeamHub","КОМАНДА"); Page(r,"TeamHub","team_banner.png",70,330,801,220); Page(r,"TeamHub","team_emblem.png",110,360,140,140); DisplayT(r,"БОГАТЫРИ РУСИ",285,365,500,58,26); BodyLightT(r,"24 / 30 участников",285,430,350,42,16);
            Page(r,"TeamHub","member_summary.png",70,575,801,190); HeadingT(r,"УЧАСТНИКИ",110,600,250,45,18); BodyLightT(r,"Онлайн: 8   Недельный вклад: 12 480",110,655,650,48,16);
            Page(r,"TeamHub","team_chest_progress.png",70,790,801,190); HeadingT(r,"КОМАНДНЫЙ СУНДУК",110,815,400,45,18); Shared(r,"Ornaments/progress_track.png",130,900,600,25); Shared(r,"Ornaments/progress_fill.png",130,900,390,25);
            string[] labs={"РЕЙТИНГ","ЧАТ","ДРУЗЬЯ"}; string[] icons={"Icons/icon_tasks.png","Icons/icon_guild.png","Icons/icon_character.png"}; for(int i=0;i<3;i++){float x=70+i*270; Page(r,"TeamHub","social_nav_tile.png",x,1020,240,170); Shared(r,icons[i],x+80,1040,80,80); HeadingT(r,labs[i],x+20,1120,200,42,15);}
            HeadingT(r,"УЧАСТНИКИ",100,1240,300,45,18); for(int i=0;i<5;i++) Page(r,"TeamHub","member_avatar_frame.png",105+i*145,1300,92,92);
        }
        static void BuildTeamRanking(RectTransform r)
        {
            Base(r,"TeamRanking","РЕЙТИНГ КОМАНД"); Page(r,"TeamRanking","team_ranking_header.png",70,330,801,145); DisplayT(r,"ЛИГА БОГАТЫРЕЙ",150,355,641,55,23);
            for(int i=0;i<7;i++){float y=510+i*128; Page(r,"TeamRanking","team_row.png",70,y,801,115); Page(r,"TeamRanking","team_emblem.png",115,y+8,92,92); HeadingT(r,(i+1)+"",80,y+30,45,42,18); BodyLightT(r,"Команда "+(i+1),230,y+18,300,42,17); HudT(r,(32000-i*1770)+"",650,y+25,150,42,17);}
            Page(r,"TeamRanking","player_team_highlight.png",70,1440,801,130); HeadingT(r,"БОГАТЫРИ РУСИ — 14 МЕСТО",115,1465,590,48,17); HudT(r,"18 760",700,1465,110,48,16);
        }
        static void BuildTeamChat(RectTransform r)
        {
            Base(r,"TeamChat","КОМАНДНЫЙ ЧАТ");
            for(int i=0;i<5;i++){bool mine=i%2==1; float y=350+i*205; float x=mine?270:105; Page(r,"TeamChat",mine?"message_bubble_out.png":"message_bubble_in.png",x,y,590,130); Page(r,"TeamChat","avatar_frame.png",mine?785:35,y+15,88,88); BodyLightT(r,mine?"Готов к походу!":"Кто идёт на турнир?",x+25,y+22,540,58,15); HudT(r,"18:"+(10+i*7),x+420,y+86,125,28,11);}
            Page(r,"TeamChat","composer_field.png",70,1440,691,82); BodyLightT(r,"Сообщение...",100,1457,500,45,15); Page(r,"TeamChat","send_icon.png",785,1442,76,76);
        }
        static void BuildFriends(RectTransform r)
        {
            Base(r,"FriendsList","ДРУЗЬЯ"); Page(r,"FriendsList","search_field.png",100,330,651,70); BodyLightT(r,"Поиск игрока...",125,342,500,44,15); Page(r,"FriendsList","invite_add_button.png",770,330,120,70); DisplayT(r,"+",790,338,80,48,24);
            for(int i=0;i<8;i++){float y=430+i*132; Page(r,"FriendsList","friend_row.png",70,y,801,120); Page(r,"FriendsList","avatar_frame.png",100,y+10,92,92); if(i<5) Page(r,"FriendsList","online_status.png",170,y+70,35,35); BodyLightT(r,"Друг "+(i+1),220,y+18,280,40,17); HudT(r,i<5?"В сети":"Был недавно",220,y+61,250,34,13); Page(r,"FriendsList","request_button.png",620,y+27,210,64); HeadingT(r,"ЭНЕРГИЯ",635,y+37,180,42,14);}
        }
        static void BuildMail(RectTransform r)
        {
            Refined(r,"Mail","top_hud_shell.png",0,0,941,132);
            Refined(r,"Mail","header_ornament_shell.png",190,120,562,158);
            Refined(r,"Mail","tabs_shell.png",62,275,822,75);
            float[] ys={352f,483f,615f,748f,880f}; float[] hs={126f,127f,129f,128f,130f};
            for(int i=0;i<5;i++) Refined(r,"Mail","message_row_"+(i+1)+"_shell.png",48,ys[i],839,hs[i]);
            Refined(r,"Mail","detail_panel_shell.png",48,1020,841,437); Refined(r,"Mail","bottom_nav_shell.png",0,1458,941,214);
            HudT(r,"Иван",175,24,150,34,18); HudT(r,"Уровень 28",175,58,160,30,14); HudT(r,"12 540",390,35,110,50,18); HudT(r,"350",590,35,95,50,18); HudT(r,"25/30",755,35,120,50,16);
            DisplayT(r,"ПОЧТА",290,165,360,72,34); TabText(r,"ВХОДЯЩИЕ",90,290,235,50); TabText(r,"ОТПРАВЛЕННЫЕ",345,290,255,50); TabText(r,"СИСТЕМА",655,290,200,50);
            string[] senders={"Щедрый дар от Добрыни","Благословение от Василисы","Гильдия богатырей","Системное сообщение","Тайный совет"}; string[] subs={"Держи, Иван! Это тебе пригодится","Пусть эта сила оберегает тебя","Еженедельные награды гильдии","Обновление 1.4.0 уже доступно","Тени шепчут... Проверь, что скрыто"}; string[] times={"2 мин. назад","15 мин. назад","1 ч. назад","3 ч. назад","12 ч. назад"};
            for(int i=0;i<5;i++){float y=ys[i]; HeadingT(r,senders[i],220,y+10,430,38,17); BodyLightT(r,subs[i],220,y+47,430,55,14); HudT(r,times[i],690,y+5,155,32,12);}
            BodyDarkT(r,"Щедрый дар от Добрыни",290,1045,480,52,23); BodyDarkT(r,"Приветствую, Иван!\nЗа твою доблесть и верность Руси принимай этот скромный дар.\nПусть он поможет тебе в грядущих испытаниях.",290,1100,480,145,16); BodyDarkT(r,"НАГРАДЫ",360,1290,220,40,16); DisplayT(r,"ЗАБРАТЬ",610,1360,230,54,19); DisplayT(r,"ЗАБРАТЬ ВСЁ",315,1492,320,62,21); HudT(r,"Писем: 5/100",370,1622,200,32,14);
        }
        static void Tab(RectTransform r,string label,float x,float y,bool selected){Shared(r,selected?"Controls/tab_selected.png":"Controls/tab_idle.png",x,y,220,70); HeadingT(r,label,x+8,y+10,204,50,14);}
        static void Refined(RectTransform r,string folder,string file,float x,float y,float w,float h){Img(r,RefinedRoot+"/"+folder+"/"+file,x,y,w,h);}
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
