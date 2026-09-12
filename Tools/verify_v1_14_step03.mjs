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
    "A v1.14 Step 2 baseline is required for release verification. " +
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

const required = [
  "README_V1_14_STEP03.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS_V1_14_STEP03.json",
  "Assets/LegendsOfRus/Documentation/NEXT_MILESTONE_V1_14_STEP04.json",
  "Assets/LegendsOfRus/Documentation/STATIC_VERIFICATION_V1_14_STEP03.json",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP03_CHANGED_FILES.txt",
  "Assets/LegendsOfRus/Documentation/V1_14_STEP03_META_UI_MOTION_TRANSITION_POLISH.md",
  "Assets/LegendsOfRus/Scripts/Editor/Step14MetaUiMotionTransitionPolishSelfTest.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs",
  "Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs",
  "Tools/verify_v1_14_step03.mjs"
];

const expectedChanged = [
  ...required,
  "Assets/LegendsOfRus/Documentation/DEVELOPMENT_PLAN_V2.md",
  "Assets/LegendsOfRus/Documentation/IMPLEMENTATION_STATUS.json",
  "Assets/LegendsOfRus/Documentation/README_FIRST.md",
  "Assets/LegendsOfRus/Scripts/Editor/ProjectValidator.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
  "MILESTONE_VERSION.txt",
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
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /enum MetaUiMotionCue/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /ScreenEnter/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /ScreenExit/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /ModalEnter/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /ModalExit/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /ButtonPress/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /MapNodePulse/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /VictoryReveal/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /FailureReveal/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /RewardReveal/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /readonly struct MetaUiTransitionStyle/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /readonly struct MetaUiPressStyle/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /readonly struct MetaUiMotionTrace/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs", /readonly struct MetaUiMotionDiagnostics/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /MaximumRegisteredMotions\s*=\s*128/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /MaximumTickSeconds\s*=\s*0\.05f/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /ResolveScreenPreset/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /ResolveModalPreset/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /ResolvePress/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs", /EaseOutCubic/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /class MetaUiMotionPresenter/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /new IMetaUiMotionTickable\[MetaUiMotionProfile\.MaximumRegisteredMotions\]/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /Time\.unscaledDeltaTime/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /TickForTesting/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /void ClearAll/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /class MetaUiActionGate/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /class MetaUiTransition/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /class MetaUiMapNodeFeedback/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /raycastTarget\s*=\s*false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs", /blocksRaycasts\s*=\s*false/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /IPointerDownHandler/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /IPointerUpHandler/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /IPointerExitHandler/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /IPointerClickHandler/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /clickEligiblePointerId/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /DispatchExactlyOnce/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /void OnDisable/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /void OnEnable/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs", /MetaUiControlRole\.OneShotReward/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /IMetaUiTransitionObserver/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /MetaUiActionGate modalGate/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /blocker\.raycastTarget\s*=\s*true/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /RequestCloseModal\(Action afterClose\)/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /modalContinuationGeneration/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /RetireContentRoot/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /MetaUiControlRole\.MapNode/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /MetaUiMotionPreset\.OutcomeVictory/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /MetaUiMotionPreset\.OutcomeFailure/],
  ["Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs", /MetaUiMotionPreset\.ChapterComplete/],
  ["Assets/LegendsOfRus/Scripts/Editor/Step14MetaUiMotionTransitionPolishSelfTest.cs", /Run v1\.14 Step 3 Self-Tests/],
  ["Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs", /Hotspot_PressExitRestoresBaselineWithoutInvoking/],
  ["Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs", /ActionGate_RejectsSiblingUntilAcceptedActionDispatches/],
  ["Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs", /PublicRouterFlow_WiresMapMotionAndAtomicModalBlocking/],
  ["Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs", /MapNodePulse_IsPrewarmedNonRaycastAndDeterministic/],
  ["Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs", /PresentationOnlySequence_DoesNotMutateSaveOrPublishStateChange/],
  ["MILESTONE_VERSION.txt", /^v1\.14-step03-meta-ui-motion-transition-polish\s*$/]
];

