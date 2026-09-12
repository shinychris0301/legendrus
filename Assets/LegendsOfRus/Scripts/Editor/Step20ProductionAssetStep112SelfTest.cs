#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep112SelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 11.2 Reference Closure + Store-RC Readiness Self-Test")]
        public static void Run()
        {
            var failures=new List<string>();
            if(Step20ProductionAssetStep103Evidence.Pages.Length!=36)failures.Add("Functional page contract count is not 36.");
            if(!ProductionCutoverFreezeGuard.MatchesExpectedCount(10))failures.Add("Accepted core production registry is not exactly ten routes.");
            int meta=0;for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)if(ProductionMetaPageCutoverRegistry.IsSupportedMetaPage(Step20ProductionAssetStep103Evidence.Pages[i].id))meta++;
            if(meta!=26)failures.Add("Generalized non-core meta cutover support is not exactly 26 pages (found "+meta+").");
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                ScreenId id=Step20ProductionAssetStep103Evidence.Pages[i].id;string abs=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id));
                if(File.Exists(abs)&&!Step20ProductionAssetStep112ReferenceClosure.HasExpectedDimensions(abs))failures.Add(id+": approved reference is not canonical 941x1672 PNG.");
            }
            string reviewRoot=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep112ReferenceClosure.Root);
            if(!File.Exists(Path.Combine(reviewRoot,"MISSING_REFERENCE_REVIEW_QUEUE_26P.jpg")))failures.Add("26-page missing-reference review queue is missing.");
            if(!File.Exists(Path.Combine(reviewRoot,"ACTIVE_REFERENCE_GAPS_Pause_ChapterComplete.jpg")))failures.Add("Pause/ChapterComplete active-gap review sheet is missing.");
            string example=Path.Combine(reviewRoot,"USER_APPROVED_REFERENCE_PAGES.example.txt");if(!File.Exists(example))failures.Add("Explicit approval-manifest example is missing.");
            List<string> global=Step20ProductionAssetStep103Evidence.ValidateGlobalAuthorityBoundaries();for(int i=0;i<global.Count;i++)failures.Add("GLOBAL: "+global[i]);
            List<string> expansion=Step20ProductionAssetStep112RouteExpansion.ValidateActiveExpansions();for(int i=0;i<expansion.Count;i++)failures.Add("EXPANSION: "+expansion[i]);
            Step20ProductionAssetStep112ReferenceClosure.ClosureStatus closure=Step20ProductionAssetStep112ReferenceClosure.EvaluateAndWrite();
            Step20ProductionAssetStep112RouteExpansion.ExpansionStatus routes=Step20ProductionAssetStep112RouteExpansion.EvaluateAndWrite();
            Step20ProductionAssetStep112StoreRc.StoreStatus store=Step20ProductionAssetStep112StoreRc.EvaluateAndWrite();
            if(closure.approved<10||closure.approved>36)failures.Add("Direct-approved reference count is outside expected 10..36 range: "+closure.approved+".");
            if(routes.active<10||routes.active>36)failures.Add("Active production route count is outside expected 10..36 range: "+routes.active+".");
            if(store.source!=36)failures.Add("Store-RC readiness lost 36/36 source candidate coverage.");
            if(failures.Count>0)throw new InvalidOperationException("v1.20 Step 11.2 self-test FAILED:\n"+string.Join(Environment.NewLine,failures.ToArray()));
            string message="v1.20 Step 11.2 source readiness PASS. Approved refs="+closure.approved+"/36, active routes="+routes.active+"/36, Store-RC decision="+store.decision+".";
            Debug.Log(message);if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus",message,"OK");
        }
    }
}
#endif
