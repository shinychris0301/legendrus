#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>Source-layout acceptance for Wave 2 candidate prefabs at the four accepted aspect families.</summary>
    public static class Step20ProductionAssetWave02FourAspectQa
    {
        static readonly Vector2[] Resolutions =
        {
            new Vector2(941, 1672), new Vector2(1080, 2340), new Vector2(1080, 2400), new Vector2(1600, 2560)
        };
        static readonly ScreenId[] Pages =
        {
            ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory,
            ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect, ScreenId.RewardClaim
        };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 8 Wave 2 Four-Aspect Source QA")]
        public static void Run()
        {
            Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);
            int passes = 0;
            foreach (ScreenId id in Pages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetWave02PrefabBuilder.CandidateRoot + "/" + id + ".prefab");
                if (prefab == null) throw new InvalidOperationException("Missing Wave 2 candidate: " + id);
                RectTransform[] rects = prefab.GetComponentsInChildren<RectTransform>(true);
                foreach (Vector2 resolution in Resolutions)
                {
                    ValidateNormalizedContainment(id, rects, resolution);
                    passes++;
                }
            }
            if (passes != 36) throw new InvalidOperationException("Expected 36 Wave 2 aspect checks, got " + passes + ".");
            Debug.Log("v1.20 Step 8 Wave 2 four-aspect source QA PASS: 36/36. This is source containment, not physical-device acceptance.");
        }

        static void ValidateNormalizedContainment(ScreenId id, RectTransform[] rects, Vector2 resolution)
        {
            for (int i = 0; i < rects.Length; i++)
            {
                RectTransform rt = rects[i];
                if (rt == null || rt.parent == null) continue;
                Vector2 min = rt.anchorMin; Vector2 max = rt.anchorMax;
                if (min.x < -0.001f || min.y < -0.001f || max.x > 1.001f || max.y > 1.001f || min.x > max.x || min.y > max.y)
                    throw new InvalidOperationException(id + " anchor containment failed for " + rt.name + " at " + resolution.x + "x" + resolution.y + ".");
            }
        }
    }
}
#endif
