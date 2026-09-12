#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using LegendsOfRus.OptimizationCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public sealed class TextureOptimizationAuditResult
    {
        public int TextureCount { get; internal set; }
        public int RuntimeTextureCount { get; internal set; }
        public int ReferenceTextureCount { get; internal set; }
        public int AtlasCandidateCount { get; internal set; }
        public long EstimatedRgbaBytes { get; internal set; }
        public List<string> Errors { get; } = new List<string>();
        public List<string> Warnings { get; } = new List<string>();

        public bool Passed { get { return Errors.Count == 0; } }
    }

    public static class TextureOptimizationAudit
    {
        const string Root = "Assets/LegendsOfRus";
        const string RuntimeResources = "Assets/LegendsOfRus/Resources/";
        const string ReferenceOnly = "/ReferenceOnly/";

        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 4 Texture Audit")]
        public static void RunMenu()
        {
            TextureOptimizationAuditResult result = Collect();
            string summary =
                "v1.17 Step 4 texture audit: " +
                (result.Passed ? "PASS" : "FAIL") +
                ", textures=" + result.TextureCount +
                ", runtime=" + result.RuntimeTextureCount +
                ", reference=" + result.ReferenceTextureCount +
                ", atlasCandidates=" + result.AtlasCandidateCount +
                ", estimatedRGBA=" + ToMiB(result.EstimatedRgbaBytes) +
                " MiB";

            if (result.Passed)
                Debug.Log(summary + WarningSuffix(result));
            else
                Debug.LogError(summary + "\n" + string.Join("\n", result.Errors));

            if (!Application.isBatchMode)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Texture Audit",
                    summary +
                    (result.Errors.Count > 0
                        ? "\n\n" + string.Join("\n", result.Errors.Take(12))
                        : WarningSuffix(result)),
                    "OK"
                );
            }
        }

        public static TextureOptimizationAuditResult Collect()
        {
            var result = new TextureOptimizationAuditResult();
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D",
                new[] { Root }
            );
            Array.Sort(guids, StringComparer.Ordinal);

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (string.IsNullOrEmpty(path))
                    continue;

                TextureImporter importer =
                    AssetImporter.GetAtPath(path) as TextureImporter;
                if (importer == null)
                    continue;

                result.TextureCount++;
                bool runtime = path.StartsWith(
                    RuntimeResources,
                    StringComparison.Ordinal
                );
                bool reference = path.IndexOf(
                    ReferenceOnly,
                    StringComparison.Ordinal
                ) >= 0;
                if (runtime) result.RuntimeTextureCount++;
                if (reference) result.ReferenceTextureCount++;

                int width = 0;
                int height = 0;
                try
                {
                    importer.GetSourceTextureWidthAndHeight(out width, out height);
                }
                catch (Exception exception)
                {
                    result.Errors.Add(
                        path + ": source dimensions unavailable: " +
                        exception.GetType().Name
                    );
                    continue;
                }

                if (runtime)
                {
                    result.EstimatedRgbaBytes +=
                        (long)width * height * 4L;
                    ValidateRuntimeTexture(
                        path,
                        width,
                        height,
                        importer,
                        result
                    );

                    if (width <= 512 && height <= 512 &&
                        path.IndexOf(
                            "/Gameplay/",
                            StringComparison.Ordinal
                        ) >= 0)
                    {
                        result.AtlasCandidateCount++;
                    }
                }
                else if (reference)
                {
                    ValidateReferenceTexture(path, width, height, importer, result);
                }
            }

            if (result.RuntimeTextureCount == 0)
                result.Errors.Add("No runtime Resources textures were discovered.");

            if (result.AtlasCandidateCount > 1)
            {
                result.Warnings.Add(
                    result.AtlasCandidateCount +
                    " small gameplay sprites are SpriteAtlas candidates. " +
                    "Atlas creation remains deferred until the Addressables/content " +
                    "delivery foundation so resource identity is not changed here."
                );
            }

            return result;
        }

        static void ValidateRuntimeTexture(
            string path,
            int width,
            int height,
            TextureImporter importer,
            TextureOptimizationAuditResult result
        )
        {
            if (importer.textureType != TextureImporterType.Sprite)
                result.Errors.Add(path + ": textureType must be Sprite.");
            if (importer.isReadable)
                result.Errors.Add(path + ": Read/Write must be disabled.");
            if (importer.mipmapEnabled)
                result.Errors.Add(path + ": mipmaps must be disabled for 2D UI/gameplay art.");
            if (importer.maxTextureSize > OptimizationPolicy.RuntimeTextureMaximumDimension)
                result.Errors.Add(path + ": default maxTextureSize exceeds 2048.");
            if (Math.Max(width, height) > OptimizationPolicy.RuntimeTextureMaximumDimension)
                result.Errors.Add(path + ": source dimension exceeds 2048.");

            TextureImporterPlatformSettings android =
                importer.GetPlatformTextureSettings("Android");
            if (!android.overridden)
                result.Errors.Add(path + ": Android texture override is required.");
            if (android.maxTextureSize > OptimizationPolicy.RuntimeTextureMaximumDimension)
                result.Errors.Add(path + ": Android maxTextureSize exceeds 2048.");
            if (android.format != TextureImporterFormat.ASTC_6x6)
                result.Errors.Add(path + ": Android format must remain ASTC_6x6.");
        }

        static void ValidateReferenceTexture(
            string path,
            int width,
            int height,
            TextureImporter importer,
            TextureOptimizationAuditResult result
        )
        {
            if (Math.Max(width, height) > OptimizationPolicy.RuntimeTextureMaximumDimension)
            {
                result.Warnings.Add(
                    path + ": reference-only source is larger than 2048; " +
                    "it is not treated as a runtime failure."
                );
            }
            if (importer.isReadable)
                result.Warnings.Add(path + ": reference texture is readable.");
        }

        static string WarningSuffix(TextureOptimizationAuditResult result)
        {
            return result.Warnings.Count == 0
                ? string.Empty
                : "\nWarnings:\n" + string.Join("\n", result.Warnings.Take(12));
        }

        static long ToMiB(long bytes)
        {
            return bytes <= 0L ? 0L : bytes / OptimizationPolicy.Mebibyte;
        }
    }
}
#endif