for (const [file, pattern] of invariants) {
  if (!fs.existsSync(path.join(root, file))) continue;
  if (!pattern.test(read(file))) fail(`${file} lacks invariant ${pattern}.`);
}

const metaFiles = [
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs",
  "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs"
];
const metaSource = metaFiles.map((file) => read(file)).join("\n");
for (const forbidden of [
  "GameState", "StoryService", "MonetizationService", "Match3Board", "PlayerPrefs",
  "AudioSource", "AudioClip", "AndroidJavaClass", "AndroidJavaObject", "Animator",
  "Animation", "DOTween", "LeanTween", "UnityEngine.Random"
]) {
  if (new RegExp(`\\b${forbidden.replace(".", "\\.")}\\b`).test(metaSource))
    fail(`Meta UI presentation contains forbidden dependency ${forbidden}.`);
}
if (/\b(?:StartCoroutine|WaitForSeconds|WaitForSecondsRealtime|Instantiate)\b/.test(metaSource))
  fail("Meta UI presentation must use its single fixed clock, not coroutines or Instantiate.");

const presenterSource = read("Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs");
const hotspotSource = read("Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs");
const routerSource = read("Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs");
const metaUpdateCount = metaSource.match(/\bvoid\s+Update\s*\(/g)?.length ?? 0;
if (metaUpdateCount !== 1)
  fail(`Expected exactly one Meta UI Update clock, found ${metaUpdateCount}.`);
for (const body of [
  methodBody(presenterSource, "public void TickMotion(float deltaTime)"),
  methodBody(presenterSource, "public void Pulse(PointerEventData eventData)"),
  methodBody(hotspotSource, "public void TickMotion(float deltaTime)")
]) {
  if (/new\s+GameObject\s*\(|\bInstantiate\s*\(/.test(body))
    fail("A Meta UI play/tick path allocates a runtime GameObject.");
}
if (/InvokeAfterFeedback|StartCoroutine\s*\(\s*InvokeAfterFeedback|WaitForSecondsRealtime/.test(hotspotSource))
  fail("Legacy coroutine hotspot path remains.");
if (/StartCoroutine\s*\(\s*FadeIn|IEnumerator\s+FadeIn/.test(routerSource))
  fail("Legacy screen FadeIn coroutine remains.");

const manifestLines = read("Assets/LegendsOfRus/Documentation/V1_14_STEP03_CHANGED_FILES.txt")
  .split(/\r?\n/).map((line) => line.trim()).filter((line) =>
    line && line !== "Added" && line !== "Modified"
  ).sort();
if (manifestLines.length !== expectedChanged.length ||
    !expectedChanged.every((file, index) => file === manifestLines[index])) {
  fail("Step 3 changed-file manifest does not match the 20-path release contract.");
}

let pngFilesCompared = 0;
let pngFilesIdentical = null;
let protectedFilesCompared = 0;
let protectedFilesIdentical = null;
let changedManifestMatchesBaseline = null;

function isProtected(file) {
  if (file.startsWith("Assets/LegendsOfRus/Resources/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/GameplayCore/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Gameplay/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Services/")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/Core/")) return true;
  if (file.startsWith("Assets/Scenes/")) return true;
  if (file.startsWith("Packages/")) return true;
  if (file.startsWith("ProjectSettings/")) return true;
  if (file === "ProjectDocs/ARTIFACT_MANIFEST.csv") return true;
  if (file === "Assets/LegendsOfRus/Scripts/Runtime/LegendsOfRus.Runtime.asmdef") return true;
  if (file.endsWith("/LegendsOfRus.GameplayCore.Tests.asmdef")) return true;
  if (file.endsWith("/LegendsOfRus.Runtime.PlayModeTests.asmdef")) return true;
  if (file.startsWith("Assets/LegendsOfRus/Scripts/Runtime/UI/")) {
    return !new Set([
      "Assets/LegendsOfRus/Scripts/Runtime/UI/AnimatedHotspot.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/UI/ScreenRouter.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionCue.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionProfile.cs",
      "Assets/LegendsOfRus/Scripts/Runtime/UI/MetaUiMotionPresenter.cs"
    ]).has(file);
  }
  return false;
}

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
      fail("PNG path list differs from the v1.14 Step 2 baseline.");
    for (const file of currentPngs) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        pngFilesIdentical = false;
        fail(`Approved PNG differs from v1.14 Step 2: ${file}`);
      }
    }

    const currentProtected = currentFiles.filter(isProtected);
    const baselineProtected = baselineFiles.filter(isProtected);
    protectedFilesCompared = currentProtected.length;
    protectedFilesIdentical = currentProtected.length === baselineProtected.length &&
      currentProtected.every((file, index) => file === baselineProtected[index]);
    if (!protectedFilesIdentical)
      fail("Protected boundary path list differs from the v1.14 Step 2 baseline.");
    for (const file of currentProtected) {
      const current = path.join(root, file);
      const original = path.join(baseline, file);
      if (!fs.existsSync(original) || sha256(current) !== sha256(original)) {
        protectedFilesIdentical = false;
        fail(`Protected boundary file differs from v1.14 Step 2: ${file}`);
      }
    }

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
const step3TestFile = "Assets/LegendsOfRus/Tests/PlayMode/MetaUiMotionTransitionPolishPlayModeTests.cs";
const targetedTests = fs.existsSync(path.join(root, step3TestFile))
  ? read(step3TestFile).match(testPattern)?.length ?? 0
  : 0;
const selfTestFile = "Assets/LegendsOfRus/Scripts/Editor/Step14MetaUiMotionTransitionPolishSelfTest.cs";
const selfTestChecks = fs.existsSync(path.join(root, selfTestFile))
  ? (read(selfTestFile).match(/\bCheck\s*\(/g)?.length ?? 1) - 1
  : 0;

const result = {
  version: "v1.14-step03",
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
  step03_source_invariants_checked: invariants.length,
  discovered_editmode_and_playmode_tests: discoveredTests,
  new_step03_targeted_tests: targetedTests,
  step03_editor_self_test_check_sites: selfTestChecks,
  project_png_files_compared: baseline ? pngFilesCompared : 0,
  project_png_files_byte_identical_to_v1_14_step02: baseline ? pngFilesIdentical : null,
  protected_boundary_files_compared: baseline ? protectedFilesCompared : 0,
  protected_boundary_files_byte_identical_to_v1_14_step02: baseline ? protectedFilesIdentical : null,
  changed_file_manifest_matches_v1_14_step02_baseline: changedManifestMatchesBaseline,
  required_v1_14_step03_files_present: required.every((file) => fs.existsSync(path.join(root, file))),
  one_bounded_unscaled_meta_motion_clock: metaUpdateCount === 1 &&
    !failures.some((failure) => /single fixed clock|coroutines/i.test(failure)),
  meta_motion_has_no_per_tick_gameobject_allocation: !failures.some((failure) => /play\/tick path allocates/i.test(failure)),
  meta_flourishes_are_non_raycast: !failures.some((failure) => /raycast|blocksRaycasts/.test(failure)),
  hotspot_pointer_and_exactly_once_contract_present: !failures.some((failure) => /AnimatedHotspot/.test(failure)),
  modal_blocker_and_generation_contract_present: !failures.some((failure) => /ScreenRouter/.test(failure)),
  presentation_isolated_from_gameplay_economy_story_save: !failures.some((failure) => /forbidden dependency/.test(failure)),
  feedback_service_and_gameplay_boundaries_unchanged: baseline ? protectedFilesIdentical : null,
  failures
};

process.stdout.write(JSON.stringify(result, null, 2) + "\n");
if (failures.length > 0) process.exitCode = 1;
