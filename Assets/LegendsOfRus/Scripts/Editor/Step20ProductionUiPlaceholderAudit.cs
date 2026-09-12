using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20ProductionUiPlaceholderAudit
    {
        const string DesignScreensPath = "Assets/LegendsOfRus/Resources/DesignScreens";
        const string MatrixPath = "Assets/LegendsOfRus/Documentation/V1_20_STEP03_1_PLACEHOLDER_REPLACEMENT_MATRIX.json";
        const string ScreenRouterPath = "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs";

        [MenuItem("Legends of Rus/Production UI/Run v1.20 Step 3.1 Placeholder Audit")]
        public static void Run()
        {
            string root = Directory.GetParent(Application.dataPath).FullName;
            string designDir = Path.Combine(root, DesignScreensPath);
            int designScreenCount = Directory.Exists(designDir)
                ? Directory.GetFiles(designDir, "*.png", SearchOption.TopDirectoryOnly).Length
                : 0;

            int prefabCount = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }).Length;
            string routerSource = ReadProjectFile(root, ScreenRouterPath);
            bool screenshotLoaderPresent =
                routerSource.Contains("DesignScreenRegistry.ResourceKey(id)") &&
                routerSource.Contains("Resources.Load<Sprite>(key)");
            bool matrixPresent = File.Exists(Path.Combine(root, MatrixPath));

            string[] dimensions = Directory.Exists(designDir)
                ? Directory.GetFiles(designDir, "*.png", SearchOption.TopDirectoryOnly)
                    .Select(Path.GetFileNameWithoutExtension)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToArray()
                : Array.Empty<string>();

            Debug.Log(
                "v1.20 Step 3.1 Production UI audit\n" +
                "Runtime DesignScreens PNGs: " + designScreenCount + "\n" +
                "Unity prefabs under Assets: " + prefabCount + "\n" +
                "Screenshot loader present: " + screenshotLoaderPresent + "\n" +
                "Replacement matrix present: " + matrixPresent + "\n" +
                "DesignScreens: " + string.Join(", ", dimensions)
            );
        }

        static string ReadProjectFile(string root, string relativePath)
        {
            string full = Path.Combine(root, relativePath);
            return File.Exists(full) ? File.ReadAllText(full) : string.Empty;
        }
    }
}
