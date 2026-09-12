#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using LegendsOfRus.AndroidBuild;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.Build;

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
}
#endif
