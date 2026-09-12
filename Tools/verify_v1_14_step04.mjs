import crypto from "node:crypto";
import fs from "node:fs";
import path from "node:path";
import { execFileSync } from "node:child_process";

const root = process.cwd();
const baselineFlag = process.argv.indexOf("--baseline");
const baseline = baselineFlag >= 0 && process.argv[baselineFlag + 1]
  ? path.resolve(process.argv[baselineFlag + 1])
  : null;
const sourceOnly = process.argv.includes("--source-only");
const failures = [];

function fail(message) {
  failures.push(message);
}

if (!baseline && !sourceOnly) {
  fail(
    "A v1.14 Step 3 baseline is required for release verification. " +
    "Pass --baseline <project> or explicitly choose --source-only."
  );
}

function projectFilesAt(directory) {
  try {
    const output = execFileSync("rg", ["--files"], {
      cwd: directory,
      encoding: "utf8"
    }).trim();
    return output ? output.split(/\r?\n/).sort() : [];
  } catch {
    return [];
  }
}

function files(glob) {
  try {
    const output = execFileSync("rg", ["--files", "-g", glob], {
      cwd: root,
      encoding: "utf8"
    }).trim();
    return output ? output.split(/\r?\n/).sort() : [];
  } catch {
    return [];
  }
}

function read(relative, directory = root) {
  return fs.readFileSync(path.join(directory, relative), "utf8");
}

function sha256(file) {
  return crypto.createHash("sha256").update(fs.readFileSync(file)).digest("hex");
}

function scrubCSharp(source) {
  let output = "";
  let state = "code";
  for (let index = 0; index < source.length; index++) {
    const char = source[index];
    const next = source[index + 1];
    if (state === "code") {
      if (char === "/" && next === "/") { state = "line"; output += "  "; index++; }
      else if (char === "/" && next === "*") { state = "block"; output += "  "; index++; }
      else if (char === "@" && next === '"') { state = "verbatim"; output += "  "; index++; }
      else if (char === '"') { state = "string"; output += " "; }
      else if (char === "'") { state = "char"; output += " "; }
      else output += char;
    } else if (state === "line") {
      if (char === "\n") { state = "code"; output += "\n"; } else output += " ";
    } else if (state === "block") {
      if (char === "*" && next === "/") { state = "code"; output += "  "; index++; }
      else output += char === "\n" ? "\n" : " ";
    } else if (state === "verbatim") {
      if (char === '"' && next === '"') { output += "  "; index++; }
      else if (char === '"') { state = "code"; output += " "; }
      else output += char === "\n" ? "\n" : " ";
    } else if (char === "\\") {
      output += "  ";
      index++;
    } else if ((state === "string" && char === '"') || (state === "char" && char === "'")) {
      state = "code";
      output += " ";
    } else {
      output += char === "\n" ? "\n" : " ";
    }
  }
  return output;
}

function methodBody(source, marker) {
  const start = source.indexOf(marker);
  if (start < 0) return "";
  const open = source.indexOf("{", start);
  if (open < 0) return "";
  let depth = 0;
  for (let index = open; index < source.length; index++) {
    if (source[index] === "{") depth++;
    if (source[index] === "}" && --depth === 0)
      return source.slice(open + 1, index);
  }
  return "";
}

const selfTestFile =
  "Assets/LegendsOfRus/Scripts/Editor/Step14AudioHapticsFidelityVerificationSelfTest.cs";
const step4TestFile =
  "Assets/LegendsOfRus/Tests/PlayMode/AudioHapticsFidelityVerificationPlayModeTests.cs";

const required = [
  "README_V1_14_STEP04.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP04.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_15_STEP01.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP04.json",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP04_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP04_AUDIO_HAPTICS_FIDELITY_VERIFICATION.md",
  "Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs",
  selfTestFile,
  "Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs",
  step4TestFile,
  "Tools/verify_v1_14_step04.mjs"
];

const expectedChanged = [
  ...required,
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  "Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs",
  "Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/SafeAreaFitter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
  "MILESTONE_VERSION.txt",
  "Packages/manifest.json",
  "README.md"
].sort();

