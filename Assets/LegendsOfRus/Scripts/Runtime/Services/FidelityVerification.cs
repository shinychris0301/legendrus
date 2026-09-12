using System;
using UnityEngine;

namespace LegendsOfRus
{
    [Serializable]
    public struct FidelityTarget
    {
        public string id;
        public string label;
        public int width;
        public int height;

        public FidelityTarget(string targetId, string targetLabel, int targetWidth, int targetHeight)
        {
            id = targetId;
            label = targetLabel;
            width = targetWidth;
            height = targetHeight;
        }

        public Vector2Int Size => new Vector2Int(width, height);
    }

    [Serializable]
    public struct FidelityLayoutEvaluation
    {
        public bool passed;
        public Rect expectedViewport;
        public Rect actualViewport;
        public float maximumEdgeErrorPixels;
        public float centerErrorPixels;
        public float aspectErrorPixels;
        public float safeAreaOverflowPixels;
        public string failure;
    }

    [Serializable]
    public sealed class FidelityCaptureMetadata
    {
        public string milestone = "v1.14-step04";
        public string captureId;
        public string targetId;
        public string targetLabel;
        public string screenId;
        public string modalId;
        public string utcTimestamp;
        public string unityVersion;
        public string platform;
        public string deviceModel;
        public string graphicsDevice;
        public string colorSpace;
        public string pngFileName;
        public int requestedWidth;
        public int requestedHeight;
        public int actualWidth;
        public int actualHeight;
        public Rect safeArea;
        public Rect viewportRect;
        public float canvasScaleFactor;
        public float canvasMatchWidthOrHeight;
        public int routeGeneration;
        public int frameCount;
        public bool screenTransitioning;
        public bool modalTransitioning;
        public FidelityLayoutEvaluation layout;
    }

    [Serializable]
    public sealed class FidelityCaptureRequest
    {
        public string captureId;
        public string targetId;
        public string targetLabel;
        public string outputDirectory;
        public int requestedWidth;
        public int requestedHeight;
        public bool showToast;
    }

    public sealed class FidelityCaptureResult
    {
        public bool Success { get; }
        public string PngPath { get; }
        public string MetadataPath { get; }
        public string Error { get; }
        public FidelityCaptureMetadata Metadata { get; }

        public FidelityCaptureResult(
            bool success,
            string pngPath,
            string metadataPath,
            string error,
            FidelityCaptureMetadata metadata
        )
        {
            Success = success;
            PngPath = pngPath ?? string.Empty;
            MetadataPath = metadataPath ?? string.Empty;
            Error = error ?? string.Empty;
            Metadata = metadata;
        }
    }

    /// <summary>
    /// Pure layout plan and evaluator for the locked 941x1672 portrait design.
    /// The four targets intentionally share one pixel width so scale drift can be
    /// distinguished from the additional letterbox space on taller displays.
    /// </summary>
    public static class FidelityVerification
    {
        public const float DesignWidth = 941f;
        public const float DesignHeight = 1672f;
        public const float DefaultTolerancePixels = 1f;

        static readonly FidelityTarget[] Targets =
        {
            new FidelityTarget("16x9", "16:9", 1080, 1920),
            new FidelityTarget("18x9", "18:9", 1080, 2160),
            new FidelityTarget("19_5x9", "19.5:9", 1080, 2340),
            new FidelityTarget("20x9", "20:9", 1080, 2400),
        };

        public static int TargetCount => Targets.Length;
        public static float DesignAspect => DesignWidth / DesignHeight;

