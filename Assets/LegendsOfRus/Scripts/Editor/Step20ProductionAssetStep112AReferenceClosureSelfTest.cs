#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetStep112AReferenceClosureSelfTest
    {
        const string PauseSha = "31ae057a57b556ba296d4415678cb6aaa1faf72e8bbe4eb6325e42bd64c55716";
        const string ChapterCompleteSha = "d537fc07cc5453512dedc994d850f833eb03a012a53aa8702162d9e99f67c3f8";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 11.2A Pause + Chapter Complete Reference Closure Self-Test")]
        public static void Run()
        {
            var failures = new List<string>();
            Check(ScreenId.Pause, PauseSha, failures);
            Check(ScreenId.ChapterComplete, ChapterCompleteSha, failures);
            if (!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.Pause)) failures.Add("Pause is expected to remain an active core production route.");
            if (!ProductionCoreLoopPageRegistry.IsCutover(ScreenId.ChapterComplete)) failures.Add("ChapterComplete is expected to remain an active core production route.");
            var closure = Step20ProductionAssetStep112ReferenceClosure.EvaluateAndWrite();
            if (closure.approved != 12) failures.Add("Expected 12/36 direct approved functional references after Step 11.2A, found " + closure.approved + ".");
            if (closure.activeMissing != 0) failures.Add("All active routes must now have direct approved visual authority; active missing=" + closure.activeMissing + ".");
            if (failures.Count > 0) throw new InvalidOperationException("v1.20 Step 11.2A self-test FAILED:\n" + string.Join(Environment.NewLine, failures.ToArray()));
            string message = "v1.20 Step 11.2A reference closure PASS. Approved refs=12/36; active authority gaps=0. Runtime fidelity remains a separate Unity-capture gate.";
            Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message, "OK");
        }

        static void Check(ScreenId id, string expectedSha, List<string> failures)
        {
            string path = Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id));
            if (!File.Exists(path)) { failures.Add(id + ": approved reference missing."); return; }
            if (!Step20ProductionAssetStep112ReferenceClosure.HasExpectedDimensions(path)) failures.Add(id + ": approved reference must be 941x1672 PNG.");
            string actual = Step20ProductionAssetStep112ReferenceClosure.Sha256(path);
            if (!string.Equals(actual, expectedSha, StringComparison.OrdinalIgnoreCase)) failures.Add(id + ": approved reference hash differs from the explicitly approved Step 11.2A asset.");
        }
    }
}
#endif