for (const file of required)
  if (!fs.existsSync(path.join(root, file))) fail(`Required file ${file} is missing.`);

const jsonFiles = files("*.json");
for (const file of jsonFiles) {
  try {
    JSON.parse(read(file));
  } catch (error) {
    fail(`JSON ${file}: ${error.message}`);
  }
}

const story = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_story.json"));
const levels = JSON.parse(read("Assets/LegendsOfRus/Resources/Config/chapter01_levels.json"));
const russian = JSON.parse(read("Assets/LegendsOfRus/Resources/Localization/story_ru.json"));
if (story.schemaVersion !== 3 || story.scenes?.length !== 8 || story.milestones?.length !== 7)
  fail("Chapter 1 story schema, scenes, or milestones changed.");
if (story.reactions?.length !== 20 || story.completion?.finalLevelId !== 10)
  fail("Chapter 1 reactions or completion contract changed.");
if (levels.schemaVersion !== 3 || levels.levels?.length !== 10)
  fail("Chapter 1 level schema or authored level count changed.");
if (russian.schemaVersion !== 1 || russian.locale !== "ru" || russian.entries?.length !== 49)
  fail("Russian story localization contract changed.");

const packageManifest = JSON.parse(read("Packages/manifest.json"));
if (packageManifest.dependencies?.["com.unity.modules.audio"] !== "1.0.0")
  fail("Unity built-in audio module dependency is missing or changed.");
if (packageManifest.dependencies?.["com.unity.modules.androidjni"] !== "1.0.0")
  fail("Unity built-in Android JNI module dependency is missing or changed.");

