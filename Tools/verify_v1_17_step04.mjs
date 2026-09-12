import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";

const VERSION = "v1.17-step04";
const MILESTONE = "v1.17-step04-memory-texture-loading-optimization-foundation";
const UNITY_VERSION = "6000.5.9f1";
const IMPLEMENTATION_STATUS =
  "IMPLEMENTED_IN_SOURCE_PENDING_V1_17_STEP02_STEP03_PREREQUISITES_AND_STEP04_UNITY_DEVICE_VERIFICATION";
const CANDIDATE_STATUS =
  "MEMORY_TEXTURE_LOADING_OPTIMIZATION_FOUNDATION_SOURCE_CANDIDATE_PRODUCTION_GATES_BLOCKED";
const root = process.cwd();
const failures = [];
const warnings = [];
const reportPath = "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP04.json";
const changedManifestPath = "Assets/LegendsOfRus/Documentation/V1_17_STEP04_CHANGED_FILES.txt";

function fail(message) { if (!failures.includes(message)) failures.push(message); }
function warn(message) { if (!warnings.includes(message)) warnings.push(message); }
function full(rel, base=root) { return path.join(base, rel); }
function exists(rel, base=root) { return fs.existsSync(full(rel, base)); }
function text(rel, base=root) { return fs.readFileSync(full(rel, base), "utf8"); }
function bytes(rel, base=root) { return fs.readFileSync(full(rel, base)); }
function sha(data) { return crypto.createHash("sha256").update(data).digest("hex"); }
function fileSha(rel, base=root) { return sha(bytes(rel, base)); }
function json(rel, base=root) { try { return JSON.parse(text(rel, base)); } catch (e) { fail(`Invalid JSON ${rel}: ${e.message}`); return null; } }
function requireFile(rel) { if (!exists(rel)) fail(`Missing required file: ${rel}`); }
function requireContains(rel, fragments) {
  requireFile(rel); if (!exists(rel)) return;
  const s=text(rel); for (const f of fragments) if (!s.includes(f)) fail(`${rel} missing required marker: ${f}`);
}
function requireNotContains(rel, fragments) {
  requireFile(rel); if (!exists(rel)) return;
  const s=text(rel).toLowerCase(); for (const f of fragments) if (s.includes(f.toLowerCase())) fail(`${rel} contains forbidden marker: ${f}`);
}
function walk(base) {
  const out=[]; const ignored=new Set([".git","Library","Temp","Logs","obj","Build","Builds","UserSettings",".gradle"]);
  function visit(dir, rel="") {
    for (const ent of fs.readdirSync(dir,{withFileTypes:true}).sort((a,b)=>a.name.localeCompare(b.name))) {
      if (!rel && ignored.has(ent.name)) continue;
      const r=rel?path.posix.join(rel,ent.name):ent.name; const f=path.join(dir,ent.name);
      const st=fs.lstatSync(f); if (st.isSymbolicLink()) { fail(`Symbolic link not allowed: ${r}`); continue; }
      if (ent.isDirectory()) visit(f,r); else if (ent.isFile()) out.push(r);
    }
  }
  visit(base); return out.sort();
}
function fingerprint(base, files) {
  const h=crypto.createHash("sha256");
  for (const f of [...files].sort()) { h.update(f); h.update("\0"); h.update(fileSha(f,base)); h.update("\n"); }
  return h.digest("hex");
}
function option(name) { const i=process.argv.indexOf(name); return i>=0 ? process.argv[i+1] : null; }
const sourceOnly=process.argv.includes("--source-only");
const baselineArg=option("--baseline");
if (!sourceOnly && !baselineArg) fail("Use --baseline <exact-v1.17-step03-project> for release-boundary verification, or --source-only.");
if (sourceOnly && baselineArg) fail("Choose --source-only or --baseline, not both.");
const baseline=baselineArg?path.resolve(baselineArg):null;
if (baseline && !fs.existsSync(baseline)) fail(`Baseline directory does not exist: ${baseline}`);

