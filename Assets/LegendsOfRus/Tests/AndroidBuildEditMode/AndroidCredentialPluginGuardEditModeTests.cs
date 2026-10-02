#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.IO;
using System.Reflection;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEngine;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidCredentialPluginGuardEditModeTests
    {
        const string PluginDirectory = "Assets/LegendsOfRus/Plugins/AndroidCredentials/";
        static readonly string[] Inventory = {
            "CredentialVault.java", "CredentialVault.java.meta",
            "AndroidCredentialBridge.java", "AndroidCredentialBridge.java.meta"
        };
        string root;

        [SetUp]
        public void SetUp()
        {
            root = Path.Combine(Path.GetTempPath(), "lor-credential-guard-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(root, "Packages"));
            Directory.CreateDirectory(Path.Combine(root, PluginDirectory));
            string project = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (string name in Inventory)
                File.Copy(Path.Combine(project, PluginDirectory + name), Path.Combine(root, PluginDirectory + name));
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(root)) Directory.Delete(root, true);
        }

        bool ArtifactGate()
        {
            var method = typeof(AndroidBuildStateReader).GetMethod("NoForbiddenAndroidArtifacts",
                BindingFlags.Static | BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            return (bool)method.Invoke(null, new object[] { root });
        }

        [Test]
        public void ReviewedPair_PassesArtifactGate_WhileJavaRemainsForbiddenByDefault()
        {
            Assert.IsTrue(AndroidBuildStateReader.IsForbiddenAndroidArtifactExtension(".java"));
            Assert.IsTrue(ArtifactGate());
        }

        [TestCase("CredentialVault.java")]
        [TestCase("CredentialVault.java.meta")]
        [TestCase("AndroidCredentialBridge.java")]
        [TestCase("AndroidCredentialBridge.java.meta")]
        public void ChangedInventory_BlocksArtifactGate(string name)
        {
            File.AppendAllText(Path.Combine(root, PluginDirectory + name), "\n// changed\n");
            Assert.IsFalse(ArtifactGate());
        }

        [TestCase("CredentialVault.java")]
        [TestCase("CredentialVault.java.meta")]
        [TestCase("AndroidCredentialBridge.java")]
        [TestCase("AndroidCredentialBridge.java.meta")]
        public void MissingInventory_BlocksRemainingPlugin(string name)
        {
            File.Delete(Path.Combine(root, PluginDirectory + name));
            Assert.IsFalse(ArtifactGate());
        }

        [TestCase("Other.java")]
        [TestCase("Other.kt")]
        [TestCase("Other.jar")]
        [TestCase("Other.aar")]
        [TestCase("Other.so")]
        [TestCase("AndroidManifest.xml")]
        [TestCase("mainTemplate.gradle")]
        public void AdditionalArtifact_StillBlocks(string name)
        {
            File.WriteAllText(Path.Combine(root, PluginDirectory + name), "synthetic guard fixture");
            Assert.IsFalse(ArtifactGate());
        }

        [Test]
        public void IdenticalSourceOutsideReviewedPath_StillBlocks()
        {
            File.Copy(Path.Combine(root, PluginDirectory + "CredentialVault.java"), Path.Combine(root, "Assets/CredentialVault.java"));
            Assert.IsFalse(ArtifactGate());
        }

        [TestCase("Assets/Plugins/Android")]
        [TestCase("Assets/Other.androidlib")]
        [TestCase("Packages/embedded")]
        public void ForbiddenDirectory_StillBlocks(string directory)
        {
            Directory.CreateDirectory(Path.Combine(root, directory));
            Assert.IsFalse(ArtifactGate());
        }

        [Test]
        public void ImporterMetadata_IsNotItselfAnExemptPlugin()
        {
            Assert.IsFalse(ReviewedAndroidCredentialPlugins.IsAllowed(root,
                Path.Combine(root, PluginDirectory + "CredentialVault.java.meta")));
        }
    }
}
#endif
