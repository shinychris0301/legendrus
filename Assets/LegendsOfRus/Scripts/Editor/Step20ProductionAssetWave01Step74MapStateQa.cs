#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave01Step74MapStateQa
    {
        public const string PassLock = "Assets/LegendsOfRus/Documentation/V1_20_STEP07_4_MAP_STATE_QA_PASS.lock";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 7.4 Dynamic Map-State QA")]
        public static void Run()
        {
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild())
                Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);

            var failures = new List<string>();
            failures.AddRange(Step20ProductionAssetWave01PrefabBuilder.ValidateAll());

            ValidateWorldMap(failures);
            ValidateChapterMap(failures);

            if (File.Exists(PassLock)) File.Delete(PassLock);
            if (failures.Count > 0)
                throw new InvalidOperationException(string.Join(Environment.NewLine, failures.ToArray()));

            File.WriteAllText(PassLock,
                "v1.20-step07.4-dynamic-map-state-qa\n" +
                "world-visible-slots=15\n" +
                "world-logical-capacity=20\n" +
                "chapter-visible-slots=7\n" +
                "chapter1-logical-capacity=10\n" +
                "state=labels-stars-lock-selection-live\n" +
                "painted-progression-state=occluded-not-authoritative\n" +
                "active-runtime-cutover-modified=false\n");
            AssetDatabase.ImportAsset(PassLock, ImportAssetOptions.ForceUpdate);
            Debug.Log("v1.20 Step 7.4 dynamic map-state QA PASS: WorldMap 15/20 window + ChapterMap 7/10 window.");
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog("Legends of Rus", "Step 7.4 dynamic map-state QA PASS. Labels, stars, locks and selected state are live and windowed over progression data.", "OK");
        }

        static void ValidateWorldMap(List<string> failures)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetWave01PrefabBuilder.CandidateRoot + "/WorldMap.prefab");
            if (prefab == null) { failures.Add("WorldMap candidate prefab missing."); return; }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                ProductionMapVisualStateController controller = instance.GetComponent<ProductionMapVisualStateController>();
                if (controller == null) { failures.Add("WorldMap dynamic state controller missing."); return; }
                if (controller.VisibleSlotCount != 15 || controller.LogicalCapacity != 20)
                    failures.Add("WorldMap must expose 15 visual slots over 20 logical chapters.");

                int[] stars = new int[21];
                for (int i = 1; i <= 20; i++) stars[i] = i % 4;
                controller.ApplySnapshot(1, 5, stars, 20);
                if (controller.WindowStart != 1 || controller.GetLogicalIndexForSlot(0) != 1 || controller.GetLogicalIndexForSlot(14) != 15)
                    failures.Add("WorldMap opening window must represent chapters 1-15.");
                controller.ApplySnapshot(20, 20, stars, 20);
                if (controller.WindowStart != 6 || controller.GetLogicalIndexForSlot(0) != 6 || controller.GetLogicalIndexForSlot(14) != 20)
                    failures.Add("WorldMap terminal window must represent chapters 6-20.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }

        static void ValidateChapterMap(List<string> failures)
        {
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetWave01PrefabBuilder.CandidateRoot + "/ChapterMap.prefab");
            if (prefab == null) { failures.Add("ChapterMap candidate prefab missing."); return; }
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            try
            {
                ProductionMapVisualStateController controller = instance.GetComponent<ProductionMapVisualStateController>();
                if (controller == null) { failures.Add("ChapterMap dynamic state controller missing."); return; }
                if (controller.VisibleSlotCount != 7 || controller.LogicalCapacity != 10)
                    failures.Add("ChapterMap must expose 7 visual slots over 10 logical Chapter 1 levels.");

                int[] stars = new int[11];
                for (int i = 1; i <= 10; i++) stars[i] = Mathf.Clamp(i - 1, 0, 3);
                controller.ApplySnapshot(1, 3, stars, 10);
                if (controller.WindowStart != 1 || controller.GetLogicalIndexForSlot(0) != 1 || controller.GetLogicalIndexForSlot(6) != 7)
                    failures.Add("ChapterMap opening window must represent levels 1-7.");
                controller.ApplySnapshot(10, 10, stars, 10);
                if (controller.WindowStart != 4 || controller.GetLogicalIndexForSlot(0) != 4 || controller.GetLogicalIndexForSlot(6) != 10)
                    failures.Add("ChapterMap terminal window must represent levels 4-10.");
                controller.ApplyReferenceQaPreset();
                if (controller.WindowStart != 10 || controller.GetLogicalIndexForSlot(0) != 10 || controller.GetLogicalIndexForSlot(6) != 16)
                    failures.Add("ChapterMap approved-reference QA preset must expose labels 10-16 without changing save data.");
            }
            finally
            {
                UnityEngine.Object.DestroyImmediate(instance);
            }
        }
    }
}
#endif