const required = [
  "MILESTONE_VERSION.txt", "README_V1_17_STEP04.md", "Tools/verify_v1_17_step04.mjs",
  "Assets/LegendsOfRus/Documentation/V1_17_STEP04_MEMORY_TEXTURE_LOADING_OPTIMIZATION_FOUNDATION.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_17_STEP04.json",
  "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_17_STEP04_UNITY_VERIFICATION.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP05.json",
  "Assets/LegendsOfRus/Documentation/V1_17_STEP04_ACTIVATION_EVIDENCE.json",
  "Assets/LegendsOfRus/Documentation/V1_17_STEP04_SOURCE_TEXTURE_INVENTORY.json",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/LegendsOfRus.OptimizationCore.asmdef",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationPolicy.cs",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationEvidence.cs",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/LoadingStateTracker.cs",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationCoreAssemblyAccess.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs",
  "Assets/LegendsOfRus/Scripts/Editor/TextureOptimizationAudit.cs",
  "Assets/LegendsOfRus/Scripts/Editor/Step17MemoryTextureLoadingOptimizationSelfTest.cs",
  "Assets/LegendsOfRus/Tests/OptimizationEditMode/LegendsOfRus.OptimizationCore.Tests.asmdef",
  "Assets/LegendsOfRus/Tests/OptimizationEditMode/OptimizationCoreEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/OptimizationPlayMode/LegendsOfRus.OptimizationCore.PlayModeTests.asmdef",
  "Assets/LegendsOfRus/Tests/OptimizationPlayMode/RuntimeOptimizationPlayModeTests.cs"
];
required.forEach(requireFile);
if (exists("MILESTONE_VERSION.txt") && text("MILESTONE_VERSION.txt").trim()!==MILESTONE) fail("MILESTONE_VERSION.txt is not pinned to Step 4.");
if (exists("ProjectSettings/ProjectVersion.txt") && !text("ProjectSettings/ProjectVersion.txt").includes(UNITY_VERSION)) fail(`ProjectVersion is not ${UNITY_VERSION}.`);

// OptimizationCore must be engine-free and data-minimized.
const optAsm=json("Assets/LegendsOfRus/Scripts/OptimizationCore/LegendsOfRus.OptimizationCore.asmdef");
if (optAsm) {
  if (optAsm.name!=="LegendsOfRus.OptimizationCore") fail("OptimizationCore asmdef name changed.");
  if (optAsm.noEngineReferences!==true) fail("OptimizationCore must set noEngineReferences=true.");
  if ((optAsm.references||[]).length!==0) fail("OptimizationCore must not reference other assemblies.");
}
for (const rel of [
  "Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationPolicy.cs",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationEvidence.cs",
  "Assets/LegendsOfRus/Scripts/OptimizationCore/LoadingStateTracker.cs"
]) requireNotContains(rel,["UnityEngine","PlayerPrefs","System.IO","HttpClient","UnityWebRequest","Application.persistentDataPath","Socket","Addressables"]);

requireContains("Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationPolicy.cs",[
  "AllocatedWarningBytes = 384L * Mebibyte", "AllocatedCriticalBytes = 512L * Mebibyte",
  "ReservedWarningBytes = 640L * Mebibyte", "ReservedCriticalBytes = 768L * Mebibyte",
  "ManagedWarningBytes = 96L * Mebibyte", "ManagedCriticalBytes = 128L * Mebibyte",
  "MaximumSteadyGrowthBytes = 32L * Mebibyte", "ScreenRouteBudgetMilliseconds = 250.0",
  "ResourceCleanupBudgetMilliseconds = 750.0", "BootstrapBudgetMilliseconds = 5000.0",
  "DefaultEvidenceCapacity = 64", "MaximumEvidenceCapacity = 128",
  "RetiredScreensBeforeCleanup = 3", "MinimumCleanupIntervalSeconds = 10.0",
  "MemorySampleIntervalSeconds = 5.0", "RuntimeTextureMaximumDimension = 2048"
]);
requireContains("Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationEvidence.cs",["BoundedOptimizationEvidenceBuffer","ToDeterministicJson","MaximumEvidenceCapacity"]);
requireNotContains("Assets/LegendsOfRus/Scripts/OptimizationCore/OptimizationEvidence.cs",["playerId","deviceId","accountId","sessionId","timestamp","stackTrace","message\"","endpoint","authorization"]);

