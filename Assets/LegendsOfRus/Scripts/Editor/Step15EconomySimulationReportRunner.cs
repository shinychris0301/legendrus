#if UNITY_EDITOR
using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step15EconomySimulationReportRunner
    {
        const string MenuPath =
            "Legends of Rus/Verification/Write v1.15 Step 1 Economy Report";
        const string AssetPath =
            "Assets/LegendsOfRus/Documentation/V1_15_STEP01_ECONOMY_SIMULATION_REPORT.json";

        [MenuItem(MenuPath)]
        public static void WriteReport()
        {
            try
            {
                EconomySimulationCoordinatorResult result =
                    EconomySimulationCoordinator.LoadAndRunProposal();
                string json = EconomySimulationCoordinator.ToDeterministicJson(result);
                string absolutePath = Path.Combine(
                    Application.dataPath,
                    "LegendsOfRus/Documentation/V1_15_STEP01_ECONOMY_SIMULATION_REPORT.json"
                );
                string directory = Path.GetDirectoryName(absolutePath);
                if (string.IsNullOrEmpty(directory))
                    throw new InvalidOperationException("Could not resolve the economy report directory.");

                Directory.CreateDirectory(directory);
                File.WriteAllText(absolutePath, json + "\n", new UTF8Encoding(false));
                AssetDatabase.ImportAsset(AssetPath, ImportAssetOptions.ForceUpdate);

                if (!result.LoadSucceeded)
                {
                    Debug.LogError(
                        "v1.15 Step 1 economy report records a proposal load failure: " +
                        result.ErrorCode + " — " + result.Error + " Report: " + AssetPath
                    );
                    return;
                }

                Debug.Log(
                    "Wrote non-authoritative v1.15 Step 1 economy simulation report: " +
                    AssetPath + ". Simulation verdict: " + result.Simulation.verdict +
                    "; prototype comparison verdict: " + result.PrototypeComparison.verdict + "."
                );
            }
            catch (Exception exception)
            {
                Debug.LogError(
                    "Failed to write v1.15 Step 1 economy report: " +
                    exception.GetType().Name + ": " + exception.Message
                );
            }
        }
    }
}
#endif
