using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using LegendsOfRus.SaveCore;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
namespace LegendsOfRus.Tests.CampaignPlayMode
{
    public sealed class CampaignUIIntegrationTests
    {
        static T View<T>(CampaignDevelopmentUI ui)where T:Component=>ui.GetComponentInChildren<T>();
        static void Click(Button b){Assert.That(b,Is.Not.Null);Assert.That(b.IsInteractable(),Is.True,b.name);b.onClick.Invoke();}
        [UnityTearDown] public IEnumerator Cleanup()
        {
            foreach(var ui in UnityEngine.Object.FindObjectsByType<CampaignDevelopmentUI>(FindObjectsInactive.Include,FindObjectsSortMode.None))
                if(ui.transform.root.name=="CampaignUIVerification")UnityEngine.Object.Destroy(ui.transform.root.gameObject);
            yield return null;
        }
        static void DrainStory(CampaignDevelopmentUI ui)
        { int steps=0;while(ui.VisiblePage=="Dialogue"){Assert.That(++steps,Is.LessThan(40));Click(View<ProductionDialogueView>(ui).Continue);} }
        static GameObject Root()=>new GameObject("CampaignUIVerification",typeof(RectTransform),typeof(Canvas));
        [UnityTest,Timeout(300000)] public IEnumerator ApprovedUIPlaysWinsAndContinuesWithCampaignValues()
        {
            var root=Root();var session=CampaignBoardIntegrationTests.Session();string language=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("en");
            var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>session,()=>{});
            try{
                yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Capture(root,"Intro_EN",576,1280);DrainStory(ui);Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));Assert.That(View<ProductionPreLevelView>(ui).HasRequiredBindings,Is.True);Capture(root,"PreLevel",941,1672);
                Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Gameplay"));Assert.That(ui.Board,Is.Not.Null);
                var game=View<ProductionGameplayView>(ui);session.TryLoad(out var before,out _);int moves=ui.Board.Moves;
                Click(game.BoosterButtons[3]);yield return new WaitForSecondsRealtime(.6f);
                session.TryLoad(out var after,out _);Assert.That(after.player.boosterSeaPearl,Is.EqualTo(before.player.boosterSeaPearl-1));Assert.That(after.booster.state,Is.EqualTo(CampaignBoosterState.Applied));Assert.That(ui.Board.Moves,Is.EqualTo(moves+5));
                Assert.That(game.BoosterCounts[3].text,Is.EqualTo(after.player.boosterSeaPearl.ToString()));Capture(root,"Gameplay",576,1280);
                Click(game.PauseButton);yield return null;Assert.That(ui.VisibleModal,Is.EqualTo("Pause"));Click(View<ProductionPauseView>(ui).Actions[0]);yield return null;Assert.That(ui.VisibleModal,Is.Null);
                int commands=0;
                while(ui.Coordinator.State==CampaignGameplayState.Playing){var board=ui.Board;Assert.That(++commands,Is.LessThan(34));var plan=CampaignBoardIntegrationTests.Choose(board,true);Assert.That(plan,Is.Not.Null);
                    Assert.That(board.SwipeCell(plan.PrimaryPosition.X,plan.PrimaryPosition.Y,new Vector2Int(plan.SecondaryPosition.X-plan.PrimaryPosition.X,plan.SecondaryPosition.Y-plan.PrimaryPosition.Y)),Is.True);
                    float deadline=Time.realtimeSinceStartup+15;while(ui.Coordinator.State==CampaignGameplayState.Playing&&board&&(board.HasActiveCommand||!board.CanAcceptPointerInput)&&Time.realtimeSinceStartup<deadline)yield return null;
                    Assert.That(Time.realtimeSinceStartup,Is.LessThan(deadline));yield return null;
                }
                Assert.That(ui.VisiblePage,Is.EqualTo("Victory"),ui.Coordinator.Diagnostic);var victory=View<ProductionVictoryView>(ui);session.TryLoad(out var won,out _);
                Assert.That(victory.Score.text.Replace(" ",""),Is.EqualTo(won.run.score.ToString()));Assert.That(victory.DoubleButton.interactable,Is.False);Capture(root,"Victory",941,1672);
                Click(victory.ContinueButton);yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));Capture(root,"PostVictory_EN",576,1280);DrainStory(ui);Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));session.TryLoad(out var next,out _);Assert.That(next.player.selectedLevel,Is.EqualTo(2));
                Assert.That(View<ProductionPreLevelView>(ui).SelectedLevel.text,Does.Contain("2"));
            }finally{UnityEngine.Object.Destroy(root);DemoFeatureState.SetLanguage(language);}yield return null;
        }
        [UnityTest] public IEnumerator InterruptedRoundUsesApprovedRecoveryPanelAndFailPage()
        {
            var root=Root();var session=CampaignBoardIntegrationTests.Session();session.StartRound(0,DateTime.UtcNow.Ticks);session.ReserveBooster(1,0,BoosterType.Firebird);
            string language=DemoFeatureState.LanguageCode;DemoFeatureState.SetLanguage("ru");var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>session,()=>{});
            try{
                yield return null;Assert.That(ui.VisibleModal,Is.EqualTo("Recovery"));session.TryLoad(out var save,out _);Assert.That(save.booster.state,Is.EqualTo(CampaignBoosterState.Refunded));Assert.That(View<ProductionPreLevelView>(ui).Header.Balances[2].text,Is.EqualTo("4/5"));Capture(root,"Recovery_RU",576,1280);
                Click(View<ProductionErrorDialogView>(ui).Close);yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Fail"));Assert.That(View<ProductionFailView>(ui).Retry.IsInteractable(),Is.True);Capture(root,"Fail_RU",941,1672);
                Click(View<ProductionFailView>(ui).Retry);yield return null;Assert.That(ui.Board,Is.Not.Null);session.TryLoad(out save,out _);Assert.That(save.player.energy,Is.EqualTo(3));
            }finally{UnityEngine.Object.Destroy(root);DemoFeatureState.SetLanguage(language);}yield return null;
        }
        [UnityTest] public IEnumerator AllTargetedBoostersUseCampaignInventory()
        {
            foreach(var type in new[]{BoosterType.BogatyrMace,BoosterType.Firebird,BoosterType.LivingLight}){
                var root=Root();var session=CampaignBoardIntegrationTests.Session();var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>session,()=>{});
                try{
                    yield return null;DrainStory(ui);Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;var board=ui.Board;session.TryLoad(out var before,out _);int count=CampaignBoosterTransactions.Count(before.player,type);
                    board.UseBooster(type);Assert.That(board.IsBoosterArmed,Is.True);Assert.That(board.TapCell(3,3),Is.True);
                    float end=Time.realtimeSinceStartup+15;while(board&&board.HasActiveCommand&&Time.realtimeSinceStartup<end)yield return null;
                    Assert.That(Time.realtimeSinceStartup,Is.LessThan(end));session.TryLoad(out var after,out _);Assert.That(after.booster.state,Is.EqualTo(CampaignBoosterState.Applied));Assert.That(CampaignBoosterTransactions.Count(after.player,type),Is.EqualTo(count-1));
                }finally{UnityEngine.Object.Destroy(root);}yield return null;
            }
        }
        sealed class FaultStore:ISaveJournalStore
        {
            Dictionary<string,string> values=new Dictionary<string,string>(),durable=new Dictionary<string,string>();
            public int Writes,FailAt;
            void Mutation(){if(++Writes==FailAt)throw new IOException("Injected UI save fault");}
            public bool TryRead(string key,out string value)=>values.TryGetValue(key,out value);
            public void Write(string key,string value){Mutation();values[key]=value;}
            public void Delete(string key){Mutation();values.Remove(key);}
            public void Flush(){Mutation();durable=new Dictionary<string,string>(values);}
            public void Restart(){values=new Dictionary<string,string>(durable);FailAt=Writes=0;}
        }
        [UnityTest] public IEnumerator StorageFailureOffersReopenWithoutStartingBoard()
        {
            var root=Root();var store=new FaultStore();var session=CampaignBoardIntegrationTests.Session(store);
            int reopens=0;var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>{if(++reopens==1)throw new IOException("Injected reopen fault");store.Restart();return CampaignBoardIntegrationTests.Session(store,false);},()=>{});
            try{
                yield return null;DrainStory(ui);store.Writes=0;store.FailAt=6;Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;
                Assert.That(ui.VisibleModal,Is.EqualTo("Error"));Assert.That(ui.Board,Is.Null);Capture(root,"SaveError",576,1280);
                Click(View<ProductionErrorDialogView>(ui).Retry);yield return null;Assert.That(ui.VisibleModal,Is.EqualTo("Error"));
                Click(View<ProductionErrorDialogView>(ui).Retry);yield return null;Assert.That(reopens,Is.EqualTo(2));Assert.That(ui.VisibleModal,Is.Null);DrainStory(ui);Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));
                var reopened=CampaignBoardIntegrationTests.Session(store,false);reopened.TryLoad(out var save,out _);Assert.That(save.player.energy,Is.EqualTo(5));
                Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;Assert.That(ui.Board,Is.Not.Null);
            }finally{UnityEngine.Object.Destroy(root);}yield return null;
        }
        [UnityTest] public IEnumerator BoosterTimeoutRestoresBoardAndRefundsOnce()
        {
            var root=Root();var session=CampaignBoardIntegrationTests.Session();var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>session,()=>{});
            try{
                yield return null;DrainStory(ui);Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;var board=ui.Board;int moves=board.Moves;string hash=board.BoardStateHash;
                session.TryLoad(out var before,out _);board.UseBooster(BoosterType.SeaPearl);
                LogAssert.Expect(LogType.Error,new Regex("Gameplay command recovered: booster:SeaPearl"));board.ForceCommandTimeoutForTesting();yield return null;yield return null;
                session.TryLoad(out var after,out _);Assert.That(after.booster.state,Is.EqualTo(CampaignBoosterState.Refunded));Assert.That(after.player.boosterSeaPearl,Is.EqualTo(before.player.boosterSeaPearl));Assert.That(board.Moves,Is.EqualTo(moves));Assert.That(board.BoardStateHash,Is.EqualTo(hash));
                board.UseBooster(BoosterType.SeaPearl);yield return new WaitForSecondsRealtime(.6f);session.TryLoad(out after,out _);Assert.That(after.player.boosterSeaPearl,Is.EqualTo(before.player.boosterSeaPearl-1));Assert.That(after.booster.sequence,Is.EqualTo(2));
            }finally{UnityEngine.Object.Destroy(root);}yield return null;
        }
        [UnityTest] public IEnumerator CurrentArtworkAndRulesAreIntegratedWithExistingPresentation()
        {
            var root=Root();var session=CampaignBoardIntegrationTests.Session();
            var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>session,()=>{});
            try
            {
                yield return null;DrainStory(ui);Click(View<ProductionPreLevelView>(ui).PlayButton);yield return null;
                var board=ui.Board;Assert.That(board.UsesCurrentBoardArt,Is.True);
                Assert.That(board.RuleVersion,Is.EqualTo(LegendsOfRus.GameplayCore.GameplayRuleVersion.PrototypeR4));
                Assert.That(board.GetComponentsInChildren<PrototypeImpactView>().Length,Is.EqualTo(1),"reuse existing effects once; new polish remains deferred");
                Assert.That(board.GetComponentsInChildren<SpecialSequenceView>().Length,Is.EqualTo(2));
                var model=new LegendsOfRus.GameplayCore.BoardModel(8,7,6,new LegendsOfRus.GameplayCore.SeededBoardRandom(3));
                for(int y=0;y<7;y++)for(int x=0;x<8;x++)model.SetCell(new LegendsOfRus.GameplayCore.BoardPosition(x,y),new LegendsOfRus.GameplayCore.BoardCellData((x+2*y)%6));
                var kinds=new[]{LegendsOfRus.GameplayCore.SpecialGemKind.Row,LegendsOfRus.GameplayCore.SpecialGemKind.Column,LegendsOfRus.GameplayCore.SpecialGemKind.Burst,LegendsOfRus.GameplayCore.SpecialGemKind.Color};
                for(int i=0;i<4;i++)model.SetCell(new LegendsOfRus.GameplayCore.BoardPosition(i+2,2),new LegendsOfRus.GameplayCore.BoardCellData(i,kinds[i]));
                var obstacles=new[]{LegendsOfRus.GameplayCore.BoardObstacleKind.Crate,LegendsOfRus.GameplayCore.BoardObstacleKind.Ice,LegendsOfRus.GameplayCore.BoardObstacleKind.Chain};
                foreach(int layers in new[]{2,1})
                {
                    for(int i=0;i<3;i++)model.SetObstacle(new LegendsOfRus.GameplayCore.BoardPosition(i+2,4),new LegendsOfRus.GameplayCore.BoardObstacleData(obstacles[i],layers));
                    Assert.That(board.LoadSnapshotForTesting(model.CreateSnapshot()),Is.True);yield return null;
                    var images=board.GetComponentsInChildren<Image>();
                    foreach(string name in new[]{"line","bomb","color"})Assert.That(System.Array.Exists(images,i=>i.enabled&&i.sprite==PrototypeBoardArt.Load(name)),Is.True,name);
                    foreach(var kind in obstacles)Assert.That(System.Array.Exists(images,i=>i.enabled&&i.sprite==PrototypeObstacleArt.Load(kind,layers)),Is.True,kind.ToString());
                    Assert.That(System.Array.Exists(board.GetComponentsInChildren<Text>(),t=>t.name=="ObstacleLayers"),Is.False,"damage uses artwork, not number badges");
                    Capture(root,"CurrentArt_"+(layers==2?"Intact":"Damaged"),576,1280);
                }
            }
            finally{UnityEngine.Object.Destroy(root);}yield return null;
        }
        [UnityTest] public IEnumerator StoryReopensAndLocaleSwitchKeepsLineWithoutTouchingLiveStory()
        {
            var root=Root();var store=new FaultStore();var session=CampaignBoardIntegrationTests.Session(store);
            string language=DemoFeatureState.LanguageCode,liveScene=StoryService.ActiveSceneId;
            var ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>CampaignBoardIntegrationTests.Session(store,false),()=>{});
            try
            {
                yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));session.TryLoad(out var before,out _);
                var v=View<ProductionDialogueView>(ui);DemoFeatureState.SetLanguage("ru");v.Refresh();Capture(root,"Intro_RU",576,1280);
                Click(v.Continue);string russian=v.Description.text;DemoFeatureState.SetLanguage("en");v.Refresh();Assert.That(v.Description.text,Is.Not.EqualTo(russian));
                Assert.That(StoryService.ActiveSceneId,Is.EqualTo(liveScene));session.TryLoad(out var partial,out _);Assert.That(JsonUtility.ToJson(partial),Is.EqualTo(JsonUtility.ToJson(before)));
                UnityEngine.Object.Destroy(ui.gameObject);yield return null;store.Restart();session=CampaignBoardIntegrationTests.Session(store,false);
                ui=CampaignDevelopmentUI.Open((RectTransform)root.transform,session,()=>CampaignBoardIntegrationTests.Session(store,false),()=>{});yield return null;
                Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));v=View<ProductionDialogueView>(ui);Assert.That(v.SpeakerKeyProvider(),Is.EqualTo("speaker.dobrynya"));
                // Fail acknowledgement at the first journal mutation; reopening must preserve eligibility.
                Click(v.Continue);Click(v.Continue);store.Writes=0;store.FailAt=1;Click(v.Continue);yield return null;Assert.That(ui.VisibleModal,Is.EqualTo("Error"));Capture(root,"StorySaveError",576,1280);
                store.Restart();Click(View<ProductionErrorDialogView>(ui).Retry);yield return null;Assert.That(ui.VisiblePage,Is.EqualTo("Dialogue"));DrainStory(ui);
                Assert.That(ui.VisiblePage,Is.EqualTo("PreLevel"));var fresh=CampaignBoardIntegrationTests.Session(store,false);fresh.TryLoad(out var after,out _);
                Assert.That(after.player.energy,Is.EqualTo(before.player.energy));Assert.That(after.player.gold,Is.EqualTo(before.player.gold));Assert.That(after.runSequence,Is.Zero);
                Assert.That(after.player.completedStoryScenes.Count,Is.EqualTo(1));Assert.That(StoryService.ActiveSceneId,Is.EqualTo(liveScene));
            }
            finally{UnityEngine.Object.Destroy(root);DemoFeatureState.SetLanguage(language);}yield return null;
        }
        static void Capture(GameObject root,string name,int width,int height)
        {
            string dir=Environment.GetEnvironmentVariable("LOR_JOURNEY_CAPTURE_DIR");if(string.IsNullOrEmpty(dir))return;Directory.CreateDirectory(dir);
            var canvas=root.GetComponent<Canvas>();var oldMode=canvas.renderMode;var oldCamera=canvas.worldCamera;float oldScale=canvas.scaleFactor;
            var camGo=new GameObject("CampaignCaptureCamera",typeof(Camera));var cam=camGo.GetComponent<Camera>();cam.enabled=false;cam.orthographic=true;cam.transform.position=new Vector3(0,0,-100);cam.nearClipPlane=.1f;cam.farClipPlane=1000;cam.clearFlags=CameraClearFlags.SolidColor;cam.backgroundColor=Color.black;
            var target=new RenderTexture(width,height,24);var texture=new Texture2D(width,height,TextureFormat.RGBA32,false);var active=RenderTexture.active;
            try{target.Create();cam.targetTexture=target;canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=cam;canvas.planeDistance=100;canvas.scaleFactor=width/941f;
                foreach(var t in root.GetComponentsInChildren<Text>(true))t.SetVerticesDirty();Canvas.ForceUpdateCanvases();foreach(var t in root.GetComponentsInChildren<Text>(true))t.SetVerticesDirty();Canvas.ForceUpdateCanvases();
                cam.Render();RenderTexture.active=target;texture.ReadPixels(new Rect(0,0,width,height),0,0);texture.Apply();File.WriteAllBytes(Path.Combine(dir,"CampaignUI_"+name+".png"),texture.EncodeToPNG());
            }finally{RenderTexture.active=active;canvas.renderMode=oldMode;canvas.worldCamera=oldCamera;canvas.scaleFactor=oldScale;UnityEngine.Object.Destroy(camGo);UnityEngine.Object.Destroy(texture);target.Release();UnityEngine.Object.Destroy(target);}
        }
    }
}
