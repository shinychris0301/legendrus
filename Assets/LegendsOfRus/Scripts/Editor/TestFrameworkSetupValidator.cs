#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using PackageManagerPackageInfo = UnityEditor.PackageManager.PackageInfo;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    public static class TestFrameworkSetupValidator
    {
        const string PackageName = "com.unity.test-framework";
        const string TestAssemblyPath =
            "Assets/LegendsOfRus/Tests/EditMode/" +
            "LegendsOfRus.GameplayCore.Tests.asmdef";

        [Serializable]
        sealed class AssemblyDefinitionData
        {
            public string[] references;
            public bool overrideReferences;
            public string[] precompiledReferences;
            public string[] optionalUnityReferences;
            public string[] includePlatforms;
        }

        [MenuItem("Legends of Rus/Tests/Validate Test Framework Setup")]
        public static void Validate()
        {
            var package = PackageManagerPackageInfo.FindForPackageName(PackageName);
            bool packageOk = package != null;
            bool asmdefExists = File.Exists(TestAssemblyPath);

            AssemblyDefinitionData data = null;
            if (asmdefExists)
            {
                try
                {
                    data = JsonUtility.FromJson<AssemblyDefinitionData>(
                        File.ReadAllText(TestAssemblyPath)
                    );
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }

            bool usesTestAssemblies = Contains(
                data?.optionalUnityReferences,
                "TestAssemblies"
            );
            bool repeatsEngineRunner = Contains(
                data?.references,
                "UnityEngine.TestRunner"
            );
            bool repeatsEditorRunner = Contains(
                data?.references,
                "UnityEditor.TestRunner"
            );
            bool repeatsNUnit = Contains(
                data?.precompiledReferences,
                "nunit.framework.dll"
            );
            bool editorOnly = Contains(
                data?.includePlatforms,
                "Editor"
            );

            bool asmdefOk =
                data != null &&
                usesTestAssemblies &&
                !repeatsEngineRunner &&
                !repeatsEditorRunner &&
                !repeatsNUnit &&
                !data.overrideReferences &&
                editorOnly;

            if (packageOk && asmdefOk)
            {
                Debug.Log(
                    "Legends of Rus Test Framework setup PASSED. " +
                    $"{PackageName} {package.version}; " +
                    "TestAssemblies supplies NUnit and TestRunner references once."
                );
                return;
            }

            string message =
                $"Test Framework package: " +
                $"{(packageOk ? package.version : "MISSING")}\n" +
                $"Test assembly file: " +
                $"{(asmdefExists ? "present" : "MISSING")}\n" +
                $"Test assembly configuration: " +
                $"{(asmdefOk ? "valid" : "INVALID")}\n\n" +
                "A test asmdef must use TestAssemblies OR explicit NUnit/" +
                "TestRunner references, never both. This project uses " +
                "TestAssemblies only.";

            Debug.LogError(message);
            EditorUtility.DisplayDialog(
                "Test Framework setup incomplete",
                message,
                "OK"
            );
        }

        static bool Contains(string[] values, string expected)
        {
            if (values == null) return false;

            foreach (string value in values)
            {
                if (string.Equals(
                    value,
                    expected,
                    StringComparison.Ordinal
                ))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
#endif