const csharpFiles = files("*.cs");
for (const file of csharpFiles) {
  const source = read(file);
  const scrubbed = scrubCSharp(source);
  for (const [open, close] of [["{", "}"], ["(", ")"], ["[", "]"]]) {
    let depth = 0;
    for (const char of scrubbed) {
      if (char === open) depth++;
      else if (char === close) depth--;
      if (depth < 0) break;
    }
    if (depth !== 0) fail(`${file} has unbalanced ${open}${close} delimiters.`);
  }
  const directives = [];
  for (const line of source.split(/\r?\n/)) {
    if (/^\s*#if\b/.test(line)) directives.push("if");
    if (/^\s*#endif\b/.test(line)) {
      if (directives.length === 0) fail(`${file} has unmatched #endif.`);
      else directives.pop();
    }
  }
  if (directives.length > 0) fail(`${file} has unmatched #if.`);
}

const invariants = [
  ["Packages/manifest.json", /"com\.unity\.modules\.audio"\s*:\s*"1\.0\.0"/],
  ["Packages/manifest.json", /"com\.unity\.modules\.androidjni"\s*:\s*"1\.0\.0"/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", /FindOrAdd<MetaUiFeedbackService>/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", /AddComponent<ProductionFeedbackProvider>/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", /provider\.Initialize\(coreFeedback, metaFeedback, state\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs", /#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD[\s\S]*FindOrAdd<RuntimePerformanceProbe>/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /interface ICoreLoopSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /interface ICoreLoopHapticSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /class NullCoreLoopSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /class NullCoreLoopHapticSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /void ConfigureSinks/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /void ResetSinks/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /SfxEnabled/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs", /VibrationEnabled/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /readonly struct MetaUiFeedbackSignal/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /interface IMetaUiSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /interface IMetaUiHapticSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /class NullMetaUiSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /class NullMetaUiHapticSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /trace\.Phase == MetaUiTransitionPhase\.Entering/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /role == MetaUiControlRole\.Gameplay/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs", /void ConfigureSinks/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /class ProductionFeedbackProvider/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /ICoreLoopSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /IMetaUiSfxSink/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /SfxVoiceCapacity\s*=\s*4/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /new AudioSource\[SfxVoiceCapacity\]/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /void PrewarmHierarchy/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /void LoadResourcesOnce/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /Resources\.Load<AudioClip>/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /MissingClipCount/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /MissingClipRequestCount/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /CooldownDropCount/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /VoiceStealCount/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /OnApplicationPause/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /OnApplicationFocus/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /void OnEnable/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /void OnDisable/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /void RefreshAudioListenerState/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /gameState\.Save\.sfx/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /gameState\.Save\.music/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs", /save\.vibration/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /enum HapticFeedbackIntent/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /interface IHapticClock/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /interface IPlatformHapticBridge/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /MinimumIntervalSeconds\s*=\s*0\.06d/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /public bool IsSuspended/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /public void Resume\(\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /disposed\s*\|\|\s*suspended/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /Priority\(intent\) <= Priority\(lastAcceptedIntent\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /class NullPlatformHapticBridge/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /#if UNITY_ANDROID && !UNITY_EDITOR/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /class AndroidViewHapticBridge/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /AndroidApplication\.InvokeOnUIThread/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /performHapticFeedback/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /ConfirmEffect\s*=\s*16/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /RejectEffect\s*=\s*17/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs", /scheduledGeneration != generation/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /DesignWidth\s*=\s*941f/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /DesignHeight\s*=\s*1672f/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /DefaultTolerancePixels\s*=\s*1f/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /"16x9", "16:9", 1080, 1920/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /"18x9", "18:9", 1080, 2160/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /"19_5x9", "19\.5:9", 1080, 2340/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /"20x9", "20:9", 1080, 2400/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /Rect NormalizeSafeArea/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /Rect CalculateExpectedViewport/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs", /FidelityLayoutEvaluation Evaluate/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs", /static float ResolveMatch/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs", /static float ResolveScale/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs", /ScaleMode\.ConstantPixelSize/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/SafeAreaFitter.cs", /FidelityVerification\.NormalizeSafeArea/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/SafeAreaFitter.cs", /rect\.anchorMin = min/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /new WaitForEndOfFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /texture\.ReadPixels/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /ImageConversion\.EncodeToPNG/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /File\.WriteAllText\(metadataPath/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /FidelityVerification\.Evaluate/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /captureGeneration/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /CancelPendingCapture/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs", /requestedWidth != width/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /Run v1\.14 Step 4 Capture\/Layout Matrix/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /FidelityVerification\.TargetCount/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /RestoreResolution/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /originalRouteGeneration/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /CancelPendingCapture/],
  ["Assets/LegendsOfRus/Scripts/Editor/FidelityVerificationRunner.cs", /Pixel fidelity was not evaluated/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /EditorMeanLimit\s*=\s*0\.025f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /EditorP95Limit\s*=\s*0\.06f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /EditorBadFractionLimit\s*=\s*0\.01f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /AndroidMeanLimit\s*=\s*0\.04f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /AndroidP95Limit\s*=\s*0\.10f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /AndroidBadFractionLimit\s*=\s*0\.03f/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /metadata\.milestone/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /metadata\.pngFileName/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /metadata\.screenTransitioning \|\| metadata\.modalTransitioning/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /CalculateAspectFitCrop/],
  ["Assets/LegendsOfRus/Scripts/Editor/DesignReferenceComparator.cs", /metadata JSON sidecar is required/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /TargetFramesPerSecond\s*=\s*60/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /P95BudgetMilliseconds\s*=\s*18\.0/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /P99BudgetMilliseconds\s*=\s*25\.0/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /MaximumSevereFrameRatio\s*=\s*0\.01/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /MinimumGpuCoverageRatio\s*=\s*0\.90/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /MinimumPassingSamples\s*=\s*600/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /DefaultWarmupFrames\s*=\s*120/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /DefaultSampleFrames\s*=\s*1800/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /MaximumSamples\s*=\s*3600/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /RequiresZeroManagedAllocation/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /RuntimePerformanceVerdict\.Incomplete/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /ConfiguredTargetFrameRate/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /"configured_target_frame_rate"/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /ToDeterministicJson/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /new double\[capacity\]/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /new long\[capacity\]/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /new bool\[capacity\]/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /FrameTimingManager\.CaptureFrameTimings/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /ProfilerRecorder\.StartNew/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /gpu\.P95 <= RuntimePerformanceProfile\.P95BudgetMilliseconds/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /gpu\.P99 <= RuntimePerformanceProfile\.P99BudgetMilliseconds/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /public bool BeginAndPersist/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /ReportDirectoryName\s*=\s*"PerformanceQARuns\/v1_14_step04"/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /keyboard\.f6Key\.wasPressedThisFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /keyboard\.f7Key\.wasPressedThisFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /keyboard\.f8Key\.wasPressedThisFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /keyboard\.f9Key\.wasPressedThisFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /keyboard\.f10Key\.wasPressedThisFrame/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /File\.WriteAllText\(path, report\.ToDeterministicJson\(\)\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /void Awake\(\)[\s\S]*#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD[\s\S]*enabled = true[\s\S]*#else[\s\S]*enabled = false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs", /#if UNITY_EDITOR \|\| DEVELOPMENT_BUILD[\s\S]*#else[\s\S]*return false/],
  [selfTestFile, /Run v1\.14 Step 4 Self-Tests/],
  [step4TestFile, /ProductionFeedbackProvider_PrewarmsHierarchyOnceAndMissingClipsNoOp/],
  [step4TestFile, /Haptic/],
  [step4TestFile, /Fidelity/],
  [step4TestFile, /Performance/],
  ["MILESTONE_VERSION.txt", /^v1\.14-step04-audio-haptics-fidelity-verification\s*$/]
];

for (const [file, pattern] of invariants) {
  if (!fs.existsSync(path.join(root, file))) continue;
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
}

const providerSource = read("Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs");
const hapticSource = read("Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs");
const performanceSource = read("Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs");
const audioResourceRequests = providerSource.match(/LoadClip\("Audio\//g)?.length ?? 0;
if (audioResourceRequests !== 24)
  fail(`Expected 24 canonical audio resource requests, found ${audioResourceRequests}.`);
const bundledAudioFiles = projectFilesAt(root).filter((file) =>
  file.startsWith("Assets/LegendsOfRus/Resources/Audio/") && !file.endsWith(".meta")
);
if (bundledAudioFiles.length !== 0)
  fail(`Step 4 unexpectedly bundles ${bundledAudioFiles.length} audio content files.`);

const audioCueBody = methodBody(providerSource, "void PlayRecipe(");
if (/\b(?:Resources\.Load|AddComponent|Instantiate|new\s+(?:GameObject|AudioSource|StringBuilder)|File\.|Debug\.)/.test(audioCueBody))
  fail("Preloaded audio cue path performs loading, hierarchy creation, file I/O, logging, or managed construction.");
if (/Handheld\.Vibrate|android\.os\.Vibrator|Manifest\.permission\.VIBRATE/.test(hapticSource))
  fail("Haptic provider uses forbidden raw vibration APIs or permissions.");
if (/\b(?:QualitySettings|Time\.timeScale)\s*=/.test(performanceSource) ||
    /Application\.targetFrameRate\s*=/.test(performanceSource))
  fail("Performance probe mutates runtime quality, time, or target frame rate.");

const manifestPath =
  "Assets/LegendsOfRus/Documentation/V1_14_STEP04_CHANGED_FILES.txt";
const manifestLines = fs.existsSync(path.join(root, manifestPath))
  ? read(manifestPath).split(/\r?\n/).map((line) => line.trim()).filter((line) =>
      line && line !== "Added" && line !== "Modified"
    ).sort()
  : [];
if (manifestLines.length !== expectedChanged.length ||
    !expectedChanged.every((file, index) => file === manifestLines[index])) {
  fail(`Step 4 changed-file manifest does not match the ${expectedChanged.length}-path release contract.`);
}

const allowedServices = new Set([
  "Assets/LegendsOfRus/Scripts/Runtime/Services/CoreLoopFeedbackService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/FidelityVerification.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/MetaUiFeedbackService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/PlatformHapticProvider.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/ProductionFeedbackProvider.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/QACaptureService.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/Services/RuntimePerformanceProbe.cs"
]);
const allowedUi = new Set([
  "Assets/LegendsOfRus/Scripts/Runtime/UI/AdaptiveCanvasScaler.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/SafeAreaFitter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs"
]);

function isProtected(file) {
  if (file.startsWith("Assets/LegendsOfRus/Resources/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/GameplayCore/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Core/"))
    return file !== "Assets/LegendsOfRus/Scripts/Runtime/Core/RuntimeBootstrap.cs";
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Services/"))
    return !allowedServices.has(file);
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/UI/"))
    return !allowedUi.has(file);
  if (file.startsWith("Assets/Scenes/")) return true;
  if (file.startsWith("ProjectSettings/")) return true;
  if (file.startsWith("ProjectDocs/")) return true;
  if (file.startsWith("Packages/")) return file !== "Packages/manifest.json";
  if (file === "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef") return true;
  if (file.endsWith("/LegendsOfRus.GameplayCore.Tests.asmdef")) return true;
  if (file.endsWith("/LegendsOfRus.Runtime.PlayModeTests.asmdef")) return true;
  return false;
}

let pngFilesCompared = 0;
let pngFilesIdentical = null;
let protectedFilesCompared = 0;
let protectedFilesIdentical = null;
let changedManifestMatchesBaseline = null;
let packageManifestOnlyAddsRequiredModules = null;

if (baseline) {
  if (!fs.existsSync(baseline)) {
    fail(`Baseline directory does not exist: ${baseline}`);
  } else {
    const currentFiles = projectFilesAt(root);
    const baselineFiles = projectFilesAt(baseline);
    const currentPngs = currentFiles.filter((file) => file.endsWith(".png"));
    const baselinePngs = baselineFiles.filter((file) => file.endsWith(".png"));
    pngFilesCompared = currentPngs.length;
    pngFilesIdentical = currentPngs.length === baselinePngs.length &&
      currentPngs.every((file, index) => file === baselinePngs[index]);
    if (currentPngs.length !== 65)
      fail(`Expected 65 approved PNGs, found ${currentPngs.length}.`);
    if (!pngFilesIdentical)
      fail("PNG path list differs from the v1.14 Step 3 baseline.");
    for (const file of currentPngs) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        pngFilesIdentical = false;
        fail(`Approved PNG differs from v1.14 Step 3: ${file}`);
      }
    }

    const currentProtected = currentFiles.filter(isProtected);
    const baselineProtected = baselineFiles.filter(isProtected);
    protectedFilesCompared = currentProtected.length;
    protectedFilesIdentical = currentProtected.length === baselineProtected.length &&
      currentProtected.every((file, index) => file === baselineProtected[index]);
    if (!protectedFilesIdentical)
      fail("Protected boundary path list differs from the v1.14 Step 3 baseline.");
    for (const file of currentProtected) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        protectedFilesIdentical = false;
        fail(`Protected boundary file differs from v1.14 Step 3: ${file}`);
      }
    }

    const baselinePackage = JSON.parse(read("Packages/manifest.json", baseline));
    const currentWithoutStep4Modules = JSON.parse(JSON.stringify(packageManifest));
    delete currentWithoutStep4Modules.dependencies["com.unity.modules.audio"];
    delete currentWithoutStep4Modules.dependencies["com.unity.modules.androidjni"];
    packageManifestOnlyAddsRequiredModules =
      JSON.stringify(currentWithoutStep4Modules) === JSON.stringify(baselinePackage);
    if (!packageManifestOnlyAddsRequiredModules)
      fail("Packages/manifest.json changes more than the built-in audio and Android JNI modules.");

    const union = new Set([...currentFiles, ...baselineFiles]);
    const actualChanged = [...union].filter((file) => {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(current) || !fs.existsSync(original)) return true;
      return sha256(current) !== sha256(original);
    }).sort();
    changedManifestMatchesBaseline = actualChanged.length === manifestLines.length &&
      actualChanged.every((file, index) => file === manifestLines[index]);
    if (!changedManifestMatchesBaseline) {
      const missing = actualChanged.filter((file) => !manifestLines.includes(file));
      const extra = manifestLines.filter((file) => !actualChanged.includes(file));
      fail(`Changed-file manifest mismatch; missing=[${missing.join(",")}], extra=[${extra.join(",")}].`);
    }
  }
}

const testPattern = /\[(?:Unity)?Test(?:Case\([^\]]*\))?\]/g;
let discoveredTests = 0;
for (const file of csharpFiles.filter((file) => file.includes("/Tests/")))
  discoveredTests += read(file).match(testPattern)?.length ?? 0;
