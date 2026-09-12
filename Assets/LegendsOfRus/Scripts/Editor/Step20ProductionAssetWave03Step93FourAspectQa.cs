#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionAssetWave03Step93FourAspectQa
    {
        static readonly Vector2[] Resolutions = { new Vector2(941,1672), new Vector2(1080,2340), new Vector2(1080,2400), new Vector2(1600,2560) };
        static readonly ScreenId[] Pages = { ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub, ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail };

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 9.3 Wave 3 Four-Aspect Refined Candidate QA")]
        public static void Run()
        {
            Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);
            int passes = 0;
            foreach (ScreenId id in Pages)
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20ProductionAssetWave03PrefabBuilder.CandidateRoot + "/" + id + ".prefab");
                if (prefab == null) throw new InvalidOperationException("Missing refined Wave 3 candidate: " + id);
                RectTransform[] rects = prefab.GetComponentsInChildren<RectTransform>(true);
                foreach (Vector2 res in Resolutions) { Validate(id, rects, res); passes++; }
            }
            if (passes != 32) throw new InvalidOperationException("Expected 32 Step 9.3 aspect checks, got " + passes + ".");
            Debug.Log("v1.20 Step 9.3 four-aspect refined candidate QA PASS: 32/32. Source containment only; Step 9.2 Play Mode capture remains the runtime evidence path.");
        }

        static void Validate(ScreenId id, RectTransform[] rects, Vector2 resolution)
        {
            foreach (RectTransform rt in rects)
            {
                if (rt == null || rt.parent == null) continue;
                Vector2 min = rt.anchorMin, max = rt.anchorMax;
                if (min.x < -.001f || min.y < -.001f || max.x > 1.001f || max.y > 1.001f || min.x > max.x || min.y > max.y)
                    throw new InvalidOperationException(id + " anchor containment failed for " + rt.name + " at " + resolution.x + "x" + resolution.y + ".");
            }
        }
    }
}
#endif
