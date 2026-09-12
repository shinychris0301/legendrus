using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus
{
    /// <summary>
    /// End-of-frame QA capture service. ReadPixels keeps this path independent
    /// from Unity's optional screenshot module while preserving the rendered
    /// Screen Space Overlay canvas in the output.
    /// </summary>
    public sealed class QACaptureService : MonoBehaviour
    {
        bool capturePending;
        Coroutine captureCoroutine;
        int captureGeneration;

        public bool IsCapturePending => capturePending;
        public event Action<FidelityCaptureResult> CaptureCompleted;

        void Update()
        {
            if (!RuntimeInput.QaCapturePressedThisFrame()) return;
            CaptureQaScreenshot();
        }

        public void CaptureQaScreenshot()
        {
            ScreenRouter router = ScreenRouter.Instance;
            string screen = router != null ? router.CurrentScreen.ToString() : "Unknown";
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff");
            var request = new FidelityCaptureRequest
            {
                captureId = $"LegendsOfRus_{screen}_{timestamp}",
                targetId = "manual",
                targetLabel = "Manual",
                outputDirectory = Path.Combine(
                    Application.persistentDataPath,
                    "DesignQACaptures"
                ),
                requestedWidth = Screen.width,
                requestedHeight = Screen.height,
                showToast = true,
            };

            if (!TryRequestCapture(request))
                router?.Toast("QA-снимок уже выполняется.");
        }

        public bool TryRequestCapture(FidelityCaptureRequest request)
        {
            if (capturePending || request == null)
                return false;

            capturePending = true;
            int generation = ++captureGeneration;
            captureCoroutine = StartCoroutine(
                CaptureAtEndOfFrame(request, generation)
            );
            return true;
        }

        /// <summary>
        /// Cancels a request before it can publish or write evidence. The
        /// generation check also invalidates a WaitForEndOfFrame continuation
        /// that Unity has already queued.
        /// </summary>
        public bool CancelPendingCapture()
        {
            if (!capturePending)
                return false;

            captureGeneration++;
            if (captureCoroutine != null)
                StopCoroutine(captureCoroutine);
            captureCoroutine = null;
            capturePending = false;
            return true;
        }

        IEnumerator CaptureAtEndOfFrame(
            FidelityCaptureRequest request,
            int generation
        )
        {
            yield return new WaitForEndOfFrame();
            if (generation != captureGeneration)
                yield break;

            Texture2D texture = null;
            FidelityCaptureResult result;
            try
            {
                int width = Mathf.Max(1, Screen.width);
                int height = Mathf.Max(1, Screen.height);
                if (
                    (request.requestedWidth > 0 &&
                        request.requestedWidth != width) ||
                    (request.requestedHeight > 0 &&
                        request.requestedHeight != height)
                )
                {
                    throw new InvalidOperationException(
                        $"Capture resolution changed before end of frame: " +
                        $"requested {request.requestedWidth}x" +
                        $"{request.requestedHeight}, actual {width}x{height}."
                    );
                }
                texture = new Texture2D(
                    width,
                    height,
                    TextureFormat.RGB24,
                    false,
                    false
                );
                texture.ReadPixels(new Rect(0f, 0f, width, height), 0, 0, false);
                texture.Apply(false, false);

                byte[] png = ImageConversion.EncodeToPNG(texture);
                if (png == null || png.Length == 0)
                    throw new InvalidOperationException("Unity returned an empty PNG payload.");

                string folder = string.IsNullOrWhiteSpace(request.outputDirectory)
                    ? Path.Combine(Application.persistentDataPath, "DesignQACaptures")
                    : request.outputDirectory;
                Directory.CreateDirectory(folder);

                string fallback = $"LegendsOfRus_{DateTime.UtcNow:yyyyMMdd_HHmmss_fff}";
                string captureId = FidelityVerification.SanitizeFileToken(
                    request.captureId,
                    fallback
                );
                string pngPath = Path.Combine(folder, captureId + ".png");
                string metadataPath = Path.Combine(folder, captureId + ".json");

                FidelityCaptureMetadata metadata = BuildMetadata(
                    request,
                    captureId,
                    Path.GetFileName(pngPath),
                    width,
                    height
                );

                File.WriteAllBytes(pngPath, png);
                File.WriteAllText(metadataPath, JsonUtility.ToJson(metadata, true));

                result = new FidelityCaptureResult(
                    true,
                    pngPath,
                    metadataPath,
                    string.Empty,
                    metadata
                );
                Debug.Log(
                    $"QA capture saved: {pngPath} | " +
                    $"layout={(metadata.layout.passed ? "PASS" : "FAIL")}"
                );
                LogBoardReplay();

                if (request.showToast)
                    ScreenRouter.Instance?.Toast("QA-снимок и метаданные сохранены.");
            }
            catch (Exception exception)
            {
                Debug.LogError($"QA capture failed: {exception.Message}");
                result = new FidelityCaptureResult(
                    false,
                    string.Empty,
                    string.Empty,
                    exception.Message,
                    null
                );
                if (request.showToast)
                    ScreenRouter.Instance?.Toast("Не удалось сохранить QA-снимок.");
            }
            finally
            {
                if (texture != null)
                    Destroy(texture);
                if (generation == captureGeneration)
                {
                    captureCoroutine = null;
                    capturePending = false;
                }
            }

            if (generation == captureGeneration)
                PublishCompletion(result);
        }

        void OnDisable()
        {
            CancelPendingCapture();
        }

        FidelityCaptureMetadata BuildMetadata(
            FidelityCaptureRequest request,
            string captureId,
            string pngFileName,
            int width,
            int height
        )
        {
            ScreenRouter router = ScreenRouter.Instance;
            Rect safe = FidelityVerification.NormalizeSafeArea(
                Screen.safeArea,
                new Vector2Int(width, height)
            );
            Rect viewportRect = ResolveViewportScreenRect(router);
            CanvasScaler scaler = router != null && router.GameCanvas != null
                ? router.GameCanvas.GetComponent<CanvasScaler>()
                : null;

            return new FidelityCaptureMetadata
            {
                captureId = captureId,
                targetId = request.targetId ?? string.Empty,
                targetLabel = request.targetLabel ?? string.Empty,
                screenId = router != null ? router.CurrentScreen.ToString() : "Unknown",
                modalId = router != null && router.ActiveModal.HasValue
                    ? router.ActiveModal.Value.ToString()
                    : string.Empty,
                utcTimestamp = DateTime.UtcNow.ToString("O"),
                unityVersion = Application.unityVersion,
                platform = Application.platform.ToString(),
                deviceModel = SystemInfo.deviceModel,
                graphicsDevice = SystemInfo.graphicsDeviceName,
                colorSpace = QualitySettings.activeColorSpace.ToString(),
                pngFileName = pngFileName,
                requestedWidth = Mathf.Max(1, request.requestedWidth),
                requestedHeight = Mathf.Max(1, request.requestedHeight),
                actualWidth = width,
                actualHeight = height,
                safeArea = safe,
                viewportRect = viewportRect,
                canvasScaleFactor = scaler != null ? scaler.scaleFactor : 0f,
                canvasMatchWidthOrHeight = scaler != null
                    ? scaler.matchWidthOrHeight
                    : 0f,
                routeGeneration = router != null ? router.RouteGeneration : 0,
                frameCount = Time.frameCount,
                screenTransitioning = router != null && router.IsScreenTransitioning,
                modalTransitioning = router != null && router.IsModalTransitioning,
                layout = FidelityVerification.Evaluate(
                    safe,
                    new Vector2Int(width, height),
                    viewportRect
                ),
            };
        }

        static Rect ResolveViewportScreenRect(ScreenRouter router)
        {
            if (router == null || router.Viewport == null)
                return new Rect(0f, 0f, Screen.width, Screen.height);

            var corners = new Vector3[4];
            router.Viewport.GetWorldCorners(corners);
            Camera camera = router.GameCanvas != null &&
                router.GameCanvas.renderMode != RenderMode.ScreenSpaceOverlay
                    ? router.GameCanvas.worldCamera
                    : null;

            Vector2 first = RectTransformUtility.WorldToScreenPoint(camera, corners[0]);
            float xMin = first.x;
            float xMax = first.x;
            float yMin = first.y;
            float yMax = first.y;
            for (int i = 1; i < corners.Length; i++)
            {
                Vector2 point = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
                xMin = Mathf.Min(xMin, point.x);
                xMax = Mathf.Max(xMax, point.x);
                yMin = Mathf.Min(yMin, point.y);
                yMax = Mathf.Max(yMax, point.y);
            }

            return Rect.MinMaxRect(xMin, yMin, xMax, yMax);
        }

        static void LogBoardReplay()
        {
            var board = UnityEngine.Object.FindAnyObjectByType<Match3Board>();
            if (board == null) return;

            Debug.Log(
                $"QA board replay: seed={board.BoardSeed}, " +
                $"hash={board.BoardStateHash}, plan={board.LastResolutionPlanHash}, " +
                $"token={board.BoardReplayToken}"
            );
        }

        void PublishCompletion(FidelityCaptureResult result)
        {
            Action<FidelityCaptureResult> callbacks = CaptureCompleted;
            if (callbacks == null) return;

            Delegate[] listeners = callbacks.GetInvocationList();
            for (int i = 0; i < listeners.Length; i++)
            {
                try
                {
                    ((Action<FidelityCaptureResult>)listeners[i]).Invoke(result);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception, this);
                }
            }
        }
    }
}
