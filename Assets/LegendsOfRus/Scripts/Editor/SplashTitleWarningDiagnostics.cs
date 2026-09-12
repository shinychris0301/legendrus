#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Text;
using LegendsOfRus.OptimizationCore;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

namespace LegendsOfRus.Editor
{
    /// <summary>Read-only diagnostics and a persistent, truthful font status display.</summary>
    [InitializeOnLoad]
    public static class SplashTitleWarningDiagnostics
    {
        static SplashTitleWarningDiagnostics()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode)
                SplashTitleTypography.ReleaseDevelopmentSession();
        }

        [MenuItem("Legends of Rus/Production UI/Splash R3/Print Font and Memory Diagnostics")]
        public static void PrintDiagnostics()
        {
            // Measure first. Inspecting Assets below can itself affect Editor memory.
            long allocated = Profiler.GetTotalAllocatedMemoryLong();
            long reserved = Profiler.GetTotalReservedMemoryLong();
            long managed = GC.GetTotalMemory(false);
            var report = new StringBuilder();
            report.AppendLine("Splash R3 warning diagnostics — EDITOR observation only");
            report.Append("Unity: ").Append(Application.unityVersion)
                .Append(" | Play Mode: ").Append(Application.isPlaying).AppendLine();
            report.AppendLine("This is not an Android memory measurement or a visual-reference pass.");
            report.Append("Allocated: ").Append(MiB(allocated)).Append(" MiB / warning ")
                .Append(MiB(OptimizationPolicy.AllocatedWarningBytes)).AppendLine(" MiB");
            report.Append("Reserved: ").Append(MiB(reserved)).Append(" MiB / warning ")
                .Append(MiB(OptimizationPolicy.ReservedWarningBytes)).AppendLine(" MiB");
            report.Append("Managed: ").Append(MiB(managed)).Append(" MiB / warning ")
                .Append(MiB(OptimizationPolicy.ManagedWarningBytes)).AppendLine(" MiB");
            report.Append("Comparison band: ")
                .Append(MemoryBudgetEvaluator.Evaluate(allocated, reserved, managed)).AppendLine();
            report.AppendLine("These three values are overlapping views of memory, not three quantities to add.");

            var typography = AssetDatabase.LoadAssetAtPath<SplashTitleTypography>(
                Step20SplashTitleR3PrefabRebuilder.TypographyPath);
            report.AppendLine();
            report.AppendLine("Font binding status (nothing assigned automatically):");
            if (typography == null)
                report.AppendLine("Typography configuration asset is missing.");
            else
            {
                AppendFont(report, "RegularFont", typography.RegularFont);
                AppendFont(report, "StartFont", typography.StartFont);
                string issue;
                report.Append("Exact-font requirement: ").AppendLine(
                    typography.TryGetBindingIssue(out issue) ? "OPEN — " + issue : "Configured as verified");
                report.Append("Declared identity: ").AppendLine(typography.ReferenceFontIdentity);
                report.AppendLine("A true identity claim needs source evidence; a font name or glyph check is not proof of a match.");
            }

            // Only inspect already-loaded textures. Never load all project textures.
            var rows = new List<TextureRow>();
            foreach (Texture2D texture in Resources.FindObjectsOfTypeAll<Texture2D>())
            {
                if (texture == null) continue;
                rows.Add(new TextureRow { Texture = texture, Bytes = Profiler.GetRuntimeMemorySizeLong(texture) });
            }
            rows.Sort((a, b) => b.Bytes.CompareTo(a.Bytes));
            report.AppendLine();
            report.AppendLine("Largest loaded Texture2D objects (Editor includes previews/internal textures):");
            for (int i = 0; i < Math.Min(15, rows.Count); i++)
            {
                Texture2D texture = rows[i].Texture;
                string path = AssetDatabase.GetAssetPath(texture);
                report.Append(MiB(rows[i].Bytes)).Append(" MiB | ")
                    .Append(texture.width).Append('x').Append(texture.height)
                    .Append(" | ").Append(texture.format)
                    .Append(" | readable=").Append(texture.isReadable)
                    .Append(" | ").AppendLine(string.IsNullOrEmpty(path) ? texture.name + " (no asset path)" : path);
            }
            report.AppendLine("Profiler object estimates do not identify a leak or explain all reserved memory.");
            report.AppendLine("No texture import settings, source artwork, thresholds, or font bindings were changed.");
            Debug.Log(report.ToString(), typography);
        }

        static void AppendFont(StringBuilder report, string slot, Font font)
        {
            report.Append(slot).Append(": ");
            if (font == null) { report.AppendLine("NOT ASSIGNED"); return; }
            report.Append(font.name).Append(" | ").Append(AssetDatabase.GetAssetPath(font))
                .Append(" | required glyph check=").Append(SplashTitleTypography.SupportsRequiredGlyphs(font))
                .AppendLine();
        }

        static string MiB(long bytes)
        {
            return (bytes / (double)OptimizationPolicy.Mebibyte).ToString("F1", System.Globalization.CultureInfo.InvariantCulture);
        }

        struct TextureRow { public Texture2D Texture; public long Bytes; }
    }

    [CustomEditor(typeof(SplashTitleTypography))]
    public sealed class SplashTitleTypographyInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.HelpBox(
                "Live text needs imported Unity Font assets. This screen uses UnityEngine.UI.Text, not TMP_FontAsset. " +
                "An available development font is not proof of an exact visual match. The title-logo lettering stays in the PNG.",
                MessageType.Info);
            using (new EditorGUI.DisabledScope(Application.isPlaying))
                DrawDefaultInspector();
            var config = (SplashTitleTypography)target;
            string issue;
            if (config.TryGetBindingIssue(out issue))
                EditorGUILayout.HelpBox(issue + " Non-development builds remain blocked by the existing exact-font guard. " +
                    "Do not enable Reference Identity Verified only to silence the warning.", MessageType.Warning);
            else
                EditorGUILayout.HelpBox("Font bindings and declaration pass the configured checks. " +
                    "This Inspector does not visually verify the source identity.", MessageType.Info);
            if (Application.isPlaying)
                EditorGUILayout.HelpBox("Stop Play Mode before changing persistent font settings.", MessageType.Info);
            if (GUILayout.Button("Print font and memory diagnostics"))
                SplashTitleWarningDiagnostics.PrintDiagnostics();
        }
    }
}
#endif
