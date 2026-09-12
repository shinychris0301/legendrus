#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Cumulative, evidence-qualified production-route expansion for Step 11.2.
    /// The accepted ten-route core registry is never edited. Non-core routes use
    /// ProductionMetaPageCutoverRegistry states and immutable per-page rollback snapshots.
    /// A later batch requires the currently active integration to have a valid Step 11
    /// development RC freeze, which proves functional + physical Android regression for
    /// the current fingerprint before more pages are added.
    /// </summary>
    public static class Step20ProductionAssetStep112RouteExpansion
    {
        public const string Root = "ProjectDocs/V1_20_STEP11_2/RouteExpansion";
        public const string RollbackRoot = Root + "/Rollback";
        public const string StatusJson = Root + "/ROUTE_EXPANSION_STATUS.json";
        public const string StatusMd = Root + "/ROUTE_EXPANSION_STATUS.md";
        const string ActivePagesRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string StateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/CutoverState";

        static readonly ScreenId[] BatchA = { ScreenId.SplashTitle, ScreenId.Dialogue };
        static readonly ScreenId[] BatchB = { ScreenId.Characters, ScreenId.DailyReward, ScreenId.Profile, ScreenId.Inventory, ScreenId.Shop, ScreenId.SpecialOffer, ScreenId.OutOfEnergy, ScreenId.BoosterSelect, ScreenId.RewardClaim };
        static readonly ScreenId[] BatchC = { ScreenId.Events, ScreenId.EventDetail, ScreenId.IndividualRanking, ScreenId.TeamHub, ScreenId.TeamRanking, ScreenId.TeamChat, ScreenId.FriendsList, ScreenId.Mail };
        static readonly ScreenId[] BatchD = { ScreenId.Settings, ScreenId.LanguageSelect, ScreenId.Login, ScreenId.TutorialOverlay, ScreenId.OfflineState, ScreenId.MaintenanceState, ScreenId.EmptyErrorState };

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Evaluate Production Route Expansion Batches")]
        public static void EvaluateMenu()
        {
            ExpansionStatus status=EvaluateAndWrite();
            string message="Step 11.2 route expansion: A="+status.batchA+", B="+status.batchB+", C="+status.batchC+", D="+status.batchD+", active="+status.active+"/36.";
            Debug.Log(message); if(!Application.isBatchMode)EditorUtility.DisplayDialog("Legends of Rus",message+"\n\nNo route is activated by evaluation.","OK");
        }

        public static ExpansionStatus EvaluateAndWrite()
        {
            List<string> globalFailures; List<Step20ProductionAssetStep103Evidence.PageState> states=Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            string a=BatchDecision("11.2A",BatchA,states,globalFailures,true);
            string b=BatchDecision("11.2B",BatchB,states,globalFailures,false);
            string c=BatchDecision("11.2C",BatchC,states,globalFailures,false);
            string d=BatchDecision("11.2D",BatchD,states,globalFailures,false);
            int active=0; for(int i=0;i<states.Count;i++)if(states[i].activeProductionRoute)active++;
            Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(Root));
            string json="{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step11.2-controlled-production-route-expansion\",\n"+
                "  \"activeProductionRoutes\": "+active+",\n  \"batchA\": \""+a+"\",\n  \"batchB\": \""+b+"\",\n  \"batchC\": \""+c+"\",\n  \"batchD\": \""+d+"\",\n"+
                "  \"coreRegistryModified\": false,\n  \"regressionRequiredBetweenBatches\": true,\n  \"immutableRollbackPerPage\": true,\n  \"automaticCutoverPerformed\": false\n}\n";
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusJson),json);
            var md=new StringBuilder(); md.AppendLine("# v1.20 Step 11.2 — Production Route Expansion").AppendLine();
            md.AppendLine("Active production routes: **"+active+" / 36**").AppendLine();
            md.AppendLine("| Batch | Scope | Status |").AppendLine("|---|---|---|")
              .AppendLine("| 11.2A | Wave 1 authority closure + SplashTitle/Dialogue route expansion | "+a+" |")
              .AppendLine("| 11.2B | Wave 2 meta/economy | "+b+" |")
              .AppendLine("| 11.2C | Wave 3 Events/Rankings/Social | "+c+" |")
              .AppendLine("| 11.2D | Wave 4 Account/Language/System | "+d+" |").AppendLine();
            md.AppendLine("A batch can be applied only when every inactive page in that batch is evidence-qualified. Before each new batch, the current integration must have a valid Step 11 Development RC freeze (functional + physical Android acceptance for the same fingerprint). Applying a batch clears that freeze and regression evidence, requiring fresh regression before the next batch.");
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(StatusMd),md.ToString()); AssetDatabase.Refresh();
            return new ExpansionStatus(active,a,b,c,d,globalFailures.Count);
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Apply Batch A — Wave 1 Closure")]
        public static void ApplyBatchA(){ApplyBatch("11.2A",BatchA,true);}
        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Apply Batch B — Wave 2 Meta Economy")]
        public static void ApplyBatchB(){RequirePriorComplete("11.2A",BatchA,true);ApplyBatch("11.2B",BatchB,false);}
        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Apply Batch C — Wave 3 Social LiveOps")]
        public static void ApplyBatchC(){RequirePriorComplete("11.2B",BatchB,false);ApplyBatch("11.2C",BatchC,false);}
        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Apply Batch D — Wave 4 Account System")]
        public static void ApplyBatchD(){RequirePriorComplete("11.2C",BatchC,false);ApplyBatch("11.2D",BatchD,false);}

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 11.2/Roll Back Most Recent Expansion Batch")]
        public static void RollBackMostRecent()
        {
            EnsureEditorSafe();
            string batch=File.Exists(BatchLockAbs("11.2D"))?"11.2D":File.Exists(BatchLockAbs("11.2C"))?"11.2C":File.Exists(BatchLockAbs("11.2B"))?"11.2B":File.Exists(BatchLockAbs("11.2A"))?"11.2A":string.Empty;
            if(string.IsNullOrEmpty(batch))throw new InvalidOperationException("No active Step 11.2 route-expansion batch lock exists.");
            if(!EditorUtility.DisplayDialog("Legends of Rus","Rollback the most recent Step 11.2 expansion batch "+batch+" to its immutable pre-batch page states?","Rollback","Cancel"))return;
            string[] lines=File.ReadAllLines(BatchLockAbs(batch)); var ids=new List<ScreenId>();
            for(int i=0;i<lines.Length;i++){ScreenId id;if(Enum.TryParse(lines[i].Trim(),false,out id))ids.Add(id);}
            for(int i=ids.Count-1;i>=0;i--)RestorePage(ids[i]);
            File.Delete(BatchLockAbs(batch)); InvalidateRegressionAndFreezes(); AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); EvaluateAndWrite();
            Debug.Log("Step 11.2 rollback complete for "+batch+". Fresh functional + Android regression is required before another expansion batch.");
        }

        public static List<string> ValidateActiveExpansions()
        {
            var failures=new List<string>();
            string[] batches={"11.2A","11.2B","11.2C","11.2D"};
            for(int b=0;b<batches.Length;b++)
            {
                string path=BatchLockAbs(batches[b]); if(!File.Exists(path))continue;
                string[] lines=File.ReadAllLines(path);
                for(int i=0;i<lines.Length;i++)
                {
                    ScreenId id; if(!Enum.TryParse(lines[i].Trim(),false,out id))continue;
                    if(!ProductionMetaPageCutoverRegistry.IsCutover(id))failures.Add(batches[b]+"/"+id+": activation state missing.");
                    ValidateActivePrefab(id,failures);
                }
            }
            return failures;
        }

        static void ApplyBatch(string batch,ScreenId[] scope,bool requireWave1ActiveClosure)
        {
            EnsureEditorSafe(); RequireValidDevelopmentRcFreeze();
            List<string> globalFailures; List<Step20ProductionAssetStep103Evidence.PageState> states=Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            if(globalFailures.Count>0)throw new InvalidOperationException("Global authority gate failed:\n"+string.Join(Environment.NewLine,globalFailures.ToArray()));
            if(requireWave1ActiveClosure)ValidateWave1ActiveReferenceClosureOrThrow(states);
            var byId=new Dictionary<ScreenId,Step20ProductionAssetStep103Evidence.PageState>(); for(int i=0;i<states.Count;i++)byId[states[i].spec.id]=states[i];
            var toApply=new List<Step20ProductionAssetStep103Evidence.PageState>(); var blockers=new List<string>();
            for(int i=0;i<scope.Length;i++)
            {
                ScreenId id=scope[i]; Step20ProductionAssetStep103Evidence.PageState state;
                if(!byId.TryGetValue(id,out state)){blockers.Add(id+": missing evidence state.");continue;}
                if(state.activeProductionRoute)continue;
                if(!state.developmentCutoverEligible)blockers.Add(id+": "+state.category+" (approved/runtime/fidelity/authority evidence incomplete)."); else toApply.Add(state);
            }
            if(blockers.Count>0)throw new InvalidOperationException(batch+" is not ready:\n"+string.Join(Environment.NewLine,blockers.ToArray()));
            if(toApply.Count==0){WriteBatchLock(batch,new List<ScreenId>());EvaluateAndWrite();Debug.Log(batch+" already complete; no new routes were required.");return;}
            string names=string.Empty;for(int i=0;i<toApply.Count;i++){if(i>0)names+=", ";names+=toApply[i].spec.id;}
            if(!EditorUtility.DisplayDialog("Legends of Rus","Apply "+batch+" rollback-controlled production-route expansion for:\n\n"+names+"\n\nThe current Development RC freeze/regression evidence will be invalidated and must be rerun before the next batch.","Apply Batch","Cancel"))return;
            ClearDevelopmentRcFreeze();
            var applied=new List<ScreenId>();
            try
            {
                for(int i=0;i<toApply.Count;i++){SnapshotOnce(toApply[i]);CopyCandidateToActive(toApply[i]);WriteStateLast(toApply[i]);applied.Add(toApply[i].spec.id);}
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); for(int i=0;i<applied.Count;i++)ValidateActiveOrThrow(applied[i]);
                WriteBatchLock(batch,applied); InvalidateRegressionAndFreezes(); AssetDatabase.Refresh(); EvaluateAndWrite();
                Debug.Log("Step 11.2 "+batch+" applied: "+names+". Run full functional + physical Android regression, then recreate a Step 11 Development RC freeze before applying the next batch.");
            }
            catch
            {
                for(int i=applied.Count-1;i>=0;i--)RestorePage(applied[i]); AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate); throw;
            }
        }

        static string BatchDecision(string batch,ScreenId[] scope,List<Step20ProductionAssetStep103Evidence.PageState> states,List<string> globalFailures,bool requireWave1ActiveClosure)
        {
            if(globalFailures.Count>0)return "BLOCKED_GLOBAL_AUTHORITY_FAILURE";
            if(requireWave1ActiveClosure)
            {
                var prereq=new List<string>(); ValidateWave1ActiveReferenceClosure(states,prereq); if(prereq.Count>0)return "BLOCKED_ACTIVE_WAVE1_REFERENCE_CLOSURE";
            }
            var map=new Dictionary<ScreenId,Step20ProductionAssetStep103Evidence.PageState>();for(int i=0;i<states.Count;i++)map[states[i].spec.id]=states[i];
            int active=0,eligible=0,blocked=0;
            for(int i=0;i<scope.Length;i++)
            {
                Step20ProductionAssetStep103Evidence.PageState s;if(!map.TryGetValue(scope[i],out s)){blocked++;continue;}
                if(s.activeProductionRoute)active++; else if(s.developmentCutoverEligible)eligible++; else blocked++;
            }
            if(active==scope.Length)return "COMPLETE_ACTIVE";
            if(blocked>0)return "BLOCKED_EVIDENCE_GAPS_"+blocked;
            return "READY_TO_APPLY_"+eligible+"_ROUTES";
        }

        static void ValidateWave1ActiveReferenceClosureOrThrow(List<Step20ProductionAssetStep103Evidence.PageState> states){var f=new List<string>();ValidateWave1ActiveReferenceClosure(states,f);if(f.Count>0)throw new InvalidOperationException("Step 11.2A requires active Wave 1 reference closure first:\n"+string.Join(Environment.NewLine,f.ToArray()));}
        static void ValidateWave1ActiveReferenceClosure(List<Step20ProductionAssetStep103Evidence.PageState> states,List<string> failures)
        {
            ScreenId[] ids={ScreenId.Pause,ScreenId.ChapterComplete};
            for(int j=0;j<ids.Length;j++)
            {
                bool found=false; for(int i=0;i<states.Count;i++)if(states[i].spec.id==ids[j]){found=true;var s=states[i];if(!s.activeProductionRoute)failures.Add(ids[j]+": expected active core route.");if(!s.directApprovedReference)failures.Add(ids[j]+": direct approved reference missing.");if(!s.runtimeEvidence)failures.Add(ids[j]+": runtime evidence missing.");if(!s.fidelityEvaluated||!s.fidelityPass)failures.Add(ids[j]+": approved runtime fidelity PASS missing.");break;}
                if(!found)failures.Add(ids[j]+": evidence state missing.");
            }
        }

        static void RequirePriorComplete(string batch,ScreenId[] scope,bool wave1)
        {
            List<string> g;List<Step20ProductionAssetStep103Evidence.PageState> states=Step20ProductionAssetStep103Evidence.Collect(out g);string d=BatchDecision(batch,scope,states,g,wave1);if(d!="COMPLETE_ACTIVE")throw new InvalidOperationException("Prior batch "+batch+" is not complete: "+d+".");
        }
        static void RequireValidDevelopmentRcFreeze(){List<string> f=Step20ProductionAssetStep11Evidence.ValidateDevelopmentRcFreeze();if(f.Count>0)throw new InvalidOperationException("A valid Step 11 Development RC freeze for the current integration is required before route expansion:\n"+string.Join(Environment.NewLine,f.ToArray()));}
        static void ClearDevelopmentRcFreeze(){DeleteIfExists(Step20ProductionAssetStep11Evidence.DevelopmentRcLock);DeleteIfExists(Step20ProductionAssetStep11Evidence.DevelopmentRcManifest);}
        static void InvalidateRegressionAndFreezes(){Step20ProductionAssetStep103AndroidRegression.ClearAcceptanceLocks();ClearDevelopmentRcFreeze();DeleteIfExists(Step20ProductionAssetStep112StoreRc.StoreRcLock);DeleteIfExists(Step20ProductionAssetStep112StoreRc.StoreRcManifest);}

        static void SnapshotOnce(Step20ProductionAssetStep103Evidence.PageState state)
        {
            string dir=SnapshotDirectory(state.spec.id);Directory.CreateDirectory(dir);string manifest=Path.Combine(dir,"baseline.json");if(File.Exists(manifest))return;
            string active=ActivePrefabPath(state.spec.id),activeMeta=active+".meta",cutover=StateAssetPath(state.spec.id),cutoverMeta=cutover+".meta";
            bool pe=File.Exists(active),pme=File.Exists(activeMeta),se=File.Exists(cutover),sme=File.Exists(cutoverMeta);
            if(pe)File.Copy(active,Path.Combine(dir,"original.prefab"),false);if(pme)File.Copy(activeMeta,Path.Combine(dir,"original.prefab.meta"),false);if(se)File.Copy(cutover,Path.Combine(dir,"original.state.json"),false);if(sme)File.Copy(cutoverMeta,Path.Combine(dir,"original.state.json.meta"),false);
            string json="{\n  \"schemaVersion\": 1,\n  \"page\": \""+state.spec.id+"\",\n  \"originalActivePrefabExisted\": "+Bool(pe)+",\n  \"originalActivePrefabMetaExisted\": "+Bool(pme)+",\n  \"originalStateExisted\": "+Bool(se)+",\n  \"originalStateMetaExisted\": "+Bool(sme)+",\n  \"originalActivePrefabSha256\": \""+(pe?Sha256(active):string.Empty)+"\",\n  \"baselineCandidateSha256\": \""+Sha256(state.candidatePath)+"\"\n}\n";File.WriteAllText(manifest,json);
        }
        static void CopyCandidateToActive(Step20ProductionAssetStep103Evidence.PageState state){string active=ActivePrefabPath(state.spec.id);Directory.CreateDirectory(Path.GetDirectoryName(active));File.Copy(state.candidatePath,active,true);AssetDatabase.ImportAsset(ProjectRelative(active),ImportAssetOptions.ForceUpdate);var f=new List<string>();ValidateActivePrefab(state.spec.id,f);if(f.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray()));}
        static void WriteStateLast(Step20ProductionAssetStep103Evidence.PageState state){string path=StateAssetPath(state.spec.id);Directory.CreateDirectory(Path.GetDirectoryName(path));string json="{\n  \"schemaVersion\": 1,\n  \"magic\": \""+ProductionMetaPageCutoverRegistry.Step103StateMagicPrefix+state.spec.id+"\",\n  \"page\": \""+state.spec.id+"\",\n  \"enabled\": true,\n  \"sourceCandidateSha256\": \""+Sha256(state.candidatePath)+"\",\n  \"runtimeEvidence\": true,\n  \"directApprovedReference\": true,\n  \"runtimeFidelityPass\": true,\n  \"androidAcceptancePending\": true,\n  \"appliedBy\": \"v1.20-step11.2-route-expansion\"\n}\n";File.WriteAllText(path,json);AssetDatabase.ImportAsset(ProjectRelative(path),ImportAssetOptions.ForceUpdate);}
        static void ValidateActiveOrThrow(ScreenId id){var f=new List<string>();if(!ProductionMetaPageCutoverRegistry.IsCutover(id))f.Add(id+": meta cutover state failed to activate.");ValidateActivePrefab(id,f);if(f.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,f.ToArray()));}
        static void ValidateActivePrefab(ScreenId id,List<string> f)
        {
            string p="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+id+".prefab";GameObject go=AssetDatabase.LoadAssetAtPath<GameObject>(p);if(go==null){f.Add(id+": active production prefab missing.");return;}
            ProductionCoreLoopPageMarker m=go.GetComponent<ProductionCoreLoopPageMarker>();if(m==null||m.ScreenId!=id||!m.ScreenshotFree)f.Add(id+": production marker/screenshot-free contract invalid.");
            if(go.GetComponentsInChildren<Button>(true).Length!=0)f.Add(id+": active prefab contains Button authority.");if(go.GetComponentsInChildren<EventTrigger>(true).Length!=0)f.Add(id+": active prefab contains EventTrigger authority.");
            string[] deps=AssetDatabase.GetDependencies(p,true);for(int i=0;i<deps.Length;i++){string n=deps[i].Replace('\\','/');if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/"))f.Add(id+": forbidden reference/QA dependency: "+deps[i]);}
        }
        static void RestorePage(ScreenId id){string dir=SnapshotDirectory(id),manifest=Path.Combine(dir,"baseline.json");if(!File.Exists(manifest))return;string text=File.ReadAllText(manifest);string active=ActivePrefabPath(id),state=StateAssetPath(id);Restore(text,"originalActivePrefabExisted",Path.Combine(dir,"original.prefab"),active);Restore(text,"originalActivePrefabMetaExisted",Path.Combine(dir,"original.prefab.meta"),active+".meta");Restore(text,"originalStateExisted",Path.Combine(dir,"original.state.json"),state);Restore(text,"originalStateMetaExisted",Path.Combine(dir,"original.state.json.meta"),state+".meta");}
        static void Restore(string manifest,string key,string backup,string target){bool existed=manifest.IndexOf("\""+key+"\": true",StringComparison.Ordinal)>=0;if(existed){Directory.CreateDirectory(Path.GetDirectoryName(target));if(!File.Exists(backup))throw new InvalidOperationException("Rollback backup missing: "+backup);File.Copy(backup,target,true);}else if(File.Exists(target))File.Delete(target);}
        static void WriteBatchLock(string batch,List<ScreenId> ids){string path=BatchLockAbs(batch);Directory.CreateDirectory(Path.GetDirectoryName(path));var sb=new StringBuilder();for(int i=0;i<ids.Count;i++)sb.AppendLine(ids[i].ToString());File.WriteAllText(path,sb.ToString());}
        static string BatchLockAbs(string batch){return Step20ProductionAssetStep103Evidence.Absolute(RollbackRoot+"/"+batch+"_ACTIVE.lock");}
        static string SnapshotDirectory(ScreenId id){return Step20ProductionAssetStep103Evidence.Absolute(RollbackRoot+"/Pages/"+id);}
        static string ActivePrefabPath(ScreenId id){return Step20ProductionAssetStep103Evidence.Absolute(ActivePagesRoot+"/"+id+".prefab");}
        static string StateAssetPath(ScreenId id){string n=id==ScreenId.Shop?"ShopCutoverState":id+"CutoverState";return Step20ProductionAssetStep103Evidence.Absolute(StateRoot+"/"+n+".json");}
        static string ProjectRelative(string absolute){string root=Directory.GetCurrentDirectory().Replace('\\','/').TrimEnd('/');string p=absolute.Replace('\\','/');return p.StartsWith(root+"/",StringComparison.OrdinalIgnoreCase)?p.Substring(root.Length+1):p;}
        static string Sha256(string path){string p=Path.IsPathRooted(path)?path:Step20ProductionAssetStep103Evidence.Absolute(path);using(var sha=SHA256.Create())using(var s=File.OpenRead(p)){return BitConverter.ToString(sha.ComputeHash(s)).Replace("-",string.Empty).ToLowerInvariant();}}
        static string Bool(bool v){return v?"true":"false";}
        static void DeleteIfExists(string relative){string p=Step20ProductionAssetStep103Evidence.Absolute(relative);if(File.Exists(p))File.Delete(p);}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Step 11.2 route expansion cannot run in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before Step 11.2 route expansion/rollback.");}

        public readonly struct ExpansionStatus
        {
            public readonly int active,globalFailures;public readonly string batchA,batchB,batchC,batchD;
            public ExpansionStatus(int a,string ba,string bb,string bc,string bd,int gf){active=a;batchA=ba;batchB=bb;batchC=bc;batchD=bd;globalFailures=gf;}
        }
    }
}
#endif
