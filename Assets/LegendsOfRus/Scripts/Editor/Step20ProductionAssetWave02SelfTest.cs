#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave02SelfTest
    {
        static readonly ScreenId[] Pages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory,
            ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect,
            ScreenId.RewardClaim
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8 Wave 2 Asset Reconstruction Self-Test")]
        public static void Run()
        {
            Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);
            int checks = 0;
            Require(Pages.Length == 9, "Wave 2 page count must be 9."); checks++;
            Require(ProductionCutoverFreezeGuard.MatchesExpectedCount(10), "Accepted active cutover registry changed."); checks++;
            Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP08_WAVE02_ASSET_MANIFEST.json"), "Wave 2 asset manifest missing."); checks++;
            Require(File.Exists("Assets/LegendsOfRus/Documentation/V1_20_STEP08_WAVE02_REQUIREMENT_COVERAGE.json"), "Wave 2 requirement coverage missing."); checks++;

            foreach (ScreenId id in Pages)
            {
                string path = Step20ProductionAssetWave02PrefabBuilder.CandidateRoot + "/" + id + ".prefab";
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                Require(prefab != null, "Missing Wave 2 prefab: " + path); checks++;
                ProductionCoreLoopPageMarker marker = prefab.GetComponent<ProductionCoreLoopPageMarker>();
                Require(marker != null && marker.ScreenId == id && marker.ScreenshotFree, "Marker mismatch: " + id); checks++;
                Require(prefab.GetComponentsInChildren<Text>(true).Length > 0, "No live text: " + id); checks++;
                foreach (string dep in AssetDatabase.GetDependencies(path, true))
                {
                    string n = dep.Replace('\\', '/');
                    Require(!n.Contains("/Art/ReferenceOnly/") && !n.Contains("/Resources/DesignScreens/") && !n.Contains("/ProjectDocs/"), "Forbidden runtime reference dependency: " + dep); checks++;
                }
            }

            string fontRoot = "Assets/LegendsOfRus/Art/Fonts/Production";
            if (AssetDatabase.IsValidFolder(fontRoot))
            {
                string[] fontAssets = AssetDatabase.FindAssets("t:Font", new[] { fontRoot });
                Require(fontAssets == null || fontAssets.Length == 0, "Step 8 must not bundle a font binary; production font is supplied separately under license."); checks++;
            }

            Debug.Log("v1.20 Step 8 Wave 2 self-test PASS. Checks=" + checks + ". Candidates=9. Active production registry unchanged.");
        }

        static void Require(bool condition, string message)
        {
            if (!condition) throw new InvalidOperationException("v1.20 Step 8 self-test: " + message);
        }
    }
}
#endif
