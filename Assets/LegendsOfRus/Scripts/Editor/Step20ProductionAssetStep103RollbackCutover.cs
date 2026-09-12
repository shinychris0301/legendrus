#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace LegendsOfRus.Editor
{
    /// <summary>
    /// Explicit rollback-controlled development cutover for evidence-qualified
    /// non-core pages only. The accepted ten-route core registry is never edited.
    /// </summary>
    public static class Step20ProductionAssetStep103RollbackCutover
    {
        const string RollbackRoot = "ProjectDocs/V1_20_STEP10_3/CutoverRollback";
        const string AppliedManifest = RollbackRoot + "/STEP103_APPLIED_PAGES.json";
        const string ActivePagesRoot = "Assets/LegendsOfRus/Resources/ProductionUI/Pages";
        const string StateRoot = "Assets/LegendsOfRus/Resources/ProductionUI/CutoverState";

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 10.3/Apply Eligible Non-Core Development Cutovers")]
        public static void ApplyEligible()
        {
            EnsureEditorSafe();
            if (File.Exists(Step20ProductionAssetStep103Evidence.Absolute(AppliedManifest)))
                throw new InvalidOperationException("A Step 10.3 cutover batch is already active. Roll it back before applying another batch.");

            List<string> globalFailures;
            List<Step20ProductionAssetStep103Evidence.PageState> states = Step20ProductionAssetStep103Evidence.Collect(out globalFailures);
            if (globalFailures.Count > 0) throw new InvalidOperationException("Global authority gate failed:\n" + string.Join(Environment.NewLine, globalFailures.ToArray()));
            var eligible = new List<Step20ProductionAssetStep103Evidence.PageState>();
            for (int i=0;i<states.Count;i++)
            {
                if (IsSplashR3Owned(states[i].spec.id))
                {
                    Debug.Log("Step 10.3 leaves SplashTitle unchanged: its canonical prefab is owned by Splash R3.");
                    continue;
                }
                if(states[i].developmentCutoverEligible) eligible.Add(states[i]);
            }
            if (eligible.Count == 0) throw new InvalidOperationException("No non-core page currently satisfies source + runtime + direct-approved-reference + fidelity + authority gates.");

            string names = string.Empty; for(int i=0;i<eligible.Count;i++){if(i>0)names+=", ";names+=eligible[i].spec.id;}
            if (!EditorUtility.DisplayDialog("Legends of Rus", "Apply rollback-controlled development cutover for: " + names + "?\n\nThis does not change authoritative game logic. A page-by-page immutable rollback snapshot is created first.", "Apply", "Cancel")) return;

            var applied = new List<ScreenId>();
            try
            {
                Directory.CreateDirectory(Step20ProductionAssetStep103Evidence.Absolute(RollbackRoot));
                for(int i=0;i<eligible.Count;i++)
                {
                    SnapshotOnce(eligible[i]);
                    CopyCandidateToActive(eligible[i]);
                    WriteStateLast(eligible[i]);
                    applied.Add(eligible[i].spec.id);
                }
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                for(int i=0;i<eligible.Count;i++) ValidateActiveOrThrow(eligible[i].spec.id);
                WriteAppliedManifest(applied);
                Step20ProductionAssetStep103AndroidRegression.ClearAcceptanceLocks();
                AssetDatabase.Refresh();
                Debug.Log("v1.20 Step 10.3 rollback-controlled development cutover applied for " + applied.Count + " non-core page(s): " + names + ". Run functional regression and physical Android acceptance before final acceptance.");
            }
            catch
            {
                for(int i=applied.Count-1;i>=0;i--) RestorePage(applied[i]);
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                throw;
            }
        }

        [MenuItem("Legends of Rus/Production UI/v1.20 Step 10.3/Roll Back All Step 10.3 Development Cutovers")]
        public static void RollBackAll()
        {
            EnsureEditorSafe();
            string manifest = Step20ProductionAssetStep103Evidence.Absolute(AppliedManifest);
            if (!File.Exists(manifest)) throw new InvalidOperationException("No active Step 10.3 cutover batch manifest exists.");
            string text=File.ReadAllText(manifest); var pages=new List<ScreenId>();
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                ScreenId id=Step20ProductionAssetStep103Evidence.Pages[i].id;
                if(text.IndexOf("\""+id+"\"",StringComparison.Ordinal)>=0) pages.Add(id);
            }
            foreach (ScreenId page in pages)
                if (IsSplashR3Owned(page))
                    throw new InvalidOperationException("This legacy rollback includes SplashTitle, now owned by Splash R3. Use the explicit project backup instead; no files were changed.");
            if(!EditorUtility.DisplayDialog("Legends of Rus","Rollback all "+pages.Count+" Step 10.3 development cutover page(s) to their immutable pre-cutover state?","Rollback","Cancel")) return;
            for(int i=pages.Count-1;i>=0;i--) RestorePage(pages[i]);
            if(File.Exists(manifest))File.Delete(manifest);
            Step20ProductionAssetStep103AndroidRegression.ClearAcceptanceLocks();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log("v1.20 Step 10.3 rollback complete. Pre-cutover page states restored.");
        }

        public static List<string> ValidateActiveBatch()
        {
            var failures=new List<string>();
            string manifest=Step20ProductionAssetStep103Evidence.Absolute(AppliedManifest);
            if(!File.Exists(manifest)) return failures;
            string text=File.ReadAllText(manifest);
            for(int i=0;i<Step20ProductionAssetStep103Evidence.Pages.Length;i++)
            {
                var p=Step20ProductionAssetStep103Evidence.Pages[i];
                if(text.IndexOf("\""+p.id+"\"",StringComparison.Ordinal)<0) continue;
                if(ProductionCoreLoopPageRegistry.IsCutover(p.id)){failures.Add(p.id+": Step 10.3 batch must not mutate/own a core route.");continue;}
                if(!ProductionMetaPageCutoverRegistry.IsCutover(p.id)) failures.Add(p.id+": activation state is missing or invalid.");
                ValidateActivePrefab(p.id,failures);
            }
            return failures;
        }

        public static bool HasActiveBatch(){return File.Exists(Step20ProductionAssetStep103Evidence.Absolute(AppliedManifest));}

        static void SnapshotOnce(Step20ProductionAssetStep103Evidence.PageState state)
        {
            string dir=SnapshotDirectory(state.spec.id); Directory.CreateDirectory(dir);
            string manifest=Path.Combine(dir,"baseline.json");
            if(File.Exists(manifest)) throw new InvalidOperationException("Immutable Step 10.3 snapshot already exists for "+state.spec.id+". Refusing to overwrite it.");
            string active=ActivePrefabPath(state.spec.id), activeMeta=active+".meta", cutoverState=StateAssetPath(state.spec.id), cutoverMeta=cutoverState+".meta";
            bool prefabExists=File.Exists(active), prefabMetaExists=File.Exists(activeMeta), stateExists=File.Exists(cutoverState), stateMetaExists=File.Exists(cutoverMeta);
            if(prefabExists)File.Copy(active,Path.Combine(dir,"original.prefab"),false);
            if(prefabMetaExists)File.Copy(activeMeta,Path.Combine(dir,"original.prefab.meta"),false);
            if(stateExists)File.Copy(cutoverState,Path.Combine(dir,"original.state.json"),false);
            if(stateMetaExists)File.Copy(cutoverMeta,Path.Combine(dir,"original.state.json.meta"),false);
            string json="{\n  \"schemaVersion\": 1,\n  \"page\": \""+state.spec.id+"\",\n  \"originalActivePrefabExisted\": "+Bool(prefabExists)+",\n  \"originalActivePrefabMetaExisted\": "+Bool(prefabMetaExists)+",\n  \"originalStateExisted\": "+Bool(stateExists)+",\n  \"originalStateMetaExisted\": "+Bool(stateMetaExists)+",\n  \"originalActivePrefabSha256\": \""+(prefabExists?Sha256(active):string.Empty)+"\",\n  \"candidateSha256\": \""+Sha256(state.candidatePath)+"\"\n}\n";
            File.WriteAllText(manifest,json);
        }

        static void CopyCandidateToActive(Step20ProductionAssetStep103Evidence.PageState state)
        {
            if (IsSplashR3Owned(state.spec.id))
                throw new InvalidOperationException("Legacy candidate cannot overwrite the Splash R3 canonical prefab.");
            string active=ActivePrefabPath(state.spec.id); Directory.CreateDirectory(Path.GetDirectoryName(active));
            File.Copy(state.candidatePath,active,true);
            AssetDatabase.ImportAsset(ProjectRelative(active),ImportAssetOptions.ForceUpdate);
            var failures=new List<string>(); ValidateActivePrefab(state.spec.id,failures);
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
        }

        static void WriteStateLast(Step20ProductionAssetStep103Evidence.PageState state)
        {
            string path=StateAssetPath(state.spec.id); Directory.CreateDirectory(Path.GetDirectoryName(path));
            string json="{\n  \"schemaVersion\": 1,\n  \"magic\": \""+ProductionMetaPageCutoverRegistry.Step103StateMagicPrefix+state.spec.id+"\",\n  \"page\": \""+state.spec.id+"\",\n  \"enabled\": true,\n  \"sourceCandidateSha256\": \""+Sha256(state.candidatePath)+"\",\n  \"runtimeEvidence\": true,\n  \"directApprovedReference\": true,\n  \"runtimeFidelityPass\": true,\n  \"androidAcceptancePending\": true\n}\n";
            File.WriteAllText(path,json); AssetDatabase.ImportAsset(ProjectRelative(path),ImportAssetOptions.ForceUpdate);
        }

        static void ValidateActiveOrThrow(ScreenId id)
        {
            var failures=new List<string>();
            if(!ProductionMetaPageCutoverRegistry.IsCutover(id)) failures.Add(id+": ProductionMetaPageCutoverRegistry did not activate after state import.");
            ValidateActivePrefab(id,failures);
            if(failures.Count>0)throw new InvalidOperationException(string.Join(Environment.NewLine,failures.ToArray()));
        }

        static void ValidateActivePrefab(ScreenId id,List<string> failures)
        {
            string projectPath="Assets/LegendsOfRus/Resources/ProductionUI/Pages/"+id+".prefab";
            GameObject active=AssetDatabase.LoadAssetAtPath<GameObject>(projectPath);
            if(active==null){failures.Add(id+": active production prefab missing at "+projectPath);return;}
            ProductionCoreLoopPageMarker marker=active.GetComponent<ProductionCoreLoopPageMarker>();
            if(marker==null||marker.ScreenId!=id||!marker.ScreenshotFree) failures.Add(id+": screenshot-free production marker missing/mismatched.");
            if(!IsSplashR3Owned(id) && active.GetComponentsInChildren<Button>(true).Length!=0) failures.Add(id+": active page contains Button components; ScreenRouter must retain interaction authority.");
            string[] deps=AssetDatabase.GetDependencies(projectPath,true);
            for(int i=0;i<deps.Length;i++){string n=deps[i].Replace('\\','/');if(n.Contains("/Art/ReferenceOnly/")||n.Contains("/Resources/DesignScreens/")||n.Contains("/ProjectDocs/")) failures.Add(id+": forbidden reference/QA dependency: "+deps[i]);}
        }

        static bool IsSplashR3Owned(ScreenId id)
        {
            if (id != ScreenId.SplashTitle) return false;
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Step20SplashTitleR3PrefabRebuilder.PrefabPath);
            return prefab != null && prefab.GetComponent<SplashTitleView>() != null;
        }

        static void RestorePage(ScreenId id)
        {
            if (IsSplashR3Owned(id))
                throw new InvalidOperationException("Legacy rollback must not replace the Splash R3 canonical prefab.");
            string dir=SnapshotDirectory(id), manifest=Path.Combine(dir,"baseline.json");
            if(!File.Exists(manifest))return;
            string text=File.ReadAllText(manifest); string active=ActivePrefabPath(id), activeMeta=active+".meta", state=StateAssetPath(id), stateMeta=state+".meta";
            Restore(text,"originalActivePrefabExisted",Path.Combine(dir,"original.prefab"),active);
            Restore(text,"originalActivePrefabMetaExisted",Path.Combine(dir,"original.prefab.meta"),activeMeta);
            Restore(text,"originalStateExisted",Path.Combine(dir,"original.state.json"),state);
            Restore(text,"originalStateMetaExisted",Path.Combine(dir,"original.state.json.meta"),stateMeta);
        }

        static void Restore(string manifest,string key,string backup,string target)
        {
            bool existed=manifest.IndexOf("\""+key+"\": true",StringComparison.Ordinal)>=0;
            if(existed){Directory.CreateDirectory(Path.GetDirectoryName(target));if(!File.Exists(backup))throw new InvalidOperationException("Rollback backup missing: "+backup);File.Copy(backup,target,true);}else if(File.Exists(target))File.Delete(target);
        }

        static void WriteAppliedManifest(List<ScreenId> pages)
        {
            var sb=new System.Text.StringBuilder(); sb.Append("{\n  \"schemaVersion\": 1,\n  \"milestone\": \"v1.20-step10.3-rollback-controlled-development-cutover\",\n  \"pages\": [");
            for(int i=0;i<pages.Count;i++){if(i>0)sb.Append(',');sb.Append('"').Append(pages[i]).Append('"');}
            sb.Append("],\n  \"functionalRegressionPending\": true,\n  \"physicalAndroidRegressionPending\": true\n}\n");
            File.WriteAllText(Step20ProductionAssetStep103Evidence.Absolute(AppliedManifest),sb.ToString());
        }

        static string SnapshotDirectory(ScreenId id){return Step20ProductionAssetStep103Evidence.Absolute(RollbackRoot+"/"+id);}
        static string ActivePrefabPath(ScreenId id){return Step20ProductionAssetStep103Evidence.Absolute(ActivePagesRoot+"/"+id+".prefab");}
        static string StateAssetPath(ScreenId id){string name=id==ScreenId.Shop?"ShopCutoverState":id+"CutoverState";return Step20ProductionAssetStep103Evidence.Absolute(StateRoot+"/"+name+".json");}
        static string ProjectRelative(string absolute){string root=Directory.GetCurrentDirectory().Replace('\\','/').TrimEnd('/');string p=absolute.Replace('\\','/');return p.StartsWith(root+"/",StringComparison.OrdinalIgnoreCase)?p.Substring(root.Length+1):p;}
        static string Bool(bool value){return value?"true":"false";}
        static string Sha256(string projectOrAbsolutePath){string path=Path.IsPathRooted(projectOrAbsolutePath)?projectOrAbsolutePath:Step20ProductionAssetStep103Evidence.Absolute(projectOrAbsolutePath);using(var sha=SHA256.Create())using(var stream=File.OpenRead(path)){byte[] hash=sha.ComputeHash(stream);return BitConverter.ToString(hash).Replace("-",string.Empty).ToLowerInvariant();}}
        static void EnsureEditorSafe(){if(Application.isBatchMode)throw new InvalidOperationException("Step 10.3 cutover cannot run in batch mode.");if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Exit Play Mode before applying/rolling back Step 10.3 cutovers.");}
    }
}
#endif
