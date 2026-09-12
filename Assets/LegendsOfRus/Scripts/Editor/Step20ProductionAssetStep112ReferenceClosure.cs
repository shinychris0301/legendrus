#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// v1.20 Step 11.2 reference-authority closure. Candidate/review art is never
    /// auto-promoted. A canonical 941x1672 PNG is copied to ApprovedScreens only
    /// when an explicit user-authored approval manifest lists that ScreenId.
    /// </summary>
    public static class Step20ProductionAssetStep112ReferenceClosure
    {
        public const string Root = "ProjectDocs/V1_20_STEP11_2/ReferenceClosure";
        public const string StagingRoot = Root + "/Staging";
        public const string ApprovalManifest = Root + "/USER_APPROVED_REFERENCE_PAGES.txt";
        public const string MatrixCsv = Root + "/REFERENCE_CLOSURE_MATRIX.csv";
        public const string StatusJson = Root + "/REFERENCE_CLOSURE_STATUS.json";
        public const string StatusMd = Root + "/REFERENCE_CLOSURE_STATUS.md";
        public const string PromotionLog = Root + "/REFERENCE_PROMOTION_LOG.json";
        public const int ExpectedWidth = 941;
        public const int ExpectedHeight = 1672;

        public static readonly ScreenId[] BatchAWave1Closure =
        {
            ScreenId.SplashTitle, ScreenId.Pause, ScreenId.ChapterComplete, ScreenId.Dialogue
        };

        [MenuItem("Legends of Rus/Reference Authority/v1.20 Step 11.2/Evaluate 36-Page Reference Closure")]
        public static void Run()
        {
            ClosureStatus status = EvaluateAndWrite();
            string message = "Step 11.2 reference closure: approved=" + status.approved + "/36, missing=" + status.missing + ", activeMissing=" + status.activeMissing + ".";
            Debug.Log(message);
            if (!Application.isBatchMode) EditorUtility.DisplayDialog("Legends of Rus", message + "\n\nReview candidates are not approved automatically. Use the staging + explicit approval-manifest workflow to close gaps.", "OK");
        }

        public static ClosureStatus EvaluateAndWrite()
        {
            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(Root));
            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(StagingRoot));
            var csv = new StringBuilder();
            csv.AppendLine("index,pageKey,screenId,wave,activeProductionRoute,directApprovedReference,referenceDimensionsValid,stagedReferencePresent,priorityBatch,status");
            var missing = new List<string>();
            var activeMissing = new List<string>();
            var invalid = new List<string>();
            int approved = 0;
            for (int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                var p=Step20ProductionAssetStep103Evidence.Pages[i];
                bool active=ProductionCoreLoopPageRegistry.IsCutover(p.id)||ProductionMetaPageCutoverRegistry.IsCutover(p.id);
                string approvedPath=Step20ProductionAssetStep103Evidence.ApprovedReferencePath(p.id);
                bool has=File.Exists(Step20ProductionAssetStep103Evidence.Absolute(approvedPath));
                bool dims=has && HasExpectedDimensions(Step20ProductionAssetStep103Evidence.Absolute(approvedPath));
                string staged=Step20ProductionAssetStep103Evidence.Absolute(StagingRoot+"/"+p.id+".png");
                bool stagedExists=File.Exists(staged);
                string status;
                if(has && dims){approved++;status="DIRECT_APPROVED_REFERENCE";}
                else if(has){status="INVALID_APPROVED_REFERENCE_DIMENSIONS";invalid.Add(p.id.ToString());}
                else {status=active?"ACTIVE_ROUTE_APPROVAL_MISSING":"REFERENCE_APPROVAL_MISSING";missing.Add(p.id.ToString());if(active)activeMissing.Add(p.id.ToString());}
                csv.Append(i).Append(',').Append(p.pageKey).Append(',').Append(p.id).Append(',').Append(p.wave).Append(',')
                    .Append(active).Append(',').Append(has).Append(',').Append(dims).Append(',').Append(stagedExists).Append(',')
                    .Append(BatchName(p.id)).Append(',').Append(status).AppendLine();
            }
            int missingCount=36-approved;
            string decision=invalid.Count>0?"NO_GO_INVALID_APPROVED_REFERENCE_ASSET":activeMissing.Count>0?"PRIORITY_ACTIVE_REFERENCE_GAPS":"REFERENCE_CLOSURE_IN_PROGRESS";
            if(approved==36)decision="REFERENCE_AUTHORITY_COMPLETE_36_OF_36";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(MatrixCsv),csv.ToString());
            string json="{\n"+
                "  \"schemaVersion\": 1,\n"+
                "  \"milestone\": \"v1.20-step11.2-approved-reference-closure\",\n"+
                "  \"decision\": \""+decision+"\",\n"+
                "  \"functionalPages\": 36,\n"+
                "  \"directApprovedReferencePages\": "+approved+",\n"+
                "  \"missingDirectApprovedReferences\": "+missingCount+",\n"+
                "  \"activeRoutesMissingDirectApproval\": "+activeMissing.Count+",\n"+
                "  \"invalidApprovedReferenceDimensions\": "+invalid.Count+",\n"+
                "  \"requiredReferenceSize\": \"941x1672\",\n"+
                "  \"automaticPromotionAllowed\": false,\n"+
                "  \"explicitApprovalManifestRequired\": true,\n"+
                "  \"missingPages\": ["+JsonArray(missing)+"],\n"+
                "  \"activeMissingPages\": ["+JsonArray(activeMissing)+"]\n"+
                "}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusJson),json);
            var md=new StringBuilder();
            md.AppendLine("# v1.20 Step 11.2 — Approved Reference Closure").AppendLine();
            md.AppendLine("Decision: **"+decision+"**").AppendLine();
            md.AppendLine("- Direct approved references: **"+approved+" / 36**");
            md.AppendLine("- Missing direct approved references: **"+missingCount+"**");
            md.AppendLine("- Active routes missing direct approval: **"+activeMissing.Count+"**");
            md.AppendLine("- Required approved reference size: **941 × 1672**");
            md.AppendLine("- Review candidates are never auto-promoted.").AppendLine();
            if(activeMissing.Count>0)md.AppendLine("Priority active gaps: **"+string.Join(", ",activeMissing.ToArray())+"**").AppendLine();
            md.AppendLine("## Closure batches");
            md.AppendLine("- **11.2A — Wave 1 closure:** SplashTitle, Pause, ChapterComplete, Dialogue.");
            md.AppendLine("- **11.2B — Wave 2:** Character Collection through Reward Claim / Shop.");
            md.AppendLine("- **11.2C — Wave 3:** Events, rankings, team/social, friends, mail.");
            md.AppendLine("- **11.2D — Wave 4:** Settings, language, login, tutorial, system states.");
            md.AppendLine("- **11.2E — Store RC:** 36/36 authority + runtime + production route + regression closure.");
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusMd),md.ToString());
            AssetDatabase.Refresh();
            return new ClosureStatus(decision,approved,missingCount,activeMissing.Count,invalid.Count);
        }

        [MenuItem("Legends of Rus/Reference Authority/v1.20 Step 11.2/Validate Staged Reference Intake")]
        public static void ValidateStagedReferenceIntake()
        {
            var failures=new List<string>(); var staged=new List<string>();
            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(StagingRoot));
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                ScreenId id=Step20ProductionAssetStep103Evidence.Pages[i].id;
                string path=Step20ProductionAssetStep103Evidence.Absolute(StagingRoot+"/"+id+".png");
                if(!File.Exists(path))continue; staged.Add(id.ToString());
                if(!HasExpectedDimensions(path))failures.Add(id+": staged reference must be 941x1672 PNG.");
                if(File.Exists(Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id))))failures.Add(id+": approved reference already exists; Step 11.2 refuses to overwrite approved authority through staging.");
            }
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            string message="Staged reference intake valid. Staged pages: "+(staged.Count==0?"none":string.Join(", ",staged.ToArray()))+". Explicit USER_APPROVED_REFERENCE_PAGES.txt is still required before promotion.";
            Debug.Log(message); if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus",message,"OK");
        }

        [MenuItem("Legends of Rus/Reference Authority/v1.20 Step 11.2/Promote Explicitly Approved Staged References")]
        public static void PromoteExplicitlyApprovedStagedReferences()
        {
            EnsureEditorSafe();
            string approvalAbs=Step20ProductionAssetStep103Evidence.Absolute(ApprovalManifest);
            if(!File.Exists(approvalAbs))throw new InvalidOperationException("Explicit approval manifest is missing: "+ApprovalManifest+". Copy the .example file, list only page ScreenIds you explicitly approved, then rerun.");
            string[] lines=File.ReadAllLines(approvalAbs); var approvedIds=new List<ScreenId>(); var failures=new List<string>();
            for(int i=0;i<lines.Length;i++)
            {
                string line=(lines[i]??string.Empty).Trim(); if(line.Length==0||line.StartsWith("#",StringComparison.Ordinal))continue;
                ScreenId id; if(!Enum.TryParse(line,false,out id)||!IsFunctionalPage(id)){failures.Add("Unknown/non-functional ScreenId in approval manifest: "+line);continue;}
                if(approvedIds.Contains(id))continue; approvedIds.Add(id);
                string src=Step20ProductionAssetStep103Evidence.Absolute(StagingRoot+"/"+id+".png");
                string dst=Step20ProductionAssetStep103Evidence.Absolute(Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id));
                if(!File.Exists(src))failures.Add(id+": staged PNG is missing.");
                else if(!HasExpectedDimensions(src))failures.Add(id+": staged PNG must be 941x1672.");
                if(File.Exists(dst))failures.Add(id+": direct approved reference already exists; overwrite is prohibited by this closure tool.");
            }
            if(approvedIds.Count==0)failures.Add("Approval manifest contains no functional ScreenIds.");
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
            string names=string.Join(", ",approvedIds.ConvertAll(x=>x.ToString()).ToArray());
            if(!EditorUtility.DisplayDialog("Legends of Rus","Promote the following STAGED 941x1672 images to direct approved reference authority?\n\n"+names+"\n\nOnly continue if you explicitly approved these exact staged images.","Promote Approved","Cancel"))return;
            var promoted=new List<string>();
            for(int i=0;i<approvedIds.Count;i++)
            {
                ScreenId id=approvedIds[i]; string src=Step20ProductionAssetStep103Evidence.Absolute(StagingRoot+"/"+id+".png");
                string projectDst=Step20ProductionAssetStep103Evidence.ApprovedReferencePath(id); string dst=Step20ProductionAssetStep103Evidence.Absolute(projectDst);
                Directory.CreateDirectory(Path.GetDirectoryName(dst)); File.Copy(src,dst,false); AssetDatabase.ImportAsset(projectDst,ImportAssetOptions.ForceUpdate); promoted.Add(id.ToString());
            }
            string log="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step11.2-explicit-reference-promotion\",\n  \"promotedAtUtc\": \""+DateTime.UtcNow.ToString("o")+"\",\n  \"pages\": ["+JsonArray(promoted)+"]\n}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(PromotionLog),log); AssetDatabase.Refresh(); EvaluateAndWrite();
            Debug.Log("Step 11.2 promoted explicitly approved staged references: "+names+". Runtime fidelity must now be re-evaluated before any production cutover.");
        }

        public static bool HasExpectedDimensions(string absolutePng)
        {
            try
            {
                using(var fs=File.OpenRead(absolutePng))
                {
                    byte[] h=new byte[24]; if(fs.Read(h,0,h.Length)!=h.Length)return false;
                    if(h[0]!=137||h[1]!=80||h[2]!=78||h[3]!=71)return false;
                    int w=(h[16]<<24)|(h[17]<<16)|(h[18]<<8)|h[19]; int ht=(h[20]<<24)|(h[21]<<16)|(h[22]<<8)|h[23];
                    return w==ExpectedWidth&&ht==ExpectedHeight;
                }
            }catch{return false;}
        }

        public static bool IsFunctionalPage(ScreenId id){for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)if(Step20ProductionAssetStep103Evidence.Pages[i].id==id)return true;return false;}
        public static string BatchName(ScreenId id)
        {
            if(id==ScreenId.SplashTitle||id==ScreenId.Pause||id==ScreenId.ChapterComplete||id==ScreenId.Dialogue)return "11.2A";
            for(int i=12;i<=20;i++)if(Step20ProductionAssetStep103Evidence.Pages[i].id==id)return "11.2B";
            for(int i=21;i<=28;i++)if(Step20ProductionAssetStep103Evidence.Pages[i].id==id)return "11.2C";
            for(int i=29;i<=35;i++)if(Step20ProductionAssetStep103Evidence.Pages[i].id==id)return "11.2D";
            return "BASELINE_CORE";
        }
        public static string Sha256(string path){using(var sha=SHA256.Create())using(var s=File.OpenRead(path)){return BitConverter.ToString(sha.ComputeHash(s)).Replace("-",string.Empty).ToLowerInvariant();}}
        static string JsonArray(List<string> values){var sb=new StringBuilder();for(int i=0;i<values.Count;i++){if(i>0)sb.Append(',');sb.Append('"').Append((values[i]??string.Empty).Replace("\\","\\\\").Replace("\"","\\\"")).Append('"');}return sb.ToString();}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Step 11.2 reference promotion cannot run in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Step 11.2 reference promotion.");}

        public readonly struct ClosureStatus
        {
            public readonly string decision; public readonly int approved,missing,activeMissing,invalid;
            public ClosureStatus(string d,int a,int m,int am,int i){decision=d;approved=a;missing=m;activeMissing=am;invalid=i;}
        }
    }
}
#endif