const targetedTests = fs.existsSync(path.join(root, step4TestFile))
  ? read(step4TestFile).match(testPattern)?.length ?? 0
  : 0;
const selfTestChecks = fs.existsSync(path.join(root, selfTestFile))
  ? (read(selfTestFile).match(/\bCheck\s*\(/g)?.length ?? 1) - 1
  : 0;

const result = {
  version: "v1.14-step04",
  all_project_json_parses: !failures.some((failure) => failure.startsWith("JSON ")),
  project_json_files: jsonFiles.length,
  story_schema_version: story.schemaVersion,
  persistent_story_scenes: story.scenes?.length ?? 0,
  post_victory_mappings: story.milestones?.length ?? 0,
  outcome_reactions: story.reactions?.length ?? 0,
  chapter_levels: levels.levels?.length ?? 0,
  russian_localization_entries: russian.entries?.length ?? 0,
  csharp_structural_scan_files: csharpFiles.length,
  csharp_delimiters_balanced: !failures.some((failure) => failure.includes("delimiters")),
  preprocessor_directives_balanced: !failures.some((failure) => failure.includes("#if") || failure.includes("#endif")),
  step04_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step04_targeted_tests: targetedTests,
  step04_editor_self_test_check_sites: selfTestChecks,
  release_contract_changed_paths: expectedChanged.length,
  canonical_audio_resource_requests: audioResourceRequests,
  bundled_audio_content_files: bundledAudioFiles.length,
  audio_module_declared: packageManifest.dependencies?.["com.unity.modules.audio"] === "1.0.0",
  android_jni_module_declared: packageManifest.dependencies?.["com.unity.modules.androidjni"] === "1.0.0",
  package_manifest_only_adds_required_modules: packageManifestOnlyAddsRequiredModules,
  project_png_files_compared: baseline ? pngFilesCompared : 0,
  project_png_files_byte_identical_to_v1_14_step03: baseline ? pngFilesIdentical : null,
  protected_boundary_files_compared: baseline ? protectedFilesCompared : 0,
  protected_boundary_files_byte_identical_to_v1_14_step03: baseline ? protectedFilesIdentical : null,
  changed_file_manifest_matches_v1_14_step03_baseline: changedManifestMatchesBaseline,
  required_v1_14_step04_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  production_feedback_adapter_contract_present: !failures.some((failure) => /ProductionFeedbackProvider/.test(failure)),
  missing_audio_content_is_safe_noop: bundledAudioFiles.length === 0 && !failures.some((failure) => /MissingClip/.test(failure)),
  null_provider_contracts_present: !failures.some((failure) => /Null(?:CoreLoop|MetaUi|Platform)/.test(failure)),
  android_haptic_contract_present: !failures.some((failure) => /PlatformHapticProvider/.test(failure)),
  four_target_fidelity_contract_present: !failures.some((failure) => /FidelityVerification/.test(failure)),
  performance_policy_and_release_stub_present: !failures.some((failure) => /RuntimePerformanceProbe/.test(failure)),
  preloaded_audio_cue_path_avoids_loading_hierarchy_and_file_work:
    !failures.some((failure) => /Preloaded audio cue path/.test(failure)),
  gameplay_balance_story_economy_save_scenes_project_settings_unchanged:
    baseline ? protectedFilesIdentical : null,
  unity_execution: "PENDING_UNITY_EDITOR",
  capture_layout_matrix: "NOT_EXECUTED_IN_THIS_ENVIRONMENT",
  pixel_fidelity_comparison: "NOT_EXECUTED_IN_THIS_ENVIRONMENT",
  android_haptic_verification: "NOT_EXECUTED_IN_THIS_ENVIRONMENT",
  representative_device_profiling: "NOT_EXECUTED_IN_THIS_ENVIRONMENT",
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