        public static FidelityTarget GetTarget(int index)
        {
            if (index < 0 || index >= Targets.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            return Targets[index];
        }

        public static Rect NormalizeSafeArea(Rect safeArea, Vector2Int screenSize)
        {
            float width = Mathf.Max(1, screenSize.x);
            float height = Mathf.Max(1, screenSize.y);
            Rect full = new Rect(0f, 0f, width, height);

            if (
                !IsFinite(safeArea.x) ||
                !IsFinite(safeArea.y) ||
                !IsFinite(safeArea.width) ||
                !IsFinite(safeArea.height) ||
                safeArea.width <= 0f ||
                safeArea.height <= 0f
            )
                return full;

            float xMin = Mathf.Clamp(safeArea.xMin, 0f, width);
            float yMin = Mathf.Clamp(safeArea.yMin, 0f, height);
            float xMax = Mathf.Clamp(safeArea.xMax, xMin, width);
            float yMax = Mathf.Clamp(safeArea.yMax, yMin, height);

            if (xMax - xMin < 1f || yMax - yMin < 1f)
                return full;

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        public static Rect CalculateExpectedViewport(Rect safeArea, Vector2Int screenSize)
        {
            Rect safe = NormalizeSafeArea(safeArea, screenSize);
            float availableAspect = safe.width / safe.height;
            float viewportWidth;
            float viewportHeight;

            if (availableAspect > DesignAspect)
            {
                viewportHeight = safe.height;
                viewportWidth = viewportHeight * DesignAspect;
            }
            else
            {
                viewportWidth = safe.width;
                viewportHeight = viewportWidth / DesignAspect;
            }

            return new Rect(
                safe.x + (safe.width - viewportWidth) * 0.5f,
                safe.y + (safe.height - viewportHeight) * 0.5f,
                viewportWidth,
                viewportHeight
            );
        }

        public static float ResolveCanvasMatch(Vector2Int screenSize)
        {
            float width = Mathf.Max(1, screenSize.x);
            float height = Mathf.Max(1, screenSize.y);
            return width / height > DesignAspect ? 1f : 0f;
        }

        public static FidelityLayoutEvaluation Evaluate(
            Rect safeArea,
            Vector2Int screenSize,
            Rect actualViewport,
            float tolerancePixels = DefaultTolerancePixels
        )
        {
            Rect safe = NormalizeSafeArea(safeArea, screenSize);
            Rect expected = CalculateExpectedViewport(safe, screenSize);
            float edgeError = Mathf.Max(
                Mathf.Abs(expected.xMin - actualViewport.xMin),
                Mathf.Abs(expected.yMin - actualViewport.yMin),
                Mathf.Abs(expected.xMax - actualViewport.xMax),
                Mathf.Abs(expected.yMax - actualViewport.yMax)
            );
            float centerError = Vector2.Distance(expected.center, actualViewport.center);
            float aspectHeight = actualViewport.width / DesignAspect;
            float aspectWidth = actualViewport.height * DesignAspect;
            float aspectError = Mathf.Min(
                Mathf.Abs(actualViewport.height - aspectHeight),
                Mathf.Abs(actualViewport.width - aspectWidth)
            );
            float safeOverflow = Mathf.Max(
                0f,
                safe.xMin - actualViewport.xMin,
                safe.yMin - actualViewport.yMin,
                actualViewport.xMax - safe.xMax,
                actualViewport.yMax - safe.yMax
            );

            float tolerance = Mathf.Max(0f, tolerancePixels);
            bool validActual =
                IsFinite(actualViewport.x) &&
                IsFinite(actualViewport.y) &&
                IsFinite(actualViewport.width) &&
                IsFinite(actualViewport.height) &&
                actualViewport.width > 0f &&
                actualViewport.height > 0f;
            bool passed =
                validActual &&
                edgeError <= tolerance &&
                centerError <= tolerance &&
                aspectError <= tolerance &&
                safeOverflow <= tolerance;

            string failure = string.Empty;
            if (!validActual)
                failure = "Viewport bounds are invalid.";
            else if (safeOverflow > tolerance)
                failure = "Viewport extends outside the safe area.";
            else if (edgeError > tolerance)
                failure = "Viewport does not match the deterministic fit plan.";
            else if (centerError > tolerance)
                failure = "Viewport is not centered in the safe area.";
            else if (aspectError > tolerance)
                failure = "Viewport aspect ratio drift exceeds tolerance.";

            return new FidelityLayoutEvaluation
            {
                passed = passed,
                expectedViewport = expected,
                actualViewport = actualViewport,
                maximumEdgeErrorPixels = edgeError,
                centerErrorPixels = centerError,
                aspectErrorPixels = aspectError,
                safeAreaOverflowPixels = safeOverflow,
                failure = failure,
            };
        }

        public static string SanitizeFileToken(string value, string fallback)
        {
            string source = string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
            char[] chars = source.ToCharArray();
            char[] invalid = System.IO.Path.GetInvalidFileNameChars();

            for (int i = 0; i < chars.Length; i++)
            {
                for (int j = 0; j < invalid.Length; j++)
                {
                    if (chars[i] != invalid[j]) continue;
                    chars[i] = '_';
                    break;
                }
            }

            return new string(chars);
        }

        static bool IsFinite(float value)
        {
            return !float.IsNaN(value) && !float.IsInfinity(value);
        }
    }
}