// Runtime wiring and lifetime controls.
const runtimeAsm=json("Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef");
if (runtimeAsm && !(runtimeAsm.references||[]).includes("LegendsOfRus.OptimizationCore")) fail("Runtime asmdef does not reference OptimizationCore.");
requireContains("Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs",[
  "Application.lowMemory += HandleLowMemory", "Application.lowMemory -= HandleLowMemory",
  "Profiler.GetTotalAllocatedMemoryLong()", "Profiler.GetTotalReservedMemoryLong()",
  "Resources.UnloadUnusedAssets()", "yield return null", "MinimumCleanupIntervalSeconds",
  "RetiredScreensBeforeCleanup", "DEVELOPMENT_BUILD"
]);
requireNotContains("Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs",[
  "PlayerPrefs", "System.IO", "File.", "Directory.", "UnityWebRequest", "HttpClient", "GC.Collect", "Addressables"
]);
requireContains("Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",[
  "RuntimeOptimizationService", "optimization.BeginBootstrap()", "optimization.MarkLocalStateReady()",
  "optimization.MarkUiConstructionReady()", "optimization.CompleteBootstrap()"
]);
requireContains("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",[
  "optimization?.BeginScreenRoute()", "optimization?.CompleteScreenRoute(",
  "id != ScreenId.Gameplay && id != ScreenId.Dialogue", "NotifyScreenRetired()"
]);

// Locked art importer: preserve format while eliminating CPU-readable copies.
requireContains("Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs",[
  "ti.isReadable = false", "ti.mipmapEnabled = false", "TextureImporterType.Sprite",
  "maxTextureSize = 2048", "TextureImporterFormat.ASTC_6x6"
]);
requireContains("Assets/LegendsOfRus/Scripts/Editor/TextureOptimizationAudit.cs",[
  "GetSourceTextureWidthAndHeight", "isReadable", "mipmapEnabled", "ASTC_6x6",
  "RuntimeTextureMaximumDimension"
]);

// Addressables/content delivery stays deferred.
const manifest=json("Packages/manifest.json");
if (manifest?.dependencies && Object.prototype.hasOwnProperty.call(manifest.dependencies,"com.unity.addressables")) fail("Addressables package must remain deferred in Step 4.");

// Production activation hard stops.
const economy=json("Assets/LegendsOfRus/Resources/Config/economy_runtime_activation.json");
if (economy && economy.runtimeActivationApproved!==false) fail("Economy runtime activation must remain false.");
const commercial=json("Assets/LegendsOfRus/Resources/Config/commercial_runtime_activation.json");
if (commercial) {
  for (const k of ["analyticsActivationApproved","iapActivationApproved","rewardedAdsActivationApproved","finalRuntimeActivationApproved"]) if (commercial[k]!==false) fail(`Commercial gate ${k} must remain false.`);
}
const commercialConfig=json("Assets/LegendsOfRus/Resources/Config/commercial_runtime_config.json");
if (commercialConfig) {
  for (const k of ["analyticsCaptureAllowed","analyticsUploadAllowed","iapAllowed","rewardedAdsAllowed","developmentSandboxAllowed"]) if (commercialConfig[k]!==false) fail(`Commercial config ${k} must remain false.`);
}

// Inventory is source fact, not device residency.
const inventory=json("Assets/LegendsOfRus/Documentation/V1_17_STEP04_SOURCE_TEXTURE_INVENTORY.json");
if (inventory) {
  const sum=inventory.summary||inventory;
  const total=sum.totalPngFiles ?? sum.pngFiles ?? sum.pngCount;
  const resources=sum.runtimeResourcesPngFiles ?? sum.resourcesPngFiles ?? sum.runtimeResourcesPngCount;
  const screens=sum.designScreenPngFiles ?? sum.designScreens ?? sum.designScreenPngCount;
  if (total!==65) fail(`Texture inventory expected 65 PNGs, found ${total}.`);
  if (resources!==47) fail(`Texture inventory expected 47 Resources PNGs, found ${resources}.`);
  if (screens!==22) fail(`Texture inventory expected 22 design screens, found ${screens}.`);
}
const currentPngs=walk(root).filter(f=>f.toLowerCase().endsWith(".png") && f.startsWith("Assets/LegendsOfRus/"));
if (currentPngs.length!==65) fail(`Project expected 65 approved PNGs, found ${currentPngs.length}.`);

// New tests are present, but source declaration count is informational only.
const newTestFiles=[
  "Assets/LegendsOfRus/Tests/OptimizationEditMode/OptimizationCoreEditModeTests.cs",
  "Assets/LegendsOfRus/Tests/OptimizationPlayMode/RuntimeOptimizationPlayModeTests.cs"
];
let declaredTests=0;
for (const f of newTestFiles) if (exists(f)) declaredTests+=(text(f).match(/\[(?:Test|UnityTest)\]/g)||[]).length;
if (declaredTests<10) fail(`Expected at least 10 Step 4 source test declarations, found ${declaredTests}.`);
requireContains("Assets/LegendsOfRus/Tests/OptimizationPlayMode/RuntimeOptimizationPlayModeTests.cs",[
  "ownedServiceHost", "Object.DestroyImmediate(ownedServiceHost)", "AssertSaveSentinels"
]);
requireNotContains("Assets/LegendsOfRus/Tests/OptimizationPlayMode/RuntimeOptimizationPlayModeTests.cs",["FindObjectsSortMode"]);

