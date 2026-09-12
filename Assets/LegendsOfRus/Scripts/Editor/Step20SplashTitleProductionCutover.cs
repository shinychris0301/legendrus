#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20SplashTitleProductionCutover
    {
        const string StatePath="Assets/LegendsOfRus/Resources/ProductionUI/CutoverState/SplashTitleCutoverState.txt";
        const string State="{\n  \"version\": \"v1.20-visual-page01-splashtitle-production-implementation\",\n  \"screenId\": \"SplashTitle\",\n  \"enabled\": true,\n  \"magic\": \"v1.20-step10.3-meta-page-development-cutover:SplashTitle\",\n  \"approvedReference\": \"Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens/SplashTitle.png\",\n  \"runtime\": \"layered-live-ui\"\n}\n";

        [MenuItem("Legends of Rus/Production UI/v1.20 Visual Page 01/Enable Splash-Title Production Page")]
        public static void Enable()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
            File.WriteAllText(StatePath,State);
            AssetDatabase.ImportAsset(StatePath,ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();
            Debug.Log("SplashTitle production page ENABLED. Start Play Mode from boot to review it.");
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Visual Page 01/Disable Splash-Title Production Page (Rollback)")]
        public static void Disable()
        {
            if(File.Exists(StatePath)) AssetDatabase.DeleteAsset(StatePath);
            AssetDatabase.Refresh();
            Debug.Log("SplashTitle production page DISABLED. The inherited screenshot-free demo route is restored.");
        }
    }
}
#endif
