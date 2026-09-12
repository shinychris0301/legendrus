using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class Step20FunctionalAcceptanceAssetReadinessGate
    {
        const string Milestone = "v1.20-step06-functional-acceptance-asset-readiness";

        [MenuItem("Legends of Rus/Tests/Run v1.20 Step 6 Functional Acceptance + Asset Readiness Gate")]
        public static void Run()
        {
            int total = 0;
            int passed = 0;
            var failures = new List<string>();

            Check(ref total, ref passed, failures, "milestone", Read("MILESTONE_VERSION.txt").Trim() == Milestone);
            Check(ref total, ref passed, failures, "Step 5 QA retained", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP05_FUNCTIONAL_QA_MATRIX.json")));
            Check(ref total, ref passed, failures, "Step 5 route acceptance retained", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json")));
            Check(ref total, ref passed, failures, "Step 6 functional gate", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP06_FUNCTIONAL_ACCEPTANCE_GATE.json")));
            Check(ref total, ref passed, failures, "Step 6 functional freeze", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP06_FUNCTIONAL_FREEZE_MANIFEST.json")));
            Check(ref total, ref passed, failures, "Step 6 page contracts", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP06_PAGE_ASSET_CONTRACTS.json")));
            Check(ref total, ref passed, failures, "Step 6 reconstruction waves", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP06_RECONSTRUCTION_WAVES.json")));
            Check(ref total, ref passed, failures, "Step 6 readiness", File.Exists(PathOf("Assets/LegendsOfRus/Documentation/V1_20_STEP06_PRODUCTION_ASSET_RECONSTRUCTION_READINESS.json")));

            string contracts = Read("Assets/LegendsOfRus/Documentation/V1_20_STEP06_PAGE_ASSET_CONTRACTS.json");
            Check(ref total, ref passed, failures, "36 page contracts", Count(contracts, "\"pageKey\"") == 36);
            Check(ref total, ref passed, failures, "no runtime screenshot composites", Count(contracts, "\"runtimeCompositeScreenshotAllowed\": false") == 36);
            Check(ref total, ref passed, failures, "preserve interactions on all pages", Count(contracts, "\"preserveExistingInteractionRolesAndCallbacks\": true") == 36);
            Check(ref total, ref passed, failures, "preserve safe-area on all pages", Count(contracts, "\"preserveSafeAreaAndAspectHandling\": true") == 36);

            string readiness = Read("Assets/LegendsOfRus/Documentation/V1_20_STEP06_PRODUCTION_ASSET_RECONSTRUCTION_READINESS.json");
            Check(ref total, ref passed, failures, "36 page readiness", readiness.Contains("\"pageCount\": 36"));
            Check(ref total, ref passed, failures, "10 direct references", readiness.Contains("\"direct36PageApprovedReferences\": 10"));
            Check(ref total, ref passed, failures, "10 production prefabs", readiness.Contains("\"existingProductionPagePrefabs\": 10"));
            Check(ref total, ref passed, failures, "6 component-covered pages", readiness.Contains("\"pagesWithReconstructedComponentCoverage\": 6"));
            Check(ref total, ref passed, failures, "runtime cutover blocked", readiness.Contains("\"runtimeVisualCutover\": \"NO_GO_UNTIL_EXECUTABLE_FUNCTIONAL_ACCEPTANCE_AND_PER_WAVE_VISUAL_ACCEPTANCE\""));

            string gate = Read("Assets/LegendsOfRus/Documentation/V1_20_STEP06_FUNCTIONAL_ACCEPTANCE_GATE.json");
            Check(ref total, ref passed, failures, "source acceptance pass", gate.Contains("\"sourceFunctionalAcceptance\": \"PASS\""));
            Check(ref total, ref passed, failures, "executable acceptance pending", gate.Contains("\"executableFunctionalAcceptance\": \"PENDING\""));
            Check(ref total, ref passed, failures, "authoring preparation go", gate.Contains("\"assetAuthoringPreparation\": \"GO\""));
            Check(ref total, ref passed, failures, "runtime cutover blocked by gate", gate.Contains("\"productionAssetReconstructionRuntimeCutover\": \"BLOCKED\""));

            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", "6b66f9d6ad08c15b087489685cd2056b9b479e3389c19ce2773094b958861a02");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs", "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenNavigationPolicy.cs", "faf54ad5bc62c45f76302c59a0570e276e99f417c17b4f16b25f108a4528ecfa");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", "Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs", "d8853d33a49a6d21d414fab1830b957d032d047ed242b259af47d749414b6163");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", "ecb26b04310415bf8c077e213b32dd625b43c0f119ffb19efba741695caaa60d");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs", "Assets/LegendsOfRus/Scripts/Runtime/UI/DemoFeatureState.cs", "655879000c576ec5bf317a85ce8743b1b06afa0aeba374c8c278fc5fa5bcf4e3");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json", "Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json", "8ac3fbc091131b7178b81590e05a8d1dc03a1991e82e95ccaf02b6b589138c55");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json", "Assets/LegendsOfRus/Documentation/V1_20_STEP05_ROUTE_ACCEPTANCE.json", "40df550a6e0542d9ee1246340b5647cacef413f52e3947e4d567b3d06ff50f6c");
            CheckHash(ref total, ref passed, failures, "freeze Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json", "Assets/LegendsOfRus/Documentation/V1_20_STEP04_PAGE_FUNCTION_ASSET_PLAN.json", "b6c1aea4f1ae56e791865fccde90af08a062a86fcbfa0efcb0904f10afa4359c");

            string gameState = Read("Assets/LegendsOfRus/Scripts/Runtime/Core/GameState.cs");
            Check(ref total, ref passed, failures, "save-v6 retained", gameState.Contains("public int saveVersion = 6;"));
            string activation = Read("Assets/LegendsOfRus/Resources/Config/functionality_first_demo_v120_step04.json");
            Check(ref total, ref passed, failures, "automatic demo economy grant disabled", activation.Contains("\"automaticEconomyGrantFromDemoEnabled\": false"));
            Check(ref total, ref passed, failures, "automatic production publish disabled", activation.Contains("\"automaticProductionPublishEnabled\": false"));
            Check(ref total, ref passed, failures, "no AAB bundled", Directory.GetFiles(Directory.GetCurrentDirectory(), "*.aab", SearchOption.AllDirectories).Length == 0);

            if (failures.Count > 0)
            {
                string summary = string.Join(Environment.NewLine, failures.ToArray());
                Debug.LogError("v1.20 Step 6 acceptance/readiness gate failed: " + passed + "/" + total + " passed." + Environment.NewLine + summary);
                throw new InvalidOperationException("v1.20 Step 6 acceptance/readiness gate failed." + Environment.NewLine + summary);
            }

            Debug.Log("v1.20 Step 6 source acceptance/readiness gate passed: " + passed + "/" + total + ". This confirms source freeze/readiness only. Complete Unity suites and Android Back verification are still required before production-art runtime cutover.");
        }

        static void CheckHash(ref int total, ref int passed, List<string> failures, string name, string relativePath, string expected)
        {
            string p = PathOf(relativePath);
            bool ok = File.Exists(p) && Sha256(p) == expected;
            Check(ref total, ref passed, failures, name, ok);
        }

        static string Sha256(string path)
        {
            using (var sha = SHA256.Create())
            using (var stream = File.OpenRead(path))
            {
                byte[] hash = sha.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLowerInvariant();
            }
        }

        static void Check(ref int total, ref int passed, List<string> failures, string name, bool ok)
        {
            total++;
            if (ok) passed++;
            else failures.Add("FAIL: " + name);
        }

        static int Count(string value, string token)
        {
            int count = 0;
            int index = 0;
            while ((index = value.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }
            return count;
        }

        static string Read(string relativePath)
        {
            return File.ReadAllText(PathOf(relativePath));
        }

        static string PathOf(string relativePath)
        {
            return System.IO.Path.Combine(Directory.GetCurrentDirectory(), relativePath.Replace('/', System.IO.Path.DirectorySeparatorChar));
        }
    }
}
