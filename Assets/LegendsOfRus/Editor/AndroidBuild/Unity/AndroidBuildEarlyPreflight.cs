#if UNITY_EDITOR && UNITY_6000_0_OR_NEWER
using UnityEditor;
using UnityEditor.Build;

namespace LegendsOfRus.AndroidBuild
{
    /// <summary>
    /// Rejects ineligible Android Player builds before content-building processors.
    /// Does not build, stop/start Play Mode, repair scenes, or alter any build permission.
    /// The original maximum-order guards remain the final validation boundaries.
    /// </summary>
    public sealed class AndroidBuildEarlyPreflight : BuildPlayerProcessor
    {
        public override int callbackOrder { get { return int.MinValue; } }

        public override void PrepareForBuild(BuildPlayerContext context)
        {
            if (context == null)
                throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.SettingsReadFailed);
            if (context.BuildPlayerOptions.target != BuildTarget.Android)
                return;

            ValidateEditorState(EditorApplication.isPlaying,
                EditorApplication.isPlayingOrWillChangePlaymode);

            // Reuse the SAME scheduled-option, policy-hash, output and profile checks.
            // Do not reinterpret AutoRunPlayer as allowed or silently strip it.
            new AndroidBuildPlayerOptionsGuard().PrepareForBuild(context);

            AndroidBuildPreflightResult result =
                AndroidBuildGuard.EvaluateScheduledPreflight(context.BuildPlayerOptions);
            if (result == null || result.Decision == null || !result.Decision.Allowed)
                throw AndroidBuildGuard.Denied(result != null && result.Decision != null
                    ? result.Decision.ReasonCode : AndroidBuildReasonCodes.UnexpectedFailure);
        }

        internal static void ValidateEditorState(bool playing, bool playingOrEntering)
        {
            if (playing || playingOrEntering)
                throw AndroidBuildGuard.Denied(AndroidBuildReasonCodes.EditorPlayModeActive);
        }
    }
}
#endif