let baselineVerified=false;
let approvedPngsIdentical=null;
let inheritedTestsIdentical=null;
let protectedResourcesIdentical=null;
let packagesIdentical=null;
let projectSettingsIdentical=null;
let noInheritedFilesDeleted=null;
let actualChanged=[];
let baselineFiles=[];
let candidateFiles=[];

// Ensure the generated paths exist before calculating the final changed-path set.
if (!exists(changedManifestPath)) fs.writeFileSync(full(changedManifestPath),"PENDING\n");
if (!exists(reportPath)) fs.writeFileSync(full(reportPath),"{}\n");
candidateFiles=walk(root);

if (baseline && fs.existsSync(baseline)) {
  baselineFiles=walk(baseline);
  const baselineMilestone=exists("MILESTONE_VERSION.txt",baseline)?text("MILESTONE_VERSION.txt",baseline).trim():"";
  if (!baselineMilestone.startsWith("v1.17-step03")) fail(`Baseline is not v1.17 Step 3: ${baselineMilestone}`);
  noInheritedFilesDeleted=baselineFiles.every(f=>candidateFiles.includes(f));
  if (!noInheritedFilesDeleted) fail("One or more inherited Step 3 files were deleted.");
  actualChanged=candidateFiles.filter(f=>!baselineFiles.includes(f) || fileSha(f)!==fileSha(f,baseline));

  const baselinePngs=baselineFiles.filter(f=>f.toLowerCase().endsWith(".png") && f.startsWith("Assets/LegendsOfRus/"));
  approvedPngsIdentical=baselinePngs.length===currentPngs.length && baselinePngs.every(f=>currentPngs.includes(f)&&fileSha(f)===fileSha(f,baseline));
  if (!approvedPngsIdentical) fail("Approved PNG bytes differ from Step 3 baseline.");

  const baselineTests=baselineFiles.filter(f=>f.startsWith("Assets/LegendsOfRus/Tests/"));
  inheritedTestsIdentical=baselineTests.every(f=>exists(f)&&fileSha(f)===fileSha(f,baseline));
  if (!inheritedTestsIdentical) fail("One or more inherited tests differ from Step 3 baseline.");

  function prefixSame(prefix) {
    const files=baselineFiles.filter(f=>f===prefix||f.startsWith(prefix+"/"));
    return files.every(f=>exists(f)&&fileSha(f)===fileSha(f,baseline));
  }
  protectedResourcesIdentical=prefixSame("Assets/LegendsOfRus/Resources");
  packagesIdentical=prefixSame("Packages");
  projectSettingsIdentical=prefixSame("ProjectSettings");
  if (!protectedResourcesIdentical) fail("Resources tree changed; Step 4 may not mutate runtime configs or approved resource bytes.");
  if (!packagesIdentical) fail("Packages changed; Addressables/package changes are out of scope.");
  if (!projectSettingsIdentical) fail("ProjectSettings changed; Step 4 may not alter production/project settings.");

  for (const prefix of [
    "Assets/LegendsOfRus/Scripts/DiagnosticsCore", "Assets/LegendsOfRus/Scripts/SaveCore",
    "Assets/LegendsOfRus/Scripts/CommercialCore", "Assets/LegendsOfRus/Scripts/CommercialRuntime",
    "Assets/LegendsOfRus/Scripts/EconomyCore", "Assets/LegendsOfRus/Scripts/RuntimeEconomy",
    "Assets/LegendsOfRus/Editor/AndroidBuild"
  ]) if (!prefixSame(prefix)) fail(`Protected inherited boundary changed: ${prefix}`);

  const allowedExact=new Set([
    "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
    "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
    "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_17_STEP04.json",
    "Assets/LegendsOfRus/Documentation/NEXT_CHECKPOINT_V1_17_STEP04_UNITY_VERIFICATION.json",
    "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_17_STEP05.json",
    "Assets/LegendsOfRus/Documentation/README_FIRST.md",
    "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_17_STEP04.json",
    "Assets/LegendsOfRus/Documentation/V1_17_STEP04_ACTIVATION_EVIDENCE.json",
    "Assets/LegendsOfRus/Documentation/V1_17_STEP04_CHANGED_FILES.txt",
    "Assets/LegendsOfRus/Documentation/V1_17_STEP04_MEMORY_TEXTURE_LOADING_OPTIMIZATION_FOUNDATION.md",
    "Assets/LegendsOfRus/Documentation/V1_17_STEP04_SOURCE_TEXTURE_INVENTORY.json",
    "Assets/LegendsOfRus/Scripts/Editor/LockedDesignImporter.cs",
    "Assets/LegendsOfRus/Scripts/Editor/Step17MemoryTextureLoadingOptimizationSelfTest.cs",
    "Assets/LegendsOfRus/Scripts/Editor/TextureOptimizationAudit.cs",
    "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",
    "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef",
    "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimeOptimizationService.cs",
    "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
    "MILESTONE_VERSION.txt", "README.md", "README_V1_17_STEP04.md", "Tools/verify_v1_17_step04.mjs"
  ]);
  const allowedPrefixes=["Assets/LegendsOfRus/Scripts/OptimizationCore/","Assets/LegendsOfRus/Tests/OptimizationEditMode/","Assets/LegendsOfRus/Tests/OptimizationPlayMode/"];
  for (const f of actualChanged) if (!allowedExact.has(f)&&!allowedPrefixes.some(p=>f.startsWith(p))) fail(`Unexpected changed path vs Step 3: ${f}`);
  baselineVerified=true;
}

