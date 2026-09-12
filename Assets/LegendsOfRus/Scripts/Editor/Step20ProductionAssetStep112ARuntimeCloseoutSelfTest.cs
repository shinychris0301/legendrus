#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep112ARuntimeCloseoutSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 11.2A Runtime Closeout Readiness Self-Test")]
        public static void Run()
        {
            var failures = new List<string>();
            CheckPage(ScreenId.Pause, failures);
            CheckPage(ScreenId.ChapterComplete, failures);

            if (!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Pause)) failures.Add("Pause must remain an active core production route.");
            if (!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.ChapterComplete)) failures.Add("ChapterComplete must remain an active core production route.");
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);
            List<string> candidateFailures = Step20ProductionAssetWave01PrefabBuilder.ValidateAll();
            for (int i = 0; i < candidateFailures.Count; i++) failures.Add("WAVE1-CANDIDATE: " + candidateFailures[i]);

            bool pauseEvidence = Step20ProductionAssetStep112ARuntimeCloseout.HasFocusedEvidence(ScreenId.Pause);
            bool completeEvidence = Step20ProductionAssetStep112ARuntimeCloseout.HasFocusedEvidence(ScreenId.ChapterComplete);
            if (pauseEvidence != completeEvidence) failures.Add("Focused runtime closeout evidence must be complete for both pages or absent for both pages.");

            if (pauseEvidence && completeEvidence)
            {
                Step20ProductionAssetStep103Evidence.PageSpec pauseSpec = FindSpec(ScreenId.Pause);
                Step20ProductionAssetStep103Evidence.PageSpec completeSpec = FindSpec(ScreenId.ChapterComplete);
                Step20ProductionAssetStep103Evidence.PageState pause = Step20ProductionAssetStep103Evidence.Evaluate(pauseSpec, true);
                Step20ProductionAssetStep103Evidence.PageState complete = Step20ProductionAssetStep103Evidence.Evaluate(completeSpec, true);
                if (!pause.runtimeEvidence || !complete.runtimeEvidence) failures.Add("Step 10.3 evidence model did not recognize focused Step 11.2A runtime evidence.");
                Debug.Log("Step 11.2A focused evidence present. Pause fidelity=" + pause.fidelityNote + ", ChapterComplete fidelity=" + complete.fidelityNote + ".");
            }

            if (failures.Count > 0)
                throw new InvalidOperationException("v1.20 Step 11.2A runtime closeout readiness FAILED:\n" + string.Join(Environment.NewLine, failures.ToArray()));

            string message = pauseEvidence
                ? "v1.20 Step 11.2A runtime closeout readiness PASS; focused 8-capture evidence is present and recognized. Run the focused fidelity analyzer."
                : "v1.20 Step 11.2A runtime closeout readiness PASS. Approved refs/candidates are ready; run the focused 8-capture command next.";
            Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message, "OK");
        }

        static void CheckPage(ScreenId id, List<string> failures)
        {
            string reference = Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id));
            if (!File.Exists(reference)) failures.Add(id + ": approved reference missing.");
            else if (!Step20ProductionAssetStep112ReferenceClosure.HasExpectedDimensions(reference)) failures.Add(id + ": approved reference must be 941x1672 PNG.");
        }

        static Step20ProductionAssetStep103Evidence.PageSpec FindSpec(ScreenId id)
        {
            for (int i = 0; i < Step20ProductionAssetStep103Evidence.Pages.Length; i++)
                if (Step20ProductionAssetStep103Evidence.Pages[i].id == id) return Step20ProductionAssetStep103Evidence.Pages[i];
            throw new InvalidOperationException("Page spec missing: " + id);
        }
    }
}
#endif
