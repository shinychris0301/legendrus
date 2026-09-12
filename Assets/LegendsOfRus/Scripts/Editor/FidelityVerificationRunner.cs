#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Captures the currently visible route at the four locked portrait targets.
    /// This runner verifies capture dimensions, route stability, and responsive
    /// layout only. Pixel fidelity requires a separate approved-design comparison.
    /// </summary>
    public static class FidelityVerificationRunner
    {
        const string RunMenu =
            "Legends of Rus/QA/Run v1.14 Step 4 Capture/Layout Matrix (Play Mode)";
        const string CancelMenu =
            "Legends of Rus/QA/Cancel v1.14 Step 4 Capture/Layout Matrix";
        const double ResolutionTimeoutSeconds = 8d;
        const double CaptureTimeoutSeconds = 15d;

        enum Stage
        {
            Idle,
            SetTarget,
            WaitForResolution,
            WaitForLayout,
            RequestCapture,
            WaitForCapture,
            RestoreResolution,
            WaitForRestore,
            RestoreRoute,
        }

        static readonly List<string> Errors = new List<string>();

        static Stage stage;
        static QACaptureService captureService;
        static ScreenRouter router;
        static FidelityCaptureResult completedCapture;
        static FidelityTarget target;
        static ScreenId originalScreen;
        static ScreenId? originalModal;
        static int originalRouteGeneration;
        static int originalWidth;
        static int originalHeight;
        static FullScreenMode originalMode;
        static int targetIndex;
        static int settleUntilFrame;
        static double deadline;
        static string outputFolder;
        static bool running;

        [MenuItem(RunMenu)]
        public static void Run()
        {
            if (running)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus Capture/Layout QA",
                    "A capture/layout matrix is already running.",
                    "OK"
                );
                return;
            }

            if (!EditorApplication.isPlaying || EditorApplication.isPaused)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus Capture/Layout QA",
                    "Enter Play Mode, leave the game unpaused, navigate to the screen " +
                    "you want to verify, then run this command again.",
                    "OK"
                );
                return;
            }

            router = ScreenRouter.Instance;
            captureService = UnityEngine.Object.FindAnyObjectByType<QACaptureService>();
            if (router == null || captureService == null)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus Capture/Layout QA",
                    "The runtime ScreenRouter or QACaptureService is not available.",
                    "OK"
                );
                return;
            }

            if (captureService.IsCapturePending)
            {
                EditorUtility.DisplayDialog(
                    "Legends of Rus Capture/Layout QA",
                    "Wait for the active QA capture to finish, then run the matrix again.",
                    "OK"
                );
                return;
            }

            originalScreen = router.CurrentScreen;
            originalModal = router.ActiveModal;
            originalRouteGeneration = router.RouteGeneration;
            originalWidth = Screen.width;
            originalHeight = Screen.height;
            originalMode = Screen.fullScreenMode;
            targetIndex = 0;
            Errors.Clear();
            string visible = originalModal.HasValue
                ? originalScreen + "_Modal_" + originalModal.Value
                : originalScreen.ToString();
            outputFolder = Path.Combine(
                Application.persistentDataPath,
                "DesignQACaptures",
                "v1_14_step04",
                FidelityVerification.SanitizeFileToken(visible, "Unknown"),
                DateTime.UtcNow.ToString("yyyyMMdd_HHmmss")
            );

            running = true;
            stage = Stage.SetTarget;
            captureService.CaptureCompleted += OnCaptureCompleted;
            EditorApplication.update += Tick;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Debug.Log(
                $"v1.14 Step 4 capture/layout matrix started for {visible}. " +
                $"Output: {outputFolder}"
            );
        }

        [MenuItem(RunMenu, true)]
        static bool ValidateRun()
        {
            return EditorApplication.isPlaying && !EditorApplication.isPaused && !running;
        }

        [MenuItem(CancelMenu)]
        static void Cancel()
        {
            if (!running) return;
            Errors.Add("The matrix was cancelled by the user.");
            BeginRestore();
        }

        [MenuItem(CancelMenu, true)]
        static bool ValidateCancel()
        {
            return running;
        }

        static void Tick()
        {
            if (!running) return;
            if (!EditorApplication.isPlaying)
            {
                StopWithoutRestore(
                    "Play Mode ended before the capture/layout matrix completed."
                );
                return;
            }

            try
            {
                switch (stage)
                {
                    case Stage.SetTarget:
                        SetNextTarget();
                        break;
                    case Stage.WaitForResolution:
                        WaitForTargetResolution();
                        break;
                    case Stage.WaitForLayout:
                        WaitForStableLayout();
                        break;
                    case Stage.RequestCapture:
                        RequestCapture();
                        break;
                    case Stage.WaitForCapture:
                        WaitForCapture();
                        break;
                    case Stage.RestoreResolution:
                        RestoreResolution();
                        break;
                    case Stage.WaitForRestore:
                        WaitForRestoredResolution();
                        break;
                    case Stage.RestoreRoute:
                        RestoreRouteAndFinish();
                        break;
                }
            }
            catch (Exception exception)
            {
                Errors.Add("Runner exception: " + exception.Message);
                Debug.LogException(exception);
                BeginRestore();
            }
        }

        static void SetNextTarget()
        {
            if (targetIndex >= FidelityVerification.TargetCount)
            {
                BeginRestore();
                return;
            }

            target = FidelityVerification.GetTarget(targetIndex);
            completedCapture = null;
            Screen.SetResolution(target.width, target.height, FullScreenMode.Windowed);
            deadline = EditorApplication.timeSinceStartup + ResolutionTimeoutSeconds;
            stage = Stage.WaitForResolution;
        }

        static void WaitForTargetResolution()
        {
            if (Screen.width == target.width && Screen.height == target.height)
            {
                ForceResponsiveLayout();
                settleUntilFrame = Time.frameCount + 2;
                deadline = EditorApplication.timeSinceStartup + ResolutionTimeoutSeconds;
                stage = Stage.WaitForLayout;
                return;
            }

            if (EditorApplication.timeSinceStartup <= deadline) return;
            Errors.Add(
                $"{target.label}: Unity reported {Screen.width}x{Screen.height} " +
                $"after requesting {target.width}x{target.height}."
            );
            BeginRestore();
        }

        static void WaitForStableLayout()
        {
            if (!RouteStillMatches())
            {
                Errors.Add(
                    $"The visible route or route generation changed from " +
                    $"{VisibleRouteLabel()} during capture."
                );
                BeginRestore();
                return;
            }

            ForceResponsiveLayout();
            if (
                Time.frameCount >= settleUntilFrame &&
                !router.IsScreenTransitioning &&
                !router.IsModalTransitioning
            )
            {
                stage = Stage.RequestCapture;
                return;
            }

            if (EditorApplication.timeSinceStartup <= deadline) return;
            Errors.Add($"{target.label}: UI layout or transition did not settle in time.");
            BeginRestore();
        }

        static void RequestCapture()
        {
            string modal = originalModal.HasValue
                ? "_Modal_" + originalModal.Value
                : string.Empty;
            string captureId = FidelityVerification.SanitizeFileToken(
                $"{originalScreen}{modal}_{target.id}_{target.width}x{target.height}",
                $"capture_{targetIndex}"
            );
            var request = new FidelityCaptureRequest
            {
                captureId = captureId,
                targetId = target.id,
                targetLabel = target.label,
                outputDirectory = outputFolder,
                requestedWidth = target.width,
                requestedHeight = target.height,
                showToast = false,
            };

            if (!captureService.TryRequestCapture(request))
            {
                Errors.Add($"{target.label}: QACaptureService rejected the request.");
                BeginRestore();
                return;
            }

            deadline = EditorApplication.timeSinceStartup + CaptureTimeoutSeconds;
            stage = Stage.WaitForCapture;
        }

        static void WaitForCapture()
        {
            if (!RouteStillMatches())
            {
                Errors.Add(
                    $"The visible route or route generation changed from " +
                    $"{VisibleRouteLabel()} during capture."
                );
                BeginRestore();
                return;
            }

            if (completedCapture != null)
            {
                ValidateCapture(completedCapture);
                targetIndex++;
                stage = Stage.SetTarget;
                return;
            }

            if (EditorApplication.timeSinceStartup <= deadline) return;
            Errors.Add($"{target.label}: end-of-frame capture timed out.");
            BeginRestore();
        }

        static void ValidateCapture(FidelityCaptureResult result)
        {
            if (!result.Success || result.Metadata == null)
            {
                Errors.Add($"{target.label}: capture failed: {result.Error}");
                return;
            }

            FidelityCaptureMetadata metadata = result.Metadata;
            if (
                metadata.actualWidth != target.width ||
                metadata.actualHeight != target.height
            )
            {
                Errors.Add(
                    $"{target.label}: capture dimensions were " +
                    $"{metadata.actualWidth}x{metadata.actualHeight}."
                );
            }
            if (!metadata.layout.passed)
            {
                Errors.Add(
                    $"{target.label}: responsive layout failed: " +
                    metadata.layout.failure
                );
            }
            if (metadata.screenTransitioning || metadata.modalTransitioning)
            {
                Errors.Add(
                    $"{target.label}: capture occurred while a screen or modal " +
                    "transition was still active."
                );
            }
            if (!string.Equals(metadata.screenId, originalScreen.ToString(), StringComparison.Ordinal))
            {
                Errors.Add(
                    $"{target.label}: captured {metadata.screenId}, expected {originalScreen}."
                );
            }

            string expectedModal = originalModal.HasValue
                ? originalModal.Value.ToString()
                : string.Empty;
            if (!string.Equals(metadata.modalId, expectedModal, StringComparison.Ordinal))
            {
                Errors.Add(
                    $"{target.label}: captured modal '{metadata.modalId}', " +
                    $"expected '{expectedModal}'."
                );
            }
            if (metadata.routeGeneration != originalRouteGeneration)
            {
                Errors.Add(
                    $"{target.label}: captured route generation " +
                    $"{metadata.routeGeneration}, expected {originalRouteGeneration}."
                );
            }
        }

        static void BeginRestore()
        {
            if (!running) return;
            if (captureService != null && captureService.IsCapturePending)
            {
                captureService.CancelPendingCapture();
                completedCapture = null;
            }
            stage = Stage.RestoreResolution;
        }

        static void RestoreResolution()
        {
            Screen.SetResolution(originalWidth, originalHeight, originalMode);
            deadline = EditorApplication.timeSinceStartup + ResolutionTimeoutSeconds;
            stage = Stage.WaitForRestore;
        }

        static void WaitForRestoredResolution()
        {
            if (Screen.width == originalWidth && Screen.height == originalHeight)
            {
                ForceResponsiveLayout();
                stage = Stage.RestoreRoute;
                return;
            }

            if (EditorApplication.timeSinceStartup <= deadline) return;
            Errors.Add(
                $"Could not restore the original {originalWidth}x{originalHeight} resolution; " +
                $"Unity reports {Screen.width}x{Screen.height}."
            );
            stage = Stage.RestoreRoute;
        }

        static void RestoreRouteAndFinish()
        {
            if (router != null && router.CurrentScreen != originalScreen)
            {
                router.Show(originalScreen);
                if (originalModal.HasValue)
                {
                    Errors.Add(
                        "The original modal could not be reconstructed after an external route " +
                        "change; the underlying screen was restored."
                    );
                }
            }

            Finish();
        }

        static bool RouteStillMatches()
        {
            return router != null &&
                router.CurrentScreen == originalScreen &&
                router.ActiveModal == originalModal &&
                router.RouteGeneration == originalRouteGeneration;
        }

        static string VisibleRouteLabel()
        {
            return originalModal.HasValue
                ? originalScreen + " + " + originalModal.Value
                : originalScreen.ToString();
        }

        static void ForceResponsiveLayout()
        {
            if (router == null) return;
            AdaptiveCanvasScaler adaptive = router.GameCanvas != null
                ? router.GameCanvas.GetComponent<AdaptiveCanvasScaler>()
                : null;
            SafeAreaFitter fitter = router.SafeArea != null
                ? router.SafeArea.GetComponent<SafeAreaFitter>()
                : null;
            adaptive?.ApplyNow();
            fitter?.ApplyNow();
            Canvas.ForceUpdateCanvases();
        }

        static void OnCaptureCompleted(FidelityCaptureResult result)
        {
            if (!running || stage != Stage.WaitForCapture) return;
            completedCapture = result;
        }

        static void OnPlayModeStateChanged(PlayModeStateChange change)
        {
            if (!running) return;
            if (
                change == PlayModeStateChange.ExitingPlayMode ||
                change == PlayModeStateChange.EnteredEditMode
            )
                StopWithoutRestore(
                    "Play Mode ended before the capture/layout matrix completed."
                );
        }

        static void Finish()
        {
            bool passed = Errors.Count == 0 &&
                targetIndex == FidelityVerification.TargetCount;
            string summary = passed
                ? $"v1.14 Step 4 capture/layout matrix PASS: " +
                    $"{FidelityVerification.TargetCount} captures saved to {outputFolder}. " +
                    "Pixel fidelity was not evaluated."
                : $"v1.14 Step 4 capture/layout matrix FAIL ({Errors.Count} issue(s)). " +
                    string.Join(" | ", Errors) + $" Output: {outputFolder}";
            if (passed) Debug.Log(summary);
            else Debug.LogError(summary);
            Cleanup();
        }

        static void StopWithoutRestore(string reason)
        {
            Errors.Add(reason);
            if (captureService != null)
                captureService.CancelPendingCapture();
            Debug.LogError(reason);
            Cleanup();
        }

        static void Cleanup()
        {
            if (captureService != null)
                captureService.CaptureCompleted -= OnCaptureCompleted;
            EditorApplication.update -= Tick;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            captureService = null;
            router = null;
            completedCapture = null;
            stage = Stage.Idle;
            running = false;
        }
    }
}
#endif
