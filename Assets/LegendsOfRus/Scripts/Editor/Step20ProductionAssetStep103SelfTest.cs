#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep103SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 10.3 Final Runtime Integration Readiness Self-Test")]
        public static void Run()
        {
            var failures=new List<string>();
            Step20ProductionAssetStep103Evidence.BuildAllCandidates();
            if(Step20ProductionAssetStep103Evidence.Pages.Length!=36)failures.Add("Step 10.3 evidence model must contain exactly 36 functional pages.");
            int candidates=0,metaSupported=0,approved=0;
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                var p=Step20ProductionAssetStep103Evidence.Pages[i];
                if(AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetStep103Evidence.CandidatePath(p))!=null)candidates++;else failures.Add("Missing candidate prefab: "+p.id);
                if(ProductionMetaPageCutoverRegistry.IsSupportedMetaPage(p.id))metaSupported++;
                if(File.Exists(Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(p.id))))approved++;
            }
            if(candidates!=36)failures.Add("Expected 36/36 candidate prefabs, found "+candidates+".");
            if(metaSupported!=26)failures.Add("Generalized non-core meta cutover registry must support exactly 26 non-core functional routes, found "+metaSupported+".");
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))failures.Add("Accepted core registry must remain exactly ten routes.");
            if(approved<10)failures.Add("Baseline direct approved reference coverage unexpectedly dropped below 10 pages; found "+approved+".");
            failures.AddRange(Step20ProductionAssetStep103Evidence.ValidateGlobalAuthorityBoundaries());

            Step20ProductionAssetStep103CutoverReadiness.ReadinessResult readiness=Step20ProductionAssetStep103CutoverReadiness.EvaluateAndWrite();
            if(readiness.sourceReady!=36)failures.Add("Readiness evaluator did not report 36/36 source candidates.");
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            Debug.Log("v1.20 Step 10.3 source readiness PASS: 36/36 candidates, 26 generalized non-core cutover routes, core registry frozen at 10, approved refs="+approved+"/36. Runtime/reference/Android evidence remains separately gated.");
            if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus","Step 10.3 source readiness PASS. 36/36 candidates are available and the rollback-controlled integration lane is armed but inactive. Review the generated cutover-readiness matrix before any development cutover.","OK");
        }
    }
}
#endif
