#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using System;
using System.IO;
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace LegendsOfRus.Tests.AndroidBuild
{
    public sealed class AndroidBuildFix03EditModeTests
    {
        [Test]
        public void EarlyGuardPrecedesFinalGuards()
        {
            Assert.AreEqual(int.MinValue, new AndroidBuildEarlyPreflight().callbackOrder);
            Assert.AreEqual(int.MaxValue, new AndroidBuildPlayerOptionsGuard().callbackOrder);
            Assert.AreEqual(int.MaxValue, new AndroidBuildGuard().callbackOrder);
        }

        [Test]
        public void EditModePassesStateGate()
        {
            Assert.DoesNotThrow(() => AndroidBuildEarlyPreflight.ValidateEditorState(false, false));
        }

        [TestCase(true, true)]
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void PlayOrPendingPlayIsBlocked(bool playing, bool pending)
        {
            var exception = Assert.Throws<BuildFailedException>(
                () => AndroidBuildEarlyPreflight.ValidateEditorState(playing, pending));
            StringAssert.Contains("ANDROID_EDITOR_PLAY_MODE_ACTIVE", exception.Message);
        }

        [TestCase(BuildOptions.AutoRunPlayer)]
        [TestCase(BuildOptions.AllowDebugging)]
        [TestCase(BuildOptions.ConnectWithProfiler)]
        [TestCase(BuildOptions.IncludeTestAssemblies)]
        public void ExtraFlagsRemainRejectedAndAreExplained(BuildOptions extra)
        {
            var options = new BuildPlayerOptions
            {
                target = BuildTarget.Android, targetGroup = BuildTargetGroup.Android,
                subtarget = 0, options = BuildOptions.Development | extra
            };
            var exception = Assert.Throws<BuildFailedException>(
                () => AndroidBuildPlayerOptionsGuard.ValidateOptions(options, AndroidBuildTestFactory.Policy()));
            StringAssert.Contains("ANDROID_BUILD_OPTIONS_UNSAFE", exception.Message);
            StringAssert.Contains(AndroidDevelopmentBuildOptions.FormatActive(extra), exception.Message);
        }

        [Test]
        public void OptionsWithoutDevelopmentRemainRejected()
        {
            var options = new BuildPlayerOptions
            {
                target = BuildTarget.Android, targetGroup = BuildTargetGroup.Android,
                subtarget = 0, options = BuildOptions.None
            };
            var exception = Assert.Throws<BuildFailedException>(
                () => AndroidBuildPlayerOptionsGuard.ValidateOptions(options, AndroidBuildTestFactory.Policy()));
            StringAssert.Contains("Development=False", exception.Message);
        }

        [Test]
        public void MissingLockApprovalIsNotSilentlyChanged()
        {
            var facts = AndroidBuildTestFactory.AllowedFacts();
            facts.PackageLockApproved = false;
            var result = AndroidBuildPolicyEvaluator.Evaluate(AndroidBuildTestFactory.Policy(), facts);
            Assert.IsFalse(result.Allowed);
            Assert.AreEqual("ANDROID_PACKAGE_LOCK_UNAPPROVED", result.ReasonCode);
        }
    }

    // Fixtures copy the reviewed package inputs into a unique temporary directory.
    // No test modifies the workspace's actual manifest or package lock.
    public sealed class AndroidResolvedPackageBaselineEditModeTests
    {
        private string fixtureRoot;

        [SetUp]
        public void CreateIsolatedPackageFixture()
        {
            fixtureRoot = Path.Combine(Path.GetTempPath(),
                "LegendsOfRus-PackageBaseline-" + Guid.NewGuid().ToString("N"));
            string packages = Path.Combine(fixtureRoot, "Packages");
            Directory.CreateDirectory(packages);
            string projectRoot = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));
            foreach (string fileName in new[] { "manifest.json", "packages-lock.json" })
                File.Copy(Path.Combine(projectRoot, "Packages", fileName),
                    Path.Combine(packages, fileName));
        }

        [TearDown]
        public void RemoveIsolatedPackageFixture()
        {
            if (!string.IsNullOrEmpty(fixtureRoot) && Directory.Exists(fixtureRoot))
                Directory.Delete(fixtureRoot, true);
        }

        [Test]
        public void ExactReviewedManifestAndLockAreAccepted()
        {
            Assert.IsTrue(AndroidBuildStateReader.ResolvedPackageBaselineIsExact(fixtureRoot));
        }

        [TestCase("manifest.json")]
        [TestCase("packages-lock.json")]
        public void MissingReviewedInputIsDenied(string fileName)
        {
            File.Delete(Path.Combine(fixtureRoot, "Packages", fileName));
            Assert.IsFalse(AndroidBuildStateReader.ResolvedPackageBaselineIsExact(fixtureRoot));
        }

        [TestCase("manifest.json")]
        [TestCase("packages-lock.json")]
        public void ChangedReviewedInputIsDenied(string fileName)
        {
            // Whitespace preserves valid JSON while changing the exact reviewed byte identity.
            File.AppendAllText(Path.Combine(fixtureRoot, "Packages", fileName), " ");
            Assert.IsFalse(AndroidBuildStateReader.ResolvedPackageBaselineIsExact(fixtureRoot));
        }

        [Test]
        public void DirectoryInsteadOfLockIsDenied()
        {
            string lockPath = Path.Combine(fixtureRoot, "Packages", "packages-lock.json");
            File.Delete(lockPath);
            Directory.CreateDirectory(lockPath);
            Assert.IsFalse(AndroidBuildStateReader.ResolvedPackageBaselineIsExact(fixtureRoot));
        }

        [Test]
        public void InvalidRootFailsClosedWithoutThrowing()
        {
            Assert.DoesNotThrow(() => Assert.IsFalse(
                AndroidBuildStateReader.ResolvedPackageBaselineIsExact(null)));
        }

        [Test]
        public void PackageFingerprintMatchDoesNotApproveReleaseOrSigning()
        {
            var policy = AndroidBuildTestFactory.Policy();
            var facts = AndroidBuildTestFactory.AllowedFacts();
            facts.PackageLockApproved = AndroidBuildStateReader.ResolvedPackageBaselineIsExact(fixtureRoot);
            Assert.IsTrue(facts.PackageLockApproved);
            facts.CustomSigning = true;
            Assert.AreEqual("ANDROID_CUSTOM_SIGNING_BLOCKED",
                AndroidBuildPolicyEvaluator.Evaluate(policy, facts).ReasonCode);
            facts.CustomSigning = false;
            facts.DevelopmentBuild = false;
            Assert.IsFalse(AndroidBuildPolicyEvaluator.Evaluate(policy, facts).Allowed);
        }
    }
}
#endif
