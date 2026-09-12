#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep11SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 11 Release Candidate Stabilization Self-Test")]
        public static void Run()
        {
            var failures=new List<string>();
            Step20ProductionAssetStep11Evidence.RcStatus status=Step20ProductionAssetStep11Evidence.EvaluateAndWrite();
            if(status.source!=36)failures.Add("Step 11 requires 36/36 source candidates; found "+status.source+".");
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))failures.Add("Accepted core production registry must remain exactly ten routes.");
            int supported=0;for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)if(ProductionMetaPageCutoverRegistry.IsSupportedMetaPage(Step20ProductionAssetStep103Evidence.Pages[i].id))supported++;
            if(supported!=26)failures.Add("Generalized meta cutover registry must support 26 non-core routes; found "+supported+".");
            if(status.globalFailures!=0)failures.Add("Global authority/integration failures are present. Review Step 11 status output.");
            if(File.Exists(Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep11Evidence.DevelopmentRcLock)))
            {
                List<string> freezeFailures=Step20ProductionAssetStep11Evidence.ValidateDevelopmentRcFreeze();
                for(int i=0;i<freezeFailures.Count;i++)failures.Add("RC-FREEZE: "+freezeFailures[i]);
            }
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 11 source stabilization PASS: 36/36 candidates, core registry frozen at 10, generalized non-core lane=26. Development RC decision="+status.developmentDecision+"; store-release decision="+status.storeDecision+".");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 11 source stabilization PASS.\n\nDevelopment RC: "+status.developmentDecision+"\nStore-release RC: "+status.storeDecision+"\n\nNo cutover or RC freeze is performed by this self-test.","OK");
        }
    }
}
#endif
