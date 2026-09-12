using System;
using System.Collections.Generic;
using UnityEngine;

namespace LegendsOfRus
{
    /// <summary>
    /// Explicit font bindings. Development fallback availability and exact reference
    /// identity are different checks. No font binaries are supplied by this patch.
    /// </summary>
    [CreateAssetMenu(menuName = "Legends of Rus/Splash Title Typography")]
    public sealed class SplashTitleTypography : ScriptableObject
    {
        public Font RegularFont;
        public Font StartFont;
        [Tooltip("Record the actual source family/style, not an inferred name from a screenshot.")]
        public string ReferenceFontIdentity = "UNIDENTIFIED: source font not supplied";
        [TextArea] public string SourceEvidence = "";
        public bool ReferenceIdentityVerified;
        public bool AllowDevelopmentFallback = true;
        public const string RequiredGlyphs = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя0123456789©–. :";

        // These are session-scoped rather than tied to the loaded ScriptableObject.
        // A route change / UnloadUnusedAssets / reload must not create another copy
        // of the same OS font or repeat the same unresolved-status warning.
        static Font previewFont;
        static string previewSource;
        static bool ownsPreviewFont;
        static readonly HashSet<string> reportedStates = new HashSet<string>();

        public bool HasVerifiedBindings
        {
            get { string issue; return !TryGetBindingIssue(out issue); }
        }

        /// <summary>Returns true when the exact-font release requirement is still open.</summary>
        public bool TryGetBindingIssue(out string issue)
        {
            if (RegularFont == null || StartFont == null)
            {
                issue = RegularFont == null && StartFont == null
                    ? "RegularFont and StartFont are not assigned."
                    : (RegularFont == null ? "RegularFont is not assigned." : "StartFont is not assigned.");
                return true;
            }
            if (!SupportsRequiredGlyphs(RegularFont) || !SupportsRequiredGlyphs(StartFont))
            {
                issue = "One or both bound fonts fail the configured Cyrillic/symbol coverage check.";
                return true;
            }
            if (!ReferenceIdentityVerified || string.IsNullOrWhiteSpace(ReferenceFontIdentity) ||
                ReferenceFontIdentity.Trim().StartsWith("UNIDENTIFIED", StringComparison.OrdinalIgnoreCase) ||
                string.IsNullOrWhiteSpace(SourceEvidence))
            {
                issue = "Font files are bound, but their exact reference identity/evidence is not verified.";
                return true;
            }
            issue = "";
            return false;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void OnPlaySessionStarted()
        {
            ReleaseDevelopmentSession();
        }

        /// <summary>
        /// Also called by the Editor hook AFTER leaving Play Mode. Never destroys
        /// imported font assets or Unity's built-in font. Do not call while UI is active.
        /// </summary>
        public static void ReleaseDevelopmentSession()
        {
            if (ownsPreviewFont && previewFont != null) DestroyCreatedFont(previewFont);
            previewFont = null;
            previewSource = null;
            ownsPreviewFont = false;
            reportedStates.Clear();
        }

        public Font Resolve(bool startLabel, out string source)
        {
            Font bound = startLabel ? (StartFont != null ? StartFont : RegularFont) : RegularFont;
            if (bound != null && SupportsRequiredGlyphs(bound))
            {
                source = bound.name + (HasVerifiedBindings ? " (source verified)" : " (reference identity unverified)");
                WarnIfUnverified(source);
                return bound;
            }

            bool development = Application.isEditor || Debug.isDebugBuild;
            if (!AllowDevelopmentFallback || !development)
            {
                source = "NO USABLE BOUND FONT; development fallback disabled or not permitted";
                WarnIfUnverified(source);
                return null;
            }

            EnsurePreviewFont();
            source = previewSource ?? "NO DEVELOPMENT FONT AVAILABLE";
            WarnIfUnverified(source);
            return previewFont;
        }

        static void EnsurePreviewFont()
        {
            if (previewFont != null) return;
            string lookupNote = "";
            try
            {
                // OS fonts remain on this machine. Nothing is copied into Assets.
                string[] candidates = { "Times New Roman", "Liberation Serif", "Noto Serif", "Droid Serif" };
                string[] installed = Font.GetOSInstalledFontNames();
                for (int c = 0; c < candidates.Length && previewFont == null; c++)
                {
                    for (int i = 0; installed != null && i < installed.Length; i++)
                    {
                        if (!string.Equals(installed[i], candidates[c], StringComparison.OrdinalIgnoreCase)) continue;
                        Font trial = Font.CreateDynamicFontFromOSFont(installed[i], 80);
                        if (trial != null && SupportsRequiredGlyphs(trial))
                        {
                            previewFont = trial;
                            ownsPreviewFont = true;
                            previewSource = "OS " + installed[i] + " — DEVELOPMENT ONLY";
                        }
                        else if (trial != null) DestroyCreatedFont(trial);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                lookupNote = " (OS lookup unavailable: " + ex.GetType().Name + ")";
            }

            if (previewFont != null) return;
            previewFont = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            ownsPreviewFont = false;
            previewSource = previewFont != null
                ? "Unity LegacyRuntime — DEVELOPMENT ONLY" + lookupNote
                : "NO DEVELOPMENT FONT AVAILABLE" + lookupNote;
        }

        static void DestroyCreatedFont(Font font)
        {
            if (Application.isPlaying) Destroy(font);
            else DestroyImmediate(font);
        }

        void WarnIfUnverified(string source)
        {
            if (!Application.isPlaying) return;
            string issue;
            if (!TryGetBindingIssue(out issue)) return;

            string state = name + "|" + source + "|" + issue;
            if (!reportedStates.Add(state)) return;
            Debug.LogWarning(
                "SplashTitle typography is NOT reference-approved. Rendering with " + source +
                ". " + issue +
                " Open Legends of Rus > Production UI > Splash R3 > Select Typography Settings." +
                " Bind project font assets and verify their identity against the reference." +
                " An image-generated reference may not have an identifiable source typeface;" +
                " do not mark a substitute as exact. This notice is emitted once per unchanged status per Play session.",
                this);
        }

        public static bool SupportsRequiredGlyphs(Font font)
        {
            if (font == null) return false;
            foreach (char ch in RequiredGlyphs)
                if (!char.IsWhiteSpace(ch) && !font.HasCharacter(ch)) return false;
            return true;
        }
    }
}
