#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Shared evidence model for v1.20 Step 10.3. Source reconstruction, actual
    /// Unity runtime evidence, approved visual authority, fidelity, authority
    /// boundaries, active routing, and Android acceptance remain independent axes.
    /// </summary>
    public static class Step20ProductionAssetStep103Evidence
    {
        public const float MinCorrelation = 0.80f;
        public const float MaxMae = 0.10f;
        public const string ApprovedRoot = "Assets/LegendsOfRus/Art/ReferenceOnly/ApprovedScreens";

        public static readonly PageSpec[] Pages =
        {
            new PageSpec("SplashTitle",ScreenId.SplashTitle,1), new PageSpec("Loading",ScreenId.Loading,1), new PageSpec("Home",ScreenId.Home,1), new PageSpec("WorldMap",ScreenId.WorldMap,1), new PageSpec("LevelSelect",ScreenId.ChapterMap,1), new PageSpec("PreLevel",ScreenId.PreLevel,1), new PageSpec("Gameplay",ScreenId.Gameplay,1), new PageSpec("Pause",ScreenId.Pause,1), new PageSpec("Victory",ScreenId.Victory,1), new PageSpec("Fail",ScreenId.Fail,1), new PageSpec("ChapterComplete",ScreenId.ChapterComplete,1), new PageSpec("Dialogue",ScreenId.Dialogue,1),
            new PageSpec("CharacterCollection",ScreenId.Characters,2), new PageSpec("DailyReward",ScreenId.DailyReward,2), new PageSpec("Profile",ScreenId.Profile,2), new PageSpec("Inventory",ScreenId.Inventory,2), new PageSpec("Shop",ScreenId.Shop,2), new PageSpec("SpecialOffer",ScreenId.SpecialOffer,2), new PageSpec("OutOfEnergy",ScreenId.OutOfEnergy,2), new PageSpec("BoosterSelect",ScreenId.BoosterSelect,2), new PageSpec("RewardClaim",ScreenId.RewardClaim,2),
            new PageSpec("EventsHub",ScreenId.Events,3), new PageSpec("EventDetail",ScreenId.EventDetail,3), new PageSpec("IndividualRanking",ScreenId.IndividualRanking,3), new PageSpec("TeamHub",ScreenId.TeamHub,3), new PageSpec("TeamRanking",ScreenId.TeamRanking,3), new PageSpec("TeamChat",ScreenId.TeamChat,3), new PageSpec("FriendsList",ScreenId.FriendsList,3), new PageSpec("Mail",ScreenId.Mail,3),
            new PageSpec("Settings",ScreenId.Settings,4), new PageSpec("LanguageSelect",ScreenId.LanguageSelect,4), new PageSpec("Login",ScreenId.Login,4), new PageSpec("TutorialOverlay",ScreenId.TutorialOverlay,4), new PageSpec("OfflineState",ScreenId.OfflineState,4), new PageSpec("MaintenanceState",ScreenId.MaintenanceState,4), new PageSpec("EmptyErrorStates",ScreenId.EmptyErrorState,4)
        };

        public static void BuildAllCandidates()
        {
            if (Step20ProductionAssetWave01PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave01PrefabBuilder.BuildAll(false);
            if (Step20ProductionAssetWave02PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave02PrefabBuilder.BuildAll(false);
            if (Step20ProductionAssetWave03PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave03PrefabBuilder.BuildAll(false);
            if (Step20ProductionAssetWave04PrefabBuilder.NeedsBuild()) Step20ProductionAssetWave04PrefabBuilder.BuildAll(false);
        }

        public static List<PageState> Collect(out List<string> globalFailures)
        {
            BuildAllCandidates();
            globalFailures = ValidateGlobalAuthorityBoundaries();
            var result = new List<PageState>(Pages.Length);
            for (int i = 0; i < Pages.Length; i++) result.Add(Evaluate(Pages[i], globalFailures.Count == 0));
            return result;
        }

        public static PageState Evaluate(PageSpec spec, bool globalAuthorityPass)
        {
            string candidatePath = CandidatePath(spec);
            bool candidate = AssetDatabase.LoadAssetAtPath<GameObject>(candidatePath) != null;
            bool runtime = HasRuntimeEvidence(spec);
            string approvedPath = ApprovedReferencePath(spec.id);
            bool approved = File.Exists(Absolute(approvedPath));
            bool active = ProductionCoreLoopPageRegistry.IsCutover(spec.id) || ProductionMetaPageCutoverRegistry.IsCutover(spec.id);
            bool fidelityEvaluated = false;
            bool fidelityPass = false;
            float correlation = 0f, mae = 1f;
            string fidelityNote = approved ? "APPROVED_REFERENCE_AWAITING_RUNTIME_CAPTURE" : "NO_DIRECT_APPROVED_REFERENCE";

            if (approved && runtime)
            {
                Metrics m;
                string failure;
                if (TryCompareRuntimeToApproved(spec, out m, out failure))
                {
                    fidelityEvaluated = true;
                    correlation = m.correlation;
                    mae = m.mae;
                    fidelityPass = correlation >= MinCorrelation && mae <= MaxMae;
                    fidelityNote = fidelityPass ? "RUNTIME_FIDELITY_PASS" : "RUNTIME_FIDELITY_FAIL";
                }
                else
                {
                    fidelityNote = "RUNTIME_FIDELITY_ERROR:" + failure;
                }
            }

            string category;
            bool developmentCutoverEligible = false;
            if (!candidate || !globalAuthorityPass)
                category = "BLOCKED_FIX_REQUIRED";
            else if (active)
                category = "ACTIVE_ACCEPTED_OR_DEVELOPMENT_ROUTE";
            else if (approved && runtime && fidelityEvaluated && fidelityPass)
            {
                category = "READY_FOR_ROLLBACK_CONTROLLED_DEVELOPMENT_CUTOVER";
                developmentCutoverEligible = !ProductionCoreLoopPageRegistry.IsCutover(spec.id) && ProductionMetaPageCutoverRegistry.IsSupportedMetaPage(spec.id);
            }
            else if (!approved && runtime)
                category = "RUNTIME_VALID_AWAITING_APPROVED_REFERENCE";
            else if (approved && !runtime)
                category = "APPROVED_REFERENCE_AWAITING_RUNTIME_EVIDENCE";
            else if (approved && runtime && fidelityEvaluated && !fidelityPass)
                category = "BLOCKED_RUNTIME_FIDELITY_FIX_REQUIRED";
            else
                category = "CANDIDATE_AWAITING_RUNTIME_AND_REFERENCE_EVIDENCE";

            return new PageState(spec, candidatePath, candidate, runtime, approved, approvedPath, fidelityEvaluated, fidelityPass, correlation, mae, fidelityNote, globalAuthorityPass, active, developmentCutoverEligible, category);
        }

        public static List<string> ValidateGlobalAuthorityBoundaries()
        {
            var failures = new List<string>();
            if (!ProductionCutoverFreezeGuard.MatchesExpectedCount(10)) failures.Add("Accepted core production registry is not exactly ten routes.");
            AddPrefixed(failures, "WAVE1-CANDIDATE: ", Step20ProductionAssetWave01PrefabBuilder.ValidateAll());
            AddPrefixed(failures, "WAVE2-PROVIDER: ", Step20ProductionAssetWave02Step84ProviderBoundaryGate.Validate(false));
            AddPrefixed(failures, "WAVE3-SOCIAL-LIVEOPS: ", Step20ProductionAssetWave03Step94BoundaryGate.Validate(false));
            AddPrefixed(failures, "WAVE4-ACCOUNT-SYSTEM: ", Step20ProductionAssetWave04Step102BoundaryGate.Validate(false));
            return Deduplicate(failures);
        }

        public static string CandidatePath(PageSpec spec)
        {
            string root = spec.wave == 1 ? Step20ProductionAssetWave01PrefabBuilder.CandidateRoot :
                          spec.wave == 2 ? Step20ProductionAssetWave02PrefabBuilder.CandidateRoot :
                          spec.wave == 3 ? Step20ProductionAssetWave03PrefabBuilder.CandidateRoot :
                                           Step20ProductionAssetWave04PrefabBuilder.CandidateRoot;
            return root + "/" + spec.id + ".prefab";
        }

        public static string ApprovedReferencePath(ScreenId id)
        {
            return ApprovedRoot + "/" + id + ".png";
        }

        public static string RuntimeCaptureRoot(int wave)
        {
            switch (wave)
            {
                case 1: return "ProjectDocs/V1_20_STEP07_4/UnityRuntimeCaptures";
                case 2: return "ProjectDocs/V1_20_STEP08_4/UnityRuntimeCaptures";
                case 3: return "ProjectDocs/V1_20_STEP09_4/RuntimeEvidence";
                default: return "ProjectDocs/V1_20_STEP10_2/RuntimeEvidence";
            }
        }

        public static int ExpectedWaveCaptureCount(int wave)
        {
            return wave == 1 ? 48 : wave == 2 ? 36 : wave == 3 ? 32 : 28;
        }

        public static bool HasRuntimeEvidence(PageSpec spec)
        {
            string root = Absolute(RuntimeCaptureRoot(spec.wave));
            string manifest = Path.Combine(root, "UNITY_RUNTIME_CAPTURE_EXECUTED.json");
            if (File.Exists(manifest))
            {
                string manifestText = File.ReadAllText(manifest);
                if (manifestText.IndexOf("\"captures\": " + ExpectedWaveCaptureCount(spec.wave), StringComparison.Ordinal) >= 0)
                {
                    string[] suffixes = { "941x1672", "1080x2340", "1080x2400", "1600x2560" };
                    bool complete = true;
                    for (int i = 0; i < suffixes.Length; i++)
                        if (!File.Exists(Path.Combine(root, spec.id + "_" + suffixes[i] + ".png"))) { complete = false; break; }
                    if (complete) return true;
                }
            }

            // Step 11.2A can close the two already-active Wave 1 authority gaps without
            // forcing a redundant 48-capture run. This supplemental path is accepted only
            // for Pause and ChapterComplete and still requires four real Unity renders each.
            return spec.wave == 1 && Step20ProductionAssetStep112ARuntimeCloseout.HasFocusedEvidence(spec.id);
        }

        static bool TryCompareRuntimeToApproved(PageSpec spec, out Metrics metrics, out string failure)
        {
            metrics = new Metrics { correlation = 0f, mae = 1f };
            failure = string.Empty;
            try
            {
                string capture = Path.Combine(Absolute(RuntimeCaptureRoot(spec.wave)), spec.id + "_941x1672.png");
                if (!File.Exists(capture) && spec.wave == 1 && (spec.id == ScreenId.Pause || spec.id == ScreenId.ChapterComplete))
                    capture = Path.Combine(Step20ProductionAssetStep112ARuntimeCloseout.AbsoluteOutputRoot(), spec.id + "_941x1672.png");
                string reference = Absolute(ApprovedReferencePath(spec.id));
                if (!File.Exists(capture)) { failure = "missing runtime 941x1672 capture"; return false; }
                if (!File.Exists(reference)) { failure = "missing approved reference"; return false; }
                Texture2D a = LoadPng(capture), b = LoadPng(reference);
                try { metrics = Compare(a, b, spec.id == ScreenId.Gameplay); }
                finally { UnityEngine.Object.DestroyImmediate(a); UnityEngine.Object.DestroyImmediate(b); }
                return true;
            }
            catch (Exception e) { failure = e.Message; return false; }
        }

        static Texture2D LoadPng(string path)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(path), false)) throw new InvalidOperationException("Could not decode PNG: " + path);
            return texture;
        }

        static Metrics Compare(Texture2D a, Texture2D b, bool gameplayShellOnly)
        {
            if (a.width != b.width || a.height != b.height) throw new InvalidOperationException("Runtime/reference size mismatch: " + a.width + "x" + a.height + " vs " + b.width + "x" + b.height + ".");
            Color32[] pa = a.GetPixels32(), pb = b.GetPixels32();
            int width = a.width, height = a.height; const int stride = 4;
            double sumA=0d,sumB=0d,sumAA=0d,sumBB=0d,sumAB=0d,mae=0d; long samples=0;
            for (int topY=0; topY<height; topY+=stride)
            {
                if (gameplayShellOnly && topY >= 357 && topY < 1299) continue;
                int py=height-1-topY,row=py*width;
                for (int x=0; x<width; x+=stride)
                {
                    Color32 ca=pa[row+x], cb=pb[row+x];
                    double la=(0.2126*ca.r+0.7152*ca.g+0.0722*ca.b)/255d, lb=(0.2126*cb.r+0.7152*cb.g+0.0722*cb.b)/255d;
                    sumA+=la; sumB+=lb; sumAA+=la*la; sumBB+=lb*lb; sumAB+=la*lb;
                    mae+=(Math.Abs(ca.r-cb.r)+Math.Abs(ca.g-cb.g)+Math.Abs(ca.b-cb.b))/(3d*255d); samples++;
                }
            }
            if (samples<=1) return new Metrics{correlation=0f,mae=1f};
            double n=samples,cov=sumAB-(sumA*sumB/n),va=sumAA-(sumA*sumA/n),vb=sumBB-(sumB*sumB/n),den=Math.Sqrt(Math.Max(0d,va)*Math.Max(0d,vb));
            return new Metrics{correlation=(float)(den>1e-12?cov/den:0d),mae=(float)(mae/n)};
        }

        public static string FormatFloat(float v) { return v.ToString("F4", CultureInfo.InvariantCulture); }
        public static string Absolute(string path) { return Path.Combine(Directory.GetCurrentDirectory(), path.Replace('/', Path.DirectorySeparatorChar)); }
        static void AddPrefixed(List<string> target, string prefix, List<string> source) { for (int i=0;i<source.Count;i++) target.Add(prefix+source[i]); }
        static List<string> Deduplicate(List<string> items) { var seen=new HashSet<string>(StringComparer.Ordinal); var result=new List<string>(); for(int i=0;i<items.Count;i++) if(seen.Add(items[i])) result.Add(items[i]); return result; }

        public struct Metrics { public float correlation; public float mae; }

        public readonly struct PageSpec
        {
            public readonly string pageKey; public readonly ScreenId id; public readonly int wave;
            public PageSpec(string pageKey, ScreenId id, int wave) { this.pageKey=pageKey; this.id=id; this.wave=wave; }
        }

        public readonly struct PageState
        {
            public readonly PageSpec spec; public readonly string candidatePath; public readonly bool sourceCandidate, runtimeEvidence, directApprovedReference;
            public readonly string approvedReferencePath; public readonly bool fidelityEvaluated, fidelityPass; public readonly float correlation, mae; public readonly string fidelityNote;
            public readonly bool authorityBoundaryPass, activeProductionRoute, developmentCutoverEligible; public readonly string category;
            public PageState(PageSpec spec,string candidatePath,bool sourceCandidate,bool runtimeEvidence,bool directApprovedReference,string approvedReferencePath,bool fidelityEvaluated,bool fidelityPass,float correlation,float mae,string fidelityNote,bool authorityBoundaryPass,bool activeProductionRoute,bool developmentCutoverEligible,string category)
            { this.spec=spec; this.candidatePath=candidatePath; this.sourceCandidate=sourceCandidate; this.runtimeEvidence=runtimeEvidence; this.directApprovedReference=directApprovedReference; this.approvedReferencePath=approvedReferencePath; this.fidelityEvaluated=fidelityEvaluated; this.fidelityPass=fidelityPass; this.correlation=correlation; this.mae=mae; this.fidelityNote=fidelityNote; this.authorityBoundaryPass=authorityBoundaryPass; this.activeProductionRoute=activeProductionRoute; this.developmentCutoverEligible=developmentCutoverEligible; this.category=category; }
        }
    }
}
#endif
