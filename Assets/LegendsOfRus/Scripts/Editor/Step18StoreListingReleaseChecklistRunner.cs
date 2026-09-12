#if UNITY_EDITOR
using System;
using System.IO;
using LegendsOfRus.StoreRelease;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step18StoreListingReleaseChecklistRunner
    {
        [MenuItem("Legends of Rus/QA/Print v1.18 Step 3 Store Release Checklist")]
        public static void Print()
        {
            StoreReleaseChecklistResult result = StoreReleaseChecklistEvaluator.Evaluate(default);
            string inventory = Read("Assets/LegendsOfRus/Documentation/V1_18_STEP03_STORE_LISTING_ASSET_INVENTORY.json");
            string checklist = Read("Assets/LegendsOfRus/Documentation/V1_18_STEP03_RELEASE_CHECKLIST.json");

            Debug.Log(
                "v1.18 Step 3 Store Release Checklist\n" +
                "May create controlled-test artifact: " + result.MayCreateControlledTestArtifact + "\n" +
                "May upload closed test: " + result.MayUploadClosedTest + "\n" +
                "Artifact blockers: " + string.Join(", ", result.ArtifactBlockers) + "\n" +
                "Upload blockers: " + string.Join(", ", result.UploadBlockers) + "\n" +
                "Warnings: " + string.Join(", ", result.Warnings) + "\n\n" +
                "Current source inventory:\n" + inventory + "\n" +
                "Current evidence snapshot:\n" + checklist + "\n" +
                "No authorization is inferred from running this menu item.");
        }

        static string Read(string relative)
        {
            string path = Path.GetFullPath(Path.Combine(Application.dataPath, "..", relative));
            return File.ReadAllText(path);
        }
    }
}
#endif