// Refresh changed manifest after baseline comparison.
if (baselineVerified) fs.writeFileSync(full(changedManifestPath),actualChanged.sort().join("\n")+"\n");
else fs.writeFileSync(full(changedManifestPath),"SOURCE_ONLY_VERIFICATION_NO_BASELINE_DELTA\n");
// Recalculate candidate files after generated manifest exists.
candidateFiles=walk(root);
if (baselineVerified) actualChanged=candidateFiles.filter(f=>!baselineFiles.includes(f)||fileSha(f)!==fileSha(f,baseline));

const report={
  version:VERSION, milestone:MILESTONE,
  status: failures.length?"FAIL_STATIC_SOURCE_VERIFICATION":"PASS_STATIC_SOURCE_AND_PROTECTED_BASELINE_VERIFICATION",
  passed:failures.length===0,
  implementationStatus:IMPLEMENTATION_STATUS,
  candidateStatus:CANDIDATE_STATUS,
  unityVersion:UNITY_VERSION,
  sourceOnlyVerification:sourceOnly,
  baselineVerified,
  unityExecutionEvidence:"NOT_RETAINED",
  unityCompileStatus:"NOT_RETAINED",
  projectValidatorExecutionStatus:"NOT_RETAINED",
  textureAuditExecutionStatus:"NOT_RETAINED",
  editorSelfTestExecutionStatus:"NOT_RETAINED",
  editModeExecutionStatus:"NOT_RETAINED",
  playModeExecutionStatus:"NOT_RETAINED",
  representativeDeviceMemoryLoadingStatus:"NOT_RETAINED",
  lowMemoryDeviceBehaviorStatus:"NOT_RETAINED",
  productionAndroidBuildAuthorized:false,
  addressablesActivated:false,
  productionCrashTransportAuthorized:false,
  economyActivationAuthorized:false,
  commercialActivationAuthorized:false,
  projectFiles:candidateFiles.length,
  csharpFiles:candidateFiles.filter(f=>f.endsWith(".cs")).length,
  step4SourceTestDeclarationsInformational:declaredTests,
  sourceCountsAreExecutionEvidence:false,
  approvedPngFiles:currentPngs.length,
  approvedPngsIdenticalToStep3:approvedPngsIdentical,
  inheritedTestsIdenticalToStep3:inheritedTestsIdentical,
  protectedResourcesIdenticalToStep3:protectedResourcesIdentical,
  packagesIdenticalToStep3:packagesIdentical,
  projectSettingsIdenticalToStep3:projectSettingsIdentical,
  noInheritedFilesDeleted,
  candidateTreeSha256:fingerprint(root,candidateFiles.filter(f=>f!==reportPath)),
  changedPathsVsStep3:baselineVerified?actualChanged.sort():[],
  failures, warnings
};
fs.writeFileSync(full(reportPath),JSON.stringify(report,null,2)+"\n");
console.log(JSON.stringify(report,null,2));
process.exit(failures.length?1:0);
