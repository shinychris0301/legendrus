#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LegendsOfRus.OptimizationCore;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step17MemoryTextureLoadingOptimizationSelfTest
    {
        [MenuItem("Legends of Rus/Tests/Run v1.17 Step 4 Self-Tests")]
        public static void Run()
        {
            int passed = 0;
            int total = 0;
            var failures = new List<string>();

            Check("optimization policy constants are pinned", PolicyIsPinned,
                ref passed, ref total, failures);
            Check("memory budget boundaries are deterministic", MemoryBoundaries,
                ref passed, ref total, failures);
            Check("loading budget boundaries are deterministic", LoadingBoundaries,
                ref passed, ref total, failures);
            Check("evidence buffer overwrites oldest and stays bounded", BufferIsBounded,
                ref passed, ref total, failures);
            Check("evidence JSON is deterministic and data-minimized", EvidenceJsonIsMinimized,
                ref passed, ref total, failures);
            Check("loading state cannot regress", LoadingStateCannotRegress,
                ref passed, ref total, failures);
            Check("runtime texture import audit passes", RuntimeTextureAuditPasses,
                ref passed, ref total, failures);
            Check("locked importer explicitly disables Read/Write and mipmaps", ImporterIsMemorySafe,
                ref passed, ref total, failures);
            Check("runtime service handles low-memory locally", LowMemoryHandlingIsLocal,
                ref passed, ref total, failures);
            Check("resource cleanup is bounded and uses UnloadUnusedAssets", ResourceCleanupIsBounded,
                ref passed, ref total, failures);
            Check("screen router reports route lifetime without gameplay cleanup", ScreenRouterWiringIsSafe,
                ref passed, ref total, failures);
            Check("runtime bootstrap installs optimization service", RuntimeBootstrapWiringExists,
                ref passed, ref total, failures);
            Check("Addressables package may be present but remote delivery remains inactive", AddressablesRemainDeferred,
                ref passed, ref total, failures);
            Check("commercial and economy activation remain disabled", CommercialGatesRemainDisabled,
                ref passed, ref total, failures);
            Check("approved PNG source inventory remains complete", ApprovedPngInventoryExists,
                ref passed, ref total, failures);
            Check("Unity version boundary remains exactly 6000.5.9f1", UnityVersionBoundaryIsExact,
                ref passed, ref total, failures);

            if (failures.Count == 0)
            {
                string message =
                    "v1.17 Step 4 memory/texture/loading foundation self-tests " +
                    "passed: " + passed + "/" + total + ".\n\n" +
                    "This validates source/runtime wiring and imported texture " +
                    "policy only. It does not claim representative-device memory, " +
                    "load-time, Android build, Addressables, provider, commercial, " +
                    "economy, or publishing evidence.";
                Debug.Log(message);
                if (!Application.isBatchMode)
                    EditorUtility.DisplayDialog("Legends of Rus — Tests", message, "OK");
                return;
            }

            string summary = string.Join("\n", failures.ToArray());
            Debug.LogError(
                "v1.17 Step 4 self-tests failed: " + passed + "/" + total +
                " passed.\n" + summary
            );
            if (!Application.isBatchMode)
                EditorUtility.DisplayDialog(
                    "Legends of Rus — Test failures",
                    summary,
                    "OK"
                );
            throw new InvalidOperationException("v1.17 Step 4 self-tests failed.");
        }

        static bool PolicyIsPinned()
        {
            return OptimizationPolicy.IsValid &&
                OptimizationPolicy.SchemaVersion == 1 &&
                OptimizationPolicy.AllocatedWarningBytes == 384L * 1024L * 1024L &&
                OptimizationPolicy.AllocatedCriticalBytes == 512L * 1024L * 1024L &&
                OptimizationPolicy.ReservedWarningBytes == 640L * 1024L * 1024L &&
                OptimizationPolicy.ReservedCriticalBytes == 768L * 1024L * 1024L &&
                OptimizationPolicy.ManagedWarningBytes == 96L * 1024L * 1024L &&
                OptimizationPolicy.ManagedCriticalBytes == 128L * 1024L * 1024L &&
                OptimizationPolicy.MaximumSteadyGrowthBytes == 32L * 1024L * 1024L &&
                OptimizationPolicy.ScreenRouteBudgetMilliseconds == 250.0 &&
                OptimizationPolicy.ResourceCleanupBudgetMilliseconds == 750.0 &&
                OptimizationPolicy.BootstrapBudgetMilliseconds == 5000.0 &&
                OptimizationPolicy.DefaultEvidenceCapacity == 64 &&
                OptimizationPolicy.MaximumEvidenceCapacity == 128 &&
                OptimizationPolicy.RetiredScreensBeforeCleanup == 3 &&
                OptimizationPolicy.MinimumCleanupIntervalSeconds == 10.0 &&
                OptimizationPolicy.MemorySampleIntervalSeconds == 5.0 &&
                OptimizationPolicy.RuntimeTextureMaximumDimension == 2048;
        }

        static bool MemoryBoundaries()
        {
            return
                MemoryBudgetEvaluator.Evaluate(0, 0, 0) ==
                    MemoryBudgetBand.WithinBudget &&
                MemoryBudgetEvaluator.Evaluate(
                    OptimizationPolicy.AllocatedWarningBytes - 1,
                    OptimizationPolicy.ReservedWarningBytes - 1,
                    OptimizationPolicy.ManagedWarningBytes - 1
                ) == MemoryBudgetBand.WithinBudget &&
                MemoryBudgetEvaluator.Evaluate(
                    OptimizationPolicy.AllocatedWarningBytes,
                    0,
                    0
                ) == MemoryBudgetBand.Warning &&
                MemoryBudgetEvaluator.Evaluate(
                    0,
                    OptimizationPolicy.ReservedCriticalBytes,
                    0
                ) == MemoryBudgetBand.Critical &&
                MemoryBudgetEvaluator.Evaluate(-1, 0, 0) ==
                    MemoryBudgetBand.Unknown &&
                MemoryBudgetEvaluator.IsSteadyGrowthWithinBudget(
                    OptimizationPolicy.MaximumSteadyGrowthBytes
                ) &&
                !MemoryBudgetEvaluator.IsSteadyGrowthWithinBudget(
                    OptimizationPolicy.MaximumSteadyGrowthBytes + 1
                );
        }

        static bool LoadingBoundaries()
        {
            return
                LoadingBudgetEvaluator.Evaluate(
                    OptimizationLoadKind.ScreenRoute,
                    OptimizationPolicy.ScreenRouteBudgetMilliseconds
                ) == OptimizationLoadVerdict.Pass &&
                LoadingBudgetEvaluator.Evaluate(
                    OptimizationLoadKind.ScreenRoute,
                    OptimizationPolicy.ScreenRouteBudgetMilliseconds + 0.001
                ) == OptimizationLoadVerdict.Fail &&
                LoadingBudgetEvaluator.Evaluate(
                    (OptimizationLoadKind)999,
                    1.0
                ) == OptimizationLoadVerdict.Incomplete &&
                LoadingBudgetEvaluator.Evaluate(
                    OptimizationLoadKind.Bootstrap,
                    -1.0
                ) == OptimizationLoadVerdict.Incomplete;
        }

        static bool BufferIsBounded()
        {
            var buffer = new BoundedOptimizationEvidenceBuffer(2);
            buffer.AddMemory(1, 2, 3);
            buffer.AddMemory(4, 5, 6);
            buffer.AddLoad(OptimizationLoadKind.ScreenRoute, 7.0);
            OptimizationEvidenceSample[] values = buffer.Snapshot();
            return buffer.Count == 2 && buffer.Capacity == 2 &&
                values.Length == 2 && values[0].Sequence == 2 &&
                values[1].Sequence == 3 &&
                values[1].Kind == OptimizationEvidenceKind.ScreenRoute;
        }

        static bool EvidenceJsonIsMinimized()
        {
            var first = new BoundedOptimizationEvidenceBuffer(2);
            var second = new BoundedOptimizationEvidenceBuffer(2);
            first.AddMemory(1, 2, 3);
            first.AddLoad(OptimizationLoadKind.ScreenRoute, 4.25);
            second.AddMemory(1, 2, 3);
            second.AddLoad(OptimizationLoadKind.ScreenRoute, 4.25);
            string a = first.ToDeterministicJson();
            string b = second.ToDeterministicJson();
            return a == b &&
                a.Contains("\"schemaVersion\":1") &&
                a.Contains("\"durationMilliseconds\":4.25") &&
                !a.Contains("player") && !a.Contains("device") &&
                !a.Contains("message") && !a.Contains("stack") &&
                !a.Contains("payload") && !a.Contains("timestamp") &&
                !a.Contains("receipt") && !a.Contains("token");
        }

        static bool LoadingStateCannotRegress()
        {
            var state = new LoadingStateTracker();
            return state.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.15) &&
                state.TryAdvance(RuntimeLoadingPhase.LocalState, 0.55) &&
                !state.TryAdvance(RuntimeLoadingPhase.Bootstrap, 0.60) &&
                !state.TryAdvance(RuntimeLoadingPhase.UiConstruction, 0.40) &&
                !state.TryAdvance(RuntimeLoadingPhase.Ready, 0.99) &&
                state.TryAdvance(RuntimeLoadingPhase.UiConstruction, 0.85) &&
                state.TryAdvance(RuntimeLoadingPhase.Ready, 1.0) &&
                state.Phase == RuntimeLoadingPhase.Ready &&
                state.Progress01 == 1.0;
        }

        static bool RuntimeTextureAuditPasses()
        {
            TextureOptimizationAuditResult result = TextureOptimizationAudit.Collect();
            return result.Passed && result.RuntimeTextureCount >= 40 &&
                result.EstimatedRgbaBytes > 0L;
        }

        static bool ImporterIsMemorySafe()
        {
            string source = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs"
            );
            return source.Contains("ti.isReadable = false;") &&
                source.Contains("ti.mipmapEnabled = false;") &&
                source.Contains("TextureImporterFormat.ASTC_6x6") &&
                source.Contains("android.maxTextureSize = 2048;");
        }

        static bool LowMemoryHandlingIsLocal()
        {
            string source = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs"
            );
            return source.Contains("Application.lowMemory += HandleLowMemory;") &&
                source.Contains("Application.lowMemory -= HandleLowMemory;") &&
                source.Contains("RequestCleanup(true);") &&
                !source.Contains("UnityWebRequest") &&
                !source.Contains("HttpClient") &&
                !source.Contains("PlayerPrefs") &&
                !source.Contains("File.Write") &&
                !source.Contains("GC.Collect(");
        }

        static bool ResourceCleanupIsBounded()
        {
            string source = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs"
            );
            return source.Contains("Resources.UnloadUnusedAssets()") &&
                source.Contains("MinimumCleanupIntervalSeconds") &&
                source.Contains("RetiredScreensBeforeCleanup") &&
                source.Contains("if (!cleanupRunning)") &&
                source.Contains("yield return null;");
        }

        static bool ScreenRouterWiringIsSafe()
        {
            string source = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs"
            );
            return source.Contains("optimization?.BeginScreenRoute();") &&
                source.Contains("optimization?.CompleteScreenRoute(") &&
                source.Contains("id != ScreenId.Gameplay && id != ScreenId.Dialogue") &&
                source.Contains("NotifyScreenRetired();");
        }

        static bool RuntimeBootstrapWiringExists()
        {
            string source = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs"
            );
            string asmdef = ReadProjectFile(
                "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef"
            );
            return source.Contains("FindOrAdd<RuntimeOptimizationService>") &&
                source.Contains("optimization.BeginBootstrap();") &&
                source.Contains("optimization.CompleteBootstrap();") &&
                asmdef.Contains("LegendsOfRus.OptimizationCore");
        }

        static bool AddressablesRemainDeferred()
        {
            string manifest = ReadProjectFile("Packages/manifest.json");
            string activation = ReadProjectFile(
                "Assets/LegendsOfRus/Resources/Config/content_delivery_activation.json"
            );
            return manifest.Contains("\"com.unity.addressables\": \"2.8.1\"") &&
                activation.Contains("\"remoteCatalogActivationApproved\": false") &&
                activation.Contains("\"remoteContentActivationApproved\": false") &&
                activation.Contains("\"catalogUpdatesActivationApproved\": false") &&
                !ReadProjectFile(
                    "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs"
                ).Contains("Addressables");
        }

        static bool CommercialGatesRemainDisabled()
        {
            string economy = ReadProjectFile(
                "Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json"
            );
            string commercial = ReadProjectFile(
                "Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json"
            );
            return economy.Contains("\"runtimeActivationApproved\": false") &&
                commercial.Contains("\"analyticsActivationApproved\": false") &&
                commercial.Contains("\"iapActivationApproved\": false") &&
                commercial.Contains("\"rewardedAdsActivationApproved\": false") &&
                commercial.Contains("\"finalRuntimeActivationApproved\": false");
        }

        static bool ApprovedPngInventoryExists()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:Texture2D",
                new[] { "Assets/LegendsOfRus" }
            );
            int pngCount = guids.Select(AssetDatabase.GUIDToAssetPath)
                .Count(path => path.EndsWith(".png", StringComparison.OrdinalIgnoreCase));
            return pngCount == 65;
        }

        static bool UnityVersionBoundaryIsExact()
        {
            return string.Equals(
                Application.unityVersion,
                "6000.5.9f1",
                StringComparison.Ordinal
            );
        }

        static string ReadProjectFile(string relativePath)
        {
            return File.ReadAllText(
                Path.Combine(Directory.GetCurrentDirectory(), relativePath)
            );
        }

        static void Check(
            string label,
            Func<bool> test,
            ref int passed,
            ref int total,
            List<string> failures
        )
        {
            total++;
            try
            {
                if (test())
                {
                    passed++;
                    return;
                }
                failures.Add("FAIL: " + label);
            }
            catch (Exception exception)
            {
                failures.Add(
                    "FAIL: " + label + " (" + exception.GetType().Name +
                    ": " + exception.Message + ")"
                );
            }
        }
    }
}
#endif
