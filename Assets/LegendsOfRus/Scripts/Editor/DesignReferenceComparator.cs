#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    [Serializable]
    public sealed class DesignComparisonMetrics
    {
        public bool passed;
        public string approvedPath;
        public string runtimePath;
        public string metadataPath;
        public string diffPath;
        public int referenceWidth;
        public int referenceHeight;
        public int runtimeWidth;
        public int runtimeHeight;
        public int comparedPixels;
        public Rect runtimeViewportCrop;
        public Rect runtimeArtworkCrop;
        public double meanAbsoluteRgbError;
        public float percentile95Error;
        public float maximumError;
        public float badPixelFraction;
        public float badPixelThreshold;
        public float meanLimit;
        public float percentile95Limit;
        public float badPixelFractionLimit;
        public bool androidThresholds;
        public bool layoutPassed;
        public string failure;
    }

    public static class DesignReferenceComparator
    {
        const float EditorMeanLimit = 0.025f;
        const float EditorP95Limit = 0.06f;
        const float EditorBadFractionLimit = 0.01f;
        const float AndroidMeanLimit = 0.04f;
        const float AndroidP95Limit = 0.10f;
        const float AndroidBadFractionLimit = 0.03f;
        const float BadPixelThreshold = 0.10f;

        [MenuItem("Legends of Rus/QA/Compare Runtime Capture To Approved Design")]
        public static void Compare()
        {
            string approved = EditorUtility.OpenFilePanel(
                "Approved design PNG",
                Application.dataPath,
                "png"
            );
            if (string.IsNullOrEmpty(approved)) return;

            string runtime = EditorUtility.OpenFilePanel(
                "Runtime capture PNG",
                Application.dataPath,
                "png"
            );
            if (string.IsNullOrEmpty(runtime)) return;

            CompareFiles(approved, runtime, true, out _);
        }

        public static bool CompareFiles(
            string approvedPath,
            string runtimePath,
            bool showDialog,
            out DesignComparisonMetrics metrics
        )
        {
            metrics = null;
            Texture2D approved = null;
            Texture2D runtime = null;
            Texture2D diff = null;

            try
            {
                approved = Load(approvedPath);
                runtime = Load(runtimePath);
                if (approved == null || runtime == null)
                    return Fail("Could not load one or both comparison PNGs.", showDialog);

                string metadataPath = Path.ChangeExtension(runtimePath, ".json");
                if (!File.Exists(metadataPath))
                {
                    return Fail(
                        "A valid v1.14 Step 4 capture metadata JSON sidecar is required.",
                        showDialog
                    );
                }

                FidelityCaptureMetadata metadata = LoadMetadata(metadataPath);
                if (metadata == null)
                    return Fail("The capture metadata sidecar is invalid.", showDialog);
                if (
                    !string.Equals(
                        metadata.milestone,
                        "v1.14-step04",
                        StringComparison.Ordinal
                    )
                )
                {
                    return Fail(
                        "The metadata sidecar is not a v1.14 Step 4 capture.",
                        showDialog
                    );
                }
                if (
                    metadata.actualWidth != runtime.width ||
                    metadata.actualHeight != runtime.height
                )
                {
                    return Fail(
                        "Capture metadata dimensions do not match the runtime PNG.",
                        showDialog
                    );
                }
                if (
                    !string.Equals(
                        metadata.pngFileName,
                        Path.GetFileName(runtimePath),
                        StringComparison.Ordinal
                    )
                )
                {
                    return Fail(
                        "Capture metadata does not belong to the selected runtime PNG.",
                        showDialog
                    );
                }
                if (metadata.screenTransitioning || metadata.modalTransitioning)
                {
                    return Fail(
                        "A capture recorded during a screen or modal transition cannot be compared.",
                        showDialog
                    );
                }

                if (
                    !TryClampCrop(
                        metadata.viewportRect,
                        runtime.width,
                        runtime.height,
                        out Rect viewportCrop
                    )
                )
                    return Fail("Capture viewport is empty or outside the runtime PNG.", showDialog);

                Rect artworkCrop = CalculateAspectFitCrop(
                    viewportCrop,
                    approved.width,
                    approved.height
                );

                int width = approved.width;
                int height = approved.height;
                int pixelCount = checked(width * height);
                var errors = new float[pixelCount];
                var diffPixels = new Color32[pixelCount];
                double total = 0d;
                float maximum = 0f;
                int badPixels = 0;
                int index = 0;

                for (int y = 0; y < height; y++)
                {
                    float v = (artworkCrop.y + ((y + 0.5f) / height) * artworkCrop.height) /
                        runtime.height;
                    for (int x = 0; x < width; x++)
                    {
                        Color referenceColor = approved.GetPixel(x, y);
                        float u = (artworkCrop.x + ((x + 0.5f) / width) * artworkCrop.width) /
                            runtime.width;
                        Color runtimeColor = runtime.GetPixelBilinear(u, v);
                        float error = (
                            Mathf.Abs(referenceColor.r - runtimeColor.r) +
                            Mathf.Abs(referenceColor.g - runtimeColor.g) +
                            Mathf.Abs(referenceColor.b - runtimeColor.b)
                        ) / 3f;

                        errors[index] = error;
                        total += error;
                        maximum = Mathf.Max(maximum, error);
                        if (error > BadPixelThreshold) badPixels++;

                        byte heat = (byte)Mathf.RoundToInt(
                            Mathf.Clamp01(error * 4f) * 255f
                        );
                        diffPixels[index] = new Color32(heat, heat, heat, 255);
                        index++;
                    }
                }

                Array.Sort(errors);
                int p95Index = Mathf.Clamp(
                    Mathf.CeilToInt(errors.Length * 0.95f) - 1,
                    0,
                    errors.Length - 1
                );
                float p95 = errors[p95Index];
                double mean = total / pixelCount;
                float badFraction = (float)badPixels / pixelCount;
                bool android = !string.IsNullOrEmpty(metadata.platform) &&
                    metadata.platform.IndexOf(
                        "Android",
                        StringComparison.OrdinalIgnoreCase
                    ) >= 0;
                float meanLimit = android ? AndroidMeanLimit : EditorMeanLimit;
                float p95Limit = android ? AndroidP95Limit : EditorP95Limit;
                float badLimit = android
                    ? AndroidBadFractionLimit
                    : EditorBadFractionLimit;
                bool layoutPassed = metadata.layout.passed;
                bool passed =
                    layoutPassed &&
                    mean <= meanLimit &&
                    p95 <= p95Limit &&
                    badFraction <= badLimit;

                string stem = Path.GetFileNameWithoutExtension(runtimePath);
                string outputFolder = Path.GetDirectoryName(runtimePath) ?? string.Empty;
                string diffPath = Path.Combine(
                    outputFolder,
                    stem + "_vs_approved_diff.png"
                );
                string metricsPath = Path.Combine(
                    outputFolder,
                    stem + "_vs_approved_metrics.json"
                );

                diff = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
                diff.SetPixels32(diffPixels);
                diff.Apply(false, false);
                File.WriteAllBytes(diffPath, ImageConversion.EncodeToPNG(diff));

                metrics = new DesignComparisonMetrics
                {
                    passed = passed,
                    approvedPath = approvedPath,
                    runtimePath = runtimePath,
                    metadataPath = metadataPath,
                    diffPath = diffPath,
                    referenceWidth = width,
                    referenceHeight = height,
                    runtimeWidth = runtime.width,
                    runtimeHeight = runtime.height,
                    comparedPixels = pixelCount,
                    runtimeViewportCrop = viewportCrop,
                    runtimeArtworkCrop = artworkCrop,
                    meanAbsoluteRgbError = mean,
                    percentile95Error = p95,
                    maximumError = maximum,
                    badPixelFraction = badFraction,
                    badPixelThreshold = BadPixelThreshold,
                    meanLimit = meanLimit,
                    percentile95Limit = p95Limit,
                    badPixelFractionLimit = badLimit,
                    androidThresholds = android,
                    layoutPassed = layoutPassed,
                    failure = passed
                        ? string.Empty
                        : "One or more fidelity acceptance thresholds were exceeded.",
                };
                File.WriteAllText(metricsPath, JsonUtility.ToJson(metrics, true));

                string summary =
                    $"Result: {(passed ? "PASS" : "FAIL")}\n" +
                    $"Mean RGB error: {mean:0.0000} / {meanLimit:0.0000}\n" +
                    $"95th percentile: {p95:0.0000} / {p95Limit:0.0000}\n" +
                    $"Pixels over {BadPixelThreshold:0.00}: " +
                    $"{badFraction:P2} / {badLimit:P2}\n" +
                    $"Layout: {(layoutPassed ? "PASS" : "FAIL")}\n\n" +
                    $"Diff: {diffPath}\nMetrics: {metricsPath}";
                Debug.Log("Design fidelity comparison: " + summary.Replace('\n', ' '));
                if (showDialog)
                    EditorUtility.DisplayDialog("Legends of Rus Design QA", summary, "OK");
                return passed;
            }
            catch (Exception exception)
            {
                return Fail("Design comparison failed: " + exception.Message, showDialog);
            }
            finally
            {
                if (approved != null) UnityEngine.Object.DestroyImmediate(approved);
                if (runtime != null) UnityEngine.Object.DestroyImmediate(runtime);
                if (diff != null) UnityEngine.Object.DestroyImmediate(diff);
            }
        }

        static FidelityCaptureMetadata LoadMetadata(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                return JsonUtility.FromJson<FidelityCaptureMetadata>(File.ReadAllText(path));
            }
            catch (Exception exception)
            {
                Debug.LogError($"Could not parse capture metadata '{path}': {exception.Message}");
                return null;
            }
        }

        static bool TryClampCrop(Rect source, int width, int height, out Rect crop)
        {
            float xMin = Mathf.Clamp(source.xMin, 0f, width);
            float yMin = Mathf.Clamp(source.yMin, 0f, height);
            float xMax = Mathf.Clamp(source.xMax, xMin, width);
            float yMax = Mathf.Clamp(source.yMax, yMin, height);
            crop = Rect.MinMaxRect(xMin, yMin, xMax, yMax);
            return crop.width >= 1f && crop.height >= 1f;
        }

        static Rect CalculateAspectFitCrop(Rect viewport, int artworkWidth, int artworkHeight)
        {
            float artworkAspect = (float)artworkWidth / artworkHeight;
            float viewportAspect = viewport.width / viewport.height;

            if (artworkAspect > viewportAspect)
            {
                float height = viewport.width / artworkAspect;
                return new Rect(
                    viewport.x,
                    viewport.y + (viewport.height - height) * 0.5f,
                    viewport.width,
                    height
                );
            }

            float width = viewport.height * artworkAspect;
            return new Rect(
                viewport.x + (viewport.width - width) * 0.5f,
                viewport.y,
                width,
                viewport.height
            );
        }

        static bool Fail(string message, bool showDialog)
        {
            Debug.LogError(message);
            if (showDialog)
                EditorUtility.DisplayDialog("Legends of Rus Design QA", message, "OK");
            return false;
        }

        static Texture2D Load(string path)
        {
            if (!File.Exists(path)) return null;
            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false, false);
            if (ImageConversion.LoadImage(texture, File.ReadAllBytes(path), false))
            {
                texture.filterMode = FilterMode.Bilinear;
                texture.wrapMode = TextureWrapMode.Clamp;
                return texture;
            }
            UnityEngine.Object.DestroyImmediate(texture);
            return null;
        }
    }
}
#endif
