#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace LegendsOfRus.Editor
{
    // Editor-only feedback; leaves the approved game pages and touch areas intact.
    public sealed class CampaignPlaytestWindow : EditorWindow
    {
        [MenuItem("Legends of Rus/Development/Campaign Playtest Monitor")]
        public static void ShowMonitor()=>GetWindow<CampaignPlaytestWindow>("Campaign playtest");
        int firstRound = 31;
        string launchError;
        [MenuItem("Legends of Rus/Development/Campaign Active Playtest (Recorded)")]
        public static void ShowActivePlaytest() => ShowMonitor();
        void OnInspectorUpdate()=>Repaint();
        void OnGUI()
        {
            EditorGUILayout.LabelField("Campaign · local playtest",EditorStyles.boldLabel);
            var recorder=Object.FindFirstObjectByType<CampaignPlaytestRecorder>();
            if (!recorder)
            {
                EditorGUILayout.HelpBox("Installed campaign · disposable progress, 30 energy, zero starting boosters. From Home in Play Mode, select a block and start. Recordings survive leaving Play Mode. Earlier candidate menus remain available.", MessageType.Info);
                try
                {
                    int[] blocks = CampaignActivePlaytestSetup.AvailableBlocks();
                    int selected = System.Array.IndexOf(blocks, firstRound);
                    if (selected < 0) selected = 0;
                    string[] labels = System.Array.ConvertAll(blocks, n => $"Rounds {n}–{System.Math.Min(n+9, ActiveCampaignContent.Load().LoadChapter(1).levels.Length)}");
                    firstRound = blocks[EditorGUILayout.Popup("Record block", selected, labels)];
                    using (new EditorGUI.DisabledScope(!CampaignDevelopmentPreviewMenu.CanOpenActivePlaytest))
                        if (GUILayout.Button("Start installed-campaign recording"))
                        { CampaignDevelopmentPreviewMenu.OpenActivePlaytest(firstRound); launchError = null; }
                }
                catch (System.Exception e) { launchError = e.Message; }
                if (!string.IsNullOrEmpty(launchError)) EditorGUILayout.HelpBox(launchError, MessageType.Error);
                if(GUILayout.Button("Open recordings folder"))EditorUtility.RevealInFinder(CampaignPlaytestRecorder.DefaultDirectory);
                return;
            }
            EditorGUILayout.LabelField($"Recording rounds {recorder.StartRound}–{recorder.EndRound}");
            var a=recorder.Log.Current;
            EditorGUILayout.LabelField(recorder.ContentVersion,EditorStyles.wordWrappedLabel);
            var ui=recorder.GetComponent<CampaignDevelopmentUI>();
            if(ui && ui.PlaytestComplete)
            {
                EditorGUILayout.HelpBox($"PLAYTEST COMPLETE ({recorder.StartRound}–{recorder.EndRound}). Save or reveal the recording below. This recording stops at round {recorder.EndRound}. Replay is optional and will be recorded separately.",MessageType.Info);
                if(GUILayout.Button($"Replay round {recorder.EndRound} (optional)"))ui.ReplayCompletedPlaytest();
                if(GUILayout.Button("Exit playtest")){ui.ExitCompletedPlaytest();return;}
            }
            EditorGUILayout.LabelField(recorder.SaveStatus??"Waiting");
            EditorGUILayout.SelectableLabel(recorder.CheckpointPath??"",EditorStyles.wordWrappedLabel,GUILayout.Height(58));
            if(a!=null)
            {
                EditorGUILayout.LabelField($"Round {a.round} · attempt {a.committedAttempt} · {a.outcome}");
                EditorGUILayout.LabelField($"Moves: {a.movesRemaining}/{a.moveBudget} · boosters used: {a.inventoryBoostersUsed}");
                EditorGUILayout.HelpBox("Rate after the round and before Continue/Retry. For special opportunities, rate whether you understood how to create the line item, bomb, Color item or combination (especially rounds 12, 18, 24 and 37).",MessageType.None);
                Choice(recorder,"Difficulty","difficulty",a.difficulty,new[]{"unrated","easy","fair","hard"});
                Choice(recorder,"Goal clarity","goals",a.goalClarity,new[]{"unrated","clear","unclear"});
                Choice(recorder,"Special opportunity","specials",a.specialClarity,new[]{"unrated","clear","unclear"});
            }
            if(GUILayout.Button("Save now"))recorder.SaveNow();
            if(GUILayout.Button("Copy recording path"))GUIUtility.systemCopyBuffer=recorder.CheckpointPath;
            if(GUILayout.Button("Show recording"))EditorUtility.RevealInFinder(recorder.CheckpointPath);
            EditorGUILayout.LabelField("Stop Play Mode when finished. No upload is automatic.",EditorStyles.wordWrappedLabel);
        }
        static void Choice(CampaignPlaytestRecorder r,string title,string field,string current,string[] values)
        {
            int selected=System.Array.IndexOf(values,current),next=EditorGUILayout.Popup(title,Mathf.Max(0,selected),values);
            if(next!=selected)r.Rate(field,values[next]);
        }
    }
}
#endif
