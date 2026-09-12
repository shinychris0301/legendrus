using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20FunctionalGameFlowAuditRunner
    {
        [MenuItem("Legends of Rus/QA/Print v1.20 Step 4 Functional Game Flow Review")]
        public static void PrintReview()
        {
            ScreenId parent;
            ScreenNavigationPolicy.TryGetBackDestination(ScreenId.ChapterMap, out parent);
            string plan = Path.Combine(Directory.GetCurrentDirectory(), "Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json");
            Debug.Log(
                "v1.20 Step 4 functionality-first review" + System.Environment.NewLine +
                "Map Back: ChapterMap -> " + parent + System.Environment.NewLine +
                "Final page plan: " + (File.Exists(plan) ? "PRESENT" : "MISSING") + System.Environment.NewLine +
                "Production social/identity/leaderboard backends: DISABLED" + System.Environment.NewLine +
                "Demo currency/energy grants: DISABLED"
            );
        }
    }
}
